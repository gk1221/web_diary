using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Msg_Manu : System.Web.UI.Page
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
                        
            string SysCode="";            
            if (Request["SysCode"] != null) SysCode = Request["SysCode"].ToString();    //先由Request決定頁面，次由Session決定
            else if (Session["SysCode"] != null) SysCode = Session["SysCode"].ToString();
            Session["SysCode"] = SysCode;

            GridView1.Columns[3].HeaderText = "<font color=\"green\">" + Page.Title + " (" + GetValue("Diary", "select [Item] from [Config] where [Kind]='系統代碼' and [Config]='" + SysCode + "'") + ")</font>";
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
            if (BtnEdit.Visible) SqlDataSourceSearch.SelectCommand = SqlDataSourceSearch.SelectCommand + " and [Status]='啟用'";    //若非編輯狀態，則不顯示停用訊息
            SqlDataSourceDiaryNo.SelectCommand = SelSQL;
            GridView1.DataBind();
        }
        else if (Session["tour"].ToString() != "")    //
        {
            if (!Page.IsPostBack)
            {
                SQL = "select *,(select max([更新日]) from [View_SOP] where [View_SOP].[訊息代碼]=[SopLog].[SopCode]) as [SopDT]"
                    + ",(SopMsg + ' (' + CAST((select count(*) from [Msg] where [SopLog].[SopCode]=[Msg].[MsgCode] and [Msg].[tour] >='" + DateTime.Now.AddDays(-30).ToString("yyyyMMdd") + "3') AS nvarchar)"
                    + " + '/' + CAST((select count(*) from [Msg] where [SopLog].[SopCode]=[Msg].[MsgCode] and [Msg].[tour] >='" + DateTime.Now.AddDays(-365).ToString("yyyyMMdd") + "3') AS nvarchar)"
                    + " + '/' + CAST((select count(*) from [Msg] where [SopLog].[SopCode]=[Msg].[MsgCode]) AS nvarchar) + ')') as [訊息內容]"
                    + " from [SopLog] where [SopCode] like '_" + Session["SysCode"].ToString() + "___'";
                SelSQL = "(select '' as [TextNo],'' as [DiaryNo]) Union (select '(新增)' as [TextNo],'*' as [DiaryNo])"
                    + " Union (select distinct substring(CAST([DiaryNo]+1000 AS nvarchar),2,3) as [TextNo],CAST([DiaryNo] AS nvarchar) AS [DiaryNo] from [Diary] where [tour]='" + Session["tour"].ToString() + "' and [DiaryNo]>0)"
                    + " order by [DiaryNo]";
                ViewState["SQL"] = SQL;
                ViewState["SelSQL"] = SelSQL;

                SqlDataSourceSearch.SelectCommand = SQL;
                if (BtnEdit.Visible) SqlDataSourceSearch.SelectCommand = SqlDataSourceSearch.SelectCommand + " and [Status]='啟用'";    //若非編輯狀態，則不顯示停用訊息
                SqlDataSourceDiaryNo.SelectCommand = SelSQL;
                GridView1.Sort("[SubSys],[SopCode]", SortDirection.Ascending);
                GridView1.DataBind();

                lblLostQA.Text = GetLostQA(Session["SysCode"].ToString());   //取得[異常訊息]有而[手動訊息]無者
                lblLostDiary.Text = GetLostDiary(Session["SysCode"].ToString());   //取得[日誌訊息]有而[手動訊息]無者
                lblEndMsg.Text = GetEndMsg(Session["SysCode"].ToString());   //取得已停用之[手動訊息]
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
        string MsgNo = GetValue("Diary", "select max([MsgNo])+1 from [Msg] where [tour]='" + qryTour + "'"); if (MsgNo == "") MsgNo = "1";
        string MsgDT = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        string MsgText = GetValue("Diary", "select [SopMsg] from [SopLog] where [SopCode]='" + MsgCode + "'");  //不要取值到統計數字

        if (MsgCode.ToLower() != "please." & MsgCode.Length == 7)
            if (sel.SelectedValue == "*")
            {
                Session["MsgDT"] = MsgDT; Session["MsgText"] = MsgText;
                Response.Redirect("../Diary/Process.aspx?qryTour=" + Session["tour"].ToString() + "&MsgCode=" + MsgCode);
            }
            else
            {
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
            HyperLink hl = (HyperLink)item.Cells[1].Controls[0];
            string dt = GetValue("Diary", "select max([更新日]) from [View_SOP] where [訊息代碼]='" + hl.Text + "'");
            string SopDT = ""; if (dt != "") SopDT=DateTime.Parse(dt).ToString("yyyy/MM/dd HH:mm");
            string SubSys = GetValue("Diary", "select [SubSys] from [SopLog] where [SopCode]='" + hl.Text + "'");

            item.Cells[0].Attributes.Add("style", "color:" + GetColor(item.Cells[0].Text));
            item.Cells[1].Attributes.Add("title", SopDT);
            item.Cells[3].Attributes.Add("style", "color:" + GetColor(SopDT));
            item.Cells[3].Text = item.Cells[3].Text + "<br />" + GetQA(hl.Text);

            if(item.Cells[6].Text == "停用") item.Cells[6].Attributes.Add("style", "color:red");
        }
    }

    protected string GetQA(string MsgCode)   //取得單一資料
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("select * from View_SOP where [訊息代碼]='" + MsgCode + "' order by [問題序號]", Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        string cfg = ""; 
        while (dr.Read()) cfg = cfg + "<font color='gray' size='2'>˙ " + dr["問題描述"].ToString() + "<font><br/> \n";        
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        return (cfg);
    }

    protected string GetLostDiary(string SysCode)   //取得[日誌訊息]有而[手動訊息]無者
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("select distinct [訊息代碼],[發生訊息] from [View_訊息記錄] where [訊息代碼] like '_" + SysCode + "___'" 
            + " and Substring([訊息代碼],5,1) not in ('0','1','2','3','4','5','6','7','8','9')" 
            + " and [訊息代碼] not in (select [SopCode] from [SopLog])" 
            + " order by [訊息代碼] desc", Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        string cfg = "";
        while (dr.Read())
        {
            cfg = cfg + "<a href='/sop/sop.asp?Sel_Code=" + dr["訊息代碼"].ToString() + "' target='_blank'>" + dr["訊息代碼"].ToString() + "</a> " 
                + "<a href='../Search/Code.aspx?MsgCode=" + dr["訊息代碼"].ToString() + "'>(日誌)</a> " 
                + dr["發生訊息"].ToString() + "<br/> \n";
        }
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        return (cfg);
    }

    protected string GetLostQA(string SysCode)   //取得[異常訊息]有而[手動訊息]無者
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("select * from [View_SOP] where [訊息代碼] in (select distinct [訊息代碼] from [View_SOP] where [訊息代碼] like '_" + SysCode + "___' and [訊息代碼] not in (select [SopCode] from [SopLog])) order by [訊息代碼],[問題序號] desc", Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        string cfg = "";
        while (dr.Read())
        {
            cfg = cfg + "<a href='/sop/sop.asp?Sel_Code=" + dr["訊息代碼"].ToString() + "' target='_blank'>" + dr["訊息代碼"].ToString() + "</a> "
                + "<a href='../Search/Code.aspx?MsgCode=" + dr["訊息代碼"].ToString() + "'>(日誌)</a> "
                + dr["問題描述"].ToString() + "<br/> \n";
        }
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        return (cfg);
    }

    protected string GetEndMsg(string SysCode)   //已停用之[手動訊息]
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("select * from [SopLog] where [SopCode] like '_" + SysCode + "%' and [Status]='停用' order by [SopCode]", Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        string cfg = "";
        while (dr.Read())
        {
            cfg = cfg + "<a href='/sop/sop.asp?Sel_Code=" + dr["SopCode"].ToString() + "' target='_blank'>" + dr["SopCode"].ToString() + "</a> "
                + "<a href='../Search/Code.aspx?MsgCode=" + dr["SopCode"].ToString() + "'>(日誌)</a> "
                + dr["SopMsg"].ToString() + "<br/> \n";
        }
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        return (cfg);
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

    protected void BtnEdit_Click(object sender, EventArgs e)  //訊息設定
    {
        if (Request.Cookies["UnitID"].Value != "SOS" | Request.Cookies["UserID"].Value.ToLower() == "operator")
            AddMsg("<script>alert('您沒有設定訊息的權限！" + "');</script>");
        else
        {
            BtnEdit.Visible = false;
            PanelEdit.Visible = true;
            PanelEdit.DataBind();   //如此才讀得到ViewState
            for (int i = 0; i < SelSys.Items.Count; i++) if (SelSys.Items[i].Value == Session["SysCode"].ToString()) SelSys.SelectedIndex = i;
            GridView1.Columns[6].Visible = true;
            GridView1.Columns[7].Visible = true;
            GridView1.Columns[5].Visible = false;
        }
    }

    protected void BtnAdd_Click(object sender, EventArgs e)  //訊息新增
    {
        string SopCode = SelCall.SelectedValue + SelSys.SelectedValue + SelKind.SelectedValue + txtFlow.Text;
        if (SopCode.Length != 7)
            AddMsg("<script>alert('訊息代碼格式錯誤！');</script>");
        else if (GetValue("Diary", "select [SopCode] from [SopLog] where [SopCode]='" + SopCode + "'") != "") 
            AddMsg("<script>alert('該訊息已存在，請使用其它訊息代碼！');</script>");
        else 
        {
            string Status = "停用"; if (ChkStatus.Checked) Status = "啟用";
            string SQL = "insert into [SopLog] values('" + SopCode + "','" + txtManu.Text + "','" + txtSub.Text + "','" + Status + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "')";
            ExecDbSQL(SQL);
            InsLifeLog(SQL, SopCode);
            GridView1.DataBind();

            GridView1.SelectRow(-1);
            for (int i = 1; i < GridView1.Rows.Count; i++)
            {
                HyperLink hl = (HyperLink)GridView1.Rows[i].Cells[1].Controls[0];
                if (hl.Text == SopCode) GridView1.SelectRow(i);
            }
        }
    }

    protected void BtnSave_Click(object sender, EventArgs e)  //訊息修改
    {
        string SopCode = SelCall.SelectedValue + SelSys.SelectedValue + SelKind.SelectedValue + txtFlow.Text;
        HyperLink hl = (HyperLink)GridView1.SelectedRow.Cells[1].Controls[0];
        if (SopCode.Length != 7)
            AddMsg("<script>alert('訊息代碼格式錯誤！');</script>");
        else if (GetValue("Diary", "select [SopCode] from [SopLog] where [SopCode] <>'" + hl.Text + "' and [SopCode]='" + SopCode + "'") != "")
            AddMsg("<script>alert('該訊息已存在，請使用其它訊息代碼！');</script>");
        else
        {
            string Status = "停用"; if (ChkStatus.Checked) Status = "啟用";
            string SQL = "update [SopLog] set [SopCode]='" + SopCode + "',[SubSys]='" + txtSub.Text + "',[SopMsg]='" + txtManu.Text + "',[Status]='" + Status + "',[SaveDT]='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "' where [SopCode]='" + hl.Text + "'";
            ExecDbSQL(SQL);
            InsLifeLog(SQL, hl.Text);
            GridView1.DataBind();

            GridView1.SelectRow(-1);
            for (int i = 1; i < GridView1.Rows.Count; i++)
            {
                HyperLink hk = (HyperLink)GridView1.Rows[i].Cells[1].Controls[0];
                if (hk.Text == SopCode) GridView1.SelectRow(i);
            }
        }
    }

    protected void BtnDel_Click(object sender, EventArgs e)  //訊息刪除
    {
        HyperLink hl = (HyperLink)GridView1.SelectedRow.Cells[1].Controls[0];
        string SQL = "delete from [SopLog] where [SopCode]='" + hl.Text + "'";
        ExecDbSQL(SQL);
        InsLifeLog(SQL, hl.Text);
        GridView1.DataBind();

        GridView1.SelectRow(-1);
        SelCall.SelectedIndex = -1; SelSys.SelectedIndex = -1; SelKind.SelectedIndex = -1; txtFlow.Text = ""; txtSub.Text = ""; txtManu.Text = ""; ChkStatus.Checked = true;
    }

    protected void InsLifeLog(string Life,string PkNo) //寫入生命履歷
    {
        string LifeNo = (int.Parse(GetValue("Diary", "select max([記錄編號]) from [異動記錄]")) + 1).ToString(); //履歷編號
        if (LifeNo == "") LifeNo = "1";
        string TblName = "訊息設定";    //表格名稱
        string UN = GetValue("Diary", "select [成員] from [View_組織架構] where [性質]='員工' and [代號]='" + Request.Cookies["UserID"].Value + "'");   //登入的UserName：異動人員
        string LiftDT = DateTime.Now.ToString("yyyy/MM/dd HH:mm");  //異動日期
        string LifeIP = Request.ServerVariables["REMOTE_ADDR"].ToString();

        ExecDbSQL("Insert into [異動記錄] values(" + LifeNo + ",'" + TblName + "','" + PkNo + "','" + Life.Replace("'","") + "','" + UN + "','" + LiftDT + "','" + LifeIP + "')");
    }

    protected void BtnExit_Click(object sender, EventArgs e)  //訊息離開
    {
        BtnEdit.Visible = true;
        PanelEdit.Visible = false;
        GridView1.Columns[5].Visible = true;
        GridView1.Columns[6].Visible = false;
        GridView1.Columns[7].Visible = false;
        GridView1.SelectedIndex = -1;

        ChkStatus.Checked = true;
        SelCall.SelectedIndex = -1;
        for (int i = 0; i < SelSys.Items.Count; i++) if (SelSys.Items[i].Value == Session["SysCode"].ToString()) SelSys.SelectedIndex = i;
        SelKind.SelectedIndex = -1;
        txtFlow.Text = "";
        txtSub.Text = "";
        txtManu.Text = "";
    }

    protected void GridView1_SelectedIndexChanged(Object sender, EventArgs e)   //選取列
    {
        if (GridView1.SelectedRow != null)
        {
            BtnSave.Enabled = true;
            BtnDel.Enabled = true;

            if (GridView1.SelectedRow.Cells[6].Text == "啟用") ChkStatus.Checked = true;
            else ChkStatus.Checked = false;
            
            HyperLink hl = (HyperLink)GridView1.SelectedRow.Cells[1].Controls[0];
            for (int i = 0; i < SelCall.Items.Count; i++) if (SelCall.Items[i].Value == hl.Text.Substring(0, 1)) SelCall.SelectedIndex = i;
            for (int i = 0; i < SelSys.Items.Count; i++) if (SelSys.Items[i].Value == hl.Text.Substring(1, 3)) SelSys.SelectedIndex = i;
            for (int i = 0; i < SelKind.Items.Count; i++) if (SelKind.Items[i].Value == hl.Text.Substring(4, 1)) SelKind.SelectedIndex = i;
            
            txtFlow.Text = hl.Text.Substring(5);
            txtSub.Text = GetValue("Diary", "select [SubSys] from [SopLog] where [SopCode]='" + hl.Text + "'");
            txtManu.Text = GetValue("Diary", "select [SopMsg] from [SopLog] where [SopCode]='" + hl.Text + "'");
        }
        else
        {
            BtnSave.Enabled = false;
            BtnDel.Enabled = false;
        }
    }
}