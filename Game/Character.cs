﻿using ElementEngine;
using GAME_OFF_2020.GameStates;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GAME_OFF_2020
{
    public enum CharacterMood
    {
        VeryUpset,
        Fine,
        InAGoodPlace,
    }

    public class CharacterBaseState : SimpleStateBase
    {
        public Character Character { get; set; }

        public CharacterBaseState(string name, Character character) : base(name)
        {
            Character = character;
        }
    }

    public class CharacterIdleState : CharacterBaseState
    {
        public CharacterIdleState(Character character) : base("Idle", character) { }

        public override void Begin()
        {
            Character.Sprite.StopAnimation();
        }
    }

    public class CharacterMovingState : CharacterBaseState
    {
        public CharacterMovingState(Character character) : base("Moving", character) { }

        public override void Begin()
        {
            Character.Sprite.PlayAnimation(AnimationManager.GetAnimation("Walk"));
        }
    }

    public class Character
    {
        public CharacterData Data { get; set; }
        public AnimatedSprite Sprite { get; set; }
        public Vector2 Position;
        public Vector2 Velocity;
        public Rectangle CollisionRect => new Rectangle(Position.X, Position.Y, Sprite.Width, Sprite.Height);

        public SimpleStateMachine StateMachine { get; set; } = new SimpleStateMachine();
        public CharacterMood Mood { get; set; } = CharacterMood.Fine;

        public Character(CharacterData data)
        {
            Data = data;

            if (Data != null) // crew
            {
                Sprite = new AnimatedSprite(AssetManager.LoadTexture2D(Data.Texture), new Vector2I(32, 32));
                Position = new Vector2(Data.StartPosition, GameConfig.CharacterY);
            }
            else // player
            {
                Sprite = new AnimatedSprite(AssetManager.LoadTexture2D("DogWalking.png"), new Vector2I(32, 32));
                Position = new Vector2(0, GameConfig.CharacterY);
            }

            StateMachine.RegisterState(new CharacterIdleState(this));
            StateMachine.RegisterState(new CharacterMovingState(this));

            SetState<CharacterIdleState>();
        }

        public void SetState<T>() where T : CharacterBaseState
        {
            if (!(StateMachine.CurrentState is T))
                StateMachine.SetState<T>();
        }
    }
}