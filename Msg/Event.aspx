<%@ Page Title="緊急應變" Language="C#" MasterPageFile="../MasterPage.master" AutoEventWireup="true" Debug="true"
    MaintainScrollPositionOnPostback="true" CodeFile="Event.aspx.cs" Inherits="Msg_Event" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server"> 
    <div style="text-align: center;">
        <font color="blue" size="4"><b>緊急應變事件編號</b></font>：
        <asp:DropDownList ID="SelEvent" runat="server" ForeColor="Green" DataSourceID="SqlDataSourceEvent"
            DataTextField="事件編號" DataValueField="事件編號" AutoPostBack="True" OnSelectedIndexChanged="SelEvent_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSourceEvent" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
            SelectCommand="SELECT distinct [Msg].[tour] + '-' + CAST([Msg].[DiaryNo] AS varchar) as [事件編號] FROM [Msg],[Diary] where [Msg].[tour]=[Diary].[tour] and [Msg].[DiaryNo]=[Diary].[DiaryNo] and ([Diary].[Degree] in (2,3) or [MsgCode]='1930e03') and [Msg].[tour]>CAST(YEAR(CURRENT_TIMESTAMP)-10  AS varchar)+'a' order by [事件編號] desc">
        </asp:SqlDataSource> &nbsp;&nbsp;                
        <asp:Button ID="BtnEventAdd" runat="server" Text="新增事件" ForeColor="Red" OnClick="BtnEventAdd_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;

        處理人員：
        <asp:DropDownList ID="SelMt" runat="server" ForeColor="Green" DataSourceID="SqlDataSourceMt" AutoPostBack="true" OnSelectedIndexChanged="BtnSearch_Click" DataTextField="維修人員" DataValueField="維修人員">
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSourceMt" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" SelectCommand=""></asp:SqlDataSource>&nbsp;&nbsp;&nbsp;&nbsp;

        <span onclick="window.open('/sop/sop.asp?SEL_Code=1930e03','_blank');">
            <u><font size="2" color="blue" style="cursor: pointer">SOP</font></u>
        </span> &nbsp;&nbsp;
        <span onclick="alert('1. 緊急應變事件定義：10年內發生訊息代碼1930e03或重大異常或資安事故\n2. 機房OP才有啟動緊急應變記錄(新增事件)的權限\n3. 登入人員即記錄人員，記錄人員才有修改權限\n4. 維修人員即處理人員\n5. 刪除記錄請至當班日誌介面執行！\n6. 頁面每30秒更新一次\n7. 外單位演練需透過中心員工帳號來記錄');">
            <u><font size="2" color="blue" style="cursor: pointer">說明</font></u>
        </span>&nbsp;&nbsp;
        <span onclick="window.open('../help/緊急應變事件處理SOP.pdf','_blank');">
            <u><font size="2" color="blue" style="cursor: pointer">使用手冊</font></u>
        </span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                
        <asp:Button ID="BtnProcessAdd" runat="server" Text="新增記錄" OnClick="BtnProcessAdd_Click" />
        <br />

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>        

        <div id="divBody" align="center" style="width: 100%; overflow: auto;">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Timer ID="Timer1" runat="server" Interval="30000" OnTick="Timer1_Tick">
                </asp:Timer>

                <asp:GridView ID="GridView1" runat="server" AllowPaging="False" AllowSorting="True"
                    AutoGenerateColumns="False" BackColor="#CCFF66" BorderColor="#999999" BorderStyle="None"
                    BorderWidth="1px" CellPadding="3" DataKeyNames="處理編號" DataSourceID="SqlDataSource1"
                    GridLines="Vertical" Font-Size="Small" OnRowCommand="GridView1_RowCommand" Style="text-align: left"
                    HeaderStyle-HorizontalAlign="Center">
                    <AlternatingRowStyle BackColor="#DCDCDC" />
                    <Columns>
                        <asp:BoundField DataField="處理編號" HeaderText="No." SortExpression="處理編號" ItemStyle-Width="25"
                            Visible="false" />
                        <asp:BoundField DataField="處理時間" HeaderText="處理時間" SortExpression="處理時間" ItemStyle-Width="100"
                            ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="處理過程" HeaderText="處理過程" SortExpression="處理過程" ItemStyle-Width="800" HtmlEncode="false" />
                        <asp:BoundField DataField="處理人員" HeaderText="處理人員" SortExpression="處理人員" />
                        <asp:BoundField DataField="存檔人員" HeaderText="記錄人員" SortExpression="存檔人員" />
                        <asp:ButtonField ButtonType="Button" CommandName="修改" Text="修改" HeaderText="執行" />
                    </Columns>
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                    <PagerSettings Mode="NumericFirstLast" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#0000A9" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#000065" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
                    SelectCommand="" ProviderName="<%$ ConnectionStrings:DiaryConnectionString.ProviderName %>">
                </asp:SqlDataSource>
            </ContentTemplate>
        </asp:UpdatePanel>
        </div>       
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
