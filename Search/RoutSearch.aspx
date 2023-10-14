<%@ Page Title="例行查詢" Language="C#" MasterPageFile="../MasterPage.master" Debug="true" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeFile="RoutSearch.aspx.cs" Inherits="Search_RoutSearch" %>

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
                <asp:BoundField DataField="訊息時間" HeaderText="訊息時間" SortExpression="訊息時間" />
                <asp:BoundField DataField="訊息代碼" HeaderText="訊息代碼" SortExpression="訊息代碼" />
                <asp:BoundField DataField="發生訊息" HeaderText="例行訊息" SortExpression="發生訊息" ItemStyle-HorizontalAlign="Left" HtmlEncode="false" />
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
