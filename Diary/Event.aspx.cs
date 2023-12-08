using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Diary_Event : System.Web.UI.Page
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
     
		
    }
}