<%@ Page Title="評分作業" Language="C#" AutoEventWireup="true" Debug="true"
MaintainScrollPositionOnPostback="true" Inherits="Diary_Sign"
CodeFile="rating.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head id="Head1" runat="server">
    <style type="text/css">
      .style1 {
        text-align: center;
      }
    </style>
  </head>
  <body runat="server" id="pMaster">
    <form id="form1" runat="server">
      <div id="divBody" align="center" style="width: 100%; overflow: auto">
        <asp:Label
          ID="lblTour"
          ForeColor="Green"
          Font-Size="X-Large"
          runat="server"
        />
        <hr />
        <asp:GridView
          ID="GridView1"
          runat="server"
          AllowPaging="false"
          AllowSorting="True"
          Caption=""
          CaptionAlign="Left"
          AutoGenerateColumns="False"
          BorderColor="#999999"
          BorderStyle="Solid"
          BorderWidth="3px"
          CellPadding="1"
          CellSpacing="1"
          DataSourceID="SqlDataSourceOP"
          ForeColor="Black"
          Font-Size="Medium"
        >
          <Columns>
            <asp:TemplateField
              ItemStyle-HorizontalAlign="Center"
              HeaderText="按讚"
            >
              <HeaderTemplate
                ><asp:CheckBox ID="ChkRout" runat="server" Enabled="false"
              /></HeaderTemplate>
              <ItemTemplate>
                <asp:CheckBox runat="server" />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField
              DataField="成員"
              HeaderText="當 班 者"
              ItemStyle-Width="120"
              ItemStyle-HorizontalAlign="Center"
            />
          </Columns>
          <FooterStyle BackColor="#CCCCCC" />
          <HeaderStyle HorizontalAlign="Center" />
          <PagerStyle
            BackColor="#CCCCCC"
            ForeColor="Black"
            HorizontalAlign="Center"
          />
          <RowStyle />
          <SelectedRowStyle BackColor="#000099" ForeColor="White" />
          <SortedAscendingCellStyle BackColor="#F1F1F1" />
          <SortedAscendingHeaderStyle BackColor="#808080" />
          <SortedDescendingCellStyle BackColor="#CAC9C9" />
          <SortedDescendingHeaderStyle BackColor="#383838" />
        </asp:GridView>
        <asp:SqlDataSource
          ID="SqlDataSourceOP"
          runat="server"
          ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
          SelectCommand="select [成員] from [View_組織架構] where [性質]='員工' and [課別]='作業管理科' order by case when [成員]='宋紹良' then 1 end DESC, [代號] ASC"
        >
        </asp:SqlDataSource>

        <asp:Button
          ID="BtnGood"
          runat="server"
          Text=" 按讚 "
          OnClick="BtnGood_Click"
        />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <br />

        <br />
        專家/小組負責人評分:
        <asp:TextBox
          ID="txtRt"
          runat="server"
          Width="40"
          ForeColor="Green"
          CssClass="style0"
        ></asp:TextBox>
        <br />
        最後修改人:
        <asp:TextBox
          ID="txtEx"
          runat="server"
          Width="70"
          ForeColor="Green"
          CssClass="style0"
        ></asp:TextBox>
        <br />
        <asp:Button
          ID="BtnRating"
          runat="server"
          Text=" 負責人修改 "
          OnClick="BtnRating_Click"
        />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button
          ID="BtnExit"
          runat="server"
          Text=" 離 開 "
          OnClientClick="window.close();"
        />
      </div>
    </form>
  </body>
</html>
