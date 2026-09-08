namespace AccountSystem_Final
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
        private void InitializeComponent() {
            usernameInputField = new TextBox();
            passwordInputField = new TextBox();
            loginButton = new Button();
            createUserButton = new Button();
            resultText = new TextBox();
            SuspendLayout();
            // 
            // usernameInputField
            // 
            usernameInputField.BackColor = SystemColors.WindowFrame;
            usernameInputField.Font = new Font("Yu Gothic", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            usernameInputField.Location = new Point(796, 337);
            usernameInputField.Name = "usernameInputField";
            usernameInputField.Size = new Size(249, 32);
            usernameInputField.TabIndex = 0;
            usernameInputField.Text = "Username...";
            usernameInputField.TextChanged += usernameInputField_TextChanged;
            // 
            // passwordInputField
            // 
            passwordInputField.BackColor = SystemColors.WindowFrame;
            passwordInputField.Font = new Font("Yu Gothic", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            passwordInputField.Location = new Point(796, 415);
            passwordInputField.Name = "passwordInputField";
            passwordInputField.Size = new Size(249, 32);
            passwordInputField.TabIndex = 1;
            passwordInputField.Text = "Password...";
            passwordInputField.TextChanged += passwordInputField_TextChanged;
            // 
            // loginButton
            // 
            loginButton.BackColor = Color.Silver;
            loginButton.Location = new Point(708, 486);
            loginButton.Name = "loginButton";
            loginButton.Size = new Size(188, 54);
            loginButton.TabIndex = 2;
            loginButton.Text = "Login";
            loginButton.UseVisualStyleBackColor = false;
            loginButton.Click += loginButton_Click;
            // 
            // createUserButton
            // 
            createUserButton.BackColor = SystemColors.ActiveCaption;
            createUserButton.Location = new Point(928, 486);
            createUserButton.Name = "createUserButton";
            createUserButton.Size = new Size(188, 54);
            createUserButton.TabIndex = 3;
            createUserButton.Text = "Create";
            createUserButton.UseVisualStyleBackColor = false;
            createUserButton.Click += createUserButton_Click;
            // 
            // resultText
            // 
            resultText.BackColor = SystemColors.Window;
            resultText.BorderStyle = BorderStyle.None;
            resultText.Location = new Point(825, 250);
            resultText.Name = "resultText";
            resultText.Size = new Size(200, 20);
            resultText.TabIndex = 4;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1804, 831);
            Controls.Add(resultText);
            Controls.Add(createUserButton);
            Controls.Add(loginButton);
            Controls.Add(passwordInputField);
            Controls.Add(usernameInputField);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox usernameInputField;
        private TextBox passwordInputField;
        private Button loginButton;
        private Button createUserButton;
        public TextBox resultText;
    }
}
