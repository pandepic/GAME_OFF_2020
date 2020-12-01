using ElementEngine;
using GAME_OFF_2020.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace GAME_OFF_2020
{
    public enum DialogueStepType
    {
        Menu,
        ChooseTopic,
        AssignJob,
        AssignRest,
        DialogueLine,
    }

    public class DialogueOption
    {
        public string Text { get; set; }
        public bool IsOption { get; set; } = true;
        public bool IsSelected { get; set; } = false;
        public DialogueStepType Step { get; set; }

        public DialogueOption(string text, DialogueStepType step, bool option = true)
        {
            Text = DialogueManager.GetWrappedText(text);
            IsOption = option;
            Step = step;
        }
    }

    public class DialogueManager
    {
        public static SpriteFont DefaultFont => GameStatePlay.DefaultFont;

        public Random RNG = new Random();
        public string CurrentText { get; set; }
        public Texture2D BackgroundAtlas { get; set; }
        public Texture2D BackgroundTexture { get; set; }

        public bool IsShowing { get; set; }
        public Character DialogueTarget { get; set; }
        public DialogueStepType DialogueStep { get; set; }
        public List<DialogueOption> DialogueOptions { get; set; } = new List<DialogueOption>();
        public StringBuilder DialogueSB { get; set; } = new StringBuilder();

        protected int _lastOpinionIndex = 1;

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
            DialogueStep = DialogueStepType.Menu;
            SetDialogueStep(DialogueStep);

            DialogueTarget.IsTalking = true;
            Globals.CharacterManager.Player.IsTalking = true;
        }

        public void UpdateDialogueStep()
        {
            DialogueSB.Clear();

            foreach (var option in DialogueOptions)
                DialogueSB.AppendLine((option.IsOption ? (option.IsSelected ? "> " : "  ") : "") + option.Text);

            ShowDialogue(DialogueSB.ToString());
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

            var screenDrawPos = new Vector2((ElementGlobals.TargetResolutionWidth / 2) - (BackgroundTexture.Width * 3 / 2), (ElementGlobals.TargetResolutionHeight / 2) - (BackgroundTexture.Height * 3 / 2));
            spriteBatch.DrawTexture2D(BackgroundTexture, screenDrawPos, null, new Vector2(3));
            spriteBatch.DrawText(DefaultFont, CurrentText, screenDrawPos + new Vector2(GameConfig.DialoguePadding), Veldrid.RgbaByte.White, GameConfig.DialogueFontSize, 0);
        }

        public void SetDialogueStep(DialogueStepType step, DialogueOption previousOption = null)
        {
            DialogueOptions.Clear();
            DialogueStep = step;

            switch (step)
            {
                case DialogueStepType.Menu:
                    DialogueOptions.Add(new DialogueOption("How's It Going?", step));
                    if (!DialogueTarget.IsWorking)
                        DialogueOptions.Add(new DialogueOption("Assign Job", step));
                    if (!DialogueTarget.IsResting)
                        DialogueOptions.Add(new DialogueOption("Assign Rest", step));
                    break;

                case DialogueStepType.ChooseTopic:
                    var workingKey = DialogueTarget.IsWorking ? "Currently Working" : "Not Currently Working";
                    foreach (var topicKVP in DialogueTarget.Data.Dialogue["How's It Going?"][workingKey])
                    {
                        if (topicKVP.Key != "Opinion of Coworker")
                            DialogueOptions.Add(new DialogueOption(topicKVP.Key, step));
                    }
                    break;

                case DialogueStepType.AssignJob:
                    foreach (var jobType in Globals.CharacterManager.JobTypes)
                        DialogueOptions.Add(new DialogueOption(jobType.Name, step));
                    break;

                case DialogueStepType.DialogueLine:
                    if (DialogueTarget.IsWorking)
                    {
                        var rng = RNG.Next(1, 4);

                        if (DialogueTarget.CurrentJob.Name == DialogueTarget.Data.Talent)
                            DialogueOptions.Add(new DialogueOption(DialogueTarget.Data.Dialogue["How's It Going?"]["Currently Working"]["Status"]["Working On Talent"], step, false));
                        else if (DialogueTarget.CurrentJob.Name == DialogueTarget.Data.Weakness)
                            DialogueOptions.Add(new DialogueOption(DialogueTarget.Data.Dialogue["How's It Going?"]["Currently Working"]["Status"]["Working On Weakness"], step, false));
                        else
                            DialogueOptions.Add(new DialogueOption(DialogueTarget.Data.Dialogue["How's It Going?"]["Currently Working"]["Status"]["Default " + rng.ToString()], step, false));
                    }
                    else
                    {
                        if (previousOption.Text == "Personal")
                        {
                            DialogueOptions.Add(new DialogueOption(DialogueTarget.Data.Dialogue["How's It Going?"]["Not Currently Working"][previousOption.Text][DialogueTarget.Mood.ToString()], step, false));
                        }
                        else
                        {
                            if (_lastOpinionIndex == 0)
                                _lastOpinionIndex = 1;
                            else
                                _lastOpinionIndex = 0;

                            DialogueOptions.Add(new DialogueOption(DialogueTarget.Data.Dialogue["How's It Going?"]["Not Currently Working"][previousOption.Text].ElementAt(_lastOpinionIndex).Value, step, false));
                        }
                    }
                    break;
            }

            DialogueOptions.Add(new DialogueOption("Back", step));

            var firstSelected = false;
            foreach (var option in DialogueOptions)
            {
                if (option.IsOption && !firstSelected)
                {
                    option.IsSelected = true;
                    firstSelected = true;
                }
            }

            UpdateDialogueStep();
        }

        public static StringBuilder WrappedTextSB = new StringBuilder();
        public static string GetWrappedText(string text)
        {
            WrappedTextSB.Clear();
            var width = 0;
            var words = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                var textSize = DefaultFont.MeasureText(word, GameConfig.DialogueFontSize);

                if (width + textSize.X > GameConfig.DialogueMaxWidth)
                {
                    width = 0;
                    WrappedTextSB.Length -= 1;
                    WrappedTextSB.Append("\n");
                }

                WrappedTextSB.Append(word + " ");
                width += (int)textSize.X;
            }

            return WrappedTextSB.ToString().TrimEnd(' ');
        }

        public void ChangeSelectedOption(int direction)
        {
            if (DialogueOptions.Where(o => o.IsOption).Count() <= 1)
                return;

            var selectedIndex = DialogueOptions.IndexOf(DialogueOptions.Where(o => o.IsSelected).First());
            DialogueOptions[selectedIndex].IsSelected = false;

            selectedIndex += direction;
            if (selectedIndex < 0) { selectedIndex = DialogueOptions.Count - 1; }
            if (selectedIndex >= DialogueOptions.Count) { selectedIndex = 0; }

            while (!DialogueOptions[selectedIndex].IsOption)
            {
                selectedIndex += direction;
                if (selectedIndex < 0) { selectedIndex = DialogueOptions.Count - 1; }
                if (selectedIndex >= DialogueOptions.Count) { selectedIndex = 0; }
            }

            DialogueOptions[selectedIndex].IsSelected = true;
            UpdateDialogueStep();
        }

        public void SelectOption()
        {
            var selectedOption = DialogueOptions.Where(o => o.IsSelected).First();
            var selectedIndex = DialogueOptions.IndexOf(selectedOption);

            if (selectedOption.Text == "Back")
            {
                Back();
                return;
            }

            switch (DialogueStep)
            {
                case DialogueStepType.Menu:
                    if (selectedIndex == 0)
                        SetDialogueStep(DialogueStepType.ChooseTopic);
                    else if (selectedIndex == 1)
                        SetDialogueStep(DialogueStepType.AssignJob);
                    else if (selectedIndex == 2)
                    {
                        DialogueTarget.Rest();
                        StopDialogue();
                    }
                    break;

                case DialogueStepType.ChooseTopic:
                    SetDialogueStep(DialogueStepType.DialogueLine, selectedOption);
                    break;

                case DialogueStepType.AssignJob:
                    var job = Globals.CharacterManager.JobTypes.Where(j => j.Name == selectedOption.Text).First();
                    if (!job.IsOccupied)
                    {
                        DialogueTarget.StartWorking(job);
                        StopDialogue();
                    }
                    break;
            }
        }

        public void Back()
        {
            switch (DialogueStep)
            {
                case DialogueStepType.Menu:
                    StopDialogue();
                    break;

                case DialogueStepType.AssignJob:
                case DialogueStepType.ChooseTopic:
                    SetDialogueStep(DialogueStepType.Menu);
                    break;

                case DialogueStepType.DialogueLine:
                    SetDialogueStep(DialogueStepType.ChooseTopic);
                    break;
            }
        }

        public void HandleGameControl(string controlName, GameControlState state, GameTimer gameTimer)
        {
            if (state != GameControlState.Released)
                return;

            if (controlName == "Interact")
            {
                if (!IsShowing)
                    StartDialogue();
                else
                    SelectOption();
            }
            else if (controlName == "Cancel")
            {
                if (IsShowing)
                    Back();
            }
            else if (controlName == "MoveUp" && IsShowing)
            {
                ChangeSelectedOption(-1);
            }
            else if (controlName == "MoveDown" && IsShowing)
            {
                ChangeSelectedOption(1);
            }
        }
    }
}
