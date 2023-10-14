using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Search_Count : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (ViewState["SQL"] != null) SqlDataSource1.SelectCommand = ViewState["SQL"].ToString();

        if (!IsPostBack)
        {
            string DbYYYY = GetValue("select [Config] from [Config] where [Kind]='統計參數' and [Item]='起始年份'");            
            string SQL = "select * from [View_存檔統計] where [YYYY]>='" + DbYYYY + "' order by [YYYY] desc,[總計] desc";
            
            SqlDataSource1.SelectCommand = SQL;
            GridView1.DataBind();

            ViewState["SQL"] = SQL;
        }
    }

    protected string GetValue(string SQL)   //取得單一資料
    {
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand(SQL, Conn);
        SqlDataReader dr = null;
        dr = cmd.ExecuteReader();
        string cfg = ""; if (dr.Read()) cfg = dr[0].ToString();
        cmd.Cancel(); cmd.Dispose(); dr.Close(); Conn.Close(); Conn.Dispose();

        return (cfg);
    }
}