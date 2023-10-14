<%@ Page Title="手動訊息" Language="C#" MasterPageFile="../MasterPage.master" AutoEventWireup="true" Debug="true" validateRequest="False"
    MaintainScrollPositionOnPostback="true" CodeFile="Manu.aspx.cs" Inherits="Msg_Manu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">  
    <table id="tblBody" Width="100%">      
        <tr><td>
            <asp:Panel ID="PanelEdit" runat="server" Visible="false"> 
            <table><tr><td align="left">
                <asp:CheckBox ID="ChkStatus" Checked="true" runat="server" Text="啟用" />&nbsp;&nbsp;&nbsp;&nbsp;
                
                緊急程度：
                <asp:DropDownList ID="SelCall" runat="server" ForeColor="Green" DataSourceID="SqlDataSourceCall" DataValueField="Config" DataTextField="Item">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSourceCall" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" SelectCommand="select [Item]+'('+[Config]+')' as [Item],[Config] from [Config] where [Kind]='緊急程度' order by [Mark]">
                </asp:SqlDataSource>&nbsp;&nbsp;

                系統代碼：
                <asp:DropDownList ID="SelSys" runat="server" ForeColor="Green" DataSourceID="SqlDataSourceSys" DataValueField="Config" DataTextField="Item">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSourceSys" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" SelectCommand="select [Item]+'('+[Config]+')' as [Item],[Config] from [Config] where [Kind]='系統代碼' order by [Mark],[Config]">
                </asp:SqlDataSource>&nbsp;&nbsp;

                訊息種類：
                <asp:DropDownList ID="SelKind" runat="server" ForeColor="Green" DataSourceID="SqlDataSourceKind" DataValueField="Config" DataTextField="Item">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSourceKind" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" SelectCommand="select [Config]+'('+[Item]+')' as [Item],[Item] as [Config] from [Config] where [Kind]='訊息種類' order by [Config]">
                </asp:SqlDataSource>&nbsp;&nbsp;

                流水號：<asp:TextBox ID="txtFlow" runat="server" Width="30" ForeColor="Green" CssClass="style0"></asp:TextBox>&nbsp;&nbsp;
                系統子集：<asp:TextBox ID="txtSub" runat="server" Width="100" ForeColor="Green" CssClass="style0"></asp:TextBox>&nbsp;&nbsp;
            </td></tr>
            
            <tr><td>
                SOP訊息：<asp:TextBox ID="txtManu" runat="server" Width="840" ForeColor="Green"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;
                        
                <asp:Button ID="BtnAdd" runat="server" Text=" 新 增 "  OnClick="BtnAdd_Click" ForeColor="Red" />&nbsp;&nbsp;
                <asp:Button ID="BtnSave" runat="server" Text=" 修 改 "  OnClick="BtnSave_Click" Enabled="false" />&nbsp;&nbsp;
                <asp:Button ID="BtnDel" runat="server" Text=" 刪 除 "  OnClick="BtnDel_Click" Enabled="false" OnClientClick="return confirm('您確定要刪除這筆資料嗎？')" />&nbsp;&nbsp;

                <asp:Button ID="BtnExit" runat="server" Text=" 離 開 "  OnClick="BtnExit_Click"/> 
            </td></tr>
            </table>   
            </asp:Panel>
        </td></tr>

        <tr>
            <td align="left">
                <asp:GridView ID="GridView1" runat="server" AllowPaging="false" AllowSorting="True" Width="100%" Caption="" CaptionAlign="Left"
                    AutoGenerateColumns="False" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" CellSpacing="2"
                    DataSourceID="SqlDataSourceSearch" ForeColor="Black" Font-Size="Medium" OnRowDataBound="GridView1_RowDataBound" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                    <Columns>
                        <asp:BoundField DataField="SaveDT" HeaderText="訊息時間" SortExpression="SaveDT" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center" />
                        <asp:HyperLinkField DataTextField="SopCode" HeaderText="訊息代碼" DataNavigateUrlFields="SopCode" DataNavigateUrlFormatString="/sop/sop.asp?SEL_Code={0}" SortExpression="SopCode" ItemStyle-Width="72" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue" Target="_blank" />                                              
                        <asp:HyperLinkField Text="(日誌)" HeaderText="查詢" DataNavigateUrlFields="SopCode" DataNavigateUrlFormatString="../Search/Code.aspx?MsgCode={0}" ItemStyle-Width="50" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue" />                                              
                        <asp:BoundField DataField="訊息內容" HeaderText="訊息內容" SortExpression="SopMsg" HtmlEncode="false" /> 
                        <asp:BoundField DataField="SubSys" HeaderText="訊息分類" SortExpression="SubSys" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Small" />  
                        <asp:TemplateField ItemStyle-Width="60" ItemStyle-HorizontalAlign="Center" HeaderText="對應">
                            <ItemTemplate>
                                <asp:DropDownList ID="SelDiaryNo" runat="server" ForeColor="Green" AutoPostBack="true" DataSourceID="SqlDataSourceDiaryNo" DataValueField="DiaryNo" DataTextField="TextNo" OnSelectedIndexChanged="SelDiaryNo_SelectedIndexChanged">
                                </asp:DropDownList>
                            </ItemTemplate>                            
                        </asp:TemplateField>  
                        <asp:BoundField DataField="Status" HeaderText="狀態" Visible="false" SortExpression="Status" ItemStyle-HorizontalAlign="Center" />  
                        <asp:CommandField ShowSelectButton="true" ButtonType="Button" Visible="false" HeaderText="選取" ItemStyle-HorizontalAlign="Center" />                      
                    </Columns>
                    <FooterStyle BackColor="#CCCCCC" />
                    <HeaderStyle Font-Bold="false" HorizontalAlign="Center" />
                    <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Center" />
                    <RowStyle />
                    <SelectedRowStyle BackColor="white" />                    
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#808080" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#383838" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceSearch" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" SelectCommand=""></asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceDiaryNo" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"></asp:SqlDataSource>
            </td>
        </tr>

        <tr><td align="center">
             <div align="center"><font size="2" color="">訊息時間：手動訊息設定　訊息內容：異常處理程序　<font color="red">紅</font>：近一週　<font color="green">綠</font>：近一月　<font color="blue">藍</font>：近一年　<font color="black">黑</font>：一年以上　<font color="brown">棕</font>：無</font></div>
        </td></tr>

        <tr><td align="right">
            <asp:Button ID="BtnEdit" runat="server" Text="訊息設定" ForeColor="Red" Font-Size="X-Large" OnClick="BtnEdit_Click" OnClientClick="alert('1.不同性質的手動訊息請另外新增 (修改意同刪此增彼)\n\n2.過期不適用請於\'啟用\'核取方塊取消勾選，不用刪除\n\n3.日誌有引用過的訊息，不刪除或更改性質或代碼以保留之');" />
        </td></tr>
    </table>   
    <asp:Label ID="lblTip" runat="server" Font-Size="X-Large" ForeColor="Green" /> <br />
    [異常訊息]有而[手動訊息]無者：<br />
    <asp:Label ID="lblLostQA" runat="server" Font-Size="Small" ForeColor="Green"></asp:Label> <br />
    [日誌訊息]有而[手動訊息]無者：<br />
    <asp:Label ID="lblLostDiary" runat="server" Font-Size="Small" ForeColor="Green"></asp:Label><br />
    已停用之[手動訊息]：<br />
    <asp:Label ID="lblEndMsg" runat="server" Font-Size="Small" ForeColor="Green"></asp:Label>
</asp:Content>
