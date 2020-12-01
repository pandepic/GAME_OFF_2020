using ElementEngine;
using ElementEngine.Tiled;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Linq;

namespace GAME_OFF_2020.GameStates
{
    public class GameStatePlay : GameState
    {
        public static SpriteFont DefaultFont { get; private set; }
        public static Camera2D Camera;

        public CharacterManager CharacterManager => Globals.CharacterManager;
        public DialogueManager DialogueManager => Globals.DialogueManager;
        public PlayerCharacter Player => CharacterManager.Player;

        public TiledMapRenderer MapRenderer;

        public SpriteBatch2D SpriteBatch;

        public override void Load()
        {
            SpriteBatch = new SpriteBatch2D();
            DefaultFont = AssetManager.LoadSpriteFont("VT323-Regular.ttf");

            MapRenderer = new TiledMapRenderer(Globals.Game.Map, null, AssetManager.LoadTexture2D("SubwayShip.png"));
            Camera = new Camera2D(new Rectangle(0, 0, ElementGlobals.TargetResolutionWidth, ElementGlobals.TargetResolutionHeight))
            {
                BoundingBox = new Rectangle(0, -5000, Globals.Game.Map.MapSize.X * Globals.Game.Map.TileSize.X, 10000),
                Zoom = 3
            };

            Globals.CharacterManager = new CharacterManager("Crew.json");
            Globals.CharacterManager.LoadJobs("Jobs.json");
            Globals.DialogueManager = new DialogueManager();
        }

        public override void Unload()
        {
        }

        public override void HandleGameControl(string controlName, GameControlState state, GameTimer gameTimer)
        {
            switch (controlName)
            {
                case "MoveLeft":
                    if (state == GameControlState.Pressed)
                        Player.Velocity.X -= GameConfig.CharacterSpeed;
                    else if (state == GameControlState.Released)
                        Player.Velocity.X += GameConfig.CharacterSpeed;
                    break;

                case "MoveRight":
                    if (state == GameControlState.Pressed)
                        Player.Velocity.X += GameConfig.CharacterSpeed;
                    else if (state == GameControlState.Released)
                        Player.Velocity.X -= GameConfig.CharacterSpeed;
                    break;
            }

            DialogueManager.HandleGameControl(controlName, state, gameTimer);
        }

        public override void Update(GameTimer gameTimer)
        {
            CharacterManager.Update(gameTimer, Globals.Game.Map);
            DialogueManager.Update(gameTimer);

            Camera.Center(Player.Position.ToVector2I());
            Camera.Update(gameTimer);
        }

        public override void Draw(GameTimer gameTimer)
        {
            var mouseWorldPos = Camera.ScreenToWorld(InputManager.MousePosition).ToVector2I();
            var mousePosText = mouseWorldPos.X + "x" + mouseWorldPos.Y;

            MapRenderer.DrawLayers(0, 1, Camera);

            SpriteBatch.Begin(SamplerType.Point, Camera.GetViewMatrix());
            CharacterManager.Draw(SpriteBatch);
            SpriteBatch.End();

            SpriteBatch.Begin(SamplerType.Point);
            CharacterManager.DrawScreenSpace(SpriteBatch);

#if DEBUG
            SpriteBatch.DrawText(DefaultFont, Player.Position.ToVector2I().ToString(), new Vector2(25, 25), Veldrid.RgbaByte.White, 20, 1);
            SpriteBatch.DrawText(DefaultFont, Camera.ToString(), new Vector2(25, 50), Veldrid.RgbaByte.White, 20, 1);
            SpriteBatch.DrawText(DefaultFont, Globals.Game.BackgroundCamera.ToString(), new Vector2(25, 75), Veldrid.RgbaByte.White, 20, 1);
            SpriteBatch.DrawText(DefaultFont, mousePosText, InputManager.MousePosition - (DefaultFont.MeasureText(mousePosText, 20, 1) / new Vector2(2f, 1f)), Veldrid.RgbaByte.White, 20, 1);
#endif
            DialogueManager.Draw(SpriteBatch, Camera);
            SpriteBatch.End();
        }
    }
}
