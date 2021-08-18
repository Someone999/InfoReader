using osuTools.Game;

namespace InfoReaderPlugin.StatusMmfPair
{
    public class MulplayerGameLobbyStatusMmf : StatusMmf
    {
        public override OsuGameStatus TargetStatus => OsuGameStatus.Lobby;

        public MulplayerGameLobbyStatusMmf() : base("InfoReaderMmfLobby")
        {
        }
    }
}