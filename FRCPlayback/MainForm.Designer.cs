namespace FRCPlayback
{
    partial class FRCPlayback
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
            MatchInput = new MaterialSkin.Controls.MaterialTextBox2();
            SuggestionsButton = new MaterialSkin.Controls.MaterialButton();
            SuggestionsList = new MaterialSkin.Controls.MaterialListBox();
            SuspendLayout();
            // 
            // MatchInput
            // 
            MatchInput.AnimateReadOnly = false;
            MatchInput.BackgroundImageLayout = ImageLayout.None;
            MatchInput.CharacterCasing = CharacterCasing.Normal;
            MatchInput.Depth = 0;
            MatchInput.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            MatchInput.HideSelection = true;
            MatchInput.Hint = "Enter event or team number";
            MatchInput.LeadingIcon = null;
            MatchInput.Location = new Point(90, 119);
            MatchInput.MaxLength = 32767;
            MatchInput.MouseState = MaterialSkin.MouseState.OUT;
            MatchInput.Name = "MatchInput";
            MatchInput.PasswordChar = '\0';
            MatchInput.PrefixSuffixText = null;
            MatchInput.ReadOnly = false;
            MatchInput.RightToLeft = RightToLeft.No;
            MatchInput.SelectedText = "";
            MatchInput.SelectionLength = 0;
            MatchInput.SelectionStart = 0;
            MatchInput.ShortcutsEnabled = true;
            MatchInput.Size = new Size(250, 48);
            MatchInput.TabIndex = 0;
            MatchInput.TabStop = false;
            MatchInput.TextAlign = HorizontalAlignment.Left;
            MatchInput.TrailingIcon = null;
            MatchInput.UseSystemPasswordChar = false;
            MatchInput.Enter += MatchInput_Enter;
            MatchInput.Leave += MatchInput_Leave;
            MatchInput.TextChanged += MatchInput_TextChanged;
            // 
            // SuggestionsButton
            // 
            SuggestionsButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            SuggestionsButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            SuggestionsButton.Depth = 0;
            SuggestionsButton.HighEmphasis = true;
            SuggestionsButton.Icon = null;
            SuggestionsButton.Location = new Point(627, 131);
            SuggestionsButton.Margin = new Padding(4, 6, 4, 6);
            SuggestionsButton.MouseState = MaterialSkin.MouseState.HOVER;
            SuggestionsButton.Name = "SuggestionsButton";
            SuggestionsButton.NoAccentTextColor = Color.Empty;
            SuggestionsButton.Size = new Size(166, 36);
            SuggestionsButton.TabIndex = 1;
            SuggestionsButton.Text = "Show Suggestions";
            SuggestionsButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            SuggestionsButton.UseAccentColor = false;
            SuggestionsButton.UseVisualStyleBackColor = true;
            SuggestionsButton.Click += SuggestionsButton_Click;
            // 
            // SuggestionsList
            // 
            SuggestionsList.BackColor = Color.White;
            SuggestionsList.BorderColor = Color.LightGray;
            SuggestionsList.Depth = 0;
            SuggestionsList.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            SuggestionsList.Location = new Point(346, 119);
            SuggestionsList.MouseState = MaterialSkin.MouseState.HOVER;
            SuggestionsList.Name = "SuggestionsList";
            SuggestionsList.SelectedIndex = -1;
            SuggestionsList.SelectedItem = null;
            SuggestionsList.Size = new Size(262, 150);
            SuggestionsList.TabIndex = 2;
            SuggestionsList.Visible = false;
            // 
            // FRCPlayback
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(800, 450);
            Controls.Add(SuggestionsList);
            Controls.Add(SuggestionsButton);
            Controls.Add(MatchInput);
            Name = "FRCPlayback";
            Text = "FRC Playback";
            Load += MainForm_Load;
            MouseClick += FRCPlayback_MouseClick;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MaterialSkin.Controls.MaterialTextBox2 MatchInput;
        private MaterialSkin.Controls.MaterialButton SuggestionsButton;
        private MaterialSkin.Controls.MaterialListBox SuggestionsList;
    }
}