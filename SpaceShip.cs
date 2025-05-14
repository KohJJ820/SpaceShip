using GAlgoT2430.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab06
{
    public class SpaceShip : SpriteGameObject, ICollidable
    {
        private Rectangle _rectangle;
        private Vector2 _velocity;
        private Vector2 _cursorVelocity;
        private bool isTurning = false;
        private float turningTime;
        private float speed;
        private float lastRealTime, loadingTime; // loadingTime is used to handle the time for Red and Blue to merge when merge_1 happened
        private float stickyTime, explodeTime; // stickyTime is used to handle time until fire mine, explodeTime is used to handle the time of mine to explode
        private float redCooldown, blueCooldown, stickyCooldown; // Cooldown for Red, Blue and sticky missile
        private bool merge_1, merge_2, collision = false; // Trigger for merge_1 and merge_2 and collision
        Dictionary<Mine, Asteroid> pairs;
        private Vector2 purplePosition;
        private SoundEffect _laserShooting;
        private SoundEffect _stickyMissile;
        private SoundEffect _explosion_1;
        private SoundEffect _explosion_2;
        private SoundEffect _red_blue;
        private SoundEffect _red_purple;
        private SoundEffect _purple_1;
        private SoundEffect _purple_2;
        public SpaceShip() : base("spaceShips_009_right")
        {

        }

        private float ToOrientation(Vector2 direction)
        {
            return (float)Math.Atan2(direction.Y, direction.X);
        }

        public override void Initialize()
        {
            Position = new Vector2(_game.Graphics.PreferredBackBufferWidth / 2f, _game.Graphics.PreferredBackBufferHeight / 2f);
            speed = 100f;

            // Origin
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            // Cooldown
            lastRealTime = ScalableGameTime.RealTime;
            redCooldown = 0;
            blueCooldown = 0;
            stickyCooldown = 5;
            stickyTime = float.PositiveInfinity;

            // Dictionary
            pairs = new Dictionary<Mine, Asteroid>();

            // Sound Effect
            _laserShooting = _game.Content.Load<SoundEffect>("laserShooting");
            _red_blue = _game.Content.Load<SoundEffect>("red_blue_sfx");
            _red_purple = _game.Content.Load<SoundEffect>("red_purple_sfx");
            _purple_1 = _game.Content.Load<SoundEffect>("purple_sfx_1");
            _purple_2 = _game.Content.Load<SoundEffect>("purple_sfx_2");
            _stickyMissile = _game.Content.Load<SoundEffect>("sticky_1");
            _explosion_1 = _game.Content.Load<SoundEffect>("sticky_2");
            _explosion_2 = _game.Content.Load<SoundEffect>("sticky_3");
            
            // Bound
            _rectangle.X = (int)(Position.X - Origin.X);
            _rectangle.Y = (int)(Position.Y - Origin.Y);
            _rectangle.Width = Texture.Width;
            _rectangle.Height = Texture.Height;
        }

        public override void Update()
        {
            float elapsedSeconds = ScalableGameTime.DeltaTime;
            float realTime = ScalableGameTime.RealTime;
            var kstate = Keyboard.GetState();
            Vector2 mousePosition = Mouse.GetState().Position.ToVector2();
            var mstate = Mouse.GetState();
            _velocity = Vector2.Zero;

            // Cooldown
            redCooldown -= elapsedSeconds;
            blueCooldown -= elapsedSeconds;
            stickyCooldown -= elapsedSeconds;

            if (kstate.IsKeyDown(Keys.W))
            {
                _velocity.Y -= speed * elapsedSeconds;
            }
            if (kstate.IsKeyDown(Keys.S))
            {
                _velocity.Y += speed * elapsedSeconds;
            }
            if (kstate.IsKeyDown(Keys.A))
            {
                _velocity.X -= speed * elapsedSeconds;
            }
            if (kstate.IsKeyDown(Keys.D))
            {
                _velocity.X += speed * elapsedSeconds;
            }

            Position += _velocity;
            _cursorVelocity = mousePosition - Position;
            Orientation = ToOrientation(_cursorVelocity);

            // Firing missle
            if (mstate.LeftButton == ButtonState.Pressed)
            {
                if (realTime - lastRealTime >= 0.2)
                {
                    lastRealTime = realTime;
                    FireMissile();
                }
            }

            // Firing missle
            if (mstate.RightButton == ButtonState.Pressed && redCooldown <=0 && blueCooldown <= 0)
            {
                redCooldown = 10;
                blueCooldown = 10;
                isTurning = true;
                turningTime = realTime;
            }

            // Firing sticky missle
            if (stickyCooldown <= 0)
            {
                stickyCooldown = 5;
                FireStickyMissile();
                stickyTime = realTime;
            }

            // Firing Blue
            if (kstate.IsKeyDown(Keys.Q) && blueCooldown <= 0)
            {
                blueCooldown = 10;
                FireBlue();
            }

            // Firing Red
            if (kstate.IsKeyDown(Keys.E) && redCooldown <= 0)
            {
                redCooldown = 10;
                FireRed();
            }

            // Firing Purple
            if (kstate.IsKeyDown(Keys.R))
            {
                if (redCooldown <= 0 && blueCooldown <= 0)
                {
                    merge_1 = true;
                    redCooldown = 10;
                    blueCooldown = 10;
                    FireRed();
                    FireBlue();
                    loadingTime = realTime;
                }
            }

            Turning(realTime);
            Merging(realTime);
            CheckRedPurple(realTime);
            FireMine(realTime);
            CheckSticky(realTime);
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin();
            _game.SpriteBatch.Draw(Texture, Position, null, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);
            _game.SpriteBatch.End();
        }

        public void FireMissile()
        {
            Missile missile = new Missile();
            missile.LoadContent();
            missile.Initialize();
            missile.Position = Position + (Vector2.Normalize(_cursorVelocity) * Texture.Width / 2);
            missile.Orientation = Orientation;
            missile.SetVelocity(Vector2.Normalize(_cursorVelocity)); 

            // Sound Effect
            _laserShooting.Play();
        }

// --------------------------------------------------------------START MISSILE 1-------------------------------------------------------------------------
        // The function that handles firing Blue
        public void FireBlue()
        {
            Blue blue = new Blue(merge_1);
            Vector2 vector = new Vector2(-_cursorVelocity.Y, _cursorVelocity.X);
            blue.LoadContent();
            blue.Initialize();
            blue.Position = Position - (Vector2.Normalize(vector) * Texture.Height / 3) + (Vector2.Normalize(_cursorVelocity) * Texture.Width / 2);
            // Move vertically if merge_1 happened
            if (merge_1)
            {
                blue.setVelocity(Vector2.Normalize(vector));
                blue.setSpeed(100f);
            }
            else
            {
                blue.setVelocity(Vector2.Normalize(_cursorVelocity));
                // Sound Effect
                _red_blue.Play();
            }
        }

        // The function that handles firing Red
        public void FireRed()
        {
            Red red = new Red(merge_1);
            Vector2 vector = new Vector2(-_cursorVelocity.Y, _cursorVelocity.X);
            red.LoadContent();
            red.Initialize();
            red.Position = Position + (Vector2.Normalize(vector) * Texture.Height / 3) + (Vector2.Normalize(_cursorVelocity) * Texture.Width / 2);
            // Move vertically if merge_1 happened
            if (merge_1)
            {
                red.setVelocity(Vector2.Normalize(-vector));
                red.setSpeed(50f);
                // Sound Effect
                _purple_2.Play();
            }
            else
            {
                red.setVelocity(Vector2.Normalize(_cursorVelocity));
                // Sound Effect
                _red_blue.Play();
                if (GameObjectCollection.FindByName("Blue") != null)
                {
                    merge_2 = true;
                }
            }
        }

        // The function that handles firing Purple
        public void FirePurple(Vector2 purplePosition)
        {
            Purple purple = new Purple();
            purple.LoadContent();
            purple.Initialize();
            if (merge_1)
            {
                purple.setVelocity(Vector2.Normalize(_cursorVelocity));
            }
            else if (merge_2)
            {
                // Purple dont move during merge_2
                purple.setVelocity(Vector2.Zero);
                // Sound Effect
                _purple_1.Play();
            }
            purple.Position = purplePosition;
        }

        // The function that handles the merging of Blue and Red
        public void Merging(float realTime)
        {
            // Need to change to Lab06 namespace
            var blue = (Lab06.Blue)GameObjectCollection.FindByName("Blue");
            var red = (Lab06.Red)GameObjectCollection.FindByName("Red");
            
            // Charging Purple
            // Set the position of purple spawn for merge_2, will be set in front of spaceship for merge_1 in the statement below
            if (blue != null)
            {
                purplePosition = blue.Position;
            }

            // Merge_1 happened
            if (merge_1 && (realTime - loadingTime > 1))
            {
                purplePosition = Position + (Vector2.Normalize(_cursorVelocity) * Texture.Width / 2);
                FirePurple(purplePosition);
                merge_1 = false;
            }

            // Merge_2 happened
            if (merge_2 && blue != null && red != null)
            {
                // Check if Red and Blue collided with each other 
                collision = CollisionEngine.AABB(red, blue);

                if (collision)
                {
                    // Remove both Red and Blue when collision happened
                    GameObjectCollection.DeInstantiate(red);
                    GameObjectCollection.DeInstantiate(blue);
                    FirePurple(purplePosition);
                    merge_2 = false;
                }
            }
        }

        // Function to handle the collision between Asteroid and Red and Purple (Blue dont destroy Asteroid)
        public void CheckRedPurple(float realTime)
        {
            List<Asteroid> asteroidToDestroy = new List<Asteroid>();
            List<Asteroid> asteroidDestroyed = new List<Asteroid>();
            GameObject[] asteroids = GameObjectCollection.FindObjectsByType(typeof(Asteroid));
            var purple = (Lab06.Purple)GameObjectCollection.FindByName("Purple");
            var red = (Lab06.Red)GameObjectCollection.FindByName("Red");

            if (asteroids != null)
            {
                foreach (Asteroid asteroid in asteroids)
                {
                    if (purple != null)
                    {
                        if (CollisionEngine.AABB(purple, asteroid) && !asteroidToDestroy.Contains(asteroid))
                        {
                            asteroidToDestroy.Add(asteroid);
                            Explosion explosion = new Explosion(1);
                            explosion.LoadContent();
                            explosion.Initialize();
                            explosion.Position = asteroid.Position;
                            _red_purple.Play();
                        }
                    }

                    if (red != null)
                    {
                        if (CollisionEngine.AABB(red, asteroid) && !asteroidToDestroy.Contains(asteroid))
                        {
                            asteroidToDestroy.Add(asteroid);
                            Explosion explosion = new Explosion(1);
                            explosion.LoadContent();
                            explosion.Initialize();
                            explosion.Position = asteroid.Position;
                            _red_purple.Play();
                        }
                    }
                }

                foreach (Asteroid asteroid in asteroidToDestroy)
                {
                    if (asteroids.Contains(asteroid) && !asteroidDestroyed.Contains(asteroid))
                    {
                        GameObjectCollection.DeInstantiate(asteroid);
                        asteroidDestroyed.Add(asteroid);
                    }
                }
            }
        }

        public void Turning(float realTime)
        {
            var red = (Lab06.Red)GameObjectCollection.FindByName("Red");
            var blue = (Lab06.Blue)GameObjectCollection.FindByName("Blue");
            Vector2 vector = new Vector2(-_cursorVelocity.Y, _cursorVelocity.X);

            if (isTurning)
            {
                if(red == null && blue == null)
                {          
                    red = new Red(merge_1);
                    red.LoadContent();
                    red.Initialize();
                    red.Position = Position + (Vector2.Normalize(vector) * Texture.Height / 3) + (Vector2.Normalize(_cursorVelocity) * Texture.Width / 2);
                    red.setVelocity(Vector2.Normalize(vector));

                    blue = new Blue(merge_1);
                    blue.LoadContent();
                    blue.Initialize();
                    blue.Position = Position - (Vector2.Normalize(vector) * Texture.Height / 3) + (Vector2.Normalize(_cursorVelocity) * Texture.Width / 2);
                    blue.setVelocity(Vector2.Normalize(-vector));

                    _red_blue.Play();
                }

                if(red != null && blue != null)
                {                   
                    // Moving around the spaceship
                    Vector3 angularV = new Vector3(0, 0, (float)(Math.PI / 2));
                    Vector3 radiusR = new Vector3((Position.X - red.Position.X), (Position.Y - red.Position.Y), 0);
                    Vector3 radiusB = new Vector3((Position.X - blue.Position.X), (Position.Y - blue.Position.Y), 0);
                    Vector3 velocityR = Vector3.Cross(angularV, radiusR);
                    Vector3 velocityB = Vector3.Cross(angularV, radiusB);

                    // Path following ?
                    Vector2 pathflwR = red.Position - Position;
                    Vector2 pathflwB = blue.Position - Position;

                    if (pathflwR.Length() < 149 && pathflwB.Length() < 149)
                    {                        
                        red.Position = Position + (Vector2.Normalize(pathflwR + _velocity) * pathflwR.Length());
                        blue.Position = Position + (Vector2.Normalize(pathflwB + _velocity) * pathflwB.Length());
                        
                        red.setSpeed(150);
                        blue.setSpeed(150);
                    }
                    else
                    {
                        // Make sure the distance is always 150f from spaceship and follow the movement of spaceship (path following ?)
                        red.Position = Position + (Vector2.Normalize(pathflwR + _velocity) * 150);
                        blue.Position = Position + (Vector2.Normalize(pathflwB + _velocity) * 150);

                        // Angluar movement
                        red.setVelocity(Vector2.Normalize(new Vector2(velocityR.X, velocityR.Y)));
                        blue.setVelocity(Vector2.Normalize(new Vector2(velocityB.X, velocityB.Y)));

                        red.setSpeed(150 * (float)(Math.PI / 2));
                        blue.setSpeed(150 * (float)(Math.PI / 2));
                    }
                    
                    
                    if (realTime - turningTime > 10)
                    {
                        GameObjectCollection.DeInstantiate(red);
                        GameObjectCollection.DeInstantiate(blue);
                        isTurning = false;
                        return;
                    }
                }
                else if (red == null || blue == null)
                {
                    isTurning = false;
                    return;
                }
            }
        }
// ---------------------------------------------------------------END MISSILE 1--------------------------------------------------------------------------

// --------------------------------------------------------------START MISSILE 2-------------------------------------------------------------------------
    // Function that handle Sticky Missile firing
    public void FireStickyMissile()
    {
        StickyMissile stickyMissile = new StickyMissile();
        stickyMissile.LoadContent();
        stickyMissile.Initialize();
        stickyMissile.Position = Position + (Vector2.Normalize(_cursorVelocity) * Texture.Width / 2);
        stickyMissile.Orientation = Orientation;
        stickyMissile.SetVelocity(Vector2.Normalize(_cursorVelocity)); 

        // Sound Effect
        _stickyMissile.Play();
    }
    
    // Function to update the Mine and Sticky Missile status
    public void CheckSticky(float realTime)
    {
        List<Mine> mineToDestroy = new List<Mine>();
        List<Mine> mineDestroyed = new List<Mine>();
        List<Asteroid> asteroidToDestroy = new List<Asteroid>();
        List<Asteroid> asteroidDestroyed = new List<Asteroid>();
        GameObject[] asteroids = GameObjectCollection.FindObjectsByType(typeof(Asteroid));
        GameObject[] mines = GameObjectCollection.FindObjectsByType(typeof(Mine));
        
        // Update the sticky missile's status
        if (asteroids != null)
        {        
            // If the stickyMissile collide with asteroid, it will fire the mine earlier
            foreach (Asteroid asteroid in asteroids)
            {
                if (GameObjectCollection.FindByName("StickyMissile") != null)
                {
                    var stickyMissile = GameObjectCollection.FindByName("StickyMissile");
                    if (CollisionEngine.AABB(asteroid, (StickyMissile)stickyMissile))
                    {
                        stickyTime = 0;
                        FireMine(realTime);
                    }
                } 
            }

            // Update the mine's status
            if (mines != null )
            {          
                // If Mine collide with other Asteroid before target, stick on that Asteroid instead
                foreach (Mine mine in mines)
                {
                    foreach (Asteroid asteroid in asteroids)
                    {
                        // If the mine initially does not have target but hit the new spawned asteroid
                        if (!pairs.ContainsKey(mine) && CollisionEngine.AABB(mine, asteroid))
                        {
                            pairs[mine] = asteroid;
                        }
                        else if (CollisionEngine.AABB(mine, asteroid) && CollisionEngine.NotOneInsideAnother(mine, pairs[mine]) && !pairs.ContainsValue(asteroid))
                        {
                            pairs[mine] = asteroid;
                        }
                    }
                }
                foreach ((Mine mine, Asteroid asteroid) in pairs)
                {                 
                    // When the asteroid is destroyed before the mine stick on it
                    if (!asteroids.Contains(asteroid) && CollisionEngine.NotOneInsideAnother(mine, asteroid))
                    {
                        continue;
                    }
                    // When the asteroid with mine is destroyed before explode
                    else if (!asteroids.Contains(asteroid))
                    {
                        // To prevent duplicate value in the arrays
                        if (!mineToDestroy.Contains(mine))
                        {
                            mineToDestroy.Add(mine);
                        }
                        
                        Explosion explosion = new Explosion(0);
                        explosion.LoadContent();
                        explosion.Initialize();
                        explosion.Position = mine.Position;
                        _explosion_2.Play();
                        continue;
                    }
                    
                    // When the mine is about to explode
                    if (CollisionEngine.AABB(mine, asteroid))
                    {
                        mine.Position = asteroid.Position;

                        if (realTime - explodeTime > 3)
                        {
                            // To prevent duplicate value in the arrays
                            if (!mineToDestroy.Contains(mine))
                            {
                                mineToDestroy.Add(mine);
                            }
                            if (!asteroidToDestroy.Contains(asteroid))
                            {
                                asteroidToDestroy.Add(asteroid);
                            }
                            
                            Explosion explosion = new Explosion(0);
                            explosion.LoadContent();
                            explosion.Initialize();
                            explosion.Position = mine.Position;
                            _explosion_2.Play();
                        }
                    }
                }
            }
            else 
            {
                // If no Mine on the screen then no Mine needs to be destroyed
                mineToDestroy.Clear();
            }
        }
    
        foreach (Mine mine in mineToDestroy)
        {
            // Make sure the Mine is still there before deinstantiate
            if (mines.Contains(mine) && !mineDestroyed.Contains(mine))
            {
                GameObjectCollection.DeInstantiate(mine);
                mineDestroyed.Add(mine);
            }
            pairs.Remove(mine);
        }
        foreach (Asteroid asteroid in asteroidToDestroy)
        {
            if (asteroids.Contains(asteroid) && !asteroidDestroyed.Contains(asteroid))
            {
                GameObjectCollection.DeInstantiate(asteroid);
                asteroidDestroyed.Add(asteroid);
            }
        }
    }

    // Function that handle the spread of Mines
    public void FireMine(float realTime)
    {
        var stickyMissile = GameObjectCollection.FindByName("StickyMissile");
        if (realTime - stickyTime > 3 && stickyMissile != null)
        {
            for (int i = 0; i < 5; i++)
            {
                Mine mine = new Mine();
                mine.LoadContent();
                mine.Initialize();
                mine.Position = stickyMissile.Position;
                GameObjectCollection.DeInstantiate(stickyMissile);
                Asteroid target = GetClosestAsteroid(mine, pairs);
                if (target == null)
                {
                    mine.SetVelocity(Vector2.Normalize(ComputeRandomPosition()));
                }
                else
                {
                    pairs.Add(mine, target);
                    Intercepting(target, mine);
                }
            }
            explodeTime = realTime;
            stickyTime = float.PositiveInfinity;
            _explosion_1.Play();
        }
    }
    
    // Intercepting for Mine and targeted Asteroid
    public void Intercepting(Asteroid asteroid, Mine mine)
    {
        float speed = mine.GetSpeed();
        Vector2 pq = asteroid.Position - mine.Position;
        float A = asteroid.GetVelocity().LengthSquared() - (speed * speed);
        float B = 2.0f * Vector2.Dot(pq, asteroid.GetVelocity());
        float C = pq.LengthSquared();
        float Q = B * B - 4.0f * A * C;
        float time = 0f;

        if (Math.Abs(Q) <= 0.0001f)
        {
            time = -B / (2.0f * A);
            if (time < 0.0f)
                time = 0.0f;
        }
        else if (Q > 0.0f)
        {
            float sqrtQ = (float)Math.Sqrt(Q);
            float time1 = Math.Max(0.0f, (-B - sqrtQ) / (2.0f * A));
            float time2 = Math.Max(0.0f, (-B + sqrtQ) / (2.0f * A));
            
            if (time1 > 0.0f && time2 > 0.0f)
            {
                time = Math.Min(time1, time2);
            }
            else
            {
                time = Math.Max(time1, time2);
            }
        }

        if (time > 0.0f)
        {
            Vector2 velocity = pq / time + asteroid.GetVelocity();
            mine.SetVelocity(Vector2.Normalize(velocity));
        }
        else
        {
            mine.SetVelocity(Vector2.Normalize(ComputeRandomPosition()));
        }
    }

    // Function to get the closest target for Mine
    public Asteroid GetClosestAsteroid(Mine mine, Dictionary<Mine, Asteroid> notTarget)
    {
        GameObject[] gameObjects = GameObjectCollection.FindObjectsByType(typeof(Asteroid));
        if (gameObjects == null)
        {
            return null;
        }
        GameObject target = null;
        float min = float.PositiveInfinity;
        foreach (GameObject gameObject in gameObjects)
        {
            if (!notTarget.ContainsValue((Asteroid)gameObject))
            {
                float distance = (gameObject.Position - mine.Position).Length();
                if (min > distance)
                {
                    min = distance;
                    target = gameObject;
                }
            }
        }
        return (Asteroid)target;
    }

    // Function to compute the velocity for Mine if it doesnt have target
    private Vector2 ComputeRandomPosition()
    {
        Random _rand = new Random();
        float width = _game.Graphics.PreferredBackBufferWidth;
        float height = _game.Graphics.PreferredBackBufferHeight;
        float x, y;
        
        while (true)
        {
            x = _rand.NextSingle() * (width + 100);
            y = _rand.NextSingle() * (height + 100);
            if (x < 50 || x > width + 50 || y < 50 || y > height + 50)
            {
                x -= 50;
                y -= 50;
                break;
            }
        }
            
        Vector2 position = new Vector2(x, y);

        return position;
    }
// ---------------------------------------------------------------END MISSILE 2--------------------------------------------------------------------------
        public string GetGroupName()
        {
            return GetType().Name;
        }

        public Rectangle GetBound()
        {
            _rectangle.Location = (Position - Origin).ToPoint();
            return _rectangle;
        }
    }
}