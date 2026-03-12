namespace WinPure.CleanAndMatch.Controls
{
    partial class FavouriteButtonControl
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
            this.FavouriteSimpleButton = new DevExpress.XtraEditors.SimpleButton();
            this.FavouriteTitle = new DevExpress.XtraEditors.LabelControl();
            this.SuspendLayout();
            // 
            // FavouriteSimpleButton
            // 
            this.FavouriteSimpleButton.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.FavouriteSimpleButton.Appearance.Options.UseFont = true;
            this.FavouriteSimpleButton.AppearanceHovered.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FavouriteSimpleButton.AppearanceHovered.Options.UseFont = true;
            this.FavouriteSimpleButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.FavouriteSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.FavouriteSimpleButton.ImageOptions.SvgImageSize = new System.Drawing.Size(48, 48);
            this.FavouriteSimpleButton.Location = new System.Drawing.Point(3, 3);
            this.FavouriteSimpleButton.Name = "FavouriteSimpleButton";
            this.FavouriteSimpleButton.Size = new System.Drawing.Size(82, 59);
            this.FavouriteSimpleButton.TabIndex = 31;
            this.FavouriteSimpleButton.Tag = "";
            // 
            // FavouriteTitle
            // 
            this.FavouriteTitle.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FavouriteTitle.Appearance.Options.UseFont = true;
            this.FavouriteTitle.Appearance.Options.UseTextOptions = true;
            this.FavouriteTitle.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.FavouriteTitle.AppearanceHovered.ForeColor = System.Drawing.SystemColors.Highlight;
            this.FavouriteTitle.AppearanceHovered.Options.UseForeColor = true;
            this.FavouriteTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.FavouriteTitle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.FavouriteTitle.Location = new System.Drawing.Point(0, 63);
            this.FavouriteTitle.Name = "FavouriteTitle";
            this.FavouriteTitle.Size = new System.Drawing.Size(93, 18);
            this.FavouriteTitle.TabIndex = 32;
            this.FavouriteTitle.Text = "Favourite Title";
            // 
            // FavouriteButtonControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FavouriteTitle);
            this.Controls.Add(this.FavouriteSimpleButton);
            this.Name = "FavouriteButtonControl";
            this.Size = new System.Drawing.Size(93, 84);
            this.ResumeLayout(false);

        }

        #endregion

        private SimpleButton FavouriteSimpleButton;
        private LabelControl FavouriteTitle;
    }
}
