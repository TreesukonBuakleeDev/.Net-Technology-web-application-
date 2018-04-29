Public Class product
    Inherits System.Web.UI.Page
    ' global variable
    Dim mySqlCon As Data.OleDb.OleDbConnection
    Dim mySqlCmd As Data.OleDb.OleDbCommand
    Dim mySqlReader As Data.OleDb.OleDbDataReader
    Dim SQL As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' when first load
        If Not IsPostBack Then
            btnNew_Click(Nothing, Nothing)  ' set mode NEW
            setLanguage("EN")               ' set Language (EN)
            setLabel()                      ' set Label 
            setButton()                     ' set attribute of button
        Else
            setLabel()                      ' set Label 
        End If

    End Sub

    Public Sub connectDB()
        ' connect to database
        Dim sConnString As String

        'sConnString = "Provider=SQLOLEDB.1;Data Source=SERVER_NAME;" & _
        '              "Initial Catalog=DB_NAME;User ID=USER;Password=PASSWORD"
        sConnString = "Provider=SQLOLEDB.1;Data Source=GLUAY-PC\SQLEXPRESS;" & _
                      "Initial Catalog=invoice;User ID=invoice;Password=1234"
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

            lbProductCode.Text = "Product Code"
            lbProductName.Text = "Product Name"
            lbUnits.Text = "Units"
            lbUnitPrice.Text = "UnitPrice"

        ElseIf Session("language") = "TH" Then
            lbNew.Text = "สร้าง"
            lbEdit.Text = "แก้ไข"
            lbCopy.Text = "คัดลอก"
            lbSave.Text = "บันทึก"
            lbDelete.Text = "ลบ"
            lbPrint.Text = "พิมพ์"
            lbClose.Text = "ปิด"

            lbProductCode.Text = "รหัสสินค้า"
            lbProductName.Text = "ชื่อสินค้า"
            lbUnits.Text = "หน่วย"
            lbUnitPrice.Text = "ราคาต่อหน่วย"

        End If
    End Sub
    Public Sub setButton()
        ' set attribute (ONCLICK) of button
        btnNew.Attributes.Add("onclick", "javascript:return checkDirty('NEW');")
        btnEdit.Attributes.Add("onclick", "javascript:if(checkDirty('EDIT')) {openListOfValue('EDIT','Product','Select ProductCode, ProductName From Product WHERE (1=1)','ProductCode,ProductName'); return false; } else {return false;}")
        btnCopy.Attributes.Add("onclick", "javascript:if(checkDirty('COPY') == true) { openListOfValue('COPY','Product','Select ProductCode, ProductName From Product WHERE (1=1)','ProductCode,ProductName'); return false; } else {return false;}")
        btnPrint.Attributes.Add("onclick", "javascript:printProduct();")
        btnClose.Attributes.Add("onclick", "javascript:checkClose();")
    End Sub
    Public Sub setEnableTextbox()
        txtProductName.Enabled = True
        txtUnits.Enabled = True
        txtUnitPrice.Enabled = True

        If Session("mode") = "NEW" Or Session("mode") = "COPY" Then
            txtProductCode.Enabled = False
        End If
    End Sub
    Public Sub setDirtyBit(ByVal value)
        ' set dirtybit : DIRTY/CLEAR
        DirtyBit.Value = value
    End Sub
    Protected Sub btnEN_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnEN.Click
        setLanguage("EN")           ' when click button EN, set language = EN
        setLabel()                  ' and set new label
    End Sub
    Protected Sub btnTH_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnTH.Click
        setLanguage("TH")           ' when click button TH, set language = TH
        setLabel()                  ' and set new label
    End Sub
#End Region

