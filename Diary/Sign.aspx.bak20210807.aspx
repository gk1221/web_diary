<%@ Page Title="簽名作業" Language="C#" AutoEventWireup="true" Debug="true" MaintainScrollPositionOnPostback="true" Inherits="Diary_Sign" CodeFile="Sign.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <style type="text/css">
        .style1
        {
            text-align: center;
        }
    </style>
</head>
<body runat="server" id="pMaster">
<form id="form1" runat="server">
    <div id="divBody" align="center" style="width: 100%; overflow: auto;">
        <asp:Label ID="lblTour" ForeColor="Green" Font-Size="X-Large" runat="server" />
        <hr />
        <asp:GridView ID="GridView1" runat="server" AllowPaging="false" AllowSorting="True"
            Caption="" CaptionAlign="Left" AutoGenerateColumns="False" BorderColor="#999999"
            BorderStyle="Solid" BorderWidth="3px" CellPadding="1" CellSpacing="1" DataSourceID="SqlDataSourceOP"
            ForeColor="Black" Font-Size="Medium">
            <Columns>                
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="">
                    <HeaderTemplate><asp:CheckBox ID="ChkRout" runat="server" Enabled="false" /></HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="成員" HeaderText="當 班 者" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center" />
				
            </Columns>
            <FooterStyle BackColor="#CCCCCC" />
            <HeaderStyle HorizontalAlign="Center" />
            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Center" />
            <RowStyle />
            <SelectedRowStyle BackColor="#000099" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#808080" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#383838" />
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSourceOP" runat="server" ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>"
          
			SelectCommand="select [成員] from [View_組織架構] where [性質]='員工' and [課別]='作業管理科' order by case when [成員]='宋紹良' then 1 end DESC, [代號] ASC"> 
			
			
		</asp:SqlDataSource>
        <br />

        <table>
            <tr>
                <td>第一機房最高　溫度：<asp:TextBox ID="txtTemp1" runat="server" ForeColor="Green" Width="40px" CssClass="style0" /></td>
                <td>&nbsp;&nbsp;溼度：<asp:TextBox ID="txtHumi1" runat="server" ForeColor="Green" Width="40px" CssClass="style0" /></td>
            </tr>
            <tr>
                <td>第一機房最低　溫度：<asp:TextBox ID="txtTemp2" runat="server" ForeColor="Green" Width="40px" CssClass="style0" /></td>
                <td>&nbsp;&nbsp;溼度：<asp:TextBox ID="txtHumi2" runat="server" ForeColor="Green" Width="40px" CssClass="style0" /></td>
            </tr>
            <tr>
                <td>第二機房最高　溫度：<asp:TextBox ID="txtTemp3" runat="server" ForeColor="Green" Width="40px" CssClass="style0" /></td>
                <td>&nbsp;&nbsp;溼度：<asp:TextBox ID="txtHumi3" runat="server" ForeColor="Green" Width="40px" CssClass="style0" /></td>
            </tr>
            <tr>
                <td>第二機房最低　溫度：<asp:TextBox ID="txtTemp4" runat="server" ForeColor="Green" Width="40px" CssClass="style0" /></td>
                <td>&nbsp;&nbsp;溼度：<asp:TextBox ID="txtHumi4" runat="server" ForeColor="Green" Width="40px" CssClass="style0" /></td>
            </tr>
        </table>
        <br />

        <asp:Button ID="BtnSave" runat="server" Text=" 完 成 "  OnClick="BtnSave_Click"/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="BtnExit" runat="server" Text=" 離 開 " OnClientClick="window.close();" />
    </div>
    </form>
</body>
</html>
