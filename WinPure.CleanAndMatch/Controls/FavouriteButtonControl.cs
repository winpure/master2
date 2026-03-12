using DevExpress.Utils.Svg;
using System.ComponentModel;

namespace WinPure.CleanAndMatch.Controls
{
    public partial class FavouriteButtonControl : XtraUserControl
    {
        private ExternalSourceTypes? _sourceType;
        private bool _isPlaceholder;

        public FavouriteButtonControl()
        {
            InitializeComponent();

            FavouriteSimpleButton.Click += OnAnyClick;
            FavouriteTitle.Click += OnAnyClick;

            FavouriteSimpleButton.MouseEnter += OnMouseEnter;
            FavouriteSimpleButton.MouseLeave += OnMouseLeave;
            FavouriteTitle.MouseEnter += OnMouseEnter;
            FavouriteTitle.MouseLeave += OnMouseLeave;
        }

        public event EventHandler<FavouriteButtonClickedEventArgs> SourceClicked;
        public event EventHandler PlaceholderClicked;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ExternalSourceTypes? SourceType
        {
            get => _sourceType;
            set
            {
                _sourceType = value;
                FavouriteSimpleButton.Tag = value;
                FavouriteTitle.Tag = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvgImage Icon
        {
            get => FavouriteSimpleButton.ImageOptions.SvgImage;
            set => FavouriteSimpleButton.ImageOptions.SvgImage = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => FavouriteTitle.Text;
            set => FavouriteTitle.Text = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPlaceholder
        {
            get => _isPlaceholder;
            set
            {
                _isPlaceholder = value;
                UpdatePlaceholderState();
            }
        }

        public void SetData(ExternalSourceTypes sourceType, string displayText, SvgImage icon)
        {
            SourceType = sourceType;
            Text = displayText;
            Icon = icon;
            IsPlaceholder = false;
        }

        public void SetPlaceholder(string message)
        {
            SourceType = null;
            Icon = null;
            Text = message;
            IsPlaceholder = true;
        }

        private void UpdatePlaceholderState()
        {
            var fontStyle = _isPlaceholder ? FontStyle.Italic : FontStyle.Regular;
            FavouriteTitle.Appearance.Font = new Font("Segoe UI Semibold", 9.75f, fontStyle);
            FavouriteTitle.Appearance.Options.UseFont = true;

            FavouriteSimpleButton.Enabled = true;

            var cursor = _isPlaceholder ? Cursors.Hand : Cursors.Hand;
            FavouriteSimpleButton.Cursor = cursor;
            FavouriteTitle.Cursor = cursor;
        }

        private void OnAnyClick(object sender, EventArgs e)
        {
            if (_isPlaceholder)
            {
                PlaceholderClicked?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (_sourceType == null)
            {
                return;
            }

            SourceClicked?.Invoke(this, new FavouriteButtonClickedEventArgs(_sourceType.Value));
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            if (_isPlaceholder)
            {
                ApplyPlaceholderHover(isHovered: true);
                return;
            }

            ApplyHover(isHovered: true);
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (_isPlaceholder)
            {
                ApplyPlaceholderHover(isHovered: false);
                return;
            }

            ApplyHover(isHovered: false);
        }

        private void ApplyPlaceholderHover(bool isHovered)
        {
            FavouriteTitle.Appearance.Font = new Font(
                "Segoe UI Semibold",
                9.75f,
                FontStyle.Italic);
            FavouriteTitle.Appearance.Options.UseFont = true;
        }

        private void ApplyHover(bool isHovered)
        {
            if (_isPlaceholder)
            {
                return;
            }

            FavouriteTitle.Appearance.Font = new Font(
                "Segoe UI Semibold",
                9.75f,
                isHovered ? FontStyle.Bold : FontStyle.Regular);
            FavouriteTitle.Appearance.Options.UseFont = true;

            FavouriteSimpleButton.Appearance.Options.UseBorderColor = isHovered;
            FavouriteSimpleButton.Appearance.BorderColor = isHovered ? Color.Gainsboro : Color.Transparent;
        }
    }

    public sealed class FavouriteButtonClickedEventArgs : EventArgs
    {
        public FavouriteButtonClickedEventArgs(ExternalSourceTypes sourceType)
        {
            SourceType = sourceType;
        }

        public ExternalSourceTypes SourceType { get; }
    }
}