#Region "Menu"
    Protected Sub btnNew_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnNew.Click
        clearProductfield()                 ' clear all field
        setDirtyBit("CLEAR")                ' when click NEW : dirtybit = CLEAR
        Session("mode") = "NEW"             ' when click NEW : mode = NEW
        txtProductCode.Text = "NEW"         ' show "NEW" on textbox
        setEnableTextbox()                  ' and set enable textbox
    End Sub
    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        Dim ProductCode As String

        ' Check Require Field
        If (txtProductName.Text = "") Then
            MsgBox("Please enter product name.")
            txtProductName.Focus()
            Exit Sub
        End If
        If (txtUnits.Text = "") Then
            MsgBox("Please enter units.")
            txtUnits.Focus()
            Exit Sub
        End If
        If (txtUnitPrice.Text = "") Then
            MsgBox("Please enter unit price.")
            txtUnitPrice.Focus()
            Exit Sub
        End If

        ' If click SAVE in NEW/COPY mode, INSERT data into table
        If txtProductCode.Text = "NEW" Then
            'Running Number
            Dim maxP As String
            Dim strSplit As Array

            connectDB()
            mySqlCon.Open()

            Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()
            Dim Transaction = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted)
            command.Connection = mySqlCon
            command.Transaction = Transaction                               '*** Command & Transaction ***
            command.CommandText = "SELECT MAX(ProductCode) FROM Product"    ' select latest ProductCode
            Try
                command.ExecuteNonQuery()
                mySqlReader = command.ExecuteReader()
                mySqlReader.Read()
                If Not mySqlReader.IsDBNull(0) Then
                    strSplit = Split(mySqlReader.Item(0), "P")
                    maxP = strSplit(1)
                Else
                    maxP = 0
                End If
                ProductCode = "P" + Format(maxP + 1, "000")
                mySqlReader.Close()

                ' insert new invoice into db
                Try
                    command.CommandText = "INSERT INTO Product (ProductCode, ProductName, Units, UnitPrice) " & _
                                          "VALUES (?,?,?,?)"
                    command.Parameters.Add("ProductCode", Data.OleDb.OleDbType.VarWChar, 10)
                    command.Parameters.Add("ProductName", Data.OleDb.OleDbType.VarWChar, 200)
                    command.Parameters.Add("Units", Data.OleDb.OleDbType.VarWChar, 50)
                    command.Parameters.Add("UnitPrice", Data.OleDb.OleDbType.Numeric, 10)
                    command.Parameters(0).Value = ProductCode
                    command.Parameters(1).Value = txtProductName.Text
                    command.Parameters(2).Value = txtUnits.Text
                    command.Parameters(3).Value = Format(txtUnitPrice.Text, "General Number")

                    command.ExecuteNonQuery()

                    Transaction.Commit()            '*** Commit Transaction ***'
                    setDirtyBit("CLEAR")            ' set dirtybit = CLEAR
                    Session("mode") = "EDIT"        ' change mode to EDIT
                    showProduct(ProductCode)        ' and show data

                Catch ex As Exception
                    Transaction.Rollback()          '*** RollBack Transaction ***'
                    MsgBox(ex.Message)
                End Try

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
            mySqlCon.Close()

        Else
            ' If click SAVE in EDIT mode, UPDATE data to table
            ProductCode = txtProductCode.Text
            connectDB()
            mySqlCon.Open()
            Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()
            Dim Transaction = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted)
            command.Connection = mySqlCon
            command.Transaction = Transaction       '*** Command & Transaction ***'


            Try
                command.CommandText = "UPDATE Product SET " & _
                                       "ProductName = ?, Units = ?, UnitPrice = ? " & _
                                       "WHERE ProductCode = ?"
                command.Parameters.Add("ProductName", Data.OleDb.OleDbType.VarWChar, 200)
                command.Parameters.Add("Units", Data.OleDb.OleDbType.VarWChar, 50)
                command.Parameters.Add("UnitPrice", Data.OleDb.OleDbType.Numeric, 10)
                command.Parameters.Add("ProductCode", Data.OleDb.OleDbType.VarWChar, 10)
                command.Parameters(0).Value = txtProductName.Text
                command.Parameters(1).Value = txtUnits.Text
                command.Parameters(2).Value = Format(txtUnitPrice.Text, "General Number")
                command.Parameters(3).Value = txtProductCode.Text
                command.ExecuteNonQuery()

                Transaction.Commit()            '*** Commit Transaction ***'
                setDirtyBit("CLEAR")            ' set dirtybit = CLEAR
                Session("mode") = "EDIT"        ' change mode to EDIT
                showProduct(ProductCode)        ' and show data
            Catch ex As Exception
                Transaction.Rollback()          '*** RollBack Transaction ***'
                MsgBox(ex.Message)
            End Try
            mySqlCon.Close()
        End If
    End Sub
    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete.Click
        ' you must select product before DELETE
        If (txtProductCode.Text = "") Or (txtProductCode.Text = "NEW") Then
            MsgBox("Please select product for delete.")
            Exit Sub
        End If

        ' confirm
        If (MsgBox("Do you want to delete product '" & txtProductCode.Text & "' ", MsgBoxStyle.YesNo) = MsgBoxResult.Yes) Then
            Dim ProductCode As String
            ProductCode = txtProductCode.Text

            connectDB()
            mySqlCon.Open()

            ' Create the Command. 
            Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()
            Dim Transaction = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted)

            ' Set the Connection, CommandText and Parameters.
            command.Connection = mySqlCon
            command.CommandText = "DELETE FROM Product WHERE ProductCode = ?"
            command.Parameters.Add("ProductCode", Data.OleDb.OleDbType.VarWChar, 10)
            command.Parameters(0).Value = ProductCode

            '*** Command & Transaction ***'
            command.Transaction = Transaction
            command.Prepare()
            Try
                command.ExecuteNonQuery()
                Transaction.Commit()
            Catch ex As Exception
                Transaction.Rollback()          '*** RollBack Transaction ***'
                MsgBox(ex.Message)
                Exit Sub
            End Try
            mySqlCon.Close()

            btnNew_Click(Nothing, Nothing)
        End If
    End Sub
