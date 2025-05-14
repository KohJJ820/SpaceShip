// Descriptions
// Purple is created when Red and Blue merged together
// Purple can be formed by merge_1 and merge_2
// merge_1  - This merge happened when player pressed the Key R on keyboard
//          - Red and Blue will be fired together and move vertically towards each other
//          - When OneInsideAnother, both Red and Blue destroy and Purple is created
//          - The Purple will charge for 1 second and shoot forward with rapid speed and destroy every asteroid collided
// merge_2  - This merge happened when player fired the Red and Blue seperately and the Red collide with Blue
//          - Once collided, Red and Blue will be destroyed and Purple will be created
//          - For merge_2, Purple will stay on its position and increase its size
//          - Purple will destroy every asteroid that collided with it
//          - Purple will be destroyed after some time
// Behaviour - Purple will attract all the Asteroid within radius to move towards it and destroy the Asteroid when collision happened
// Contains animation that updates 4 frames per second
// The Purple will be destroyed once get out from the screen

using GAlgoT2430.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Lab06
{
    public class Purple : SpriteGameObject, ICollidable
    {
        private Rectangle _rectangle;
        private Vector2 _velocity;
        private float speed = 350f;
        private float lastRealTime, loadingTime = ScalableGameTime.RealTime; // Loading time is used to handle the charging time and staying time
        private int count = 0; // To handle the animation
        private String[] textures; // To handle the animation
        public Purple() : base("Purple", "purple_1")
        {

        }

        public override void Initialize()
        {
            // Position
            Position = new Vector2(_game.Graphics.PreferredBackBufferWidth / 2f, _game.Graphics.PreferredBackBufferHeight / 2f);
            
            // Origin
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            // Scale
            Scale = Vector2.One / 2;
            
            // Bound
            _rectangle.X = (int)(Position.X - Origin.X);
            _rectangle.Y = (int)(Position.Y - Origin.Y);
            _rectangle.Width = Texture.Width;
            _rectangle.Height = Texture.Height;

            // Collision
            _game.CollisionEngine.Listen(typeof(Background), typeof(Purple), CollisionEngine.NotAABB);

            // Textures
            textures = ["purple_1", "purple_2"];
        }

        public override void Update()
        {
            float elapsedSeconds = ScalableGameTime.DeltaTime;
            float realTime = ScalableGameTime.RealTime;

            // 4 frame per second
            if (realTime - lastRealTime > 0.25)
            {
                lastRealTime = realTime;
                count += 1;
                if (count == 2)
                {
                    count = 0;
                }
                _textureName = textures[count];
                LoadContent();
            }
            
            Orientation += 1 * elapsedSeconds;

            // Charge for 1 second
            if (realTime - loadingTime > 1)
            {
                Position.X += _velocity.X * speed * elapsedSeconds;
                Position.Y += _velocity.Y * speed * elapsedSeconds;
            }
            else
            {
                Scale += Vector2.One * elapsedSeconds;
            }  

            // If merge_2 happened, increase the size
            if (_velocity == Vector2.Zero && realTime - loadingTime < 4)
            {
                Scale.X += 1 * elapsedSeconds;
                Scale.Y += 1 * elapsedSeconds;

                _rectangle.Width = (int)(Texture.Width * Scale.X);
                _rectangle.Height = (int)(Texture.Height * Scale.Y);
            }   

            // A few time later, destroy the instance
            if (realTime - loadingTime >= 9)
            {
                GameObjectCollection.DeInstantiate(this);
            }
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin();
            _game.SpriteBatch.Draw(Texture, Position, null, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);
            _game.SpriteBatch.End();
        }

        public void setVelocity(Vector2 _velocity)
        {
            this._velocity = _velocity;
        }

        public string GetGroupName()
        {
            return GetType().Name;
        }

        public Rectangle GetBound()
        {
            _rectangle.Location = (Position - Origin * Scale).ToPoint();
            return _rectangle;
        }

        public void OnCollision(CollisionInfo collisionInfo)
        {
            if (collisionInfo.Other is Background)
            {
                // Destruct the missile
                GameObjectCollection.DeInstantiate(this);
            }
        }
    }
}