using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Search_Search : System.Web.UI.Page
{
    protected void Page_Preload(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GridView1.Style.Add("table-layout", "fixed");
            GridView1.Columns[0].HeaderStyle.Width = 80;
            GridView1.Columns[2].HeaderStyle.Width = 50;
            GridView1.Columns[3].HeaderStyle.Width = 50;
            GridView1.Columns[4].HeaderStyle.Width = 50;
        }
    } 
    
    protected void Page_PreRenderComplete(object sender, EventArgs e)   //放Page_Load無法顯示
    {
        if (ViewState["SQL"] != null) SqlDataSourceSearch.SelectCommand = ViewState["SQL"].ToString();

        if (!IsPostBack)
        {
            if (ViewState["SQL"] != null) BtnSearch_Click(null, null);
        }
    }

    protected void BtnSearch_Click(object sender, EventArgs e)
    {
        if (txtKey.Text != "" | SelSysCode.SelectedValue != "")
        {
            string SQL = "SELECT distinct [日誌班別],[日誌編號] FROM [View_工作日誌] WHERE not ([員工代號]<>'" + Request.Cookies["UserID"].Value.ToString() + "' and [處理過程] like '%@@%')";

            //---------------------------------------------------------關鍵字
            String[] keyA = txtKey.Text.Trim().Split(' ');
            if (ChkMsg.Checked) { foreach (string key in keyA) if (key.Trim() != "") SQL = SQL + " and [發生訊息] like '%" + key + "%'"; }
            else { foreach (string key in keyA) if (key.Trim() != "") SQL = SQL + " and [處理過程] like '%" + key + "%'"; }

            //---------------------------------------------------------日期
            if (SelDate.SelectedValue != "") SQL = SQL + " and [日誌班別]>='" + DateTime.Now.AddDays(-365*(int.Parse(SelDate.SelectedValue))).ToString("yyyyMMdd") + "3'";

            //---------------------------------------------------------系統
            if (SelSysCode.SelectedValue != "") SQL = SQL + " and ([系統代碼]='" + SelSysCode.SelectedValue + "' or [訊息代碼] like '_" + SelSysCode.SelectedValue + "%')";

            //---------------------------------------------------------限公告
            if (ChkKnow.Checked) SQL = SQL + " and ([狀態代碼]=1 or [狀態代碼]=3)";

            //---------------------------------------------------------分頁
            if (ChkPage.Checked) GridView1.AllowPaging = true;
            else GridView1.AllowPaging = false;
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
                + " WHERE A.[日誌班別] = [View_處理過程].[日誌班別] AND A.[日誌編號] = [View_處理過程].[日誌編號] order by [處理時間] FOR XML PATH('')),1,12,'') AS [工作日誌]"
                + " from [View_日誌記錄] AS A,(" + SQL + ") AS B"                
                + " where A.[日誌班別]=B.[日誌班別] and A.[日誌編號]=B.[日誌編號]";
            
            SqlDataSourceSearch.SelectCommand = SQL ;
            GridView1.Sort("[日誌班別]", SortDirection.Descending);
            GridView1.DataBind();
            //Response.Write(SQL);
            ViewState["SQL"] = SQL;
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
        }
    }

    protected string FormatTourOP(string str)    //格式化班別及當班OP
    {
        string tour=str.Substring(0,9),strTour="",OP="",c="",tourW="";
        strTour = "<span title=\"日誌時間：" + str.Substring(10, 16) + "\n" + "存檔時間：" + str.Substring(27, 16) + "\">" + tour + "</span>";

        String[] opA = str.Substring(44).Trim().Split(' ');
        foreach (string op in opA) if (op.Trim() != "") OP = OP + "<span title=\"" + op + "\">" + op.Substring(0, 1) + "</span>";

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

        return("<font size=\"2\"><font color=\"blue\" onClick=\"TourClick(" + tour + ");\" style=\"cursor:pointer\"><u>" + strTour + "</u></font><br>"
            + OP + " " + c + tourW + "</font>\n");
    }

    protected void AddMsg(string strMsg)
    {
        Literal Msg = new Literal();
        Msg.Text = strMsg;
        Page.Controls.Add(Msg);
    }
}