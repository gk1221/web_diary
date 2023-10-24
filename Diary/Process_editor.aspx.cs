using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Diary_Process : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //取得起始班別，並記錄狀態資訊
            string tour = GetTour("", 0);
            if (Session["tour"] != null) tour = Session["tour"].ToString();
            if (Request["tour"] != null) tour = Request["tour"].ToString();
            Session["tour"] = tour;

            //放Msg資訊，避免Session逾期
            if (Session["MsgDT"] != null) lblMsgDT.Text = Session["MsgDT"].ToString();
            if (Session["MsgText"] != null) lblMsgText.Text = Session["MsgText"].ToString();

            //沒有qryTour一律離開
            if (Request["qryTour"] == null) BtnExit_Click(null, null);

            //產生各種年月日下拉式選單  
            string DbYYYY = GetValue("diary", "select [Config] from [Config] where [Kind]='統計參數' and [Item]='起始年份'");
            AddSel(SelProYYYY, int.Parse(DbYYYY), DateTime.Now.Year + 1); AddSel(SelProMM, 1, 12); AddSel(SelProDD, 1, 31);
            AddSel(SelYYYY, int.Parse(DbYYYY), DateTime.Now.Year + 1); AddSel(SelMM, 1, 12); AddSel(SelDD, 1, 31);
            ListItem Item = new ListItem(); Item.Value = "yyyy"; Item.Text = "永久"; SelYYYY.Items.Add(Item);
            AddSel(SelProHH, 0, 23); AddSel(SelProMI, 0, 59);
            AddSel(SelMoveYYYY, int.Parse(DbYYYY), DateTime.Now.Year + 1); AddSel(SelMoveMM, 1, 12); AddSel(SelMoveDD, 1, 31);
            SqlDataSourceDiaryNo.SelectCommand = GetSelSQL(tour);
            SelDiaryNo.DataBind();

            SqlDataSourceOP.SelectCommand = "select '' as [成員],'' as [代號]"
                + " Union select [成員],[代號] from [View_組織架構] where [性質]='員工' and ([課別]='作業管理科' or [代號]='" + Request.Cookies["UserID"].Value.ToString() + "') order by [代號]";
        }
    }

    protected void Page_PreRenderComplete(object sender, EventArgs e)   //放Page_Load無法顯示
    {
        if (!IsPostBack)
        {
            //讀取傳入參數
            string tour = Session["tour"].ToString(), qryTour = "", DiaryNo = "", MsgNo = "", ProcessNo = "";
            if (Request["qryTour"] != null) qryTour = Request["qryTour"].ToString();
            if (Request["DiaryNo"] != null) DiaryNo = Request["DiaryNo"].ToString();
            if (Request["MsgNo"] != null) MsgNo = Request["MsgNo"].ToString();
            if (Request["ProcessNo"] != null) ProcessNo = Request["ProcessNo"].ToString();

            string ProYYYY = DateTime.Now.ToString("yyyy"), ProMM = DateTime.Now.ToString("MM"), ProDD = DateTime.Now.ToString("dd"), ProHH = DateTime.Now.ToString("HH"), ProMI = DateTime.Now.ToString("mm");
            string OP = Request.Cookies["UserID"].Value.ToString(), Call = "", Mt = "", ProText = "";            
			string txtFloorArea = "";			
			string txtSys = "", txtKind = "", txtDegree = "", txtStatus = "", YYYY = "", MM = "", DD = "", c = "";			
			string rt = "", nView = "";
			
            if (qryTour != "" & ProcessNo != "")  //讀值(全部) for 修改處理過程
            {
                SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
                Conn.Open();
                SqlCommand cmd = new SqlCommand("select * from [View_處理過程] where [日誌班別]='" + qryTour + "' and [處理編號]=" + ProcessNo, Conn);
                SqlDataReader dr = null;
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    LinkMove.Visible = true;    //有ProcessNo則顯示 [轉移至 ...] 按鈕

                    DiaryNo = dr["日誌編號"].ToString();
                    string ProYMD = dr["處理時間"].ToString();
                    ProYYYY = ProYMD.Substring(0, 4); ProMM = ProYMD.Substring(5, 2); ProDD = ProYMD.Substring(8, 2); ProHH = ProYMD.Substring(11, 2); ProMI = ProYMD.Substring(14, 2);

                    OP = dr["員工代號"].ToString(); Call = dr["叫修時段"].ToString(); Mt = dr["叫修人員"].ToString(); ProText = dr["處理過程"].ToString();
                    SelOP.Items.Clear();
                    ListItem Item = new ListItem();
                    Item.Text = GetValue("Diary", "select [成員] from [View_組織架構] where [性質]='員工' and [代號]='" + OP + "'"); ;
                    Item.Value = OP;
                    SelOP.Items.Add(Item);
                    txtPM.Text = dr["處理時長"].ToString();
					
					txtFloorArea = dr["樓層/機房"].ToString();
                    
                    txtSys = dr["根因系統"].ToString(); txtKind = dr["資產類型"].ToString(); txtDegree = dr["異常等級"].ToString(); txtStatus = dr["狀態說明"].ToString(); 
					rt = dr["評分"].ToString(); nView = dr["觀看數"].ToString();
					string YMD = dr["公告期限"].ToString();
                    if (YMD.Length > 3) YYYY = YMD.Substring(0, 4);
                    if (YMD.Length > 5) MM = YMD.Substring(4, 2);
                    if (YMD.Length > 7) DD = YMD.Substring(6, 2);
                    if (YMD.Length > 8) c = YMD.Substring(8, 1);
                }
                cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();
            }            
            else if (qryTour != "" & DiaryNo != "")  //讀值(日誌屬性) for 新增處理過程
            {
                SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
                Conn.Open();
                SqlCommand cmd = new SqlCommand("select * from [View_日誌記錄] where [日誌班別]='" + qryTour + "' and [日誌編號]=" + DiaryNo, Conn);
                SqlDataReader dr = null;
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtSys = dr["根因系統"].ToString(); txtKind = dr["資產類型"].ToString(); txtDegree = dr["異常等級"].ToString(); txtStatus = dr["狀態說明"].ToString();
                    
					string YMD = dr["公告期限"].ToString();
                    if (YMD.Length > 3) YYYY = YMD.Substring(0, 4);
                    if (YMD.Length > 5) MM = YMD.Substring(4, 2);
                    if (YMD.Length > 7) DD = YMD.Substring(6, 2);
                    if (YMD.Length > 8) c = YMD.Substring(8, 1);
                }
                cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();
            }

            //顯示
            for (int i = 0; i < SelProYYYY.Items.Count; i++) if (SelProYYYY.Items[i].Value == ProYYYY) SelProYYYY.SelectedIndex = i;
            for (int i = 0; i < SelProMM.Items.Count; i++) if (SelProMM.Items[i].Value == ProMM) SelProMM.SelectedIndex = i;
            for (int i = 0; i < SelProDD.Items.Count; i++) if (SelProDD.Items[i].Value == ProDD) SelProDD.SelectedIndex = i;
            for (int i = 0; i < SelProHH.Items.Count; i++) if (SelProHH.Items[i].Value == ProHH) SelProHH.SelectedIndex = i;
            for (int i = 0; i < SelProMI.Items.Count; i++) if (SelProMI.Items[i].Value == ProMI) SelProMI.SelectedIndex = i;

            if (ProcessNo == "") for (int i = 0; i < SelOP.Items.Count; i++) if (SelOP.Items[i].Value == OP) SelOP.SelectedIndex = i;
            
            for (int i = 0; i < SelCall.Items.Count; i++)
            {
                if (SelCall.Items[i].Value == Call) SelCall.SelectedIndex = i;
                if (SelCall.Items[i].Text != "") SelCall.Items[i].Text = SelCall.Items[i].Text + GetCount("Call", tour, SelCall.Items[i].Value);
            }

            if (ProText != "" & Call == "") SelCall.SelectedIndex = 1;
            txtMt.Text = Mt;
            txtProcess.Text = ProText;

            for (int i = 0; i < SelSysCode.Items.Count; i++)
            {
                if (SelSysCode.Items[i].Text == txtSys) SelSysCode.SelectedIndex = i;
                if (SelSysCode.Items[i].Text != "") SelSysCode.Items[i].Text = SelSysCode.Items[i].Text + GetCount("SysCode", tour, SelSysCode.Items[i].Value);
            }
            for (int i = 0; i < SelKind.Items.Count; i++)
            {
                if (SelKind.Items[i].Text == txtKind) SelKind.SelectedIndex = i;
                if (SelKind.Items[i].Text != "") SelKind.Items[i].Text = SelKind.Items[i].Text + GetCount("Kind", tour, SelKind.Items[i].Value);
            }
            for (int i = 0; i < SelDegree.Items.Count; i++)
            {
                if (SelDegree.Items[i].Text == txtDegree) SelDegree.SelectedIndex = i;
                if (SelDegree.Items[i].Text != "") SelDegree.Items[i].Text = SelDegree.Items[i].Text + GetCount("Degree", tour, SelDegree.Items[i].Value);
            }
            for (int i = 0; i < SelStatus.Items.Count; i++)
            {
                if (SelStatus.Items[i].Text == txtStatus) SelStatus.SelectedIndex = i;
                if (SelStatus.Items[i].Text != "") SelStatus.Items[i].Text = SelStatus.Items[i].Text + GetCount("Status", tour, SelStatus.Items[i].Value);
            }						
			for (int i = 0; i < SelFloorArea.Items.Count; i++)
            {
                if (SelFloorArea.Items[i].Value == txtFloorArea) SelFloorArea.SelectedIndex = i;                
            }
            
			txtRt.Text= rt;
			//txtView.Text= nView;
			
            for (int i = 0; i < SelYYYY.Items.Count; i++) if (SelYYYY.Items[i].Value == YYYY) SelYYYY.SelectedIndex = i;
            for (int i = 0; i < SelMM.Items.Count; i++) if (SelMM.Items[i].Value == MM) SelMM.SelectedIndex = i;
            for (int i = 0; i < SelDD.Items.Count; i++) if (SelDD.Items[i].Value == DD) SelDD.SelectedIndex = i;
            for (int i = 0; i < SelClass.Items.Count; i++) if (SelClass.Items[i].Value == c) SelClass.SelectedIndex = i;

            for (int i = 0; i < SelMoveYYYY.Items.Count; i++) if (SelMoveYYYY.Items[i].Value == tour.Substring(0,4)) SelMoveYYYY.SelectedIndex = i;
            for (int i = 0; i < SelMoveMM.Items.Count; i++) if (SelMoveMM.Items[i].Value == tour.Substring(4, 2)) SelMoveMM.SelectedIndex = i;
            for (int i = 0; i < SelMoveDD.Items.Count; i++) if (SelMoveDD.Items[i].Value == tour.Substring(6, 2)) SelMoveDD.SelectedIndex = i;
            for (int i = 0; i < SelMoveClass.Items.Count; i++) if (SelMoveClass.Items[i].Value == tour.Substring(8, 1)) SelMoveClass.SelectedIndex = i;

            AddMenu(MenuWord);  //導入詞彙
            AddMenu(MenuMt);  //導入人員(服務系統)
            AddMenuMt();  //導入人員
			
			
			string sBtnRate="window.open('../Diary/Rating.aspx?tour=" + qryTour + "&DiaryNo=" + DiaryNo + "','_blank','scrolling=yes, scrollbars=yes, width=480,height=680,top=' + (screen.height-720)/2 + ',left=' + (screen.width-400)/2) ;";
			//string sBtnRate="window.open('../Diary/Rating.aspx?tour=" + qryTour + "&DiaryNo=" + DiaryNo + "')";
			BtnRate.Attributes.Add("onclick",sBtnRate);
		}
    }

    protected void AddMenu(Menu menu)
    {
        SqlConnection Conn = new SqlConnection();
        string SQL = "";        
        
        if (menu.ID == "MenuMt")
        {
            MenuItem itemx = new MenuItem();
            itemx.Text = "軟-服務系統";
            menu.Items[0].ChildItems.Add(itemx);

            Conn.ConnectionString = WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString;
            SQL = "Select [成員],[成員] from [View_維護群組] where [群組]='軟-服務系統' order by [排序]";
        }
        else 
        {
            Conn.ConnectionString = WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString;
            SQL = "select [Mark] + ' ' + [Item] as [Item],[Memo] from [Config] where [Kind]='導入詞彙'";
            string SysCode = SelSysCode.SelectedValue; if (SysCode == "" & Request["MsgCode"] != null) SysCode = Request["MsgCode"].ToString().Substring(1, 3); 
            if (SysCode != "")
            {
                string SysGroup = GetValue("Diary", "select [Memo] from [Config] where [Kind]='系統代碼' and [Config]='" + SysCode + "'");
                SQL = SQL + " and ([Config]='***' or [Config] in (select [Config] from [Config] where [Kind]='系統代碼' and Memo='" + SysGroup + "'))";
            }
            SQL = SQL + " order by [Mark]";
        }
        
        
        Conn.Open();
        SqlCommand cmd = new SqlCommand(SQL, Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            MenuItem itemx = new MenuItem();
            itemx.Text = dr[0].ToString();
            itemx.Value = dr[1].ToString();
            itemx.ToolTip = dr[1].ToString();
            if (menu.ID == "MenuMt") menu.Items[0].ChildItems[0].ChildItems.Add(itemx);
            else menu.Items[0].ChildItems.Add(itemx);
        }
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();
    }

    protected void AddMenuMt()
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("Select * from [View_組織架構] where [性質]<>'單位' order by [單位] desc,[課別],[性質]", Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        string tmp = ""; MenuItem tmpItem = new MenuItem();
        while (dr.Read())
        {            
            if (tmp != dr[2].ToString())
            {
                tmpItem = new MenuItem();
                tmpItem.Text = dr[2].ToString() ;
                MenuMt.Items[0].ChildItems.Add(tmpItem);
            }
            tmp = dr[2].ToString();

            if(dr[0].ToString() != tmp)
            {
                MenuItem itemx = new MenuItem();
                itemx.Text = dr[0].ToString();
                tmpItem.ChildItems.Add(itemx);
            }
        }
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();
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

    //----------------------------取得統計資訊----------------------------------------------------------------------------------------------------------------------
    protected string GetCount(string kind, string tour, string x)
    {
        string Count30 = "0", Count365 = "0";
        string tour30 = DtToTour(DateTime.Parse(tour.Substring(0, 4) + "/" + tour.Substring(4, 2) + "/" + tour.Substring(6, 2)).AddDays(-30).ToString("yyyy/MM/dd HH:mm"));
        string tour365 = DtToTour(DateTime.Parse(tour.Substring(0, 4) + "/" + tour.Substring(4, 2) + "/" + tour.Substring(6, 2)).AddDays(-365).ToString("yyyy/MM/dd HH:mm"));

        if (kind == "Msg")
        {
            if (x.Substring(4, 1) != "d" & x != "2991f00" & x != "2991f02")    //排除例行工作及門禁自動導入
            {
                Count30 = GetValue("Diary", "select count(*) from [Msg] where [MsgCode]='" + x + "' and [tour] between '" + tour30 + "' and '" + tour + "'");
                Count365 = GetValue("Diary", "select count(*) from [Msg] where [MsgCode]='" + x + "' and [tour] between '" + tour365 + "' and '" + tour + "'");
            }
        }
        else if (kind == "Call")
        {
            Count30 = GetValue("Diary", "select count(*) from [CallMt] where [叫修時段]='" + x + "' and [班別] between '" + tour30 + "' and '" + tour + "'");
            Count365 = GetValue("Diary", "select count(*) from [CallMt] where [叫修時段]='" + x + "' and [班別] between '" + tour365 + "' and '" + tour + "'");
        }
        else
        {
            Count30 = GetValue("Diary", "select count(*) from [Diary] where [" + kind + "]='" + x + "' and [tour] between '" + tour30 + "' and '" + tour + "'");
            Count365 = GetValue("Diary", "select count(*) from [Diary] where [" + kind + "]='" + x + "' and [tour] between '" + tour365 + "' and '" + tour + "'");
        }

        return ("：" + Count30 + "/" + Count365);
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

    protected void BtnSave_Click(object sender, EventArgs e)  //存檔
    {
        string UnitID = Request.Cookies["UnitID"].Value;
        string qryTour = ""; if (Request["qryTour"] != null) qryTour = Request["qryTour"].ToString();
        string DiaryNo = ""; if (Request["DiaryNo"] != null) DiaryNo = Request["DiaryNo"].ToString();
        string ProcessNo = ""; if (Request["ProcessNo"] != null) ProcessNo = Request["ProcessNo"].ToString();
        string MsgCode = ""; if (Request["MsgCode"] != null) MsgCode = Request["MsgCode"].ToString();
        string StatisticDate = GetValue("Diary", "select [Config] from [Config] where [Kind]='統計參數' and [Item]='統計日期'");
        Boolean ExitYN = true;  //是否執行離開函數

        if (SelSysCode.SelectedValue == "") AddMsg("<script>alert('請設定根因系統 !');</script>");
        else if (SelKind.SelectedValue == "") AddMsg("<script>alert('請設定資產類型 !');</script>");
        else if (SelDegree.SelectedValue == "") AddMsg("<script>alert('請設定異常等級 !');</script>");
        else if (Request.Cookies["UserID"].Value.ToLower() == "operator" & (SelOP.SelectedValue == "" | SelOP.SelectedValue.ToLower() == "operator"))
            AddMsg("<script>alert('無法識別您的身份，請使用個人帳號記錄 !');</script>");
        else if (SelCall.SelectedValue == "" | SelCall.SelectedValue == "不用" & txtMt.Text != "" | SelCall.SelectedValue != "不用" & txtMt.Text == "") 
            AddMsg("<script>alert('請設定叫修時段或維護人員，若不需要請選不用 !');</script>");
        else
        {
            //if (UnitID != "SOS" & DiaryNo == "" | qryTour == "")   //僅機房可起始一筆新的日誌記錄
            if (qryTour == "")  //************************支援負責人輪班開放權限
            {
                AddMsg("<script>alert('請在現存記錄之下新增處理過程，或請機房起始一筆新的日誌記錄！');</script>");
                ExitYN = false;
            }
            if (GetValue("Diary","select [tour] from [Sign] where [tour]='" + qryTour + "'") == "") 
            {
                AddMsg("<script>alert('請先簽名！');</script>");
                ExitYN = false;
            }
            else if (UnitID != "SOS" & DateTime.Now.AddDays(-5).ToString("yyyyMM").CompareTo(qryTour.Substring(0, 6)) > 0)
            {
                AddMsg("<script>alert('每月" + StatisticDate + "日之後,不能再修改以前的日誌記錄與統計屬性，若仍有需要，請洽操作課課資安！');</script>");
                ExitYN = false;
            }
            else
            {
                if (DiaryNo == "") DiaryNo = InsDiary(qryTour);	            //無日誌編號代表要新增一筆日誌記錄
                else SaveDiary(qryTour, DiaryNo);	                //有日誌序號代表要修改日誌設定值                

                if (ProcessNo == "") InsProcess(qryTour, DiaryNo);	//無處理過程序號代表要新增一筆處理過程記錄
                else ExitYN = SaveProcess(qryTour, DiaryNo, ProcessNo);	    //有處理過程序號代表要修改處理過程記錄	    	    

                if (MsgCode != "") InsMsg(qryTour, DiaryNo, MsgCode);//有訊息編號代表要新增一筆訊息記錄
            }

            if(ExitYN) BtnExit_Click(null, null);  //離開
        }        
    }

    protected void BtnExit_Click(object sender, EventArgs e)  //離開
    {
        string qryFrom = "";
        if (Request["qryFrom"] != null) qryFrom = Request["qryFrom"].ToString();

        if (qryFrom == "Event") Response.Redirect("../Msg/Event.aspx");
        else Response.Redirect("diary.aspx");
    }

    protected string InsDiary(string qryTour)  //無日誌編號代表要新增一筆日誌記錄
    {
        string DiaryNo = GetValue("Diary", "select max([DiaryNo])+1 from [Diary] where [tour]='" + qryTour + "'"); if (DiaryNo == "") DiaryNo = "1";
        string DiaryDT = SelProYYYY.SelectedValue + "/" + SelProMM.SelectedValue + "/" + SelProDD.SelectedValue + " " + SelProHH.SelectedValue + ":" + SelProMI.SelectedValue;
        string SaveDT = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        string e_tour = SelYYYY.SelectedValue + SelMM.SelectedValue + SelDD.SelectedValue + SelClass.SelectedValue;

        ExecDbSQL("insert into [Diary] values('" + qryTour + "'," + DiaryNo + ",'" + DiaryDT + "','" + SaveDT + "',"
            + SelStatus.SelectedValue + ",'" + e_tour + "'," + SelDegree.SelectedValue + ",'" + SelSysCode.SelectedValue + "','" + SelKind.SelectedValue + "', '', '', '')");
        return (DiaryNo);
    }

    protected void SaveDiary(string qryTour, string DiaryNo) //有日誌序號代表要修改日誌設定值，任何人均可修改Diary屬性
    {
        string e_tour = SelYYYY.SelectedValue + SelMM.SelectedValue + SelDD.SelectedValue + SelClass.SelectedValue;
        string SaveDT = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        string pm = txtPM.Text==""?"0":txtPM.Text;
        //有修改屬性才需存檔
        string SaveFlag = "1";
        if (DiaryNo != "") SaveFlag = GetValue("Diary", "select count(*) from [Diary] where [tour]='" + qryTour + "' and [DiaryNo]=" + DiaryNo
             + " and ([Status]<>" + SelStatus.SelectedValue + " or [e_tour]<>'" + e_tour + "' or [Degree]<>" + SelDegree.SelectedValue
             + " or [SysCode]<>'" + SelSysCode.SelectedValue + "' or [Kind]<>'" + SelKind.SelectedValue + "') ");
        if (SaveFlag == "1") ExecDbSQL("update [Diary] set [SaveDT]='" + SaveDT + "',[Degree]=" + SelDegree.SelectedValue + ",[SysCode]='" + SelSysCode.SelectedValue
             + "',[Kind]='" + SelKind.SelectedValue + "',[Status]=" + SelStatus.SelectedValue + ",[e_tour]='" + e_tour 
             + "' where [tour]='" + qryTour + "' and [DiaryNo]=" + DiaryNo );
    }

    protected void InsProcess(string qryTour, string DiaryNo) //無處理過程序號代表要新增一筆處理過程記錄
    {
        string ProcessNo = GetValue("Diary", "select max(ProcessNo)+1 from [Process] where [tour]='" + qryTour + "'"); if (ProcessNo == "") ProcessNo = "1";
        string SaveDT = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        string ProcessDT = SelProYYYY.SelectedValue + "/" + SelProMM.SelectedValue + "/" + SelProDD.SelectedValue + " " + SelProHH.SelectedValue + ":" + SelProMI.SelectedValue;
        string ProText = txtProcess.Text.Replace("'", "''");
        string UserName = HttpUtility.UrlDecode(Request.Cookies["UserName"].Value);
        string UserID = Request.Cookies["UserID"].Value;
        string UnitID = Request.Cookies["UnitID"].Value;
        string UnitName = HttpUtility.UrlDecode(Request.Cookies["UnitName"].Value);
        string pm = txtPM.Text==""?"0":txtPM.Text;
        if (SelOP.SelectedValue != "" & UnitID == "SOS")  //機房新增記錄可改身分
        {
            UserID = SelOP.SelectedValue;
            UserName = SelOP.SelectedItem.Text;
        }

        ExecDbSQL("insert into [Process] values('" + qryTour + "'," + DiaryNo + "," + ProcessNo + ",'" + ProcessDT + "','" + SaveDT + "',N'"
            + ProText + "','" + UnitID + "','" + UserID + "','" + UnitName + "','" + UserName + "','" + SelFloorArea.SelectedValue + "' , "+ pm  +")");

        if (SelCall.SelectedValue != "不用") ExecDbSQL("insert into [CallMt] values('" + qryTour + "'," + DiaryNo + "," + ProcessNo + ",'" + SelCall.SelectedValue + "','" + txtMt.Text + "','" + SaveDT + "','" + UserName + "')");
    }

    protected Boolean SaveProcess(string qryTour, string DiaryNo, string ProcessNo)   //有處理過程序號代表要修改處理過程記錄   
    {
        string ProcessDT = "", SaveDT = "", ProText = "", UnitID = "", UserID = "";
        string ProDT = SelProYYYY.SelectedValue + "/" + SelProMM.SelectedValue + "/" + SelProDD.SelectedValue + " " + SelProHH.SelectedValue + ":" + SelProMI.SelectedValue;
        string UserName = HttpUtility.UrlDecode(Request.Cookies["UserName"].Value);
        Boolean ExitYN = true;  //是否執行離開函數		
		string FloorArea = "";
        string NewPm = txtPM.Text==""?"0":txtPM.Text;

        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("select * from [Process] where [tour]='" + qryTour + "' and [ProcessNo]=" + ProcessNo, Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            ProcessDT = dr["ProcessDT"].ToString(); SaveDT = dr["SaveDT"].ToString(); ProText = dr["ProText"].ToString();
            UnitID = dr["UnitID"].ToString(); UserID = dr["UserID"].ToString();			
			FloorArea = dr["Dist"].ToString();
            string Pm = dr["process_time"].ToString();
			
            if (UserID.ToLower() != Request.Cookies["UserID"].Value.ToLower() & (txtProcess.Text != ProText | ProcessDT != ProDT))
            {   //處理過程有動，就一定要本人才能改
                AddMsg("<script>alert('非本人記錄之處理過程不能修改(日誌屬性可以)，若仍須異動,請先新增再刪除 !');</script>");
                ExitYN = false;
            }
            else
            {   //修改處理資訊
                if (ProText != txtProcess.Text | ProcessDT != ProDT | FloorArea != SelFloorArea.SelectedValue | Pm != NewPm)
                {
                    ExecDbSQL("update [Process] set [SaveDT]='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "',[ProcessDT]='" + ProDT + "',"
                        + "[ProText]=N'" + txtProcess.Text.Replace("'", "''") + "',[Dist]='" + SelFloorArea.SelectedValue + "',[process_time]=" + NewPm
                        + " where [tour]='" + qryTour + "' and [ProcessNo]=" + ProcessNo);
                    InsProcessLog(qryTour, ProcessNo, ProcessDT, ProText, SaveDT);
                }                
            }
        }
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        //修改叫修資訊
        if (GetValue("Diary", "select [叫修時段] from [CallMt] where [班別]='" + qryTour + "' and [處理編號]=" + ProcessNo) != SelCall.SelectedValue
          | GetValue("Diary", "select [維修人員] from [CallMt] where [班別]='" + qryTour + "' and [處理編號]=" + ProcessNo) != txtMt.Text)
        {
            ExecDbSQL("delete from [CallMt] where [班別]='" + qryTour + "' and [處理編號]=" + ProcessNo);
            if (SelCall.SelectedValue != "不用" & SelCall.SelectedValue != "") ExecDbSQL("insert into [CallMt] values('" + qryTour + "'," + DiaryNo + "," + ProcessNo + ",'" + SelCall.SelectedValue + "','" + txtMt.Text + "','" + SaveDT + "','" + UserName + "')");
        }

        return (ExitYN);
    }

    protected void InsProcessLog(string qryTour, string ProcessNo, string ProcessDT, string ProText, string SaveDT)  //處理過程異動記錄
    {
        if (txtProcess.Text.IndexOf("@@") < 0 & ProText != txtProcess.Text & DateTime.Now.AddHours(-12).ToString("yyyy/MM/dd HH:mm").CompareTo(SaveDT) > 0)
        {
            string PkNo = GetValue("Diary", "select max([記錄編號])+1 from [異動記錄]"); if (PkNo == "") PkNo = "1";
            string ProDT = SelProYYYY.SelectedValue + "/" + SelProMM.SelectedValue + "/" + SelProDD.SelectedValue + " " + SelProHH.SelectedValue + ":" + SelProMI.SelectedValue;
            string strLog = "(原)：" + ProcessDT + " " + ProText + "<br /><br />(新)：" + ProDT + " " + txtProcess.Text;
            string UserName = HttpUtility.UrlDecode(Request.Cookies["UserName"].Value);

            ExecDbSQL("insert into [異動記錄] values(" + PkNo + ",'處理過程','" + qryTour + "." + ProcessNo + "','" + strLog.Replace("'", "''")
                + "','" + UserName + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "','" + Request.ServerVariables["REMOTE_ADDR"].ToString() + "')");
        }
    }


    protected void InsMsg(string qryTour, string DiaryNo, string MsgCode)    //有訊息編號代表要新增一筆訊息記錄
    {
        if (MsgCode != "")
        {   //以Session傳值
            string MsgNo = GetValue("Diary", "select max(MsgNo)+1 from [Msg] where [tour]='" + qryTour + "'"); if (MsgNo == "") MsgNo = "1";

            ExecDbSQL("insert into [Msg] values('" + qryTour + "'," + DiaryNo + "," + MsgNo + ",'" + lblMsgDT.Text
               + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "','" + MsgCode
               + "','" + lblMsgText.Text + "',0)");
        }
    }

    protected void SelStatus_SelectedIndexChanged(object sender, EventArgs e)   //結案日期預設為今日
    {
        if (SelStatus.SelectedValue == "2")
        {
            string toue = GetTour("", 0);
            for (int i = 0; i < SelYYYY.Items.Count; i++) if (SelYYYY.Items[i].Value == toue.Substring(0, 4)) SelYYYY.SelectedIndex = i;
            for (int i = 0; i < SelMM.Items.Count; i++) if (SelMM.Items[i].Value == toue.Substring(4, 2)) SelMM.SelectedIndex = i;
            for (int i = 0; i < SelDD.Items.Count; i++) if (SelDD.Items[i].Value == toue.Substring(6, 2)) SelDD.SelectedIndex = i;
            for (int i = 0; i < SelClass.Items.Count; i++) if (SelClass.Items[i].Value == toue.Substring(8, 1)) SelClass.SelectedIndex = i;
        }

    }

    protected void MenuMt_MenuItemClick(Object sender, MenuEventArgs e)   //導入人員
    {
        txtMt.Text = e.Item.Text;
    }

    protected void MenuWord_MenuItemClick(Object sender, MenuEventArgs e)   //導入詞彙
    {
        //AddMsg("<script type=\"text/javascript\">insertAtCaret(txtProcess,\"" + e.Item.Value + "\");</script>");

        txtProcess.Text = txtProcess.Text + e.Item.Value;
    }

    protected void SelSysCode_SelectedIndexChanged(object sender, EventArgs e)   //結案日期預設為今日
    {
        MenuWord.Items[0].ChildItems.Clear();
        AddMenu(MenuWord);
    }

    protected void LinkMove_Click(object sender, EventArgs e)  //轉移處理過程(轉移至 ...)
    {
        PanelMove.Visible = true;
        PanelSave.Visible = false;

        MoveTour_Changed(null, null);
    }

    protected void BtnCancel_Click(object sender, EventArgs e)  //轉移處理過程(取消)
    {
        PanelMove.Visible = false;
        PanelSave.Visible = true;
    }

    protected void BtnMove_Click(object sender, EventArgs e)  //轉移處理過程(確定)
    {
        string qryTour = Request["qryTour"].ToString(), DiaryNo = Request["DiaryNo"].ToString(), ProcessNo = Request["ProcessNo"].ToString();
        string MoveTour = SelMoveYYYY.SelectedValue + SelMoveMM.SelectedValue + SelMoveDD.SelectedValue + SelMoveClass.SelectedValue;

        if (qryTour == MoveTour & DiaryNo == int.Parse(SelDiaryNo.SelectedValue).ToString())
            AddMsg("<script>alert('同一筆日誌，不需要移動 ！');</script>");
        else if (GetValue("Diary", "select count(*) from [Diary] where [tour]='" + MoveTour + "' and [DiaryNo]=" + SelDiaryNo.SelectedValue) == "0")
            AddMsg("<script>alert('查無資料，請重新設定 ！');</script>");
        else
        {
            if (int.Parse(GetValue("Diary", "select count(*) from [Process] where [tour]='" + qryTour + "' and [DiaryNo]=" + DiaryNo)) <= 1)
            {
                ExecDbSQL("delete from [Diary] where [tour]='" + qryTour + "' and [DiaryNo]=" + DiaryNo);
                ExecDbSQL("delete from [Msg] where [tour]='" + qryTour + "' and [DiaryNo]=" + DiaryNo);
            }

            string MovePno = GetValue("Diary", "select max(ProcessNo)+1 from [Process] where [tour]='" + MoveTour + "'"); if (ProcessNo == "") ProcessNo = "1";
            ExecDbSQL("update [Process] set [tour]='" + MoveTour + "',[DiaryNo]=" + SelDiaryNo.SelectedValue + " ,[ProcessNo]=" + MovePno
                + " where [tour]='" + qryTour + "' and [ProcessNo]=" + ProcessNo);
            ExecDbSQL("update [CallMt] set [班別]='" + MoveTour + "',[日誌編號]=" + SelDiaryNo.SelectedValue + " ,[處理編號]=" + MovePno
                + " where [班別]='" + qryTour + "' and [處理編號]=" + ProcessNo);

            Session["tour"] = MoveTour;
            BtnExit_Click(null, null);  //離開
        }
    }

    protected void MoveTour_Changed(object sender, EventArgs e)   //轉移處理過程
    {
        string tour = SelMoveYYYY.SelectedValue + SelMoveMM.SelectedValue + SelMoveDD.SelectedValue + SelMoveClass.SelectedValue;
        SqlDataSourceDiaryNo.SelectCommand = GetSelSQL(tour);
        SelDiaryNo.DataBind();
    }

    protected string GetSelSQL(string tour)   //取得下拉式班別選單SQL
    {
        string qryTour = Request["qryTour"].ToString();
        string DiaryNo = "0"; if (Request["DiaryNo"] != null) Request["DiaryNo"].ToString();
        
        return ("(select '(請選擇)' as [TextNo],'-1' as [DiaryNo])"
            + " Union (select distinct substring(CAST([DiaryNo]+1000 AS nvarchar),2,3) as [TextNo],"
            + "CAST([DiaryNo] AS nvarchar) AS [DiaryNo] from [Diary] where [tour]='" + tour
            + "' and [DiaryNo]>0 and not ([tour]='" + qryTour + "' and [DiaryNo]=" + DiaryNo + ")) order by [TextNo]");
    }
}