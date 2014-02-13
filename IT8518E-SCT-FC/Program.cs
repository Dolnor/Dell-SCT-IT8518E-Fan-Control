using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCTFanControl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Regulator regulator = null;
            try
            {
                regulator = new Regulator();
                TrayIcon context = new TrayIcon(regulator);
                Application.Run(context);
                regulator.Stop();
            }
            catch (Exception e)
            {
                if (regulator != null) regulator.Stop();
                MessageBox.Show(e.ToString(), "Dell SCT IT8518E Fan Control", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
