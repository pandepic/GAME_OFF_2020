using ElementEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace GAME_OFF_2020.GameStates
{
    public class GameStateMainMenu : GameState, IPUIEventHandler
    {
        protected PUIMenu _menu;

        public override void Load()
        {
            if (_menu == null)
            {
                _menu = new PUIMenu();
                //_menu.Load("UI/MainMenu.xml", "UI/_Templates.xml");
            }

            _menu.AddPUIEventHandler(this);
        }

        public override void Unload()
        {
            _menu.RemovePUIEventHandler(this);
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
        }

        public void HandlePUIEvent(PUIMenu source, PUIEventType type, PUIWidget widget)
        {
        }
    }
}
