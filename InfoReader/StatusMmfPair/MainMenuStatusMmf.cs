using osuTools.Game;

namespace InfoReaderPlugin.StatusMmfPair
{
    public class MainMenuStatusMmf : StatusMmf
    {
        public override OsuGameStatus TargetStatus => OsuGameStatus.Idle;

        public MainMenuStatusMmf() : base("InfoReaderMmfMainMenu")
        {
        }
    }
}