namespace InfoReaderPlugin
{
    partial class MemFileMapFormat
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabControl();
            this.tabPage_playing = new System.Windows.Forms.TabPage();
            this.tx_playing = new System.Windows.Forms.TextBox();
            this.tabPage_selectSong = new System.Windows.Forms.TabPage();
            this.tx_selectSong = new System.Windows.Forms.TextBox();
            this.tabPage_ranking = new System.Windows.Forms.TabPage();
            this.tx_rank = new System.Windows.Forms.TextBox();
            this.tabPage_idle = new System.Windows.Forms.TabPage();
            this.tx_idle = new System.Windows.Forms.TextBox();
            this.tabPage_editing = new System.Windows.Forms.TabPage();
            this.tx_editing = new System.Windows.Forms.TextBox();
            this.tabPage_matchSet = new System.Windows.Forms.TabPage();
            this.tx_matchSet = new System.Windows.Forms.TextBox();
            this.tabPage_lobby = new System.Windows.Forms.TabPage();
            this.tx_lobby = new System.Windows.Forms.TextBox();
            this.tabPage2.SuspendLayout();
            this.tabPage_playing.SuspendLayout();
            this.tabPage_selectSong.SuspendLayout();
            this.tabPage_ranking.SuspendLayout();
            this.tabPage_idle.SuspendLayout();
            this.tabPage_editing.SuspendLayout();
            this.tabPage_matchSet.SuspendLayout();
            this.tabPage_lobby.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(580, 412);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(77, 26);
            this.button1.TabIndex = 1;
            this.button1.Text = "保存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(664, 414);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tabPage_playing);
            this.tabPage2.Controls.Add(this.tabPage_selectSong);
            this.tabPage2.Controls.Add(this.tabPage_ranking);
            this.tabPage2.Controls.Add(this.tabPage_idle);
            this.tabPage2.Controls.Add(this.tabPage_editing);
            this.tabPage2.Controls.Add(this.tabPage_matchSet);
            this.tabPage2.Controls.Add(this.tabPage_lobby);
            this.tabPage2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPage2.Location = new System.Drawing.Point(13, 13);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.SelectedIndex = 0;
            this.tabPage2.Size = new System.Drawing.Size(775, 395);
            this.tabPage2.TabIndex = 3;
            // 
            // tabPage_playing
            // 
            this.tabPage_playing.Controls.Add(this.tx_playing);
            this.tabPage_playing.Location = new System.Drawing.Point(4, 26);
            this.tabPage_playing.Name = "tabPage_playing";
            this.tabPage_playing.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_playing.Size = new System.Drawing.Size(767, 365);
            this.tabPage_playing.TabIndex = 0;
            this.tabPage_playing.Text = "Playing";
            this.tabPage_playing.UseVisualStyleBackColor = true;
            // 
            // tx_playing
            // 
            this.tx_playing.Location = new System.Drawing.Point(7, 7);
            this.tx_playing.Multiline = true;
            this.tx_playing.Name = "tx_playing";
            this.tx_playing.Size = new System.Drawing.Size(754, 356);
            this.tx_playing.TabIndex = 0;
            // 
            // tabPage_selectSong
            // 
            this.tabPage_selectSong.Controls.Add(this.tx_selectSong);
            this.tabPage_selectSong.Location = new System.Drawing.Point(4, 26);
            this.tabPage_selectSong.Name = "tabPage_selectSong";
            this.tabPage_selectSong.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_selectSong.Size = new System.Drawing.Size(767, 365);
            this.tabPage_selectSong.TabIndex = 1;
            this.tabPage_selectSong.Text = "SelectSong";
            this.tabPage_selectSong.UseVisualStyleBackColor = true;
            // 
            // tx_selectSong
            // 
            this.tx_selectSong.Location = new System.Drawing.Point(7, 7);
            this.tx_selectSong.Multiline = true;
            this.tx_selectSong.Name = "tx_selectSong";
            this.tx_selectSong.Size = new System.Drawing.Size(754, 356);
            this.tx_selectSong.TabIndex = 1;
            // 
            // tabPage_ranking
            // 
            this.tabPage_ranking.Controls.Add(this.tx_rank);
            this.tabPage_ranking.Location = new System.Drawing.Point(4, 26);
            this.tabPage_ranking.Name = "tabPage_ranking";
            this.tabPage_ranking.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_ranking.Size = new System.Drawing.Size(767, 365);
            this.tabPage_ranking.TabIndex = 2;
            this.tabPage_ranking.Text = "Rank";
            this.tabPage_ranking.UseVisualStyleBackColor = true;
            // 
            // tx_rank
            // 
            this.tx_rank.Location = new System.Drawing.Point(7, 7);
            this.tx_rank.Multiline = true;
            this.tx_rank.Name = "tx_rank";
            this.tx_rank.Size = new System.Drawing.Size(754, 356);
            this.tx_rank.TabIndex = 2;
            // 
            // tabPage_idle
            // 
            this.tabPage_idle.Controls.Add(this.tx_idle);
            this.tabPage_idle.Location = new System.Drawing.Point(4, 26);
            this.tabPage_idle.Name = "tabPage_idle";
            this.tabPage_idle.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_idle.Size = new System.Drawing.Size(767, 365);
            this.tabPage_idle.TabIndex = 3;
            this.tabPage_idle.Text = "Idle";
            this.tabPage_idle.UseVisualStyleBackColor = true;
            // 
            // tx_idle
            // 
            this.tx_idle.Location = new System.Drawing.Point(7, 7);
            this.tx_idle.Multiline = true;
            this.tx_idle.Name = "tx_idle";
            this.tx_idle.Size = new System.Drawing.Size(754, 356);
            this.tx_idle.TabIndex = 3;
            // 
            // tabPage_editing
            // 
            this.tabPage_editing.Controls.Add(this.tx_editing);
            this.tabPage_editing.Location = new System.Drawing.Point(4, 26);
            this.tabPage_editing.Name = "tabPage_editing";
            this.tabPage_editing.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_editing.Size = new System.Drawing.Size(767, 365);
            this.tabPage_editing.TabIndex = 4;
            this.tabPage_editing.Text = "Eiditing";
            this.tabPage_editing.UseVisualStyleBackColor = true;
            // 
            // tx_editing
            // 
            this.tx_editing.Location = new System.Drawing.Point(7, 7);
            this.tx_editing.Multiline = true;
            this.tx_editing.Name = "tx_editing";
            this.tx_editing.Size = new System.Drawing.Size(754, 356);
            this.tx_editing.TabIndex = 3;
            // 
            // tabPage_matchSet
            // 
            this.tabPage_matchSet.Controls.Add(this.tx_matchSet);
            this.tabPage_matchSet.Location = new System.Drawing.Point(4, 26);
            this.tabPage_matchSet.Name = "tabPage_matchSet";
            this.tabPage_matchSet.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_matchSet.Size = new System.Drawing.Size(767, 365);
            this.tabPage_matchSet.TabIndex = 5;
            this.tabPage_matchSet.Text = "MatchSet";
            this.tabPage_matchSet.UseVisualStyleBackColor = true;
            // 
            // tx_matchSet
            // 
            this.tx_matchSet.Location = new System.Drawing.Point(7, 7);
            this.tx_matchSet.Multiline = true;
            this.tx_matchSet.Name = "tx_matchSet";
            this.tx_matchSet.Size = new System.Drawing.Size(754, 356);
            this.tx_matchSet.TabIndex = 3;
            // 
            // tabPage_lobby
            // 
            this.tabPage_lobby.Controls.Add(this.tx_lobby);
            this.tabPage_lobby.Location = new System.Drawing.Point(4, 26);
            this.tabPage_lobby.Name = "tabPage_lobby";
            this.tabPage_lobby.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_lobby.Size = new System.Drawing.Size(767, 365);
            this.tabPage_lobby.TabIndex = 6;
            this.tabPage_lobby.Text = "Lobby";
            this.tabPage_lobby.UseVisualStyleBackColor = true;
            // 
            // tx_lobby
            // 
            this.tx_lobby.Location = new System.Drawing.Point(7, 7);
            this.tx_lobby.Multiline = true;
            this.tx_lobby.Name = "tx_lobby";
            this.tx_lobby.Size = new System.Drawing.Size(754, 356);
            this.tx_lobby.TabIndex = 3;
            // 
            // MemFileMapFormat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabPage2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "MemFileMapFormat";
            this.Text = "Form3";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form3_FormClosing);
            this.Load += new System.EventHandler(this.Form3_Load);
            this.tabPage2.ResumeLayout(false);
            this.tabPage_playing.ResumeLayout(false);
            this.tabPage_playing.PerformLayout();
            this.tabPage_selectSong.ResumeLayout(false);
            this.tabPage_selectSong.PerformLayout();
            this.tabPage_ranking.ResumeLayout(false);
            this.tabPage_ranking.PerformLayout();
            this.tabPage_idle.ResumeLayout(false);
            this.tabPage_idle.PerformLayout();
            this.tabPage_editing.ResumeLayout(false);
            this.tabPage_editing.PerformLayout();
            this.tabPage_matchSet.ResumeLayout(false);
            this.tabPage_matchSet.PerformLayout();
            this.tabPage_lobby.ResumeLayout(false);
            this.tabPage_lobby.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabControl tabPage2;
        private System.Windows.Forms.TabPage tabPage_playing;
        private System.Windows.Forms.TextBox tx_playing;
        private System.Windows.Forms.TabPage tabPage_selectSong;
        private System.Windows.Forms.TextBox tx_selectSong;
        private System.Windows.Forms.TabPage tabPage_ranking;
        private System.Windows.Forms.TextBox tx_rank;
        private System.Windows.Forms.TabPage tabPage_idle;
        private System.Windows.Forms.TextBox tx_idle;
        private System.Windows.Forms.TabPage tabPage_editing;
        private System.Windows.Forms.TextBox tx_editing;
        private System.Windows.Forms.TabPage tabPage_matchSet;
        private System.Windows.Forms.TextBox tx_matchSet;
        private System.Windows.Forms.TabPage tabPage_lobby;
        private System.Windows.Forms.TextBox tx_lobby;
    }
}