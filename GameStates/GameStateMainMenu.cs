using ElementEngine;
using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;

namespace GAME_OFF_2020.GameStates
{
    public class GameStateMainMenu : GameState
    {
        public SpriteFont DefaultFont { get; set; }
        public SpriteBatch2D SpriteBatch;
        public Sprite TitleSprite { get; set; }

        public override void Load()
        {
            SpriteBatch = new SpriteBatch2D();
            DefaultFont = AssetManager.LoadSpriteFont("VT323-Regular.ttf");
            TitleSprite = new Sprite(AssetManager.LoadTexture2D("Title.png"))
            {
                Scale = new System.Numerics.Vector2(3),
            };
        }

        public override void Unload()
        {
        }

        public override void Update(GameTimer gameTimer)
        {
        }

        public override void Draw(GameTimer gameTimer)
        {
            var menuText = "Press enter to start...";
            var textSize = DefaultFont.MeasureText(menuText, 60, 1);
            SpriteBatch.Begin(SamplerType.Point);
            SpriteBatch.DrawSprite(TitleSprite, new System.Numerics.Vector2((ElementGlobals.TargetResolutionWidth / 2) - ((TitleSprite.Width * 3) / 2), 100));
            SpriteBatch.DrawText(DefaultFont, menuText, new System.Numerics.Vector2((ElementGlobals.TargetResolutionWidth / 2) - (textSize.X / 2), 600), RgbaByte.White, 60, 1);
            SpriteBatch.End();
        }

        public override void HandleKeyReleased(Key key, GameTimer gameTimer)
        {
            if (key == Key.Enter)
                Globals.Game.SetGameState(GameStateType.Play);
        }
    }
}
