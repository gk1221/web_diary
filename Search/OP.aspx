<%@ Page Title="人員查詢" Language="C#" MasterPageFile="../MasterPage.master" Debug="true" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeFile="OP.aspx.cs" Inherits="Search_OP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="center">        
        OP：
        <asp:DropDownList ID="SelOP" runat="server" ForeColor="Green" DataSourceID="SqlDataSourceOP" DataValueField="代號" DataTextField="成員" AppendDataBoundItems="true">
            <asp:ListItem Value="#">非機房</asp:ListItem>
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSourceOP" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" 
            SelectCommand="select [成員],[代號] from [View_組織架構] where [課別]='作業管理科' and [性質]='員工' order by [代號]">
        </asp:SqlDataSource>&nbsp;&nbsp;&nbsp;&nbsp;
        
        日期：
        <asp:DropDownList ID="SelYYYY" runat="server" ForeColor="Green" AppendDataBoundItems="true">
            <asp:ListItem Value="____">****</asp:ListItem>
        </asp:DropDownList>年
        <asp:DropDownList ID="SelMM" runat="server" ForeColor="Green" AppendDataBoundItems="true">
            <asp:ListItem Value="__">**</asp:ListItem>
        </asp:DropDownList>月&nbsp;&nbsp;&nbsp;&nbsp;

        <asp:CheckBox ID="ChkOut" Checked="true" runat="server" Font-Size="Small" Text="排除" />&nbsp;&nbsp;
        <asp:CheckBox ID="ChkPage" Checked="true" runat="server" Font-Size="Small" Text="分頁" />&nbsp;&nbsp;
        <asp:Button ID="BtnSearch" runat="server" Text=" 當 班 " OnClick="BtnSearch_Click" /> &nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" Text=" 存 檔 " OnClick="BtnSave_Click" /> &nbsp;&nbsp;
        
        <font size="2" color="blue" style="cursor:pointer"><u onClick="alert('當班：查當班者\n存檔：查存檔者\n排除：不顯示例行性事項，系統設定->排除次數 中有定義');">說明</u></font> &nbsp;&nbsp;        
        <a href="Count.aspx"><font size="2" color="blue" style="cursor:pointer">總計</font></a>       
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
