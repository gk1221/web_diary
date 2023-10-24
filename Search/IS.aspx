﻿<%@ Page Title="資安統計" Language="C#" MasterPageFile="../MasterPage.master"
Debug="false" EnableEventValidation="false" AutoEventWireup="true"
MaintainScrollPositionOnPostback="true" CodeFile="IS.aspx.cs" Trace='false'
Inherits="Search_IS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content
  ID="Content2"
  ContentPlaceHolderID="ContentPlaceHolder1"
  runat="Server"
>
  <div align="center">
    系統：
    <asp:DropDownList
      ID="SelSys"
      runat="server"
      ForeColor="Green"
      DataSourceID="SqlDataSourceSys"
      DataValueField="Config"
      DataTextField="Item"
      AppendDataBoundItems="true"
    >
      <asp:ListItem Value="">全部系統(***)</asp:ListItem>
      <asp:ListItem Value="001">MIPS(已刪除)</asp:ListItem>
      <asp:ListItem Value="106">AIMS(已刪除)</asp:ListItem>
      <asp:ListItem Value="200">AMID(已刪除)</asp:ListItem>
    </asp:DropDownList>
    <asp:SqlDataSource
      ID="SqlDataSourceSys"
      runat="server"
      ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
      SelectCommand="select [Item]+'('+[Config]+')' as [Item],[Config] from [Config] where [Kind]='系統代碼' order by [Mark],[Config]"
    >
    </asp:SqlDataSource
    >&nbsp;&nbsp; 日期：
    <asp:DropDownList
      ID="SelYYYY"
      runat="server"
      ForeColor="Green"
      AppendDataBoundItems="true"
    >
      <asp:ListItem Value="____">****</asp:ListItem> </asp:DropDownList
    >年
    <asp:DropDownList
      ID="SelMM"
      runat="server"
      ForeColor="Green"
      AppendDataBoundItems="true"
    >
      <asp:ListItem Value="__">**</asp:ListItem> </asp:DropDownList
    >月 上下班
    <asp:DropDownList
      ID="SelwkTime"
      runat="server"
      ForeColor="Green"
      AppendDataBoundItems="true"
    >
      <asp:ListItem Value="__">**</asp:ListItem>
    </asp:DropDownList>
    &nbsp;&nbsp;&nbsp;&nbsp;

    <asp:CheckBox
      ID="ChkPage"
      runat="server"
      Font-Size="Small"
      Text="分頁"
    />&nbsp;&nbsp;
    <asp:Button
      ID="BtnSearch"
      runat="server"
      Text="　查　詢　"
      OnClick="BtnSearch_Click"
    />
    <asp:Button
      ID="BtnHTML"
      runat="server"
      Text="匯出Excel檔 (html格式)"
      OnClick="BtnHTML_Click"
    />
    <asp:Button
      ID="BtnExcel"
      runat="server"
      Text="匯出csv檔"
      OnClick="BtnExcel_Click"
    />
  </div>

  <div id="divBody" align="center" style="width: 100%; overflow: auto">
    <asp:GridView
      ID="GridView1"
      runat="server"
      AllowPaging="True"
      PageSize="25"
      AllowSorting="True"
      AutoGenerateColumns="False"
      Width="95%"
      BorderColor="#999999"
      BorderStyle="Solid"
      BorderWidth="3px"
      CellPadding="0"
      CellSpacing="0"
      DataSourceID="SqlDataSourceSearch"
      DataKeyNames="日誌班別,日誌編號"
      OnRowDataBound="GridView1_RowDataBound"
      ForeColor="Black"
      Font-Size="Small"
      Font-Underline="False"
      GridLines="Both"
      ShowHeaderWhenEmpty="True"
    >
      <Columns>
        <asp:BoundField
          DataField="班別"
          HeaderText="班別"
          SortExpression="班別"
          ItemStyle-Width="72"
          HtmlEncode="false"
        />
        <asp:BoundField
          DataField="工作日誌"
          HeaderText="工作日誌內容"
          ItemStyle-HorizontalAlign="Left"
          HtmlEncode="false"
        />
        <asp:BoundField
          DataField="異常代碼"
          HeaderText="異常"
          SortExpression="異常代碼"
          ItemStyle-Width="40"
          ItemStyle-HorizontalAlign="Center"
        />
        <asp:BoundField
          DataField="根因系統"
          HeaderText="根因"
          SortExpression="根因系統"
          ItemStyle-Width="60"
          ItemStyle-HorizontalAlign="Center"
        />
        <asp:BoundField
          DataField="資產類型"
          HeaderText="資產"
          SortExpression="資產類型"
          ItemStyle-Width="40"
          ItemStyle-HorizontalAlign="Center"
        />
      </Columns>
      <FooterStyle BackColor="#CCCCCC" />
      <HeaderStyle Font-Size="Medium" Font-Bold="True" />
      <PagerStyle
        BackColor="#CCCCCC"
        ForeColor="Black"
        HorizontalAlign="Center"
      />
      <RowStyle />
      <SelectedRowStyle
        BackColor="#000099"
        Font-Bold="True"
        ForeColor="White"
      />
    </asp:GridView>
    <asp:SqlDataSource
      ID="SqlDataSourceSearch"
      runat="server"
      ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
      SelectCommand=""
    ></asp:SqlDataSource>
  </div>
  <script type="text/javascript">
    function TourClick(qryTour) {
      //移到qryTour當班的日誌
      window.open("../Diary/diary.aspx?tour=" + qryTour);
    }
  </script>
</asp:Content>
