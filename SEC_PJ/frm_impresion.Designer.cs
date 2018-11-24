namespace SEC_PJ
{
    partial class frm_impresion
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rpt_afiliacion1 = new SEC_PJ.rpt_afiliacion();
            this.CRV = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.rpt_afi = new SEC_PJ.rpt_afi();
            this.SuspendLayout();
            // 
            // CRV
            // 
            this.CRV.ActiveViewIndex = 0;
            this.CRV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CRV.Cursor = System.Windows.Forms.Cursors.Default;
            this.CRV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CRV.Location = new System.Drawing.Point(0, 0);
            this.CRV.Name = "CRV";
            this.CRV.ReportSource = this.rpt_afi;
            this.CRV.ShowLogo = false;
            this.CRV.Size = new System.Drawing.Size(952, 552);
            this.CRV.TabIndex = 0;
            this.CRV.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // frm_impresion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(952, 552);
            this.Controls.Add(this.CRV);
            this.Name = "frm_impresion";
            this.Text = "frm_impresion";
            this.Load += new System.EventHandler(this.frm_impresion_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer CRV;
        private rpt_afiliacion rpt_afiliacion1;
        private rpt_afi rpt_afi;
    }
}