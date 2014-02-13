using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SCTFanControl.Targets
{
    class EmbeddedController
    {


        protected byte ReadEC(byte addr)
        {
            WaitWriteEC();
            WritePort(0x66, 0x80);
            WaitWriteEC();
            WritePort(0x62, addr);
            WaitReadEC();
            return ReadPort(0x62);
        }

        protected void WriteEC(byte addr, byte value)
        {
            WaitWriteEC();
            WritePort(0x66, 0x81);
            WaitWriteEC();
            WritePort(0x62, addr);
            WaitWriteEC();
            WritePort(0x62, value);
        }
        
        
        [DllImport("TVicPort.dll", EntryPoint = "OpenTVicPort", ExactSpelling = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint OpenTVicPort();
        [DllImport("TVicPort.dll", EntryPoint = "CloseTVicPort", ExactSpelling = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern void CloseTVicPort();
        [DllImport("TVicPort.dll", EntryPoint = "IsDriverOpened", ExactSpelling = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint IsDriverOpened();
        [DllImport("TVicPort.dll", EntryPoint = "WritePort", ExactSpelling = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern void WritePort(ushort PortAddr, byte bValue);
        [DllImport("TVicPort.dll", EntryPoint = "ReadPort", ExactSpelling = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern byte ReadPort(ushort PortAddr);

        private void WaitWriteEC()
        {
            if (IsDriverOpened() != 1) OpenTVicPort();
            Stopwatch oStopWatch = new Stopwatch();
            oStopWatch.Start();
            while ((ReadPort(0x66) & 0x02) != 0)
            {
                if (oStopWatch.ElapsedMilliseconds > 1000)
                {
                    throw new TimeoutException("EC timed out, while waiting for WBF to clear");
                }
                //System.Threading.Thread.Sleep(0);
            }
            oStopWatch.Stop();
        }

        private void WaitReadEC()
        {
            if (IsDriverOpened() != 1) OpenTVicPort();
            Stopwatch oStopWatch = new Stopwatch();
            oStopWatch.Start();
            while ((ReadPort(0x66) & 0x01) == 0)
            {
                if (oStopWatch.ElapsedMilliseconds > 1000)
                {
                    throw new TimeoutException("EC timed out, while waiting for RBF to set");
                }
                //System.Threading.Thread.Sleep(0);
            }
            oStopWatch.Stop();
        }


    
    }
}
