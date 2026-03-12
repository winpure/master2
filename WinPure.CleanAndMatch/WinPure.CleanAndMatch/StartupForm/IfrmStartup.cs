namespace WinPure.CleanAndMatch.StartupForm;

public interface IFrmStartup
{
    StartupOption StartOption { get; }
    void Show(IWin32Window owner);
    DialogResult ShowDialog();
}