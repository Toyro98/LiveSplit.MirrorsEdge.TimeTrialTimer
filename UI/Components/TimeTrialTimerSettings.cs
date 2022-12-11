using LiveSplit.ComponentUtil;
using LiveSplit.TimeFormatters;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class TimeTrialTimerSettings : UserControl
    {
        public bool Display2Rows { get; set; }
        public LayoutMode Mode { get; set; }
        public TimeFormatter Formatter { get; set; }
        public DeepPointer TimePointer { get; set; }

        public TimeTrialTimerSettings()
        {
            InitializeComponent();

            Display2Rows = false;
            Formatter = new TimeFormatter(TimeAccuracy.Hundredths);
        }

        public void GetGameVersion(Process process)
        {
            try
            {
                switch (process.MainModule.ModuleMemorySize)
                {
                    case 32976896: // Steam
                    case 32632832: // GoG
                        TimePointer = new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x424);
                        break;

                    case 60298504: // Reloaded
                    case 42876928: // Dvd
                        TimePointer = new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x424);
                        break;

                    case 42889216: // Origin
                        TimePointer = new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x424);
                        break;

                    case 43794432: // Origin DLC
                        TimePointer = new DeepPointer(0x01C1BE24, 0x50, 0x1E0, 0x318, 0x424);
                        break;

                    case 42717184: // Origin Asia
                        TimePointer = new DeepPointer(0x01BE6134, 0x50, 0x1E0, 0x318, 0x424);
                        break;

                    default:
                        MessageBox.Show("Unsupported version!", "Memory Size: " + process.MainModule.ModuleMemorySize);
                        break;
                }
            }
            catch (Win32Exception e)
            {
                MessageBox.Show("Win32Exception", e.Message);
            }
        }

        private void TimeTrialTimerSettings_Load(object sender, EventArgs e)
        {
            if (Mode == LayoutMode.Horizontal)
            {
                chkTwoRows.Enabled = false;
                chkTwoRows.DataBindings.Clear();
                chkTwoRows.Checked = true;
            }
            else
            {
                chkTwoRows.Enabled = true;
                chkTwoRows.DataBindings.Clear();
                chkTwoRows.DataBindings.Add("Checked", this, "Display2Rows", false, DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.0") ^
            SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"], false);
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }
    }
}
