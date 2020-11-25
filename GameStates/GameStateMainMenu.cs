using ElementEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace GAME_OFF_2020.GameStates
{
    public class GameStateMainMenu : GameState, IUIEventHandler
    {
        protected UIMenu _menu;

        public override void Load()
        {
            if (_menu == null)
            {
                _menu = new UIMenu();
                //_menu.Load("UI/MainMenu.xml", "UI/_Templates.xml");
            }

            _menu.AddUIEventHandler(this);
        }

        public override void Unload()
        {
            _menu.RemoveUIEventHandler(this);
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
        }

        public void HandleUIEvent(UIMenu source, UIEventType type, UIWidget widget)
        {
        }
    }
}
