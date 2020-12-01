using ElementEngine;
using ElementEngine.Tiled;
using GAME_OFF_2020.GameStates;
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
        public float RoomPosition { get; set; }
        public string Species { get; set; }
        public string Talent { get; set; }
        public string Weakness { get; set; }
        public string Likes { get; set; }
        public string Hates { get; set; }

        public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> Dialogue { get; set; }
    }

    public class Job
    {
        public string Name { get; set; }
        public float Position { get; set; }
        public bool IsOccupied { get; set; } = false;
    }

    public class CharacterManager
    {
        public List<Job> JobTypes { get; private set; } = new List<Job>();

        public Camera2D Camera => GameStatePlay.Camera;

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

            JobTypes.Clear();

            foreach (var data in CharacterData)
                Characters.Add(new Character(data));

            Player = new PlayerCharacter(null);
            Characters.Add(Player);
        }

        public void LoadJobs(string assetName)
        {
            using var fs = AssetManager.GetAssetStream(assetName);
            using var streamReader = new StreamReader(fs);
            using var jsonTextReader = new JsonTextReader(streamReader);

            var serializer = new JsonSerializer();
            JobTypes = serializer.Deserialize<List<Job>>(jsonTextReader);
        }

        public void Update(GameTimer gameTimer, TiledMap map)
        {
            if (!Player.IsTalking)
                InteractTarget = null;

            foreach (var character in Characters)
            {
                character.Sprite.Update(gameTimer);

                if (character.IsTalking)
                {
                    character.SetState<CharacterIdleState>();
                    continue;
                }

                var movingToWork = false;
                var workDirection = 1f;
                var movingToRest = false;
                var restDirection = 1f;

                if (character.IsWorking && (int)character.Position.X != (int)character.CurrentJob.Position)
                {
                    workDirection = character.Position.X < character.CurrentJob.Position ? 1f : -1f;
                    character.Velocity.X = GameConfig.CharacterSpeed * workDirection;
                    movingToWork = true;
                }
                else if (character.IsResting && (int)character.Position.X != (int)character.Data.RoomPosition)
                {
                    restDirection = character.Position.X < character.Data.RoomPosition ? 1f : -1f;
                    character.Velocity.X = GameConfig.CharacterSpeed * restDirection;
                    movingToRest = true;
                }

                character.Position += character.Velocity * gameTimer.DeltaS;

                if (movingToWork)
                {
                    if (character.Position.X > character.CurrentJob.Position && workDirection == 1f
                        || character.Position.X < character.CurrentJob.Position && workDirection == -1f)
                    {
                        character.Velocity = Vector2.Zero;
                        character.Position.X = (int)character.CurrentJob.Position;
                    }
                }
                else if (movingToRest)
                {
                    if (character.Position.X > character.Data.RoomPosition && restDirection == 1f
                        || character.Position.X < character.Data.RoomPosition && restDirection == -1f)
                    {
                        character.Velocity = Vector2.Zero;
                        character.Position.X = (int)character.Data.RoomPosition;
                    }
                }

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

                if (!Player.IsTalking && character != Player && character.CollisionRect.Intersects(Player.CollisionRect))
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
                spriteBatch.DrawText(
                    GameStatePlay.DefaultFont,
                    InteractTarget.Data.Name,
                    Camera.WorldToScreen(new Vector2I((int)InteractTarget.Position.X, GameConfig.CharacterNameY)),
                    Veldrid.RgbaByte.White,
                    GameConfig.CharacterNameSize, 1);
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
