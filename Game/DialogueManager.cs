using ElementEngine;
using GAME_OFF_2020.GameStates;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GAME_OFF_2020
{
    public class DialogueManager
    {
        public SpriteFont DefaultFont => GameStatePlay.DefaultFont;

        public string CurrentText { get; set; }
        public Texture2D BackgroundAtlas { get; set; }
        public Texture2D BackgroundTexture { get; set; }
        public bool IsShowing { get; set; }
        public Character DialogueTarget { get; set; }

        public DialogueManager()
        {
            BackgroundAtlas = AssetManager.LoadTexture2D("SpeechBubble.png");
        }

        public void StartDialogue()
        {
            var character = Globals.CharacterManager.InteractTarget;
            if (character == null || IsShowing)
                return;

            DialogueTarget = character;
            ShowDialogue(DialogueTarget.Data.Name);
            DialogueTarget.IsTalking = true;
            Globals.CharacterManager.Player.IsTalking = true;
        }

        public void ShowDialogue(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            BackgroundTexture?.Dispose();
            CurrentText = text;
            IsShowing = true;

            var textSizeRect = DefaultFont.MeasureTextRect(CurrentText, GameConfig.DialogueFontSize, 0);
            var textSize = new Vector2(textSizeRect.Width + textSizeRect.X, textSizeRect.Height + textSizeRect.Y);
            textSize += new Vector2(GameConfig.DialoguePadding * 2, GameConfig.DialoguePadding * 2);

            BackgroundTexture = GraphicsHelper.Create9SliceTexture((int)textSize.X / 3, (int)textSize.Y / 3, BackgroundAtlas,
                GameConfig.SpeechBubbleConfig.TopLeft, GameConfig.SpeechBubbleConfig.TopCenter, GameConfig.SpeechBubbleConfig.TopRight,
                GameConfig.SpeechBubbleConfig.MiddleLeft, GameConfig.SpeechBubbleConfig.MiddleCenter, GameConfig.SpeechBubbleConfig.MiddleRight,
                GameConfig.SpeechBubbleConfig.BottomLeft, GameConfig.SpeechBubbleConfig.BottomCenter, GameConfig.SpeechBubbleConfig.BottomRight);
        }

        public void StopDialogue()
        {
            HideDialogue();
            DialogueTarget.IsTalking = false;
            Globals.CharacterManager.Player.IsTalking = false;
        }

        public void HideDialogue()
        {
            CurrentText = "";
            IsShowing = false;
        }

        public void Update(GameTimer gameTimer)
        {

        }

        public void Draw(SpriteBatch2D spriteBatch, Camera2D camera)
        {
            if (string.IsNullOrWhiteSpace(CurrentText) || !IsShowing)
                return;

            var screenDrawPos = new Vector2((ElementGlobals.TargetResolutionWidth / 2) - (BackgroundTexture.Width * 3 / 2), 550);
            spriteBatch.DrawTexture2D(BackgroundTexture, screenDrawPos, null, new Vector2(3));
            spriteBatch.DrawText(DefaultFont, CurrentText, screenDrawPos + new Vector2(GameConfig.DialoguePadding), Veldrid.RgbaByte.White, GameConfig.DialogueFontSize, 0);
        }
    }
}
