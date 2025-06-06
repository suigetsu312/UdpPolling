namespace WinFormsApp1
{
    partial class Form1
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
            label_socket1 = new Label();
            label_socket2 = new Label();
            richTextBox_socket1 = new RichTextBox();
            richTextBox_socket2 = new RichTextBox();
            button_send = new Button();
            button_endsend = new Button();
            SuspendLayout();
            // 
            // label_socket1
            // 
            label_socket1.AutoSize = true;
            label_socket1.Location = new Point(112, 58);
            label_socket1.Name = "label_socket1";
            label_socket1.Size = new Size(72, 15);
            label_socket1.TabIndex = 0;
            label_socket1.Text = "socket1_rec";
            // 
            // label_socket2
            // 
            label_socket2.AutoSize = true;
            label_socket2.Location = new Point(393, 58);
            label_socket2.Name = "label_socket2";
            label_socket2.Size = new Size(72, 15);
            label_socket2.TabIndex = 1;
            label_socket2.Text = "socket2_rec";
            // 
            // richTextBox_socket1
            // 
            richTextBox_socket1.Location = new Point(116, 99);
            richTextBox_socket1.Name = "richTextBox_socket1";
            richTextBox_socket1.Size = new Size(158, 96);
            richTextBox_socket1.TabIndex = 2;
            richTextBox_socket1.Text = "";
            // 
            // richTextBox_socket2
            // 
            richTextBox_socket2.Location = new Point(393, 99);
            richTextBox_socket2.Name = "richTextBox_socket2";
            richTextBox_socket2.Size = new Size(173, 96);
            richTextBox_socket2.TabIndex = 3;
            richTextBox_socket2.Text = "";
            // 
            // button_send
            // 
            button_send.Location = new Point(152, 283);
            button_send.Name = "button_send";
            button_send.Size = new Size(75, 23);
            button_send.TabIndex = 4;
            button_send.Text = "send";
            button_send.UseVisualStyleBackColor = true;
            button_send.Click += button_send_Click;
            // 
            // button_endsend
            // 
            button_endsend.Location = new Point(357, 283);
            button_endsend.Name = "button_endsend";
            button_endsend.Size = new Size(75, 23);
            button_endsend.TabIndex = 5;
            button_endsend.Text = "end send";
            button_endsend.UseVisualStyleBackColor = true;
            button_endsend.Click += button_endsend_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button_endsend);
            Controls.Add(button_send);
            Controls.Add(richTextBox_socket2);
            Controls.Add(richTextBox_socket1);
            Controls.Add(label_socket2);
            Controls.Add(label_socket1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_socket1;
        private Label label_socket2;
        private RichTextBox richTextBox_socket1;
        private RichTextBox richTextBox_socket2;
        private Button button_send;
        private Button button_endsend;
    }
}
