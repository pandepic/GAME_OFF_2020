using ElementEngine;
using ElementEngine.Tiled;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

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

    public class CharacterManager
    {
        public Camera2D Camera => GameStates.GameStatePlay.Camera;

        public List<CharacterData> CharacterData { get; set; }
        public List<Character> Characters { get; set; } = new List<Character>();

        public PlayerCharacter Player { get; set; }
        public Character InteractTarget { get; set; }

        public CharacterManager(string assetName)
        {
            using var fs = AssetManager.GetAssetStream(assetName);
            using var streamReader = new StreamReader(fs);
            using var jsonTextReader = new JsonTextReader(streamReader);

            var serializer = new JsonSerializer();
            CharacterData = serializer.Deserialize<List<CharacterData>>(jsonTextReader);

            foreach (var data in CharacterData)
                Characters.Add(new Character(data));

            Player = new PlayerCharacter(null);
            Characters.Add(Player);
        }

        public void Update(GameTimer gameTimer, TiledMap map)
        {
            InteractTarget = null;

            foreach (var character in Characters)
            {
                character.Sprite.Update(gameTimer);

                if (character.IsTalking)
                {
                    character.SetState<CharacterIdleState>();
                    continue;
                }

                character.Position += character.Velocity * gameTimer.DeltaS;

                if (character.Velocity.X > 0 && character.Position.X > (map.MapPixelSize.X + character.Sprite.Width))
                    character.Position.X = -character.Sprite.Width;
                else if (character.Velocity.X < 0 && character.Position.X < -character.Sprite.Width)
                    character.Position.X = (map.MapPixelSize.X + character.Sprite.Width);

                if (character.Velocity != Vector2.Zero)
                    character.SetState<CharacterMovingState>();
                else
                    character.SetState<CharacterIdleState>();

                if (character.Velocity.X > 0)
                    character.Sprite.Flip = SpriteFlipType.None;
                else if (character.Velocity.X < 0)
                    character.Sprite.Flip = SpriteFlipType.Horizontal;

                if (character != Player && character.CollisionRect.Intersects(Player.CollisionRect))
                    InteractTarget = character;
            }
        }

        public void Draw(SpriteBatch2D spriteBatch)
        {
            foreach (var character in Characters)
                spriteBatch.DrawSprite(character.Sprite, character.Position.ToVector2I());
        }

        public void DrawScreenSpace(SpriteBatch2D spriteBatch)
        {
            if (InteractTarget != null)
                spriteBatch.DrawText(GameStates.GameStatePlay.DefaultFont, InteractTarget.Data.Name, Camera.WorldToScreen(InteractTarget.Position.ToVector2I() + new Vector2I(0, -6)), Veldrid.RgbaByte.White, 24, 1);
        }

        public Character GetCharacterFriend(Character character)
        {
            for (var i = 0; i < Characters.Count; i++)
            {
                var c = Characters[i];
                if (character.Data.Likes == c.Data.Name)
                    return c;
            }

            return null;
        }

        public Character GetCharacterEnemy(Character character)
        {
            for (var i = 0; i < Characters.Count; i++)
            {
                var c = Characters[i];
                if (character.Data.Hates == c.Data.Name)
                    return c;
            }

            return null;
        }
    }
}
