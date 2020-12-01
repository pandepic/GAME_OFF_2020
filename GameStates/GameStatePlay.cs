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

        public Random RNG = new Random();
        public Camera2D BackgroundCamera;
        public TiledMap Map;
        public TiledMapRenderer MapRenderer;
        public TileBatch2D BackgroundStars;

        public SpriteBatch2D SpriteBatch;

        public override void Load()
        {
            GameConfig.Load();

            SpriteBatch = new SpriteBatch2D();
            DefaultFont = AssetManager.LoadSpriteFont("VT323-Regular.ttf");

            Map = AssetManager.LoadTiledMap("Ship.tmx");
            MapRenderer = new TiledMapRenderer(Map, null, AssetManager.LoadTexture2D("SubwayShip.png"));
            Camera = new Camera2D(new Rectangle(0, 0, ElementGlobals.TargetResolutionWidth, ElementGlobals.TargetResolutionHeight))
            {
                BoundingBox = new Rectangle(0, -5000, Map.MapSize.X * Map.TileSize.X, 10000),
                Zoom = 3
            };

            BackgroundCamera = new Camera2D(new Rectangle(0, 0, ElementGlobals.TargetResolutionWidth, ElementGlobals.TargetResolutionHeight))
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

            Globals.CharacterManager = new CharacterManager("Crew.json");
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

                case "Interact":
                    if (state == GameControlState.Released)
                        DialogueManager.StartDialogue();
                    break;
            }
        }

        public override void Update(GameTimer gameTimer)
        {
            BackgroundCamera.X += GameConfig.BackgroundSpeed * gameTimer.DeltaS;
            BackgroundCamera.Update(gameTimer);

            CharacterManager.Update(gameTimer, Map);
            DialogueManager.Update(gameTimer);

            Camera.Center(Player.Position.ToVector2I());
            Camera.Update(gameTimer);
        }

        public override void Draw(GameTimer gameTimer)
        {
            BackgroundStars.DrawAll(BackgroundCamera.Position.ToVector2I(), 2);
            MapRenderer.DrawLayers(0, 1, Camera);

            SpriteBatch.Begin(SamplerType.Point, Camera.GetViewMatrix());
            CharacterManager.Draw(SpriteBatch);
            SpriteBatch.End();

            SpriteBatch.Begin(SamplerType.Point);
            CharacterManager.DrawScreenSpace(SpriteBatch);

#if DEBUG
            SpriteBatch.DrawText(DefaultFont, Player.Position.ToVector2I().ToString(), new Vector2(25, 25), Veldrid.RgbaByte.White, 20, 1);
            SpriteBatch.DrawText(DefaultFont, Camera.ToString(), new Vector2(25, 50), Veldrid.RgbaByte.White, 20, 1);
            SpriteBatch.DrawText(DefaultFont, BackgroundCamera.ToString(), new Vector2(25, 75), Veldrid.RgbaByte.White, 20, 1);
#endif
            DialogueManager.Draw(SpriteBatch, Camera);
            SpriteBatch.End();
        }
    }
}
