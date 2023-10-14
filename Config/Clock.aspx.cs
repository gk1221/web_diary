using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Config_Clock : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //產生各種年月日下拉式選單  
            string DbYYYY = DateTime.Now.Year.ToString();
            AddSel(SelYYYY, int.Parse(DbYYYY), int.Parse(DbYYYY) + 1); 
            AddSel(SelMM, 1, 12); 
            AddSel(SelDD, 1, 31);
            AddSel(SelHH, 0, 23);
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

    protected void Page_PreRenderComplete(object sender, EventArgs e)   //放Page_Load無法顯示
    {
        string SQL = "";
        if (ViewState["SQL"] != null)
        {
            SQL = ViewState["SQL"].ToString();
            SqlDataSourceClock.SelectCommand = SQL;
            GridView1.DataBind();
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

    protected void BtnAdd_Click(object sender, EventArgs e)  //訊息新增
    {
        string MsgCode = txtMsgCode.Text.Trim(); if (MsgCode == "") MsgCode = "Please.";
        string WorkYN = "N"; if (ChkWork.Checked) WorkYN = "Y";
        string BeepYN = "N"; if (ChkBeep.Checked) BeepYN = "Y";
        string RoutYN = "N"; if (ChkRout.Checked) RoutYN = "Y";

        string DD = ""; 
        if (rdoDD.Checked) DD = SelDD.SelectedValue;
        else if (rdoWeek.Checked) DD = SelWeek.SelectedValue;
        else DD = SelOP.SelectedValue;        
        
        if (Request.Cookies["UnitID"].Value != "SOS" | Request.Cookies["UserID"].Value.ToLower() == "operator")
            AddMsg("<script>alert('您沒有設定小鬧鐘的權限！" + "');</script>");
        else if (txtMsgCode.Text.Length != 7)
            AddMsg("<script>alert('訊息代碼格式錯誤！');</script>");
        else if (MsgCode != "Please." & GetValue("Diary", "select [SopCode] from [SopLog] where [SopCode]='" + MsgCode + "'") == "")
            AddMsg("<script>alert('訊息設定無此代碼(" + MsgCode + ")，請先新增代碼再設定小鬧鐘！');</script>");
        else
        {
            string Serial = GetValue("Diary", "select max([Serial])+1 from [Clock]"); //主鍵序號
            
            string SQL = "insert into [Clock] values(" + Serial + ",'" + SelYYYY.SelectedValue.Substring(2, 2) + "','" + SelMM.SelectedValue + "','" + DD + "','"
                + SelHH.SelectedValue + "','" + SelMI.SelectedValue + "','" + txtMsgCode.Text + "','" + txtMsgText.Text
                + "','" + WorkYN + "','" + BeepYN + "','" + RoutYN + "')";
            ExecDbSQL(SQL);
            InsLifeLog(SQL, Serial);
            GridView1.DataBind();

            for (int i = 1; i < GridView1.Rows.Count; i++) if (GridView1.Rows[i].Cells[0].Text == Serial) GridView1.SelectRow(i);
        }
    }

    protected void BtnSave_Click(object sender, EventArgs e)  //訊息修改
    {
        string Serial = GridView1.SelectedRow.Cells[0].Text;
        string MsgCode = txtMsgCode.Text.Trim(); if (MsgCode == "") MsgCode = "Please.";
        string WorkYN = "N"; if (ChkWork.Checked) WorkYN = "Y";
        string BeepYN = "N"; if (ChkBeep.Checked) BeepYN = "Y";
        string RoutYN = "N"; if (ChkRout.Checked) RoutYN = "Y";
        
        string DD = ""; 
        if (rdoDD.Checked) DD = SelDD.SelectedValue;
        else if (rdoWeek.Checked) DD = SelWeek.SelectedValue;
        else DD = SelOP.SelectedValue;

        if (Request.Cookies["UnitID"].Value != "SOS" | Request.Cookies["UserID"].Value.ToLower() == "operator")
            AddMsg("<script>alert('您沒有設定小鬧鐘的權限！" + "');</script>");
        else if (txtMsgCode.Text.Length != 7)
            AddMsg("<script>alert('訊息代碼格式錯誤！');</script>");
        else if (MsgCode != "Please." & GetValue("Diary", "select [SopCode] from [SopLog] where [SopCode]='" + MsgCode + "'") == "")
            AddMsg("<script>alert('訊息設定無此代碼(" + MsgCode + ")，請先新增代碼再設定小鬧鐘！');</script>");
        else
        {
            string SQL = "update [Clock] set [YY]='" + SelYYYY.SelectedValue.Substring(2, 2) + "',[MM]='" + SelMM.SelectedValue + "',[DD]='" + DD + "',"
                + "[HH]='" + SelHH.SelectedValue + "',[MI]='" + SelMI.SelectedValue + "',[MsgCode]='" + txtMsgCode.Text + "',[MsgText]='" + txtMsgText.Text + "',"
                + "[WorkYN]='" + WorkYN + "',[BeepYN]='" + BeepYN + "',[RoutYN]='" + RoutYN + "' where [Serial]='" + Serial + "'";
            ExecDbSQL(SQL);
            InsLifeLog(SQL, Serial);
            GridView1.DataBind();            
        }
    }

    protected void BtnDel_Click(object sender, EventArgs e)  //訊息刪除
    {
        string Serial = GridView1.SelectedRow.Cells[0].Text;
        
        if (Request.Cookies["UnitID"].Value != "SOS" | Request.Cookies["UserID"].Value.ToLower() == "operator")
            AddMsg("<script>alert('您沒有設定小鬧鐘的權限！" + "');</script>");
        else
        {
            string SQL = "delete from [Clock] where [Serial]='" + Serial + "'";
            ExecDbSQL(SQL);
            InsLifeLog(SQL, Serial);
            GridView1.DataBind();

            GridView1.SelectRow(-1);
            txtMsgCode.Text = ""; txtMsgText.Text = "";
            SelYYYY.SelectedIndex = 0; SelMM.SelectedIndex = 0; SelDD.SelectedIndex = 0; SelHH.SelectedIndex = 0; SelMI.SelectedIndex = 0;
            SelOP.SelectedIndex = 0; SelWeek.SelectedIndex = 0;
            ChkBeep.Checked = false; ChkRout.Checked = false; ChkWork.Checked = false;
        }
    }

    protected void InsLifeLog(string Life,string PkNo) //寫入生命履歷
    {
        string LifeNo = (int.Parse(GetValue("Diary", "select max([記錄編號]) from [異動記錄]")) + 1).ToString(); //履歷編號
        if (LifeNo == "") LifeNo = "1";
        string TblName = "小鬧鐘";    //表格名稱
        string UN = GetValue("Diary", "select [成員] from [View_組織架構] where [性質]='員工' and [代號]='" + Request.Cookies["UserID"].Value + "'");   //登入的UserName：異動人員
        string LiftDT = DateTime.Now.ToString("yyyy/MM/dd HH:mm");  //異動日期
        string LifeIP = Request.ServerVariables["REMOTE_ADDR"].ToString();

        ExecDbSQL("Insert into [異動記錄] values(" + LifeNo + ",'" + TblName + "','" + PkNo + "','" + Life.Replace("'","") + "','" + UN + "','" + LiftDT + "','" + LifeIP + "')");
    }

    protected void GridView1_SelectedIndexChanged(Object sender, EventArgs e)   //選取列
    {
        if (GridView1.SelectedRow != null)
        {
            BtnSave.Enabled = true;
            BtnDel.Enabled = true;

            SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
            Conn.Open();
            SqlCommand cmd = new SqlCommand("select * from [Clock] where [Serial]=" + GridView1.SelectedRow.Cells[0].Text, Conn);
            SqlDataReader dr = null;
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                txtMsgCode.Text = dr["MsgCode"].ToString();
                txtMsgText.Text = dr["MsgText"].ToString();

                for (int i = 0; i < SelYYYY.Items.Count; i++) if (SelYYYY.Items[i].Value == "20" + dr["YY"].ToString()) SelYYYY.SelectedIndex = i;
                for (int i = 0; i < SelMM.Items.Count; i++) if (SelMM.Items[i].Value == dr["MM"].ToString()) SelMM.SelectedIndex = i;
                for (int i = 0; i < SelDD.Items.Count; i++) if (SelDD.Items[i].Value == dr["DD"].ToString()) SelDD.SelectedIndex = i;
                for (int i = 0; i < SelWeek.Items.Count; i++) if (SelWeek.Items[i].Value == dr["DD"].ToString()) SelWeek.SelectedIndex = i;
                for (int i = 0; i < SelOP.Items.Count; i++) if (SelOP.Items[i].Value == dr["DD"].ToString()) SelOP.SelectedIndex = i;
                for (int i = 0; i < SelHH.Items.Count; i++) if (SelHH.Items[i].Value == dr["HH"].ToString()) SelHH.SelectedIndex = i;
                for (int i = 0; i < SelMI.Items.Count; i++) if (SelMI.Items[i].Value == dr["MI"].ToString()) SelMI.SelectedIndex = i;

                int n;
                if (dr["DD"].ToString().Substring(0, 1) != "*" | dr["DD"].ToString() == "**")
                {
                    rdoDD.Checked = true;
                    rdoDD_CheckedChanged(null, null);
                }
                else if (int.TryParse(dr["DD"].ToString().Substring(1, 1), out n))
                {
                    rdoWeek.Checked = true;
                    rdoWeek_CheckedChanged(null, null);
                }
                else
                {
                    rdoOP.Checked = true;
                    rdoOP_CheckedChanged(null, null);
                }


                if (dr["WorkYN"].ToString() == "Y") ChkWork.Checked = true;
                else ChkWork.Checked = false;
                if (dr["BeepYN"].ToString() == "Y") ChkBeep.Checked = true;
                else ChkBeep.Checked = false;
                if (dr["RoutYN"].ToString() == "Y") ChkRout.Checked = true;
                else ChkRout.Checked = false;
            }
            cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();
        }
        else
        {
            BtnSave.Enabled = false;
            BtnDel.Enabled = false;
        }
    }

    protected void rdoDD_CheckedChanged(Object sender, EventArgs e)
    {
        if (rdoDD.Checked)
        {
            rdoWeek.Checked = false;
            rdoOP.Checked = false;
        }
    }
    protected void rdoWeek_CheckedChanged(Object sender, EventArgs e)
    {
        if (rdoWeek.Checked)
        {
            rdoDD.Checked = false;
            rdoOP.Checked = false;
        }
    }
    protected void rdoOP_CheckedChanged(Object sender, EventArgs e)
    {
        if (rdoOP.Checked)
        {
            rdoDD.Checked = false;
            rdoWeek.Checked = false;
        }
    }

    protected void SelDD_OnSelectedIndexChanged(Object sender, EventArgs e)
    {
        rdoDD.Checked = true;
        rdoWeek.Checked = false;
        rdoOP.Checked = false;        
    }
    protected void SelWeek_OnSelectedIndexChanged(Object sender, EventArgs e)
    {
        rdoDD.Checked = false;
        rdoWeek.Checked = true;
        rdoOP.Checked = false;
    }
    protected void SelOP_OnSelectedIndexChanged(Object sender, EventArgs e)
    {
        rdoDD.Checked = false;
        rdoWeek.Checked = false;
        rdoOP.Checked = true;
    }
}