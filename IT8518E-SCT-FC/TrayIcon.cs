using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace SCTFanControl
{

    class TrayIcon : ApplicationContext
    {
    
        private NotifyIcon notifyIcon;
        private IContainer components;
        private ContextMenu contextMenu;
        private Regulator regulator;
        Config config;
        
        public TrayIcon(Regulator regulator)
        {
            components = new System.ComponentModel.Container();
            notifyIcon = new NotifyIcon(this.components);

            this.regulator = regulator;
            config = new Config("config.xml");
            regulator.profile = config.defaultProfile;

            //context menu
            contextMenu = new ContextMenu();
            foreach (IProfile profile in config.profiles)
            {
                MenuItem menuItem = new MenuItem(profile.name);
                menuItem.Checked = config.defaultProfile == profile;
                menuItem.Click += delegate(Object sender, EventArgs e)
                    {
                        regulator.profile = profile;
                        foreach (MenuItem other in contextMenu.MenuItems) other.Checked = false;
                        menuItem.Checked = true;
                    };
                contextMenu.MenuItems.Add(menuItem);
            }
            contextMenu.MenuItems.Add("-");
            MenuItem bios = new MenuItem("Automatic");
            bios.Checked = config.defaultProfile == null;
            bios.Click += delegate(Object sender, EventArgs e)
                {
                    regulator.profile = null;
                    foreach (MenuItem other in contextMenu.MenuItems) other.Checked = false;
                    bios.Checked = true;
                };
            contextMenu.MenuItems.Add(bios);
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add("Exit", OnExitClicked);
            notifyIcon.ContextMenu = contextMenu;

            RenderIcon(-1,0,0);
            notifyIcon.Visible = true;
            regulator.UpdateEvent += UpdateView;
        }

        public void UpdateView(Regulator regulator, int fanSpeed, int temperature, int mode, String status)
        {
            RenderIcon((int)fanSpeed, (int)temperature, (int)mode);
            notifyIcon.Text = status; //this is limited to 64 characters only
        }


        
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle); 

        private void RenderIcon(int temp, int fan, int mode)
        {
            SolidBrush brush;
            brush = new SolidBrush(Color.White);
            if (mode == 0) { brush = new SolidBrush(Color.FromArgb(0,238,0));} // in auto mode the bar will be green
            if (mode == 1) { brush = new SolidBrush(Color.White); } // if fan is locked it will be white. peach orange is 255,127,0
            using (brush)

            using (Bitmap bitmap = Properties.Resources.icon16)
            using (Bitmap font = Properties.Resources.font16)
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                if (temp >= 0 && temp <= 99)
                {
                    graphics.DrawImage(font, new Rectangle(0, 0, 8, 11), new Rectangle(((temp / 10) % 10) * 8, 0, 8, 11), GraphicsUnit.Pixel);
                    graphics.DrawImage(font, new Rectangle(8, 0, 8, 11), new Rectangle((temp % 10) * 8, 0, 8, 11), GraphicsUnit.Pixel);
                }
                else
                {
                    graphics.DrawImage(font, new Rectangle(0, 0, 8, 11), new Rectangle(80, 0, 8, 11), GraphicsUnit.Pixel);
                    graphics.DrawImage(font, new Rectangle(8, 0, 8, 11), new Rectangle(80, 0, 8, 11), GraphicsUnit.Pixel);
                }

                /*
                 * Dell fans utilize 3 levels of speed when they are controled from EC
                 * Typically, the maximums speed a fan will run at is 5300 rpm according to ePSA tests
                 */

                graphics.FillRectangle(brush, 1, 12, fan * 14 / 5300, 3);
                Icon prevIcon = notifyIcon.Icon;
                notifyIcon.Icon = Icon.FromHandle(bitmap.GetHicon());
                if(prevIcon != null) DestroyIcon(prevIcon.Handle);
            }
        }

        private void OnExitClicked(Object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            this.Dispose();
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.notifyIcon.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
