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

            // Elevate
            Elevator elevator = new Elevator();
            if (!elevator.IsElevated())
            {
                if (elevator.Elevate())
                {
                    // Log
                    log("Elevated");
                    // Kill current process
                    Process.GetCurrentProcess().Kill();
                    return;
                }
                else
                {
                    // Log
                    log("Error elevating");
                }
            }
            else
            {
                // Log
                log("Already elevated");
            }

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
            // Check if process name is empty
            if (txtProcessName.Text == "")
            {
                log("Please enter a process name...");
                return;
            }

            // Check if password is empty
            if (txtPassword.Text == "")
            {
                log("Please enter a password...");
                return;
            }

            // Check if process name is already locked
            foreach (var _lockedApp in lockedApps)
            {
                if (_lockedApp.ProcessName == txtProcessName.Text)
                {
                    log("App " + txtProcessName.Text + " already added");
                    return;
                }
            }

            // Create object and lock app
            LockedApp lockedApp = new LockedApp(txtProcessName.Text, txtPassword.Text, this);
            lockedApp.Lock();

            // Add to array of locked apps
            Array.Resize(ref lockedApps, lockedApps.Length + 1);
            lockedApps[lockedApps.Length - 1] = lockedApp;

            // Update list of locked apps
            checkedListBoxTargets.Items.Add(lockedApp.ProcessName);
            // Change the item in checkedListBoxTargets
            checkedListBoxTargets.SetItemChecked(lockedApp.GetPlaceInList(), true);

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

                        // Log
                        log("Monitoring process started");

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

        private void btnRemoveApp_Click(object sender, EventArgs e)
        {
            // Check if item is selected
            if (checkedListBoxTargets.SelectedIndex == -1)
            {
                log("Please select an item to remove...");
                return;
            }

            // Get selected item
            string selected = checkedListBoxTargets.SelectedItem.ToString();

            if (!string.IsNullOrEmpty(selected))
            {
                // Find locked app
                LockedApp lockedApp = null;
                foreach (var _lockedApp in lockedApps)
                {
                    if (_lockedApp.ProcessName == selected)
                    {
                        lockedApp = _lockedApp;
                        break;
                    }
                }
                // Check if locked app was found
                if (lockedApp != null)
                {
                    // Ask for password
                    string password = Microsoft.VisualBasic.Interaction.InputBox(
                        "Enter password for " + lockedApp.ProcessName,
                        "Password",
                        "");

                    // Unlock app
                    if (lockedApp.UnlockApp(password))
                    {
                        // Remove from array of locked apps
                        List<LockedApp> lockedAppsList = new List<LockedApp>(lockedApps);
                        lockedAppsList.Remove(lockedApp);
                        lockedApps = lockedAppsList.ToArray();
                        // Update list of locked apps
                        checkedListBoxTargets.Items.Remove(selected);
                        // Log
                        log("App " + selected + " removed from locked apps");
                    }
                }
                else
                {
                    // Log
                    log("App " + selected + " not found in locked apps");
                }
            }
        }


        private void checkedListBoxTargets_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Log
            log("Selected index: " + checkedListBoxTargets.SelectedIndex);
        }

        private void checkedListBoxTargets_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Check if item is checked
            if (e.NewValue == CheckState.Checked)
            {
                // Log
                log("Item checked: " + checkedListBoxTargets.Items[e.Index].ToString());
                // Find locked app
                LockedApp lockedApp = null;
                foreach (var _lockedApp in lockedApps)
                {
                    if (_lockedApp.ProcessName == checkedListBoxTargets.Items[e.Index].ToString())
                    {
                        lockedApp = _lockedApp;
                        break;
                    }
                }
                // Check if locked app was found
                if (lockedApp != null)
                {
                    // Check if app is locked
                    if (lockedApp.IsLocked)
                    {
                        // Log
                        log("App " + lockedApp.ProcessName + " already locked");
                        return;
                    }
                    // Lock app
                    lockedApp.Lock();
                    // Log
                    log("App " + lockedApp.ProcessName + " added to locked apps");
                }
                else
                {
                    // Log
                    log("App " + checkedListBoxTargets.Items[e.Index].ToString() + " not found in locked apps");
                }
            }
            else
            {
                // Log
                log("Item unchecked: " + checkedListBoxTargets.Items[e.Index].ToString());
                // Find locked app
                LockedApp lockedApp = null;
                foreach (var _lockedApp in lockedApps)
                {
                    if (_lockedApp.ProcessName == checkedListBoxTargets.Items[e.Index].ToString())
                    {
                        lockedApp = _lockedApp;
                        break;
                    }
                }
                // Check if locked app was found
                if (lockedApp != null)
                {
                    // Check if app is locked
                    if (lockedApp.IsLocked)
                    {
                        // Ask for password
                        string password = Microsoft.VisualBasic.Interaction.InputBox(
                            "Enter password for " + lockedApp.ProcessName,
                            "Password",
                            "");
                        // Unlock app
                        if (lockedApp.UnlockApp(password))
                        {
                            // Change is locked to false
                            lockedApp.IsLocked = false;
                            // Log
                            log("App " + lockedApp.ProcessName + " unlocked");
                        }
                        else
                        {
                            // Log
                            log("App " + lockedApp.ProcessName + " not unlocked");
                            // Change the item in checkedListBoxTargets
                            checkedListBoxTargets.SetItemChecked(lockedApp.GetPlaceInList(), true);
                            e.NewValue = CheckState.Checked;
                            checkedListBoxTargets_ItemCheck(sender, e);
                        }
                    }
                    else
                    {
                        // Log
                        log("App " + lockedApp.ProcessName + " not locked");
                    }
                }
                else
                {
                    // Log
                    log("App " + checkedListBoxTargets.Items[e.Index].ToString() + " not found in locked apps");
                }
            }
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
        public int GetPlaceInList()
        {
            // Go through all items in checkedListBoxTargets
            for (int i = 0; i < form.checkedListBoxTargets.Items.Count; i++)
            {
                // Check if item is the same as the process name
                if (form.checkedListBoxTargets.Items[i].ToString() == ProcessName)
                {
                    return i;
                }
            }
            return -1;
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

        public bool UnlockApp(string password)
        {
            password = Crypto.Hash(password);
            // Log password
            form.log("Password hash: " + password);
            if (password == PassHash)
            {
                IsLocked = false;
                // Log
                form.log("App " + ProcessName + " unlocked");
                // Change the item in checkedListBoxTargets
                form.checkedListBoxTargets.SetItemChecked(GetPlaceInList(), false);
                // Show message
                MessageBox.Show("App unlocked (You are now allowed to run process)", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // App will continue to run
                return true;
            }
            else
            {
                // Log
                form.log("Wrong password for app " + ProcessName);
                // Show message
                MessageBox.Show("Wrong password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }
    }
}
