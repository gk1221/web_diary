using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Search_CallPrint : System.Web.UI.Page
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
        if (!IsPostBack)
        {
            lblCall.Text = Request["YYYY"].ToString() + "年" + Request["MM"].ToString() + "月" + Request["Sys"].ToString() + " 故障叫修紀錄單";
            
            SqlDataSourceSearch.SelectCommand = Session["SQL"].ToString();
            GridView1.Sort("[日誌班別]", SortDirection.Ascending);
            GridView1.DataBind();
        }
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
}