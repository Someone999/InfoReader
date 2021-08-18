using osuTools.Game;

namespace InfoReaderPlugin.StatusMmfPair
{
    public class MulplayerGameRoomStatusMmf : StatusMmf
    {
        public override OsuGameStatus TargetStatus => OsuGameStatus.MatchSetup;

        public MulplayerGameRoomStatusMmf() : base("InfoReaderMmfGameRoom")
        {
        }
    }
}