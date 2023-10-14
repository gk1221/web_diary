<%@ Page Title="叫修記錄" Language="C#" MasterPageFile="../MasterPage.master" Debug="true" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeFile="Call.aspx.cs" Inherits="Search_Call" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="center">        
        系統：
        <asp:DropDownList ID="SelSys" runat="server" ForeColor="Green" DataSourceID="SqlDataSourceSys" DataValueField="Memo" DataTextField="Sys" AppendDataBoundItems="true">            
            <asp:ListItem>(全部)</asp:ListItem>
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSourceSys" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" 
            SelectCommand="select distinct [Memo],substring([Memo],4,len([Memo])-1) as [Sys] from [Config] where [Kind]='系統代碼' order by [Memo]">
        </asp:SqlDataSource>&nbsp;&nbsp;

        時段：
        <asp:DropDownList ID="SelCall" runat="server" ForeColor="Green">    
            <asp:ListItem Value="全部">(全部)</asp:ListItem>
            <asp:ListItem Value="有叫修" Selected="True">有叫修</asp:ListItem>
            <asp:ListItem Value="非上班">非上班</asp:ListItem> 
            <asp:ListItem Value="上班"> 上班 </asp:ListItem> 
            <asp:ListItem Value="下班"> 下班 </asp:ListItem>       
	        <asp:ListItem Value="半夜"> 半夜 </asp:ListItem>      
        </asp:DropDownList>&nbsp;&nbsp;
        
        日期：
        <asp:DropDownList ID="SelYYYY" runat="server" ForeColor="Green" AppendDataBoundItems="true">
            <asp:ListItem Value="____">****</asp:ListItem>
        </asp:DropDownList>年
        <asp:DropDownList ID="SelMM" runat="server" ForeColor="Green" AppendDataBoundItems="true">
            <asp:ListItem Value="__">**</asp:ListItem>
        </asp:DropDownList>月&nbsp;&nbsp;&nbsp;&nbsp;

        <asp:CheckBox ID="ChkPage" runat="server" Font-Size="Small" Text="另開視窗" />&nbsp;&nbsp;
        <asp:Button ID="BtnSearch" runat="server" Text="　查　詢　" OnClick="BtnSearch_Click" />        
    </div>

    <div id="divBody" align="center" style="width: 100%; overflow: auto;">
        <asp:GridView ID="GridView1" runat="server" AllowPaging="false" PageSize="25" AllowSorting="True" AutoGenerateColumns="False" Width="95%"
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
    </div>
    <script type="text/javascript">
        function TourClick(qryTour) { //移到qryTour當班的日誌
	        window.open("../Diary/diary.aspx?tour=" + qryTour);
        } 
    </script>    
</asp:Content>
