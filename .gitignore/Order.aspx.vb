Public Class index
    Inherits System.Web.UI.Page
    ' global variable
    Dim mySqlCon As Data.OleDb.OleDbConnection
    Dim mySqlCmd As Data.OleDb.OleDbCommand
    Dim mySqlReader As Data.OleDb.OleDbDataReader
    Dim SQL As String

    Dim dataLineItem As DataTable
    Dim editLine As Integer

    Private Sub index_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        ' set local
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US")
        System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            btnNew_Click(Nothing, Nothing)              ' set mode NEW
            setLanguage("EN")                           ' set Language (EN)
            setLabel()                                  ' set Label 
            setButton()                                 ' set attribute of button
            setTextBoxStyle()                           ' set textbox style (format/alignment)

            If (dataLineItem Is Nothing) Then
                createNewDataLineItem()                 ' create datatable
            End If
            Session("dataLineItem") = dataLineItem      ' save datatable to session
            Session("editLine") = editLine = -1         ' set editline = -1
        Else
            dataLineItem = Session("dataLineItem")
            editLine = Session("editLine")
            setLabel()
        End If

        If Not (editLine = -1) Then
            insertLineItem(editLine)                    ' show textbox at the and of gridview for add lineitem
        End If

        setColumnName()                                 ' set columnname (TH/EN)

    End Sub

    Public Sub connectDB()
        ' connect to database
        Dim sConnString As String

        'sConnString = "Provider=SQLOLEDB.1;Data Source=SERVER_NAME;" & _
        '              "Initial Catalog=DB_NAME;User ID=USER;Password=PASSWORD"
        sConnString = "Provider=SQLOLEDB.1;Data Source=AUTO-PC\SQLEXPRESS;" & _
                      "Initial Catalog=Automation;User ID=CPE332;Password=cpe332ae"
        mySqlCon = New Data.OleDb.OleDbConnection(sConnString)
    End Sub

