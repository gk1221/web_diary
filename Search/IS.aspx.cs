﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public partial class Search_IS : System.Web.UI.Page
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
            //產生各種年月日下拉式選單  
            string DbYYYY = GetValue("select [Config] from [Config] where [Kind]='統計參數' and [Item]='起始年份'");
            AddSel(SelYYYY, int.Parse(DbYYYY), DateTime.Now.Year + 1); 
            AddSel(SelMM, 1, 12);
			AddSel(SelwkTime,"上班","下班");

            //預設值
            string YYYY = DateTime.Now.AddMonths(-1).Year.ToString();
            string MM = DateTime.Now.AddMonths(-1).Month.ToString(); if (MM.Length < 2) MM = "0" + MM;
            for (int i = 0; i < SelYYYY.Items.Count; i++) if (SelYYYY.Items[i].Value == YYYY) SelYYYY.SelectedIndex = i;
            for (int i = 0; i < SelMM.Items.Count; i++) if (SelMM.Items[i].Value == MM) SelMM.SelectedIndex = i;

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
	
	protected void AddSel(DropDownList Sel, string st1, string st2)   //產生下拉式班別選單2
    {
        //Sel.Items.Add("");
		ListItem Item = new ListItem();
		Sel.Items.Add("");
		Sel.Items.Add(st1);
        Sel.Items.Add(st2);
		Sel.SelectedIndex = 1;
    }

    protected void BtnSearch_Click(object sender, EventArgs e)
    {
        string YYYYMM = SelYYYY.SelectedValue + SelMM.SelectedValue;

        if (SelSys.SelectedValue != "" | YYYYMM != "______")
        {
            string SQL = "SELECT distinct [日誌班別],[日誌編號] FROM [View_工作日誌] WHERE not ([員工代號]<>'" + Request.Cookies["UserID"].Value.ToString() + "' and [處理過程] like '%@@%')";

            //---------------------------------------------------------去掉例行
            SQL = SQL + " and [異常代碼]>0"; 
            
            //---------------------------------------------------------系統
            if (SelSys.SelectedValue != "") SQL = SQL + " and ([訊息代碼] like '_" + SelSys.SelectedValue + "%' or [系統代碼]='" + SelSys.SelectedValue + "')";

            //---------------------------------------------------------日期
            if (YYYYMM != "______") SQL = SQL + " and [日誌班別] like '" + YYYYMM + "%'"; 
			
			//---------------------------------------------------------時間
			if(SelwkTime.SelectedValue == "上班"){
				SQL = SQL + " and ([日誌時間] like '%06:%' or [日誌時間] like'%07:%' or [日誌時間] like'%08:%' or "+
				"[日誌時間] like '%09:%' or [日誌時間] like'%10:%' or [日誌時間] like'%11:%' or "+
				"[日誌時間] like '%12:%' or [日誌時間] like'%13:%' or [日誌時間] like'%14:%' or "+
				"[日誌時間] like '%15:%' or [日誌時間] like'%16:%' or [日誌時間] like'%17:%' "+
				")";
			}
			else if(SelwkTime.SelectedValue == "下班"){
				SQL = SQL + " and ([日誌時間] like '%00:%' or [日誌時間] like'%01:%' or [日誌時間] like'%02:%' or "+
				"[日誌時間] like '%03:%' or [日誌時間] like'%04:%' or [日誌時間] like'%05:%' or "+
				"[日誌時間] like '%18:%' or [日誌時間] like'%19:%' or [日誌時間] like'%20:%' or "+
				"[日誌時間] like '%21:%' or [日誌時間] like'%22:%' or [日誌時間] like'%23:%' "+
				")";
			} 
			
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
                + " WHERE A.[日誌班別] = [View_處理過程].[日誌班別] AND A.[日誌編號] = [View_處理過程].[日誌編號] and [處理過程] not like '%@@%' order by [處理時間] FOR XML PATH('')),1,12,'') AS [工作日誌]"
                + " from [View_日誌記錄] AS A,(" + SQL + ") AS B"                
                + " where A.[日誌班別]=B.[日誌班別] and A.[日誌編號]=B.[日誌編號]";
            
            SqlDataSourceSearch.SelectCommand = SQL ;
            GridView1.Sort("[日誌班別]", SortDirection.Ascending);
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
	
	protected void BtnHTML_Click(object sender, EventArgs e)
    {
        
        if (GridView1.Rows.Count > 0)
        {
            //int pos = ViewState["SQL"].ToString().IndexOf("FROM");
            //string ViewCols = SelShow.SelectedValue; //所有欲顯示欄位名稱

            //GridView gvExport = new GridView();
            //gvExport.DataSource = SqlDataSource1;
            //SqlDataSource1.SelectCommand = "Select " + ViewCols + ViewState["SQL"].ToString().Substring(pos);
           // gvExport.DataBind();
            
            string strExportFilename = SelSys.SelectedItem.Text + '-' + SelYYYY.Text + '.' + SelMM.Text;
            Trace.Warn(SelSys.SelectedItem.Text);

            Response.Clear();
            Response.ClearContent();
            Response.Write("<meta http-equiv=Content-Type content=text/html;charset=utf-8>");
            Response.AddHeader("content-disposition", "attachment;filename=" + strExportFilename + ".html");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.xls";
            Response.Charset = "utf-8";

            System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            GridView1.RenderControl(htmlWrite);
            Response.Write(stringWrite.ToString().Replace("<div>", "").Replace("</div>", ""));
            Response.End();
        }
    }

    protected void BtnExcel_Click(object sender, EventArgs e)
    {
        Trace.Warn("SWSW" );
        if (GridView1.Rows.Count > 0)
        {		
            // int pos = ViewState["SQL"].ToString().IndexOf("FROM");
            // string ViewCols = SelShow.SelectedValue; //所有欲顯示欄位名稱

            GridView gvExport = GridView1;
            // // gvExport.DataSource = SqlDataSource1;
            // SqlDataSource1.SelectCommand = "Select " + ViewCols + ViewState["SQL"].ToString().Substring(pos);
			gvExport.AllowPaging = false;
			//gvExport.DataBind();

            Response.Clear();
            Response.ClearContent();
			Response.AddHeader("content-disposition", "attachment;filename=IDMS.csv");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
			Response.ContentType = "application/text";			            
			Response.Charset = "BIG5";
			Response.ContentEncoding = System.Text.Encoding.GetEncoding(950);
						
			StringBuilder sb = new StringBuilder();			
			foreach (var c in gvExport.HeaderRow.Cells)
            {
				sb.Append('"' + (((System.Web.UI.WebControls.DataControlFieldCell)(c)).ContainingField).HeaderText + '"' + ",");				
            }
            sb.Append(gvExport.Rows.Count);
			sb.Append("\r\n");

            
			for (int i = 0; i < gvExport.Rows.Count; i++)
			{
				for (int k = 0; k < gvExport.Rows[i].Cells.Count; k++)
				{
                    Trace.Warn("SWSW" + gvExport.Rows[i].Cells[k].Text);
					sb.Append('"' + gvExport.Rows[i].Cells[k].Text.Replace("&nbsp;", "") + '"' + ',');
				}

				sb.Append("\r\n");
			}

			Response.Output.Write(sb.ToString());
			Response.Flush();
			Response.End();
        }
    }
	
	public override void VerifyRenderingInServerForm(Control control)    
    { 
        // '處理'GridView' 的控制項 'GridView' 必須置於有 runat=server 的表單標記之中   
    } 
}