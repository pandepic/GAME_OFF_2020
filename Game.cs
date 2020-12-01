using ElementEngine;
using ElementEngine.Tiled;
using GAME_OFF_2020.GameStates;
using System;
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

    public enum SoundType
    {
        Music,
    }

    public class Game : BaseGame
    {
        protected Dictionary<GameStateType, GameState> _gameStates = new Dictionary<GameStateType, GameState>()
        {
            { GameStateType.MainMenu, new GameStateMainMenu() },
            { GameStateType.Settings, new GameStateSettings() },
            { GameStateType.Play, new GameStatePlay() },
        };

        public TiledMap Map;
        public Camera2D BackgroundCamera;
        public TileBatch2D BackgroundStars;
        public Random RNG = new Random();

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

            SoundManager.SetMasterVolume(SettingsManager.GetSetting<float>("Sound", "MasterVolume"));
            SoundManager.SetVolume((int)SoundType.Music, SettingsManager.GetSetting<float>("Sound", "MusicVolume"));

#if RELEASE || RELEASEOGL
            SoundManager.Play("CaptainShostakovich.ogg", (int)SoundType.Music, AudioSourceType.Auto, true);
#endif

            GameConfig.Load();
            Map = AssetManager.LoadTiledMap("Ship.tmx");

            BackgroundCamera = new Camera2D(new ElementEngine.Rectangle(0, 0, ElementGlobals.TargetResolutionWidth, ElementGlobals.TargetResolutionHeight))
            {
                Zoom = 3
            };

            BackgroundStars = new TileBatch2D(4000, 200, Map.TileSize.X, Map.TileSize.Y, AssetManager.LoadTexture2D("SubwayShip.png"), TileBatch2DWrapMode.Both);
            BackgroundStars.BeginBuild();
            for (var y = 0; y < BackgroundStars.MapHeight; y++)
            {
                for (var x = 0; x < BackgroundStars.MapWidth; x++)
                {
                    var rng = RNG.Next(0, 100);

                    if (rng > 2)
                        BackgroundStars.SetTileAtPosition(x, y, 16);
                    else if (rng == 1)
                        BackgroundStars.SetTileAtPosition(x, y, 32);
                    else
                        BackgroundStars.SetTileAtPosition(x, y, 48);
                }
            }
            BackgroundStars.EndBuild();

            SetGameState(GameStateType.MainMenu);
        }

        public void SetGameState(GameStateType type)
        {
            SetGameState(_gameStates[type]);
        }

        public override void Update(GameTimer gameTimer)
        {
            BackgroundCamera.X += GameConfig.BackgroundSpeed * gameTimer.DeltaS;
            BackgroundCamera.Update(gameTimer);
        }

        public override void Draw(GameTimer gameTimer)
        {
            BackgroundStars.DrawAll(BackgroundCamera.Position.ToVector2I(), 2);
        }
    }
}
