Public Class Fill

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

            lbFillingGasNo.Text = "FillingGasNo"
            lbFillingGasDate.Text = "FillingGasDate"
            lbTruckID.Text = "Truck ID"
            lbTruckLicense.Text = "Truck License"
            lbDriverID.Text = "DriverID"
            lbDriverName.Text = "DriverName"

            lbTotalFilling.Text = "Total Filling"
            'lbVAT.Text = "VAT"
            'lbAmountDue.Text = "Amount Due"

            lbInvoiceLine.Text = "Invoice Line"

        ElseIf Session("language") = "TH" Then
            lbNew.Text = "สร้าง"
            lbEdit.Text = "แก้ไข"
            lbCopy.Text = "คัดลอก"
            lbSave.Text = "บันทึก"
            lbDelete.Text = "ลบ"
            lbPrint.Text = "พิมพ์"
            lbClose.Text = "ปิด"

            lbFillingGasNo.Text = "หมายเลขรายการเติม"
            lbFillingGasDate.Text = "วันที่เติมผลิตภัณฑ์"
            lbTruckID.Text = "รหัสรถ"
            lbTruckLicense.Text = "ทะเบียนรถ"
            lbDriverID.Text = "รหัสพนักงานขับรถ"
            lbDriverName.Text = "ชื่อพนักงานขับรถ"

            lbTotalFilling.Text = "รวมราคา"
            lbVAT.Text = "ภาษีมูลค่าเพิ่ม"
            lbAmountDue.Text = "จำนวนเงินรวม"

            lbInvoiceLine.Text = "รายการสั่งซื้อ"

        End If
    End Sub
    Public Sub setButton()
        ' set attribute (ONCLICK) of button
        btnNew.Attributes.Add("onclick", "javascript:return checkDirty('NEW');")
        btnEdit.Attributes.Add("onclick", "javascript:if(checkDirty('EDIT')) {openListOfValue('EDIT','FillingGas','Select FillingGasNo, FillingGasDate From FillingGas WHERE (1=1)','FillingGasNo,FillingGasDate'); return false; } else {return false;}")
        btnCopy.Attributes.Add("onclick", "javascript:if(checkDirty('COPY') == true) { openListOfValue('COPY','FillingGas','Select FillingGasNo, FillingGasDate From FillingGas WHERE (1=1)','FillingGasNo,FillingGasDate'); return false; } else {return false;}")
        btnPrint.Attributes.Add("onclick", "javascript:printInvoice();")
        btnClose.Attributes.Add("onclick", "javascript:checkClose();")
        btnGetDriver.Attributes.Add("onclick", "javascript:openListOfValue('','Driver','Select DriverID, DriverName From Driver WHERE (1=1)','DriverID,DriverName'); return false;")
        btnGetTruck.Attributes.Add("onclick", "javascript:openListOfValue('','Truck','Select TruckID, TruckLicense From Truck WHERE (1=1)','TruckID,TruckLicense'); return false;")
        btnGetOrderNo.Attributes.Add("onclick", "javascript:openListOfValue('','OrderLine','Select OrderNoLine, CompartmentNo, ProductID From OrderLine WHERE (1=1)','OrderNoLine,CompartmentNo,ProductID'); return false;")

    End Sub
    Public Sub setTextBoxStyle()
        'style textbox
        txtTotalFilling.Style("text-align") = "right"
        txtVAT.Style("text-align") = "right"
        txtAmountDue.Style("text-align") = "right"
    End Sub
    Public Sub setGridStyle()
        ' set width of column and style of text
        gridFillingLineItem.HeaderRow.Cells(0).Width = 25
        gridFillingLineItem.HeaderRow.Cells(1).Width = 25
        gridFillingLineItem.HeaderRow.Cells(2).Width = 30
        gridFillingLineItem.HeaderRow.Cells(3).Width = 175
        gridFillingLineItem.HeaderRow.Cells(4).Width = 175
        gridFillingLineItem.HeaderRow.Cells(5).Width = 100
        gridFillingLineItem.HeaderRow.Cells(6).Width = 100
        'gridFillingLineItem.HeaderRow.Cells(7).Width = 100
        'gridFillingLineItem.HeaderRow.Cells(8).Width = 100
        gridFillingLineItem.HeaderRow.Style("font-weight") = "normal"
        gridFillingLineItem.HeaderRow.Style("text-align") = "center"
        gridFillingLineItem.HeaderRow.Height = 22

        ' set alignment in each column
        For i = 0 To gridFillingLineItem.Rows.Count - 1
            gridFillingLineItem.Rows(i).Cells(0).Style("text-align") = "center"
            gridFillingLineItem.Rows(i).Cells(1).Style("text-align") = "center"
            gridFillingLineItem.Rows(i).Cells(2).Style("text-align") = "center"
            gridFillingLineItem.Rows(i).Cells(3).Style("text-align") = "left"
            gridFillingLineItem.Rows(i).Cells(3).Style("padding-left") = "5px"
            gridFillingLineItem.Rows(i).Cells(4).Style("text-align") = "left"
            gridFillingLineItem.Rows(i).Cells(4).Style("padding-left") = "5px"
            gridFillingLineItem.Rows(i).Cells(5).Style("text-align") = "right"
            gridFillingLineItem.Rows(i).Cells(5).Style("padding-right") = "5px"
            gridFillingLineItem.Rows(i).Cells(6).Style("text-align") = "right"
            gridFillingLineItem.Rows(i).Cells(6).Style("padding-right") = "5px"
            'gridFillingLineItem.Rows(i).Cells(7).Style("text-align") = "right"
            'gridFillingLineItem.Rows(i).Cells(7).Style("padding-right") = "5px"
            'gridFillingLineItem.Rows(i).Cells(8).Style("text-align") = "right"
            'gridFillingLineItem.Rows(i).Cells(8).Style("padding-right") = "5px"
            gridFillingLineItem.Rows(i).Height = 22
        Next

        gridFillingLineItem.Rows(gridFillingLineItem.Rows.Count - 1).Style("text-align") = "center"
    End Sub
    Public Sub setEnableTextbox()
        txtTruckID.Enabled = True
        txtTruckLicense.Enabled = True
        txtDriverID.Enabled = True
        txtDriverName.Enabled = True
        txtOrderNoLine.Enabled = True
        txtFillingGasDate.Enabled = True

        txtCompartmentNo.Enabled = True
        txtProductID.Enabled = True
        txtFillingQuantity.Enabled = True
        txtTotalFilling.Enabled = True
        txtVAT.Enabled = False
        txtAmountDue.Enabled = False

        If Session("mode") = "NEW" Or Session("mode") = "COPY" Then
            txtFillingGasNo.Enabled = False
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
        clearFillingGasField()
        clearDriverField()
        clearTruckField()
        clearFillingfield()

        txtFillingGasNo.Text = "NEW"

        txtFillingGasDate.Text = Date.Today.ToString("dd/MM/yyyy")
        createNewDataLineItem()
        addLineItem()
        setDirtyBit("CLEAR")

        setEnableTextbox()
    End Sub
    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        Dim FillingNo As String

        ' Check Require Field
        If (txtFillingGasDate.Text = "") Then
            MsgBox("Please enter Filling Gas date.")
            txtFillingGasDate.Focus()
            Exit Sub
        End If
        If (txtTruckID.Text = "") Then
            MsgBox("Please enter Truck ID.")
            txtTruckID.Focus()
            Exit Sub
        End If
        If dataLineItem.Rows.Count = 1 Then
            MsgBox("No data in lineitem.")
            txtOrderNoLine.Focus()
            Exit Sub
        End If


        If txtFillingGasNo.Text = "NEW" Then

            'Running Number
            Dim maxIN As String
            Dim strSplit As Array

            connectDB()
            mySqlCon.Open()

            Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()
            Dim Transaction = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted)
            command.Connection = mySqlCon
            command.Transaction = Transaction                           '*** Command & Transaction ***'
            command.CommandText = "SELECT MAX(FillingGasNo) FROM FillingGas"
            Try
                'command.Prepare()
                command.ExecuteNonQuery()
                mySqlReader = command.ExecuteReader()
                If mySqlReader.HasRows = True Then
                    mySqlReader.Read()
                    strSplit = Split(mySqlReader.Item(0), "F")
                    maxIN = strSplit(1)
                Else
                    maxIN = 0
                End If
                FillingNo = "F" + Format(maxIN + 1, "000")
                mySqlReader.Close()

                ' insert new invoice into db
                Try
                    command.CommandText = "INSERT INTO FillingGas(FillingGasNo, FillingGasDate, DriverID, DriverName, TruckID, TruckLicense, TotalFill) " & _
                                          "VALUES (?,?,?,?,?,?,?)"
                    command.Parameters.Add("FillingGasNo", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("FillingGasDate", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("DriverID", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("DriverName", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("TruckID", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("TruckLicense", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("TotalFill", Data.OleDb.OleDbType.Double)
                    ' command.Parameters.Add("TotalAmount", Data.OleDb.OleDbType.Numeric, 18)
                    'command.Parameters.Add("VAT", Data.OleDb.OleDbType.Numeric, 18)
                    'command.Parameters.Add("AmountDue", Data.OleDb.OleDbType.Numeric, 18)
                    command.Parameters(0).Value = FillingNo
                    command.Parameters(1).Value = txtFillingGasDate.Text
                    command.Parameters(2).Value = txtDriverID.Text
                    command.Parameters(3).Value = txtDriverName.Text
                    command.Parameters(4).Value = txtTruckID.Text()
                    command.Parameters(5).Value = txtTruckLicense.Text
                    command.Parameters(6).Value = txtTotalFilling.Text


                    ' command.Parameters(3).Value = Format(txtTotalWeight.Text, "General Number")
                    'command.Parameters(4).Value = Format(txtVAT.Text, "General Number")
                    'command.Parameters(5).Value = Format(txtAmountDue.Text, "General Number")
                    command.ExecuteNonQuery()

                    '' insert lineItem into db
                    Dim i As Integer
                    For i = 0 To dataLineItem.Rows.Count - 1
                        If i = editLine Then
                            Continue For
                        End If

                        command.Parameters.Clear()
                        command.CommandText = "INSERT INTO FillingGasLine (FillingGasNoLine, OrderNoLine, CompartmentNo, ProductID, FillingQuantity) " & _
                                                " VALUES (?,?,?,?,?)"
                        command.Parameters.Add("FillingGasNoLine", Data.OleDb.OleDbType.VarWChar, 50)
                        command.Parameters.Add("OrderNoLine", Data.OleDb.OleDbType.VarWChar, 50)
                        command.Parameters.Add("CompartmentNo", Data.OleDb.OleDbType.VarWChar, 50)
                        command.Parameters.Add("ProductID", Data.OleDb.OleDbType.VarWChar, 50)
                        command.Parameters.Add("FillingQuantity", Data.OleDb.OleDbType.Numeric, 18)

                        command.Parameters(0).Value = FillingNo
                        command.Parameters(1).Value = dataLineItem.Rows(i).Item(0)
                        command.Parameters(2).Value = dataLineItem.Rows(i).Item(1)
                        command.Parameters(3).Value = dataLineItem.Rows(i).Item(2)
                        command.Parameters(4).Value = Format(dataLineItem.Rows(i).Item(3), "General Number")

                        'command.Parameters(4).Value = Format(dataLineItem.Rows(i).Item(3), "General Number")
                        'command.Parameters(5).Value = Format(dataLineItem.Rows(i).Item(4), "General Number")
                        ' command.Parameters(5).Value = Format(dataLineItem.Rows(i).Item(4), "General Number")
                        'command.Parameters(6).Value = (i + 1)
                        command.ExecuteNonQuery()

                    Next
                    Transaction.Commit() '*** Commit Transaction ***'
                    setDirtyBit("CLEAR")
                    showInvoice(lbFillingGasNo)
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
            FillingNo = txtFillingGasNo.Text
            connectDB()
            mySqlCon.Open()
            Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()
            Dim Transaction = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted)
            command.Connection = mySqlCon
            command.Transaction = Transaction                           '*** Command & Transaction ***'


            Try
                command.CommandText = "UPDATE FillingGas SET " & _
                                       "FillingGasDate=?, DriverID=?, DriverName=?, TruckID=?, TruckLicense=? TotalFill=? " & _
                                       "WHERE FillingGasNo = ? "

                'command.Parameters.Add("ProductNo", Data.OleDb.OleDbType.VarWChar, 10)
                command.Parameters.Add("FillingGasDate", Data.OleDb.OleDbType.VarWChar, 50)
                ' command.Parameters.Add("TotalAmount", Data.OleDb.OleDbType.Numeric, 18)
                ' command.Parameters.Add("VAT", Data.OleDb.OleDbType.Numeric, 18)
                '  command.Parameters.Add("AmountDue", Data.OleDb.OleDbType.Numeric, 18)
                command.Parameters.Add("DriverID", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("DriverName", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("TruckID", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("TruckLicense", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("FillingGasNo", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("TotalFill", Data.OleDb.OleDbType.Double)
                'command.Parameters(0).Value = txtProductNo.Text
                command.Parameters(0).Value = txtFillingGasDate.Text
                ' command.Parameters(1).Value = Format(txtTotalWeight.Text, "General Number")
                ' command.Parameters(2).Value = Format(txtVAT.Text, "General Number")
                'command.Parameters(3).Value = Format(txtAmountDue.Text, "General Number")
                command.Parameters(1).Value = txtDriverID.Text
                command.Parameters(2).Value = txtDriverName.Text
                command.Parameters(3).Value = txtTruckID.Text
                command.Parameters(4).Value = txtTruckLicense.Text
                command.Parameters(5).Value = FillingNo
                command.Parameters(6).Value = txtTotalFilling.Text
                command.ExecuteNonQuery()

                command.Parameters.Clear()
                command.CommandText = "DELETE FROM FillingGasLine WHERE FillingNoLine = ? "
                command.Parameters.Add("FillingNoDel", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters(0).Value = FillingNo
                command.ExecuteNonQuery()

                Dim i As Integer
                For i = 0 To dataLineItem.Rows.Count - 1
                    If i = editLine Then
                        Continue For
                    End If
                    command.Parameters.Clear()
                    command.CommandText = "INSERT INTO FillingGasLine (FillingGasNoLine, OrderNoLine, CompartmentNo, ProductID, FillingQuantity) " & _
                                           "VALUES (?,?,?,?,?) "

                    command.Parameters.Add("FillingGasNoLine", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("OrderNoLine", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("CompartmentNo", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("ProductID", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("FillingQuantity", Data.OleDb.OleDbType.Numeric, 18)
                    'command.Parameters.Add("OrderList", Data.OleDb.OleDbType.Numeric, 18)
                    command.Parameters(0).Value = FillingNo
                    command.Parameters(1).Value = dataLineItem.Rows(i).Item(0)
                    command.Parameters(2).Value = dataLineItem.Rows(i).Item(1)
                    command.Parameters(3).Value = dataLineItem.Rows(i).Item(2)

                    command.Parameters(4).Value = Format(dataLineItem.Rows(i).Item(3), "General Number")

                    'command.Parameters(3).Value = Format(dataLineItem.Rows(i).Item(2), "General Number")
                    'command.Parameters(4).Value = Format(dataLineItem.Rows(i).Item(3), "General Number")
                    'command.Parameters(5).Value = (i + 1)
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

        If (txtFillingGasNo.Text = "") Or (txtFillingGasNo.Text = "NEW") Then
            txtFillingGasNo.Enabled = True
            MsgBox("Please select Filling Gas No for delete.")
            Exit Sub
        End If

        If (MsgBox("Do you want to delete Filling No '" & txtFillingGasNo.Text & "' ", MsgBoxStyle.YesNo) = MsgBoxResult.Yes) Then
            Dim FillingNo As String
            FillingNo = txtFillingGasNo.Text

            connectDB()
            mySqlCon.Open()

            ' Create the Command. 
            Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()
            Dim Transaction = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted)

            ' Set the Connection, CommandText and Parameters.
            command.Connection = mySqlCon
            command.CommandText = "DELETE FROM FillingGas WHERE FillingGasNo = ?"
            command.Parameters.Add("FillingGasNo", Data.OleDb.OleDbType.VarWChar, 50)
            command.Parameters(0).Value = FillingNo

            '*** Command & Transaction ***'
            command.Transaction = Transaction
            command.Prepare()
            Try
                command.ExecuteNonQuery()

                command.CommandText = "DELETE FROM FillingGas WHERE FillingGasNo = ?"
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
        dataLineItem = New DataTable("FillingGasLine")

        dataLineItem.Columns.Add(New DataColumn("OrderNoLine"))
        dataLineItem.Columns.Add(New DataColumn("CompartmentNo"))
        dataLineItem.Columns.Add(New DataColumn("ProductID"))
        dataLineItem.Columns.Add(New DataColumn("FillingQuantity"))

    End Sub
    Protected Sub addLineItem()
        If (dataLineItem Is Nothing) Then
            createNewDataLineItem()
        End If
        Dim dr As DataRow

        dr = dataLineItem.NewRow()

        dr("OrderNoLine") = ""
        dr("CompartmentNo") = ""
        dr("ProductID") = ""
        dr("FillingQuantity") = ""


        dataLineItem.Rows.Add(dr)
        updateGridLineItem()
        Session("dataLineItem") = dataLineItem
        insertLineItem(dataLineItem.Rows.Count - 1)

    End Sub
    Protected Sub insertLineItem(ByVal line As Integer)
        editLine = line
        Session("editLine") = line

        gridFillingLineItem.Rows(line).Cells(0).Controls.Clear()
        gridFillingLineItem.Rows(line).Cells(0).Controls.Add(btnInsertProduct)
        gridFillingLineItem.Rows(line).Cells(1).Controls.Clear()
        gridFillingLineItem.Rows(line).Cells(1).Controls.Add(btnCancelProduct)
        gridFillingLineItem.Rows(line).Cells(3).Controls.Add(lbRequireField0)
        gridFillingLineItem.Rows(line).Cells(3).Controls.Add(txtOrderNoLine)
        gridFillingLineItem.Rows(line).Cells(3).Controls.Add(btnGetOrderNo)
        gridFillingLineItem.Rows(line).Cells(4).Controls.Add(txtCompartmentNo)
        gridFillingLineItem.Rows(line).Cells(5).Controls.Add(lbRequireField1)
        gridFillingLineItem.Rows(line).Cells(5).Controls.Add(txtProductID)
        gridFillingLineItem.Rows(line).Cells(6).Controls.Add(txtFillingQuantity)


    End Sub

    Protected Sub updateGridLineItem()
        ' Copy data from Collection to DataTable
        If (dataLineItem Is Nothing) Then
            dataLineItem = New DataTable("FillingGasLine")
            dataLineItem.Columns.Add(New DataColumn("OrderNoLine"))
            dataLineItem.Columns.Add(New DataColumn("CompartmentNo"))
            dataLineItem.Columns.Add(New DataColumn("ProductID"))
            dataLineItem.Columns.Add(New DataColumn("FillingQuantity"))

        End If
        gridFillingLineItem.DataSource = dataLineItem
        gridFillingLineItem.DataBind()
        setGridStyle()

    End Sub
    Protected Sub setColumnName()
        If Session("language") = "EN" Then
            gridFillingLineItem.HeaderRow.Cells(3).Text = "Order No"
            gridFillingLineItem.HeaderRow.Cells(4).Text = "Compartment No"
            gridFillingLineItem.HeaderRow.Cells(5).Text = "ProductID"
            gridFillingLineItem.HeaderRow.Cells(6).Text = "FillingQuantity"

        ElseIf Session("language") = "TH" Then
            gridFillingLineItem.HeaderRow.Cells(3).Text = "รายการเลขที่"
            gridFillingLineItem.HeaderRow.Cells(4).Text = "ช่องเติมผลิตภัณฑ์"
            gridFillingLineItem.HeaderRow.Cells(5).Text = "ชื่อผลิตภัณฑ์"
            gridFillingLineItem.HeaderRow.Cells(6).Text = "ปริมาณการเติม"

        End If
    End Sub


    Protected Sub grdFillingLineItem_RowEditing(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gridFillingLineItem.RowEditing
        setGridStyle()

        If Not editLine = -1 Then
            dataLineItem.Rows.RemoveAt(editLine)
        End If

        editLine = -1
        Session("editLine") = editLine
        gridFillingLineItem.EditIndex = e.NewEditIndex

        updateGridLineItem()


        ' Textbox in row 
        Dim textboxRow As TextBox
        gridFillingLineItem.HeaderRow.Cells(0).Width = 50
        textboxRow = gridFillingLineItem.Rows(e.NewEditIndex).Cells(3).Controls(0)
        textboxRow.Width = 100
        textboxRow = gridFillingLineItem.Rows(e.NewEditIndex).Cells(4).Controls(0)
        textboxRow.Width = 100
        textboxRow = gridFillingLineItem.Rows(e.NewEditIndex).Cells(5).Controls(0)
        textboxRow.Width = 100
        textboxRow = gridFillingLineItem.Rows(e.NewEditIndex).Cells(6).Controls(0)
        textboxRow.Width = 100


        gridFillingLineItem.Rows(e.NewEditIndex).Cells(5).Controls.Add(lbRequireField1)

        gridFillingLineItem.Rows(e.NewEditIndex).Cells(3).Enabled = False
        gridFillingLineItem.Rows(e.NewEditIndex).Cells(4).Enabled = False
        gridFillingLineItem.Rows(e.NewEditIndex).Cells(6).Enabled = False

    End Sub

    Protected Sub grdFillingLineItem_CancelEditing(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gridFillingLineItem.RowCancelingEdit
        gridFillingLineItem.EditIndex = -1
        addLineItem()
    End Sub
    Protected Sub grdFillingLineItem_RowDeleting(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles gridFillingLineItem.RowDeleting
        If Not editLine = -1 Then
            dataLineItem.Rows.RemoveAt(editLine)
            MsgBox(editLine)

        End If
        editLine = -1
        MsgBox(e.RowIndex)
        dataLineItem.Rows.RemoveAt(e.RowIndex)
        addLineItem()
        'calculateAmount()
        setDirtyBit("DIRTY")
    End Sub

    Protected Sub grdFillingLineItem_RowUpdateing(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gridFillingLineItem.RowUpdating
        Dim Row = gridFillingLineItem.Rows(e.RowIndex)
        Dim Quantity As Integer = CInt(e.NewValues(2))
        Dim UnitPrice As Double = CDbl(e.NewValues(3))


        ' Check Quantity
        If (Quantity < 1) Then
            MsgBox("Please enter quantity.")
            'txtInvoiceNo.Text = grdProductLineItem.Rows(e.RowIndex).Cells(2).Text
            'grdInvoiceLineItem.Rows(e.RowIndex).Cells(2).
            'gridFillingLineItem.Rows(e.RowIndex).Cells(2).Focus()
            Exit Sub
        End If

        dataLineItem.Rows(e.RowIndex).Item(1) = e.NewValues(1)
        dataLineItem.Rows(e.RowIndex).Item(2) = Quantity
        dataLineItem.Rows(e.RowIndex).Item(3) = FormatNumber(Quantity, 2)
        gridFillingLineItem.Rows(e.RowIndex).Cells(4).Enabled = True
        gridFillingLineItem.Rows(e.RowIndex).Cells(6).Enabled = True


        gridFillingLineItem.EditIndex = -1
        updateGridLineItem()
        addLineItem()
        calculateAmount()
        setDirtyBit("DIRTY")
    End Sub

    Protected Sub btnCancelProduct_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnCancelProduct.Click
        clearFillingfield()
    End Sub
    Protected Sub btnInsertProduct_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnInsertProduct.Click
        ' Check Require Field
        If (txtOrderNoLine.Text = "") Then
            MsgBox("Please enter product code.")
            txtOrderNoLine.Focus()
            Exit Sub
        End If
        If (txtCompartmentNo.Text = "") Then
            MsgBox("Please enter quantity.")
            txtCompartmentNo.Focus()
            Exit Sub
        End If

        ' Add Data in Row
        If dataLineItem Is Nothing Then
            createNewDataLineItem()
        End If
        Dim dr As DataRow
        dr = dataLineItem.NewRow()

        dr("OrderNoLine") = txtOrderNoLine.Text
        dr("CompartmentNo") = txtCompartmentNo.Text
        dr("ProductID") = txtProductID.Text
        dr("FillingQuantity") = FormatNumber(txtFillingQuantity.Text, 2)

        dataLineItem.Rows.RemoveAt(editLine)
        dataLineItem.Rows.InsertAt(dr, editLine)
        updateGridLineItem()
        editLine = -1
        calculateAmount()
        addLineItem()
        setDirtyBit("DIRTY")
        clearFillingfield()
        Session("dataLineItem") = dataLineItem
        txtOrderNoLine.Focus()
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
            TotalAmount = TotalAmount + tempPrice
        Next

        txtTotalFilling.Text = FormatNumber(TotalAmount, 2)
        'txtVAT.Text = FormatNumber(TotalAmount * 0.07, 2)
        'txtAmountDue.Text = FormatNumber(TotalAmount + (TotalAmount * 0.07), 2)
    End Sub
#End Region

#Region "Text Changed"
    Protected Sub txtFillingGassNo_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtFillingGasNo.TextChanged
        showInvoice(txtFillingGasNo.Text)

        If Session("mode") = "COPY" Then            ' in mode COPY
            txtFillingGasNo.Enabled = True
            txtFillingGasNo.Text = "NEW"               ' set text = NEW
            txtFillingGasNo.Enabled = False
        End If
    End Sub
    Protected Sub txtDriverID_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDriverID.TextChanged
        ' query Customer

        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT DriverID, DriverName " & _
                              "FROM   Driver " & _
                              "WHERE  DriverID =? "
        command.Parameters.Add("DriverID", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = txtDriverID.Text

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        mySqlCmd = New Data.OleDb.OleDbCommand(SQL, mySqlCon)

        Try
            mySqlReader = command.ExecuteReader()
            If mySqlReader.HasRows = True Then
                mySqlReader.Read()
                txtDriverID.Text = mySqlReader.Item(0)
                txtDriverName.Text = mySqlReader.Item(1)
                setDirtyBit("DIRTY")                                ' set dirtybit
            Else
                clearDriverField()
                txtDriverID.Focus()
            End If
        Catch ex As Exception
        End Try
        mySqlCon.Close()

    End Sub
    Protected Sub txtTruckID_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTruckID.TextChanged
        ' query Customer

        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT TruckID, TruckLicense " & _
                              "FROM   Truck " & _
                              "WHERE  TruckID =? "
        command.Parameters.Add("TruckID", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = txtTruckID.Text

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        mySqlCmd = New Data.OleDb.OleDbCommand(SQL, mySqlCon)

        Try
            mySqlReader = command.ExecuteReader()
            If mySqlReader.HasRows = True Then
                mySqlReader.Read()
                txtTruckID.Text = mySqlReader.Item(0)
                txtTruckLicense.Text = mySqlReader.Item(1)
                setDirtyBit("DIRTY")                                ' set dirtybit
            Else
                clearTruckField()
                txtTruckID.Focus()
            End If
        Catch ex As Exception
        End Try
        mySqlCon.Close()

    End Sub
    Protected Sub txtOrderNoLine_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtOrderNoLine.TextChanged
        ' query Customer

        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT OrderNoLine, CompartmentNo, ProductID " & _
                              "FROM OrderLine " & _
                              "WHERE  OrderNoLine =? "
        command.Parameters.Add("OrderNoLine", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = txtOrderNoLine.Text

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        mySqlCmd = New Data.OleDb.OleDbCommand(SQL, mySqlCon)

        Try
            mySqlReader = command.ExecuteReader()
            If mySqlReader.HasRows = True Then
                mySqlReader.Read()
                txtOrderNoLine.Text = mySqlReader.Item(0)
                txtCompartmentNo.Text = mySqlReader.Item(1)
                txtProductID.Text = mySqlReader.Item(2)

                setDirtyBit("DIRTY")                                ' set dirtybit
            Else
                clearFillingfield()
                txtOrderNoLine.Focus()
            End If
        Catch ex As Exception
        End Try
        mySqlCon.Close()

    End Sub

    'Protected Sub txtProductCode_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtProductCode.TextChanged
    '    ' query Product

    '    connectDB()
    '    mySqlCon.Open()

    '    Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

    '    ' Set the Connection, CommandText and Parameters.
    '    command.Connection = mySqlCon
    '    command.CommandText = "SELECT ProductCode, ProductName, UnitPrice " & _
    '                          "FROM Product " & _
    '                          "WHERE ProductCode = ?"
    '    command.Parameters.Add("ProductCode", Data.OleDb.OleDbType.VarWChar, 10)
    '    command.Parameters(0).Value = txtProductCode.Text

    '    ' Call  Prepare and ExecuteNonQuery.
    '    command.Prepare()
    '    command.ExecuteNonQuery()

    ' Load Data Line Item into Collection
    'Try
    '    mySqlReader = command.ExecuteReader()
    '    If mySqlReader.HasRows = True Then
    '        mySqlReader.Read()
    '        txtProductCode.Text = mySqlReader.Item(0)
    '        txtProductName.Text = mySqlReader.Item(1)
    '        txtUnitPrice.Text = FormatNumber(mySqlReader.Item(2), 2)
    '        setDirtyBit("DIRTY")                ' set dirtybit
    '    Else                                    ' if it cannot query data
    '        clearProductfield()                 ' clear data in textbox
    '        txtProductCode.Focus()              ' focus textbox for type new ProductCode
    '        Exit Sub
    '    End If
    'Catch ex As Exception

    'End Try

    '    mySqlCon.Close()

    'End Sub
    Protected Sub txtFillingGasDate_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtFillingGasDate.TextChanged
        setDirtyBit("DIRTY")
    End Sub
#End Region

#Region "Clear Textbox"
    Public Sub clearFillingGasField()
        txtFillingGasNo.Text = ""
        txtFillingGasDate.Text = ""
        txtTotalFilling.Text = ""
        'txtVAT.Text = ""
        'txtAmountDue.Text = ""
    End Sub
    Public Sub clearDriverField()
        txtDriverID.Text = ""
        txtDriverName.Text = ""

    End Sub
    Public Sub clearTruckField()
        txtTruckID.Text = ""
        txtTruckLicense.Text = ""
    End Sub
    Public Sub clearFillingfield()
        txtOrderNoLine.Text = ""
        txtCompartmentNo.Text = ""
        txtProductID.Text = ""
        txtFillingQuantity.Text = ""

    End Sub
#End Region

    Public Sub showInvoice(ByVal FillingNo)
        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT FG.FillingGasNo, FG.FillingGasDate, FG.DriverID, FG.DriverName, FG.TruckID, FG.TruckLicense, FG.TotalFill " & _
                              "FROM   ((FillingGas FG INNER JOIN Driver D ON FG.DriverID = D.DriverID) INNER JOIN Truck T ON FG.TruckID = T.TruckID) " & _
                              "WHERE  FG.FillingGasNo=?"
        command.Parameters.Add("FillingGasNo", Data.OleDb.OleDbType.VarWChar, 50)
        command.Parameters(0).Value = FillingNo

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        Try
            mySqlReader = command.ExecuteReader()
            If mySqlReader.HasRows = True Then
                mySqlReader.Read()
                txtFillingGasNo.Text = mySqlReader.Item(0)
                txtFillingGasDate.Text = mySqlReader.Item(1)
                txtDriverID.Text = mySqlReader.Item(2)
                txtDriverName.Text = mySqlReader.Item(3)
                txtTruckID.Text = mySqlReader.Item(4)
                txtTruckLicense.Text = mySqlReader.Item(5)
                txtTotalFilling.Text = mySqlReader.Item(6)

                'txtTotalWeight.Text = FormatNumber(mySqlReader.Item(4), 2)
                '  txtVAT.Text = FormatNumber(mySqlReader.Item(5), 2)
                ' txtAmountDue.Text = FormatNumber(mySqlReader.Item(6), 2)

                ' Load Data Line Item into Collection
                createNewDataLineItem()
                showFillingLine(FillingNo)
                updateGridLineItem()
                addLineItem()
                calculateAmount()
            Else
                clearFillingGasField()
                clearDriverField()
                clearTruckField()

            End If
        Catch ex As Exception

        End Try

        mySqlCon.Close()

        If Session("mode") = "COPY" Then
            txtFillingGasNo.Text = "NEW"
        End If

        'txtProductNo.Enabled = False
        'setEnableTextbox()

    End Sub
    Public Sub showFillingLine(ByVal FillingNoLine)
        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT OrderNoLine, CompartmentNo, ProductID, FillingQuantity " & _
                              "FROM FillingGasLine " & _
                              "WHERE FillingGasNoLine = ? "
        command.Parameters.Add("FillingGasNoLine", Data.OleDb.OleDbType.VarWChar, 10)
        command.Parameters(0).Value = FillingNoLine

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        Try

            mySqlReader = command.ExecuteReader()
            While mySqlReader.Read()
                Dim dr As DataRow
                dr = dataLineItem.NewRow()
                dr("OrderNoLine") = mySqlReader.Item(0)
                dr("CompartmentNo") = mySqlReader.Item(1)
                dr("ProductID") = mySqlReader.Item(2)
                dr("FillingQuantity") = FormatNumber(mySqlReader.Item(3), 2)


                dataLineItem.Rows.Add(dr)
            End While
            Session("dataLineItem") = dataLineItem
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        mySqlCon.Close()
    End Sub


End Class