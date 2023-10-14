<%@ Page Title="自動查詢" Language="C#" MasterPageFile="../MasterPage.master" Debug="true" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeFile="AutoSearch.aspx.cs" Inherits="Search_AutoSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="center">        
        訊息代碼：
        <asp:DropDownList ID="SelCall" runat="server" ForeColor="Green" DataSourceID="SqlDataSourceCall" DataValueField="Config" DataTextField="Item" AppendDataBoundItems="true">
            <asp:ListItem Value="_">緊急程度(*)</asp:ListItem>
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSourceCall" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" 
            SelectCommand="select [Item]+'('+[Config]+')' as [Item],[Config] from [Config] where [Kind]='緊急程度' order by [Config]">
        </asp:SqlDataSource>&nbsp;&nbsp;
        
        <asp:DropDownList ID="SelSys" runat="server" ForeColor="Green" DataSourceID="SqlDataSourceSys" DataValueField="Config" DataTextField="Item" AppendDataBoundItems="true">
            <asp:ListItem Value="___">訊息系統(***)</asp:ListItem>
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSourceSys" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" 
            SelectCommand="select [Item]+'('+[Config]+')' as [Item],[Config] from [Config] where [Kind]='系統代碼' order by [Mark],[Config]">
        </asp:SqlDataSource>&nbsp;&nbsp;

        <asp:DropDownList ID="SelKind" runat="server" ForeColor="Green" DataSourceID="SqlDataSourceKind" DataValueField="Item" DataTextField="Config" AppendDataBoundItems="true">
            <asp:ListItem Value="_">全部(*)</asp:ListItem>
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSourceKind" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" 
            SelectCommand="select [Config]+'('+[Item]+')' as [Config],[Item] from [Config] where [Kind]='訊息種類' order by [Mark],[Config]">
        </asp:SqlDataSource>&nbsp;&nbsp;

        <asp:TextBox ID="txtFlow" runat="server" ForeColor="Green" CssClass="style0" Width="30" MaxLength="2"></asp:TextBox> (2碼)&nbsp;&nbsp;&nbsp;&nbsp;

        
        
        日期：
        <asp:DropDownList ID="SelYYYY" runat="server" ForeColor="Green" AppendDataBoundItems="true">
            <asp:ListItem Value="____">****</asp:ListItem>
        </asp:DropDownList>年
        <asp:DropDownList ID="SelMM" runat="server" ForeColor="Green" AppendDataBoundItems="true">
            <asp:ListItem Value="__">**</asp:ListItem>
        </asp:DropDownList>月&nbsp;&nbsp;&nbsp;&nbsp;

        <asp:CheckBox ID="ChkPage" Checked="true" runat="server" Font-Size="Small" Text="分頁" />&nbsp;&nbsp;
        <asp:Button ID="BtnSearch" runat="server" Text="　查　詢　" OnClick="BtnSearch_Click" />        
    </div>

    <div id="divBody" align="center" style="width: 100%; overflow: auto;">
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" PageSize="25" AllowSorting="True" AutoGenerateColumns="False" Width="95%"
            BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px"
            CellPadding="0" CellSpacing="0" DataSourceID="SqlDataSourceSearch" OnRowDataBound="GridView1_RowDataBound"
            ForeColor="Black" Font-Size="Small" Font-Underline="False" GridLines="Both" ShowHeaderWhenEmpty="True">
            <Columns>
                <asp:BoundField DataField="WarnDT" HeaderText="訊息日期" SortExpression="WarnDT" />
                <asp:BoundField DataField="Serial" HeaderText="序號" SortExpression="Serial" />
                <asp:BoundField DataField="WarnCode" HeaderText="訊息代碼" SortExpression="WarnCode" />
                <asp:BoundField DataField="WarnMsg" HeaderText="自動訊息" SortExpression="WarnMsg" ItemStyle-HorizontalAlign="Left" HtmlEncode="false" />
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
