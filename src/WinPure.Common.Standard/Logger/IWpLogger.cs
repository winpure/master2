namespace WinPure.Common.Logger;

internal interface IWpLogger
{
    string ReportPath { get; }
    void SetReportPath(string newPath);
    void Trace(string message, params object[] args);
    void Debug(string message, params object[] args);
    void Information(string message, params object[] args);
    void Warning(string message, params object[] args);
    void Error(string message, params object[] args);
    void Fatal(string message, params object[] args);
}