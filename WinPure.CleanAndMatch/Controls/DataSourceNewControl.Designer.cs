using DevExpress.LookAndFeel;

namespace WinPure.CleanAndMatch.Controls
{
    partial class DataSourceNewControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataSourceNewControl));
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.ExternalSourceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.SourceTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemImageComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.svgImageCollection1 = new DevExpress.Utils.SvgImageCollection(this.components);
            this.DisplayNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.IsFavoriteGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.favRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.GroupGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.popupContainerControl1 = new DevExpress.XtraEditors.PopupContainerControl();
            this.popupContainerEdit1 = new DevExpress.XtraEditors.PopupContainerEdit();
            this.panel1 = new System.Windows.Forms.Panel();
            this.FavoriteFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.favouriteButtonControl1 = new WinPure.CleanAndMatch.Controls.FavouriteButtonControl();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExternalSourceBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgImageCollection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.favRepositoryItemCheckEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupContainerControl1)).BeginInit();
            this.popupContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit1.Properties)).BeginInit();
            this.panel1.SuspendLayout();
            this.FavoriteFlowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.DataSource = this.ExternalSourceBindingSource;
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.favRepositoryItemCheckEdit,
            this.repositoryItemImageComboBox1});
            this.gridControl1.Size = new System.Drawing.Size(252, 351);
            this.gridControl1.TabIndex = 22;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Appearance.GroupRow.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridView1.Appearance.GroupRow.Options.UseFont = true;
            this.gridView1.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.gridView1.Appearance.Row.Options.UseFont = true;
            this.gridView1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.SourceTypeGridColumn,
            this.DisplayNameGridColumn,
            this.IsFavoriteGridColumn,
            this.GroupGridColumn});
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.GroupCount = 1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsBehavior.AllowGroupExpandAnimation = DevExpress.Utils.DefaultBoolean.True;
            this.gridView1.OptionsBehavior.AutoExpandAllGroups = true;
            this.gridView1.OptionsCustomization.AllowColumnMoving = false;
            this.gridView1.OptionsCustomization.AllowColumnResizing = false;
            this.gridView1.OptionsCustomization.AllowSort = false;
            this.gridView1.OptionsFind.AllowFindPanel = false;
            this.gridView1.OptionsFind.AllowMruItems = false;
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.gridView1.OptionsSelection.UseIndicatorForSelection = false;
            this.gridView1.OptionsView.ShowColumnHeaders = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.RowHeight = 40;
            this.gridView1.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.GroupGridColumn, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // SourceTypeGridColumn
            // 
            this.SourceTypeGridColumn.Caption = "Source Type";
            this.SourceTypeGridColumn.ColumnEdit = this.repositoryItemImageComboBox1;
            this.SourceTypeGridColumn.FieldName = "SourceType";
            this.SourceTypeGridColumn.Name = "SourceTypeGridColumn";
            this.SourceTypeGridColumn.OptionsColumn.AllowEdit = false;
            this.SourceTypeGridColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.SourceTypeGridColumn.OptionsColumn.ReadOnly = true;
            this.SourceTypeGridColumn.Visible = true;
            this.SourceTypeGridColumn.VisibleIndex = 0;
            this.SourceTypeGridColumn.Width = 305;
            // 
            // repositoryItemImageComboBox1
            // 
            this.repositoryItemImageComboBox1.AutoHeight = false;
            this.repositoryItemImageComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemImageComboBox1.GlyphAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.repositoryItemImageComboBox1.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 0, 0),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 1, 1),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 2, 2),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 3, 3),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 4, 4),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 5, 5),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 6, 6),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 7, 7),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 8, 18),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 9, 19),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 10, 8),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 11, 9),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 12, 10),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 13, 11),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 14, 12),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 15, 13),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 16, 18),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 17, 20),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 18, 18),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 19, 14),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 20, 15),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 21, 16),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 22, 17)});
            this.repositoryItemImageComboBox1.LargeImages = this.svgImageCollection1;
            this.repositoryItemImageComboBox1.Name = "repositoryItemImageComboBox1";
            // 
            // svgImageCollection1
            // 
            this.svgImageCollection1.ImageSize = new System.Drawing.Size(32, 32);
            this.svgImageCollection1.Add("_2026_TXT_64", "_2026_TXT_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_Microsoft_SQL_Server_64", "_2026_Microsoft_SQL_Server_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_MySQL_Logo_64", "_2026_MySQL_Logo_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_excel_32", "_2026_excel_32", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_Access_32", "_2026_Access_32", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_Oracle_Database_64", "_2026_Oracle_Database_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_SQLite_64", "_2026_SQLite_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_IBM_64", "_2026_IBM_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_JSON_64", "_2026_JSON_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_XML_64", "_2026_XML_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_Azure_64", "_2026_Azure_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_Mongo_Db_64", "_2026_Mongo_Db_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_PostgreSQL_64", "_2026_PostgreSQL_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_Dynamics_365_64", "_2026_Dynamics_365_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_Salesforce_64", "_2026_Salesforce_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_Snowflake_2_64", "_2026_Snowflake_2_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_Senzing_64", "_2026_Senzing_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_JSONL_64", "_2026_JSONL_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("_2026_File_64", "_2026_File_64", typeof(WinPure.CleanAndMatch.Properties.Resources));
            this.svgImageCollection1.Add("2026_Datatable_64", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImageCollection1.2026_Datatable_64"))));
            this.svgImageCollection1.Add("2026_zoho-logo_small", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImageCollection1.2026_zoho-logo_small"))));
            // 
            // DisplayNameGridColumn
            // 
            this.DisplayNameGridColumn.Caption = "Display Name";
            this.DisplayNameGridColumn.FieldName = "DisplayName";
            this.DisplayNameGridColumn.Name = "DisplayNameGridColumn";
            this.DisplayNameGridColumn.OptionsColumn.AllowEdit = false;
            this.DisplayNameGridColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.DisplayNameGridColumn.OptionsColumn.ReadOnly = true;
            this.DisplayNameGridColumn.Visible = true;
            this.DisplayNameGridColumn.VisibleIndex = 1;
            this.DisplayNameGridColumn.Width = 1131;
            // 
            // IsFavoriteGridColumn
            // 
            this.IsFavoriteGridColumn.AppearanceCell.Options.UseTextOptions = true;
            this.IsFavoriteGridColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.IsFavoriteGridColumn.Caption = "Is Favorite";
            this.IsFavoriteGridColumn.ColumnEdit = this.favRepositoryItemCheckEdit;
            this.IsFavoriteGridColumn.FieldName = "IsFavorite";
            this.IsFavoriteGridColumn.Name = "IsFavoriteGridColumn";
            this.IsFavoriteGridColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.IsFavoriteGridColumn.Visible = true;
            this.IsFavoriteGridColumn.VisibleIndex = 2;
            this.IsFavoriteGridColumn.Width = 202;
            // 
            // favRepositoryItemCheckEdit
            // 
            this.favRepositoryItemCheckEdit.AutoHeight = false;
            this.favRepositoryItemCheckEdit.Caption = "";
            this.favRepositoryItemCheckEdit.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.SvgStar2;
            this.favRepositoryItemCheckEdit.CheckBoxOptions.SvgImageSize = new System.Drawing.Size(20, 20);
            this.favRepositoryItemCheckEdit.GlyphAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.favRepositoryItemCheckEdit.Name = "favRepositoryItemCheckEdit";
            this.favRepositoryItemCheckEdit.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.favRepositoryItemCheckEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.FavRepositoryItemCheckEdit_EditValueChanging);
            // 
            // GroupGridColumn
            // 
            this.GroupGridColumn.Caption = "Group";
            this.GroupGridColumn.FieldName = "SourceGroup";
            this.GroupGridColumn.Name = "GroupGridColumn";
            this.GroupGridColumn.Visible = true;
            this.GroupGridColumn.VisibleIndex = 3;
            // 
            // popupContainerControl1
            // 
            this.popupContainerControl1.Controls.Add(this.gridControl1);
            this.popupContainerControl1.Location = new System.Drawing.Point(1027, 3);
            this.popupContainerControl1.Name = "popupContainerControl1";
            this.popupContainerControl1.Size = new System.Drawing.Size(252, 351);
            this.popupContainerControl1.TabIndex = 23;
            // 
            // popupContainerEdit1
            // 
            this.popupContainerEdit1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.popupContainerEdit1.Location = new System.Drawing.Point(6, 18);
            this.popupContainerEdit1.Name = "popupContainerEdit1";
            this.popupContainerEdit1.Properties.Appearance.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.popupContainerEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.popupContainerEdit1.Properties.AutoHeight = false;
            editorButtonImageOptions1.Image = global::WinPure.CleanAndMatch.Properties.Resources._2019_Plus;
            this.popupContainerEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.popupContainerEdit1.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.popupContainerEdit1.Properties.PopupControl = this.popupContainerControl1;
            this.popupContainerEdit1.Properties.PopupSizeable = false;
            this.popupContainerEdit1.Properties.ShowPopupCloseButton = false;
            this.popupContainerEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.popupContainerEdit1.Size = new System.Drawing.Size(71, 49);
            this.popupContainerEdit1.TabIndex = 24;
            this.popupContainerEdit1.ToolTip = "Click to view source types";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.popupContainerEdit1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(135, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(88, 120);
            this.panel1.TabIndex = 25;
            // 
            // FavoriteFlowLayoutPanel
            // 
            this.FavoriteFlowLayoutPanel.AutoScroll = true;
            this.FavoriteFlowLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.FavoriteFlowLayoutPanel.Controls.Add(this.favouriteButtonControl1);
            this.FavoriteFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.FavoriteFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.FavoriteFlowLayoutPanel.Name = "FavoriteFlowLayoutPanel";
            this.FavoriteFlowLayoutPanel.Padding = new System.Windows.Forms.Padding(22, 5, 22, 0);
            this.FavoriteFlowLayoutPanel.Size = new System.Drawing.Size(135, 120);
            this.FavoriteFlowLayoutPanel.TabIndex = 0;
            // 
            // favouriteButtonControl1
            // 
            this.favouriteButtonControl1.Appearance.BackColor = System.Drawing.Color.White;
            this.favouriteButtonControl1.Appearance.Options.UseBackColor = true;
            this.favouriteButtonControl1.Location = new System.Drawing.Point(25, 8);
            this.favouriteButtonControl1.Name = "favouriteButtonControl1";
            this.favouriteButtonControl1.Size = new System.Drawing.Size(96, 77);
            this.favouriteButtonControl1.TabIndex = 26;
            this.favouriteButtonControl1.Text = "favouriteButtonControl1";
            // 
            // DataSourceNewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.FavoriteFlowLayoutPanel);
            this.Controls.Add(this.popupContainerControl1);
            this.Name = "DataSourceNewControl";
            this.Size = new System.Drawing.Size(1291, 120);
            this.Load += new System.EventHandler(this.DataSourceNewControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExternalSourceBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgImageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.favRepositoryItemCheckEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupContainerControl1)).EndInit();
            this.popupContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit1.Properties)).EndInit();
            this.panel1.ResumeLayout(false);
            this.FavoriteFlowLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private GridControl gridControl1;
        private GridView gridView1;
        private GridColumn SourceTypeGridColumn;
        private GridColumn DisplayNameGridColumn;
        private GridColumn IsFavoriteGridColumn;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit favRepositoryItemCheckEdit;
        private SvgImageCollection svgImageCollection1;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repositoryItemImageComboBox1;
        private BindingSource ExternalSourceBindingSource;
        private PopupContainerControl popupContainerControl1;
        private PopupContainerEdit popupContainerEdit1;
        private Panel panel1;
        private FlowLayoutPanel FavoriteFlowLayoutPanel;
        private FavouriteButtonControl favouriteButtonControl1;
        private GridColumn GroupGridColumn;
    }
}
