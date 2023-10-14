using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Config_LifeLog : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Timeout = 720;
        if (!IsPostBack)
        {
            string x = "";

            ListItem ItemY = new ListItem(); ItemY.Text = ""; ItemY.Value = ""; SelYYYY.Items.Add(ItemY);
            int YYYY = int.Parse((DateTime.Now).ToString("yyyy"));
            for (int i = YYYY; i >= YYYY - 5; i--)   //產生異動日期年之選項
            {
                x = i.ToString(); if (i < 10) x = "0" + x;
                ListItem ItemX = new ListItem(); ItemX.Text = x; ItemX.Value = x; if (i == YYYY) ItemX.Selected = true;
                SelYYYY.Items.Add(ItemX);
            }

            ListItem ItemM = new ListItem(); ItemM.Text = ""; ItemM.Value = ""; SelMM.Items.Add(ItemM);
            int MM = int.Parse((DateTime.Now).ToString("MM"));
            for (int i = 1; i <= 12; i++)   //產生異動日期月之選項
            {
                x = i.ToString(); if (i < 10) x = "0" + x;
                ListItem ItemX = new ListItem(); ItemX.Text = x; ItemX.Value = x; if (i == MM) ItemX.Selected = true;
                SelMM.Items.Add(ItemX);
            }

            ListItem ItemD = new ListItem(); ItemD.Text = ""; ItemD.Value = ""; SelDD.Items.Add(ItemD);
            int DD = int.Parse(DateTime.Now.ToString("dd"));
            for (int i = 1; i <= 31; i++)   //產生異動日期月之選項
            {
                x = i.ToString(); if (i < 10) x = "0" + x;
                ListItem ItemX = new ListItem(); ItemX.Text = x; ItemX.Value = x;
                SelDD.Items.Add(ItemX);
            }

            if (Request["Search"] != null)  //外部查詢
            {
                if (Session["LifeSQL"] != null)
                {
                    ViewState["SQL"] = Session["LifeSQL"];
                    SelYYYY.SelectedIndex = 0; SelMM.SelectedIndex = 0;
                    for (int i = 0; i < SelTbl.Items.Count; i++) if (SelTbl.Items[i].Value == Request["Tbl"].ToString()) SelTbl.SelectedIndex = i;
                    TextPK.Text = Request["PK"].ToString();
                }
            }
            else
            {
                ViewState["SQL"] = "select * from [異動記錄] where [異動日期]>='" + DateTime.Now.ToString("yyyy/MM/01 00:00:00") + "' order by [記錄編號] desc";
            }
            SqlDataSource3.SelectCommand = ViewState["SQL"].ToString();
            GridView1.DataBind();
        }
    }

    protected void Page_PreRenderComplete(object sender, EventArgs e)   //放Page_Load無法顯示
    {
        SqlDataSource3.SelectCommand = ViewState["SQL"].ToString(); //查詢後，SQL要保持住，否則會用預設值
    }

    protected void Button1_Click(object sender, EventArgs e)    //執行查詢
    {
        string SQL = "select * from [異動記錄] where 1=1";
        if (ChkMt.Checked) SQL = SQL + " and [異動人員]='" + SelMt.SelectedValue + "'";

        string YYYY1, YYYY2, MM1, MM2, DD1, DD2;
        if (SelYYYY.SelectedValue != "") { YYYY1 = SelYYYY.SelectedValue; YYYY2 = YYYY1; }
        else { YYYY1 = "1900"; YYYY2 = "9999"; }
        if (SelMM.SelectedValue != "") { MM1 = SelMM.SelectedValue; MM2 = MM1; }
        else { MM1 = "01"; MM2 = "12"; }
        if (SelDD.SelectedValue != "") { DD1 = SelDD.SelectedValue; DD2 = DD1; }
        else { DD1 = "01"; DD2 = DateTime.DaysInMonth(int.Parse(YYYY2), int.Parse(MM2)).ToString(); }

        SQL = SQL + " and [異動日期] between '" + YYYY1 + "/" + MM1 + "/" + DD1 + " 00:00:00' and '" + YYYY2 + "/" + MM2 + "/" + DD2 + " 23:59:59'";

        if (SelTbl.SelectedValue != "") SQL = SQL + " and [表格名稱]='" + SelTbl.SelectedValue + "'";


        if (TextPK.Text != "") SQL = SQL + " and [主鍵編號]='" + TextPK.Text + "'";

        if (TextLife.Text != "")
        {
            string[] KeyA = TextLife.Text.Split(',');
            for (int i = 0; i < KeyA.GetLength(0); i++) SQL = SQL + " and ([異動記錄] like '%" + KeyA[i] + "%' or [主鍵編號] like '%" + KeyA[i] + "%')";
        }

        SQL = SQL + " order by [記錄編號] desc";

        ViewState["SQL"] = SQL;
        //Response.Write(SQL);
        //Response.End();
        SqlDataSource3.SelectCommand = SQL;

        GridView1.AllowPaging = ChkPage.Checked;
        GridView1.DataBind();
    }

    protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)    //選取後就另開編輯視窗或帶入設備編號
    {
        Literal Msg = new Literal();
        string LifeNo = GridView1.Rows[e.NewSelectedIndex].Cells[1].Text;

        if (TextLife.Text != "#delete#")
        {
            string tblName = GridView1.Rows[e.NewSelectedIndex].Cells[2].Text;
            string PKno = GridView1.Rows[e.NewSelectedIndex].Cells[3].Text;
            
            switch (tblName)
            {
                //case "系統參數": OpenExecWindow("window.open('Config.aspx','_blank');"); break;
                //case "訊息設定": OpenExecWindow("window.open('../Msg/Manu.aspx?SysCode=" + PKno.Substring(1, 3) + "','_blank');"); break;
                //case "小鬧鐘": OpenExecWindow("window.open('Clock.aspx','_blank');"); break;
                case "異常處理": OpenExecWindow("window.open('/sop/sop.asp?SEL_Code=" + PKno.Substring(0, 7) + "','_blank');"); break;
                case "處理過程": OpenExecWindow("window.open('../Diary/Process.aspx?qryTour=" + PKno.Substring(0, 9) + "&ProcessNo=" + PKno.Substring(10) + "','_blank');"); break;
                default: Msg.Text = "<script>alert('系統設定未設計直接帶出資料機制，請自行至該介面查詢!');</script>"; break;
            }   
        }
        else
        {
            SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
            Conn.Open();
            SqlCommand cmd = new SqlCommand("delete from [異動記錄] where [記錄編號]=" + LifeNo, Conn);
            cmd.ExecuteNonQuery();
            cmd.Cancel(); cmd.Dispose(); Conn.Close(); Conn.Dispose();
        }

        if (Msg.Text != "") Page.Controls.Add(Msg);
    }

    protected void OpenExecWindow(string strJavascript)    //選取後就另開視窗
    {
        if (ScriptManager.GetCurrent(Page) == null)
        {
            Page.ClientScript.RegisterStartupScript(Page.GetType(), "LifeLog", strJavascript, true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "LifeLog", strJavascript, true);
        }
    }
}