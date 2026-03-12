using System.Reflection;
using DevExpress.XtraCharts;
using WinPure.Matching.Models.Reports;

namespace WinPure.CleanAndMatch.Reports;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public partial class MatchReport : DevExpress.XtraReports.UI.XtraReport
{
    public MatchReport()
    {
        InitializeComponent();
    }

    private void MatchReport_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
    {
        chrtResult.Series.Clear();
        chrtSource.Series.Clear();
        if (DataSource is ReportData rd)
        {
            if (!rd.MatchSettings.MatchAcrossTables)
            {
                lbMainTableCaption.Visible = false;
                lbMainTableName.Visible = false;
            }

            var series1 = new Series(Resources.CAPTION_MATCHING_RESULT, ViewType.Doughnut3D);


            foreach (var res in rd.ResultData)
            {
                series1.Points.Add(new SeriesPoint(res.Description, res.RecordValue));
            }
            chrtResult.Series.Add(series1);
            series1.Label.LineLength = 30;
            ((SimpleDiagram3D)chrtResult.Diagram).ZoomPercent = 200;


            // Specify the text pattern of series labels.
            series1.Label.TextPattern = "{A}:{V} ({VP:P0})";// "{A}: {VP:P0}";

            var seriesSrc1 = new Series(Resources.CAPTION_REPORT_RECORDCOUNT, ViewType.Bar);
            var seriesSrc2 = new Series(Resources.CAPTION_REPORT_DUPLICATES, ViewType.Bar);
            var seriesSrc3 = new Series(Resources.CAPTION_REPORT_MATCHING, ViewType.Bar);

            rd.SourceData.OrderByDescending(x => x.RecordsCount).ToList().ForEach(x =>
            {
                seriesSrc1.Points.Add(new SeriesPoint(x.Description, x.RecordsCount));
                seriesSrc2.Points.Add(new SeriesPoint(x.Description, x.Duplicates));
                seriesSrc3.Points.Add(new SeriesPoint(x.Description, x.MatchedRecords));
            });

            chrtSource.Series.Add(seriesSrc1);
            chrtSource.Series.Add(seriesSrc2);
            chrtSource.Series.Add(seriesSrc3);
        }

    }
}