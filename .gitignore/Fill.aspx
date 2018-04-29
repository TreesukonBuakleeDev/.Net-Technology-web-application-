<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Fill.aspx.vb" Inherits="invoice.Fill" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>Invoice Form</title>
    
<!-- include file jQuery -->
<script src="jquery-ui-1.9.1.custom/js/jquery-1.8.2.js"></script>
<script src="jquery-ui-1.9.1.custom/js/jquery-ui-1.9.1.custom.js"></script>
<link rel="stylesheet" href="jquery-ui-1.9.1.custom/css/redmond/jquery-ui-1.9.1.custom.css" />

<!-- JavaScript -->
<script language="javascript" type="text/javascript">
    /* use jQuery for show calendar */
    $(function () {
        $('#txtOrderDate').datepicker({ dateFormat: 'dd/mm/yy' });
    });


    function openListOfValue(mode, table, initSQL, columnname) {
        window.open("listofvalue.aspx?mode=" + mode + "&table=" + table + "&initSQL=" + initSQL + "&columnname=" + columnname, "popup", "width=600,height=350");
    }

    //function getListOfValue(dataArray, table, mode) {
    function getListOfValue(dataArray, table) {
        /* get value from invoice list */
        if (table == "FillingGas") {
            document.getElementById('txtFillingGasNo').disabled = false;
            document.getElementById('txtFillingGasNo').value = dataArray[0];
            setDirtyBit('CLEAR');

            //setMode(mode);               
        }
        else if (table == "Driver") {
            setDirtyBit('DIRTY');
            document.getElementById('txtDriverID').value = dataArray[0];
        }
        else if (table == "Truck") {
            setDirtyBit('DIRTY');
            document.getElementById('txtTruckID').value = dataArray[0];
        }

        else if (table == "OrderLine") {
            setDirtyBit('DIRTY');
            document.getElementById('<%= txtOrderNoLine.ClientID%>').value = dataArray[0];
        }

       <%-- else if (table == "MaterialLine") {
            setDirtyBit('DIRTY');
            document.getElementById('<%= txtMaterialCode.ClientID%>').value = dataArray[0];
        }--%>

        formInvoice.submit();
    }

    function checkDirty(button) {
        var DirtyBit = document.getElementById('DirtyBit').value;
        var strConfirm;

        if (DirtyBit == "DIRTY") {
            if (button == "NEW") {
                strConfirm = "Changes not yet saved. Are you sure you want to discard changes and start a new invoice?";
            }
            else if (button == "EDIT") {
                strConfirm = "Changes not yet saved. Are you sure you want to discard changes and load a new invoice?";
            }
            else if (button == "COPY") {
                strConfirm = "Changes not yet saved. Are you sure you want to discard changes and copy a new invoice?";
            }
            else if (button == "PRINT") {
                strConfirm = "Changes not yet saved. Are you sure you want to discard changes and print this invoice?";
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

    function printInvoice() {
        var txtFillingGasNo = document.getElementById("txtFillingGasNo").value;

        if (txtFillingGasNo == '' || txtFillingGasNo == "NEW") {
            alert("Please select invoice for print");
        }
        else {
            window.open("Report.aspx?FillingGasNo=" + txtFillingGasNo);
        }
    }

    function checkClose() {
        if (confirm('Do you want to exit ?') == true) {
            if (checkDirty('CLOSE') == true) {
                var win = window.open("", "_self"); window.close();
            }
        }
    }

    function setDirtyBit(DirtyBit) {
        document.getElementById('DirtyBit').value = DirtyBit;
    }

    function setMode(mode) {
        document.getElementById('mode').value = mode;
    }

    <%-- function calExtendedPrice() {
        var Quantity = parseFloat(document.getElementById('<%=txtQuantity.ClientID %>').value); 
        var UnitPrice = parseFloat(document.getElementById('<%=txtUnitPrice.ClientID %>').value.replace(/\,/g, "")); 
      
        if (Quantity != '' && UnitPrice != '') {
            document.getElementById('<%=txtExtendedPrice.ClientID %>').value = Number(Quantity*UnitPrice).toFixed(2);
        }
    }--%>
</script>

<!-- style sheet -->
<style type="text/css">
body {
    font: 13px "Trebuchet MS", Arial, Helvetica, sans-serif;
    text-align: center;
    background: #222;
}
#main #middle #middle2 #middle3 table tr th {
	 color: #222;
}
#lineItem{
    height:150px;
	width:850px;
	overflow:scroll;
	overflow-x:hidden
}
input[type="text"]{
    }
    .auto-style1 {
        width: 271px;
    }
    .auto-style2 {
        width: 87px;
    }
    .auto-style3 {
        height: 33px;
    }
    .auto-style4 {
        width: 87px;
        height: 33px;
    }
    .auto-style5 {
        width: 271px;
        height: 33px;
    }
</style>
</head>
<body>
    <form id="formInvoice" runat="server" method="post" style="background-color: #000000">
    <div id="main">
        <div id="header" style="background-color: #000000">
        <h1>&nbsp;</h1>
        <!-------------------------- Menu -->
        <table border="none" cellspacing="0" cellpadding="0" style="border:none" bgcolor="#666666">
        <tr style="border:none">
            <td align="center" class="border" bgcolor="Black">
            <asp:ImageButton ID="btnNew" runat="server" ImageUrl="~/image/btnNew.png"></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbNew" runat="server"></asp:Label></div>
            </td>
            <td align="center" class="border" bgcolor="Black">
            <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/image/btnEdit.png" ></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbEdit" runat="server"></asp:Label></div>
            </td>
            <td align="center" class="border" bgcolor="Black">
            <asp:ImageButton ID="btnCopy" runat="server" ImageUrl="~/image/btnCopy.png"></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbCopy" runat="server"></asp:Label></div>
            </td>
            <td align="center" class="border" bgcolor="Black">
            <asp:ImageButton ID="btnSave" runat="server" ImageUrl="~/image/btnSave.png"></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbSave" runat="server"></asp:Label></div>
            </td>
            <td align="center" class="border" bgcolor="Black">
            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/image/btnDelete.png"></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbDelete" runat="server"></asp:Label></div>
            </td>
            <td align="center" class="border" bgcolor="Black">
            <asp:ImageButton ID="btnPrint" runat="server" ImageUrl="~/image/btnPrint.png"></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbPrint" runat="server"></asp:Label></div>
            </td>
            <td align="center" class="border" bgcolor="Black">
            <asp:ImageButton ID="btnClose" runat="server" ImageUrl="~/image/btnClose.png"></asp:ImageButton>
            <div class="text_menu"><asp:Label ID="lbClose" runat="server"></asp:Label></div>
            </td>
            <td width="100%" align="right" style="border:none" bgcolor="Black"><h1>Filling Gas Form</h1></td>
        </tr>
        </table>
        </div>

        <!-------------------------- Header -->
        <div id="middle" style="height:480px">
        <div id="middle2">
        <div id="middle3">
        <table border="0" cellpadding="0" cellspacing="0" style="width: 820px">
        <tr height="32" >
            <td width="125"><span class="red_star">* </span><asp:Label ID="lbFillingGasNo" runat="server"></asp:Label>&nbsp;:</td>
            <td class="auto-style2"><asp:TextBox ID="txtFillingGasNo" runat="server" AutoPostBack="True" 
                    Width="80px"></asp:TextBox></td>   
            <td class="auto-style1"><asp:Label ID="lbTruckID" runat="server"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:TextBox ID="txtTruckID" runat="server" AutoPostBack="True" 
                    Width="80px"></asp:TextBox>
                <asp:ImageButton ID="btnGetTruck" runat="server" Height="20px" ImageUrl="~/image/btnSearch.png" CssClass="example4demo" /></td>     
            <td width="130"><asp:Label ID="lbTruckLicense" runat="server"></asp:Label></td>
            <td width="145"><asp:TextBox ID="txtTruckLicense" runat="server" AutoPostBack="True" 
                    Width="100px"></asp:TextBox></td>
        </tr>
        <tr >
            <td class="auto-style3"><span class="red_star">* </span>&nbsp;<asp:Label ID="lbFillingGasDate" runat="server"></asp:Label>:</td> 
            <td class="auto-style4"><asp:TextBox ID="txtFillingGasDate" runat="server" AutoPostBack="True" 
                    Width="80px"></asp:TextBox></td>     
            <td class="auto-style5">&nbsp;<asp:Label ID="lbDriverID" runat="server"></asp:Label>:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:TextBox ID="txtDriverID" runat="server" AutoPostBack="True" Width="80px"></asp:TextBox>
                <asp:ImageButton ID="btnGetDriver" runat="server" Height="20px" ImageUrl="~/image/btnSearch.png" CssClass="example4demo" /></td>  
            <td class="auto-style3"><span class="red_star">*</span>&nbsp;<asp:Label ID="lbDriverName" runat="server"></asp:Label></td>
            <td class="auto-style3"> <asp:TextBox ID="txtDriverName" runat="server" Width="106px"></asp:TextBox></td>
        </tr>
        <tr  height="32" >
            <td>&nbsp;</td>
        </tr>
        </table>

        <table border="0">
        <tr>
            <td><asp:Label ID="lbInvoiceLine" runat="server"></asp:Label></td>
        </tr>
        </table>

        <p>
        
        <!-------------------------- Line Item -->

        <div id="lineItem" style="border:1px dashed #034E85;padding:5px;">
        <asp:GridView ID="gridFillingLineItem" runat="server" CellPadding="4" 
                BackColor="White" BorderColor="Black" BorderStyle="Solid" 
                BorderWidth="1px">
            <Columns>
                <asp:CommandField ButtonType="Image" 
                    CancelImageUrl="~/image/lineDelete.png" 
                    DeleteImageUrl="~/image/lineDelete.png" 
                    EditImageUrl="~/image/lineEdit.gif" ShowEditButton="True" 
                    UpdateImageUrl="~/image/lineAccept.png" />
                <asp:CommandField ButtonType="Image" 
                    DeleteImageUrl="~/image/lineDelete.png" ShowDeleteButton="True" />
                <asp:TemplateField HeaderText="#">
                     <ItemTemplate>
                        <asp:Label ID="RunningNumber" runat="server"><%# Container.DisplayIndex + 1%></asp:Label>
                     </ItemTemplate>
                </asp:TemplateField>
                          </Columns>
            <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
            <HeaderStyle BackColor="#034E85" Font-Bold="True" ForeColor="#CCCCFF" />
            <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="Left" />
            <RowStyle BackColor="White" ForeColor="#003399" />
            <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
            <SortedAscendingCellStyle BackColor="#EDF6F6" ForeColor="#034E85" />
            <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
            <SortedDescendingCellStyle BackColor="#D6DFDF" />
            <SortedDescendingHeaderStyle BackColor="#002876" />
        </asp:GridView>
        </div>

        <!-------------------------- Total Amount -->
        <table>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
           <td style="width:700px"></td>
           <td style="width:140px"><asp:Label ID="lbTotalFilling" runat="server"></asp:Label>&nbsp;:</td>
           <td><asp:TextBox ID="txtTotalFilling" runat="server"></asp:TextBox></td>
        </tr>
        <tr> 
            <td style="width:700px"></td>
            <td><asp:Label ID="lbVAT" runat="server"></asp:Label>&nbsp;:</td> 
            <td><asp:TextBox ID="txtVAT" runat="server"></asp:TextBox></td> 
        </tr>
        <tr>
            <td style="width:700px"></td>
            <td><asp:Label ID="lbAmountDue" runat="server"></asp:Label>&nbsp;:</td>
            <td><asp:TextBox ID="txtAmountDue" runat="server"></asp:TextBox></td>
        </tr>
        </table>        
        </div>
        <p>
        <p>
        <p>
        <p>

        <!-------------------------- Panel Insert Product -->
        <asp:Panel ID="panelProduct" runat="server" Visible="False">
        <table>
        <tr>
        <td>
        <asp:TextBox ID="txtOrderNoLine" runat="server" Width="145px" AutoPostBack="True" TabIndex="1"></asp:TextBox>
        <asp:ImageButton ID="btnGetOrderNo" runat="server" ImageUrl="~/image/btnSearch.png" 
                Height="20px" Width="20px" />
        </td>

               <td>
        <asp:TextBox ID="txtCompartmentNo" runat="server" Width="145px"></asp:TextBox>
        </td>

        <td>
        <asp:TextBox ID="txtProductID" runat="server" Width="145px"></asp:TextBox>
        </td>
        
        <td>
        <asp:TextBox ID="txtFillingQuantity" runat="server" Width="145px"></asp:TextBox>
        </td>


        <td style="width:100px">
        <asp:ImageButton ID="btnInsertProduct" runat="server" 
                ImageUrl="~/image/lineAccept.png" />
        <asp:ImageButton ID="btnCancelProduct" runat="server" 
                ImageUrl="~/image/lineDelete.png" />
        </td>
        </tr>
        </table>                        
        <asp:Label ID="lbRequireField0" runat="server" ForeColor="#CC3300" Text="*"></asp:Label>
        <asp:Label ID="lbRequireField1" runat="server" ForeColor="#CC3300" Text="*"></asp:Label>
        </asp:Panel>

        <!-------------------------- Hidden Field -->
        <asp:HiddenField ID="DirtyBit" runat="server" />
        
           
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