#End Region



#Region "Text Changed"
    Protected Sub txtProductCode_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtProductCode.TextChanged
        showProduct(txtProductCode.Text)
    End Sub
    
#End Region

#Region "Clear Textbox"
    Public Sub clearProductfield()
        txtProductCode.Text = ""
        txtProductName.Text = ""
        txtUnits.Text = ""
        txtUnitPrice.Text = ""
    End Sub
#End Region

    Sub showProduct(ByVal ProductCode)
        connectDB()
        mySqlCon.Open()

        Dim command As Data.OleDb.OleDbCommand = New Data.OleDb.OleDbCommand()

        ' Set the Connection, CommandText and Parameters.
        command.Connection = mySqlCon
        command.CommandText = "SELECT ProductCode, ProductName, Units, UnitPrice " & _
                              "FROM Product " & _
                              "WHERE ProductCode = ?"
        command.Parameters.Add("ProductCode", Data.OleDb.OleDbType.VarWChar, 10)
        command.Parameters(0).Value = ProductCode

        ' Call  Prepare and ExecuteNonQuery.
        command.Prepare()
        command.ExecuteNonQuery()

        ' Load Data Line Item into Collection
        Try
            mySqlReader = command.ExecuteReader()
            mySqlReader.Read()
            txtProductCode.Text = mySqlReader.Item(0)
            txtProductName.Text = mySqlReader.Item(1)
            txtUnits.Text = mySqlReader.Item(2)
            txtUnitPrice.Text = FormatNumber(mySqlReader.Item(3), 2)
            setDirtyBit("CLEAR")
        Catch ex As Exception

        End Try

        mySqlCon.Close()

        ' when mode = COPY, show "NEW" in textbox ProductCode
        If Session("mode") = "COPY" Then
            txtProductCode.Text = "NEW"
        End If
    End Sub

#Region "setDirty"
    Protected Sub txtProductName_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtProductName.TextChanged
        setDirtyBit("DIRTY")
    End Sub

    Protected Sub txtUnits_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtUnits.TextChanged
        setDirtyBit("DIRTY")
    End Sub

    Protected Sub txtUnitPrice_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtUnitPrice.TextChanged
        setDirtyBit("DIRTY")
    End Sub
#End Region

End Class