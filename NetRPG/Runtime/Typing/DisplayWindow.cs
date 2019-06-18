
using Terminal.Gui;
using System.Collections.Generic;
using System.Linq;

namespace NetRPG.Runtime.Typing
{
    public class DisplayWindow : Window
    {
        public List<Key> EndWith;
        public Key EndedWith;
        public DisplayWindow(string text) : base(text)
        {
            EndWith = new List<Key>();
        }

        //https://github.com/bradphelan/loopdi/blob/c94c10782299948b2f8afafdf660be50600a9bfa/loopdi/Accelerator.cs

        public override bool ProcessColdKey(KeyEvent keyEvent)
        {
            if (EndWith.Contains(keyEvent.Key)) {
                EndedWith = keyEvent.Key;
                Application.RequestStop();
            }
            return base.ProcessColdKey(keyEvent);
        }
    }
}