<%@ Page Title="自動訊息" Language="C#" MasterPageFile="../MasterPage.master" AutoEventWireup="true" Debug="true"
    MaintainScrollPositionOnPostback="true" CodeFile="Auto.aspx.cs" Inherits="Msg_Auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">  
     <table id="tblBody" width="100%">
        <tr><td align="right">對應班別：
            <asp:DropDownList ID="SelYYYY" runat="server" ForeColor="Green" AutoPostBack="true" OnSelectedIndexChanged="SaveTour_Changed">
            </asp:DropDownList>年 &nbsp;&nbsp;
            <asp:DropDownList ID="SelMM" runat="server" ForeColor="Green" AutoPostBack="true" OnSelectedIndexChanged="SaveTour_Changed">
            </asp:DropDownList>月 &nbsp;&nbsp;
            <asp:DropDownList ID="SelDD" runat="server" ForeColor="Green" AutoPostBack="true" OnSelectedIndexChanged="SaveTour_Changed">
            </asp:DropDownList>日 &nbsp;&nbsp;
            <asp:DropDownList ID="SelClass" runat="server" ForeColor="Green" AutoPostBack="true" OnSelectedIndexChanged="SaveTour_Changed">
                <asp:ListItem Value="1">早</asp:ListItem>
                <asp:ListItem Value="2">午</asp:ListItem>
                <asp:ListItem Value="3">晚</asp:ListItem>
            </asp:DropDownList>班 &nbsp;&nbsp;
        </td></tr>
        <tr>
            <td align="left">
                <asp:GridView ID="GridView1" runat="server" AllowPaging="false" AllowSorting="True" Width="100%" Caption="" CaptionAlign="Left" 
                    AutoGenerateColumns="False" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" CellSpacing="2"
                    DataSourceID="SqlDataSourceSearch" ForeColor="Black" Font-Size="Medium" OnRowDataBound="GridView1_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="Serial" HeaderText="識別碼" SortExpression="Serial" ItemStyle-Width="60" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="訊息時間" HeaderText="時間" SortExpression="WarnDT" ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center" />
                        <asp:HyperLinkField DataTextField="WarnCode" DataNavigateUrlFields="WarnCode" DataNavigateUrlFormatString="/sop/sop.asp?SEL_Code={0}" HeaderText="訊息代碼" SortExpression="WarnCode" ItemStyle-Width="72" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue" Target="_blank" />
                        <asp:BoundField DataField="WarnMsg" HeaderText="訊息內容" SortExpression="WarnMsg" HtmlEncode="false" />                        
                        <asp:HyperLinkField DataTextField="訊息狀態" DataNavigateUrlFields="Serial" DataNavigateUrlFormatString="Auto.aspx?DBaction=w&Serial={0}" HeaderText="執行" ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue" />
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="對應">
                            <ItemTemplate>
                                <asp:DropDownList ID="SelDiaryNo" runat="server" ForeColor="Green" AutoPostBack="true" DataSourceID="SqlDataSourceDiaryNo" DataValueField="DiaryNo" DataTextField="TextNo" OnSelectedIndexChanged="SelDiaryNo_SelectedIndexChanged">
                                </asp:DropDownList>
                            </ItemTemplate>                            
                        </asp:TemplateField>                        
                    </Columns>
                    <FooterStyle BackColor="#CCCCCC" />
                    <HeaderStyle Font-Bold="false" HorizontalAlign="Center" />
                    <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Center" />
                    <RowStyle />
                    <SelectedRowStyle BackColor="#000099" ForeColor="White" />                    
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#808080" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#383838" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceSearch" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" SelectCommand=""></asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceDiaryNo" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"></asp:SqlDataSource>                
            </td>
        </tr>
    </table>
    <asp:Label ID="lblTip" runat="server" Font-Size="X-Large" ForeColor="Green" />
    <br />
    <div align="center"><font size="2" color="">訊息內容：異常處理程序　<font color="red">紅</font>：近一週　<font color="green">綠</font>：近一月　<font color="blue">藍</font>：近一年　<font color="black">黑</font>：一年以上　<font color="brown">棕</font>：無</font></div>

</asp:Content>
