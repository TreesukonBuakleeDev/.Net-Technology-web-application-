Imports CrystalDecisions.Shared

Public Class frmCrystalReport
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' get InvoiceNo form Invoice Form
        Dim InvoiceNo As String = Request.QueryString("InvoiceNo")

        ' connect to database
        Dim m_Connection As New Data.SqlClient.SqlConnection
        m_Connection.ConnectionString = "Server=GLUAY-PC\SQLEXPRESS;UID=invoice;PASSWORD=1234;database=invoice;Max Pool Size=400;Connect Timeout=600;"

        Dim myDS As New DatasetReport
        Dim MyDA As New Data.SqlClient.SqlDataAdapter
        Dim MyCommand As New Data.SqlClient.SqlCommand

        ' query data for report
        MyCommand.Connection = m_Connection
        MyCommand.CommandText = "SELECT * FROM InvoiceLineItem " & _
                                "WHERE InvoiceNo='" & InvoiceNo & "' ORDER BY OrderList"
        MyCommand.CommandType = Data.CommandType.Text
        ' save data to dataset
        MyDA.SelectCommand = MyCommand
        MyDA.Fill(myDS, "InvoiceLineItem")

        ' query data for report
        MyCommand.CommandText = "SELECT * FROM Invoice " & _
                                "WHERE InvoiceNo='" & InvoiceNo & "'"
        MyCommand.CommandType = Data.CommandType.Text
        ' save data to dataset
        MyDA.SelectCommand = MyCommand
        MyDA.Fill(myDS, "Invoice")

        ' map this file to CrystalReport_Product.rpt
        Dim oRpt As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        oRpt.Load(Server.MapPath("CrystalReport_Invoice.rpt"))

        oRpt.SetDataSource(myDS)

        ' set output format (PDF) and filename
        oRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, True, "InvoiceNo - " & InvoiceNo)
        CrystalReportViewer_Invoice.ReportSource = oRpt
        CrystalReportViewer_Invoice.DisplayToolbar = False
    End Sub

End Class