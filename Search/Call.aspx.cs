using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Search_Call : System.Web.UI.Page
{
    protected void Page_Preload(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GridView1.Style.Add("table-layout", "fixed");
            GridView1.Columns[0].HeaderStyle.Width = 120;
            GridView1.Columns[1].HeaderStyle.Width = 180;
        }
    }  
    
    protected void Page_PreRenderComplete(object sender, EventArgs e)   //放Page_Load無法顯示
    {
        if (ViewState["SQL"] != null) SqlDataSourceSearch.SelectCommand = ViewState["SQL"].ToString();

        if (!IsPostBack)
        {
            //產生各種年月日下拉式選單  
            string DbYYYY = GetValue("select [Config] from [Config] where [Kind]='統計參數' and [Item]='起始年份'");
            AddSel(SelYYYY, int.Parse(DbYYYY), DateTime.Now.Year + 1); 
            AddSel(SelMM, 1, 12);

            //預設值
            string YYYY = DateTime.Now.AddMonths(-1).Year.ToString();
            string MM = DateTime.Now.AddMonths(-1).Month.ToString(); if (MM.Length < 2) MM = "0" + MM;
            for (int i = 0; i < SelYYYY.Items.Count; i++) if (SelYYYY.Items[i].Value == YYYY) SelYYYY.SelectedIndex = i;
            for (int i = 0; i < SelMM.Items.Count; i++) if (SelMM.Items[i].Value == MM) SelMM.SelectedIndex = i;

            for (int i = 0; i < SelSys.Items.Count; i++) if (SelSys.Items[i].Value.IndexOf("服務系統")>=0) SelSys.SelectedIndex = i;

            BtnSearch_Click(null, null);
        }
    }

    protected void AddSel(DropDownList Sel, int Idx1, int Idx2)   //產生下拉式班別選單
    {
        //Sel.Items.Add("");
        for (int i = Idx1; i <= Idx2; i++)
        {
            ListItem Item = new ListItem();

            if (i < 10)
            {
                Item.Text = "0" + i.ToString();
                Item.Value = "0" + i.ToString();
            }
            else
            {
                Item.Text = i.ToString();
                Item.Value = i.ToString();
            }

            Sel.Items.Add(Item);
        }
    }

    protected void BtnSearch_Click(object sender, EventArgs e)
    {
        string YYYYMM = SelYYYY.SelectedValue + SelMM.SelectedValue;

        if (SelSys.SelectedValue != "" | YYYYMM != "")
        {
            string SQL = "SELECT distinct [日誌班別],[日誌編號] FROM [View_工作日誌] WHERE not ([員工代號]<>'" + Request.Cookies["UserID"].Value.ToString() + "' and [處理過程] like '%@@%')";

            //---------------------------------------------------------系統
            if (SelSys.SelectedValue != "") SQL = SQL + " and ([系統代碼] in (select [Config] from [Config] where [Memo]='" + SelSys.SelectedValue + "')" 
                + " or Substring([訊息代碼],2,3) in (select [Config] from [Config] where [Memo]='" + SelSys.SelectedValue + "'))";

            //---------------------------------------------------------日期
            if (YYYYMM != "______") SQL = SQL + " and ([日誌班別] like '" + YYYYMM + "%' or [處理時間] like '" + SelYYYY.SelectedValue + "/" + SelMM.SelectedValue + "%')";

            //---------------------------------------------------------時段
            switch (SelCall.SelectedValue)
            {
                case "全部": break;
                case "有叫修":SQL = SQL + " and [叫修時段] is not null"; break;
                case "非上班":SQL = SQL + " and [叫修時段]<>'上班'"; break;
                default:SQL = SQL + " and [叫修時段]='" + SelCall.SelectedValue + "'"; break;
            }

            //---------------------------------------------------------另開視窗

            
            //(CASE WHEN PUR_AMT IS NULL THEN 0 ELSE PUR_AMT END) PUR_AMT 
            //---------------------------------------------------------查詢顯示
            SQL = "select A.*,A.[日誌班別] + ' ' + A.[日誌時間] + ' ' + A.[日誌存檔] + ' ' + A.[當班OP] as [班別],"
                + " (SELECT '<font color=\"darkblue\"><span title=\"訊息時間：' + [訊息時間] + '\n存檔時間：' + [訊息存檔] + '\">' + SUBSTRING([訊息時間],6,11) + '</span>'" 
                + " + ' (<font color=\"blue\" style=\"cursor:hand\" onclick=window.open(\"/sop/sop.asp?SEL_Code=' + [訊息代碼] + '\",\"_blank\")>' + [訊息代碼] + '</font>) ' + [發生訊息] + '</font><br />'" 
                + " FROM [View_發生訊息] WHERE A.[日誌班別] = [View_發生訊息].[日誌班別] AND A.[日誌編號] = [View_發生訊息].[日誌編號] order by [訊息時間] FOR XML PATH(''))"
                + " + STUFF((SELECT '<br /><font color=\"green\" title=\"處理時間：' + [處理時間] + '\n存檔時間：' + [處理存檔] + '\n'"
                + " + '叫修資訊：' + CASE WHEN [叫修人員] IS NULL THEN '' ELSE [叫修人員] END + ' ' + CASE WHEN [叫修時段] IS NULL THEN '' ELSE [叫修時段] END + '\n叫修存檔：' + CASE WHEN [叫修存檔] IS NULL THEN '' ELSE [叫修存檔] END + '\n'" 
                + " + '記錄人員：' + [單位名稱] + '(' + [單位代號] + ')\\' + [員工姓名] + '(' + [員工代號] + ')'" 
                + " + '\">' + SUBSTRING([處理時間],6,11) + '</font> ' + [處理過程] FROM [View_處理過程]"
                + " WHERE A.[日誌班別] = [View_處理過程].[日誌班別] AND A.[日誌編號] = [View_處理過程].[日誌編號] and [處理過程] not like '%@@%' order by [處理時間] FOR XML PATH('')),1,12,'') AS [工作日誌],"
                + "STUFF((select '<br />' + SUBSTRING([處理時間],6,11) + ' <b title=\"設定時間：' + [叫修存檔] + '\n設定人員：' + [叫修OP] + '\n叫修時段：' + [叫修時段] + '\"><font size=\"4\">' + [叫修人員] + '</font></b> (' +[當班OP]+')'" 
                + " from [View_處理過程] WHERE A.[日誌班別] = [View_處理過程].[日誌班別] AND A.[日誌編號] = [View_處理過程].[日誌編號] and [叫修時段] is not null order by [處理時間] FOR XML PATH('')),1,12,'') AS [叫修資訊]"
                + " from [View_日誌記錄] AS A,(" + SQL + ") AS B"                
                + " where A.[日誌班別]=B.[日誌班別] and A.[日誌編號]=B.[日誌編號]";

            if (!ChkPage.Checked)
            {
                SqlDataSourceSearch.SelectCommand = SQL;
                GridView1.Sort("[日誌班別]", SortDirection.Ascending);
                GridView1.DataBind();
                //Response.Write(SQL);
                ViewState["SQL"] = SQL;
            }
            else
            {
                Session["SQL"] = SQL;
                OpenExecWindow("window.open('CallPrint.aspx?YYYY=" + SelYYYY.SelectedValue + "&MM=" + SelMM.SelectedValue + "&Sys=" + SelSys.SelectedItem.Text + "','_blank');");
            }
        }
        else AddMsg("<script>alert('請設定查詢條件 !');</script>");
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)    //將SopDT存於SopCode.ToolTip
    {
        GridViewRow item = e.Row;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            item.Cells[0].Text = FormatTourOP(item.Cells[0].Text);
            item.Cells[1].Text = Server.HtmlDecode(item.Cells[1].Text);
            item.Cells[2].Text = Server.HtmlDecode(item.Cells[2].Text);
        }
    }

    protected string FormatTourOP(string str)    //格式化班別及當班OP
    {
        string tour=str.Substring(0,9),strTour="",OP="",c="",tourW="";
        strTour = "<span title=\"日誌時間：" + str.Substring(10, 16) + "\n" + "存檔時間：" + str.Substring(27, 16) + "\">" + tour + "</span>";

        OP = str.Substring(44).Trim();

	    switch(tour.Substring(8,1))
        {
            case "1"  : c="<font color=\"blue\">早</font>"; break;
	        case "2"  : c="<font color=\"black\">午</font>"; break;
	        case "3"  : c="<font color=\"red\">晚</font>"; break;
        }

        string week = DateTime.Parse(tour.Substring(0, 4) + "/" + tour.Substring(4, 2) + "/" + tour.Substring(6, 2)).DayOfWeek.ToString();
        switch (week)
        {
            case "Sunday": tourW = "<font color=\"red\">(日)</font>"; break;
            case "Monday": tourW = "(一)"; break;
            case "Tuesday": tourW = "(二)"; break;
            case "Wednesday": tourW = "(三)"; break;
            case "Thursday": tourW = "(四)"; break;
            case "Friday": tourW = "(五)"; break;
            case "Saturday": tourW = "<font color=\"red\">(六)</font>"; break;
            default: tourW = "(" + week + ")"; break;
        }

        return("<font size=\"2\"><font color=\"blue\" onClick=\"TourClick(" + tour + ");\" style=\"cursor:pointer\"><u>" + strTour + "</u></font>"
            + " " + c + tourW + "</font>\n<br />" + OP);
    }

    protected string GetValue(string SQL)   //取得單一資料
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand(SQL, Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        string cfg = ""; if (dr.Read()) cfg = dr[0].ToString();
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        return (cfg);
    }

    protected void AddMsg(string strMsg)
    {
        Literal Msg = new Literal();
        Msg.Text = strMsg;
        Page.Controls.Add(Msg);
    }

    protected void OpenExecWindow(string strJavascript)    //選取後就另開視窗
    {
        if (ScriptManager.GetCurrent(Page) == null)
        {
            Page.ClientScript.RegisterStartupScript(Page.GetType(), "Config", strJavascript, true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Config", strJavascript, true);
        }
    }
}