using ElementEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace GAME_OFF_2020
{
    public class CharacterData
    {
        public string Name { get; set; }
        public string Texture { get; set; }
        public float StartPosition { get; set; }
        public string Species { get; set; }
        public string Talent { get; set; }
        public string Weakness { get; set; }
        public string Likes { get; set; }
        public string Hates { get; set; }

        public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> Dialogue { get; set; }
    }

    public class Character
    {
        public CharacterData Data { get; set; }
        public AnimatedSprite Sprite { get; set; }
        public Vector2 Position;
        public Vector2 Velocity;
    }

    public class CharacterManager
    {
        public List<CharacterData> CharacterData { get; set; }
        public List<Character> Characters { get; set; } = new List<Character>();

        public CharacterManager(string assetName)
        {
            using var fs = AssetManager.GetAssetStream(assetName);
            using var streamReader = new StreamReader(fs);
            using var jsonTextReader = new JsonTextReader(streamReader);

            var serializer = new JsonSerializer();
            CharacterData = serializer.Deserialize<List<CharacterData>>(jsonTextReader);

            foreach (var data in CharacterData)
            {
                Characters.Add(new Character()
                {
                    Data = data,
                    Sprite = new AnimatedSprite(AssetManager.LoadTexture2D(data.Texture), new Vector2I(32, 32)),
                });
            }
        }
    }
}
