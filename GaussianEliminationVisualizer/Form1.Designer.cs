namespace GaussianEliminationVisualizer
{
    partial class MainForm
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
            label1 = new Label();
            label2 = new Label();
            numericUpDown1 = new NumericUpDown();
            label3 = new Label();
            numericUpDown2 = new NumericUpDown();
            button1 = new Button();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            matrixPanel = new Panel();
            solveButton = new Button();
            stepsRichTextBox = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(459, 9);
            label1.Name = "label1";
            label1.Size = new Size(518, 46);
            label1.TabIndex = 0;
            label1.Text = "Gaussian Elimination Visualizer";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(30, 59);
            label2.Name = "label2";
            label2.Size = new Size(227, 23);
            label2.TabIndex = 1;
            label2.Text = "Number of equations (rows):";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(321, 52);
            numericUpDown1.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(190, 30);
            numericUpDown1.TabIndex = 2;
            numericUpDown1.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(30, 137);
            label3.Name = "label3";
            label3.Size = new Size(168, 23);
            label3.TabIndex = 3;
            label3.Text = "Number of variables:";
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new Point(321, 130);
            numericUpDown2.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            numericUpDown2.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(190, 30);
            numericUpDown2.TabIndex = 4;
            numericUpDown2.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(442, 233);
            button1.Name = "button1";
            button1.Size = new Size(147, 33);
            button1.TabIndex = 5;
            button1.Text = "Create Matrix";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(numericUpDown1);
            groupBox1.Controls.Add(numericUpDown2);
            groupBox1.Controls.Add(label3);
            groupBox1.Location = new Point(397, 80);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(633, 302);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "System Setup";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(matrixPanel);
            groupBox2.Location = new Point(452, 399);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(534, 285);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "Augmented Matrix";
            groupBox2.UseWaitCursor = true;
            // 
            // matrixPanel
            // 
            matrixPanel.AutoScroll = true;
            matrixPanel.Location = new Point(6, 20);
            matrixPanel.Name = "matrixPanel";
            matrixPanel.Size = new Size(522, 259);
            matrixPanel.TabIndex = 0;
            matrixPanel.UseWaitCursor = true;
            // 
            // solveButton
            // 
            solveButton.Location = new Point(642, 716);
            solveButton.Name = "solveButton";
            solveButton.Size = new Size(129, 29);
            solveButton.TabIndex = 8;
            solveButton.Text = "Solve Matrix";
            solveButton.UseVisualStyleBackColor = true;
            solveButton.Click += solveButton_Click;
            // 
            // stepsRichTextBox
            // 
            stepsRichTextBox.Location = new Point(1064, 214);
            stepsRichTextBox.Name = "stepsRichTextBox";
            stepsRichTextBox.ReadOnly = true;
            stepsRichTextBox.Size = new Size(534, 589);
            stepsRichTextBox.TabIndex = 9;
            stepsRichTextBox.Text = "";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1562, 982);
            Controls.Add(stepsRichTextBox);
            Controls.Add(solveButton);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MinimumSize = new Size(650, 1000);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Gaussian Elimination Visualizer";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private NumericUpDown numericUpDown1;
        private Label label3;
        private NumericUpDown numericUpDown2;
        private Button button1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button solveButton;
        private Panel matrixPanel;
        private RichTextBox stepsRichTextBox;
    }
}
