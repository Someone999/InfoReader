using RealTimePPDisplayer;
using RealTimePPDisplayer.Displayer;

namespace InfoReaderPlugin
{
    public class RtppdInfo : DisplayerBase
    {
        private PPTuple _tuple,_refTuple;
        public PPTuple SmoothPP => _tuple;
        public override void FixedDisplay(double time) => _tuple = SmoothMath.SmoothDampPPTuple(_tuple, Pp, ref _refTuple, 0.033);
    }
}