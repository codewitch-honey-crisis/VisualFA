namespace FsmExplorer
{
	partial class Main
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
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.Regex = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.Input = new System.Windows.Forms.TextBox();
			this.Graph = new System.Windows.Forms.PictureBox();
			this.Timeout = new System.Windows.Forms.Timer(this.components);
			this.NfaDfa = new System.Windows.Forms.RadioButton();
			this.OptimizedDfa = new System.Windows.Forms.RadioButton();
			this.Error = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.Graph)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Regex";
			// 
			// Regex
			// 
			this.Regex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Regex.Location = new System.Drawing.Point(74, 9);
			this.Regex.Name = "Regex";
			this.Regex.Size = new System.Drawing.Size(532, 26);
			this.Regex.TabIndex = 1;
			this.Regex.Text = "foo|(bar)+";
			this.Regex.Validating += new System.ComponentModel.CancelEventHandler(this.Regex_Validating);
			this.Regex.Validated += new System.EventHandler(this.Regex_Validated);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(46, 20);
			this.label2.TabIndex = 2;
			this.label2.Text = "Input";
			// 
			// Input
			// 
			this.Input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Input.Location = new System.Drawing.Point(72, 67);
			this.Input.Name = "Input";
			this.Input.Size = new System.Drawing.Size(532, 26);
			this.Input.TabIndex = 3;
			this.Input.TextChanged += new System.EventHandler(this.Input_TextChanged);
			// 
			// Graph
			// 
			this.Graph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Graph.Location = new System.Drawing.Point(16, 130);
			this.Graph.Name = "Graph";
			this.Graph.Size = new System.Drawing.Size(590, 282);
			this.Graph.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.Graph.TabIndex = 4;
			this.Graph.TabStop = false;
			// 
			// Timeout
			// 
			this.Timeout.Interval = 250;
			this.Timeout.Tick += new System.EventHandler(this.Timeout_Tick);
			// 
			// NfaDfa
			// 
			this.NfaDfa.AutoSize = true;
			this.NfaDfa.Checked = true;
			this.NfaDfa.Location = new System.Drawing.Point(18, 99);
			this.NfaDfa.Name = "NfaDfa";
			this.NfaDfa.Size = new System.Drawing.Size(103, 24);
			this.NfaDfa.TabIndex = 5;
			this.NfaDfa.TabStop = true;
			this.NfaDfa.Text = "NFA/DFA";
			this.NfaDfa.UseVisualStyleBackColor = true;
			// 
			// OptimizedDfa
			// 
			this.OptimizedDfa.AutoSize = true;
			this.OptimizedDfa.Location = new System.Drawing.Point(128, 100);
			this.OptimizedDfa.Name = "OptimizedDfa";
			this.OptimizedDfa.Size = new System.Drawing.Size(142, 24);
			this.OptimizedDfa.TabIndex = 6;
			this.OptimizedDfa.TabStop = true;
			this.OptimizedDfa.Text = "Optimized DFA";
			this.OptimizedDfa.UseVisualStyleBackColor = true;
			this.OptimizedDfa.CheckedChanged += new System.EventHandler(this.OptimizedDfa_CheckedChanged);
			// 
			// Error
			// 
			this.Error.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Error.AutoEllipsis = true;
			this.Error.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.Error.Location = new System.Drawing.Point(13, 38);
			this.Error.Name = "Error";
			this.Error.Size = new System.Drawing.Size(591, 23);
			this.Error.TabIndex = 7;
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(618, 424);
			this.Controls.Add(this.Error);
			this.Controls.Add(this.OptimizedDfa);
			this.Controls.Add(this.NfaDfa);
			this.Controls.Add(this.Graph);
			this.Controls.Add(this.Input);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.Regex);
			this.Controls.Add(this.label1);
			this.Name = "Main";
			this.Text = "FSM Explorer";
			((System.ComponentModel.ISupportInitialize)(this.Graph)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox Regex;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox Input;
		private System.Windows.Forms.PictureBox Graph;
		private System.Windows.Forms.Timer Timeout;
		private System.Windows.Forms.RadioButton NfaDfa;
		private System.Windows.Forms.RadioButton OptimizedDfa;
		private System.Windows.Forms.Label Error;
	}
}

