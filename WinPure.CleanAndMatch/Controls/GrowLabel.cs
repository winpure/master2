namespace WinPure.CleanAndMatch.Controls;

public class GrowLabel : DevExpress.XtraEditors.LabelControl
{
    private bool _isGrowing;

    public GrowLabel()
    {
        AutoSize = false;
    }

    private void resizeLabel()
    {
        if (_isGrowing)
        {
            return;
        }
        try
        {
            _isGrowing = true;
            Size sz = new Size(Width, Int32.MaxValue);
            sz = TextRenderer.MeasureText(Text, Font, sz, TextFormatFlags.WordBreak);
            Height = sz.Height;
        }
        finally
        {
            _isGrowing = false;
        }
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        resizeLabel();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        resizeLabel();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        resizeLabel();
    }

    private void InitializeComponent()
    {
            this.SuspendLayout();
            // 
            // GrowLabel
            // 
            this.Size = new System.Drawing.Size(0, 15);
            this.ResumeLayout(false);

    }
}