using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Dashboard
{
    class Lap
    {
        public float time { get; set; }
        public float lapTime { get; set; }
        public Lap(F1Data data)
        {
            time = (float)data.Get("time");
            lapTime = (float)data.Get("lapTime");
        }

        public override string ToString() {
            return String.Format("time: {0}, lapTime: {1}", Formatter.Seconds(time), Formatter.Seconds(lapTime));
        }
    }
}
