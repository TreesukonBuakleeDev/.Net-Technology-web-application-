<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="listofvalue.aspx.vb" Inherits="invoice.listofvalue" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<link rel="stylesheet" type="text/css" href="StyleSheet1.css"/>
<link rel="stylesheet" href="css/style.css" />
<script type="text/javascript" src="jquery-ui-1.9.1.custom/js/jquery-1.8.2.js"></script>  
<script type="text/javascript" src="jquery-ui-1.9.1.custom/js/jquery-ui-1.9.1.custom.js"></script>  
<script type="text/javascript" src="jquery-ui-1.9.1.custom/tablesorter/jquery.tablesorter.min.js"></script>
<link rel="stylesheet" href="jquery-ui-1.9.1.custom/tablesorter/themes/blue/style.css" />

<title>List of Value</title>
<style type="text/css">
#divList{
	height:200px;
	width:80%;
	overflow:scroll;
	overflow-x:auto;
	border:1px dashed White;
	padding-left:5px;
	padding-right:5px;
    margin:0 auto;
}   
#gridListOfValue tr:hover td{
	cursor:pointer ;
	background-color:#ebebfe;	
}
#gridListOfValue{	
	font: 13px "Trebuchet MS", Arial, Helvetica, sans-serif;
}
</style>


<script language="javascript" type="text/javascript">
    
    /* jQuery */
    $(document).ready(function () {
        $("#gridListOfValue").tablesorter();
    });

    /* get header text when clicked */
    window.onload = function () {
        var grid = document.getElementById("<%=gridListOfValue.ClientID %>");
        var headerCells = grid.getElementsByTagName("th");
        for (var i = 0; i < headerCells.length; i++) {
            headerCells[i].onclick = function () {
                setSelectedValue(this.innerHTML);
            };
        }
    }


    function setSelectedValue(columnName) {
        var txtColumnName = document.getElementById('txtColumnName');
        txtColumnName.value = columnName;
    }

    function getListOfValue(rowData) {
        var dataArray = rowData.split(";");
        //opener.getListOfValue(dataArray, '<%=Session("table")%>', '<%=Session("mode")%>');
        opener.getListOfValue(dataArray, '<%=Session("table")%>');
        window.close();
    }

</script>
</head>
<body>
    <form id="formList" runat="server">
    <h1><asp:Label ID="lbTitle" runat="server" Text="Label"></asp:Label></h1>
    <br>
    <div id="divList">    
        <asp:GridView ID="gridListOfValue" runat="server" CellPadding="0" 
            ForeColor="#333333" GridLines="None" 
            CellSpacing="1" HorizontalAlign="Center" EmptyDataText="data not found" >
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:CommandField ButtonType="Image" SelectImageUrl="~/image/lineSelect.png" 
                    ShowSelectButton="True" />
            </Columns>
        <EditRowStyle BackColor="#999999" />
            <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" BorderColor="White" />
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#E9E7E2" />
        <SortedAscendingHeaderStyle BackColor="#506C8C" />
        <SortedDescendingCellStyle BackColor="#FFFDF8" />
        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
        </asp:GridView>
    </div>
    <br>
    <div align="center">
        <table>
        <tr valign="middle">
            <td width="50px" align="left" style="color:#FFF">
                <asp:Label ID="lbSearch" runat="server"></asp:Label>
            </td>
            <td><asp:TextBox ID="txtColumnName" runat="server" Width="100px"></asp:TextBox></td>
            <td><asp:DropDownList ID="listCondition" runat="server" Width="90px" 
                    AutoPostBack="True"></asp:DropDownList></td>
            <td><asp:TextBox ID="txtSearch1" runat="server" Width="100px"></asp:TextBox></td>
            <td><asp:TextBox ID="txtSearch2" runat="server" Width="100px"></asp:TextBox></td>
            <td><asp:Button ID="btnSearch" runat="server" /></td>
            <td><asp:Button ID="btnExpand" runat="server" /></td>
        </tr>
        </table>
        
        <asp:HiddenField ID="sqlCondition" runat="server" />
    </div>
    </form>
</body>
</html>
