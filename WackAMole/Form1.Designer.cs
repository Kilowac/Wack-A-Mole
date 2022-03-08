namespace WackAMole
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.BtnSetGrid = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.lblMisses = new System.Windows.Forms.Label();
            this.BtnStopGame = new System.Windows.Forms.Button();
            this.BtnPauseResume = new System.Windows.Forms.Button();
            this.lblStoppingState = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BtnSetGrid
            // 
            this.BtnSetGrid.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtnSetGrid.Location = new System.Drawing.Point(230, 26);
            this.BtnSetGrid.Name = "BtnSetGrid";
            this.BtnSetGrid.Size = new System.Drawing.Size(110, 66);
            this.BtnSetGrid.TabIndex = 0;
            this.BtnSetGrid.Text = "Start";
            this.BtnSetGrid.UseVisualStyleBackColor = true;
            this.BtnSetGrid.Click += new System.EventHandler(this.BtnSetGrid_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(499, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Score:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(499, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Misses:";
            // 
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScore.Location = new System.Drawing.Point(552, 34);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(18, 20);
            this.lblScore.TabIndex = 3;
            this.lblScore.Text = "0";
            // 
            // lblMisses
            // 
            this.lblMisses.AutoSize = true;
            this.lblMisses.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMisses.Location = new System.Drawing.Point(552, 61);
            this.lblMisses.Name = "lblMisses";
            this.lblMisses.Size = new System.Drawing.Size(18, 20);
            this.lblMisses.TabIndex = 4;
            this.lblMisses.Text = "0";
            // 
            // BtnStopGame
            // 
            this.BtnStopGame.Enabled = false;
            this.BtnStopGame.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtnStopGame.Location = new System.Drawing.Point(346, 25);
            this.BtnStopGame.Name = "BtnStopGame";
            this.BtnStopGame.Size = new System.Drawing.Size(110, 66);
            this.BtnStopGame.TabIndex = 5;
            this.BtnStopGame.Text = "Stop";
            this.BtnStopGame.UseVisualStyleBackColor = true;
            this.BtnStopGame.Click += new System.EventHandler(this.BtnStopGame_Click);
            // 
            // BtnPauseResume
            // 
            this.BtnPauseResume.Enabled = false;
            this.BtnPauseResume.Location = new System.Drawing.Point(100, 48);
            this.BtnPauseResume.Name = "BtnPauseResume";
            this.BtnPauseResume.Size = new System.Drawing.Size(57, 23);
            this.BtnPauseResume.TabIndex = 6;
            this.BtnPauseResume.Text = "Pause";
            this.BtnPauseResume.UseVisualStyleBackColor = true;
            this.BtnPauseResume.Click += new System.EventHandler(this.BtnPauseResume_Click);
            // 
            // lblStoppingState
            // 
            this.lblStoppingState.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStoppingState.Location = new System.Drawing.Point(173, 622);
            this.lblStoppingState.Name = "lblStoppingState";
            this.lblStoppingState.Size = new System.Drawing.Size(340, 20);
            this.lblStoppingState.TabIndex = 7;
            this.lblStoppingState.Text = "Text";
            this.lblStoppingState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStoppingState.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 663);
            this.Controls.Add(this.lblStoppingState);
            this.Controls.Add(this.BtnPauseResume);
            this.Controls.Add(this.BtnStopGame);
            this.Controls.Add(this.lblMisses);
            this.Controls.Add(this.lblScore);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnSetGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(702, 702);
            this.MinimumSize = new System.Drawing.Size(702, 702);
            this.Name = "Form1";
            this.Text = "Multi-Threaded Whac-A-Mole";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnSetGrid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Label lblMisses;
        private System.Windows.Forms.Button BtnStopGame;
        private System.Windows.Forms.Button BtnPauseResume;
        private System.Windows.Forms.Label lblStoppingState;
    }
}

