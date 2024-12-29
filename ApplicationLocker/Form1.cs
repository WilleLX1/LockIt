using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

namespace ApplicationLocker
{
    public partial class Form1 : Form
    {
        LockedApp[] lockedApps = new LockedApp[0];
        private const string RegistryKeyPath = "SOFTWARE\\LockIt";
        private NotifyIcon notifyIcon;

        public Form1()
        {
            InitializeComponent();

            // Initialize NotifyIcon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Shield;
            notifyIcon.Text = "LockIt";
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            // Add context menu to NotifyIcon
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show", null, (s, e) => ShowForm());
            contextMenu.Items.Add("Hide", null, (s, e) => HideForm());
            contextMenu.Items.Add("Exit", null, (s, e) => ExitApplication());
            notifyIcon.ContextMenuStrip = contextMenu;

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

            LoadLockedAppsFromRegistry();

            MonitorProcessStart();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }
        
        private void ShowForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
        
        private void HideForm()
        {
            this.Hide();
        }
        
        private void ExitApplication()
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
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

        private void SaveLockedAppsToRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(RegistryKeyPath))
                {
                    foreach (var lockedApp in lockedApps)
                    {
                        key.SetValue(Crypto.base64Encode(lockedApp.ProcessName), Crypto.base64Encode(lockedApp.PassHash));
                    }
                }
                log("Locked apps saved to registry.");
            }
            catch (Exception ex)
            {
                log("Error saving to registry: " + ex.Message);
            }
        }

        private void DeleteLockedAppFromRegistry(string processName)
        {
            if (string.IsNullOrEmpty(processName))
            {
                log("Please enter a process name...");
                return;
            }

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(RegistryKeyPath, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue(Crypto.base64Encode(processName));
                        log("App " + processName + " removed from registry");
                    }
                    else
                    {
                        log("No saved locked apps found in registry.");
                    }
                }
            }
            catch (Exception ex)
            {
                log("Error deleting from registry: " + ex.Message);
            }
        }

        private void LoadLockedAppsFromRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(RegistryKeyPath))
                {
                    if (key != null)
                    {
                        // Log
                        log("Loading locked apps from registry...");
                        foreach (string processName in key.GetValueNames())
                        {
                            string decodedProcessName = Crypto.base64Decode(processName);
                            string passHash = key.GetValue(processName).ToString();
                            var lockedApp = new LockedApp(decodedProcessName, passHash, this, true);
                            Array.Resize(ref lockedApps, lockedApps.Length + 1);
                            lockedApps[lockedApps.Length - 1] = lockedApp;
                            checkedListBoxTargets.Items.Add(lockedApp.ProcessName);
                            checkedListBoxTargets.SetItemChecked(lockedApp.GetPlaceInList(), true);
                        }
                        log("Locked apps loaded from registry.");
                    }
                    else
                    {
                        log("No saved locked apps found in registry.");
                    }
                }
            }
            catch (Exception ex)
            {
                log("Error loading from registry: " + ex.Message);
            }
        }

        private void btnAddApp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProcessName.Text))
            {
                log("Please enter a process name...");
                return;
            }

            if (string.IsNullOrEmpty(txtPassword.Text))
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

            // Save to registry
            SaveLockedAppsToRegistry();
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
                    if (lockedApp.UnlockApp(password, true))
                    {
                        // Remove from array of locked apps
                        List<LockedApp> lockedAppsList = new List<LockedApp>(lockedApps);
                        lockedAppsList.Remove(lockedApp);
                        lockedApps = lockedAppsList.ToArray();
                        // Update list of locked apps
                        checkedListBoxTargets.Items.Remove(selected);
                        // Log
                        log("App " + selected + " removed from locked apps");

                        // Delete from registry
                        DeleteLockedAppFromRegistry(selected);
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

        Form1 form;

        public LockedApp(string processName, string passOrHash, Form1 form, bool isHash = false)
        {
            ProcessName = processName;
            PassHash = isHash ? Crypto.base64Decode(passOrHash) : Crypto.Hash(passOrHash);
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

        public bool UnlockApp(string password, bool removal = false)
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
                if (!removal)
                    MessageBox.Show("App unlocked (You are now allowed to run process)", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // App will continue to run
                return true;
            }
            else
            {
                // Log
                form.log("Wrong password for app " + ProcessName);
                // Log difference
                form.log(PassHash + " != " + password);
                // Show message
                MessageBox.Show("Wrong password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }
    }
}
