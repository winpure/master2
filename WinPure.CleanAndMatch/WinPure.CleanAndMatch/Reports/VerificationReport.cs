using System.Reflection;
using DevExpress.XtraCharts;
using WinPure.AddressVerification.Models;

namespace WinPure.CleanAndMatch.Reports;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public partial class VerificationReport : DevExpress.XtraReports.UI.XtraReport
{
    public VerificationReport()
    {
        InitializeComponent();
    }

    private void VerificationReport_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
    {
        chrtSource.Series.Clear();
        if (DataSource is AddressVerificationReport rd)
        {

            var seriesSrc1 = new Series(Resources.CAPTION_REPORT_RECORDCOUNT, ViewType.Bar);
            var seriesSrc2 = new Series(Resources.CAPTION_REPORT_ADDRESSVERIFIED, ViewType.Bar);
            var seriesSrc3 = new Series(Resources.CAPTION_REPORT_GEOCODESFOUND, ViewType.Bar);

            seriesSrc1.Points.Add(new SeriesPoint(Resources.CAPTION_REPORT_TOTAL, rd.CommonData.TotalRecords));
            seriesSrc2.Points.Add(new SeriesPoint(Resources.CAPTION_REPORT_ADDRESS, rd.CommonData.AddressSuccess));
            seriesSrc3.Points.Add(new SeriesPoint(Resources.CAPTION_REPORT_GEOCODE, rd.CommonData.GeoCodeSuccess));

            chrtSource.Series.Add(seriesSrc1);
            chrtSource.Series.Add(seriesSrc2);
            chrtSource.Series.Add(seriesSrc3);
        }
    }
}