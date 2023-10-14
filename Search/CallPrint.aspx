<%@ Page Title="叫修列印" Language="C#" Debug="true" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeFile="CallPrint.aspx.cs" Inherits="Search_CallPrint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">    
</head>
<body runat="server">
<form id="form1" runat="server">       
    <table width="100%">
    <tr><td align="center">
        <asp:Label id="lblCall" runat="server" Font-Bold="true" Font-Size="X-Large" />        
    </td></tr>

    <tr><td align="center">        
        <br />
        <asp:GridView ID="GridView1" runat="server" AllowPaging="false" AllowSorting="True" AutoGenerateColumns="False" Width="95%"
            BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px"
            CellPadding="0" CellSpacing="0" DataSourceID="SqlDataSourceSearch" DataKeyNames="日誌班別,日誌編號" OnRowDataBound="GridView1_RowDataBound"
            ForeColor="Black" Font-Size="Small" Font-Underline="False" GridLines="Both" ShowHeaderWhenEmpty="True">
            <Columns>
                <asp:BoundField DataField="班別" HeaderText="起始班別" ItemStyle-Width="150" HtmlEncode="false" />
                <asp:BoundField DataField="叫修資訊" HeaderText="叫修資訊：維修員(值班)" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="300" HtmlEncode="false" />
                <asp:BoundField DataField="工作日誌" HeaderText="工作日誌內容" ItemStyle-HorizontalAlign="Left" HtmlEncode="false" />
            </Columns>
            <FooterStyle BackColor="#CCCCCC" />            
            <HeaderStyle Font-Size="Medium" Font-Bold="True" />
            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Center" />
            <RowStyle />
            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />        
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSourceSearch" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" SelectCommand=""></asp:SqlDataSource>        
    </td></tr>
    
    <tr><td align="left">
        <br />
    　　科長：　　　　　　　　　　　　　　單位主管：
    </td></tr></table>
    
    <script type="text/javascript">
        function TourClick(qryTour) { //移到qryTour當班的日誌
	        window.open("../Diary/diary.aspx?tour=" + qryTour);
        } 
    </script>    
    </form>
</body>
</html>
