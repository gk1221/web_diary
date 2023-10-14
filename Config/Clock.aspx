<%@ Page Title="小鬧鐘" Language="C#" MasterPageFile="../MasterPage.master" AutoEventWireup="true" Debug="true" validateRequest="False"
    MaintainScrollPositionOnPostback="true" CodeFile="Clock.aspx.cs" Inherits="Config_Clock" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">  
    <div id="divBody" align="center" style="width:100%; overflow:auto;">

    

    <table id="tblBody" Width="100%">      
        <tr><td align="center">            
            <table><tr><td align="left">
                代碼：<asp:TextBox ID="txtMsgCode" runat="server" Width="60" ForeColor="Green" CssClass="style0"></asp:TextBox>&nbsp;&nbsp;
                訊息：<asp:TextBox ID="txtMsgText" runat="server" Width="840" ForeColor="Green"></asp:TextBox>
            </td></tr>
            
            <tr><td align="left">
                日期：<asp:DropDownList ID="SelYYYY" runat="server" ForeColor="Green" AppendDataBoundItems="true">
                    <asp:ListItem Value="20**">****</asp:ListItem>
                </asp:DropDownList>年 &nbsp;&nbsp;
                <asp:DropDownList ID="SelMM" runat="server" ForeColor="Green" AppendDataBoundItems="true">
                    <asp:ListItem Value="**">**</asp:ListItem>
                </asp:DropDownList>月 &nbsp;&nbsp;

                <asp:RadioButton ID="rdoDD"  runat="server" AutoPostBack="true" Checked="true" OnCheckedChanged="rdoDD_CheckedChanged" />
                <asp:DropDownList ID="SelDD" runat="server" ForeColor="Green" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="SelDD_OnSelectedIndexChanged">
                    <asp:ListItem Value="**">**</asp:ListItem>
                </asp:DropDownList>日 &nbsp;&nbsp;

                <asp:RadioButton ID="rdoWeek" runat="server" Text="星期" AutoPostBack="true" OnCheckedChanged="rdoWeek_CheckedChanged" />
                <asp:DropDownList ID="SelWeek" runat="server" ForeColor="Green" AutoPostBack="true" OnSelectedIndexChanged="SelWeek_OnSelectedIndexChanged">
                    <asp:ListItem Value="**">*</asp:ListItem>
                    <asp:ListItem Value="*1">一</asp:ListItem>
                    <asp:ListItem Value="*2">二</asp:ListItem>
                    <asp:ListItem Value="*3">三</asp:ListItem>
                    <asp:ListItem Value="*4">四</asp:ListItem>
                    <asp:ListItem Value="*5">五</asp:ListItem>
                    <asp:ListItem Value="*6">六</asp:ListItem>
                    <asp:ListItem Value="*7">日</asp:ListItem>
                </asp:DropDownList> &nbsp;&nbsp;

                <asp:RadioButton ID="rdoOP" runat="server" Text="OP" AutoPostBack="true" OnCheckedChanged="rdoOP_CheckedChanged" />
                <asp:DropDownList ID="SelOP" runat="server" ForeColor="Green" AutoPostBack="true" OnSelectedIndexChanged="SelOP_OnSelectedIndexChanged">
                    <asp:ListItem Value="**">*</asp:ListItem>
                    <asp:ListItem Value="*A">A</asp:ListItem>
                    <asp:ListItem Value="*B">B</asp:ListItem>
                    <asp:ListItem Value="*C">C</asp:ListItem>
                    <asp:ListItem Value="*D">D</asp:ListItem>
                    <asp:ListItem Value="*E">E</asp:ListItem>
                    <asp:ListItem Value="*F">F</asp:ListItem>
                    <asp:ListItem Value="*G">G</asp:ListItem>
                    <asp:ListItem Value="*H">H</asp:ListItem>
                    <asp:ListItem Value="*I">I</asp:ListItem>
                    <asp:ListItem Value="*K">K</asp:ListItem>
                    <asp:ListItem Value="*X">X</asp:ListItem>
                </asp:DropDownList> &nbsp;&nbsp;

                時間：<asp:DropDownList ID="SelHH" runat="server" ForeColor="Green" AppendDataBoundItems="true">
                    <asp:ListItem Value="**">**</asp:ListItem>
                    <asp:ListItem Value="0*">0*</asp:ListItem>
                    <asp:ListItem Value="1*">1*</asp:ListItem>
                    <asp:ListItem Value="2*">2*</asp:ListItem>
                </asp:DropDownList>時
                <asp:DropDownList ID="SelMI" runat="server" ForeColor="Green" AppendDataBoundItems="true">
                    <asp:ListItem Value="**">**</asp:ListItem>
                    <asp:ListItem Value="**">*0</asp:ListItem>
                    <asp:ListItem Value="**">*5</asp:ListItem>
                    <asp:ListItem Value="**">#1</asp:ListItem>
                    <asp:ListItem Value="**">#2</asp:ListItem>
                    <asp:ListItem Value="**">#3</asp:ListItem>
                    <asp:ListItem Value="00">00</asp:ListItem>
                    <asp:ListItem Value="05">05</asp:ListItem>
                    <asp:ListItem Value="10">10</asp:ListItem>
                    <asp:ListItem Value="15">15</asp:ListItem>
                    <asp:ListItem Value="20">20</asp:ListItem>
                    <asp:ListItem Value="25">25</asp:ListItem>
                    <asp:ListItem Value="30">30</asp:ListItem>
                    <asp:ListItem Value="35">35</asp:ListItem>
                    <asp:ListItem Value="40">40</asp:ListItem>
                    <asp:ListItem Value="45">45</asp:ListItem>
                    <asp:ListItem Value="50">50</asp:ListItem>
                    <asp:ListItem Value="55">55</asp:ListItem>
                </asp:DropDownList>分 &nbsp;&nbsp;

                <asp:CheckBox ID="ChkWork" runat="server" Text="啟用" /> &nbsp;&nbsp;
                <asp:CheckBox ID="ChkBeep" runat="server" Text="Beep" /> &nbsp;&nbsp;
                <asp:CheckBox ID="ChkRout" runat="server" Text="例行工作" /> &nbsp; &nbsp; &nbsp; &nbsp;
                        
                <asp:Button ID="BtnAdd" runat="server" Text=" 新 增 "  OnClick="BtnAdd_Click" ForeColor="Red" />&nbsp;&nbsp;
                <asp:Button ID="BtnSave" runat="server" Text=" 修 改 "  OnClick="BtnSave_Click" Enabled="false" />&nbsp;&nbsp;
                <asp:Button ID="BtnDel" runat="server" Text=" 刪 除 "  OnClick="BtnDel_Click" Enabled="false" OnClientClick="return confirm('您確定要刪除這筆資料嗎？')" />&nbsp;&nbsp;
            </td></tr>
            </table>   
        </td></tr>

        <tr>
            <td align="left">
                <asp:GridView ID="GridView1" runat="server" AllowPaging="false" AllowSorting="True" Width="100%" Caption="" CaptionAlign="Left"
                    AutoGenerateColumns="False" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" CellSpacing="2"
                    DataSourceID="SqlDataSourceClock" ForeColor="Black" Font-Size="Medium" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                    <Columns>
                        <asp:BoundField DataField="No" HeaderText="No" SortExpression="No" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="訊息日期" HeaderText="訊息日期" SortExpression="訊息日期" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center" />
                        <asp:HyperLinkField DataTextField="代碼" HeaderText="代碼" DataNavigateUrlFields="代碼" DataNavigateUrlFormatString="/sop/sop.asp?SEL_Code={0}" SortExpression="代碼" ItemStyle-Width="60" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue" Target="_blank" />                                              
                        <asp:BoundField DataField="訊息內容" HeaderText="訊息內容" SortExpression="訊息內容" HtmlEncode="false" /> 
                        <asp:BoundField DataField="啟" HeaderText="啟" SortExpression="啟" />  
                        <asp:BoundField DataField="響" HeaderText="響" SortExpression="響" />  
                        <asp:BoundField DataField="例" HeaderText="例" SortExpression="例" />  
                        <asp:CommandField ShowSelectButton="true" ButtonType="Button" HeaderText="選取" ItemStyle-HorizontalAlign="Center" />                      
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
                <asp:SqlDataSource ID="SqlDataSourceClock" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" 
                    SelectCommand="SELECT [Serial] as [No],'20'+[YY]+'/'+[MM]+'/'+[DD]+' '+[HH]+':'+[MI] as [訊息日期],[MsgCode] as [代碼],[MsgText] as [訊息內容],[WorkYN] as [啟],[BeepYN] as [響],[RoutYN] as [例] FROM [Clock]">
                </asp:SqlDataSource>
            </td>
        </tr>
    </table>   

    </div>

<script type="text/javascript">
    //ResizeBody();   //把表格設為與頁面一樣大小，以配合捲軸顯示
    function ResizeBody() {
        divBody.style.width = (screen.width - 40);
        divBody.style.height = (screen.height - 250);
        tblBody.style.width = (screen.width - 60);
    }
</script>
</asp:Content>
