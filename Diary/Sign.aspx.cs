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
        }
    }      

    protected void Page_PreRenderComplete(object sender, EventArgs e)   //放Page_Load無法顯示
    {
        if (!IsPostBack)
        {
            //取得當班OP資訊
            //string OPs = GetValue("Diary", "select [OPname] from [Sign] where [tour]='" + Session["tour"].ToString() + "'");
            string OPs = GetValue("Diary", "select [OPname] from [Sign] where [tour]= @tour", "tour", Session["tour"].ToString());
            string op = "";
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                CheckBox chk = (CheckBox)GridView1.Rows[i].Cells[0].Controls[1];
                op = GridView1.Rows[i].Cells[1].Text;
                if (OPs.IndexOf(op) >= 0) chk.Checked = true;
            }
            
            //取得溫濕度資訊
            string txt=GetValue("Diary","select [處理過程] from [View_工作日誌] where [日誌班別]='" + Session["tour"].ToString() + "' and [訊息代碼]='" + THCode + "'");
            if (txt != "")
            {
                int pos1 = 0, pos2 = 0, pos3 = 0;

                pos1 = txt.IndexOf("溫度", pos3 + 1); pos2 = txt.IndexOf("溼度", pos3 + 1); pos3 = txt.IndexOf("第一", pos1 + 1);
                txtTemp1.Text = txt.Substring(pos1 + 3, pos2 - pos1 - 3).Trim();
                txtHumi1.Text = txt.Substring(pos2 + 3, pos3 - pos2 - 3).Trim();

                pos1 = txt.IndexOf("溫度", pos3 + 1); pos2 = txt.IndexOf("溼度", pos3 + 1); pos3 = txt.IndexOf("<br>第二", pos1 + 1);
                txtTemp2.Text = txt.Substring(pos1 + 3, pos2 - pos1 - 3).Trim();
                txtHumi2.Text = txt.Substring(pos2 + 3, pos3 - pos2 - 3).Trim();

				pos1 = txt.IndexOf("溫度", pos3 + 1); pos2 = txt.IndexOf("溼度", pos3 + 1); pos3 = txt.IndexOf("第二", pos1 + 1);
                txtTemp3.Text = txt.Substring(pos1 + 3, pos2 - pos1 - 3).Trim();
                txtHumi3.Text = txt.Substring(pos2 + 3, pos3 - pos2 - 3).Trim();
				
				pos1 = txt.IndexOf("溫度", pos3 + 1); pos2 = txt.IndexOf("溼度", pos3 + 1); pos3 = txt.IndexOf("<br>第三", pos1 + 1);
                txtTemp4.Text = txt.Substring(pos1 + 3, pos2 - pos1 - 3).Trim();
                txtHumi4.Text = txt.Substring(pos2 + 3, pos3 - pos2 - 3).Trim();
				
				pos1 = txt.IndexOf("溫度", pos3 + 1); pos2 = txt.IndexOf("溼度", pos3 + 1); pos3 = txt.IndexOf("第三", pos1 + 1);
                txtTemp5.Text = txt.Substring(pos1 + 3, pos2 - pos1 - 3).Trim();
                txtHumi5.Text = txt.Substring(pos2 + 3, pos3 - pos2 - 3).Trim();
				
				pos1 = txt.IndexOf("溫度", pos3 + 1); pos2 = txt.IndexOf("溼度", pos3 + 1); pos3 = txt.IndexOf("<br>第三", pos1 + 1);
                txtTemp6.Text = txt.Substring(pos1 + 3, pos2 - pos1 - 3).Trim();
                txtHumi6.Text = txt.Substring(pos2 + 3, pos3 - pos2 - 3).Trim();
				
				pos1 = txt.IndexOf("溫度", pos3 + 1); pos2 = txt.IndexOf("溼度", pos3 + 1); pos3 = txt.IndexOf("第三", pos1 + 1);
                if (pos3 > 0)
                {
                    txtTemp7.Text = txt.Substring(pos1 + 3, pos2 - pos1 - 3).Trim();
                    txtHumi7.Text = txt.Substring(pos2 + 3, pos3 - pos2 - 3).Trim();

                    pos1 = txt.IndexOf("溫度", pos3 + 1); pos2 = txt.IndexOf("溼度", pos3 + 1); pos3 = txt.Length;
                    txtTemp8.Text = txt.Substring(pos1 + 3, pos2 - pos1 - 3).Trim();
                    txtHumi8.Text = txt.Substring(pos2 + 3, pos3 - pos2 - 3).Trim();
                }
                else
                {
                    pos3 = txt.Length;
                    txtTemp7.Text = txt.Substring(pos1 + 3, pos2 - pos1 - 3).Trim();
                    txtHumi7.Text = txt.Substring(pos2 + 3, pos3 - pos2 - 3).Trim();
                }               
            }
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

    protected string GetValue(string DB, string SQL, string key, string value)   //取得單一資料
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[DB + "ConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand(SQL, Conn);
        cmd.Parameters.AddWithValue(key, value);
        SqlDataReader dr = cmd.ExecuteReader();
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
        if (OPs != ""){
			ExecDbSQL("insert into [Sign] values('" + tour + "','" + OPs + "')");
    
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
			ExecDbSQL("insert into [diary] values('" + tour + "',0,'" + DiaryDT + "','" + nowDT + "',0,'',0,'***','f', NULL, NULL, NULL)");

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
			string strTH =     "第一機房最高：溫度:" + txtTemp1.Text + "　溼度:" + txtHumi1.Text
						   + "　第一機房最低：溫度:" + txtTemp2.Text + "　溼度:" + txtHumi2.Text
						   + "<br>第二機房最高：溫度:" + txtTemp3.Text + "　溼度:" + txtHumi3.Text
						   + "　第二機房最低：溫度:" + txtTemp4.Text + "　溼度:" + txtHumi4.Text
						   + "<br>第三機房熱通道最高：溫度:" + txtTemp5.Text + "　溼度:" + txtHumi5.Text
						   + "　第三機房熱通道最低：溫度:" + txtTemp6.Text + "　溼度:" + txtHumi6.Text
						   + "<br>第三機房冷通道最高：溫度:" + txtTemp7.Text + "　溼度:" + txtHumi7.Text
						   + "　第三機房冷通道最低：溫度:" + txtTemp8.Text + "　溼度:" + txtHumi8.Text;
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

					ExecDbSQL("insert into [Diary] values('" + tour + "'," + DiaryNo + ",'" + DiaryDT + "','" + nowDT + "',0,'',0,'992','d', NULL, NULL, NULL)");
					ExecDbSQL("insert into [Msg] values('" + tour + "'," + DiaryNo + "," + MsgNo + ",'" + DiaryDT + "','" + nowDT + "','" + THCode + "','"+ SopMsg + "',0)");
					ExecDbSQL("insert into [Process] values('" + tour + "'," + DiaryNo + "," + ProcessNo + ",'" + DiaryDT + "','" + nowDT
						+ "','" + strTH + "','" + Request.Cookies["UnitID"].Value + "','" + Request.Cookies["UserID"].Value + "','" 
						+ HttpUtility.UrlDecode(Request.Cookies["UnitName"].Value) + "','" + HttpUtility.UrlDecode(Request.Cookies["UserName"].Value) + "', '', '')");
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

					ExecDbSQL("insert into [Diary] values('" + tour + "'," + DiaryNo + ",'" + DiaryDT + "','" + nowDT + "',0,'',0,'992','d', NULL, NULL, NULL)");
					ExecDbSQL("insert into [Msg] values('" + tour + "'," + DiaryNo + "," + MsgNo + ",'" + DiaryDT + "','" + nowDT + "','" + RoutCode + "','" + SopMsg + "',0)");
					string strRout = GetValue("Diary", "select [Memo] from [Config] where [Kind]='導入詞彙' and [Mark]='841'");
					ExecDbSQL("insert into [Process] values('" + tour + "'," + DiaryNo + "," + ProcessNo + ",'" + DiaryDT + "','" + nowDT
						+ "','" + strRout + "','" + Request.Cookies["UnitID"].Value + "','" + Request.Cookies["UserID"].Value + "','" 
						+ HttpUtility.UrlDecode(Request.Cookies["UnitName"].Value) + "','" + HttpUtility.UrlDecode(Request.Cookies["UserName"].Value) + "', '', '')");
					ExecDbSQL("delete from [Msg] where [tour]='" + tour + "' and [DiaryNo]=0 and [MsgCode]='" + RoutCode + "'");
				}
			}

			AddMsg("<script>opener.open('../Msg/Rout.aspx','_self');window.close();</script>");
		}
		else{
			Literal Msg = new Literal();
            Msg.Text = "<script>alert('尚未輸入OP');</script>";
            Page.Controls.Add(Msg);
		}
		
    }
}