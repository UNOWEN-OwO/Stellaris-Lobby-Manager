namespace Stellaris_Lobby_Manager
{
    partial class OffsetSetter
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
            offsetPanel = new TableLayoutPanel();
            lobbyOffset1 = new NumericUpDown();
            lobbyOffset2 = new NumericUpDown();
            sizeOffset1 = new NumericUpDown();
            sizeOffset2 = new NumericUpDown();
            sizeCountOffset = new NumericUpDown();
            sizeStringOffset = new NumericUpDown();
            shapeOffset = new NumericUpDown();
            overflowOffsetDesc = new Label();
            shapeStringOffsetDesc = new Label();
            shapeOffsetDesc = new Label();
            sizeStringOffsetDesc = new Label();
            sizeCountOffsetDesc = new Label();
            sizeOffset2Desc = new Label();
            sizeOffset1Desc = new Label();
            lobbyOffset2Desc = new Label();
            lobbyOffset1Desc = new Label();
            overflowOffset = new NumericUpDown();
            shapeStringOffset = new NumericUpDown();
            panel1 = new Panel();
            OffsetWarn = new Label();
            offsetPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)lobbyOffset1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lobbyOffset2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sizeOffset1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sizeOffset2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sizeCountOffset).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sizeStringOffset).BeginInit();
            ((System.ComponentModel.ISupportInitialize)shapeOffset).BeginInit();
            ((System.ComponentModel.ISupportInitialize)overflowOffset).BeginInit();
            ((System.ComponentModel.ISupportInitialize)shapeStringOffset).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // offsetPanel
            // 
            offsetPanel.ColumnCount = 2;
            offsetPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            offsetPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            offsetPanel.Controls.Add(overflowOffsetDesc, 0, 9);
            offsetPanel.Controls.Add(shapeStringOffsetDesc, 0, 8);
            offsetPanel.Controls.Add(shapeOffsetDesc, 0, 7);
            offsetPanel.Controls.Add(sizeStringOffsetDesc, 0, 6);
            offsetPanel.Controls.Add(sizeCountOffsetDesc, 0, 5);
            offsetPanel.Controls.Add(sizeOffset2Desc, 0, 4);
            offsetPanel.Controls.Add(sizeOffset1Desc, 0, 3);
            offsetPanel.Controls.Add(lobbyOffset2Desc, 0, 2);
            offsetPanel.Controls.Add(lobbyOffset1Desc, 0, 1);
            offsetPanel.Controls.Add(overflowOffset, 1, 9);
            offsetPanel.Controls.Add(shapeStringOffset, 1, 8);
            offsetPanel.Controls.Add(shapeOffset, 1, 7);
            offsetPanel.Controls.Add(sizeStringOffset, 1, 6);
            offsetPanel.Controls.Add(sizeCountOffset, 1, 5);
            offsetPanel.Controls.Add(sizeOffset2, 1, 4);
            offsetPanel.Controls.Add(sizeOffset1, 1, 3);
            offsetPanel.Controls.Add(lobbyOffset2, 1, 2);
            offsetPanel.Controls.Add(lobbyOffset1, 1, 1);
            offsetPanel.Dock = DockStyle.Fill;
            offsetPanel.Location = new Point(0, 0);
            offsetPanel.Name = "offsetPanel";
            offsetPanel.Padding = new Padding(10);
            offsetPanel.RowCount = 10;
            offsetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            offsetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            offsetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            offsetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            offsetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            offsetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            offsetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            offsetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            offsetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            offsetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            offsetPanel.Size = new Size(253, 314);
            offsetPanel.TabIndex = 0;
            // 
            // lobbyOffset1
            // 
            lobbyOffset1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lobbyOffset1.Increment = new decimal(new int[] { 4, 0, 0, 0 });
            lobbyOffset1.Location = new Point(129, 42);
            lobbyOffset1.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            lobbyOffset1.Name = "lobbyOffset1";
            lobbyOffset1.Size = new Size(111, 23);
            lobbyOffset1.TabIndex = 1;
            lobbyOffset1.Tag = 0;
            lobbyOffset1.ValueChanged += OnOffsetEdit;
            // 
            // lobbyOffset2
            // 
            lobbyOffset2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lobbyOffset2.Increment = new decimal(new int[] { 4, 0, 0, 0 });
            lobbyOffset2.Location = new Point(129, 71);
            lobbyOffset2.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            lobbyOffset2.Name = "lobbyOffset2";
            lobbyOffset2.Size = new Size(111, 23);
            lobbyOffset2.TabIndex = 1;
            lobbyOffset2.Tag = 0;
            lobbyOffset2.ValueChanged += OnOffsetEdit;
            // 
            // sizeOffset1
            // 
            sizeOffset1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            sizeOffset1.Increment = new decimal(new int[] { 4, 0, 0, 0 });
            sizeOffset1.Location = new Point(129, 100);
            sizeOffset1.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            sizeOffset1.Name = "sizeOffset1";
            sizeOffset1.Size = new Size(111, 23);
            sizeOffset1.TabIndex = 1;
            sizeOffset1.Tag = 0;
            sizeOffset1.ValueChanged += OnOffsetEdit;
            // 
            // sizeOffset2
            // 
            sizeOffset2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            sizeOffset2.Increment = new decimal(new int[] { 4, 0, 0, 0 });
            sizeOffset2.Location = new Point(129, 129);
            sizeOffset2.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            sizeOffset2.Name = "sizeOffset2";
            sizeOffset2.Size = new Size(111, 23);
            sizeOffset2.TabIndex = 1;
            sizeOffset2.Tag = 0;
            sizeOffset2.ValueChanged += OnOffsetEdit;
            // 
            // sizeCountOffset
            // 
            sizeCountOffset.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            sizeCountOffset.Increment = new decimal(new int[] { 4, 0, 0, 0 });
            sizeCountOffset.Location = new Point(129, 158);
            sizeCountOffset.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            sizeCountOffset.Name = "sizeCountOffset";
            sizeCountOffset.Size = new Size(111, 23);
            sizeCountOffset.TabIndex = 1;
            sizeCountOffset.Tag = 0;
            sizeCountOffset.ValueChanged += OnOffsetEdit;
            // 
            // sizeStringOffset
            // 
            sizeStringOffset.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            sizeStringOffset.Increment = new decimal(new int[] { 4, 0, 0, 0 });
            sizeStringOffset.Location = new Point(129, 187);
            sizeStringOffset.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            sizeStringOffset.Name = "sizeStringOffset";
            sizeStringOffset.Size = new Size(111, 23);
            sizeStringOffset.TabIndex = 1;
            sizeStringOffset.Tag = 0;
            sizeStringOffset.ValueChanged += OnOffsetEdit;
            // 
            // shapeOffset
            // 
            shapeOffset.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            shapeOffset.Increment = new decimal(new int[] { 4, 0, 0, 0 });
            shapeOffset.Location = new Point(129, 216);
            shapeOffset.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            shapeOffset.Name = "shapeOffset";
            shapeOffset.Size = new Size(111, 23);
            shapeOffset.TabIndex = 1;
            shapeOffset.Tag = 0;
            shapeOffset.ValueChanged += OnOffsetEdit;
            // 
            // overflowOffsetDesc
            // 
            overflowOffsetDesc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            overflowOffsetDesc.AutoSize = true;
            overflowOffsetDesc.Location = new Point(13, 279);
            overflowOffsetDesc.Name = "overflowOffsetDesc";
            overflowOffsetDesc.Size = new Size(110, 17);
            overflowOffsetDesc.TabIndex = 0;
            overflowOffsetDesc.Text = "OverflowOffset";
            // 
            // shapeStringOffsetDesc
            // 
            shapeStringOffsetDesc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            shapeStringOffsetDesc.AutoSize = true;
            shapeStringOffsetDesc.Location = new Point(13, 248);
            shapeStringOffsetDesc.Name = "shapeStringOffsetDesc";
            shapeStringOffsetDesc.Size = new Size(110, 17);
            shapeStringOffsetDesc.TabIndex = 0;
            shapeStringOffsetDesc.Text = "ShapeString";
            // 
            // shapeOffsetDesc
            // 
            shapeOffsetDesc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            shapeOffsetDesc.AutoSize = true;
            shapeOffsetDesc.Location = new Point(13, 219);
            shapeOffsetDesc.Name = "shapeOffsetDesc";
            shapeOffsetDesc.Size = new Size(110, 17);
            shapeOffsetDesc.TabIndex = 0;
            shapeOffsetDesc.Text = "ShapeOffset";
            // 
            // sizeStringOffsetDesc
            // 
            sizeStringOffsetDesc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            sizeStringOffsetDesc.AutoSize = true;
            sizeStringOffsetDesc.Location = new Point(13, 190);
            sizeStringOffsetDesc.Name = "sizeStringOffsetDesc";
            sizeStringOffsetDesc.Size = new Size(110, 17);
            sizeStringOffsetDesc.TabIndex = 0;
            sizeStringOffsetDesc.Text = "SizeString";
            // 
            // sizeCountOffsetDesc
            // 
            sizeCountOffsetDesc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            sizeCountOffsetDesc.AutoSize = true;
            sizeCountOffsetDesc.Location = new Point(13, 161);
            sizeCountOffsetDesc.Name = "sizeCountOffsetDesc";
            sizeCountOffsetDesc.Size = new Size(110, 17);
            sizeCountOffsetDesc.TabIndex = 0;
            sizeCountOffsetDesc.Text = "SizeCount";
            // 
            // sizeOffset2Desc
            // 
            sizeOffset2Desc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            sizeOffset2Desc.AutoSize = true;
            sizeOffset2Desc.Location = new Point(13, 132);
            sizeOffset2Desc.Name = "sizeOffset2Desc";
            sizeOffset2Desc.Size = new Size(110, 17);
            sizeOffset2Desc.TabIndex = 0;
            sizeOffset2Desc.Text = "SizeOffset2";
            // 
            // sizeOffset1Desc
            // 
            sizeOffset1Desc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            sizeOffset1Desc.AutoSize = true;
            sizeOffset1Desc.Location = new Point(13, 103);
            sizeOffset1Desc.Name = "sizeOffset1Desc";
            sizeOffset1Desc.Size = new Size(110, 17);
            sizeOffset1Desc.TabIndex = 0;
            sizeOffset1Desc.Text = "SizeOffset1";
            // 
            // lobbyOffset2Desc
            // 
            lobbyOffset2Desc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lobbyOffset2Desc.AutoSize = true;
            lobbyOffset2Desc.Location = new Point(13, 74);
            lobbyOffset2Desc.Name = "lobbyOffset2Desc";
            lobbyOffset2Desc.Size = new Size(110, 17);
            lobbyOffset2Desc.TabIndex = 0;
            lobbyOffset2Desc.Text = "LobbyOffset2";
            // 
            // lobbyOffset1Desc
            // 
            lobbyOffset1Desc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lobbyOffset1Desc.AutoSize = true;
            lobbyOffset1Desc.Location = new Point(13, 45);
            lobbyOffset1Desc.Name = "lobbyOffset1Desc";
            lobbyOffset1Desc.Size = new Size(110, 17);
            lobbyOffset1Desc.TabIndex = 0;
            lobbyOffset1Desc.Text = "LobbyOffset1";
            // 
            // overflowOffset
            // 
            overflowOffset.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            overflowOffset.Increment = new decimal(new int[] { 4, 0, 0, 0 });
            overflowOffset.Location = new Point(129, 276);
            overflowOffset.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            overflowOffset.Name = "overflowOffset";
            overflowOffset.Size = new Size(111, 23);
            overflowOffset.TabIndex = 1;
            overflowOffset.Tag = 0;
            overflowOffset.ValueChanged += OnOffsetEdit;
            // 
            // shapeStringOffset
            // 
            shapeStringOffset.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            shapeStringOffset.Increment = new decimal(new int[] { 4, 0, 0, 0 });
            shapeStringOffset.Location = new Point(129, 245);
            shapeStringOffset.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            shapeStringOffset.Name = "shapeStringOffset";
            shapeStringOffset.Size = new Size(111, 23);
            shapeStringOffset.TabIndex = 1;
            shapeStringOffset.Tag = 0;
            shapeStringOffset.ValueChanged += OnOffsetEdit;
            // 
            // panel1
            // 
            panel1.Controls.Add(OffsetWarn);
            panel1.Controls.Add(offsetPanel);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(253, 314);
            panel1.TabIndex = 1;
            // 
            // OffsetWarn
            // 
            OffsetWarn.Dock = DockStyle.Top;
            OffsetWarn.Location = new Point(0, 0);
            OffsetWarn.Margin = new Padding(3);
            OffsetWarn.Name = "OffsetWarn";
            OffsetWarn.Size = new Size(253, 37);
            OffsetWarn.TabIndex = 1;
            OffsetWarn.Text = "Do not change the number unless you know what you are doing";
            // 
            // OffsetSetter
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(253, 314);
            Controls.Add(panel1);
            Name = "OffsetSetter";
            Text = "Offset Setter";
            offsetPanel.ResumeLayout(false);
            offsetPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)lobbyOffset1).EndInit();
            ((System.ComponentModel.ISupportInitialize)lobbyOffset2).EndInit();
            ((System.ComponentModel.ISupportInitialize)sizeOffset1).EndInit();
            ((System.ComponentModel.ISupportInitialize)sizeOffset2).EndInit();
            ((System.ComponentModel.ISupportInitialize)sizeCountOffset).EndInit();
            ((System.ComponentModel.ISupportInitialize)sizeStringOffset).EndInit();
            ((System.ComponentModel.ISupportInitialize)shapeOffset).EndInit();
            ((System.ComponentModel.ISupportInitialize)overflowOffset).EndInit();
            ((System.ComponentModel.ISupportInitialize)shapeStringOffset).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel offsetPanel;
        private Label lobbyOffset1Desc;
        private Label lobbyOffset2Desc;
        private Label sizeOffset1Desc;
        private Label sizeOffset2Desc;
        private Label sizeStringOffsetDesc;
        private Label shapeOffsetDesc;
        private Label shapeStringOffsetDesc;
        private Label overflowOffsetDesc;
        private Label sizeCountOffsetDesc;
        private NumericUpDown lobbyOffset1;
        private NumericUpDown lobbyOffset2;
        private NumericUpDown sizeOffset1;
        private NumericUpDown sizeOffset2;
        private NumericUpDown sizeCountOffset;
        private NumericUpDown sizeStringOffset;
        private NumericUpDown shapeOffset;
        private NumericUpDown shapeStringOffset;
        private NumericUpDown overflowOffset;
        private Panel panel1;
        private Label OffsetWarn;
    }
}