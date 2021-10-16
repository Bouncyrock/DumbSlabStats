using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DumbSlabStats
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs evtArgs)
        {
            var str = slabTextBox.Text;

            if (string.IsNullOrWhiteSpace(str))
            {
                return;
            }
            else
            {
                try
                {
                    var slab = SlabV2.DecodeString(str);
                    var sb = new StringBuilder();
                    sb.AppendLine("layouts count: " + slab.Layouts.Count);


                    foreach (var layout in slab.Layouts)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"Layout: AssetKindId={layout.LayoutData.AssetKindId} AssetCount={layout.LayoutData.AssetCount}");
                    }
                    reportTextBox.Text = sb.ToString();
                }
                catch (Exception e)
                {
                    reportTextBox.Text = e.Message;
                }
            }            
        }
    }
}
