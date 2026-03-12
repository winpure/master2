using System.Globalization;

namespace WinPure.Integration.Helpers;

public static class FormatHelper
{
    public static string GetDateStringFormat(TextImportExportOptions opt)
    {
        string res;
        if (opt.DateOrder.Length != 3)
        {
            res = CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern + ((opt.AddTime) ? CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern : "");
            res = res.Replace(opt.FieldDelimiter, opt.DateDelimiter);
        }
        else
        {
            res = (opt.DateOrder[0].ToString() + opt.DateDelimiter + opt.DateOrder[1] + opt.DateDelimiter + opt.DateOrder[2]).Replace("D", "dd").Replace("Y", "yyyy") + " " + ((opt.AddTime) ? "hh" + opt.TimeDelimiter + "mm" : "");
        }
        return res.Trim();
    }
}