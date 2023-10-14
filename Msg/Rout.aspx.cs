using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Msg_Rout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //取得起始班別，並記錄狀態資訊
            string tour = "";
            if (Session["tour"] != null) tour = Session["tour"].ToString();
            if (Request["tour"] != null) tour = Request["tour"].ToString();
            if (tour.Trim() == "") tour = GetTour("", 0);
            Session["tour"] = tour;

            GridView1.Columns[2].HeaderText = "<font color=\"green\">" + Page.Title + "</font>";
        }
    }      

    protected void Page_PreRenderComplete(object sender, EventArgs e)   //放Page_Load無法顯示
    {
        string SQL = "", SelSQL = "";
        if (ViewState["SQL"] != null)
        {
            SQL = ViewState["SQL"].ToString();
            SelSQL = ViewState["SelSQL"].ToString();

            SqlDataSourceSearch.SelectCommand = SQL;
            SqlDataSourceDiaryNo.SelectCommand = SelSQL;
            GridView1.DataBind();
        }
        else if (Session["tour"].ToString() != "")    //
        {
            if (!Page.IsPostBack)
            {
                SQL = "select *,substring([MsgDT],12,5) AS [訊息時間] from [Msg] where [tour]='" + Session["tour"].ToString() + "' and [DiaryNo]=0";
                SelSQL = "(select '' as [TextNo],'' as [DiaryNo]) Union (select '(新增)' as [TextNo],'*' as [DiaryNo])"
                    + " Union (select distinct substring(CAST([DiaryNo]+1000 AS nvarchar),2,3) as [TextNo],CAST([DiaryNo] AS nvarchar) AS [DiaryNo] from [Diary] where [tour]='" + Session["tour"].ToString() + "' and [DiaryNo]>0)"
                    + " Union (select '(查詢)' as [TextNo],'Code' as [DiaryNo]) order by [DiaryNo]";
                
                ViewState["SQL"] = SQL;
                ViewState["SelSQL"] = SelSQL;

                SQL = SQL + " order by [MsgDT]";

                SqlDataSourceSearch.SelectCommand = SQL;
                SqlDataSourceDiaryNo.SelectCommand = SelSQL;
                GridView1.DataBind();
            }
        }

        if (GridView1.Rows.Count <= 0) lblTip.Text = "無資料！";
    }

    protected void SelDiaryNo_SelectedIndexChanged(object sender, EventArgs e)  //選訊息記日誌
    {
        DropDownList sel = (DropDownList)sender;
        string MsgCode = "";
        TableRow row = (TableRow)sel.Parent.Parent;
        foreach (Control c in row.Cells[1].Controls)
        {
            if (c.GetType().Equals(typeof(HyperLink)))
            {
                HyperLink hl = (HyperLink)c;
                MsgCode = hl.Text;
            }
        }   

        string qryTour = Session["tour"].ToString();
        string DiaryNo = sel.SelectedValue;
        string MsgDT = row.Cells[0].Text;
        if (MsgDT.CompareTo("07:30") < 0)
        {
            string tmpTour = GetTour(qryTour, +1);
            MsgDT = tmpTour.Substring(0, 4) + "/" + tmpTour.Substring(4, 2) + "/" + tmpTour.Substring(6, 2) + " " + MsgDT;
        }
        else MsgDT = qryTour.Substring(0, 4) + "/" + qryTour.Substring(4, 2) + "/" + qryTour.Substring(6, 2) + " " + MsgDT;
        string MsgText = row.Cells[2].Text.Replace("'", "''");

        if (MsgCode.ToLower() != "please." & MsgCode.Length == 7)
            if (sel.SelectedValue == "*")
            {
                Session["MsgDT"] = MsgDT; Session["MsgText"] = MsgText;
                Response.Redirect("../Diary/Process.aspx?qryTour=" + Session["tour"].ToString() + "&MsgCode=" + MsgCode);
            }
            else if (sel.SelectedValue == "Code") Response.Redirect("../Search/Code.aspx?MsgCode=" + MsgCode);
            else
            {
                ExecDbSQL("update [Msg] set [DiaryNo]=" + DiaryNo + " where [tour]='" + qryTour + "' and [DiaryNo]=0 and [MsgCode]='" + MsgCode + "'") ;
                Response.Redirect("../Diary/diary.aspx");
            }
        else
        {
            AddMsg("<script>alert('訊息代碼格式不符，不能新增日誌記錄！');</script>");
        }
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)    //將SopDT存於SopCode.ToolTip
    {
        GridViewRow item = e.Row;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HyperLink hl = (HyperLink)item.Cells[1].Controls[0];
            string dt = GetValue("Diary", "select max([更新日]) from [View_SOP] where [訊息代碼]='" + hl.Text + "'");
            string SopDT = ""; if (dt != "") SopDT = DateTime.Parse(dt).ToString("yyyy/MM/dd HH:mm");
            //item.Cells[0].Attributes.Add("style", "color:" + GetColor(item.Cells[0].Text));
            item.Cells[1].Attributes.Add("title", SopDT);
            item.Cells[2].Attributes.Add("style", "color:" + GetColor(SopDT));
        }
    }

    protected string DtToTour(string dt)   //時間轉班別
    {
        string tour = dt.Substring(0, 4) + dt.Substring(5, 2) + dt.Substring(8, 2);
        int hhmi = int.Parse(dt.Substring(11, 2) + dt.Substring(14, 2));

        if (hhmi < 0730) tour = GetTour(tour + "1", -1).Substring(0, 8) + "3";
        else if (hhmi >= 0730 & hhmi < 1530) tour = tour + "1";
        else if (hhmi >= 1530 & hhmi < 1930) tour = tour + "2";
        else tour = tour + "3";

        return (tour);
    }

    protected string GetColor(string dt)   //時間轉班別
    {
        string DtColor = "Brown";
        if (dt != "")
        {
            string tour = Session["tour"].ToString();
            string dt00 = DtToTour(DateTime.Parse(dt).ToString("yyyy/MM/dd HH:mm"));
            string dt07 = DtToTour(DateTime.Parse(dt).AddDays(7).ToString("yyyy/MM/dd HH:mm"));
            string dt30 = DtToTour(DateTime.Parse(dt).AddDays(30).ToString("yyyy/MM/dd HH:mm"));
            string dt365 = DtToTour(DateTime.Parse(dt).AddDays(365).ToString("yyyy/MM/dd HH:mm"));


            if (dt07.CompareTo(tour) >= 0) DtColor = "Red"; //一星期內修改顯示綠色 
            else if (dt07.CompareTo(tour) < 0 & dt30.CompareTo(tour) >= 0) DtColor = "Green"; //一月內修改顯示藍色 
            else if (dt30.CompareTo(tour) < 0 & dt365.CompareTo(tour) >= 0) DtColor = "Blue"; //一年內修改顯示黑色 
            else if (dt365.CompareTo(tour) < 0) DtColor = "Black"; //一年前修改顯示灰色 
            else if (dt00.CompareTo(tour) > 0) DtColor = "#9900CC"; //當班後修改顯示紫色 
        }
        return (DtColor);
    }

    protected string GetValue(string DB, string SQL)   //取得單一資料
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[DB + "ConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand(SQL, Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        string cfg = ""; if (dr.Read()) cfg = dr[0].ToString();
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        return (cfg);
    }

    protected void ExecDbSQL(string SQL) //執行資料庫異動
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand(SQL, Conn);
        cmd.ExecuteNonQuery();
        cmd.Cancel(); cmd.Dispose(); Conn.Close(); Conn.Dispose();
    }

    protected void AddMsg(string strMsg)
    {
        Literal Msg = new Literal();
        Msg.Text = strMsg;
        Page.Controls.Add(Msg);
    }

    protected string GetTour(string tour,int diff)   //取得班別
    {
        string TourOut = "",c="3"; DateTime dt;

        if (tour == "")
        {
            dt = DateTime.Now;
            int hhmi =int.Parse(dt.ToString("HHmm"));

            if(hhmi < 0730) dt=dt.AddDays(-1);
            else if(hhmi >= 0730 & hhmi < 1530) c = "1";
            else if (hhmi >= 1530 & hhmi < 1930) c = "2";
        }
        else
        {
            dt = DateTime.Parse(tour.Substring(0,4) + "/" + tour.Substring(4,2) + "/" + tour.Substring(6,2));
            c = tour.Substring(8, 1);
        }

        if (diff == 0) TourOut = dt.ToString("yyyyMMdd") + c;
        else if (diff > 0)
        {
            if (c == "1") TourOut = dt.ToString("yyyyMMdd") + "2";
            else if (c == "2") TourOut = dt.ToString("yyyyMMdd") + "3";
            else TourOut =dt.AddDays(1).ToString("yyyyMMdd") + "1";
        }
        else 
        {
            if (c == "3") TourOut = dt.ToString("yyyyMMdd") + "2";
            else if (c == "2") TourOut = dt.ToString("yyyyMMdd") + "1";
            else TourOut = dt.AddDays(-1).ToString("yyyyMMdd") + "3";
        }

        return (TourOut);
    }

    protected string GetTourSQL(string tour)   //取得班別SQL
    {
        string SQL = "", c = tour.Substring(8, 1);
        string NowYMD = tour.Substring(0, 4) + "/" + tour.Substring(4, 2) + "/" + tour.Substring(6, 2);
        string NextTour = GetTour(tour, 1);
        string NextYMD = NextTour.Substring(0, 4) + "/" + NextTour.Substring(4, 2) + "/" + NextTour.Substring(6, 2);

        if (c == "1") SQL = "'" + NowYMD + " 07:30:00' and '" + NowYMD + " 15:29:59'";
        else if (c == "2") SQL = "'" + NowYMD + " 15:30:00' and '" + NowYMD + " 19:29:59'";
        else SQL = "'" + NowYMD + " 19:30:00' and '" + NextYMD + " 07:29:59'";

        return (SQL);
    }
}