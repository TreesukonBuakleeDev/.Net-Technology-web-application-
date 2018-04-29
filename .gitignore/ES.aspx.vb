Imports System.Data.OleDb
Imports System.Data.SqlClient

'Partial Class ES
Public Class ES
    Inherits System.Web.UI.Page
    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Dim connStringExcel As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\AUTO\Desktop\File.xlsx;Extended Properties=""Excel 12.0;HDR=YES;"""
        Dim excelConn As New OleDbConnection(connStringExcel)
        Dim excelCmd As New OleDbCommand("Select * From [Invoice$]", excelConn)

        Try
            excelConn.Open()
            Dim excelReader As OleDbDataReader = excelCmd.ExecuteReader()

            Dim connStringSql As String = "Data Source=AUTO-PC\SQLEXPRESS;Initial Catalog=Automation;Integrated Security=True"
            Dim sqlConn As New SqlConnection(connStringSql)
            Try
                sqlConn.Open()
                Dim bulkCopy As New SqlBulkCopy(sqlConn)

                bulkCopy.DestinationTableName = "Invoice"
                bulkCopy.WriteToServer(excelReader)

                Label1.Text = "Data successfully copied to SQL Server database table"
            Catch exs As Exception
                Label1.Text = exs.Message
            Finally
                sqlConn.Close()

            End Try
        Catch exo As Exception

            Label1.Text = exo.Message
        Finally
            excelConn.Close()
        End Try
    End Sub
End Class
