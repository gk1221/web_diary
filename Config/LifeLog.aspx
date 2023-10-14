<%@ Page Title="異動記錄" Language="C#" MasterPageFile="../MasterPage.master" AutoEventWireup="true" validateRequest="false" Debug="true" MaintainScrollPositionOnPostback="true" CodeFile="LifeLog.aspx.cs" Inherits="Config_LifeLog" %>
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
        <asp:CheckBox ID="ChkMt" runat="server" />
        <asp:Label ID="Label2" runat="server" Font-Size="Small" Text="人員："></asp:Label>
        <asp:DropDownList ID="SelMt" runat="server" DataSourceID="SqlDataSource1" ForeColor="Green" DataTextField="成員" DataValueField="成員">
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
            ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" 
            SelectCommand="SELECT [成員] FROM [View_組織架構] WHERE [性質]='員工' and [課別]='作業管理科' ORDER BY [代號]">
        </asp:SqlDataSource>　

        <asp:Label ID="Label4" runat="server" Font-Size="Small" Text="日期："></asp:Label>
        <asp:DropDownList ID="SelYYYY" runat="server" ForeColor="Green"></asp:DropDownList>
        <asp:Label ID="Label6" runat="server" Font-Size="Small" Text="年"></asp:Label>
        <asp:DropDownList ID="SelMM" runat="server" ForeColor="Green"></asp:DropDownList>
        <asp:Label ID="Label7" runat="server" Font-Size="Small" Text="月"></asp:Label>
        <asp:DropDownList ID="SelDD" runat="server" ForeColor="Green"></asp:DropDownList>
        <asp:Label ID="Label8" runat="server" Font-Size="Small" Text="日"></asp:Label>　

        <asp:Label ID="Label1" runat="server" Font-Size="Small" Text="表格："></asp:Label>
        <asp:DropDownList ID="SelTbl" runat="server" ForeColor="Green">
            <asp:ListItem></asp:ListItem>
            <asp:ListItem>系統參數</asp:ListItem>
            <asp:ListItem>訊息設定</asp:ListItem>
            <asp:ListItem>小鬧鐘</asp:ListItem>
            <asp:ListItem>異常處理</asp:ListItem>
            <asp:ListItem>處理過程</asp:ListItem>
        </asp:DropDownList>　

        <asp:Label ID="Label9" runat="server" Font-Size="Small" Text="主鍵："></asp:Label>
        <asp:TextBox ID="TextPK" runat="server" Columns="6" ForeColor="Green"></asp:TextBox>&nbsp;&nbsp;
        
        <asp:CheckBox ID="ChkPage" Checked="true" Text="分頁" runat="server" Font-Size="Small" />&nbsp;&nbsp;

        <asp:Label ID="Label10" runat="server" Font-Size="Small" Text="字串："></asp:Label>
        <asp:TextBox ID="TextLife" runat="server" Columns="6" ForeColor="Green"></asp:TextBox>&nbsp;<font color="green">(逗點隔開)</font>&nbsp;&nbsp;&nbsp;&nbsp;

        <asp:Button ID="Button1" runat="server" Text=" 查 詢 " onclick="Button1_Click" />

    <div id="divBody" style="width: 100%; overflow: auto;">
    <table width="98%"><tr><td><p>
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" BackColor="#CCCCCC" 
            BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" 
            CellSpacing="2" DataKeyNames="記錄編號" DataSourceID="SqlDataSource3" 
            ForeColor="Black" Font-Size="Small" 
            OnSelectedIndexChanging="GridView1_SelectedIndexChanging" HorizontalAlign="Center">
            <Columns>
                <asp:CommandField ShowSelectButton="True" ButtonType="Button" HeaderText="執行" SelectText="選取" />
                <asp:BoundField DataField="記錄編號" HeaderText="No." ReadOnly="True" SortExpression="記錄編號" />
                <asp:BoundField DataField="表格名稱" HeaderText="表格名稱" SortExpression="表格名稱" />
                <asp:BoundField DataField="主鍵編號" HeaderText="PK." SortExpression="主鍵編號" />
                <asp:BoundField DataField="異動記錄" HeaderText="異動記錄" SortExpression="異動記錄" HtmlEncode="false" />
                <asp:BoundField DataField="異動人員" HeaderText="異動人員" SortExpression="異動人員" />
                <asp:BoundField DataField="異動日期"  DataFormatString="{0:yyyy/MM/dd HH:mm}" HeaderText="異動日期" SortExpression="異動日期" />
                <asp:BoundField DataField="執行機器" HeaderText="執行機器" SortExpression="執行機器" />
            </Columns>
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
        <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" SelectCommand="">
        </asp:SqlDataSource>
    </p></td></tr></table>
    </div>

    <script type="text/javascript">
        //ResizeBody();   //把表格設為與頁面一樣大小，以配合捲軸顯示
        function ResizeBody() {
            divBody.style.width = (screen.width - 40);
            divBody.style.height = (screen.height - 200);
            tblDiary.width = screen.width - 60;
            tblSave.width = screen.width - 60;
            tblTrace.width = screen.width - 60;
            tblKnow.width = screen.width - 60;
        }
    </script>
</asp:Content>

