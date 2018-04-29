<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ES.aspx.vb" Inherits="invoice.ES" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <br />
    
    </div>
       <asp:Button ID="Button1" runat="server" Text="Import from Excel to SQL Server"
            onclick="Button1_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <br />
        <asp:Label ID="Label1" runat="server"></asp:Label>
    </form>
</body>
</html>

