Public Class invoice1

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

            lbInvoiceNo.Text = "Invoice No"
            lbInvoiceDate.Text = "Invoice Date"
            lbOrderNo.Text = "Order No"
            lbCustomerID.Text = "CustomerID"
            lbCustomerName.Text = "CustomerName"

            lbInvoiceTotal.Text = "Invoice Total"
            lbVAT.Text = "VAT"
            lbInvoiceAmount.Text = "Invoice Amount"
            ' lbTotalPaid.Text = "Total Paid"

            lbInvoiceLine.Text = "Invoice Line"

        ElseIf Session("language") = "TH" Then
            lbNew.Text = "สร้าง"
            lbEdit.Text = "แก้ไข"
            lbCopy.Text = "คัดลอก"
            lbSave.Text = "บันทึก"
            lbDelete.Text = "ลบ"
            lbPrint.Text = "พิมพ์"
            lbClose.Text = "ปิด"

            lbInvoiceNo.Text = "หมายเลขรายการ"
            lbInvoiceDate.Text = "วันที่ทำรายการ"
            lbOrderNo.Text = "รหัสรายการ"
            lbCustomerID.Text = "รหัสลูกค้า"
            lbCustomerName.Text = "ชื่อลูกค้า"

            lbInvoiceTotal.Text = "รวมราคา"
            lbVAT.Text = "ภาษีมูลค่าเพิ่ม"
            lbInvoiceAmount.Text = "จำนวนเงินรวม"
            ' lbTotalPaid.Text = "จำนวนเงินจ่าย"

            lbInvoiceLine.Text = "รายการสั่งซื้อ"

        End If
    End Sub
    Public Sub setButton()
        ' set attribute (ONCLICK) of button
        btnNew.Attributes.Add("onclick", "javascript:return checkDirty('NEW');")
        btnEdit.Attributes.Add("onclick", "javascript:if(checkDirty('EDIT')) {openListOfValue('EDIT','Invoice','Select InvoiceNo, InvoiceDate From Invoice WHERE (1=1)','InvoiceNo, InvoiceDate'); return false; } else {return false;}")
        btnCopy.Attributes.Add("onclick", "javascript:if(checkDirty('COPY') == true) { openListOfValue('COPY','Invoice','Select InvoiceNo, InvoiceDate From Invoice WHERE (1=1)','InvoiceNo, InvoiceDate'); return false; } else {return false;}")
        btnPrint.Attributes.Add("onclick", "javascript:printInvoice();")
        btnClose.Attributes.Add("onclick", "javascript:checkClose();")
        btnGetOrder.Attributes.Add("onclick", "javascript:openListOfValue('','OrderHead','Select OrderNo, CustomerID, CustomerName From OrderHead WHERE (1=1)','OrderNo, CustomerID, CustomerName'); return false;")
        btnGetPayment.Attributes.Add("onclick", "javascript:openListOfValue('','Payment','Select PaymentNo, PaymentMethod, PaymentRefNo From Payment WHERE (1=1)','PaymentNo,PaymentMethod,PaymentRefNo'); return false;")
    End Sub
    Public Sub setTextBoxStyle()
        'style textbox
        txtInvoiceTotal.Style("text-align") = "right"
        txtVAT.Style("text-align") = "right"
        txtInvoiceAmount.Style("text-align") = "right"
    End Sub
    Public Sub setGridStyle()
        ' set width of column and style of text
        gridInvoiceLineItem.HeaderRow.Cells(0).Width = 25
        gridInvoiceLineItem.HeaderRow.Cells(1).Width = 25
        gridInvoiceLineItem.HeaderRow.Cells(2).Width = 30
        gridInvoiceLineItem.HeaderRow.Cells(3).Width = 175
        gridInvoiceLineItem.HeaderRow.Cells(4).Width = 175
        gridInvoiceLineItem.HeaderRow.Cells(5).Width = 100
        gridInvoiceLineItem.HeaderRow.Cells(6).Width = 100
        'gridInvoiceLineItem.HeaderRow.Cells(7).Width = 100
        'gridInvoiceLineItem.HeaderRow.Cells(8).Width = 100
        gridInvoiceLineItem.HeaderRow.Style("font-weight") = "normal"
        gridInvoiceLineItem.HeaderRow.Style("text-align") = "center"
        gridInvoiceLineItem.HeaderRow.Height = 22

        ' set alignment in each column
        For i = 0 To gridInvoiceLineItem.Rows.Count - 1
            gridInvoiceLineItem.Rows(i).Cells(0).Style("text-align") = "center"
            gridInvoiceLineItem.Rows(i).Cells(1).Style("text-align") = "center"
            gridInvoiceLineItem.Rows(i).Cells(2).Style("text-align") = "center"
            gridInvoiceLineItem.Rows(i).Cells(3).Style("text-align") = "left"
            gridInvoiceLineItem.Rows(i).Cells(3).Style("padding-left") = "5px"
            gridInvoiceLineItem.Rows(i).Cells(4).Style("text-align") = "left"
            gridInvoiceLineItem.Rows(i).Cells(4).Style("padding-left") = "5px"
            gridInvoiceLineItem.Rows(i).Cells(5).Style("text-align") = "right"
            gridInvoiceLineItem.Rows(i).Cells(5).Style("padding-right") = "5px"
            gridInvoiceLineItem.Rows(i).Cells(6).Style("text-align") = "right"
            gridInvoiceLineItem.Rows(i).Cells(6).Style("padding-right") = "5px"
            'gridInvoiceLineItem.Rows(i).Cells(7).Style("text-align") = "right"
            'gridInvoiceLineItem.Rows(i).Cells(7).Style("padding-right") = "5px"
            'gridInvoiceLineItem.Rows(i).Cells(8).Style("text-align") = "right"
            'gridInvoiceLineItem.Rows(i).Cells(8).Style("padding-right") = "5px"
            gridInvoiceLineItem.Rows(i).Height = 22
        Next

        gridInvoiceLineItem.Rows(gridInvoiceLineItem.Rows.Count - 1).Style("text-align") = "center"
    End Sub
    Public Sub setEnableTextbox()
        txtOrderNo.Enabled = True
        txtCustomerID.Enabled = True
        txtCustomerName.Enabled = True
        txtInvoiceDate.Enabled = True

        txtPaymentNo.Enabled = True
        txtPaymentMethod.Enabled = True
        txtPaymentRefNo.Enabled = True
        ' txtAmountPaid.Enabled = True
        txtInvoicePaid.Enabled = True
        ' txtUnpaid.Enabled = True

        txtInvoiceTotal.Enabled = True
        txtVAT.Enabled = True
        txtInvoiceAmount.Enabled = True
        ' txtTotalPaid.Enabled = True


        If Session("mode") = "NEW" Or Session("mode") = "COPY" Then
            txtInvoiceNo.Enabled = False
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
        clearOrderField()
        clearInvoiceLinefield()

        txtInvoiceNo.Text = "NEW"

        txtInvoiceDate.Text = Date.Today.ToString("dd/MM/yyyy")
        createNewDataLineItem()
        addLineItem()
        setDirtyBit("CLEAR")

        setEnableTextbox()
    End Sub
    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        Dim InvoiceNo As String

        ' Check Require Field
        If (txtInvoiceDate.Text = "") Then
            MsgBox("Please enter Invoice date.")
            txtInvoiceDate.Focus()
            Exit Sub
        End If
        If (txtInvoiceNo.Text = "") Then
            MsgBox("Please enter Invoice No.")
            txtInvoiceNo.Focus()
            Exit Sub
        End If
        If dataLineItem.Rows.Count = 1 Then
            MsgBox("No data in lineitem.")
            txtPaymentNo.Focus()
            Exit Sub
        End If


        If txtInvoiceNo.Text = "NEW" Then

            'Running Number
            Dim maxIN As String
            Dim strSplit As Array

            connectDB()
            mySqlCon.Open()

            Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()
            Dim Transaction = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted)
            command.Connection = mySqlCon
            command.Transaction = Transaction                           '*** Command & Transaction ***'
            command.CommandText = "SELECT MAX(InvoiceNo) FROM Invoice "
            Try
                'command.Prepare()
                command.ExecuteNonQuery()
                mySqlReader = command.ExecuteReader()
                If mySqlReader.HasRows = True Then
                    mySqlReader.Read()
                    strSplit = Split(mySqlReader.Item(0), "INV")
                    maxIN = strSplit(1)
                Else
                    maxIN = 0
                End If
                InvoiceNo = "INV" + Format(maxIN + 1, "000")
                mySqlReader.Close()

                ' insert new invoice into db
                Try
                    command.CommandText = "INSERT INTO Invoice(InvoiceNo, InvoiceDate, OrderNo, CustomerID, InvoiceTotal, VAT, InvoiceAmount) " & _
                                          "VALUES (?,?,?,?,?,?,?)"
                    command.Parameters.Add("InvoiceNo", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("InvoiceDate", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("OrderNo", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("CustomerID", Data.OleDb.OleDbType.VarWChar, 50)

                    command.Parameters.Add("InvoiceTotal", Data.OleDb.OleDbType.Double)
                    command.Parameters.Add("VAT", Data.OleDb.OleDbType.Double)
                    command.Parameters.Add("InvoiceAmount", Data.OleDb.OleDbType.Double)
                    'command.Parameters.Add("TotalPaid", Data.OleDb.OleDbType.Double)
                    

                    ' command.Parameters.Add("TotalAmount", Data.OleDb.OleDbType.Numeric, 18)
                    'command.Parameters.Add("VAT", Data.OleDb.OleDbType.Numeric, 18)
                    'command.Parameters.Add("AmountDue", Data.OleDb.OleDbType.Numeric, 18)
                    command.Parameters(0).Value = InvoiceNo
                    command.Parameters(1).Value = txtInvoiceDate.Text
                    command.Parameters(2).Value = txtOrderNo.Text()
                    command.Parameters(3).Value = txtCustomerID.Text

                    command.Parameters(4).Value = txtInvoiceTotal.Text
                    command.Parameters(5).Value = txtVAT.Text
                    command.Parameters(6).Value = txtInvoiceAmount.Text
                    'command.Parameters(7).Value = txtTotalPaid.Text
                   
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
                        command.CommandText = "INSERT INTO InvoiceLine(InvoiceNoLine, PaymentNo, PaymentMethod, PaymentRefNo, InvoicePaid) " & _
                                                " VALUES (?,?,?,?,?)"
                        command.Parameters.Add("InvoiceNoLine", Data.OleDb.OleDbType.VarWChar, 50)
                        command.Parameters.Add("PaymentNo", Data.OleDb.OleDbType.VarWChar, 50)
                        command.Parameters.Add("PaymentMethod", Data.OleDb.OleDbType.VarWChar, 50)
                        command.Parameters.Add("PaymentRefNo", Data.OleDb.OleDbType.VarWChar, 50)
                        ' command.Parameters.Add("AmountPaid", Data.OleDb.OleDbType.Double)
                        command.Parameters.Add("InvoicePaid", Data.OleDb.OleDbType.Double)
                        '  command.Parameters.Add("Unpaid", Data.OleDb.OleDbType.Double)

                        command.Parameters(0).Value = InvoiceNo
                        command.Parameters(1).Value = dataLineItem.Rows(i).Item(0)
                        command.Parameters(2).Value = dataLineItem.Rows(i).Item(1)
                        command.Parameters(3).Value = dataLineItem.Rows(i).Item(2)

                        command.Parameters(4).Value = Format(dataLineItem.Rows(i).Item(3), "General Number")
                        
                        'command.Parameters(5).Value = Format(dataLineItem.Rows(i).Item(4), "General Number")
                        'command.Parameters(6).Value = (i + 1)
                        command.ExecuteNonQuery()

                    Next
                    Transaction.Commit() '*** Commit Transaction ***'
                    setDirtyBit("CLEAR")
                    showInvoice(lbInvoiceNo)
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
            InvoiceNo = txtInvoiceNo.Text
            connectDB()
            mySqlCon.Open()
            Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()
            Dim Transaction = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted)
            command.Connection = mySqlCon
            command.Transaction = Transaction                           '*** Command & Transaction ***'

            Try
                command.CommandText = "UPDATE Invoice SET " & _
                                       "InvoiceDate=?, OrderNo=?, CustomerID=?, InvoiceTotal=?, VAT=?, InvoiceAmount=? " & _
                                       "WHERE InvoiceNo = ? "

                'command.Parameters.Add("ProductNo", Data.OleDb.OleDbType.VarWChar, 10)
                command.Parameters.Add("InvoiceDate", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("OrderNo", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("CustomerID", Data.OleDb.OleDbType.VarWChar, 50)

                command.Parameters.Add("InvoiceTotal", Data.OleDb.OleDbType.Double)
                command.Parameters.Add("VAT", Data.OleDb.OleDbType.Double)
                command.Parameters.Add("InvoiceAmount", Data.OleDb.OleDbType.Double)
                'command.Parameters.Add("TotalPaid", Data.OleDb.OleDbType.Double)


                command.Parameters.Add("InvoiceNo", Data.OleDb.OleDbType.VarWChar, 50)

                'command.Parameters(0).Value = txtProductNo.Text
                command.Parameters(0).Value = txtInvoiceDate.Text
                command.Parameters(1).Value = txtOrderNo.Text
                command.Parameters(2).Value = txtCustomerID.Text

                command.Parameters(3).Value = txtInvoiceTotal.Text
                command.Parameters(4).Value = txtVAT.Text
                command.Parameters(5).Value = txtInvoiceAmount.Text
                'command.Parameters(6).Value = txtTotalPaid.Text
                command.Parameters(6).Value = InvoiceNo
                command.ExecuteNonQuery()

                command.Parameters.Clear()
                command.CommandText = "DELETE FROM InvoiceLine WHERE InvoiceNoLine = ?"
                command.Parameters.Add("InvoiceNoDel", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters(0).Value = InvoiceNo
                command.ExecuteNonQuery()

                Dim i As Integer
                For i = 0 To dataLineItem.Rows.Count - 1
                    If i = editLine Then
                        Continue For
                    End If
                    command.Parameters.Clear()
                    command.CommandText = "INSERT INTO InvoiceLine (InvoiceNoLine, PaymentNo, PaymentMethod, PaymentRefNo, InvoicePaid) " & _
                                           "VALUES (?,?,?,?,?)"

                    command.Parameters.Add("InvoiceNoLine", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("PaymentNo", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("PaymentMethod", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("PaymentRefNo", Data.OleDb.OleDbType.VarWChar, 50)
                    '  command.Parameters.Add("AmountPaid", Data.OleDb.OleDbType.Double)
                    command.Parameters.Add("InvoicePaid", Data.OleDb.OleDbType.Double)
                    ' command.Parameters.Add("Unpaid", Data.OleDb.OleDbType.Double)

                    command.Parameters(0).Value = InvoiceNo
                    command.Parameters(1).Value = dataLineItem.Rows(i).Item(0)
                    command.Parameters(2).Value = dataLineItem.Rows(i).Item(1)
                    command.Parameters(3).Value = dataLineItem.Rows(i).Item(2)

                    command.Parameters(4).Value = Format(dataLineItem.Rows(i).Item(3), "General Number")
                    'command.Parameters(5).Value = Format(dataLineItem.Rows(i).Item(4), "General Number")
                    'command.Parameters(6).Value = Format(dataLineItem.Rows(i).Item(5), "General Number")

                    ' command.Parameters(5).Value = (i + 1)
                    command.ExecuteNonQuery()
                Next
                Transaction.Commit() '*** Commit Transaction ***'
                setDirtyBit("CLEAR")
                Session("mode") = "EDIT"
            Catch ex As Exception
                Transaction.Rollback() '*** RollBack Transaction ***'
                MsgBox(ex.Message)
            End Try
            mySqlCon.Close()
        End If
        insertLineItem(editLine)
    End Sub
    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete.Click

        If (txtInvoiceNo.Text = "") Or (txtInvoiceNo.Text = "NEW") Then
            txtInvoiceNo.Enabled = True
            MsgBox("Please select Invoice No for delete.")
            Exit Sub
        End If

        If (MsgBox("Do you want to delete Invoice No '" & txtInvoiceNo.Text & "' ", MsgBoxStyle.YesNo) = MsgBoxResult.Yes) Then
            Dim InvoiceNo As String
            InvoiceNo = txtInvoiceNo.Text

            connectDB()
            mySqlCon.Open()

            ' Create the Command. 
            Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()
            Dim Transaction = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted)

            ' Set the Connection, CommandText and Parameters.
            command.Connection = mySqlCon
            command.CommandText = "DELETE FROM Invoice WHERE InvoiceNo = ?"
            command.Parameters.Add("InvoiceNo", Data.OleDb.OleDbType.VarWChar, 50)
            command.Parameters(0).Value = InvoiceNo

            '*** Command & Transaction ***'
            command.Transaction = Transaction
            command.Prepare()
            Try
                command.ExecuteNonQuery()

                command.CommandText = "DELETE FROM Invoice WHERE InvoiceNo = ?"
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
        dataLineItem = New DataTable("InvoiceLine")

        dataLineItem.Columns.Add(New DataColumn("PaymentNo"))
        dataLineItem.Columns.Add(New DataColumn("PaymentMethod"))
        dataLineItem.Columns.Add(New DataColumn("PaymentRefNo"))
        'dataLineItem.Columns.Add(New DataColumn("AmountPaid"))
        dataLineItem.Columns.Add(New DataColumn("InvoicePaid"))
        'dataLineItem.Columns.Add(New DataColumn("Unpaid"))
    End Sub
    Protected Sub addLineItem()
        If (dataLineItem Is Nothing) Then
            createNewDataLineItem()
        End If
        Dim dr As DataRow

        dr = dataLineItem.NewRow()

        dr("PaymentNo") = ""
        dr("PaymentMethod") = ""
        dr("PaymentRefNo") = ""
        'dr("AmountPaid") = ""
        dr("InvoicePaid") = ""
        'dr("Unpaid") = ""

        dataLineItem.Rows.Add(dr)
        updateGridLineItem()
        Session("dataLineItem") = dataLineItem
        insertLineItem(dataLineItem.Rows.Count - 1)

    End Sub
    Protected Sub insertLineItem(ByVal line As Integer)
        editLine = line
        Session("editLine") = line

        gridInvoiceLineItem.Rows(line).Cells(0).Controls.Clear()
        gridInvoiceLineItem.Rows(line).Cells(0).Controls.Add(btnInsertProduct)
        gridInvoiceLineItem.Rows(line).Cells(1).Controls.Clear()
        gridInvoiceLineItem.Rows(line).Cells(1).Controls.Add(btnCancelProduct)
        gridInvoiceLineItem.Rows(line).Cells(3).Controls.Add(lbRequireField0)
        gridInvoiceLineItem.Rows(line).Cells(3).Controls.Add(txtPaymentNo)
        gridInvoiceLineItem.Rows(line).Cells(3).Controls.Add(btnGetPayment)
        gridInvoiceLineItem.Rows(line).Cells(4).Controls.Add(txtPaymentMethod)
        gridInvoiceLineItem.Rows(line).Cells(5).Controls.Add(lbRequireField1)
        gridInvoiceLineItem.Rows(line).Cells(5).Controls.Add(txtPaymentRefNo)
        'gridInvoiceLineItem.Rows(line).Cells(6).Controls.Add(txtAmountPaid)
        gridInvoiceLineItem.Rows(line).Cells(6).Controls.Add(txtInvoicePaid)
        'gridInvoiceLineItem.Rows(line).Cells(8).Controls.Add(txtUnpaid)

    End Sub
    Protected Sub updateGridLineItem()
        ' Copy data from Collection to DataTable
        If (dataLineItem Is Nothing) Then
            dataLineItem = New DataTable("InvoiceLine")
            dataLineItem.Columns.Add(New DataColumn("PaymentNo"))
            dataLineItem.Columns.Add(New DataColumn("PaymentMethod"))
            dataLineItem.Columns.Add(New DataColumn("PaymentRefNo"))
            'dataLineItem.Columns.Add(New DataColumn("AmountPaid"))
            dataLineItem.Columns.Add(New DataColumn("InvoicePaid"))
            'dataLineItem.Columns.Add(New DataColumn("Unpaid"))
        End If
        gridInvoiceLineItem.DataSource = dataLineItem
        gridInvoiceLineItem.DataBind()
        setGridStyle()

    End Sub
    Protected Sub setColumnName()
        If Session("language") = "EN" Then
            gridInvoiceLineItem.HeaderRow.Cells(3).Text = "PaymentNo"
            gridInvoiceLineItem.HeaderRow.Cells(4).Text = "Payment Method"
            gridInvoiceLineItem.HeaderRow.Cells(5).Text = "Payment Ref No"
            'gridInvoiceLineItem.HeaderRow.Cells(6).Text = "AmountPaid"
            gridInvoiceLineItem.HeaderRow.Cells(6).Text = "InvoicePaid"
            'gridInvoiceLineItem.HeaderRow.Cells(8).Text = "Unpaid"
        ElseIf Session("language") = "TH" Then
            gridInvoiceLineItem.HeaderRow.Cells(3).Text = "เลขที่การจ่ายเงิน"
            gridInvoiceLineItem.HeaderRow.Cells(4).Text = "วิธีการจ่ายเงิน"
            gridInvoiceLineItem.HeaderRow.Cells(5).Text = "อ้างอิง"
            '  gridInvoiceLineItem.HeaderRow.Cells(6).Text = "ยอดรวม"
            gridInvoiceLineItem.HeaderRow.Cells(6).Text = "จ่ายแล้ว"
            '  gridInvoiceLineItem.HeaderRow.Cells(8).Text = "ค้างจ่าย"
        End If
    End Sub

    Protected Sub grdInvoiceLineItem_RowEditing(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gridInvoiceLineItem.RowEditing
        setGridStyle()

        If Not editLine = -1 Then
            dataLineItem.Rows.RemoveAt(editLine)
        End If

        editLine = -1
        Session("editLine") = editLine
        gridInvoiceLineItem.EditIndex = e.NewEditIndex

        updateGridLineItem()


        ' Textbox in row 
        Dim textboxRow As TextBox
        gridInvoiceLineItem.HeaderRow.Cells(0).Width = 50
        textboxRow = gridInvoiceLineItem.Rows(e.NewEditIndex).Cells(3).Controls(0)
        textboxRow.Width = 100
        textboxRow = gridInvoiceLineItem.Rows(e.NewEditIndex).Cells(4).Controls(0)
        textboxRow.Width = 150
        textboxRow = gridInvoiceLineItem.Rows(e.NewEditIndex).Cells(5).Controls(0)
        textboxRow.Width = 95
        textboxRow = gridInvoiceLineItem.Rows(e.NewEditIndex).Cells(6).Controls(0)
        textboxRow.Width = 95
        'textboxRow = gridInvoiceLineItem.Rows(e.NewEditIndex).Cells(7).Controls(0)
        'textboxRow.Width = 95
        'textboxRow = gridInvoiceLineItem.Rows(e.NewEditIndex).Cells(8).Controls(0)
        'textboxRow.Width = 95

        gridInvoiceLineItem.Rows(e.NewEditIndex).Cells(5).Controls.Add(lbRequireField1)

        gridInvoiceLineItem.Rows(e.NewEditIndex).Cells(3).Enabled = False
        gridInvoiceLineItem.Rows(e.NewEditIndex).Cells(4).Enabled = False
        gridInvoiceLineItem.Rows(e.NewEditIndex).Cells(6).Enabled = False
        'gridInvoiceLineItem.Rows(e.NewEditIndex).Cells(7).Enabled = False
        'gridInvoiceLineItem.Rows(e.NewEditIndex).Cells(8).Enabled = False
    End Sub
    Protected Sub grdInvoiceLineItem_CancelEditing(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gridInvoiceLineItem.RowCancelingEdit
        gridInvoiceLineItem.EditIndex = -1
        addLineItem()
    End Sub
    Protected Sub grdInvoiceLineItem_RowDeleting(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles gridInvoiceLineItem.RowDeleting
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
    Protected Sub grdProductLineItem_RowUpdateing(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gridInvoiceLineItem.RowUpdating
        Dim Row = gridInvoiceLineItem.Rows(e.RowIndex)
        Dim Quantity As Double = CDbl(e.NewValues(2))
        Dim UnitPrice As Double = CDbl(e.NewValues(3))


        ' Check Quantity
        If (Quantity < 1) Then
            MsgBox("Please enter quantity.")
            'txtInvoiceNo.Text = grdProductLineItem.Rows(e.RowIndex).Cells(2).Text
            'grdInvoiceLineItem.Rows(e.RowIndex).Cells(2).
            'gridInvoiceLineItem.Rows(e.RowIndex).Cells(2).Focus()
            Exit Sub
        End If

        dataLineItem.Rows(e.RowIndex).Item(1) = e.NewValues(1)
        dataLineItem.Rows(e.RowIndex).Item(2) = Quantity
        'dataLineItem.Rows(e.RowIndex).Item(4) = FormatNumber(Quantity, 2)
        dataLineItem.Rows(e.RowIndex).Item(4) = FormatNumber(Quantity, 2)
        gridInvoiceLineItem.Rows(e.RowIndex).Cells(4).Enabled = True
        gridInvoiceLineItem.Rows(e.RowIndex).Cells(6).Enabled = True
        'gridInvoiceLineItem.Rows(e.RowIndex).Cells(7).Enabled = True
        'gridInvoiceLineItem.Rows(e.RowIndex).Cells(8).Enabled = True
        'gridInvoiceLineItem.Rows(e.RowIndex).Cells(9).Enabled = True

        gridInvoiceLineItem.EditIndex = -1
        updateGridLineItem()
        addLineItem()
        calculateAmount()
        setDirtyBit("DIRTY")
    End Sub

    Protected Sub btnCancelProduct_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnCancelProduct.Click
        clearInvoiceLinefield()
    End Sub
    Protected Sub btnInsertProduct_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnInsertProduct.Click
        ' Check Require Field
        If (txtPaymentNo.Text = "") Then
            MsgBox("Please enter payment No.")
            txtPaymentNo.Focus()
            Exit Sub
        End If
        If (txtPaymentRefNo.Text = "") Then
            MsgBox("Please enter quantity.")
            txtPaymentRefNo.Focus()
            Exit Sub
        End If

        ' Add Data in Row
        If dataLineItem Is Nothing Then
            createNewDataLineItem()
        End If
        Dim dr As DataRow
        dr = dataLineItem.NewRow()

        dr("PaymentNo") = txtPaymentNo.Text
        dr("PaymentMethod") = txtPaymentMethod.Text
        dr("PaymentRefNo") = txtPaymentRefNo.Text
        ' dr("AmountPaid") = txtAmountPaid.Text
        dr("InvoicePaid") = txtInvoicePaid.Text
        'dr("InvoicePaid") = FormatNumber(txtInvoicePaid.Text)
        'dr("Unpaid") = txtUnpaid.Text

        dataLineItem.Rows.RemoveAt(editLine)
        dataLineItem.Rows.InsertAt(dr, editLine)
        updateGridLineItem()
        editLine = -1
        calculateAmount()
        addLineItem()
        setDirtyBit("DIRTY")
        clearInvoiceLinefield()
        Session("dataLineItem") = dataLineItem
        txtPaymentNo.Focus()
    End Sub

    Public Sub calculateAmount()
        Dim TotalAmount As Double
        TotalAmount = 0.0
        Dim tempPrice As Double
        For i = 0 To dataLineItem.Rows.Count - 1
            If i = editLine Then
                Continue For
            End If

            Double.TryParse(dataLineItem.Rows(i).Item(3).Trim(), tempPrice)
            TotalAmount = txtInvoicePaid.Text
        Next

        txtInvoiceTotal.Text = FormatNumber(TotalAmount, 2)
        txtVAT.Text = FormatNumber(TotalAmount * 0.07, 2)
        txtInvoiceAmount.Text = FormatNumber(TotalAmount + (TotalAmount * 0.07), 2)
    End Sub
#End Region

#Region "Text Changed"
    Protected Sub txtInvoiceNo_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtInvoiceNo.TextChanged
        showInvoice(txtInvoiceNo.Text)

        If Session("mode") = "COPY" Then            ' in mode COPY
            txtInvoiceNo.Enabled = True
            txtInvoiceNo.Text = "NEW"               ' set text = NEW
            txtInvoiceNo.Enabled = False
        End If
    End Sub
    Protected Sub txtOrderNo_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtOrderNo.TextChanged
        ' query Customer

        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT OrderNo, CustomerID, CustomerName " & _
                              "FROM   OrderHead " & _
                              "WHERE  OrderNo =? "
        command.Parameters.Add("OrderNo", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = txtOrderNo.Text

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        mySqlCmd = New Data.OleDb.OleDbCommand(SQL, mySqlCon)

        Try
            mySqlReader = command.ExecuteReader()
            If mySqlReader.HasRows = True Then
                mySqlReader.Read()
                txtOrderNo.Text = mySqlReader.Item(0)
                txtCustomerID.Text = mySqlReader.Item(1)
                txtCustomerName.Text = mySqlReader.Item(2)
                setDirtyBit("DIRTY")                                ' set dirtybit
            Else
                clearOrderField()
                txtOrderNo.Focus()
            End If
        Catch ex As Exception
        End Try
        mySqlCon.Close()

    End Sub

    Protected Sub txtPaymentNo_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtPaymentNo.TextChanged
        ' query Product

        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT PaymentNo, PaymentMethod, PaymentRefNo " & _
                              "FROM Payment " & _
                              "WHERE PaymentNo = ?"
        command.Parameters.Add("PaymentNo", Data.OleDb.OleDbType.VarWChar, 10)
        command.Parameters(0).Value = txtPaymentNo.Text

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        'Load DataLine Item into Collection
        Try
            mySqlReader = command.ExecuteReader()
            If mySqlReader.HasRows = True Then
                mySqlReader.Read()
                txtPaymentNo.Text = mySqlReader.Item(0)
                txtPaymentMethod.Text = mySqlReader.Item(1)
                txtPaymentRefNo.Text = mySqlReader.Item(2)
                setDirtyBit("DIRTY")                ' set dirtybit
            Else                                    ' if it cannot query data
                clearInvoiceLinefield()                 ' clear data in textbox
                txtPaymentNo.Focus()              ' focus textbox for type new ProductCode
                Exit Sub
            End If
        Catch ex As Exception

        End Try

        mySqlCon.Close()

    End Sub
    Protected Sub txtInvoiceDate_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtInvoiceDate.TextChanged
        setDirtyBit("DIRTY")
    End Sub
#End Region

#Region "Clear Textbox"
    Public Sub clearInvoiceField()
        txtInvoiceNo.Text = ""
        txtInvoiceDate.Text = ""
        txtInvoiceTotal.Text = ""
        txtVAT.Text = ""
        txtInvoiceAmount.Text = ""
        '  txtTotalPaid.Text = ""

        'txtAmountDue.Text = ""
    End Sub

    Public Sub clearOrderField()
        txtOrderNo.Text = ""
        txtCustomerID.Text = ""
        txtCustomerName.Text = ""

    End Sub
    Public Sub clearInvoiceLinefield()
        txtPaymentNo.Text = ""
        txtPaymentMethod.Text = ""
        txtPaymentRefNo.Text = ""
        '  txtAmountPaid.Text = ""
        txtInvoicePaid.Text = ""
        '  txtUnpaid.Text = ""
    End Sub
#End Region

    Public Sub showInvoice(ByVal InvoiceNo)
        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        'command.CommandText = "SELECT I.InvoiceNo, I.InvoiceDate, O.OrderNo, O.CustomerID, O.CustomerName, I.InvoiceTotal, I.VAT, I.InvoiceAmount, I.TotalPaid " & _
        '                      "FROM   ((Invoice I INNER JOIN OrderHead O ON I.OrderNo = O.OrderNo) INNER JOIN Customer C ON C.CustomerID = O.CustomerID) " & _
        '                      "WHERE  I.InvoiceNo=?"
        command.CommandText = "SELECT I.InvoiceNo, I.InvoiceDate, O.OrderNo, O.CustomerID, O.CustomerName, I.InvoiceTotal, I.VAT, I.InvoiceAmount " & _
                              "FROM Invoice I INNER JOIN OrderHead O ON I.OrderNo = O.OrderNo " & _
                              "WHERE  I.InvoiceNo=? "
        command.Parameters.Add("InvoiceNo", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = InvoiceNo

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        Try
            mySqlReader = command.ExecuteReader()
            If mySqlReader.HasRows = True Then
                mySqlReader.Read()
                txtInvoiceNo.Text = mySqlReader.Item(0)
                txtInvoiceDate.Text = mySqlReader.Item(1)
                txtOrderNo.Text = mySqlReader.Item(2)
                txtCustomerID.Text = mySqlReader.Item(3)
                txtCustomerName.Text = mySqlReader.Item(4)

                txtInvoiceTotal.Text = FormatNumber(mySqlReader.Item(5), 2)
                txtVAT.Text = FormatNumber(mySqlReader.Item(6), 2)
                txtInvoiceAmount.Text = FormatNumber(mySqlReader.Item(7), 2)
                'txtTotalPaid.Text = FormatNumber(mySqlReader.Item(8), 2)

                ' Load Data Line Item into Collection
                createNewDataLineItem()
                showInvoiceLine(InvoiceNo)
                updateGridLineItem()
                addLineItem()
                calculateAmount()
            Else
                clearInvoiceField()
                clearOrderField()
            End If
        Catch ex As Exception

        End Try

        mySqlCon.Close()

        If Session("mode") = "COPY" Then
            txtInvoiceNo.Text = "NEW"
        End If

        'txtProductNo.Enabled = False
        'setEnableTextbox()

    End Sub
    Public Sub showInvoiceLine(ByVal InvoiceNoLine)
        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT InvoiceNoLine, PaymentNo, PaymentMethod, PaymentRefNo, InvoicePaid " & _
                              "FROM InvoiceLine " & _
                              "WHERE InvoiceNoLine = ? "
        command.Parameters.Add("InvoiceNoLine", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = InvoiceNoLine

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        Try

            mySqlReader = command.ExecuteReader()
            While mySqlReader.Read()
                Dim dr As DataRow
                dr = dataLineItem.NewRow()
                dr("PaymentNo") = mySqlReader.Item(0)
                dr("PaymentMethod") = mySqlReader.Item(1)
                dr("PaymentRefNo") = mySqlReader.Item(2)
                ' dr("AmountPaid") = mySqlReader.Item(3)
                dr("InvoicePaid") = mySqlReader.Item(3)
                ' dr("Unpaid") = mySqlReader.Item(5)
                dataLineItem.Rows.Add(dr)
            End While
            Session("dataLineItem") = dataLineItem
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        mySqlCon.Close()
    End Sub


End Class