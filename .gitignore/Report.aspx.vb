Imports CrystalDecisions.Shared

Public Class Report
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' get InvoiceNo form Invoice Form
        Dim FillingGasNo As String = Request.QueryString("FillingGasNo")

        ' connect to database
        Dim m_Connection As New Data.SqlClient.SqlConnection
        m_Connection.ConnectionString = "Server=AUTO-PC\SQLEXPRESS;UID=CPE332;PASSWORD=cpe332ae;database=Automation;Max Pool Size=400;Connect Timeout=600;"

        Dim myDS As New DataSet
        Dim MyDA As New Data.SqlClient.SqlDataAdapter
        Dim MyCommand As New Data.SqlClient.SqlCommand

        '' query data for report
        'MyCommand.Connection = m_Connection
        'MyCommand.CommandText = "SELECT F.OrderNoLine FROM FillingGasLine F " & _
        '                        "WHERE OrderNoLine='"
        'MyCommand.CommandType = Data.CommandType.Text
        '' save data to dataset
        'MyDA.SelectCommand = MyCommand
        'MyDA.Fill(myDS, "FillingGas")

        ' query data for report
        MyCommand.CommandText = "SELECT FillingGasNo, FillingGasDate, TruckID, TruckLicense, DriverID, DriverName FROM FillingGas " & _
                                "WHERE FillingGasNo='" & FillingGasNo & "'"
        MyCommand.CommandType = Data.CommandType.Text
        ' save data to dataset
        MyDA.SelectCommand = MyCommand
        MyDA.Fill(myDS, "FillingGas")

        ' map this file to CrystalReport_Product.rpt
        Dim oRpt As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        oRpt.Load(Server.MapPath("CrystalReport1.rpt"))

        oRpt.SetDataSource(myDS)

        ' set output format (PDF) and filename
        oRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, True, "FillingGasNo - " & FillingGasNo)
        CrystalReportViewer.ReportSource = oRpt
        CrystalReportViewer.DisplayToolbar = False
    End Sub

End Class