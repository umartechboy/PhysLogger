using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace MagneticPendulum
{
    public class Button2:Button
    {
        public Button2()
        {
            MouseEnter += Button2_MouseEnter;
            MouseLeave += Button2_MouseLeave;
            BackgroundImageChanged += Button2_BackgroundImageChanged;
        }

        private void Button2_BackgroundImageChanged(object sender, EventArgs e)
        {
            back = BackgroundImage;
        }

        private void Button2_MouseLeave(object sender, EventArgs e)
        {
            BackgroundImageChanged -= Button2_BackgroundImageChanged;
            BackgroundImage = back;
            BackgroundImageChanged += Button2_BackgroundImageChanged;
        }

        private void Button2_MouseEnter(object sender, EventArgs e)
        {
            BackgroundImageChanged -= Button2_BackgroundImageChanged;
            BackgroundImage = HoverBackgroundImage;
            BackgroundImageChanged += Button2_BackgroundImageChanged;
        }

        System.Drawing.Image back;
        public System.Drawing.Image HoverBackgroundImage { get; set; }
    }
}
