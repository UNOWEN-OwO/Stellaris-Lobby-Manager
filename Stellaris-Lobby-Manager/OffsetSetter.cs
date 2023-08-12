using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stellaris_Lobby_Manager
{
    public partial class OffsetSetter : Form
    {
        public OffsetSetter()
        {
            InitializeComponent();
            StartUp();
        }

        private void StartUp()
        {
            foreach (var control in Controls.Find("offsetPanel", true).FirstOrDefault().Controls)
            {
                if (control is NumericUpDown)
                {
                    var offset = (NumericUpDown)control;
                    offset.Tag = 1;
                    offset.Value = Convert.ToDecimal(Properties.Settings.Default[offset.Name]);
                    offset.Tag = 0;
                }
            }
        }

        private void OnOffsetEdit(object sender, EventArgs e)
        {
            var offset = (NumericUpDown)sender;
            if ((int)offset.Tag == 0)
            {
                Properties.Settings.Default.Save();
                Properties.Settings.Default[offset.Name] = (int)offset.Value;
            }
        }
    }
}
