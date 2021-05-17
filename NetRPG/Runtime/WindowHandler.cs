using Terminal.Gui;
using System.Linq;

namespace NetRPG.Runtime
{

    public class WindowHandler {
        private static Typing.DisplayWindow Window;
        private static Application.RunState State;
        public static void Init() {
            Application.Init();
            Colors.Base.Normal = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);
            Colors.Base.Focus = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);

            if (Window == null) {
                Window = new Typing.DisplayWindow ("NetRPG") {
                    X = 0,
                    Y = 0,
                    Width = 83,
                    Height = 27
                };
                
                Window.ColorScheme = Colors.Base;
            }

            State = Application.Begin(Application.Top);
        }

        public static void Add(View item) {
            Window.Add(item);
        }

        public static void SetKeys(Key[] keys) {
            Window.EndWith = keys.ToList();
        }

        public static Key Run() {
            Window.EndWith.Add(Key.Enter);

            Application.Run(Window);
            Window.RemoveAll();
            Window.EndWith.Clear();

            return Window.EndedWith;
        }

        public static void End() {
            Application.RequestStop();
            Application.End(State);
        }
    }
}