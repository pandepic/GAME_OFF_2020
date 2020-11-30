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
        public static float CharacterSpeed { get; private set; }
        public static float BackgroundSpeed { get; private set; }
        public static float CharacterY { get; private set; }

        public static SpriteFont DefaultFont { get; private set; }
        public static Camera2D Camera;

        public Character Player => CharacterManager.Player;

        public Random RNG = new Random();
        public Camera2D BackgroundCamera;
        public TiledMap Map;
        public TiledMapRenderer MapRenderer;
        public TileBatch2D BackgroundStars;
        public CharacterManager CharacterManager;

        public SpriteBatch2D SpriteBatch;

        public override void Load()
        {
            using var fs = AssetManager.GetAssetStream("GameConfig.xml");
            var configDoc = XDocument.Load(fs);

            CharacterSpeed = float.Parse(configDoc.Root.Element("CharacterSpeed").Value);
            BackgroundSpeed = float.Parse(configDoc.Root.Element("BackgroundSpeed").Value);
            CharacterY = float.Parse(configDoc.Root.Element("CharacterY").Value);

            SpriteBatch = new SpriteBatch2D();
            DefaultFont = AssetManager.LoadSpriteFont("Lato-Bold.ttf");

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

            CharacterManager = new CharacterManager("Crew.json");
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
                        Player.Velocity.X -= CharacterSpeed;
                    else if (state == GameControlState.Released)
                        Player.Velocity.X += CharacterSpeed;
                    break;

                case "MoveRight":
                    if (state == GameControlState.Pressed)
                        Player.Velocity.X += CharacterSpeed;
                    else if (state == GameControlState.Released)
                        Player.Velocity.X -= CharacterSpeed;
                    break;
            }
        }

        public override void Update(GameTimer gameTimer)
        {
            BackgroundCamera.X += BackgroundSpeed * gameTimer.DeltaS;
            BackgroundCamera.Update(gameTimer);

            CharacterManager.Update(gameTimer, Map);
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
            SpriteBatch.DrawText(DefaultFont, Player.Position.ToVector2I().ToString(), new Vector2(25, 25), Veldrid.RgbaByte.White, 20, 1);
            SpriteBatch.DrawText(DefaultFont, Camera.ToString(), new Vector2(25, 50), Veldrid.RgbaByte.White, 20, 1);
            SpriteBatch.DrawText(DefaultFont, BackgroundCamera.ToString(), new Vector2(25, 75), Veldrid.RgbaByte.White, 20, 1);
            CharacterManager.DrawScreenSpace(SpriteBatch);
            SpriteBatch.End();
        }
    }
}
