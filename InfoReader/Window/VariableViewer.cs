using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using InfoReaderPlugin.MemoryMapWriter;
using osuTools.Attributes;
using osuTools.OrtdpWrapper;

namespace InfoReaderPlugin.Window
{
    public partial class VariableViewer : Form
    {
        readonly OrtdpWrapper _ortdpWrapper;
        readonly System.Timers.Timer _tm = new System.Timers.Timer();
        readonly List<string> _propertyStrList = new List<string>();
        int SortFunc(string a,string b)
        {
            return String.CompareOrdinal(a, b);
        }
        void GetAllProperties(object target)
        {
            foreach (PropertyInfo property in target.GetType().GetProperties())
            {
                try
                {
                    object obj = property.GetValue(target);
                    if (obj != null)
                    {
                        if (property.GetCustomAttribute<AvailableVariableAttribute>() is AvailableVariableAttribute attr)
                        {
                            _propertyStrList.Add(attr.VariableName);
                            if (property.PropertyType.FullName != null && property.PropertyType.FullName.StartsWith("osuTools."))
                            {
                                GetAllProperties(obj);
                            }
                        }

                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }   
        }
        public VariableViewer(osuTools.OrtdpWrapper.OrtdpWrapper ortdp)
        {
            InitializeComponent();
            textBox1.ReadOnly = true;
            _ortdpWrapper = ortdp;
            _tm.Elapsed += Tm_Elapsed;
            _tm.AutoReset = true;
            _tm.Enabled = true;
            GetAllProperties(_ortdpWrapper);
            _propertyStrList.Sort(SortFunc);
            comboBox1.DataSource = _propertyStrList;
        }

        private void Tm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            VariableExpression expression = new VariableExpression(comboBox1.Text,_ortdpWrapper);
            textBox1.Text = expression.GetProcessedValue().ToString();

        }

        

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            
            Hide();
        }

        private void VariableViewer_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
                _tm.Start();
            else
                _tm.Stop();
        }
    }
}
