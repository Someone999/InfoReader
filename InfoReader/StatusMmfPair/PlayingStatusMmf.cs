using osuTools.Game;

namespace InfoReaderPlugin.StatusMmfPair
{
    public class PlayingStatusMmf : StatusMmf
    {
        public override OsuGameStatus TargetStatus => OsuGameStatus.Playing;

        public PlayingStatusMmf() : base("InfoReaderMmfPlaying")
        {
        }
    }
}