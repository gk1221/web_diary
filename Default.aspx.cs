using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data.SqlClient;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Cookies["UserID"] == null) ResponseLogin();
        else
        {
            string UserID = Request.Cookies["UserID"].Value.ToString();    //自SSM首頁取得
            if (UserID == "" ) ResponseLogin();
            else Response.Redirect("Diary/diary.aspx");            
        }
    }

    protected void ResponseLogin()
    {
        Response.Write("無法取得認證帳號，請先<a href='/' target='_top'>登入Windows網域</a>！若無法解決，請重啟瀏覽器。");
        Response.End();
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
}