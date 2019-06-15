using Terminal.Gui;

namespace NetRPG.Runtime
{
    public class WindowHandler {
        public static void Init() {
            Application.Init();
            Colors.Base.Normal = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);
            Colors.Base.Focus = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);
        }

        public static void Add(View window) {
            Application.Top.Add (window);
        }

        public static void Run() {
            Application.Run(Application.Top);
        }

        public static void End() {
            Application.RequestStop();
        }
    }
}