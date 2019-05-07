using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysLogger.Forms
{
    public partial class AskFloat : Form
    {
        public AskFloat()
        {
            InitializeComponent();
        }
        AskFloatResult ans;
        public static AskFloatResult ShowDialog(string message, float defaultValue)
        {
            AskFloat aff = new Forms.AskFloat();
            aff.textBox1.Text = defaultValue.ToString();
            aff.label1.Text = message;
            aff.WindowState = FormWindowState.Maximized;
            aff.ans = new Forms.AskFloatResult();
            aff.ans.dr = DialogResult.Cancel;
            aff.ans.Value = defaultValue;
            aff.ShowDialog();
            return aff.ans;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ans.dr = DialogResult.Cancel;
            try
            {
                ans.Value = Convert.ToSingle(textBox1.Text);
                ans.dr = DialogResult.OK;
                Close();
            }
            catch
            {
                textBox1.BackColor = Color.LightPink;
                textBox1.ForeColor = Color.Red;
                textBox1.Focus();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.White;
            textBox1.ForeColor = Color.Black;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ans.dr = DialogResult.Cancel;
            Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ans.dr = DialogResult.OK;
                Close();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                ans.dr = DialogResult.Cancel;
                Close();
            }

        }
    }
    public class AskFloatResult
    {
        public DialogResult dr;
        public float Value;
    }
}
