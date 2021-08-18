using osuTools.Game;

namespace InfoReaderPlugin.StatusMmfPair
{
    public class ResultStatusMmf : StatusMmf
    {
        public override OsuGameStatus TargetStatus => OsuGameStatus.Rank;

        public ResultStatusMmf() : base("InfoReaderMmfResult")
        {
        }
    }
}