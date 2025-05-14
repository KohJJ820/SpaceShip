// Descriptions
// The Explosion created when the Mine that stick on the Asteroid exploded and the Asteroid destroyed by Red or Purple
// An animation of 4 frames per second will be played
// After the animation ended, this instance destroyed
// Two types of animation: Mine explosion and Red_Purple explosion

using GAlgoT2430.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Lab06
{
    public class Explosion : SpriteGameObject
    {
        private float lastRealTime = ScalableGameTime.RealTime; // Loading time is used to handle the charging time and staying time
        private int count = 0; // To handle the animation frame
        private int animationNum; // The number of animation, 1 for Asteroid destroyed by Red or Purple and 0 for Mine explosion
        private String[] textures; // To store the animation textures
        public Explosion(int animationNum) : base("explosion_1")
        {
            this.animationNum = animationNum;
        }

        public override void Initialize()
        {
            // Position
            Position = new Vector2(_game.Graphics.PreferredBackBufferWidth / 2f, _game.Graphics.PreferredBackBufferHeight / 2f);

            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            // Scale
            Scale *= 2;

            // Textures (depend on animation number)
            if (animationNum == 0)
            {    
                textures = ["explosion_1", "explosion_2", "explosion_3", "explosion_4", "explosion_5", "explosion_6"];
            }
            else if (animationNum == 1)
            {
                textures = ["explosion_7", "explosion_8", "explosion_9"];
            }
        }

        public override void Update()
        {
            float realTime = ScalableGameTime.RealTime;

            // 4 frame per second
            if (realTime - lastRealTime > 0.25)
            {
                lastRealTime = realTime;
                count += 1;
                if ((count == 6 && animationNum == 0) || (count == 3 && animationNum == 1))
                {
                    GameObjectCollection.DeInstantiate(this);
                }
                else
                {
                    _textureName = textures[count];
                    LoadContent();
                }
            }
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin();
            _game.SpriteBatch.Draw(Texture, Position, null, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);
            _game.SpriteBatch.End();
        }
    }
}