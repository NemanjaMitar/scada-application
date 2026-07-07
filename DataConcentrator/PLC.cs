using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PLCSimulator;

namespace DataConcentrator
{
    public static class PLC
    {
        private static PLCSimulatorManager instance;
        private static readonly object lockObj = new object();

        public static object LockObject => lockObj;

        public static PLCSimulatorManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new PLCSimulatorManager();
                            instance.StartPLCSimulator();
                        }
                    }
                }
                return instance;
            }
        }

        public static void StopSimulator()
        {
            instance?.Abort();
            instance = null;
        }
    }
}
