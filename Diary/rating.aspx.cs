using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Diary_Sign : System.Web.UI.Page
{
    public const string THCode = "1992d00";	//溫溼度代碼
    public const string RoutCode = "1992d20";   //例行性工作
	

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //取得起始班別，並記錄狀態資訊
            string tour = "";
            if (Session["tour"] != null) tour = Session["tour"].ToString();
            if (Request["tour"] != null) tour = Request["tour"].ToString();
            if (tour == "") tour = GetTour("", 0);
            Session["tour"] = tour;

            string c = tour.Substring(8, 1);
            lblTour.Text = tour.Substring(0, 4) + " 年 " + tour.Substring(4, 2) + " 月 " + tour.Substring(6, 2) + " 日";

            switch (c)
            {
                case "1":
                    {
                        pMaster.Style.Add("background-color", "#E2ECFC");
                        lblTour.Text = lblTour.Text + " 早班";
                        break;
                    }
                case "2":
                    {
                        pMaster.Style.Add("background-color", "#FFE4CA");
                        lblTour.Text = lblTour.Text + " 午班";
                        break;
                    }
                case "3":
                    {
                        pMaster.Style.Add("background-color", "#E0E0E0");
                        lblTour.Text = lblTour.Text + " 晚班";
                        break;
                    }
            }
			string UserID = ""; 
			if (Request.Cookies["UserID"] != null) UserID = Request.Cookies["UserID"].Value;
			string DiaryNo = "";
			if (Request["DiaryNo"] != null) DiaryNo = Request["DiaryNo"].ToString();
			Session["DiaryNo"] = DiaryNo;
			
			Response.Write("使用者:" + UserID + ",tour=" + tour + ",DiaryNo=" + DiaryNo);
        }
		
		
    }      
	
    protected void Page_PreRenderComplete(object sender, EventArgs e)   //放Page_Load無法顯示
    {
        if (!IsPostBack)
        {
            //取得當班OP資訊
			
			
            string OPs = "";//GetValue("Diary", "select [OPname] from [Rating] where [tour]='" + Session["tour"].ToString() + "' and [DiaryNo]='" +Session["tour"].ToString() + "'");
            string op = "";
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
				op = GridView1.Rows[i].Cells[1].Text;
				string sRating ="";
				sRating=GetValue("Diary", "select [rating] from [Rating] where [tour]='" + Session["tour"].ToString() + "' and [DiaryNo]='" +Session["DiaryNo"].ToString() + "' and [OPname]='" + op + "'");
                CheckBox chk = (CheckBox)GridView1.Rows[i].Cells[0].Controls[1];
				//Response.Write("test"+sRating+",");
                int rating=-1; if(sRating.Length>0)rating = Convert.ToInt32(sRating);
				
                if ( rating  >= 0) {
					
					chk.Checked = true;
					
				}
				
            }
			
			txtRt.Text=GetValue("Diary", "select [rating] from [Diary] where [tour]='" + Session["tour"].ToString() + "' and [DiaryNo]='" +Session["DiaryNo"].ToString() + "'");
            txtEx.Text=GetValue("Diary", "select [Expert] from [Diary] where [tour]='" + Session["tour"].ToString() + "' and [DiaryNo]='" +Session["DiaryNo"].ToString() + "'");
            
        } 
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
        try
        {
            SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
            Conn.Open();
            SqlCommand cmd = new SqlCommand(SQL, Conn);
            cmd.ExecuteNonQuery();
            cmd.Cancel(); cmd.Dispose(); Conn.Close(); Conn.Dispose();
        }
        catch 
        {
            Response.Write(SQL);
            Response.End();
        }
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

    protected void BtnSave_Click(object sender, EventArgs e)  //存檔
    {
        string tour = Session["tour"].ToString();        
        string nowDT = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        string DiaryNo = "0", MsgNo = "0", ProcessNo = "0";

        //---------簽名作業--------------------------------------
        string OPs = "";    //當班OP
        ExecDbSQL("delete from [Sign] where [tour]='" + tour + "'");
        
        for (int i = 0; i < GridView1.Rows.Count; i++)
        {
            CheckBox chk = (CheckBox)GridView1.Rows[i].Cells[0].Controls[1];            
            if (chk.Checked) OPs = OPs + GridView1.Rows[i].Cells[1].Text + " ";
        }
        OPs = OPs.Trim();
        if (OPs != "") ExecDbSQL("insert into [Sign] values('" + tour + "','" + OPs + "')");
    
        //---------小鬧鐘例行工作--------------------------------------
        string YYYY = tour.Substring(0, 4), MM = tour.Substring(4, 2), DD = tour.Substring(6, 2), YY = YYYY.Substring(2, 2);
        string c = tour.Substring(8, 1);
        string YMD = YYYY + "/" + MM + "/" + DD;
        string DiaryDT = YMD ;        

        switch(c) //日誌時間
        {
            case "1": DiaryDT = DiaryDT + " 07:30"; break;
            case "2": DiaryDT = DiaryDT + " 15:30"; break;
            case "3": DiaryDT = DiaryDT + " 19:30"; break;
        }

        ExecDbSQL("delete from [diary] where [tour]='" + tour + "' and [DiaryNo]=0");
        ExecDbSQL("delete from [Msg] where [tour]='" + tour + "' and [DiaryNo]=0");
        ExecDbSQL("insert into [diary] values('" + tour + "',0,'" + DiaryDT + "','" + nowDT + "',0,'',0,'***','f', NULL, NULL,NULL, NULL)");

        string WeekDay = DateTime.Parse(DiaryDT).DayOfWeek.ToString("d"); if (WeekDay == "0") WeekDay = "7";
        string SQL = "select * from [Clock] where [WorkYN]='Y' and [RoutYN]='Y'" 
            + " and ([YY]='"  + YY + "' or [YY]='**')" 
            + " and ([MM]='"  + MM + "' or [MM]='**')"
            + " and ([DD]='" + DD + "' or [DD]='**'  or [DD]='*" + WeekDay + "')";

        MsgNo = GetValue("Diary", "select max([MsgNo])+1 from [Msg] where [tour]='" + tour + "'");
        if (MsgNo == "") MsgNo = "1";

        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand(SQL, Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        int hhmi = 0;
        while (dr.Read())
        {
            hhmi = int.Parse(dr["HH"].ToString() + dr["MI"].ToString());
            if (c == "1" & hhmi>=0730 & hhmi<1530 | c == "2" & hhmi>=1530 & hhmi<1930 | c == "3" & (hhmi>=1930 | hhmi<0730))
            {   
                ExecDbSQL("insert into [Msg] values('" + tour + "',0," + MsgNo + ",'" + YMD + " " + dr["HH"].ToString() + ":" + dr["MI"].ToString() + "','"
                    + nowDT + "','" + dr["MsgCode"].ToString() + "','" + dr["MsgText"].ToString() + "',0)");
                MsgNo = (int.Parse(MsgNo) + 1).ToString();
            }
        }
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        //---------溫溼度(取第一筆溫溼度記錄)--------------------------------------	
        string strTH =  "";
        string Pno = GetValue("Diary", "select [處理編號] from [View_工作日誌] where [日誌班別]='" + tour + "' and [訊息代碼]='" + THCode + "'");
        if (Pno != "") ExecDbSQL("Update [Process] set [ProText]='" + strTH + "',[SaveDT]='" + nowDT + "' where [tour]='" + tour + "' and [ProcessNo]=" + Pno);      
        else
        {
            string SopMsg = GetValue("Diary", "select [SopMsg] from [SopLog] where [SopCode]='" + THCode + "'");
            if (SopMsg != "")
            {
                DiaryNo = GetValue("Diary", "select max([DiaryNo])+1 from [Diary] where [tour]='" + tour + "'"); if (DiaryNo == "") DiaryNo = "1";
                MsgNo = GetValue("Diary", "select max([MsgNo])+1 from [Msg] where [tour]='" + tour + "'"); if (MsgNo == "") MsgNo = "1";
                ProcessNo = GetValue("Diary", "select max([ProcessNo])+1 from [Process] where [tour]='" + tour + "'"); if (ProcessNo == "") ProcessNo = "1";

                ExecDbSQL("insert into [Diary] values('" + tour + "'," + DiaryNo + ",'" + DiaryDT + "','" + nowDT + "',0,'',0,'992','d', NULL, NULL, NULL, NULL)");
                ExecDbSQL("insert into [Msg] values('" + tour + "'," + DiaryNo + "," + MsgNo + ",'" + DiaryDT + "','" + nowDT + "','" + THCode + "','"+ SopMsg + "',0)");
                ExecDbSQL("insert into [Process] values('" + tour + "'," + DiaryNo + "," + ProcessNo + ",'" + DiaryDT + "','" + nowDT
                    + "','" + strTH + "','" + Request.Cookies["UnitID"].Value + "','" + Request.Cookies["UserID"].Value + "','" 
                    + HttpUtility.UrlDecode(Request.Cookies["UnitName"].Value) + "','" + HttpUtility.UrlDecode(Request.Cookies["UserName"].Value) + "', '')");
            }
        }

        //---------例行性工作--------------------------------------	
        if (GetValue("Diary", "select [MsgCode] from [Msg] where [tour]='" + tour + "' and [MsgCode]='" + RoutCode + "' and [DiaryNo]<>0") == "")
        {
            string SopMsg = GetValue("Diary", "select [SopMsg] from [SopLog] where [SopCode]='" + RoutCode + "'");
            if (SopMsg != "")
            {
                DiaryNo = GetValue("Diary", "select max([DiaryNo])+1 from [Diary] where [tour]='" + tour + "'"); if (DiaryNo == "") DiaryNo = "1";
                MsgNo = GetValue("Diary", "select max([MsgNo])+1 from [Msg] where [tour]='" + tour + "'"); if (MsgNo == "") MsgNo = "1";
                ProcessNo = GetValue("Diary", "select max([ProcessNo])+1 from [Process] where [tour]='" + tour + "'"); if (ProcessNo == "") ProcessNo = "1";

                ExecDbSQL("insert into [Diary] values('" + tour + "'," + DiaryNo + ",'" + DiaryDT + "','" + nowDT + "',0,'',0,'992','d', NULL, NULL, NULL, NULL)");
                ExecDbSQL("insert into [Msg] values('" + tour + "'," + DiaryNo + "," + MsgNo + ",'" + DiaryDT + "','" + nowDT + "','" + RoutCode + "','" + SopMsg + "',0)");
                string strRout = GetValue("Diary", "select [Memo] from [Config] where [Kind]='導入詞彙' and [Mark]='841'");
                ExecDbSQL("insert into [Process] values('" + tour + "'," + DiaryNo + "," + ProcessNo + ",'" + DiaryDT + "','" + nowDT
                    + "','" + strRout + "','" + Request.Cookies["UnitID"].Value + "','" + Request.Cookies["UserID"].Value + "','" 
                    + HttpUtility.UrlDecode(Request.Cookies["UnitName"].Value) + "','" + HttpUtility.UrlDecode(Request.Cookies["UserName"].Value) + "', '')");
                ExecDbSQL("delete from [Msg] where [tour]='" + tour + "' and [DiaryNo]=0 and [MsgCode]='" + RoutCode + "'");
            }
        }

        AddMsg("<script>opener.open('../Msg/Rout.aspx','_self');window.close();</script>");
    }

	protected void BtnGood_Click(object sender, EventArgs e)  //存檔
	{
		string tour = Session["tour"].ToString(); 
		string DiaryNo = Session["DiaryNo"].ToString(); 
		string UserID = ""; 
		if (Request.Cookies["UserID"] != null) UserID = Request.Cookies["UserID"].Value;
		
		for (int i = 0; i < GridView1.Rows.Count; i++)
        {	
			
			
			string UserName = GetValue("IDMS","select [Item] from [Config] where [mark] = '" + UserID + "'");
			if (UserName != GridView1.Rows[i].Cells[1].Text)continue; //僅可以修改自己按讚紀錄
            CheckBox chk = (CheckBox)GridView1.Rows[i].Cells[0].Controls[1];       
			ExecDbSQL("delete from [Rating] where [tour]='" + tour + "' and [DiaryNo]='" + DiaryNo + "' and [OPname]='" + GridView1.Rows[i].Cells[1].Text + "'");
            if (chk.Checked) {
				//Response.Write(OPname);
				ExecDbSQL("insert into [Rating] values('" + tour + "','" + DiaryNo + "','" + GridView1.Rows[i].Cells[1].Text + "',1)");
			}
			if (!chk.Checked) {
				//
			}
        }
		
		Response.Redirect("rating.aspx?tour=" + tour +"&DiaryNo=" + DiaryNo);
		
		
	}
	
	protected void BtnRating_Click(object sender, EventArgs e)  //存檔
	{
		string tour = Session["tour"].ToString(); 
		string DiaryNo = Session["DiaryNo"].ToString(); 
		string UserID = ""; 
		if (Request.Cookies["UserID"] != null) UserID = Request.Cookies["UserID"].Value;
		string UserName = GetValue("IDMS","select [Item] from [Config] where [Kind] = '作業管理科' and [mark] = '" + UserID + "'");

		if (UserName != "") {
			if (int.Parse(txtRt.Text.ToString())>10) txtRt.Text="10";			
			ExecDbSQL("update [Diary] set [rating] = '" + txtRt.Text + "' , [Expert] = '" + UserID + "' where [tour]='" + tour + "' and [DiaryNo]='" + DiaryNo + "'");
		}

		Response.Redirect("rating.aspx?tour=" + tour +"&DiaryNo=" + DiaryNo);
	}
	
	
}