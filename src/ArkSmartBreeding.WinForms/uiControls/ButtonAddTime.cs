using System;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    class ButtonAddTime : Button
    {
        public TimeSpan timeSpan;

        public delegate void addTimerEventHandler(TimeSpan timeSpan);

        public event addTimerEventHandler addTimer;

        public ButtonAddTime()
        {
            timeSpan = new TimeSpan(0, 1, 0);
            Click += ButtonAddTime_Click;
        }

        private void ButtonAddTime_Click(object sender, EventArgs e)
        {
            addTimer(timeSpan);
        }
    }
}
