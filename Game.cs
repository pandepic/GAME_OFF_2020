using ElementEngine;
using GAME_OFF_2020.GameStates;
using Veldrid;

namespace GAME_OFF_2020
{
    public class Game : BaseGame
    {
        protected GameStateMainMenu _mainMenuState = new GameStateMainMenu();

        public override void Load()
        {
            SettingsManager.LoadFromPath("Settings.xml");

            var windowRect = new ElementEngine.Rectangle()
            {
                X = 100,
                Y = 100,
                Width = SettingsManager.GetSetting<int>("Window", "Width"),
                Height = SettingsManager.GetSetting<int>("Window", "Height")
            };

            SetupWindow(windowRect, "Swords and Sorcery", GraphicsBackend.Direct3D11);
            SetupAssets();

            Window.Resizable = false;
            ClearColor = RgbaFloat.CornflowerBlue;

            //AnimationManager.LoadAnimations("Data/Animations.xml");
            InputManager.LoadGameControls();
            //CursorManager.SetCursor("normal", "Images/NormalCursor.png");

            SetGameState(_mainMenuState);
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
        }
    }
}
