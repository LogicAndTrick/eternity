using Eternity.Controls;
using Eternity.Controls.Layouts;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public abstract class MenuElement : LayoutControl
    {
        public MenuScreen Screen { get; set; }

        public bool Focuses { get; set; }
        public bool ChangesScreen { get; set; }

        protected MenuElement(MenuScreen screen, ILayout layout) : base(layout)
        {
            Screen = screen;
        }

        public override void OnMouseEnter(Input.EternityEvent e)
        {
            if (Focuses)
            {
                Screen.OnFocusControl(this);
            }
        }

        public override void OnMouseDown(Input.EternityEvent e)
        {
            if (ChangesScreen)
            {
                Screen.OnChangeScreen(GetScreen());
            }
        }

        protected abstract MenuScreen GetScreen();
    }
}
