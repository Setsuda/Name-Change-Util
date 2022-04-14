namespace 간단한_이름_바꾸기
{
    partial class BeingProcessed
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
            this.lblCurrentFileCount = new System.Windows.Forms.Label();
            this.pcbBeingProcessed = new System.Windows.Forms.ProgressBar();
            this.lblCurrentFileName = new System.Windows.Forms.Label();
            this.btnRestoration = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCurrentFileCount
            // 
            this.lblCurrentFileCount.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCurrentFileCount.Location = new System.Drawing.Point(14, 36);
            this.lblCurrentFileCount.Name = "lblCurrentFileCount";
            this.lblCurrentFileCount.Size = new System.Drawing.Size(830, 12);
            this.lblCurrentFileCount.TabIndex = 2;
            this.lblCurrentFileCount.Text = "965535/965535";
            this.lblCurrentFileCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pcbBeingProcessed
            // 
            this.pcbBeingProcessed.Location = new System.Drawing.Point(12, 51);
            this.pcbBeingProcessed.Name = "pcbBeingProcessed";
            this.pcbBeingProcessed.Size = new System.Drawing.Size(832, 23);
            this.pcbBeingProcessed.TabIndex = 3;
            // 
            // lblCurrentFileName
            // 
            this.lblCurrentFileName.AutoSize = true;
            this.lblCurrentFileName.Location = new System.Drawing.Point(12, 9);
            this.lblCurrentFileName.Name = "lblCurrentFileName";
            this.lblCurrentFileName.Size = new System.Drawing.Size(100, 12);
            this.lblCurrentFileName.TabIndex = 1;
            this.lblCurrentFileName.Text = "CurrentFielName";
            // 
            // btnRestoration
            // 
            this.btnRestoration.Location = new System.Drawing.Point(311, 85);
            this.btnRestoration.Name = "btnRestoration";
            this.btnRestoration.Size = new System.Drawing.Size(93, 23);
            this.btnRestoration.TabIndex = 6;
            this.btnRestoration.Text = "복원";
            this.btnRestoration.UseVisualStyleBackColor = true;
            this.btnRestoration.Click += new System.EventHandler(this.btnRestoration_Click);
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(425, 85);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(93, 23);
            this.BtnOK.TabIndex = 7;
            this.BtnOK.Text = "완료";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BeingProcessed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 120);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.btnRestoration);
            this.Controls.Add(this.lblCurrentFileName);
            this.Controls.Add(this.pcbBeingProcessed);
            this.Controls.Add(this.lblCurrentFileCount);
            this.Name = "BeingProcessed";
            this.Text = "BeingProcessed";
            this.Load += new System.EventHandler(this.BeingProcessed_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCurrentFileCount;
        private System.Windows.Forms.ProgressBar pcbBeingProcessed;
        private System.Windows.Forms.Label lblCurrentFileName;
        private System.Windows.Forms.Button btnRestoration;
        private System.Windows.Forms.Button BtnOK;
    }
}