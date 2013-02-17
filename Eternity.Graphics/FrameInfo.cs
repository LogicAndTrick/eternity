using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Eternity.Graphics
{
    public class FrameInfo
    {
        private static readonly Stopwatch Timer;
        private static long _frameCount;
        private static long _lastTick;

        static FrameInfo()
        {
            Timer = new Stopwatch();
            Timer.Start();
            _frameCount = 0;
            _lastTick = 0;
        }

        public static FrameInfo Create()
        {
            var tick = Timer.ElapsedMilliseconds;
            var elapsed = tick - _lastTick;
            var fi = new FrameInfo(elapsed, tick, _frameCount);
            _lastTick = tick;
            _frameCount++;
            return fi;
        }

        public long MillisecondsSinceLastFrame { get; private set; }
        public long TotalMilliseconds { get; private set; }
        public long FrameNumber { get; private set; }

        private FrameInfo(long millis, long totalMillis, long num)
        {
            MillisecondsSinceLastFrame = millis;
            FrameNumber = num;
            TotalMilliseconds = totalMillis;
        }
    }
}
