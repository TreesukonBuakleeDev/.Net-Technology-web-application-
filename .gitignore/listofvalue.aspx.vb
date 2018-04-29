Imports System.Web.UI.WebControls.ListItem

Public Class listofvalue
    Inherits System.Web.UI.Page

    Dim mySqlCon As Data.OleDb.OleDbConnection
    Dim mySqlCmd As Data.OleDb.OleDbCommand
    Dim mySqlReader As Data.OleDb.OleDbDataReader
    Dim SQL As String

    Dim i As Integer
    Dim j As Integer

    Dim mode As String


    Public Sub connectDB()
        ' connect to database
        Dim sConnString As String

        'sConnString = "Provider=SQLOLEDB.1;Data Source=SERVER_NAME;" & _
        '              "Initial Catalog=DB_NAME;User ID=USER;Password=PASSWORD"
        sConnString = "Provider=SQLOLEDB.1;Data Source=AUTO-PC\SQLEXPRESS;" & _
                      "Initial Catalog=Automation;User ID=CPE332;Password=cpe332ae"
        mySqlCon = New Data.OleDb.OleDbConnection(sConnString)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ' when first load, set all value (got from Invoice Form) to SESSION
        If Not IsPostBack Then
            Session("table") = Request.QueryString("table")
            Session("initSQL") = Request.QueryString("initSQL")
            Session("columnname") = Request.QueryString("columnname")
            Session("numberOfColumn") = Request.QueryString("numberOfColumn")
            Session("mode") = Request.QueryString("mode")

            setInitSQL()                ' set initial SQL
            setLabel()                  ' set label
            setButton()                 ' set text on button
            setListCondition()          ' set text in list condition
            bindData()                  ' bind data with gridview
            setTableHeader()            ' set table header with tablesorter
        End If

    End Sub

    Public Sub setButton()
        ' set text on button
        If Session("Language") = "EN" Then
            btnSearch.Text = " Search "
            btnExpand.Text = " Expand "
        ElseIf Session("Language") = "TH" Then
            btnSearch.Text = " ค้นหา "
            btnExpand.Text = " ยกเลิก "
        End If
    End Sub
    Public Sub setInitSQL()
        ' set initial SQL (when first load/click expand)
        sqlCondition.Value = Session("initSQL")
    End Sub
    Public Sub setLabel()
        ' set label
        If Session("Language") = "EN" Then
            lbTitle.Text = Session("table") & " List"
            lbSearch.Text = "Search:"
        ElseIf Session("Language") = "TH" Then
            lbTitle.Text = Session("table") & " List"
            lbSearch.Text = "ค้นหา:"
        End If
    End Sub
    Public Sub setListCondition()
        ' set text in list condition
        listCondition.Items.Clear()
        listCondition.Items.Add("start with")
        listCondition.Items.Add("has")
        listCondition.Items.Add("between")
        listCondition.Items.Add("=")
        listCondition.Items.Add(">")
        listCondition.Items.Add(">=")
        listCondition.Items.Add("<")
        listCondition.Items.Add("<=")
    End Sub
    Public Sub setTableHeader()
        ' set table header with tablesorter
        gridListOfValue.CssClass = "tablesorter"
        If gridListOfValue.Rows.Count > 0 Then
            gridListOfValue.UseAccessibleHeader = True
            gridListOfValue.HeaderRow.TableSection = TableRowSection.TableHeader
        End If
    End Sub

    Public Sub bindData()
        Dim SQL As String
        SQL = sqlCondition.Value

        connectDB()
        mySqlCon.Open()
        mySqlCmd = New Data.OleDb.OleDbCommand(SQL, mySqlCon)
        Try
            mySqlReader = mySqlCmd.ExecuteReader()

            '*** BindData to GridView ***'
            gridListOfValue.DataSource = mySqlReader
            gridListOfValue.DataBind()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        mySqlCon.Close()


    End Sub

    Protected Sub btnExpand_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnExpand.Click
        ' when click expand
        setInitSQL()            ' set initial SQL
        bindData()              ' bind data with gridview
        setTableHeader()        ' set tableheader
    End Sub
    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click

        ' set SQL for query
        Dim strCondition As String

        If listCondition.Text = "start with" Then
            strCondition = "(" & txtColumnName.Text & " like '" & txtSearch1.Text & "%')"
        ElseIf listCondition.Text = "has" Then
            strCondition = "(" & txtColumnName.Text & " like '%" & txtSearch1.Text & "%')"
        ElseIf listCondition.Text = "between" Then
            strCondition = "(" & txtColumnName.Text & " between '" & txtSearch1.Text & "' AND '" & txtSearch2.Text & "')"
        Else
            strCondition = "(" & txtColumnName.Text & " " & listCondition.SelectedValue & "'" & txtSearch1.Text & "')"
        End If

        sqlCondition.Value = sqlCondition.Value & " AND " & strCondition

        bindData()              ' bind data with gridview
        setTableHeader()        ' set tableheader
    End Sub
    Private Sub gridListOfValue_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gridListOfValue.RowDataBound
        Dim rowData As String
        rowData = e.Row.Cells(1).Text

        For i = 2 To Session("numberOfColumn")
            rowData = rowData & ";" & e.Row.Cells(i).Text
        Next

        e.Row.Attributes.Add("onclick", "javascript:getListOfValue('" & rowData & "');")
    End Sub
End Class
