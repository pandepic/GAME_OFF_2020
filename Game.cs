using ElementEngine;
using GAME_OFF_2020.GameStates;
using System.Collections.Generic;
using Veldrid;

namespace GAME_OFF_2020
{
    public enum GameStateType
    {
        MainMenu,
        Settings,
        Play
    }

    public class Game : BaseGame
    {
        protected Dictionary<GameStateType, GameState> _gameStates = new Dictionary<GameStateType, GameState>()
        {
            { GameStateType.MainMenu, new GameStateMainMenu() },
            { GameStateType.Settings, new GameStateSettings() },
            { GameStateType.Play, new GameStatePlay() },
        };

        public override void Load()
        {
            Globals.Game = this;
            SettingsManager.LoadFromPath("Settings.xml");

            var windowRect = new ElementEngine.Rectangle()
            {
                X = 100,
                Y = 100,
                Width = SettingsManager.GetSetting<int>("Window", "Width"),
                Height = SettingsManager.GetSetting<int>("Window", "Height")
            };

            var graphicsBackend = GraphicsBackend.Direct3D11;

#if OPENGL
            graphicsBackend = GraphicsBackend.OpenGL;
#endif

            SetupWindow(windowRect, "Captain Shostakovich", graphicsBackend);
            SetupAssets();

            Window.Resizable = false;
            ClearColor = RgbaFloat.Black;

            AnimationManager.LoadAnimations("Animations.xml");
            InputManager.LoadGameControls();
            //CursorManager.SetCursor("normal", "Images/NormalCursor.png");

            SetGameState(GameStateType.Play);
        }

        public void SetGameState(GameStateType type)
        {
            SetGameState(_gameStates[type]);
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
        }
    }
}
