using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Msg_Auto : System.Web.UI.Page
{
    protected void Page_Preload(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GridView1.Style.Add("table-layout", "fixed");
            GridView1.Columns[0].HeaderStyle.Width = 60;
            GridView1.Columns[1].HeaderStyle.Width = 40;
            GridView1.Columns[2].HeaderStyle.Width = 75;
            GridView1.Columns[4].HeaderStyle.Width = 40;
            GridView1.Columns[5].HeaderStyle.Width = 70;
        }
    } 
    
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

            GridView1.Columns[3].HeaderText = "<font color=\"green\">" + Page.Title + "</font>";

            if (Request["DBaction"] != null) ExecDbSQL("update [Warnlog] set [Status]='R' where [Serial]=" + Request["Serial"].ToString());

        }
    }      

    protected void Page_PreRenderComplete(object sender, EventArgs e)   //放Page_Load無法顯示
    {
        //產生各種年月日下拉式選單
        if (!Page.IsPostBack)
        {
            string DbYYYY = GetValue("diary", "select [Config] from [Config] where [Kind]='統計參數' and [Item]='起始年份'");
            AddSel(SelYYYY, int.Parse(DbYYYY), DateTime.Now.Year + 1); AddSel(SelMM, 1, 12); AddSel(SelDD, 1, 31);
        }
        string SaveTour = Session["tour"].ToString(); if (ViewState["SaveTour"] != null) SaveTour = ViewState["SaveTour"].ToString();
        SetSel(SaveTour);   //選取下拉式存檔班別選單        

        string SQL = "";
        if (ViewState["SQL"] != null)
        {
            SQL = ViewState["SQL"].ToString();

            SqlDataSourceDiaryNo.SelectCommand = GetSelSQL(SaveTour);
            SqlDataSourceSearch.SelectCommand = SQL ;
            GridView1.Sort("[WarnDT]", SortDirection.Descending);
            GridView1.DataBind();
        }
        else if (Session["tour"].ToString() != "")    //
        {
            if (!Page.IsPostBack)
            {
                SQL = "select *,substring([WarnDT],12,5) as [訊息時間],Case [Status] When 'W' Then '讀取' When 'R' Then '已讀' Else '其它' End [訊息狀態] from [WarnLog] where [WarnDT] between " + GetTourSQL(Session["tour"].ToString());
                ViewState["SQL"] = SQL;

                SqlDataSourceDiaryNo.SelectCommand = GetSelSQL(SaveTour);
                SqlDataSourceSearch.SelectCommand = SQL ;
                GridView1.Sort("[WarnDT]", SortDirection.Descending);
                GridView1.DataBind();
            }
        }

        if (GridView1.Rows.Count <= 0) lblTip.Text = "無資料！";
    }

    protected string GetSelSQL(string tour)   //取得下拉式班別選單SQL
    {
        return("(select '' as [TextNo],'' as [DiaryNo]) Union (select '(新增)' as [TextNo],'*' as [DiaryNo])"
            + " Union (select distinct substring(CAST([DiaryNo]+1000 AS nvarchar),2,3) as [TextNo]," 
            + "CAST([DiaryNo] AS nvarchar) AS [DiaryNo] from [Diary] where [tour]='" + tour + "' and [DiaryNo]>0)"
            + " Union (select '(查詢)' as [TextNo],'Code' as [DiaryNo]) order by [DiaryNo]");
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

    protected void SetSel(string tour)   //選取下拉式班別選單
    {
        if (tour != "")
        {
            for (int i = 0; i < SelYYYY.Items.Count; i++) if (SelYYYY.Items[i].Value == tour.Substring(0, 4)) SelYYYY.SelectedIndex = i;
            for (int i = 0; i < SelMM.Items.Count; i++) if (SelMM.Items[i].Value == tour.Substring(4, 2)) SelMM.SelectedIndex = i;
            for (int i = 0; i < SelDD.Items.Count; i++) if (SelDD.Items[i].Value == tour.Substring(6, 2)) SelDD.SelectedIndex = i;
            for (int i = 0; i < SelClass.Items.Count; i++) if (SelClass.Items[i].Value == tour.Substring(8, 1)) SelClass.SelectedIndex = i;
        }
    }

    protected void SaveTour_Changed(object sender, EventArgs e)   //改變存檔班別
    {
        string tour=SelYYYY.SelectedValue + SelMM.SelectedValue + SelDD.SelectedValue + SelClass.SelectedValue;
        SqlDataSourceDiaryNo.SelectCommand = GetSelSQL(tour);
        SqlDataSourceSearch.SelectCommand = ViewState["SQL"].ToString() ;
        GridView1.Sort("[WarnDT]", SortDirection.Descending);
        GridView1.DataBind();        
        ViewState["SaveTour"] = tour;   //存檔班別
    } 

    protected void SelDiaryNo_SelectedIndexChanged(object sender, EventArgs e)  //選訊息記日誌
    {
        //取得訊息代碼，以便判斷能否新增日誌
        DropDownList sel = (DropDownList)sender;
        string MsgCode = ""; 
        TableRow row = (TableRow)sel.Parent.Parent;                        
        foreach (Control c in row.Cells[2].Controls)
        {
            if (c.GetType().Equals(typeof(HyperLink)))
            {
                HyperLink hl = (HyperLink)c;
                MsgCode = hl.Text;
            }
        }

        string qryTour = Session["tour"].ToString();
        string DiaryNo = sel.SelectedValue;        
        string MsgDT = GetValue("Diary", "select [WarnDT] from [WarnLog] where [Serial]=" + row.Cells[0].Text);
        string MsgText = row.Cells[3].Text.Replace("'", "''");

        if (MsgCode.ToLower() != "please." & MsgCode.Length == 7)
            if (sel.SelectedValue == "*")
            {
                //需用Request主鍵控制操作，用Session傳值
                qryTour = SelYYYY.SelectedValue + SelMM.SelectedValue + SelDD.SelectedValue + SelClass.SelectedValue;   //存檔班別
                Session["tour"] = qryTour; Session["MsgDT"] = MsgDT; Session["MsgText"] = MsgText;
                Response.Redirect("../Diary/Process.aspx?qryTour=" + Session["tour"].ToString() + "&MsgCode=" + MsgCode);
            }
            else if (sel.SelectedValue == "Code") Response.Redirect("../Search/Code.aspx?MsgCode=" + MsgCode);
            else if (sel.SelectedValue != "")
            {
                qryTour = SelYYYY.SelectedValue + SelMM.SelectedValue + SelDD.SelectedValue + SelClass.SelectedValue;   //存檔班別
                Session["tour"] = qryTour;
                string MsgNo = GetValue("Diary", "select max([MsgNo])+1 from [Msg] where [tour]='" + qryTour + "'"); if (MsgNo == "") MsgNo = "1";
                ExecDbSQL("insert into [Msg] values('" + qryTour + "'," + DiaryNo + "," + MsgNo + ",'" + MsgDT
                   + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "','" + MsgCode + "','" + MsgText + "',0)");
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
            HyperLink hl = (HyperLink)item.Cells[2].Controls[0];
            HyperLink hlStatus = (HyperLink)item.Cells[4].Controls[0];
            string dt = GetValue("Diary", "select max([更新日]) from [View_SOP] where [訊息代碼]='" + hl.Text + "'");
            string SopDT = ""; if (dt != "") SopDT=DateTime.Parse(dt).ToString("yyyy/MM/dd HH:mm");
            string MsgDT = GetValue("Diary", "select [WarnDT] from [WarnLog] where [Serial]=" + item.Cells[0].Text);

            //item.Cells[1].Attributes.Add("style", "color:" + GetColor(item.Cells[1].Text));
            item.Cells[1].Attributes.Add("title", MsgDT);
            item.Cells[2].Attributes.Add("title", SopDT);
            item.Cells[3].Attributes.Add("style", "color:" + GetColor(SopDT));
            if (hlStatus.Text == "讀取")
            {
                hlStatus.ForeColor = System.Drawing.Color.Green;
                item.Cells[3].Text = "<font color='green' size='4'><b>" + item.Cells[3].Text + "</b></font>";
            }
            else
            {
                hlStatus.ForeColor = System.Drawing.Color.Black;
                hlStatus.NavigateUrl = "";
            }
        }
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

    protected string DtToTour(string dt)   //時間轉班別
    {
        string tour=dt.Substring(0,4)+dt.Substring(5,2)+dt.Substring(8,2);
        int hhmi =int.Parse(dt.Substring(11,2)+dt.Substring(14,2));

        if(hhmi<0730) tour=GetTour(tour+"1",-1).Substring(0,8)+"3";
        else if(hhmi>=0730 & hhmi<1530) tour=tour + "1";
        else if(hhmi>=1530 & hhmi<1930) tour=tour + "2";
        else tour=tour + "3";

        return(tour);
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