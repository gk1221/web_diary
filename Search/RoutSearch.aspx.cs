﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Search_RoutSearch : System.Web.UI.Page
{
    protected void Page_Preload(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GridView1.Style.Add("table-layout", "fixed");
            GridView1.Columns[0].HeaderStyle.Width = 100;
            GridView1.Columns[1].HeaderStyle.Width = 75;
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
            string YYYY = DateTime.Now.Year.ToString();
            string MM = DateTime.Now.Month.ToString(); if (MM.Length < 2) MM = "0" + MM;
            for (int i = 0; i < SelYYYY.Items.Count; i++) if (SelYYYY.Items[i].Value == YYYY) SelYYYY.SelectedIndex = i;
            for (int i = 0; i < SelMM.Items.Count; i++) if (SelMM.Items[i].Value == MM) SelMM.SelectedIndex = i;

            if (ViewState["SQL"] != null) BtnSearch_Click(null, null);
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
        string YYYYMM = SelYYYY.SelectedValue + "/" + SelMM.SelectedValue;

        if (YYYYMM != "____/__")
        {
            string SQL = "SELECT * FROM [View_發生訊息] where [日誌編號]=0"; 

            //---------------------------------------------------------日期
            if (YYYYMM != "____/__") SQL = SQL + " and [訊息時間] like '" + YYYYMM + "%'"; 

            //---------------------------------------------------------分頁
            if (ChkPage.Checked) GridView1.AllowPaging = true;
            else GridView1.AllowPaging = false;
            //(CASE WHEN PUR_AMT IS NULL THEN 0 ELSE PUR_AMT END) PUR_AMT 
            //---------------------------------------------------------查詢顯示
                       
            
            SqlDataSourceSearch.SelectCommand = SQL ;
            GridView1.Sort("[訊息時間]", SortDirection.Ascending);
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
            item.Cells[2].Text = Server.HtmlDecode(item.Cells[2].Text);
        }
    }

    protected string FormatTourOP(string str)    //格式化班別及當班OP
    {
        string tour=str.Substring(0,9),strTour="",OP="",c="",tourW="";
        strTour = "<span title=\"日誌時間：" + str.Substring(10, 16) + "\n" + "存檔時間：" + str.Substring(27, 16) + "\">" + tour + "</span>";

        String[] opA = str.Substring(44).Trim().Split(' ');
        foreach (string op in opA) if(op.Trim() != "") OP=OP + "<span title=\"" + op + "\">" + op.Substring(0,1) + "</span>";

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
}