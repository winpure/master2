namespace WinPure.CleanAndMatch.Support;

public partial class LoadingMaskForm : DevExpress.XtraEditors.XtraForm
{
    private bool _isPbShowed;
    private string _displayText = "";
    private CancellationTokenSource _token;

    public LoadingMaskForm()
    {
        InitializeComponent();
        Localization();
    }

    private void Localization()
    {
        btnCancel.Text = Resources.UI_CANCEL.ToUpper();
        Text = Resources.UI_LOADINGMASKFORM_LOADINGMASK;
    }

    private Task _thread;

    public void ShowLoadingMask(string loadingText, Task thread, bool showProgressBar, CancellationTokenSource cancelToken = null)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { ShowLoadingMask(loadingText, thread, showProgressBar, cancelToken); }));
            return;
        }
        MarqueeBar.Text = loadingText;
        MarqueeBar.Properties.Stopped = false;
        btnCancel.Visible = cancelToken != null;
        _token = cancelToken;
        _isPbShowed = showProgressBar;
        if (_isPbShowed)
        {
            progressBar.Visible = true;
            _displayText = loadingText;
            progressBar.Position = 0;
            MarqueeBar.Visible = false;
        }
        else
        {
            progressBar.Visible = false;
            MarqueeBar.Visible = true;
        }
        _thread = thread;

        ShowDialog();
    }

    public void UpdateText(string text, int value = 0)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { UpdateText(text, value); }));
            return;
        }
        try
        {
            if (_isPbShowed)
            {
                _displayText = text;
                if (progressBar.Position != value)
                {
                    progressBar.Position = value;
                }
                else
                {
                    progressBar.Text = _displayText;
                    progressBar.Position = value;
                }
            }
            else
            {
                MarqueeBar.Text = text;
            }
        }
        catch
        {
            CloseFormAndStopTimer();
        }
    }

    private void timerForClose_Tick(object sender, EventArgs e)
    {
        try
        {
            if (_thread?.Exception != null)
            {
                WinPureUiDependencyResolver.Resolve<IWpLogger>().Error("THREAD ERROR!", new[] { (object)_thread.Exception, _thread.Exception.StackTrace });
            }
        }
        catch (Exception ex)
        {
            WinPureUiDependencyResolver.Resolve<IWpLogger>().Warning("THREAD ERROR! Cannot get thread error", ex);
        }

        try
        {
            if (_thread.Status != TaskStatus.Running)
            {
                CloseFormAndStopTimer();
            }
        }
        catch (Exception)
        {
            CloseFormAndStopTimer();
        }

    }

    public void CloseFormAndStopTimer()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(CloseFormAndStopTimer));
            return;
        }
        timerForClose.Stop();
        Close();
    }

    private void progressBar_CustomDisplayText(object sender,
        DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
    {
        try
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate { e.DisplayText = _displayText; }));
                return;
            }
            e.DisplayText = _displayText;
        }
        catch
        {
            CloseFormAndStopTimer();
        }
    }

    private void LoadingMaskForm_Shown(object sender, EventArgs e)
    {
        _thread.Start();
        timerForClose.Start();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(Resources.MESSAGE_CANCEL_OPERATION_CONFIRMATION, Resources.MESSAGE_CONFIRMATION_CAPTION,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {

            _token.Cancel();
        }
    }
}