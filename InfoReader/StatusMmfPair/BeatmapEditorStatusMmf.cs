using osuTools.Game;

namespace InfoReaderPlugin.StatusMmfPair
{
    public class BeatmapEditorStatusMmf : StatusMmf
    {
        public override OsuGameStatus TargetStatus => OsuGameStatus.Editing;

        public BeatmapEditorStatusMmf() : base("InfoReaderMmfEditing")
        {
        }
    }
}