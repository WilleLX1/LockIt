using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Windows.Forms;

namespace ApplicationLocker
{
    public partial class Form1 : Form
    {
        LockedApp[] lockedApps = new LockedApp[0];

        public Form1()
        {
            InitializeComponent();
            MonitorProcessStart();
        }

        public void log(string text)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => txtLog.Text += text + Environment.NewLine));
            }
            else
            {
                txtLog.Text += text + Environment.NewLine;
            }
        }

        private void btnAddApp_Click(object sender, EventArgs e)
        {
            // Add Notepad to locked apps
            LockedApp lockedApp = new LockedApp(txtProcessName.Text, txtPassword.Text, this);
            lockedApp.Lock();

            // Add to array
            Array.Resize(ref lockedApps, lockedApps.Length + 1);
            lockedApps[lockedApps.Length - 1] = lockedApp;

            // Log
            log("App " + lockedApp.ProcessName + " added to locked apps");
        }

        private void MonitorProcessStart()
        {
            Task.Run(() =>
            {
                try
                {
                    using (ManagementEventWatcher startWatcher = new ManagementEventWatcher(
                        new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace")))
                    {
                        startWatcher.EventArrived += new EventArrivedEventHandler(ProcessStarted);
                        startWatcher.Start();

                        // Keep the watcher running
                        while (true)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log("Error monitoring process start: " + ex.Message);
                }
            });
        }

        private void ProcessStarted(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            foreach (var lockedApp in lockedApps)
            {
                if (lockedApp.IsLocked && processName.Equals(lockedApp.ProcessName + ".exe", StringComparison.OrdinalIgnoreCase))
                {
                    log("App " + lockedApp.ProcessName + " started");
                    Task.Run(() => RealTimeLock(lockedApp));
                }
            }
        }

        private Task RealTimeLock(LockedApp lockedApp)
        {
            if (lockedApp.IsLocked)
            {
                lockedApp.CloseApp();

                if (lockedApp.IsPromptingPassword)
                {
                    log("Password prompt already active for " + lockedApp.ProcessName);
                    return Task.CompletedTask;
                }

                lockedApp.IsPromptingPassword = true;

                string password = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter password for " + lockedApp.ProcessName,
                    "Password",
                    "");

                log("Password entered: " + password);

                lockedApp.UnlockApp(password);

                lockedApp.IsPromptingPassword = false;

            }
            return Task.CompletedTask;
        }

    }

    public class LockedApp
    {
        public string ProcessName { get; set; }
        public string PassHash { get; set; }
        public bool IsLocked { get; set; }
        public bool IsPromptingPassword { get; set; } = false;

        Form1 form; // Form1 instance

        public LockedApp(string ProcessName, string Pass, Form1 form)
        {
            PassHash = Crypto.Hash(Pass);
            // Log
            form.log("Password: " + Pass);
            form.log("Password hash: " + PassHash);
            this.ProcessName = ProcessName;
            this.PassHash = PassHash;
            this.form = form;
        }

        public void Lock()
        {
            CloseApp();
            IsLocked = true;
            // Log
            form.log("App " + ProcessName + " locked with password " + PassHash);
        }

        public void CloseApp()
        {
            int num = 0;
            Process[] processes = Process.GetProcessesByName(ProcessName);
            foreach (Process p in processes)
            {
                try
                {
                    p.Kill();
                    num++;
                    // Log
                    form.log("App " + p.ProcessName + " killed (PID: " + p.Id + ")");
                }
                catch (Exception ex)
                {
                    form.log("Error killing app " + p.ProcessName + ": " + ex.Message);
                }
            }
            if (num == 0)
            {
                // Log
                form.log("App " + ProcessName + " not running");
            }
        }

        public void UnlockApp(string password)
        {
            password = Crypto.Hash(password);
            // Log password
            form.log("Password hash: " + password);
            if (password == PassHash)
            {
                IsLocked = false;
                Process.Start(ProcessName);
                // Log
                form.log("App " + ProcessName + " unlocked");
                // Show message
                MessageBox.Show("App unlocked", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // App will continue to run
            }
            else
            {
                // Log
                form.log("Wrong password for app " + ProcessName);
                // Show message
                MessageBox.Show("Wrong password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
