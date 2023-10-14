<%@ Page Title="參數設定" Language="C#" MasterPageFile="../MasterPage.master" AutoEventWireup="true" validateRequest="false" Debug="true" MaintainScrollPositionOnPostback="true" CodeFile="Config.aspx.cs" Inherits="Config_Config" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .style8
        {
            border-style: solid;
            border-width: 1px;
        }
        .style16
        {
            text-align: center;
        }
        .style18
        {
            font-weight: bold;
            text-align: center;
            font-family: 標楷體;
            font-size: x-large;
        }
        .style19
        {
            text-align: center;
            font-size: medium;
            font-family: 標楷體;
        }
        .style20
        {
            text-align: center;
            color: #006600;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="divBody" align="center" style="width: 100%; overflow: auto;">
    <table border="1" cellpadding="0" cellspacing="0" class="style8" align="center">
        <tr>
            <td class="style18"><strong>參數類型</strong></td>
            <td class="style18" colspan="4">參數設定</td>
        </tr>
        
        <tr>
            <td align="center">
                <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" 
                    DataTextField="Item" DataValueField="Item"  
                    DataSourceID = "SqlDataSource1"                 
                    onselectedindexchanged="DropDownList1_SelectedIndexChanged" 
                    ForeColor="#009933">
                </asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                    SelectCommand = "SELECT [Item] FROM [Config] WHERE ([Kind] = @Kind) ORDER BY [mark]"
                    ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="Kind" QueryStringField="Kind" Type="String" DefaultValue="工作日誌" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </td>
            <td class="style19">
                項目：</td>
            <td class="style16">
                <asp:TextBox ID="TextItem" runat="server" 
                    Columns="24"></asp:TextBox>
            </td>
            <td class="style19">
                設定：</td>
            <td class="style16">
                <asp:TextBox ID="TextConfig" runat="server" 
                    Columns="24"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td rowspan="2" align="center">
                <asp:ListBox ID="ListBox1" runat="server" AutoPostBack="True" 
                    DataTextField="Item" DataValueField="Item" Rows="22" 
                    onselectedindexchanged="ListBox1_SelectedIndexChanged"></asp:ListBox>
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:DiaryConnectionString %>" 
                    SelectCommand="SELECT [Item] FROM [Config] WHERE ([Kind] = @Kind) ORDER BY [mark]">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DropDownList1" Name="Kind" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </td>
            <td class="style19">
                註解：</td>
            <td colspan="3" class="style16">
                <asp:TextBox ID="TextMark" runat="server" 
                    Columns="68"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style19">
                說明：</td>
            <td colspan="3" class="style16">
                <asp:TextBox ID="TextMemo" runat="server" 
                    Columns="50" Rows="20" 
                    TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style19">
                <asp:Button ID="BtnUp" runat="server" Text="上提" onclick="BtnUp_Click" ToolTip="樹狀系統設定之顯示介面上提一階層" />
                <asp:Button ID="BtnDn" runat="server" Text="下探" onclick="BtnDn_Click" ToolTip="樹狀系統設定之顯示介面下探一階層" />                
            </td>
            <td colspan="4" class="style16">
                <asp:Button ID="BtnAdd" runat="server" Text="新增" onclick="BtnAdd_Click" />　　
                <asp:Button ID="BtnEdit" runat="server" Text="修改" onclick="BtnEdit_Click" />　　
                <asp:Button ID="BtnDel" runat="server" Text="刪除" OnClientClick="return confirm('刪除前請再一次確認，所有引用到此參數的資料均已先刪除或修改，您確定要刪除這筆資料嗎？')" onclick="BtnDel_Click" />
            </td>
        </tr>
    </table>

    <p align="center">
        <asp:Label ID="lblKind" runat="server" ForeColor="Red" Font-Size="Smaller"></asp:Label> <br /><br />
        關鍵字：<asp:TextBox ID="TextKey" runat="server"></asp:TextBox>&nbsp;&nbsp;
        <asp:Button ID="BtnSearch" runat="server" Text=" 查　詢 " OnClick="BtnSearch_Click" /> <br />
        <asp:Label ID="lblKey" runat="server" Text="" Font-Size="Smaller"></asp:Label>
    </p>
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