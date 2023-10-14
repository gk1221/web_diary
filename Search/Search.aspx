﻿<%@ Page Title="關鍵字查詢" Language="C#" MasterPageFile="../MasterPage.master" Debug="true" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeFile="Search.aspx.cs" Inherits="Search_Search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="center"> 
		<asp:Panel ID="Panel" runat="server" DefaultButton="BtnSearch">
        關鍵字：<asp:TextBox ID="txtKey" runat="server" ForeColor="Green" CssClass="style0"></asp:TextBox>
               　
        日期：
        <asp:DropDownList ID="SelDate" runat="server" ForeColor="Green">
            <asp:ListItem Value="">(全部)</asp:ListItem>
            <asp:ListItem Value="1" Selected="True">近一年</asp:ListItem>
            <asp:ListItem Value="3">近三年</asp:ListItem>
            <asp:ListItem Value="5">近五年</asp:ListItem>
        </asp:DropDownList>&nbsp;&nbsp;
        
        系統：
        <asp:DropDownList ID="SelSysCode" runat="server" ForeColor="Green" DataSourceID="SqlDataSourceSysCode" DataValueField="Config" DataTextField="Item" AppendDataBoundItems="true">
            <asp:ListItem></asp:ListItem>
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSourceSysCode" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" 
            SelectCommand="select [Item],[Config] from [Config] where [Kind]='系統代碼' order by [Mark],[Config]">
        </asp:SqlDataSource>&nbsp;&nbsp;&nbsp;&nbsp;

        <asp:CheckBox ID="ChkMsg" Checked="false" runat="server" Font-Size="Small" Text="查訊息" />&nbsp;&nbsp;
        <asp:CheckBox ID="ChkKnow" Checked="false" runat="server" Font-Size="Small" Text="限公告" />&nbsp;&nbsp;
        <asp:CheckBox ID="ChkPage" Checked="true" runat="server" Font-Size="Small" Text="分頁" />&nbsp;&nbsp;
        <asp:Button ID="BtnSearch" runat="server" Text="　查　詢　" OnClick="BtnSearch_Click" />     
		</asp:Panel>
    </div>

    <div id="divBody" align="center" style="width: 100%; overflow: auto;">
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" PageSize="25" AllowSorting="True" AutoGenerateColumns="False" Width="95%"
            BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px"
            CellPadding="0" CellSpacing="0" DataSourceID="SqlDataSourceSearch" DataKeyNames="日誌班別,日誌編號" OnRowDataBound="GridView1_RowDataBound"
            ForeColor="Black" Font-Size="Small" Font-Underline="False" GridLines="Both" ShowHeaderWhenEmpty="True">
            <Columns>
                <asp:BoundField DataField="班別" HeaderText="班別" SortExpression="班別" ItemStyle-Width="72" HtmlEncode="false" />
                <asp:BoundField DataField="工作日誌" HeaderText="工作日誌內容" ItemStyle-HorizontalAlign="Left" HtmlEncode="false" />
                <asp:BoundField DataField="異常代碼" HeaderText="異常" SortExpression="異常代碼" ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center"/>
                <asp:BoundField DataField="根因系統" HeaderText="根因" SortExpression="根因系統" ItemStyle-Width="60" ItemStyle-HorizontalAlign="Center"/>
                <asp:BoundField DataField="資產類型" HeaderText="資產" SortExpression="資產類型" ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center"/>
            </Columns>
            <FooterStyle BackColor="#CCCCCC" />
            <HeaderStyle Font-Size="Medium" Font-Bold="True" />
            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Center" />
            <RowStyle />
            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />        
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSourceSearch" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" SelectCommand=""></asp:SqlDataSource>        
    </div>
    <script type="text/javascript">
        function TourClick(qryTour) { //移到qryTour當班的日誌
	        window.open("../Diary/diary.aspx?tour=" + qryTour);
        } 
    </script>    
</asp:Content>