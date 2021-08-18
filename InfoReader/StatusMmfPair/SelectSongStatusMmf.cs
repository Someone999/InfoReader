using osuTools.Game;

namespace InfoReaderPlugin.StatusMmfPair
{
    public class SelectSongStatusMmf : StatusMmf
    {
        public override OsuGameStatus TargetStatus => OsuGameStatus.SelectSong;

        public SelectSongStatusMmf() : base("InfoReaderMmfSelectSong")
        {
        }
    }
}