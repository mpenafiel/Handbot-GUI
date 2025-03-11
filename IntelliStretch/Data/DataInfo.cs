using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelliStretch.Data
{
    public class DataInfo
    {
        public enum DataMode
        {
            Position = 0,
            Torque = 1
        }

        public enum TorqueMode
        {
            Extension = 0,
            Flexion = 1
        }
    }
}
