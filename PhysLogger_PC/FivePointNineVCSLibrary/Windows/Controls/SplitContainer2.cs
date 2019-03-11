using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace FivePointNine.Windows.Controls
{
    public class SplitContainer2:SplitContainer
    {
        public SplitContainer2()
        {
            MouseDown += splitCont_MouseDown;
            MouseUp += splitCont_MouseUp;
            MouseMove += splitCont_MouseMove;
        }
        //assign this to the SplitContainer's MouseDown event
        private void splitCont_MouseDown(object sender, MouseEventArgs e)
        {
            // This disables the normal move behavior
            this.IsSplitterFixed = true;
        }

        //assign this to the SplitContainer's MouseUp event
        private void splitCont_MouseUp(object sender, MouseEventArgs e)
        {
            // This allows the splitter to be moved normally again
            this.IsSplitterFixed = false;
        }

        //assign this to the SplitContainer's MouseMove event
        private void splitCont_MouseMove(object sender, MouseEventArgs e)
        {
            // Check to make sure the splitter won't be updated by the
            // normal move behavior also
            if (this.IsSplitterFixed)
            {
                // Make sure that the button used to move the splitter
                // is the left mouse button
                if (e.Button.Equals(MouseButtons.Left))
                {
                    // Checks to see if the splitter is aligned Vertically
                    if (this.Orientation.Equals(Orientation.Vertical))
                    {
                        // Only move the splitter if the mouse is within
                        // the appropriate bounds
                        if (e.X > 0 && e.X < this.Width)
                        {
                            // Move the splitter & force a visual refresh
                            this.SplitterDistance = e.X;
                            this.Refresh();
                        }
                    }
                    // If it isn't aligned vertically then it must be
                    // horizontal
                    else
                    {
                        // Only move the splitter if the mouse is within
                        // the appropriate bounds
                        if (e.Y > 0 && e.Y < this.Height)
                        {
                            // Move the splitter & force a visual refresh
                            this.SplitterDistance = e.Y;
                            this.Refresh();
                        }
                    }
                }
                // If a button other than left is pressed or no button
                // at all
                else
                {
                    // This allows the splitter to be moved normally again
                    this.IsSplitterFixed = false;
                }
            }
        }
    }

}