#Region "Setting"
    Public Sub setLanguage(ByVal language)
        ' set language to session 
        ' TH for Thai
        ' EN for English
        Session("language") = language
    End Sub
    Public Sub setLabel()
        If Session("language") = "EN" Then
            lbNew.Text = "NEW"
            lbEdit.Text = "EDIT"
            lbCopy.Text = "COPY"
            lbSave.Text = "SAVE"
            lbDelete.Text = "DELETE"
            lbPrint.Text = "PRINT"
            lbClose.Text = "CLOSE"

            lbOrderNo.Text = "OrderNo"
            lbFillingGasDate.Text = "FillingGasDate"
            lbFillingGasNo.Text = "FillingGas No"
            lbCustomerID.Text = "CustomerID"
            lbCustomerName.Text = "CustomerName"
            lbOrderDate.Text = "OrderDate"


            lbTotalWeight.Text = "Total Weight"
            'lbVAT.Text = "VAT"
            'lbAmountDue.Text = "Amount Due"

            'lbInvoiceLine.Text = "Invoice Line"

        ElseIf Session("language") = "TH" Then
            lbNew.Text = "สร้าง"
            lbEdit.Text = "แก้ไข"
            lbCopy.Text = "คัดลอก"
            lbSave.Text = "บันทึก"
            lbDelete.Text = "ลบ"
            lbPrint.Text = "พิมพ์"
            lbClose.Text = "ปิด"

            lbOrderNo.Text = "หมายเลขรายการ"
            lbFillingGasDate.Text = "วันที่เติมผลิตภัณฑ์"
            lbCustomerID.Text = "รหัสลูกค้า"
            lbCustomerName.Text = "ชื่อลูกค้า"
            lbOrderDate.Text = "วันที่ทำรายการ"
            lbTotalWeight.Text = "รวมราคา"
            'lbVAT.Text = "ภาษีมูลค่าเพิ่ม"
            'lbAmountDue.Text = "จำนวนเงินรวม"

            lbInvoiceLine.Text = "รายการสั่งซื้อ"

        End If
    End Sub
    Public Sub setButton()
        ' set attribute (ONCLICK) of button
        btnNew.Attributes.Add("onclick", "javascript:return checkDirty('NEW');")
        btnEdit.Attributes.Add("onclick", "javascript:if(checkDirty('EDIT')) {openListOfValue('EDIT','OrderHead','Select OrderNo, FillingGasDate From OrderHead WHERE (1=1)','OrderNo,FillingGasDate'); return false; } else {return false;}")
        btnCopy.Attributes.Add("onclick", "javascript:if(checkDirty('COPY') == true) { openListOfValue('COPY','OrderHead','Select OrderNo, FillingGasDate From OrderHead WHERE (1=1)','OrderNo,FillingGasDate'); return false; } else {return false;}")
        btnPrint.Attributes.Add("onclick", "javascript:printInvoice();")
        btnClose.Attributes.Add("onclick", "javascript:checkClose();")
        btnGetCustomer.Attributes.Add("onclick", "javascript:openListOfValue('','Customer','Select CustomerID, CustomerName From Customer WHERE (1=1)','CustomerID,CustomerName'); return false;")
        btnGetFill.Attributes.Add("onclick", "javascript:openListOfValue('','FillingGas','Select FillingGasNo, FillingGasDate From FillingGas WHERE (1=1)','FillingGasNo,FillingGasDate'); return false;")
        btnGetProduct.Attributes.Add("onclick", "javascript:openListOfValue('','Product','Select ProductID, ProductName From Product WHERE (1=1)','ProductID,ProductName'); return false;")
        btnGetCompart.Attributes.Add("onclick", "javascript:openListOfValue('','Truck','Select CompartmentNo From Truck WHERE (1=1)','CompartmentNo'); return false;")
    End Sub
    Public Sub setTextBoxStyle()
        'style textbox
        txtTotalWeight.Style("text-align") = "right"
        'txtVAT.Style("text-align") = "right"
        'txtAmountDue.Style("text-align") = "right"
    End Sub
    Public Sub setGridStyle()
        ' set width of column and style of text
        gridOrderLineItem.HeaderRow.Cells(0).Width = 25
        gridOrderLineItem.HeaderRow.Cells(1).Width = 25
        gridOrderLineItem.HeaderRow.Cells(2).Width = 30
        gridOrderLineItem.HeaderRow.Cells(3).Width = 140
        gridOrderLineItem.HeaderRow.Cells(4).Width = 140
        gridOrderLineItem.HeaderRow.Cells(5).Width = 100
        gridOrderLineItem.HeaderRow.Cells(6).Width = 100
        gridOrderLineItem.HeaderRow.Cells(7).Width = 100
        'gridProductLineItem.HeaderRow.Cells(8).Width = 100
        gridOrderLineItem.HeaderRow.Style("font-weight") = "normal"
        gridOrderLineItem.HeaderRow.Style("text-align") = "center"
        gridOrderLineItem.HeaderRow.Height = 22

        ' set alignment in each column
        For i = 0 To gridOrderLineItem.Rows.Count - 1
            gridOrderLineItem.Rows(i).Cells(0).Style("text-align") = "center"
            gridOrderLineItem.Rows(i).Cells(1).Style("text-align") = "center"
            gridOrderLineItem.Rows(i).Cells(2).Style("text-align") = "center"
            gridOrderLineItem.Rows(i).Cells(3).Style("text-align") = "left"
            gridOrderLineItem.Rows(i).Cells(3).Style("padding-left") = "5px"
            gridOrderLineItem.Rows(i).Cells(4).Style("text-align") = "left"
            gridOrderLineItem.Rows(i).Cells(4).Style("padding-left") = "5px"
            gridOrderLineItem.Rows(i).Cells(5).Style("text-align") = "right"
            gridOrderLineItem.Rows(i).Cells(5).Style("padding-right") = "5px"
            gridOrderLineItem.Rows(i).Cells(6).Style("text-align") = "right"
            gridOrderLineItem.Rows(i).Cells(6).Style("padding-right") = "5px"
            gridOrderLineItem.Rows(i).Cells(7).Style("text-align") = "right"
            gridOrderLineItem.Rows(i).Cells(7).Style("padding-right") = "5px"
            'gridProductLineItem.Rows(i).Cells(8).Style("text-align") = "right"
            'gridProductLineItem.Rows(i).Cells(8).Style("padding-right") = "5px"
            gridOrderLineItem.Rows(i).Height = 22
        Next

        gridOrderLineItem.Rows(gridOrderLineItem.Rows.Count - 1).Style("text-align") = "center"
    End Sub
    Public Sub setEnableTextbox()
        txtFillingGasDate.Enabled = True
        txtFillingGasNo.Enabled = True
        txtCustomerID.Enabled = True
        txtCustomerName.Enabled = True
        txtOrderDate.Enabled = True

        txtProductID.Enabled = True
        txtCompartmentNo.Enabled = True
        txtOrderQuantity.Enabled = True
        txtUnitPrice.Enabled = True
        txtExtendedPrice.Enabled = True
       

        If Session("mode") = "NEW" Or Session("mode") = "COPY" Then
            txtOrderNo.Enabled = False
        End If
    End Sub
    Public Sub setDirtyBit(ByVal value)
        ' set dirtybit : DIRTY/CLEAR
        DirtyBit.Value = value
    End Sub
    Protected Sub btnEN_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnEN.Click
        setLanguage("EN")           ' when click button EN, set language = EN
        setLabel()                  ' and set new label
        setColumnName()             ' set columnname
    End Sub
    Protected Sub btnTH_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnTH.Click
        setLanguage("TH")           ' when click button TH, set language = TH
        setLabel()                  ' and set new label
        setColumnName()             ' set columnname
    End Sub
#End Region

