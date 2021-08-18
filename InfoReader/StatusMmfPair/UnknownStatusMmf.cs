using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osuTools.Game;

namespace InfoReaderPlugin.StatusMmfPair
{
    public class UnknownStatusMmf:StatusMmf
    {
        public override OsuGameStatus TargetStatus => OsuGameStatus.Unkonwn;

        public UnknownStatusMmf() : base(null)
        {
        }
    }
}
