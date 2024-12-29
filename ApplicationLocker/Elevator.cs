using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLocker
{
    public class Elevator
    {
        public bool IsElevated()
        {
            return new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent()).IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        public bool Elevate()
        {
            if (!IsElevated())
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
                startInfo.Verb = "runas";
                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                    return true;
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("The application could not be elevated. Please run the application as an administrator.");
                    return false;
                }
            }
            else
            {
                return true;
            }

        }
    }
}
