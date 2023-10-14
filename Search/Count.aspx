<%@ Page Title="登錄查詢" Language="C#" MasterPageFile="../MasterPage.master" AutoEventWireup="true" validateRequest="false" Debug="true" MaintainScrollPositionOnPostback="true" CodeFile="Count.aspx.cs" Inherits="Search_Count" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .style4
        {
            font-size: x-small;
            color: Blue;
            cursor: pointer;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="divBody" align="center" style="width: 100%; overflow: auto;">
    <br />
    <table width="100%"><tr><td><p>
        <asp:GridView ID="GridView1" runat="server" AllowPaging="False" AllowSorting="True"
            BackColor="#CCCCCC" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px"
            CellPadding="4" CellSpacing="2" DataSourceID="SqlDataSource1"
            ForeColor="Black" Font-Size="Large">
            <FooterStyle BackColor="#CCCCCC" />
            <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Center" />
            <RowStyle BackColor="White" />
            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#808080" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#383838" />
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
            SelectCommand=""></asp:SqlDataSource>
    </p></td></tr></table>
    </div>

    <script type="text/javascript">
        //ResizeBody();   //把表格設為與頁面一樣大小，以配合捲軸顯示
        function ResizeBody() {
            divBody.style.width = (screen.width - 40);
            divBody.style.height = (screen.height - 150);
            tblDiary.width = screen.width - 60;
            tblSave.width = screen.width - 60;
            tblTrace.width = screen.width - 60;
            tblKnow.width = screen.width - 60;
        }
    </script>
    </asp:Content>