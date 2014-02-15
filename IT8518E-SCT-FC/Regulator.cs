using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using SCTFanControl.Targets;
using System.Windows.Forms;

namespace SCTFanControl
{
    class Regulator
    {
        public delegate void RegulatorUpdateHandler(Regulator regulator, int temperature, int fanSpeed, int mode, string text);
        public event RegulatorUpdateHandler UpdateEvent;
        public delegate void RegulatorStoppedHandler(Regulator regulator, Exception e);
        //public event RegulatorStoppedHandler RegulatorStopped;

        private Boolean running;
        private IProfile _profile;
        private Thread thread;
        public Regulator()
        {
            running = true;
            thread = new Thread(new ThreadStart(loop));
            thread.Start();
        }

        public void Stop()
        {
            running = false;
        }

        public IProfile profile
        {
            get { return _profile; }
            set {
                _profile = value;
            }
        }

        private void loop()
        {
            ITarget target = new IT8518EFanControl();
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                long next = 0;

                while (running)
                {
                    try
                    {
                        /* 
                        // output average temp to tray icon for debugging
                        target.GetTemperature();
                        int temperature = target.GetAverageTemp();
                        */
                        
                        int temperature = target.GetTemperature(); // get max temp in celsius from EC
                        int fanSpeed = target.GetFanSpeed(); // get fan speed as WORD from EC
                        int mode = 0;

                        if (profile != null) // if profile is selected from menu or by default
                        {
                            bool auto = target.GetBiosControl(); // determine if fan is being controlled by EC
                            target.GetAverageTemp(); // compute average temp for monitoring in manual control
                            target.SetFanSpeed(profile.safe, profile.trip, profile.speed); // try to set fan speed according to profile

                            if (auto) { mode = 0;}
                            else { mode = 1;}
                            if (profile.speed == 0 && mode == 1) // bar won't colorize if silent mode used and fan has locked - append mode to tooltip
                            {
                                if (UpdateEvent != null) UpdateEvent(this, temperature, fanSpeed, mode, "Profile: " + profile.name + ", Mode: Locked\n" + target.Information);
                            }
                            else
                            {
                                if (UpdateEvent != null) UpdateEvent(this, temperature, fanSpeed, mode, "Profile: " + profile.name + "\n" + target.Information);
                            }
                        }
                        else // we should use automatic control from EC
                        {
                            target.SetBiosControl(true);
                            if (UpdateEvent != null) UpdateEvent(this, temperature, fanSpeed, mode, "Profile: Automatic\n"+target.Information);
                        }
                    }
                    catch (TimeoutException e)
                    {
                        System.Console.WriteLine(e.Message);
                    }
                    //System.Console.WriteLine("time: {0} ms", oStopWatch.ElapsedMilliseconds);
                    long now = stopwatch.ElapsedMilliseconds;
                    int interval = profile != null ? profile.intervalMs : 1000; // 1sec update interval unless profile defined otherwise
                    next += interval;
                    if (now > next)
                    {
                        System.Console.WriteLine("skipped {0} cycles", (now - next) / interval);
                        next = now;
                    }

                    Thread.Sleep((int)(next - now));
                }
            }
            catch (Exception e)
            {
                try
                {
                    target.SetBiosControl(true);
                }
                finally
                {
                    MessageBox.Show(e.ToString(), "Dell SCT IT8518E Fan Control", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                target.SetBiosControl(true);
            }
        }
    }
}
