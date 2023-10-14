using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Config_Config : System.Web.UI.Page
{
    string bgcolor = ""; 

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string UserID = "";
            try
            {
                UserID = Request.Cookies["UserID"].Value.ToString().ToUpper();

                ListBox1.DataSourceID = "SqlDataSource2";
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
        string Kind = Request.QueryString["kind"];
        if (Kind == "" | Kind == null) Kind = "工作日誌";
        lblKind.Text = GetValue("Diary", "select [Memo] from [Config] where [Kind]='" + Kind + "' and Item='" + DropDownList1.SelectedValue + "'").Replace("\r\n", "<br />");
    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        ListBox1.DataSourceID = "SqlDataSource2";
        ClearData();
        string Kind = Request.QueryString["kind"];
        if (Kind == "" | Kind == null) Kind = "工作日誌";
        lblKind.Text = GetValue("Diary", "select [Memo] from [Config] where [Kind]='" + Request.QueryString["kind"] + "' and Item='" + DropDownList1.SelectedValue + "'");
    }

    protected void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("select * from Config where Kind='" + DropDownList1.SelectedValue + "' and Item='" + ListBox1.SelectedValue + "'", Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            TextItem.Text = dr["Item"].ToString();
            TextConfig.Text = dr["Config"].ToString();
            TextMark.Text = dr["Mark"].ToString();
            TextMemo.Text = dr["Memo"].ToString();
        }
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();
    }

    protected Boolean HasData(string Kind, string Item)
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("select * from Config where Kind='" + Kind + "' and Item='" + Item + "'", Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        Boolean TF = false; if (dr.Read()) TF=true;
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        return (TF);
    }

    protected string GetValue(string DB, string SQL)   //取得單一資料
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[DB + "ConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand(SQL, Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();

        string cfg = ""; if (dr.Read()) cfg=dr[0].ToString();
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        return(cfg);
    }

    protected string GetValue(string DB, string SQL, string key, string value)   //取得單一資料
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[DB + "ConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand(SQL, Conn);
        cmd.Parameters.AddWithValue(key, value);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();

        string cfg = ""; if (dr.Read()) cfg=dr[0].ToString();
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        return(cfg);
    }

    protected void InsLifeLog(string SQL) //寫入異動記錄
    {
        string LifeNo = GetPKNo("記錄編號", "異動記錄").ToString(); //履歷編號
        string TblName = "系統參數";    //表格名稱
        string PKno = "0";   //主鍵編號
        string UserID = Request.Cookies["UserID"].Value.ToUpper();
        string UserName = GetValue("Diary", "select [成員] from [View_組織架構] where [性質]='員工' and [代號]='" + UserID + "'");
        string LifeDT = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");  //異動日期
        string LifeIP = Request.ServerVariables["REMOTE_ADDR"].ToString();

        ExecDbSQL("Insert into [異動記錄] values(" + LifeNo + ",'" + TblName + "','" + PKno + "','" + SQL.Replace("'", "''") + "','" + UserName + "','" + LifeDT + "','" + LifeIP + "')");
    }

    protected int GetPKNo(string PKfield, string PKtbl) //取得主鍵編號
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("select max(" + PKfield + ") from " + PKtbl, Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        int PkNo = 1; if (dr.Read()) PkNo=int.Parse(dr[0].ToString()) + 1;
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        return (PkNo);
    }

    protected void ExecDbSQL(string SQL) //執行資料庫異動
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand(SQL, Conn);
        cmd.ExecuteNonQuery();
        cmd.Cancel(); cmd.Dispose(); Conn.Close(); Conn.Dispose();
    }

    protected void BtnAdd_Click(object sender, EventArgs e)
    {
        Literal Msg = new Literal();

        if (!RightCheck()) Msg.Text = "<script>alert('您沒有權限異動資料，若有需求，請洽機房！');</script>";
        else
        {
            if (HasData(DropDownList1.SelectedValue, TextItem.Text))
            {
                Msg.Text = "<script>alert('該筆資料已存在，無法再新增！');</script>";
            }
            else
            {
                string SQL = "insert into Config values('" + DropDownList1.SelectedValue + "','" + TextItem.Text + "','" + TextConfig.Text + "','" + TextMark.Text + "','" + TextMemo.Text.Replace("'", "''") + "')";

                SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
                Conn.Open();
                SqlCommand cmd = new SqlCommand(SQL, Conn);
                cmd.ExecuteNonQuery();
                cmd.Cancel(); cmd.Dispose(); Conn.Close(); Conn.Dispose();

                ListBox1.ClearSelection();
                ListBox1.Items.Add(TextItem.Text);
                ListBox1.Items[ListBox1.Items.Count - 1].Selected = true;               

                Msg.Text = "<script>alert('資料已新增！');</script>";

                InsLifeLog(SQL);
            }
        }
        Page.Controls.Add(Msg);
    }

    protected void BtnEdit_Click(object sender, EventArgs e)
    {
        Literal Msg = new Literal();

        if (!RightCheck()) Msg.Text = "<script>alert('您沒有權限異動資料，若有需求，請洽機房！');</script>";
        else
        {
            if (ListBox1.SelectedValue != "")
            {
                InsLifeLog("修改 [" + DropDownList1.SelectedValue + "." + ListBox1.SelectedValue + "]  ：： " + GetUpdate("Life"));
                ExecDbSQL("Update [Config] set " + GetUpdate("SQL") + " where Kind='" + DropDownList1.SelectedValue + "' and Item='" + ListBox1.SelectedValue + "'");

                Msg.Text = "<script>alert('更新資料 [" + DropDownList1.SelectedValue + "." + ListBox1.SelectedValue + "] 完成！');</script>";
            }
            else
            {
                Msg.Text = "<script>alert('您尚未點選欲修改之資料！');</script>";
            }
        }
        Page.Controls.Add(Msg);
    }

    protected string GetUpdate(string SQLorLife) //取得修改資料的SQL語法
    {
        string SQL = "";
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("select * from [Config] where [Kind]='" + DropDownList1.SelectedValue + "' and [Item]='" + ListBox1.SelectedValue + "'", Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            SQL = GetUpdateCol("Item", dr["Item"].ToString(), TextItem.Text, "string", SQLorLife)
                + GetUpdateCol("Config", dr["Config"].ToString(), TextConfig.Text, "string", SQLorLife)
                + GetUpdateCol("Mark", dr["Mark"].ToString(), TextMark.Text, "string", SQLorLife)
                + GetUpdateCol("Memo", dr["Memo"].ToString(), TextMemo.Text.Replace("'", "''"), "string", SQLorLife);                

            if (SQL != "") SQL = SQL.Substring(1);
            else SQL = "Kind = Kind ";
        }
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        return SQL;
    }

    protected string GetUpdateCol(string ColName, string Source, string Target, string Kind, string SQLorLife) //取得單一欄位修改資料的語法
    {
        string SQL = "";
        if (Kind == "date" & Target.Length != 10 | Kind == "datetime" & Target.Length != 16)
        {
            Kind = "null"; Target = "null";
        }

        if (Source != Target)
        {
            if (SQLorLife == "SQL")
            {
                switch (Kind)
                {
                    case "string":
                    case "date":
                    case "datetime": SQL = SQL + ",[" + ColName + "]='" + Target + "'"; break;
                    case "integer":
                    case "money": SQL = SQL + ",[" + ColName + "]=" + Target; break;
                    case "null": SQL = SQL + ",[" + ColName + "]=" + null; break;
                    default: SQL = SQL + ",[" + ColName + "]='" + Target + "'"; break;
                }
            }
            else if (SQLorLife == "Life")
            {
                if (Source == "") Source = "(空白)";
                if (Target == "") Target = "(空白)";
                SQL = SQL + ",[" + ColName + "]：" + Source + " -> " + Target;
            }
        }
        return (SQL);
    }

    protected void BtnDel_Click(object sender, EventArgs e)
    {
        Literal Msg = new Literal();

        if (!RightCheck()) Msg.Text = "<script>alert('您沒有權限異動資料，若有需求，請洽機房！');</script>";
        else
        {
            string SQL = "delete Config where Kind='" + DropDownList1.SelectedValue + "' and Item='" + ListBox1.SelectedValue + "'";
            
            SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
            Conn.Open();
            SqlCommand cmd = new SqlCommand(SQL, Conn);
            cmd.ExecuteNonQuery();
            cmd.Cancel(); cmd.Dispose(); Conn.Close(); Conn.Dispose();
            
            InsLifeLog(SQL + ",[Config]='" + TextConfig.Text + "',[Mark]='" + TextMark.Text);

            ListBox1.Items.Remove(ListBox1.SelectedItem);
            ClearData();
            Msg.Text = "<script>alert('刪除資料完成！');</script>";
        }
        Page.Controls.Add(Msg);
    }

    protected void ClearData()
    {
        TextItem.Text = "";
        TextConfig.Text = "";
        TextMark.Text = "";
        TextMemo.Text = "";
    }

    protected Boolean RightCheck() //是否有權修改資料
    {
        string UserID = Request.Cookies["UserID"].Value.ToUpper();        
        string UnitName = GetValue("Diary", "select [課別] from [View_組織架構] where [性質]='員工' and [代號]= @user", "@user", UserID);        
        
        if (UserID != "OPERATOR" & UnitName == "作業管理科") return (true);
        else return (false);
    }

    protected void BtnSearch_Click(object sender, EventArgs e)  //關鍵字查詢
    {
        string[] KeyA = TextKey.Text.Trim().Split(',');
        string SQL = "";

        for (int i = 0; i < KeyA.GetLength(0); i++)
        {
            if (i > 0) SQL = SQL + " and ";

            SQL = SQL + "[Kind]+','+[Item]+','+[Config]+','+[Mark]+','+[Memo] like '%" + KeyA[i] + "%'";
        }
        
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("select * from [Config] where " + SQL, Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        lblKey.Text = "";
        while (dr.Read())
        {
            lblKey.Text = lblKey.Text + dr[0].ToString() + " " + dr[1].ToString() + " " + dr[2].ToString() + " " + dr[3].ToString() + " " + dr[4].ToString() + "<br/>";
        }
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();
    }

    protected void OpenExecWindow(string strJavascript)    //選取後就另開視窗
    {
        if (ScriptManager.GetCurrent(Page) == null)
        {
            Page.ClientScript.RegisterStartupScript(Page.GetType(), "Config", strJavascript, true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Config", strJavascript, true);
        }
    }

    protected void BtnDn_Click(object sender, EventArgs e)  //下探
    {
        Literal Msg = new Literal();

        if (DropDownList1.SelectedValue == "")
        {
            Msg.Text = "<script>alert('請先選擇下拉式選單要下探的項目！');</script>";
            Page.Controls.Add(Msg);
        }
        else
        {
            if (ListBox1.Items.Count==0)
            {
                Msg.Text = "<script>alert('已經是最下層！');</script>";
                Page.Controls.Add(Msg);
            }
            else OpenExecWindow("window.open('Config.aspx?Kind=" + DropDownList1.SelectedValue + "&bgColor=" + bgcolor + "','_self');");
        }
    }
    protected void BtnUp_Click(object sender, EventArgs e)  //上提
    {
        string Kind = GetValue("Diary", "select [Kind] from [Config] where [Item]<>'' and [Item] in (select [Kind] from [Config] where [Item]='" + DropDownList1.SelectedValue + "')");

        if (Kind == "")
        {
            Literal Msg = new Literal();
            Msg.Text = "<script>alert('已經是最上層！');</script>";
            Page.Controls.Add(Msg);
        }
        else
            OpenExecWindow("window.open('Config.aspx?Kind=" + Kind + "&bgColor=" + bgcolor + "','_self');");
    }
}