#Region "Menu"
    Protected Sub btnNew_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnNew.Click
        Session("mode") = "NEW"
        clearInvoiceField()
        clearCustomerField()
        clearFillField()
        clearProductfield()

        txtOrderNo.Text = "NEW"

        txtOrderDate.Text = Date.Today.ToString("dd/MM/yyyy")
        createNewDataLineItem()
        addLineItem()
        setDirtyBit("CLEAR")

        setEnableTextbox()
    End Sub
    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        Dim OrderNo As String

        ' Check Require Field
        If (txtFillingGasDate.Text = "") Then
            MsgBox("Please enter invoice date.")
            txtFillingGasDate.Focus()
            Exit Sub
        End If
        If (txtCustomerID.Text = "") Then
            MsgBox("Please enter customer code.")
            txtCustomerID.Focus()
            Exit Sub
        End If
        If dataLineItem.Rows.Count = 1 Then
            MsgBox("No data in lineitem.")
            txtProductID.Focus()
            Exit Sub
        End If


        If txtOrderNo.Text = "NEW" Then

            'Running Number
            Dim maxIN As String
            Dim strSplit As Array

            connectDB()
            mySqlCon.Open()

            Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()
            Dim Transaction = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted)
            command.Connection = mySqlCon
            command.Transaction = Transaction                           '*** Command & Transaction ***'
            command.CommandText = "SELECT MAX(OrderNo) FROM OrderHead"
            Try
                'command.Prepare()
                command.ExecuteNonQuery()
                mySqlReader = command.ExecuteReader()
                If mySqlReader.HasRows = True Then
                    mySqlReader.Read()
                    strSplit = Split(mySqlReader.Item(0), "IN")
                    maxIN = strSplit(1)
                Else
                    maxIN = 0
                End If
                OrderNo = "IN" + Format(maxIN + 1, "000")
                mySqlReader.Close()

                ' insert new invoice into db
                Try
                    command.CommandText = "INSERT INTO OrderHead(OrderNo, FillingGasDate, CustomerID, OrderDate, CustomerName, FillingGasNo, TotalWeight) " & _
                                          "VALUES (?,?,?,?,?,?,?)"
                    command.Parameters.Add("OrderNo", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("FillingGasDate", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("CustomerID", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("OrderDate", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("CustomerName", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("FillingGasNo", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("TotalWeight", Data.OleDb.OleDbType.Double)
                    ' command.Parameters.Add("TotalAmount", Data.OleDb.OleDbType.Numeric, 18)
                    'command.Parameters.Add("VAT", Data.OleDb.OleDbType.Numeric, 18)
                    'command.Parameters.Add("AmountDue", Data.OleDb.OleDbType.Numeric, 18)
                    command.Parameters(0).Value = OrderNo
                    command.Parameters(1).Value = txtFillingGasDate.Text
                    command.Parameters(2).Value = txtCustomerID.Text
                    command.Parameters(3).Value = txtOrderDate.Text
                    command.Parameters(4).Value = txtCustomerName.Text()
                    command.Parameters(5).Value = txtFillingGasNo.Text
                    command.Parameters(6).Value = txtTotalWeight.Text


                    ' command.Parameters(3).Value = Format(txtTotalWeight.Text, "General Number")
                    'command.Parameters(4).Value = Format(txtVAT.Text, "General Number")
                    'command.Parameters(5).Value = Format(txtAmountDue.Text, "General Number")
                    command.ExecuteNonQuery()

                    ' insert lineItem into db
                    Dim i As Integer
                    For i = 0 To dataLineItem.Rows.Count - 1
                        If i = editLine Then
                            Continue For
                        End If

                        command.Parameters.Clear()
                        command.CommandText = "INSERT INTO OrderLine (OrderNoLine, ProductID, CompartmentNo, OrderQuantity, UnitPrice, ExtendedPrice) " & _
                                                " VALUES (?,?,?,?,?,?)"
                        command.Parameters.Add("OrderNoLine", Data.OleDb.OleDbType.VarWChar, 50)
                        command.Parameters.Add("ProductID", Data.OleDb.OleDbType.VarWChar, 50)
                        command.Parameters.Add("CompartmentNo", Data.OleDb.OleDbType.VarWChar, 50)
                        command.Parameters.Add("OrderQuantity", Data.OleDb.OleDbType.Double)
                        command.Parameters.Add("UnitPrice", Data.OleDb.OleDbType.Double)
                        command.Parameters.Add("ExtendedPrice", Data.OleDb.OleDbType.Double)
                        'command.Parameters.Add("OrderList", Data.OleDb.OleDbType.Numeric, 18)

                        command.Parameters(0).Value = OrderNo
                        command.Parameters(1).Value = dataLineItem.Rows(i).Item(0)
                        command.Parameters(2).Value = dataLineItem.Rows(i).Item(1)

                        command.Parameters(3).Value = Format(dataLineItem.Rows(i).Item(2), "General Number")
                        command.Parameters(4).Value = Format(dataLineItem.Rows(i).Item(3), "General Number")
                        command.Parameters(5).Value = Format(dataLineItem.Rows(i).Item(4), "General Number")

                        ' command.Parameters(5).Value = Format(dataLineItem.Rows(i).Item(4), "General Number")
                        'command.Parameters(6).Value = (i + 1)
                        command.ExecuteNonQuery()

                    Next
                    Transaction.Commit() '*** Commit Transaction ***'
                    setDirtyBit("CLEAR")
                    showInvoice(OrderNo)
                    Session("mode") = "EDIT"
                Catch ex As Exception
                    Transaction.Rollback() '*** RollBack Transaction ***'
                    MsgBox(ex.Message)
                End Try

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
            mySqlCon.Close()

        Else
            ' Update
            OrderNo = txtOrderNo.Text
            connectDB()
            mySqlCon.Open()
            Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()
            Dim Transaction = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted)
            command.Connection = mySqlCon
            command.Transaction = Transaction                           '*** Command & Transaction ***'


            Try
                command.CommandText = "UPDATE OrderHead SET " & _
                                       "FillingGasDate=?, CustomerID=?, OrderDate=?, CustomerName=?, FillingGasNo=?, TotalWeight=? " & _
                                       "WHERE OrderNo =? "

                'command.Parameters.Add("ProductNo", Data.OleDb.OleDbType.VarWChar, 10)
                command.Parameters.Add("FillingGasDate", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("CustomerID", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("Orderdate", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("CustomerName", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("FillingGasNo", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("OrderNo", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("TotalWeight", Data.OleDb.OleDbType.Double)

                'command.Parameters(0).Value = txtProductNo.Text
                command.Parameters(0).Value = txtFillingGasDate.Text
                command.Parameters(1).Value = txtCustomerID.Text
                command.Parameters(2).Value = txtOrderDate.Text
                command.Parameters(3).Value = txtCustomerName.Text
                command.Parameters(4).Value = txtFillingGasNo.Text
                command.Parameters(5).Value = OrderNo
                command.Parameters(6).Value = txtTotalWeight.Text
                command.ExecuteNonQuery()

                command.Parameters.Clear()
                command.CommandText = "DELETE FROM OrderLine WHERE OrderNoLine = ?"
                command.Parameters.Add("OrderNoDel", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters(0).Value = OrderNo
                command.ExecuteNonQuery()

                Dim i As Integer
                For i = 0 To dataLineItem.Rows.Count - 1
                    If i = editLine Then
                        Continue For
                    End If
                    command.Parameters.Clear()
                    command.CommandText = "INSERT INTO OrderLine (OrderNoLine, ProductID, CompartmentNo, OrderQuantity, UnitPrice, ExtendedPrice) " & _
                                           "VALUES (?,?,?,?,?,?)"

                    command.Parameters.Add("OrderNoLine", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("ProductID", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("CompartmentNo", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("OrderQuantity", Data.OleDb.OleDbType.Double)
                    command.Parameters.Add("UnitPrice", Data.OleDb.OleDbType.Double)
                    command.Parameters.Add("ExtendedPrice", Data.OleDb.OleDbType.Double)
                    command.Parameters(0).Value = OrderNo
                    command.Parameters(1).Value = dataLineItem.Rows(i).Item(0)
                    command.Parameters(2).Value = dataLineItem.Rows(i).Item(1)
                    command.Parameters(3).Value = Format(dataLineItem.Rows(i).Item(2), "General Number")
                    command.Parameters(4).Value = Format(dataLineItem.Rows(i).Item(3), "General Number")
                    command.Parameters(5).Value = Format(dataLineItem.Rows(i).Item(4), "General Number")
                    'command.Parameters(5).Value = (i + 1)
                    command.ExecuteNonQuery()
                Next
                Transaction.Commit() '*** Commit Transaction ***'
                setDirtyBit("CLEAR")
                Session("mode") = "EDIT "
            Catch ex As Exception
                Transaction.Rollback() '*** RollBack Transaction ***'
                MsgBox(ex.Message)
            End Try
            mySqlCon.Close()
        End If
        insertLineItem(editLine)
    End Sub
    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete.Click

        If (txtOrderNo.Text = "") Or (txtOrderNo.Text = "NEW") Then
            txtOrderNo.Enabled = True
            MsgBox("Please select invoice for delete.")
            Exit Sub
        End If

        If (MsgBox("Do you want to delete Product No '" & txtOrderNo.Text & "' ", MsgBoxStyle.YesNo) = MsgBoxResult.Yes) Then
            Dim ProductNo As String
            ProductNo = txtOrderNo.Text

            connectDB()
            mySqlCon.Open()

            ' Create the Command. 
            Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()
            Dim Transaction = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted)

            ' Set the Connection, CommandText and Parameters.
            command.Connection = mySqlCon
            command.CommandText = "DELETE FROM OrderHead WHERE OrderNo = ?"
            command.Parameters.Add("OrderNo", Data.OleDb.OleDbType.VarWChar, 50)
            command.Parameters(0).Value = ProductNo

            '*** Command & Transaction ***'
            command.Transaction = Transaction
            command.Prepare()
            Try
                command.ExecuteNonQuery()

                command.CommandText = "DELETE FROM OrderHead WHERE OrderNo = ?"
                command.ExecuteNonQuery()

                Transaction.Commit()
            Catch ex As Exception
                Transaction.Rollback()
                MsgBox(ex.Message)
                mySqlCon.Close()
                Exit Sub
            End Try
            mySqlCon.Close()

            btnNew_Click(Nothing, Nothing)

        End If
    End Sub
#End Region


#Region "LineItem"
    Public Sub createNewDataLineItem()
        dataLineItem = New DataTable("ProductLine")

        dataLineItem.Columns.Add(New DataColumn("ProductID"))
        dataLineItem.Columns.Add(New DataColumn("CompartmentNo"))
        dataLineItem.Columns.Add(New DataColumn("OrderQuantity"))
        dataLineItem.Columns.Add(New DataColumn("UnitPrice"))
        dataLineItem.Columns.Add(New DataColumn("ExtendedPrice"))
        'dataLineItem.Columns.Add(New DataColumn("Unit"))
    End Sub
    Protected Sub addLineItem()
        If (dataLineItem Is Nothing) Then
            createNewDataLineItem()
        End If
        Dim dr As DataRow

        dr = dataLineItem.NewRow()

        dr("ProductID") = ""
        dr("CompartmentNo") = ""
        dr("OrderQuantity") = ""
        dr("UnitPrice") = ""
        dr("ExtendedPrice") = ""
        'dr("Unit") = ""

        dataLineItem.Rows.Add(dr)
        updateGridLineItem()
        Session("dataLineItem") = dataLineItem
        insertLineItem(dataLineItem.Rows.Count - 1)

    End Sub
    Protected Sub insertLineItem(ByVal line As Integer)
        editLine = line
        Session("editLine") = line

        gridOrderLineItem.Rows(line).Cells(0).Controls.Clear()
        gridOrderLineItem.Rows(line).Cells(0).Controls.Add(btnInsertProduct)
        gridOrderLineItem.Rows(line).Cells(1).Controls.Clear()
        gridOrderLineItem.Rows(line).Cells(1).Controls.Add(btnCancelProduct)
        gridOrderLineItem.Rows(line).Cells(3).Controls.Add(lbRequireField0)
        gridOrderLineItem.Rows(line).Cells(3).Controls.Add(txtProductID)
        gridOrderLineItem.Rows(line).Cells(3).Controls.Add(btnGetProduct)
        gridOrderLineItem.Rows(line).Cells(4).Controls.Add(txtCompartmentNo)
        gridOrderLineItem.Rows(line).Cells(4).Controls.Add(btnGetCompart)
        gridOrderLineItem.Rows(line).Cells(5).Controls.Add(lbRequireField1)
        gridOrderLineItem.Rows(line).Cells(5).Controls.Add(txtOrderQuantity)
        gridOrderLineItem.Rows(line).Cells(6).Controls.Add(txtUnitPrice)
        gridOrderLineItem.Rows(line).Cells(7).Controls.Add(txtExtendedPrice)
        'gridOrderLineItem.Rows(line).Cells(8).Controls.Add(txtUnit)

    End Sub
    Protected Sub updateGridLineItem()
        ' Copy data from Collection to DataTable
        If (dataLineItem Is Nothing) Then
            dataLineItem = New DataTable("ProductLine")
            dataLineItem.Columns.Add(New DataColumn("ProductID"))
            dataLineItem.Columns.Add(New DataColumn("CompartmentNo"))
            dataLineItem.Columns.Add(New DataColumn("OrderQuantity"))
            dataLineItem.Columns.Add(New DataColumn("UnitPrice"))
            dataLineItem.Columns.Add(New DataColumn("ExtendedPrice"))
            'dataLineItem.Columns.Add(New DataColumn("Unit"))
        End If
        gridOrderLineItem.DataSource = dataLineItem
        gridOrderLineItem.DataBind()
        setGridStyle()

    End Sub
    Protected Sub setColumnName()
        If Session("language") = "EN" Then
            gridOrderLineItem.HeaderRow.Cells(3).Text = "Product ID"
            gridOrderLineItem.HeaderRow.Cells(4).Text = "CompartmentNo"
            gridOrderLineItem.HeaderRow.Cells(5).Text = "OrderQuantity"
            gridOrderLineItem.HeaderRow.Cells(6).Text = "UnitPrice"
            gridOrderLineItem.HeaderRow.Cells(7).Text = "ExtendedPrice"
            'gridProductLineItem.HeaderRow.Cells(8).Text = "Unit"
        ElseIf Session("language") = "TH" Then
            gridOrderLineItem.HeaderRow.Cells(3).Text = "รหัสสินค้า"
            gridOrderLineItem.HeaderRow.Cells(4).Text = "ชื่อสินค้า"
            gridOrderLineItem.HeaderRow.Cells(5).Text = "น้ำหนัก"
            gridOrderLineItem.HeaderRow.Cells(6).Text = "ความดัน"
            gridOrderLineItem.HeaderRow.Cells(7).Text = "ความเข้มข้น"
            'gridProductLineItem.HeaderRow.Cells(7).Text = "หน่วย"
        End If
    End Sub

    Protected Sub grdOrderLineItem_RowEditing(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gridOrderLineItem.RowEditing
        setGridStyle()

        If Not editLine = -1 Then
            dataLineItem.Rows.RemoveAt(editLine)
        End If

        editLine = -1
        Session("editLine") = editLine
        gridOrderLineItem.EditIndex = e.NewEditIndex

        updateGridLineItem()


        ' Textbox in row 
        Dim textboxRow As TextBox
        gridOrderLineItem.HeaderRow.Cells(0).Width = 50
        textboxRow = gridOrderLineItem.Rows(e.NewEditIndex).Cells(3).Controls(0)
        textboxRow.Width = 100
        textboxRow = gridOrderLineItem.Rows(e.NewEditIndex).Cells(4).Controls(0)
        textboxRow.Width = 100
        textboxRow = gridOrderLineItem.Rows(e.NewEditIndex).Cells(5).Controls(0)
        textboxRow.Width = 100
        textboxRow = gridOrderLineItem.Rows(e.NewEditIndex).Cells(6).Controls(0)
        textboxRow.Width = 100
        textboxRow = gridOrderLineItem.Rows(e.NewEditIndex).Cells(7).Controls(0)
        textboxRow.Width = 100
        'textboxRow = gridOrderLineItem.Rows(e.NewEditIndex).Cells(8).Controls(0)
        'textboxRow.Width = 95

        gridOrderLineItem.Rows(e.NewEditIndex).Cells(5).Controls.Add(lbRequireField1)

        gridOrderLineItem.Rows(e.NewEditIndex).Cells(3).Enabled = False
        gridOrderLineItem.Rows(e.NewEditIndex).Cells(4).Enabled = False
        gridOrderLineItem.Rows(e.NewEditIndex).Cells(6).Enabled = False
        gridOrderLineItem.Rows(e.NewEditIndex).Cells(7).Enabled = False
        'gridOrderLineItem.Rows(e.NewEditIndex).Cells(8).Enabled = False
    End Sub
    Protected Sub grdOrderLineItem_CancelEditing(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gridOrderLineItem.RowCancelingEdit
        gridOrderLineItem.EditIndex = -1
        addLineItem()
    End Sub
    Protected Sub grdOrderLineItem_RowDeleting(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles gridOrderLineItem.RowDeleting
        If Not editLine = -1 Then
            dataLineItem.Rows.RemoveAt(editLine)
            MsgBox(editLine)

        End If
        editLine = -1
        MsgBox(e.RowIndex)
        dataLineItem.Rows.RemoveAt(e.RowIndex)
        addLineItem()
        calculateAmount()
        setDirtyBit("DIRTY")
    End Sub
    Protected Sub grdOrderLineItem_RowUpdateing(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gridOrderLineItem.RowUpdating
        Dim Row = gridOrderLineItem.Rows(e.RowIndex)
        Dim Quantity As Double = CDbl(e.NewValues(2))
        Dim UnitPrice As Double = CDbl(e.NewValues(3))


        ' Check Quantity
        If (Quantity < 1) Then
            MsgBox("Please enter quantity.")
            ''txtOrderNo.Text = grdProductLineItem.Rows(e.RowIndex).Cells(2).Text
            ''grdInvoiceLineItem.Rows(e.RowIndex).Cells(2).
            ''gridProductLineItem.Rows(e.RowIndex).Cells(2).Focus()
            Exit Sub
        End If

        dataLineItem.Rows(e.RowIndex).Item(1) = e.NewValues(1)
        dataLineItem.Rows(e.RowIndex).Item(2) = Quantity
        dataLineItem.Rows(e.RowIndex).Item(4) = FormatNumber(Quantity * UnitPrice, 2)

        gridOrderLineItem.Rows(e.RowIndex).Cells(4).Enabled = True
        gridOrderLineItem.Rows(e.RowIndex).Cells(7).Enabled = True

        'gridOrderLineItem.Rows(e.RowIndex).Cells(4).Enabled = True
        gridOrderLineItem.Rows(e.RowIndex).Cells(6).Enabled = True
        ' gridOrderLineItem.Rows(e.RowIndex).Cells(7).Enabled = True
        gridOrderLineItem.Rows(e.RowIndex).Cells(8).Enabled = True
        gridOrderLineItem.Rows(e.RowIndex).Cells(9).Enabled = True

        gridOrderLineItem.EditIndex = -1
        updateGridLineItem()
        addLineItem()
        calculateAmount()

        setDirtyBit("DIRTY")
    End Sub

    Protected Sub btnCancelProduct_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnCancelProduct.Click
        clearProductfield()
    End Sub
    Protected Sub btnInsertProduct_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnInsertProduct.Click
        ' Check Require Field
        If (txtProductID.Text = "") Then
            MsgBox("Please enter product code.")
            txtProductID.Focus()
            Exit Sub
        End If
        If (txtOrderQuantity.Text = "") Then
            MsgBox("Please enter quantity.")
            txtOrderQuantity.Focus()
            Exit Sub
        End If

        '' Add Data in Row
        'If dataLineItem Is Nothing Then
        '    createNewDataLineItem()
        'End If

        Dim dr As DataRow
        dr = dataLineItem.NewRow()

        dr("ProductID") = txtProductID.Text
        dr("CompartmentNo") = txtCompartmentNo.Text
        dr("OrderQuantity") = FormatNumber(txtOrderQuantity.Text, 2)
        dr("UnitPrice") = FormatNumber(txtUnitPrice.Text, 2)

        dr("ExtendedPrice") = FormatNumber(txtExtendedPrice.Text, 2)
        'dr("Unit") = FormatNumber(txtUnit.Text, 2)

        dataLineItem.Rows.RemoveAt(editLine)
        dataLineItem.Rows.InsertAt(dr, editLine)
        updateGridLineItem()
        editLine = -1
        calculateAmount()
        addLineItem()
        setDirtyBit("DIRTY")
        clearProductfield()
        Session("dataLineItem") = dataLineItem
        txtProductID.Focus()
    End Sub

    Public Sub calculateAmount()
        Dim TotalWeight As Double
        TotalWeight = 0.0
        Dim tempPrice As Double
        For i = 0 To dataLineItem.Rows.Count - 1
            If i = editLine Then
                Continue For
            End If

            Double.TryParse(dataLineItem.Rows(i).Item(4).Trim(), tempPrice)
            TotalWeight = TotalWeight + tempPrice
        Next

        txtTotalWeight.Text = FormatNumber(TotalWeight, 2)
        'txtVAT.Text = FormatNumber(TotalAmount * 0.07, 2)
        'txtAmountDue.Text = FormatNumber(TotalAmount + (TotalAmount * 0.07), 2)
    End Sub
#End Region

#Region "Text Changed"
    Protected Sub txtOrderNo_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtOrderNo.TextChanged
        showInvoice(txtOrderNo.Text)

        If Session("mode") = "COPY" Then            ' in mode COPY
            txtOrderNo.Enabled = True
            txtOrderNo.Text = "NEW"               ' set text = NEW
            txtOrderNo.Enabled = False
        End If
    End Sub
    Protected Sub txtCustomerID_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCustomerID.TextChanged
        ' query Customer

        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT CustomerID, CustomerName " & _
                              "FROM   Customer " & _
                              "WHERE  CustomerID =? "
        command.Parameters.Add("CustomerID", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = txtCustomerID.Text

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        mySqlCmd = New Data.OleDb.OleDbCommand(SQL, mySqlCon)

        Try
            mySqlReader = command.ExecuteReader()
            If mySqlReader.HasRows = True Then
                mySqlReader.Read()
                txtCustomerID.Text = mySqlReader.Item(0)
                txtCustomerName.Text = mySqlReader.Item(1)
                setDirtyBit("DIRTY")                                ' set dirtybit
            Else
                clearCustomerField()
                txtCustomerID.Focus()
            End If
        Catch ex As Exception
        End Try
        mySqlCon.Close()

    End Sub
    Protected Sub txtFillingGasNo_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtFillingGasNo.TextChanged
        ' query Customer

        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT FillingGasNo, FillingGasDate " & _
                              "FROM   FillingGas " & _
                              "WHERE  FillingGasNo =? "
        command.Parameters.Add("FillingGasNo", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = txtFillingGasNo.Text

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        mySqlCmd = New Data.OleDb.OleDbCommand(SQL, mySqlCon)

        Try
            mySqlReader = command.ExecuteReader()
            If mySqlReader.HasRows = True Then
                mySqlReader.Read()
                txtFillingGasNo.Text = mySqlReader.Item(0)
                txtFillingGasDate.Text = mySqlReader.Item(1)
                setDirtyBit("DIRTY")                                ' set dirtybit
            Else
                clearFillField()
                txtFillingGasNo.Focus()
            End If
        Catch ex As Exception
        End Try
        mySqlCon.Close()

    End Sub

    Protected Sub txtProductID_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtProductID.TextChanged
        ' query Product

        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT ProductID, ProductName, UnitPrice " & _
                              "FROM Product " & _
                              "WHERE ProductID = ? "
        command.Parameters.Add("ProductID", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = txtProductID.Text

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        '  Load Data Line Item into Collection
        Try
            mySqlReader = command.ExecuteReader()
            If mySqlReader.HasRows = True Then
                mySqlReader.Read()
                txtProductID.Text = mySqlReader.Item(0)
                txtUnitPrice.Text = mySqlReader.Item(2)
                'txtUnitPrice.Text = FormatNumber(mySqlReader.Item(1), 2)
                setDirtyBit("DIRTY")                ' set dirtybit
            Else                                    ' if it cannot query data
                clearProductfield()                 ' clear data in textbox
                txtProductID.Focus()              ' focus textbox for type new ProductCode
                Exit Sub
            End If
        Catch ex As Exception

        End Try

        mySqlCon.Close()

    End Sub
    Protected Sub txtCompartmentNo_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtCompartmentNo.TextChanged
        ' query Product

        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT CompartmentNo " & _
                              "FROM Truck " & _
                              "WHERE CompartmentNo = ? "
        command.Parameters.Add("CompartmentNo", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = txtCompartmentNo.Text

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        '  Load Data Line Item into Collectio
        Try
            mySqlReader = command.ExecuteReader()
            If mySqlReader.HasRows = True Then
                mySqlReader.Read()
                txtCompartmentNo.Text = mySqlReader.Item(0)
                ' txtUnitPrice.Text = mySqlReader.Item(2)
                'txtUnitPrice.Text = FormatNumber(mySqlReader.Item(1), 2)
                setDirtyBit("DIRTY")                ' set dirtybit
            Else                                    ' if it cannot query data
                clearProductfield()                 ' clear data in textbox
                txtCompartmentNo.Focus()              ' focus textbox for type new ProductCode
                Exit Sub
            End If
        Catch ex As Exception

        End Try

        mySqlCon.Close()

    End Sub
    Protected Sub txtFillingGasDate_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtFillingGasDate.TextChanged
        setDirtyBit("DIRTY")
    End Sub
#End Region

#Region "Clear Textbox"
    Public Sub clearInvoiceField()
        txtOrderNo.Text = ""
        txtFillingGasDate.Text = ""
        txtTotalWeight.Text = ""
        'txtVAT.Text = ""
        'txtAmountDue.Text = ""
    End Sub
    Public Sub clearFillField()
        txtFillingGasNo.Text = ""
        txtFillingGasDate.Text = ""

    End Sub
    Public Sub clearCustomerField()
        txtCustomerID.Text = ""
        txtCustomerName.Text = ""
    End Sub
    Public Sub clearProductfield()
        txtProductID.Text = ""
        txtCompartmentNo.Text = ""
        txtOrderQuantity.Text = ""
        txtUnitPrice.Text = ""
        txtExtendedPrice.Text = ""
        'txtUnit.Text = ""
    End Sub
#End Region

    Public Sub showInvoice(ByVal OrderNo)
        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT P.OrderNo, P.FillingGasDate, L.CustomerID, L.CustomerName, P.OrderDate, P.FillingGasNo, P.TotalWeight " & _
                              "FROM   ((OrderHead P JOIN Customer L ON P.CustomerID = L.CustomerID) INNER JOIN FillingGas F ON F.FillingGasNo = P.FillingGasNo) " & _
                              "WHERE  P.OrderNo=?"
        command.Parameters.Add("OrderNo", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = OrderNo

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        Try
            mySqlReader = command.ExecuteReader()
            If mySqlReader.HasRows = True Then
                mySqlReader.Read()
                txtOrderNo.Text = mySqlReader.Item(0)
                txtFillingGasDate.Text = mySqlReader.Item(1)
                txtCustomerID.Text = mySqlReader.Item(2)
                'txtProductNameHead.Text = mySqlReader.Item(3)
                txtCustomerName.Text = mySqlReader.Item(3)
                txtOrderDate.Text = mySqlReader.Item(4)
                txtFillingGasNo.Text = mySqlReader.Item(5)


                txtTotalWeight.Text = FormatNumber(mySqlReader.Item(6), 2)
                '  txtVAT.Text = FormatNumber(mySqlReader.Item(5), 2)
                ' txtAmountDue.Text = FormatNumber(mySqlReader.Item(6), 2)

                ' Load Data Line Item into Collection
                createNewDataLineItem()
                showOrderLine(OrderNo)
                updateGridLineItem()
                addLineItem()
                calculateAmount()
            Else
                clearInvoiceField()
                clearCustomerField()
                clearFillField()
            End If
        Catch ex As Exception

        End Try

        mySqlCon.Close()

        If Session("mode") = "COPY" Then
            txtOrderNo.Text = "NEW"
        End If

        'txtProductNo.Enabled = False
        'setEnableTextbox()

    End Sub
    Public Sub showOrderLine(ByVal OrderNoLine)
        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT ProductID, CompartmentNo, OrderQuantity, UnitPrice, ExtendedPrice " & _
                              "FROM OrderLine " & _
                              "WHERE OrderNoLine = ? "
        command.Parameters.Add("OrderNoLine", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = OrderNoLine

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        Try

            mySqlReader = command.ExecuteReader()
            While mySqlReader.Read()
                Dim dr As DataRow
                dr = dataLineItem.NewRow()
                dr("ProductID") = mySqlReader.Item(0)
                dr("CompartmentNo") = mySqlReader.Item(1)
                dr("OrderQuantity") = FormatNumber(mySqlReader.Item(2), 2)
                dr("UnitPrice") = FormatNumber(mySqlReader.Item(3), 2)
                dr("ExtendedPrice") = FormatNumber(mySqlReader.Item(4), 2)
                ' dr("Unit") = FormatNumber(mySqlReader.Item(5), 2)
                dataLineItem.Rows.Add(dr)
            End While
            Session("dataLineItem") = dataLineItem
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        mySqlCon.Close()
    End Sub


End Class