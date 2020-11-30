using ElementEngine;
using ElementEngine.Tiled;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GAME_OFF_2020.GameStates
{
    public class GameStatePlay : GameState
    {
        public const float CHARACTER_SPEED = 100f;
        public const float CHARACTER_Y = 217;

        public Camera2D Camera;
        public TiledMap Map;
        public TiledMapRenderer MapRenderer;
        public CharacterManager CharacterManager;

        public SpriteBatch2D SpriteBatch;
        public SpriteFont DefaultFont;

        public AnimatedSprite PlayerSprite;
        public Vector2 PlayerVelocity;
        public Vector2 PlayerPosition;

        public override void Load()
        {
            SpriteBatch = new SpriteBatch2D();
            DefaultFont = AssetManager.LoadSpriteFont("Lato-Bold.ttf");

            Map = AssetManager.LoadTiledMap("Ship.tmx");
            MapRenderer = new TiledMapRenderer(Map, null, AssetManager.LoadTexture2D("SubwayShip.png"));
            Camera = new Camera2D(new Rectangle(0, 0, ElementGlobals.TargetResolutionWidth, ElementGlobals.TargetResolutionHeight));
            Camera.BoundingBox = new Rectangle(0, -5000, Map.MapSize.X * Map.TileSize.X, 10000);
            Camera.Zoom = 2;

            CharacterManager = new CharacterManager("Crew.json");
            PlayerSprite = new AnimatedSprite(AssetManager.LoadTexture2D("DogWalking.png"), new Vector2I(32, 32));
            PlayerPosition.Y = CHARACTER_Y;

            foreach (var character in CharacterManager.Characters)
                character.Position = new Vector2(character.Data.StartPosition, CHARACTER_Y);
        }

        public override void Unload()
        {
        }

        public override void HandleGameControl(string controlName, GameControlState state, GameTimer gameTimer)
        {
            switch (controlName)
            {
                //case "MoveUp":
                //    if (state == GameControlState.Pressed)
                //        PlayerVelocity.Y -= CHARACTER_SPEED;
                //    else if (state == GameControlState.Released)
                //        PlayerVelocity.Y += CHARACTER_SPEED;
                //    break;

                //case "MoveDown":
                //    if (state == GameControlState.Pressed)
                //        PlayerVelocity.Y += CHARACTER_SPEED;
                //    else if (state == GameControlState.Released)
                //        PlayerVelocity.Y -= CHARACTER_SPEED;
                //    break;

                case "MoveLeft":
                    if (state == GameControlState.Pressed)
                        PlayerVelocity.X -= CHARACTER_SPEED;
                    else if (state == GameControlState.Released)
                        PlayerVelocity.X += CHARACTER_SPEED;
                    break;

                case "MoveRight":
                    if (state == GameControlState.Pressed)
                        PlayerVelocity.X += CHARACTER_SPEED;
                    else if (state == GameControlState.Released)
                        PlayerVelocity.X -= CHARACTER_SPEED;
                    break;
            }
        }

        public override void Update(GameTimer gameTimer)
        {
            PlayerPosition += PlayerVelocity * gameTimer.DeltaS;
            Camera.Center(PlayerPosition.ToVector2I());
            Camera.Update(gameTimer);

            if (PlayerVelocity.X > 0 && PlayerPosition.X > (Map.MapPixelSize.X + PlayerSprite.Width))
                PlayerPosition.X = -PlayerSprite.Width;
            else if (PlayerVelocity.X < 0 && PlayerPosition.X < -PlayerSprite.Width)
                PlayerPosition.X = (Map.MapPixelSize.X + PlayerSprite.Width);
        }

        public override void Draw(GameTimer gameTimer)
        {
            MapRenderer.DrawLayers(0, 1, Camera);

            SpriteBatch.Begin(SamplerType.Point, Camera.GetViewMatrix());

            foreach (var character in CharacterManager.Characters)
                SpriteBatch.DrawSprite(character.Sprite, character.Position);

            SpriteBatch.DrawSprite(PlayerSprite, PlayerPosition.ToVector2I());

            SpriteBatch.End();

            SpriteBatch.Begin(SamplerType.Point);
            SpriteBatch.DrawText(DefaultFont, PlayerPosition.ToVector2I().ToString(), new Vector2(25, 25), Veldrid.RgbaByte.White, 20, 1);
            SpriteBatch.DrawText(DefaultFont, Camera.ToString(), new Vector2(25, 50), Veldrid.RgbaByte.White, 20, 1);
            SpriteBatch.End();
        }
    }
}
