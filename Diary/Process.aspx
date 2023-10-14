<%@ Page Title="處理過程" Language="C#" MasterPageFile="../MasterPage.master"
AutoEventWireup="true" validateRequest="false" Debug="true"
MaintainScrollPositionOnPostback="true" CodeFile="Process.aspx.cs"
Inherits="Diary_Process" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content
  ID="Content2"
  ContentPlaceHolderID="ContentPlaceHolder1"
  runat="Server"
>
  <!----------------標題--------------------------------------------------------------->
  <br />
  <div align="center">
    <table
      style="
        border-bottom: solid 1px gray;
        border-left: solid 1px gray;
        border-right: solid 1px gray;
        border-top: solid 1px gray;
      "
    >
      <tr>
        <td align="center" style="font-size: xx-large; font-family: 標楷體">
          處理過程登錄畫面
        </td>
      </tr>
      <tr>
        <td>&nbsp;</td>
      </tr>
      <!----------------處理時間--------------------------------------------------------------->
      <tr>
        <td align="left" style="font-size: large; font-family: 標楷體">
          &nbsp;處理時間:&nbsp;
          <asp:DropDownList ID="SelProYYYY" runat="server" ForeColor="Green">
          </asp:DropDownList
          >年
          <asp:DropDownList ID="SelProMM" runat="server" ForeColor="Green">
          </asp:DropDownList
          >月
          <asp:DropDownList ID="SelProDD" runat="server" ForeColor="Green">
          </asp:DropDownList
          >日&nbsp;&nbsp;
          <asp:DropDownList ID="SelProHH" runat="server" ForeColor="Green">
          </asp:DropDownList
          >時
          <asp:DropDownList ID="SelProMI" runat="server" ForeColor="Green">
          </asp:DropDownList
          >分 &nbsp;&nbsp; 記錄人員:
          <asp:DropDownList
            ID="SelOP"
            runat="server"
            ForeColor="Green"
            DataSourceID="SqlDataSourceOP"
            DataValueField="代號"
            DataTextField="成員"
          >
            <asp:ListItem></asp:ListItem>
          </asp:DropDownList>
          <asp:SqlDataSource
            ID="SqlDataSourceOP"
            runat="server"
            ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
            SelectCommand=""
          >
          </asp:SqlDataSource
          >&nbsp;&nbsp; 權責小組/負責人評分:
          <asp:TextBox
            ID="txtRt"
            runat="server"
            Width="10"
            ForeColor="Green"
            CssClass="style0"
          ></asp:TextBox>
          <asp:Button ID="BtnRate" runat="server" Text="評!" />

          <font
            color="blue"
            size="2"
            style="cursor: pointer"
            onclick="alert('於機房之安內或公用電腦新增記錄時，可改選其它記錄人員！\n\n但新增以後，唯記錄人員可修改記錄。\n\n非機房人員本選項無作用，請忽略之。');"
          >
            <u>說明</u>
          </font>
          <br />
        </td>
      </tr>
      <tr>
        <td>&nbsp;</td>
      </tr>

      <!----------------處理過程--------------------------------------------------------------->
      <tr>
        <td align="left">
          <table border="0" cellpadding="0" cellspacing="0">
            <tr>
              <td align="left" style="font-size: large; font-family: 標楷體">
                &nbsp;處理過程:
              </td>

              <td align="left" valign="middle">
                <asp:Menu
                  ID="MenuWord"
                  runat="server"
                  BackColor=""
                  DynamicHorizontalOffset="2"
                  StaticEnableDefaultPopOutImage="false"
                  MaximumDynamicDisplayLevels="3"
                  Font-Names="標楷體"
                  Font-Size="Small"
                  ForeColor="Black"
                  Orientation="Horizontal"
                  StaticMenuStyle-BorderWidth="0"
                  StaticSubMenuIndent="10px"
                  BorderStyle="Groove"
                  Font-Italic="False"
                  StaticMenuItemStyle-Font-Size="Small"
                  StaticMenuItemStyle-Font-Underline="true"
                  RenderingMode="Table"
                  OnMenuItemClick="MenuWord_MenuItemClick"
                >
                  <DynamicHoverStyle BackColor="#666666" ForeColor="White" />
                  <DynamicMenuItemStyle
                    HorizontalPadding="5px"
                    VerticalPadding="2px"
                  />
                  <DynamicMenuStyle BackColor="#E3EAEB" />
                  <DynamicSelectedStyle BackColor="LightGreen" />
                  <StaticMenuItemStyle ForeColor="Green" />
                  <Items>
                    <asp:MenuItem Text="導入詞彙"> </asp:MenuItem>
                  </Items>
                  <StaticHoverStyle BackColor="#666666" ForeColor="White" />
                  <StaticMenuItemStyle
                    HorizontalPadding="5px"
                    VerticalPadding="2px"
                  />
                  <StaticSelectedStyle BackColor="#1C5E55" />
                </asp:Menu>
              </td>

              <!----------------樓層/機房--------------------------------------------------------------->
              <td
                align="right"
                style="
                  font-family: 標楷體;
                  font-size: large;
                  padding-left: 20px;
                "
              >
                <!-- 樓層/機房:
                <asp:DropDownList
                  ID="SelFloorArea"
                  runat="server"
                  ForeColor="Green"
                  DataSourceID="SqlDataSourceFloorArea"
                  DataValueField="Config"
                  DataTextField="Item"
                  AppendDataBoundItems="true"
                >
                  <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource
                  ID="SqlDataSourceFloorArea"
                  runat="server"
                  ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
                  SelectCommand="select [Item],[Config] from [Config] where [Kind]='樓層/機房' order by [Mark],[Config]"
                >
                </asp:SqlDataSource
                >&nbsp; -->
                <asp:LinkButton
                  runat="server"
                  tooltip="請輸入接收到事件至處理的時間"
                  onclientclick="return false"
                  Forecolor="#0000AC"
                  style="text-decoration: none"
                  >處理時間:</asp:LinkButton
                >
                <asp:TextBox
                  ID="txtPM"
                  runat="server"
                  Width="30"
                  ForeColor="Green"
                  CssClass="style0"
                ></asp:TextBox>
                分
              </td>

              <td align="right" style="font-family: 標楷體; font-size: large">
                &nbsp;&nbsp;叫修時段:
                <asp:DropDownList ID="SelCall" runat="server" ForeColor="Green">
                  <asp:ListItem></asp:ListItem>
                  <asp:ListItem Value="不用">不用</asp:ListItem>
                  <asp:ListItem Value="上班">上班</asp:ListItem>
                  <asp:ListItem Value="下班">下班</asp:ListItem>
                  <asp:ListItem Value="半夜"
                    >半夜</asp:ListItem
                  > </asp:DropDownList
                >&nbsp;&nbsp; 維修人員:
                <asp:TextBox
                  ID="txtMt"
                  runat="server"
                  Width="100"
                  ForeColor="Green"
                  CssClass="style0"
                ></asp:TextBox
                >&nbsp;
              </td>

              <td align="left" valign="middle">
                <asp:Menu
                  ID="MenuMt"
                  runat="server"
                  BackColor=""
                  DynamicHorizontalOffset="2"
                  StaticEnableDefaultPopOutImage="false"
                  MaximumDynamicDisplayLevels="3"
                  Font-Names="標楷體"
                  Font-Size="Small"
                  ForeColor="Black"
                  Orientation="Horizontal"
                  StaticMenuStyle-BorderWidth="0"
                  StaticSubMenuIndent="10px"
                  BorderStyle="Groove"
                  Font-Italic="False"
                  StaticMenuItemStyle-Font-Size="Small"
                  StaticMenuItemStyle-Font-Underline="true"
                  RenderingMode="Table"
                  OnMenuItemClick="MenuMt_MenuItemClick"
                >
                  <DynamicHoverStyle BackColor="#666666" ForeColor="White" />
                  <DynamicMenuItemStyle
                    HorizontalPadding="5px"
                    VerticalPadding="2px"
                  />
                  <DynamicMenuStyle BackColor="#E3EAEB" />
                  <DynamicSelectedStyle BackColor="LightGreen" />
                  <StaticMenuItemStyle ForeColor="Green" />
                  <Items>
                    <asp:MenuItem Text="導入"> </asp:MenuItem>
                  </Items>
                  <StaticHoverStyle BackColor="#666666" ForeColor="White" />
                  <StaticMenuItemStyle
                    HorizontalPadding="5px"
                    VerticalPadding="2px"
                  />
                  <StaticSelectedStyle BackColor="#1C5E55" />
                </asp:Menu>
              </td>

              <td align="left" style="font-family: 標楷體">
                &nbsp;&nbsp;
                <font
                  style="
                    color: Blue;
                    text-decoration: underline;
                    font-size: x-small;
                    cursor: pointer;
                    font-size: small;
                  "
                >
                  <u
                    onclick="alert('1.需呈核長官供維修人員加班申請之用\n\n2.每筆處理過程只能記錄一位維修人員，若叫修數人，請分行記錄\n\n3.叫修時段：\n不用：不用叫修，維修人員請空白\n上班：上班時間\n下班：06:00~23:00非上班時間\n半夜：23:00~06:00\n\n4.叫修時段若非不用，則維修人員必填，反之亦然\n\n5.維修人員即處理人員');"
                    >說明</u
                  >
                </font>
              </td>
            </tr>
          </table>
        </td>
      </tr>

      <tr>
        <td align="left" style="font-size: large; font-family: 標楷體">
          &nbsp;<asp:TextBox
            ID="txtProcess"
            name="txtProcess"
            runat="server"
            Columns="112"
            Rows="10"
            TextMode="MultiLine"
            onfocus="this.style.backgroundColor='#FDFFCF'"
            onblur="this.style.backgroundColor='white'"
          ></asp:TextBox
          >&nbsp;
        </td>
      </tr>

      <tr>
        <td align="left" colspan="5">
          <br />&nbsp;共用屬性區：
          <hr />
        </td>
      </tr>
      <!----------------共用屬性--------------------------------------------------------------->
      <tr>
        <td
          align="left"
          style="font-size: large; font-family: 標楷體; color: Red"
        >
          &nbsp;根因系統:
          <asp:DropDownList
            ID="SelSysCode"
            runat="server"
            ForeColor="Green"
            DataSourceID="SqlDataSourceSysCode"
            DataValueField="Config"
            DataTextField="Item"
            AppendDataBoundItems="true"
            AutoPostBack="true"
            OnSelectedIndexChanged="SelSysCode_SelectedIndexChanged"
          >
            <asp:ListItem></asp:ListItem>
          </asp:DropDownList>
          <asp:SqlDataSource
            ID="SqlDataSourceSysCode"
            runat="server"
            ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
            SelectCommand="select [Item],[Config] from [Config] where [Kind]='系統代碼' order by [Mark],[Config]"
          >
          </asp:SqlDataSource
          >&nbsp;&nbsp; 資產類型:
          <asp:DropDownList
            ID="SelKind"
            runat="server"
            ForeColor="Green"
            DataSourceID="SqlDataSourceKind"
            DataValueField="Item"
            DataTextField="Config"
            AppendDataBoundItems="true"
          >
            <asp:ListItem></asp:ListItem>
          </asp:DropDownList>
          <asp:SqlDataSource
            ID="SqlDataSourceKind"
            runat="server"
            ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
            SelectCommand="select [Item],[Config] from [Config] where [Kind]='訊息種類' and [Mark]<>'z' order by [Mark]"
          >
          </asp:SqlDataSource
          >&nbsp;&nbsp; 異常等級:
          <asp:DropDownList
            ID="SelDegree"
            runat="server"
            ForeColor="Green"
            DataSourceID="SqlDataSourceDegree"
            DataValueField="Config"
            DataTextField="Item"
            AppendDataBoundItems="true"
          >
            <asp:ListItem></asp:ListItem>
          </asp:DropDownList>
          <asp:SqlDataSource
            ID="SqlDataSourceDegree"
            runat="server"
            ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
            SelectCommand="select [Item],[Config] from [Config] where [Kind]='異常等級' order by [Config]"
          >
          </asp:SqlDataSource
          >&nbsp;

          <font
            color="blue"
            size="2"
            onclick="alert('紅色欄位負責人可於統計日期(下一月5日)之前修改');"
          >
            <u style="cursor: pointer">期限</u> </font
          >&nbsp;
          <a href="../help/異常事件等級舉例說明.txt" target="_blank"
            ><font color="blue" size="2">說明</font></a
          >&nbsp;
        </td>
      </tr>
      <!----------------追蹤公告--------------------------------------------------------------->
      <tr>
        <td align="left" style="font-size: large; font-family: 標楷體">
          <br />&nbsp;處理狀態:
          <asp:DropDownList
            ID="SelStatus"
            runat="server"
            ForeColor="Green"
            DataSourceID="SqlDataSourceStatus"
            DataValueField="Config"
            DataTextField="Memo"
            AutoPostBack="true"
            OnSelectedIndexChanged="SelStatus_SelectedIndexChanged"
          >
            <asp:ListItem></asp:ListItem>
          </asp:DropDownList>
          <asp:SqlDataSource
            ID="SqlDataSourceStatus"
            runat="server"
            ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
            SelectCommand="select [Config],[Memo] from [Config] where [Kind]='處理狀態' order by [Mark]"
          >
          </asp:SqlDataSource
          >&nbsp;&nbsp; 公告期限:
          <asp:DropDownList
            ID="SelYYYY"
            runat="server"
            ForeColor="Green"
            AppendDataBoundItems="true"
          >
            <asp:ListItem></asp:ListItem> </asp:DropDownList
          >年
          <asp:DropDownList
            ID="SelMM"
            runat="server"
            ForeColor="Green"
            AppendDataBoundItems="true"
          >
            <asp:ListItem></asp:ListItem> </asp:DropDownList
          >月
          <asp:DropDownList
            ID="SelDD"
            runat="server"
            ForeColor="Green"
            AppendDataBoundItems="true"
          >
            <asp:ListItem></asp:ListItem> </asp:DropDownList
          >日
          <asp:DropDownList ID="SelClass" runat="server" ForeColor="Green">
            <asp:ListItem></asp:ListItem>
            <asp:ListItem Value="1">早</asp:ListItem>
            <asp:ListItem Value="2">午</asp:ListItem>
            <asp:ListItem Value="3">晚</asp:ListItem> </asp:DropDownList
          >班

          <font
            color="blue"
            size="2"
            style="cursor: pointer"
            onclick="alert('1. 欲設定追蹤或知識公告，請將處理狀態設定為 [列追蹤/知識公告]\n'
                                                         + '2. 知識公告亦需將年期限設為 [永久]\n'
                                                         + '3. 有設定日期之追蹤公告，到期會黃底提示，以便追蹤處理\n'
                                                         + '4. 欲將追蹤公告結案，請將處理狀態改為 [公告已結案/失效]\n'
                                                         + '5. 欲使知識公告失效，請將處理狀態改為 [公告已結案/失效]\n'
                                                         + '6. 追蹤結案時，公告期限的預設值是現在當班，若改成永久，會變成失效之知識公告\n'
                                                         + '7. 將處理狀態設為 [(未定狀態)]，會同時顯示於追蹤及知識公告');"
            >&nbsp; <u>說明</u> </font
          >&nbsp;
        </td>
      </tr>
      <!----------------存　檔--------------------------------------------------------------->
      <tr>
        <td align="right" colspan="5">
          <br />
          <asp:Panel
            ID="PanelMove"
            runat="server"
            ForeColor="Green"
            Visible="false"
          >
            <asp:DropDownList
              ID="SelMoveYYYY"
              runat="server"
              ForeColor="Green"
              AutoPostBack="true"
              OnSelectedIndexChanged="MoveTour_Changed"
            >
            </asp:DropDownList
            >年&nbsp;

            <asp:DropDownList
              ID="SelMoveMM"
              runat="server"
              ForeColor="Green"
              AutoPostBack="true"
              OnSelectedIndexChanged="MoveTour_Changed"
            >
            </asp:DropDownList
            >月&nbsp;

            <asp:DropDownList
              ID="SelMoveDD"
              runat="server"
              ForeColor="Green"
              AutoPostBack="true"
              OnSelectedIndexChanged="MoveTour_Changed"
            >
            </asp:DropDownList
            >日&nbsp;

            <asp:DropDownList
              ID="SelMoveClass"
              runat="server"
              ForeColor="Green"
              AutoPostBack="true"
              OnSelectedIndexChanged="MoveTour_Changed"
            >
              <asp:ListItem Value="1">早</asp:ListItem>
              <asp:ListItem Value="2">午</asp:ListItem>
              <asp:ListItem Value="3">晚</asp:ListItem> </asp:DropDownList
            >班 &nbsp;&nbsp;

            <asp:DropDownList
              ID="SelDiaryNo"
              runat="server"
              ForeColor="Green"
              DataSourceID="SqlDataSourceDiaryNo"
              DataValueField="DiaryNo"
              DataTextField="TextNo"
            >
            </asp:DropDownList>
            <asp:SqlDataSource
              ID="SqlDataSourceDiaryNo"
              runat="server"
              ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
            ></asp:SqlDataSource
            >&nbsp;&nbsp;&nbsp;&nbsp;

            <asp:Button
              ID="BtnMove"
              runat="server"
              Text="確定移入"
              OnClick="BtnMove_Click"
              ToolTip="若僅有一筆處理過程，則連帶刪除原日誌狀態及訊息資訊。"
            />&nbsp;&nbsp;
            <asp:Button
              ID="BtnCancel"
              runat="server"
              Text="取消"
              OnClick="BtnCancel_Click"
            />&nbsp;&nbsp;
          </asp:Panel>

          <asp:Panel ID="PanelSave" runat="server">
            <asp:LinkButton
              ID="LinkMove"
              Visible="false"
              runat="server"
              Font-Size="Small"
              ForeColor="Blue"
              OnClick="LinkMove_Click"
              ToolTip="若僅有一筆處理過程，則連帶刪除原日誌狀態及訊息資訊。"
              >將處理過程移至 ...</asp:LinkButton
            >&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button
              ID="BtnSave"
              runat="server"
              Text="　存　檔　"
              Font-Size="Large"
              OnClick="BtnSave_Click"
            />&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button
              ID="BtnExit"
              runat="server"
              Text="　離　開　"
              Font-Size="Large"
              OnClick="BtnExit_Click"
            />&nbsp;&nbsp;
          </asp:Panel>
        </td>
      </tr>
    </table>
  </div>

  <asp:Label ID="lblMsgDT" runat="server" Visible="false" />
  <asp:Label ID="lblMsgText" runat="server" Visible="false" />
</asp:Content>
