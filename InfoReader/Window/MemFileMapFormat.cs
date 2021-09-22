using System;
using System.IO;
using System.Windows.Forms;
using InfoReaderPlugin.I18n;

namespace InfoReaderPlugin
{
    public delegate void MmfSaved();

    public delegate void MmfLoaded();
    public partial class MemFileMapFormat : Form
    {
        public MmfSaved OnSaved;
        public MmfLoaded OnFormatLoaded;
        public MemFileMapFormat()
        {
            InitializeComponent();
            
            tabPage_playing.Text = NI18n.GetLanguageElement("LANG_MMFWIN_PLAYING");
            tabPage_selectSong.Text = NI18n.GetLanguageElement("LANG_MMFWIN_SELECTSONG");
            tabPage_ranking.Text = NI18n.GetLanguageElement("LANG_MMFWIN_RANKING");
            tabPage_editing.Text = NI18n.GetLanguageElement("LANG_MMFWIN_EDITING");
            tabPage_matchSet.Text = NI18n.GetLanguageElement("LANG_MMFWIN_MATCHSET");
            tabPage_idle.Text = NI18n.GetLanguageElement("LANG_MMFWIN_IDLE");
            tabPage_lobby.Text = NI18n.GetLanguageElement("LANG_MMFWIN_LOBBY");
            if (!Directory.Exists("FormatInfo"))
            {
                Directory.CreateDirectory("FormatInfo");
            }
            string[] dirs = { 
                "FormatInfo\\PlayingFormatConfig.ini", 
                "FormatInfo\\SelectSongFormatConfig.ini" ,
                "FormatInfo\\RankFormatConfig.ini",
                "FormatInfo\\LobbyFormatConfig.ini",
                "FormatInfo\\MatchSetupFormatConfig.ini",
                "FormatInfo\\IdleFormatConfig.ini",
                "FormatInfo\\EditingFormatConfig.ini",
            };
            foreach (var file in dirs)
                if (!File.Exists(file))
                    File.Create(file);
            tx_playing.Text = File.ReadAllText("FormatInfo\\PlayingFormatConfig.ini");
            tx_selectSong.Text = File.ReadAllText("FormatInfo\\SelectSongFormatConfig.ini");
            tx_rank.Text = File.ReadAllText("FormatInfo\\RankFormatConfig.ini");
            tx_lobby.Text = File.ReadAllText("FormatInfo\\LobbyFormatConfig.ini");
            tx_matchSet.Text = File.ReadAllText("FormatInfo\\MatchSetupFormatConfig.ini");
            tx_idle.Text = File.ReadAllText("FormatInfo\\IdleFormatConfig.ini");
            tx_editing.Text = File.ReadAllText("FormatInfo\\EditingFormatConfig.ini");
            OnFormatLoaded?.Invoke();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists("FormatInfo"))
            {
                Directory.CreateDirectory("FormatInfo");
            }
            File.WriteAllText("FormatInfo\\PlayingFormatConfig.ini", tx_playing.Text);
            File.WriteAllText("FormatInfo\\SelectSongFormatConfig.ini", tx_selectSong.Text);
            File.WriteAllText("FormatInfo\\RankFormatConfig.ini", tx_rank.Text);
            File.WriteAllText("FormatInfo\\LobbyFormatConfig.ini", tx_lobby.Text);
            File.WriteAllText("FormatInfo\\MatchSetupFormatConfig.ini", tx_matchSet.Text);
            File.WriteAllText("FormatInfo\\IdleFormatConfig.ini", tx_idle.Text);
            File.WriteAllText("FormatInfo\\EditingFormatConfig.ini", tx_editing.Text);
            OnSaved?.Invoke();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}
