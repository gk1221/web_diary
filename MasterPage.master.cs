using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //取得起始班別，並記錄狀態資訊
            string tour = GetTour("", 0);
            if (Session["tour"] != null) tour = Session["tour"].ToString();
            if (Request["tour"] != null) tour = Request["tour"].ToString();
            if (tour == "") tour = GetTour("",0);
            Session["tour"] = tour;

            if (Request.Cookies["UserID"] == null) Response.Redirect("../");
            else
            {
                AddMenu(0, 1);

                InitializeMaster(tour.Substring(8, 1)); //初始化主板版面

                //產生各種年月日下拉式選單  
                string DbYYYY = GetValue("diary", "select [Config] from [Config] where [Kind]='統計參數' and [Item]='起始年份'");
                AddSel(SelYYYY, int.Parse(DbYYYY), DateTime.Now.Year+1); AddSel(SelMM, 1, 12); AddSel(SelDD, 1, 31);
                SetSel(tour);   //選取下拉式班別選單

                SetWeekSeason(tour);   //設定星期X與節氣
                //SetOPs();   //設定OP(在版面執行response)                 
                
                //登入資訊
                lblLogin.Text = "(" + Request.Cookies["UnitID"].Value.ToString() + "\\" + Request.Cookies["UserID"].Value.ToString() + ")";
                lblLogin.ToolTip = HttpUtility.UrlDecode(Request.Cookies["UnitName"].Value.ToString()) + "\\" + HttpUtility.UrlDecode(Request.Cookies["UserName"].Value.ToString());

                //於發生訊息/當班日誌點發生訊息/當班日誌，tour套用今日
                if (this.Page.Title == "自動訊息") Menu1.Items[0].NavigateUrl = "Msg/Auto.aspx?tour=";
                if (this.Page.Title == "當班日誌") Menu1.Items[1].NavigateUrl = "Diary/diary.aspx?tour=";

                //隱藏上下一班查詢列
                if (this.Page.Title != "當班日誌" & this.Page.Title != "自動訊息" & this.Page.Title != "手動訊息" & this.Page.Title != "例行訊息" & this.Page.Title != "環控訊息") tblHead.Rows[1].Visible = false;
            }
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

    protected void InitializeMaster(string c)   //改變主板版面
    {
        //主體背景色、標頭背景圖、標頭背景色、選單文字色
        //System.Web.UI.HtmlControls.HtmlGenericControl pMaster = (System.Web.UI.HtmlControls.HtmlGenericControl)this.Master.FindControl("pMaster");
        //Table tblHead = (Table)pMaster.FindControl("tblHead");
        //Menu menu1 = (Menu)tblHead.FindControl("Menu1");
        switch (c)
        {
            case "1":
                {
                    pMaster.Style.Add("background-color", "#E2ECFC");
                    tblHead.Rows[0].Cells[0].Style.Add("background", "url(../image/head-1.jpg)");
                    tblHead.Rows[0].Cells[0].Style.Add("background-color", "#7AAEFF");
                    Menu1.StaticMenuItemStyle.ForeColor = System.Drawing.Color.Blue;
                    break;
                }
            case "2":
                {
                    pMaster.Style.Add("background-color", "#FFE4CA");
                    tblHead.Rows[0].Cells[0].Style.Add("background", "url(../image/head-2.jpg)");
                    tblHead.Rows[0].Cells[0].Style.Add("background-color", "#FFA459");
                    Menu1.StaticMenuItemStyle.ForeColor = System.Drawing.Color.Brown;
                    break;
                }
            case "3":
                {
                    pMaster.Style.Add("background-color", "#E0E0E0");
                    tblHead.Rows[0].Cells[0].Style.Add("background", "url(../image/head-3.jpg)");
                    tblHead.Rows[0].Cells[0].Style.Add("background-color", "#0F0F0F");
                    Menu1.StaticMenuItemStyle.ForeColor = System.Drawing.Color.White;
                    break;
                }
        }
    }

    protected void SetOPs()   //設定OP
    {
        string tour = Session["tour"].ToString();
        string OPs = GetValue("Diary", "select [OPname] from [Sign] where [tour]='" + tour + "'");
        string[] opA = OPs.Trim().Split(' ');
        for (int i = 0; i < opA.GetLength(0); i++)
        {
            if (opA[i].Trim() != "")
            {
                string tour1 = GetValue("Diary", "select max([tour]) from [Sign] where [tour]<'" + tour + "' and [OPname] like '%" + opA[i] + "%'"); if (tour1 == "") tour1 = tour;
                string tour2 = GetValue("Diary", "select min([tour]) from [Sign] where [tour]>'" + tour + "' and [OPname] like '%" + opA[i] + "%'"); if (tour2 == "") tour2 = tour;
                Response.Write("<u><font color=\"brown\" style=\"cursor:pointer\" onClick=\"OPClick('" + tour1 + "','" + tour2 + "',event);\" title=\"click:至該op上一班  alt+click:至該op下一班\">" + opA[i] + "</font></u>&nbsp;&nbsp;");
            }
        }
    }

    protected void SetWeekSeason(string tour)   //設定節氣
    {
        if (tour != "")
        {
            string week = DateTime.Parse(tour.Substring(0, 4) + "/" + tour.Substring(4, 2) + "/" + tour.Substring(6, 2)).DayOfWeek.ToString();
            switch (week)
            {
                case "Sunday": lblWeek.Text = "星期日"; break;
                case "Monday": lblWeek.Text = "星期一"; break;
                case "Tuesday": lblWeek.Text = "星期二"; break;
                case "Wednesday": lblWeek.Text = "星期三"; break;
                case "Thursday": lblWeek.Text = "星期四"; break;
                case "Friday": lblWeek.Text = "星期五"; break;
                case "Saturday": lblWeek.Text = "星期六"; break;
                default: lblWeek.Text = week; break;
            }

            lblSeason.Text = GetValue("Diary", "select [Item] from [Config] where [Kind]='24節氣' and ',' + [Config] + ',' like '%," + tour.Substring(4, 4) + ",%'");
            lblSeason.ToolTip = GetValue("Diary", "select [Memo] from [Config] where [Kind]='24節氣' and ',' + [Config] + ',' like '%," + tour.Substring(4, 4) + ",%'");
        }
    }

    protected string GetOpener()
    {
        string opener = "../Diary/diary.aspx";        

        switch (this.Page.Title)
        {
            case "自動訊息": opener = "../Msg/auto.aspx"; break;
            case "手動訊息": opener = "../Msg/Manu.aspx"; break;
            case "例行訊息": opener = "../Msg/Rout.aspx"; break;
            case "環控訊息": opener = "../Msg/Ems.aspx"; break;
        }

        return (opener);
    }

    protected void BtnSearch_Click(object sender, EventArgs e)  //日期查詢
    {
        Session["tour"] = SelYYYY.SelectedValue + SelMM.SelectedValue + SelDD.SelectedValue + SelClass.SelectedValue;
        Response.Redirect(GetOpener());
    }

    protected void BtnPrev_Click(object sender, EventArgs e)  //上一班
    {
        Session["tour"] = GetTour(SelYYYY.SelectedValue + SelMM.SelectedValue + SelDD.SelectedValue + SelClass.SelectedValue, -1);
        Response.Redirect(GetOpener());
    }

    protected void BtnNext_Click(object sender, EventArgs e)  //下一班
    {
        Session["tour"] = GetTour(SelYYYY.SelectedValue + SelMM.SelectedValue + SelDD.SelectedValue + SelClass.SelectedValue, +1);
        Response.Redirect(GetOpener());
    }

    protected void AddMenu(int idx1, int idx2)
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("Select * from [Config] where [Kind]='系統代碼' order by [Mark],[Config]", Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        string tmp = ""; MenuItem tmpItem = new MenuItem();
        while (dr.Read())
        {
            
            if (tmp != dr[3].ToString())
            {
                tmpItem = new MenuItem();
                tmpItem.Text = "【" + dr[4].ToString().Substring(0, 2) + "】";
                tmpItem.Selectable = false;
                Menu1.Items[idx1].ChildItems[idx2].ChildItems.Add(tmpItem);
            }            
            tmp = dr[3].ToString();

            MenuItem itemx = new MenuItem();
            itemx.Text = dr[1].ToString();
            itemx.ToolTip = "當班" + dr[1].ToString() + "(" + dr[2].ToString() + ")手動訊息";
            itemx.NavigateUrl = "Msg/Manu.aspx?tour=&SysCode=" + dr[2].ToString();
            tmpItem.ChildItems.Add(itemx);
        }
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();
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

    protected void MenuMt_MenuItemClick(Object sender, MenuEventArgs e)   //導入人員
    {
        if (e.Item.Text == "訊息設定") AddMsg("<script>alert('" + e.Item.ToolTip + "');</script>");
    }
}
