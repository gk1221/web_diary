<%@ Page Title="手動查詢" Language="C#" MasterPageFile="../MasterPage.master" Debug="true" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeFile="ManuSearch.aspx.cs" Inherits="Search_ManuSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="center">        
        日期：
        <asp:DropDownList ID="SelYYYY" runat="server" ForeColor="Green" AppendDataBoundItems="true">
            <asp:ListItem Value="____">****</asp:ListItem>
        </asp:DropDownList>年
        <asp:DropDownList ID="SelMM" runat="server" ForeColor="Green" AppendDataBoundItems="true">
            <asp:ListItem Value="__">**</asp:ListItem>
        </asp:DropDownList>月&nbsp;&nbsp;

        <asp:DropDownList ID="SelSys" runat="server" ForeColor="Green" DataSourceID="SqlDataSourceSys" DataValueField="Config" DataTextField="Item" AppendDataBoundItems="true">
            <asp:ListItem Value="">全部系統(***)</asp:ListItem>
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSourceSys" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" 
            SelectCommand="select [Item]+'('+[Config]+')' as [Item],[Config] from [Config] where [Kind]='系統代碼' order by [Mark],[Config]">
        </asp:SqlDataSource>&nbsp;&nbsp;&nbsp;&nbsp;

        <asp:CheckBox ID="ChkEnd" Checked="false" runat="server" Font-Size="Small" Text="僅停用" />&nbsp;&nbsp;
        <asp:CheckBox ID="ChkPage" Checked="true" runat="server" Font-Size="Small" Text="分頁" />&nbsp;&nbsp;
        <asp:Button ID="BtnSearch" runat="server" Text="　查　詢　" OnClick="BtnSearch_Click" />        
    </div>

    <div id="divBody" align="center" style="width: 100%; overflow: auto;">
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" PageSize="25" AllowSorting="True" AutoGenerateColumns="False" Width="95%"
            BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px"
            CellPadding="0" CellSpacing="0" DataSourceID="SqlDataSourceSearch" OnRowDataBound="GridView1_RowDataBound"
            ForeColor="Black" Font-Size="Small" Font-Underline="False" GridLines="Both" ShowHeaderWhenEmpty="True">
            <Columns>
                <asp:BoundField DataField="SaveDT" HeaderText="修改時間" SortExpression="SaveDT" />
                <asp:BoundField DataField="SopCode" HeaderText="訊息代碼" SortExpression="SopCode" />
                <asp:BoundField DataField="SopMsg" HeaderText="手動訊息" SortExpression="SopMsg" ItemStyle-HorizontalAlign="Left" HtmlEncode="false" />
                <asp:BoundField DataField="SubSys" HeaderText="訊息分類" SortExpression="SubSys" />
                <asp:BoundField DataField="Status" HeaderText="狀態" SortExpression="Status" HeaderStyle-Width="50"  ItemStyle-Width="50"  />
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
