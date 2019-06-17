using Terminal.Gui;

namespace NetRPG.Runtime
{

    public class WindowHandler {
        private static Typing.DisplayWindow Window;
        public static void Init() {
            Application.Init();
            Colors.Base.Normal = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);
            Colors.Base.Focus = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);

            if (Window == null) {
                Window = new Typing.DisplayWindow ("NetRPG") {
                    X = 0,
                    Y = 0,
                    Width = 80,
                    Height = 24
                };
                
                Window.ColorScheme = Colors.Base;
            }
        }

        public static void Add(View item) {
            Window.Add(item);
        }

        public static void Run() {
            Application.Top.Add(Window);
            Application.Run(Window);
            Window.RemoveAll();
        }

        public static void End() {
            Application.RequestStop();
        }
    }
}