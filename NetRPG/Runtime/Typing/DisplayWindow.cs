
using Terminal.Gui;
using System.Linq;

namespace NetRPG.Runtime.Typing
{
    public class DisplayWindow : Window
    {
        private static Key[] EndWith = new[] {Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.ControlJ, Key.PageDown, Key.PageUp, Key.Esc};
        public Key EndedWith;
        public DisplayWindow(string text) : base(text)
        {
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