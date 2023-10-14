using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Msg_Event : System.Web.UI.Page
{
    protected void Page_Preload(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GridView1.Style.Add("table-layout", "fixed");
            GridView1.Columns[1].HeaderStyle.Width = 100;
            GridView1.Columns[3].HeaderStyle.Width = 60;
            GridView1.Columns[4].HeaderStyle.Width = 60;
            GridView1.Columns[5].HeaderStyle.Width = 40;
        }
    } 
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string UserID = "";
            try
            {
                UserID = Request.Cookies["UserID"].Value.ToString().ToUpper();
            }
            catch 
            {
                ResponseLogin(UserID);
            }            
        }
    }

    protected void ResponseLogin(string UserID)
    {
        Response.Write("無法取得認證帳號(" + UserID + ")，請先<a href='/' target='_top'>登入Windows網域</a>！若無法解決，請重啟瀏覽器。");
        Response.End();
    }

    protected void Page_PreRenderComplete(object sender, EventArgs e)   //放Page_Load無法顯示
    {
        if (ViewState["SQL"] != null) SqlDataSource1.SelectCommand = ViewState["SQL"].ToString();

        if (!IsPostBack)
        {
            SelMt_List();   //產生處理人員選單 

            if (Session["EventNo"] != null) //保持EventNo狀態
            {
                for (int i = 0; i < SelEvent.Items.Count; i++) if (SelEvent.Items[i].Value == Session["EventNo"].ToString()) SelEvent.SelectedIndex=i;
            }
            else SelEvent.Items[0].Selected = true;
            BtnSearch_Click(null, null);                       
        }
    }

    protected void SelEvent_SelectedIndexChanged(object sender, EventArgs e)   //事故編號
    {
        SelMt.Items.Clear();
        BtnSearch_Click(null, null);
        SelMt_List();   //產生處理人員選單 
    }

    protected void SelMt_List()   //產生處理人員選單
    {
        string EventNo = SelEvent.SelectedValue;
        string tour = EventNo.Substring(0, 9);
        string DiaryNo = EventNo.Substring(10);
        SqlDataSourceMt.SelectCommand = "select ''AS [維修人員] Union select distinct [維修人員] from [CallMt] where [班別]='" + tour + "' and [日誌編號]=" + DiaryNo;
        SelMt.DataBind();
    }

    protected void BtnEventAdd_Click(object sender, EventArgs e) //新增事故
    {
        string UserID = Request.Cookies["UserID"].Value.ToString().ToUpper();
        string UnitName = GetValue("Diary", "select [課別] from [View_組織架構] where [性質]='員工' and [代號]='" + UserID + "'");
        if (UnitName != "作業管理科") AddMsg("<script>alert('機房OP才有啟動緊急應變記錄(新增事件)的權限！');</script>");
        else
        {
            string tour = GetTour("", 0);
            string MsgCode="1930e03";
            string MsgDT=GetValue("Diary","select [SaveDT] from [SopLog] where [SopCode]='" + MsgCode + "'");
            string MsgText = GetValue("Diary", "select [SopMsg] from [SopLog] where [SopCode]='" + MsgCode + "'");
            Session["EventNo"] = null;

            Session["tour"] = tour;
            Session["MsgDT"] = MsgDT;
            Session["MsgText"] = MsgText;
            Response.Redirect("../Diary/Process.aspx?tour=" + tour + "&qryTour=" + tour + "&MsgCode=" + MsgCode + "&qryFrom=Event", true);
        }
    }

    protected void BtnProcessAdd_Click(object sender, EventArgs e) //新增記錄
    {
        string EventNo = SelEvent.SelectedValue;
        string tour = EventNo.Substring(0, 9);
        string DiaryNo = EventNo.Substring(10);
        Response.Redirect("../Diary/Process.aspx?qryTour=" + tour + "&DiaryNo=" + DiaryNo + "&qryFrom=Event");
    }

    protected void BtnSearch_Click(object sender, EventArgs e) //查詢
    {
        string EventNo = SelEvent.SelectedValue; 
        if (EventNo == "")
        {
            AddMsg("<script>alert('請先選取事件編號！');</script>");
        }
        else
        {
            string tour = EventNo.Substring(0, 9); Session["tour"] = tour; Session["EventNo"] = EventNo;
            string DiaryNo = EventNo.Substring(10); 
            string SQL = "select [ProcessNo] as [處理編號],[ProcessDT] as [處理時間],[ProText] as [處理過程],[維修人員] as [處理人員],[成員] as [存檔人員] from [Process]"
                + " LEFT OUTER JOIN [CallMt] ON [Process].[tour]=[CallMt].[班別] and [Process].[DiaryNo]=[CallMt].[日誌編號] and [Process].[ProcessNo]=[CallMt].[處理編號]"
                + " LEFT OUTER JOIN [View_組織架構] ON [Process].[UserID]=[View_組織架構].[代號] and [View_組織架構].[性質]='員工'"
                + " where [tour]='" + tour + "' and [DiaryNo]=" + DiaryNo;
            if (SelMt.SelectedValue != "" & SelMt.SelectedValue != "(空白)") SQL = SQL + " and ([維修人員]='" + SelMt.SelectedValue + "' or [維修人員] in (select [成員] from [View_組織架構] where [課別]='" + SelMt.SelectedValue + "' or [單位]='" + SelMt.SelectedValue + "'))";
            //else if (SelMtUnit.SelectedValue != "" & SelMtUnit.SelectedValue != "(空白)") SQL = SQL + " and ([維修人員]='" + SelMtUnit.SelectedValue + "' or [維修人員] in (select [成員] from [View_組織架構] where [課別]='" + SelMtUnit.SelectedValue + "' or [單位]='" + SelMtUnit.SelectedValue + "'))";

            SqlDataSource1.SelectCommand = SQL;
            GridView1.Sort("[處理時間]", SortDirection.Ascending);
            GridView1.DataBind();
            ViewState["SQL"] = SQL;            
        }
    }

    protected void GridView1_RowCommand(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "修改" | e.CommandName == "刪除")
        {
            string EventNo = SelEvent.SelectedValue;
            string tour = EventNo.Substring(0, 9);
            string DiaryNo = EventNo.Substring(10);
            int Idx = int.Parse(e.CommandArgument.ToString());
            string Pno = GridView1.DataKeys[Idx].Value.ToString(); if (Pno == "") Pno = "0";
            
            if (RightCheck(tour, DiaryNo, Pno))
            {
                switch (e.CommandName)
                {
                    case "修改":
                        {
                            Response.Redirect("../Diary/Process.aspx?qryTour=" + tour + "&DiaryNo=" + DiaryNo + "&ProcessNo=" + Pno + "&qryFrom=Event");
                            break;
                        }
                    case "刪除":
                        {
                            int Pcount = int.Parse(GetValue("Diary", "select count(*) from Process where tour='" + tour + "' and DiaryNo=" + DiaryNo));
                            if (Pcount < 2)
                            {
                                ExecDbSQL("delete from [Msg] where [tour]='" + tour + "' and [DiaryNo]=" + DiaryNo);
                                ExecDbSQL("delete from [Diary] where [tour]='" + tour + "' and [DiaryNo]=" + DiaryNo);
                                ExecDbSQL("delete from [CallMt] where [班別]='" + tour + "' and [DiaryNo]=" + DiaryNo);
                            }
                            else ExecDbSQL("delete from [CallMt] where [班別]='" + tour + "' and [處理編號]=" + Pno);                            

                            ExecDbSQL("delete from [Process] where [tour]='" + tour + "' and [ProcessNo]=" + Pno);
                            GridView1.DataBind();

                            break;
                        }
                }
            }
            else AddMsg("<script>alert('記錄人員才有修改的權限！');</script>");
        }
    }

    protected void BtnDiary_Click(object sender, EventArgs e)  //至工作日誌
    {
        string EventNo = SelEvent.SelectedValue;
        string tour = EventNo.Substring(0, 9);
        Response.Redirect("../Default.asp?Kind=Diary&qryTo=Diary&tour=" + tour + "&qryTour=" + tour);
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        GridView1.DataBind();
    }

    protected void ExecDbSQL(string SQL) //執行資料庫異動
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand(SQL, Conn);
        cmd.ExecuteNonQuery();
        cmd.Cancel(); cmd.Dispose(); Conn.Close(); Conn.Dispose();
    }

    protected Boolean RightCheck(string tour, string DiaryNo, string ProcessNo) //是否有權修改資料
    {
        string UserID = Request.Cookies["UserID"].Value.ToString().ToUpper();        
        string UnitName = GetValue("Diary", "select [課別] from [View_組織架構] where [性質]='員工' and [代號]='" + UserID + "'");        
        string KeyUserID = GetValue("Diary", "select [UserID] from [Process] where [tour]='" + tour + "' and DiaryNo=" + DiaryNo + " and ProcessNo=" + ProcessNo).ToUpper();
        string KeyUnitName = GetValue("Diary", "select [UnitName] from [Process] where [tour]='" + tour + "' and DiaryNo=" + DiaryNo + " and ProcessNo=" + ProcessNo).ToUpper();
        if (UserID != "operator" & (UserID.ToString() == KeyUserID.ToString() | UnitName == KeyUnitName | UnitName == "作業管理科")) return (true);
        else return (false);
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

    protected void AddMsg(string strMsg)
    {
        Literal Msg = new Literal();
        Msg.Text = strMsg;
        Page.Controls.Add(Msg);
    }

    protected string GetTour(string tour, int diff)   //取得班別，diff限1、0、-1
    {
        string TourOut = "", c = "3"; DateTime dt;

        if (tour == "")
        {
            dt = DateTime.Now;
            int hhmi = int.Parse(dt.ToString("HHmm"));

            if (hhmi < 0730) dt = dt.AddDays(-1);
            else if (hhmi >= 0730 & hhmi < 1530) c = "1";
            else if (hhmi >= 1530 & hhmi < 1930) c = "2";
        }
        else
        {
            dt = DateTime.Parse(tour.Substring(0, 4) + "/" + tour.Substring(4, 2) + "/" + tour.Substring(6, 2));
            c = tour.Substring(8, 1);
        }

        if (diff == 0) TourOut = dt.ToString("yyyyMMdd") + c;
        else if (diff > 0)
        {
            if (c == "1") TourOut = dt.ToString("yyyyMMdd") + "2";
            else if (c == "2") TourOut = dt.ToString("yyyyMMdd") + "3";
            else TourOut = dt.AddDays(1).ToString("yyyyMMdd") + "1";
        }
        else
        {
            if (c == "3") TourOut = dt.ToString("yyyyMMdd") + "2";
            else if (c == "2") TourOut = dt.ToString("yyyyMMdd") + "1";
            else TourOut = dt.AddDays(-1).ToString("yyyyMMdd") + "3";
        }

        return (TourOut);
    }
}