using ElementEngine;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Linq;

namespace GAME_OFF_2020
{
    public class SpeechBubbleConfig
    {
        public Rectangle TopLeft;
        public Rectangle TopCenter;
        public Rectangle TopRight;

        public Rectangle MiddleLeft;
        public Rectangle MiddleCenter;
        public Rectangle MiddleRight;

        public Rectangle BottomLeft;
        public Rectangle BottomCenter;
        public Rectangle BottomRight;
    }

    public static class GameConfig
    {
        public static float CharacterSpeed { get; set; }
        public static float BackgroundSpeed { get; set; }
        public static float CharacterY { get; set; }
        public static int DialogueMaxWidth { get; set; }
        public static int DialoguePadding { get; set; }
        public static int DialogueFontSize { get; set; }
        public static SpeechBubbleConfig SpeechBubbleConfig { get; set; }

        public static void Load()
        {
            using var fs = AssetManager.GetAssetStream("GameConfig.xml");
            var configDoc = XDocument.Load(fs);

            CharacterSpeed = float.Parse(configDoc.Root.Element("CharacterSpeed").Value);
            BackgroundSpeed = float.Parse(configDoc.Root.Element("BackgroundSpeed").Value);
            CharacterY = float.Parse(configDoc.Root.Element("CharacterY").Value);
            DialogueMaxWidth = int.Parse(configDoc.Root.Element("DialogueMaxWidth").Value);
            DialoguePadding = int.Parse(configDoc.Root.Element("DialoguePadding").Value);
            DialogueFontSize = int.Parse(configDoc.Root.Element("DialogueFontSize").Value);

            var speechBubble = configDoc.Root.Element("SpeechBubble9Slice");
            var speechBubbleTileSizeSplit = speechBubble.Attribute("TileSize").Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var speechBubbleTileSize = new Vector2I(int.Parse(speechBubbleTileSizeSplit[0]), int.Parse(speechBubbleTileSizeSplit[1]));

            var speechBubbleTopLeftSplit = speechBubble.Element("TopLeft").Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var speechBubbleTopCenterSplit = speechBubble.Element("TopCenter").Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var speechBubbleTopRightSplit = speechBubble.Element("TopRight").Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var speechBubbleMiddleLeftSplit = speechBubble.Element("MiddleLeft").Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var speechBubbleMiddleCenterSplit = speechBubble.Element("MiddleCenter").Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var speechBubbleMiddleRightSplit = speechBubble.Element("MiddleRight").Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var speechBubbleBottomLeftSplit = speechBubble.Element("BottomLeft").Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var speechBubbleBottomCenterSplit = speechBubble.Element("BottomCenter").Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var speechBubbleBottomRightSplit = speechBubble.Element("BottomRight").Value.Split(",", StringSplitOptions.RemoveEmptyEntries);

            SpeechBubbleConfig = new SpeechBubbleConfig()
            {
                TopLeft = new Rectangle(int.Parse(speechBubbleTopLeftSplit[0]) * speechBubbleTileSize.X, int.Parse(speechBubbleTopLeftSplit[1]) * speechBubbleTileSize.Y, speechBubbleTileSize.X, speechBubbleTileSize.Y),
                TopCenter = new Rectangle(int.Parse(speechBubbleTopCenterSplit[0]) * speechBubbleTileSize.X, int.Parse(speechBubbleTopCenterSplit[1]) * speechBubbleTileSize.Y, speechBubbleTileSize.X, speechBubbleTileSize.Y),
                TopRight = new Rectangle(int.Parse(speechBubbleTopRightSplit[0]) * speechBubbleTileSize.X, int.Parse(speechBubbleTopRightSplit[1]) * speechBubbleTileSize.Y, speechBubbleTileSize.X, speechBubbleTileSize.Y),
                MiddleLeft = new Rectangle(int.Parse(speechBubbleMiddleLeftSplit[0]) * speechBubbleTileSize.X, int.Parse(speechBubbleMiddleLeftSplit[1]) * speechBubbleTileSize.Y, speechBubbleTileSize.X, speechBubbleTileSize.Y),
                MiddleCenter = new Rectangle(int.Parse(speechBubbleMiddleCenterSplit[0]) * speechBubbleTileSize.X, int.Parse(speechBubbleMiddleCenterSplit[1]) * speechBubbleTileSize.Y, speechBubbleTileSize.X, speechBubbleTileSize.Y),
                MiddleRight = new Rectangle(int.Parse(speechBubbleMiddleRightSplit[0]) * speechBubbleTileSize.X, int.Parse(speechBubbleMiddleRightSplit[1]) * speechBubbleTileSize.Y, speechBubbleTileSize.X, speechBubbleTileSize.Y),
                BottomLeft = new Rectangle(int.Parse(speechBubbleBottomLeftSplit[0]) * speechBubbleTileSize.X, int.Parse(speechBubbleBottomLeftSplit[1]) * speechBubbleTileSize.Y, speechBubbleTileSize.X, speechBubbleTileSize.Y),
                BottomCenter = new Rectangle(int.Parse(speechBubbleBottomCenterSplit[0]) * speechBubbleTileSize.X, int.Parse(speechBubbleBottomCenterSplit[1]) * speechBubbleTileSize.Y, speechBubbleTileSize.X, speechBubbleTileSize.Y),
                BottomRight = new Rectangle(int.Parse(speechBubbleBottomRightSplit[0]) * speechBubbleTileSize.X, int.Parse(speechBubbleBottomRightSplit[1]) * speechBubbleTileSize.Y, speechBubbleTileSize.X, speechBubbleTileSize.Y),
            };
        }
    }
}
