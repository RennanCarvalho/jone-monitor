namespace jone_fora
{
    partial class JoneFora
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataLabel = new Label();
            SaidaLabel = new Label();
            CronometroLabel = new Label();
            EventoBt = new Button();
            Cronometro = new System.Windows.Forms.Timer(components);
            SaidaPorDiaLabel = new Label();
            TempoPorSaidaLabel = new Label();
            SuspendLayout();
            // 
            // DataLabel
            // 
            DataLabel.Font = new Font("Courier New", 10F);
            DataLabel.Location = new Point(12, 9);
            DataLabel.Name = "DataLabel";
            DataLabel.Size = new Size(176, 22);
            DataLabel.TabIndex = 0;
            DataLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // SaidaLabel
            // 
            SaidaLabel.Font = new Font("Courier New", 10F);
            SaidaLabel.Location = new Point(12, 36);
            SaidaLabel.Name = "SaidaLabel";
            SaidaLabel.Size = new Size(176, 22);
            SaidaLabel.TabIndex = 1;
            SaidaLabel.Text = "Saídas      : ";
            // 
            // CronometroLabel
            // 
            CronometroLabel.Font = new Font("Courier New", 20F);
            CronometroLabel.Location = new Point(12, 102);
            CronometroLabel.Name = "CronometroLabel";
            CronometroLabel.Size = new Size(176, 51);
            CronometroLabel.TabIndex = 2;
            CronometroLabel.Text = "00:00:00";
            CronometroLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // EventoBt
            // 
            EventoBt.FlatStyle = FlatStyle.Popup;
            EventoBt.Font = new Font("Courier New", 10F);
            EventoBt.Location = new Point(12, 156);
            EventoBt.Name = "EventoBt";
            EventoBt.Size = new Size(176, 32);
            EventoBt.TabIndex = 4;
            EventoBt.Text = "Saiu";
            EventoBt.UseVisualStyleBackColor = true;
            EventoBt.Click += EventoBt_Click;
            // 
            // Cronometro
            // 
            Cronometro.Interval = 1000;
            Cronometro.Tick += Cronometro_Tick;
            // 
            // SaidaPorDiaLabel
            // 
            SaidaPorDiaLabel.Font = new Font("Courier New", 10F);
            SaidaPorDiaLabel.Location = new Point(12, 58);
            SaidaPorDiaLabel.Name = "SaidaPorDiaLabel";
            SaidaPorDiaLabel.Size = new Size(176, 22);
            SaidaPorDiaLabel.TabIndex = 0;
            SaidaPorDiaLabel.Text = "Saída/Dia   : ";
            // 
            // TempoPorSaidaLabel
            // 
            TempoPorSaidaLabel.Font = new Font("Courier New", 10F);
            TempoPorSaidaLabel.Location = new Point(12, 80);
            TempoPorSaidaLabel.Name = "TempoPorSaidaLabel";
            TempoPorSaidaLabel.Size = new Size(176, 22);
            TempoPorSaidaLabel.TabIndex = 1;
            TempoPorSaidaLabel.Text = "Tempo/Saída : ";
            // 
            // JoneFora
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.MenuBar;
            ClientSize = new Size(200, 200);
            Controls.Add(EventoBt);
            Controls.Add(CronometroLabel);
            Controls.Add(TempoPorSaidaLabel);
            Controls.Add(SaidaLabel);
            Controls.Add(SaidaPorDiaLabel);
            Controls.Add(DataLabel);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            Name = "JoneFora";
            Opacity = 0.7D;
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "jone";
            TopMost = true;
            FormClosing += JoneFora_FormClosing;
            Load += JoneFora_Load;
            Shown += JoneFora_Shown;
            DoubleClick += JoneFora_DoubleClick;
            ResumeLayout(false);
        }

        #endregion

        private Label DataLabel;
        private Label SaidaLabel;
        private Label CronometroLabel;
        private Button EventoBt;
        private System.Windows.Forms.Timer Cronometro;
        private Label SaidaPorDiaLabel;
        private Label TempoPorSaidaLabel;
    }
}
