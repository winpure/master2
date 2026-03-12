using DevExpress.Utils.Svg;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.ComponentModel;
using WinPure.Integration.Enums;

namespace WinPure.CleanAndMatch.Controls
{
    public partial class DataSourceNewControl : XtraUserControl
    {

        [DefaultValue(OperationType.Import)]
        public OperationType DataSourceOperationType
        {
            get => _dataSourceOperationType;
            set => _dataSourceOperationType = value;
        }

        public event Action<OperationType, ExternalSourceTypes> OnDataSourceClick;
        public event Action<string, string, MessagesType, Exception> OnException;


        private readonly IExternalSourceService _externalSourceService;

        private int _lastFavouriteCount;
        private OperationType _dataSourceOperationType = OperationType.Import;

        public DataSourceNewControl()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                _externalSourceService = (IExternalSourceService)WinPureUiDependencyResolver.Instance
                  .ServiceProvider
                  .GetService(typeof(IExternalSourceService));
            }

            gridView1.CustomColumnDisplayText += GridView1_CustomColumnDisplayText;
            gridView1.CustomDrawGroupRow += GridView1_CustomDrawGroupRow;
        }

        private void GridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column != GroupGridColumn)
            {
                return;
            }

            if (e.Value == null || e.Value == DBNull.Value)
            {
                return;
            }

            if (!int.TryParse(e.Value.ToString(), out var groupValue))
            {
                return;
            }

            e.DisplayText = ((ExternalSourceGroup)groupValue).ToString();
        }

        private void GridView1_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            if (sender is not GridView view)
            {
                return;
            }

            if (e.Info is not GridGroupRowInfo groupInfo)
            {
                return;
            }

            if (groupInfo.Column != GroupGridColumn)
            {
                return;
            }

            var groupValue = view.GetGroupRowValue(e.RowHandle);
            if (groupValue == null || groupValue == DBNull.Value)
            {
                return;
            }

            if (!int.TryParse(groupValue.ToString(), out var groupInt))
            {
                return;
            }

            groupInfo.GroupText = ((ExternalSourceGroup)groupInt).ToString();
        }

        private void DataSourceNewControl_Load(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            if (_externalSourceService == null)
            {
                return;
            }

            InitializeFavouriteButtonsArea();

            FetchSourceData();
            BuildFavouriteButtons();
        }

        private void InitializeFavouriteButtonsArea()
        {
            FavoriteFlowLayoutPanel.SuspendLayout();
            try
            {
                FavoriteFlowLayoutPanel.Controls.Clear();
            }
            finally
            {
                FavoriteFlowLayoutPanel.ResumeLayout(true);
            }
        }

        private void FavRepositoryItemCheckEdit_EditValueChanging(object sender, ChangingEventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0)
            {
                return;
            }

            if (!TryGetRowSourceType(gridView1.FocusedRowHandle, out var sourceType))
            {
                return;
            }

            var isFavorite = e.NewValue is bool b && b;

            try
            {
                if (isFavorite)
                {
                    _externalSourceService.AddFavorite(sourceType);
                }
                else
                {
                    _externalSourceService.RemoveFavorite(sourceType);
                }

                FetchSourceData();
                BuildFavouriteButtons();
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                OnException?.Invoke(ex.Message, "Favorite", MessagesType.Warning, null);
            }
        }

        private bool TryGetRowSourceType(int rowHandle, out ExternalSourceTypes sourceType)
        {
            sourceType = default;

            var value = gridView1.GetRowCellValue(rowHandle, SourceTypeGridColumn);
            if (value == null)
            {
                return false;
            }

            if (value is int i)
            {
                sourceType = (ExternalSourceTypes)i;
                return true;
            }

            if (int.TryParse(value.ToString(), out var parsed))
            {
                sourceType = (ExternalSourceTypes)parsed;
                return true;
            }

            return false;
        }

        private void FetchSourceData()
        {
            if (_externalSourceService == null)
            {
                return;
            }

            var sources = _externalSourceService.GetSources();

            sources = DataSourceOperationType switch
            {
                OperationType.Import => [.. sources.Where(s => s.CanImport)],
                OperationType.Export => [.. sources.Where(s => s.CanExport)],
                _ => sources,
            };

            var items = sources
                .Select(c => new DataSourceItem
                {
                    SourceType = (int)c.Source,
                    DisplayName = GetDisplayText(c.Source),
                    IsFavorite = c.Favorite,
                    SourceGroup = (int)c.Group,
                }).OrderBy(i => i.SourceGroup).ToList();

            ExternalSourceBindingSource.DataSource = items;
            ExternalSourceBindingSource.ResetBindings(false);
        }

        private void BuildFavouriteButtons()
        {
            if (_externalSourceService == null)
            {
                return;
            }

            var favorites = _externalSourceService.GetSources()
                .Where(s => s.Favorite)
                .Select(s => s.Source)
                .ToList();

            FavoriteFlowLayoutPanel.SuspendLayout();
            try
            {
                FavoriteFlowLayoutPanel.Controls.Clear();

                if (favorites.Count == 0)
                {
                    var placeholder = new FavouriteButtonControl
                    {
                        Margin = new Padding(3),
                    };
                    placeholder.SetPlaceholder(Resources.UI_ADD_FAVOURITES);
                    placeholder.PlaceholderClicked += PlaceholderButton_Clicked;
                    FavoriteFlowLayoutPanel.Controls.Add(placeholder);

                    UpdateFavouritePanelWidth(visibleItemCount: 1);
                }
                else
                {
                    foreach (var sourceType in favorites)
                    {
                        var fav = new FavouriteButtonControl
                        {
                            Name = $"fav_{sourceType}",
                            Margin = new Padding(3),
                        };

                        fav.SetData(sourceType, GetDisplayText(sourceType), GetIcon(sourceType));
                        fav.SourceClicked += FavouriteButton_SourceClicked;

                        FavoriteFlowLayoutPanel.Controls.Add(fav);
                    }

                    UpdateFavouritePanelWidth(visibleItemCount: favorites.Count);
                }
            }
            finally
            {
                FavoriteFlowLayoutPanel.ResumeLayout(true);
            }

            _lastFavouriteCount = favorites.Count;
        }

        private void UpdateFavouritePanelWidth(int visibleItemCount)
        {
            var count = Math.Max(1, visibleItemCount);
            
            // Calculate content width: each button is 96 pixels wide with 3px margin on each side (6px total)
            // Plus left and right padding of 22px each
            const int buttonWidth = 96;
            const int buttonMarginTotal = 6; // 3px margin left + 3px margin right per button
            const int panelHorizontalPadding = 44; // 22px left + 22px right padding
            
            var contentWidth = (buttonWidth + buttonMarginTotal) * count;
            FavoriteFlowLayoutPanel.Width = panelHorizontalPadding + contentWidth;

            FavoriteFlowLayoutPanel.PerformLayout();
            FavoriteFlowLayoutPanel.Invalidate(true);
        }

        private void FavouriteButton_SourceClicked(object sender, FavouriteButtonClickedEventArgs e)
        {
            OnDataSourceClick?.Invoke(DataSourceOperationType, e.SourceType);
        }

        private void PlaceholderButton_Clicked(object sender, EventArgs e)
        {
            popupContainerEdit1.ShowPopup();
        }

        private static string GetDisplayText(ExternalSourceTypes sourceType)
        {
            return sourceType switch
            {
                ExternalSourceTypes.Excel => "MS EXCEL",
                ExternalSourceTypes.TextFile => "TEXT/CSV",
                ExternalSourceTypes.SqlServer => "SQL SERVER",
                ExternalSourceTypes.MySqlServer => "MySQL",
                ExternalSourceTypes.Access => "MS ACCESS",
                ExternalSourceTypes.AzureDb => "MS Azure",
                ExternalSourceTypes.Postgres => "PostgreSQL",
                ExternalSourceTypes.SqLite => "SQLite",
                ExternalSourceTypes.JSONL => "JSONL",
                ExternalSourceTypes.Oracle => "Oracle",
                ExternalSourceTypes.Json => "JSON",
                ExternalSourceTypes.Xml => "XML",
                ExternalSourceTypes.Db2 => "DB2",
                ExternalSourceTypes.Salesforce => "Salesforce",
                ExternalSourceTypes.Senzing => "Senzing",
                ExternalSourceTypes.Snowflake => "Snowflake",
                ExternalSourceTypes.Odbc => "ODBC",
                ExternalSourceTypes.DataTable => "DataTable",
                ExternalSourceTypes.MongoDb => "MongoDB",
                ExternalSourceTypes.MsDynamics => "MS Dynamics",
                ExternalSourceTypes.Hadoop => "Hadoop",
                ExternalSourceTypes.ZohoCrm => "Zoho CRM",
                ExternalSourceTypes.SugarCrm => "Sugar CRM",
                _ => sourceType.ToString(),
            };
        }

        private static SvgImage GetIcon(ExternalSourceTypes sourceType)
        {
            return sourceType switch
            {
                ExternalSourceTypes.Excel => Resources._2026_excel_32,
                ExternalSourceTypes.TextFile => Resources._2026_TXT_64,
                ExternalSourceTypes.SqlServer => Resources._2026_Microsoft_SQL_Server_64,
                ExternalSourceTypes.MySqlServer => Resources._2026_MySQL_Logo_64,
                ExternalSourceTypes.Access => Resources._2026_Access_32,
                ExternalSourceTypes.Oracle => Resources._2026_Oracle_Database_64,
                ExternalSourceTypes.SqLite => Resources._2026_SQLite_64,
                ExternalSourceTypes.Json => Resources._2026_JSON_64,
                ExternalSourceTypes.Xml => Resources._2026_XML_64,
                ExternalSourceTypes.AzureDb => Resources._2026_Azure_64,
                ExternalSourceTypes.Postgres => Resources._2026_PostgreSQL_64,
                ExternalSourceTypes.Db2 => Resources._2026_IBM_64,
                ExternalSourceTypes.Salesforce => Resources._2026_Salesforce_64,
                ExternalSourceTypes.Senzing => Resources._2026_Senzing_64,
                ExternalSourceTypes.Snowflake => Resources._2026_Snowflake_2_64,
                ExternalSourceTypes.JSONL => Resources._2026_JSONL_64,
                ExternalSourceTypes.ZohoCrm => Resources._2026_zoho_logo_48,
                _ => Resources._2026_File_64,
            };
        }

        public class DataSourceItem
        {
            public int SourceType { get; set; }
            public string DisplayName { get; set; }
            public bool IsFavorite { get; set; }
            public int SourceGroup { get; set; }
        }
    }
}
