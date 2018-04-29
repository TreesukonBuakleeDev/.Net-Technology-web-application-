<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="product.aspx.vb" Inherits="invoice.product" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title>Product Form</title>
    
<!-- JavaScript -->
<script language="javascript" type="text/javascript">
    function openListOfValue(mode, table, initSQL, columnname) {
        window.open("listofvalue.aspx?mode=" + mode + "&table=" + table + "&initSQL=" + initSQL + "&columnname=" + columnname, "popup", "width=600,height=350");
    }

    //function getListOfValue(dataArray, table, mode) {
    function getListOfValue(dataArray, table) {
        /* get value from invoice list */
        if (table == "Product") {
            document.getElementById('txtProductCode').disabled = false;
            document.getElementById('txtProductCode').value = dataArray[0];
            setDirtyBit('CLEAR');
        }

        formProduct.submit();
    }

    function checkDirty(button) {
        var DirtyBit = document.getElementById('DirtyBit').value;
        var strConfirm;

        if (DirtyBit == "DIRTY") {
            if (button == "NEW") {
                strConfirm = "Changes not yet saved. Are you sure you want to discard changes and start a new product?";
            }
            else if (button == "EDIT") {
                strConfirm = "Changes not yet saved. Are you sure you want to discard changes and load a new product?";
            }
            else if (button == "COPY") {
                strConfirm = "Changes not yet saved. Are you sure you want to discard changes and copy a new product?";
            }
            else if (button == "PRINT") {
                strConfirm = "Changes not yet saved. Are you sure you want to discard changes and print this product?";
            }
            else if (button == "CLOSE") {
                strConfirm = "Changes not yet saved. Are you sure you want to discard changes and close this form?";
            }

            if (confirm(strConfirm) == true)
                return true;
            else {
                return false;
            }
        }
        else {
            return true;
        }
    }

    function printProduct() {
        var txtInvoiceNo = document.getElementById("txtProductCode").value;

        if (txtProductCode == '' || txtProductCode == "NEW") {
            alert("Please select product for print");
        }
        else {
            window.open("frmCrystalReport_Product.aspx?ProductCode=" + txtProductCode);
        }
    }

    function checkClose() {
        if (confirm('Do you want to exit ?') == true) {
            if (checkDirty('CLOSE') == true) {
                var win = window.open("", "_self"); win.close();
            }
        }
    }

    function setDirtyBit(DirtyBit) {
        document.getElementById('DirtyBit').value = DirtyBit;
    }

</script>

<!-- style sheet -->
<style type="text/css">
body {
    font: 13px "Trebuchet MS", Arial, Helvetica, sans-serif;
    text-align: center;
    background: #034E85;
}
#main #middle #middle2 #middle3 table tr th {
	 color: #FBFBF5;
}
#lineItem{
    height:150px;
	width:850px;
	overflow:scroll;
	overflow-x:hidden
}
input[type="text"]{
	height:18px;               
}
</style>
</head>
<body>
    <form id="formProduct" runat="server" method="post">
    <div id="main">
        <div id="header">
        <h1>&nbsp;</h1>
        <!-------------------------- Menu -->
        <table border="none" cellspacing="0" cellpadding="0" style="border:none">
        <tr style="border:none">
            <td align="center" class="border">
            <asp:ImageButton ID="btnNew" runat="server" ImageUrl="~/image/btnNew.png"></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbNew" runat="server"></asp:Label></div>
            </td>
            <td align="center" class="border">
            <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/image/btnEdit.png" ></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbEdit" runat="server"></asp:Label></div>
            </td>
            <td align="center" class="border">
            <asp:ImageButton ID="btnCopy" runat="server" ImageUrl="~/image/btnCopy.png"></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbCopy" runat="server"></asp:Label></div>
            </td>
            <td align="center" class="border">
            <asp:ImageButton ID="btnSave" runat="server" ImageUrl="~/image/btnSave.png"></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbSave" runat="server"></asp:Label></div>
            </td>
            <td align="center" class="border">
            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/image/btnDelete.png"></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbDelete" runat="server"></asp:Label></div>
            </td>
            <td align="center" class="border">
            <asp:ImageButton ID="btnPrint" runat="server" ImageUrl="~/image/btnPrint.png"></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbPrint" runat="server"></asp:Label></div>
            </td>
            <td align="center" class="border">
            <asp:ImageButton ID="btnClose" runat="server" ImageUrl="~/image/btnClose.png"></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbClose" runat="server"></asp:Label></div>
            </td>
            <td width="100%" align="right" style="border:none"><h1>Product Form</h1></td>
        </tr>
        </table>
        </div>

        <!-------------------------- Header -->
        <div id="middle" style="height:480px">
        <div id="middle2">
        <div id="middle3">
        <table border="0" cellpadding="0" cellspacing="0">
        <tr height="32" >
            <td width="125"><span class="red_star">* </span><asp:Label ID="lbProductCode" 
                    runat="server"></asp:Label>&nbsp;:</td>
            <td width="150">
                <asp:TextBox ID="txtProductCode" runat="server" AutoPostBack="True" 
                    Width="120px"></asp:TextBox></td>   
            <td width="85">&nbsp;</td>     
            <td width="130">&nbsp;</td>
            <td width="145">&nbsp;</td>
        </tr>
        <tr height="32" >
            <td><span class="red_star">* </span><asp:Label ID="lbProductName" runat="server"></asp:Label>&nbsp;:</td> 
            <td><asp:TextBox ID="txtProductName" runat="server" AutoPostBack="True" 
                    Width="120px"></asp:TextBox>
                </td>     
            <td>&nbsp;</td>  
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr height="32" >
            <td><span class="red_star">* </span><asp:Label ID="lbUnits" runat="server"></asp:Label>&nbsp;:</td> 
            <td><asp:TextBox ID="txtUnits" runat="server" AutoPostBack="True" 
                    Width="120px"></asp:TextBox></td>     
            <td>&nbsp;</td>  
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr height="32" >
            <td><span class="red_star">* </span><asp:Label ID="lbUnitPrice" runat="server"></asp:Label>&nbsp;:</td> 
            <td><asp:TextBox ID="txtUnitPrice" runat="server" Width="120px" AutoPostBack="True"></asp:TextBox></td>     
            <td>&nbsp;</td>  
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr  height="32" >
            <td>&nbsp;</td>
        </tr>
        </table>

        <table border="0">
        <tr>
            <td>&nbsp;</td>
        </tr>
        <asp:HiddenField ID="DirtyBit" runat="server" />
        
           
        </table>

        <!-------------------------- Total Amount -->
        </div>

        <!-------------------------- Hidden Field -->
        
           
    </div>
    
    </div>
          
        <div class="clearing">&nbsp;</div>
    
       
    <div id="footer" style="text-align:right">
        <asp:ImageButton ID="btnEN" runat="server" ImageUrl="~/image/EN.png"></asp:ImageButton>
        <asp:ImageButton ID="btnTH" runat="server" ImageUrl="~/image/TH.png"></asp:ImageButton>
    </div>
    </div>
    


    </form>
</body>
</html>
<link rel="stylesheet" href="/css/style.css" type="text/css" />