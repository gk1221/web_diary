using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

public partial class Diary_Event : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //取得起始班別，並記錄狀態資訊
            string tour = Request["qryTour"].ToString();
            string process = Request["ProcessNo"].ToString();

            string c = tour.Substring(8, 1);
            lblTour.Text = tour.Substring(0, 4) + " 年 " + tour.Substring(4, 2) + " 月 " + tour.Substring(6, 2) + " 日 ";


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

            //lblTour.Text += " 日誌" + process;



            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("tour", tour));
            parameters.Add(new SqlParameter("process", process));

            Dictionary<string, string> dict = GetValue(@"SELECT convert(varchar, StartDate, 120) as sdate 
                    ,convert(varchar, enddate, 120) as edate, event FROM Event WHERE tour=@tour AND processno=@process", parameters);

            if (dict == null) action.Text = "新增活動";
            else
            {
                action.Text = "修改活動";
                sdate.Text = dict["sdate"].ToString();
                edate.Text = dict["edate"].ToString();
                eventbox.Text = dict["event"].ToString();
            }
        }
    }

    protected void Page_PreRenderComplete(object sender, EventArgs e)   //放Page_Load無法顯示
    {

    }

    protected Dictionary<string, string> GetValue(string SQL, List<SqlParameter> pars)   //取得單一資料
    {

        using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(SQL, Conn))
            {
                cmd.Parameters.AddRange(pars.ToArray());
                Conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    Dictionary<string, string> ans = new Dictionary<string, string>();
                    ans.Add("event", dr["event"].ToString());
                    ans.Add("sdate", dr["sdate"].ToString());
                    ans.Add("edate", dr["edate"].ToString());
                    return ans;
                }
            }
        }
        return null;
    }

    protected void Btnsave_click(object sender, EventArgs e)
    {
        if (action.Text == "新增活動") Event_create();
        else Event_update();
    }

    protected void Event_create()
    {
        string tour = Request["qryTour"].ToString();
        string process = Request["ProcessNo"].ToString();
        string eventcontent = eventbox.Text;
        string start = sdate.Text;
        string enddate = edate.Text;
        string outwaring = ErrorCheck();

        string SQL = "INSERT INTO [dbo].[event] VALUES (@tour, @process, @eventcontent, @startdate, @enddate)";

        Literal msg = new Literal();

        using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(SQL, Conn))
            {
                try
                {
                    if (outwaring != "") throw new Exception();
                    List<SqlParameter> pars = new List<SqlParameter>();
                    pars.Add(new SqlParameter("tour", tour));
                    pars.Add(new SqlParameter("process", process));
                    pars.Add(new SqlParameter("eventcontent", eventcontent));
                    pars.Add(new SqlParameter("startdate", start));
                    pars.Add(new SqlParameter("enddate", enddate));
                    cmd.Parameters.AddRange(pars.ToArray());
                    Conn.Open();
                    cmd.ExecuteNonQuery();

                    action.Text = "修改活動";

                    msg.Text = "<script>alert('新增成功');</script>";
                }
                catch (System.Exception e)
                {
                    Trace.Warn(e.ToString());

                    msg.Text = "<script>alert('新增失敗，請再試一次\\n" + outwaring + "');</script>";
                }

            }
        }

        Page.Controls.Add(msg);
    }

    protected void Event_update()
    {
        string tour = Request["qryTour"].ToString();
        string process = Request["ProcessNo"].ToString();
        string eventcontent = eventbox.Text;
        string start = sdate.Text;
        string enddate = edate.Text;
        string outwaring = ErrorCheck();
        Dictionary<string, string> ans = new Dictionary<string, string>();
        string upSQL = " SET ";

        Literal msg = new Literal();

        using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand("", Conn))
            {
                try
                {
                    if (outwaring != "") throw new Exception();
                    Conn.Open();
                    upSQL += " event=@event,   startDate=@sdate,   endDate=@edate ";

                    if (upSQL != "")
                    {
                        upSQL = "UPDATE event " + upSQL + " WHERE tour=@tour AND processno=@process";
                        cmd.CommandText = upSQL;
                        cmd.Parameters.AddWithValue("event", eventcontent);
                        cmd.Parameters.AddWithValue("sdate", start);
                        cmd.Parameters.AddWithValue("edate", enddate);
                        cmd.Parameters.AddWithValue("tour", tour);
                        cmd.Parameters.AddWithValue("process", process);

                        cmd.ExecuteNonQuery();

                        msg.Text = "<script>alert('修改成功');</script>";
                    }
                    else
                    {
                        msg.Text = "<script>alert('無修改內容');</script>";
                    }
                }
                catch (System.Exception e)
                {
                    Trace.Warn(e.ToString());


                    msg.Text = "<script>alert('修改失敗，請再試一次\\n" + outwaring + "');</script>";
                }
            }
        }
        Page.Controls.Add(msg);
    }
    protected string ErrorCheck()
    {
        string outwaring = "";
        int result = DateTime.Compare(DateTime.Parse(sdate.Text), DateTime.Parse(edate.Text));
        Trace.Warn(result.ToString());

        if (eventbox.Text.Length > 100) outwaring += "\\n活動內容太長，請縮短至100字內(目前" + eventbox.Text.Length + "個字)!";
        if (result > 0) outwaring += "\\n起始日期不得小於結束日期!";

        return outwaring;
    }

    protected void BtnDel_Click(object o, EventArgs e)
    {
        string tour = Request["qryTour"].ToString();
        string process = Request["ProcessNo"].ToString();

        Literal msg = new Literal();

        using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DiaryConnectionString"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand("", Conn))
            {
                try
                {
                    Conn.Open();
                    cmd.CommandText = "DELETE event WHERE tour=@tour AND processno=@process";
                    cmd.Parameters.AddWithValue("tour", tour);
                    cmd.Parameters.AddWithValue("process", process);
                    cmd.ExecuteNonQuery();
                    msg.Text = "<script>alert('刪除成功');window.close();</script>";

                }
                catch (System.Exception C)
                {
                    Trace.Warn(C.ToString());
                    msg.Text = "<script>alert('刪除失敗，請再試一次');</script>";
                }
            }
        }
        Page.Controls.Add(msg);
    }
}