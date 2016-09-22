using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using System.Collections;
using System;

namespace _3DAsteroids
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 
    static class Constants
    {
        public const int OUTERLIMIT = 50; //the outer +- x, y, and z of the universe (as a cube)
        public const float SHOOTTIMER = 0.5F; //2 bullets per second
        public const float MAXSPEED = 0.5F; //The max speed the ship can reach with thrusters

        public const int SMALLSIZE = 100; //small universe size
        public const int MEDIUMSIZE = 200; //medium universe size
        public const int LARGESIZE = 300; //large universe size

        public const int SMALLASTEROIDS = 20; //number of asteroids in a small universe
        public const int MEDIUMASTEROIDS = 30; //number of asteroids in a medium universe
        public const int LARGEASTEROIDS = 40; //number of asteroids in a large universe

        public const int EASYMODE = 100; //all asteroids will break when colliding
        public const int MEDMODE = 50;
        public const int HARDMODE = 0; //all asteroids never break on collision

        public enum PowerUpType
        {
            Fuel,
            MultiShot,
            Speed
        }

    }

    public class Game1 : Game
    {

        private enum GameStates
        {
            StartMenu,
            Controls,
            PrePlaying,
            Playing,
            Dead,
            End
        }

        public enum GameDifficulties
        {
            Easy,
            Medium,
            Hard
        }

        public enum GameSize
        {
            Small,
            Medium,
            Large
        }




        private GameStates currentState;
        public GameDifficulties currentDifficulty;
        public GameSize currentSize;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont myFont;

        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 100, 100), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 2000f);

        private Model skyboxModel;
        private Texture2D menuTexture;

        //private Texture2D blackScreen;

        private Texture2D titleText;
        private Texture2D controlText;

        private Random rand;

        private float gameTimer;

        private bool goodToGo;

        private Model[] asteroidModels;
        private ArrayList allAsteroids;

        private Model fuelModel;
        private Model speedModel;
        private Model multiModel;
        private Powerup[] allItems;
        private int currentItem;
        private float itemTimer;

        public int asteroidsStart;
        public int asteroidsLeft;

        private Player player;
        private Model bullet;

        private Texture2D[] buttonTextures;

        private Explosion[] allExplosions;
        private Texture2D explosion;
        private int currentExplosion;

        private int lives;

        private SoundEffect missileShot;
        private SoundEffect explosionSound;
        private SoundEffect powerupSound;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            allAsteroids = new ArrayList();
            allItems = new Powerup[20];

            rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            player = new Player();

            currentState = GameStates.StartMenu;

            buttonTextures = new Texture2D[11];
            asteroidModels = new Model[7];
            currentDifficulty = GameDifficulties.Medium;
            currentSize = GameSize.Medium;

            allExplosions = new Explosion[15]; //should be enough

            for (int i = 0; i < allExplosions.Length; i++)
            {
                allExplosions[i] = new Explosion();
            }


            lives = 3;

            gameTimer = 0;

            goodToGo = false;

            base.Initialize();


        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //player.model = Content.Load<Model>("karenspaceship");
            player.model = Content.Load<Model>("droidShip");

            //http://thebrb.com/stockpile/files/starfieldimage/starfield.png
            skyboxModel = Content.Load<Model>("skyboxStarField");
            menuTexture = Content.Load<Texture2D>("menuImage");


            //blackScreen = Content.Load<Texture2D>("blackScreen");


            asteroidModels[0] = Content.Load<Model>("Asteroids/asteroid1");
            asteroidModels[1] = Content.Load<Model>("Asteroids/asteroid2");
            asteroidModels[2] = Content.Load<Model>("Asteroids/asteroid3");
            asteroidModels[3] = Content.Load<Model>("Asteroids/asteroid4");
            asteroidModels[4] = Content.Load<Model>("Asteroids/asteroid5");
            asteroidModels[5] = Content.Load<Model>("Asteroids/asteroid6");
            //asteroidModel = Content.Load<Model>("Asteroids/asteroid1");
            bullet = Content.Load<Model>("bullet");
            myFont = Content.Load<SpriteFont>("myFont");

            buttonTextures[0] = Content.Load<Texture2D>("Buttons/Easy1");
            buttonTextures[1] = Content.Load<Texture2D>("Buttons/Easy2");
            buttonTextures[2] = Content.Load<Texture2D>("Buttons/Medium1");
            buttonTextures[3] = Content.Load<Texture2D>("Buttons/Medium2");
            buttonTextures[4] = Content.Load<Texture2D>("Buttons/Hard1");
            buttonTextures[5] = Content.Load<Texture2D>("Buttons/Hard2");
            buttonTextures[6] = Content.Load<Texture2D>("Buttons/Small1");
            buttonTextures[7] = Content.Load<Texture2D>("Buttons/Small2");
            buttonTextures[8] = Content.Load<Texture2D>("Buttons/Large1");
            buttonTextures[9] = Content.Load<Texture2D>("Buttons/Large2");

            explosion = Content.Load<Texture2D>("explosion");

            missileShot = Content.Load<SoundEffect>("missileFire");
            explosionSound = Content.Load<SoundEffect>("ExplosionSound");
            powerupSound = Content.Load<SoundEffect>("powerupGet");
            player.setSound(missileShot);

            fuelModel = Content.Load<Model>("fuelItem");
            speedModel = Content.Load<Model>("speedItem");
            multiModel = Content.Load<Model>("multiItem");

            titleText = Content.Load<Texture2D>("titleText");
            controlText = Content.Load<Texture2D>("ControlsText");



            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds; //the change in time since last update;

            if (currentState == GameStates.StartMenu)
            {
                //check for option changes
                //Mouse.GetState().
                this.IsMouseVisible = true;

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    Point mousePos = Mouse.GetState().Position;

                    if (mousePos.Y >= 200 && mousePos.Y <= 280)
                    {
                        if (mousePos.X >= 120 && mousePos.X <= 280)
                        {
                            currentDifficulty = GameDifficulties.Easy;
                        }
                        if (mousePos.X >= 320 && mousePos.X <= 480)
                        {
                            currentDifficulty = GameDifficulties.Medium;
                        }
                        if (mousePos.X >= 520 && mousePos.X <= 680)
                        {
                            currentDifficulty = GameDifficulties.Hard;
                        }

                    }

                    if (mousePos.Y >= 330 && mousePos.Y <= 410)
                    {
                        if (mousePos.X >= 120 && mousePos.X <= 280)
                        {
                            currentSize = GameSize.Small;
                        }
                        if (mousePos.X >= 320 && mousePos.X <= 480)
                        {
                            currentSize = GameSize.Medium;
                        }
                        if (mousePos.X >= 520 && mousePos.X <= 680)
                        {
                            currentSize = GameSize.Large;
                        }

                    }

                }

                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {

                    currentState = GameStates.Controls;

                }
            }

            if (currentState == GameStates.Controls)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && goodToGo)
                {

                    currentState = GameStates.PrePlaying;

                }
                if (Keyboard.GetState().IsKeyUp(Keys.Enter))
                {
                    goodToGo = true;
                }
            }


            if (currentState == GameStates.PrePlaying)
            {
                //create our starting asteroids
                int numAsteroids = 0;
                int outerLimit = 0;
                if (currentSize == GameSize.Small)
                {
                    numAsteroids = Constants.SMALLASTEROIDS;
                    outerLimit = Constants.SMALLSIZE;
                }
                if (currentSize == GameSize.Medium)
                {
                    numAsteroids = Constants.MEDIUMASTEROIDS;
                    outerLimit = Constants.MEDIUMSIZE;
                }
                if (currentSize == GameSize.Large)
                {
                    numAsteroids = Constants.LARGEASTEROIDS;
                    outerLimit = Constants.LARGESIZE;
                }
                asteroidsStart = numAsteroids;

                int difficulty = 0;
                if (currentDifficulty == GameDifficulties.Easy)
                {
                    difficulty = Constants.EASYMODE;
                }
                if (currentDifficulty == GameDifficulties.Medium)
                {
                    difficulty = Constants.MEDMODE;
                }
                if (currentDifficulty == GameDifficulties.Hard)
                {
                    difficulty = Constants.HARDMODE;
                }

                for (int i = 0; i < numAsteroids; i++)
                {
                    allAsteroids.Add(new Asteroid(rand, outerLimit, difficulty));
                    asteroidsLeft++;
                }


                int j = 0;
                foreach (Asteroid a in allAsteroids)
                {
                    if (j % 2 == 0)
                    {
                        a.bigModel = asteroidModels[0];
                        a.medModel = asteroidModels[1];
                        a.smallModel = asteroidModels[2];
                    }
                    else
                    {
                        a.bigModel = asteroidModels[3];
                        a.medModel = asteroidModels[4];
                        a.smallModel = asteroidModels[5];
                    }
                    j++;
                }

                player.setBounds(outerLimit);


                for (int i = 0; i < 20; i++)
                {
                    Powerup p = new Powerup(rand, outerLimit);
                    p.setModel(fuelModel, multiModel, speedModel);
                    allItems[i] = (p);
                }

                this.IsMouseVisible = false;


                currentState = GameStates.Playing;
            }



            if (currentState == GameStates.Playing)
            {

                player.playerFlight(); //keyboard flight
                updateCamera(); //update camera position

                gameTimer += delta;


                itemTimer += delta;
                //spawn an item every 15 seconds
                if (itemTimer > 15)
                {
                    allItems[currentItem].spawnPowerup();
                    currentItem++;
                    currentItem %= (allItems.Length);
                    itemTimer = 0;
                }

                //have all asteroid float in space
                foreach (Asteroid a in allAsteroids)
                {
                    if (a.isActive)
                    {
                        a.updatePosition(delta);
                    }
                }

                //update player stuff
                player.updateTimer(delta);

                //update explosions
                foreach (Explosion e in allExplosions)
                {
                    if (e.isExploding)
                    {
                        e.updateTimer(delta);
                    }
                }

                //bullets also fly through space
                foreach (Bullet b in player.bulletSupply)
                {
                    b.updateBullet();
                    if (b.outOfBounds() && b.isActive)
                    {
                        //EXPLODE!
                        createExplosion(b.position);
                        b.isActive = false;
                        explosionSound.Play();
                    }
                }

                foreach (Powerup p in allItems)
                {
                    if (p.isActive)
                    {
                        p.updatePosition(delta);
                    }
                }



                //see if stuff is colliding with other stuff
                checkCollisions();




                if (asteroidsStart * .25F >= asteroidsLeft)
                {
                    //You Win!
                    currentState = GameStates.End;
                }
            }



            if (currentState == GameStates.Dead)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    player.position = Vector3.Zero;
                    currentState = GameStates.Playing;
                }

                if (lives == 0)
                {
                    currentState = GameStates.End;
                }

            }

            if (currentState == GameStates.End)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    Exit();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DeepSkyBlue);
            Vector2 center = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);

            Rectangle screen = new Rectangle(0, 0, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height);


            if (currentState == GameStates.StartMenu)
            {
                spriteBatch.Begin();

                spriteBatch.Draw(menuTexture, screen, Color.White);

                spriteBatch.Draw(titleText, new Vector2(center.X - 160, 20), Color.White);

                string pressEnter = "Press Enter to Start";
                Vector2 stringLen = myFont.MeasureString(pressEnter);
                spriteBatch.DrawString(myFont, pressEnter, center - stringLen / 2, Color.White);

                string selectDifficulty = "Select Your Difficulty";
                stringLen = myFont.MeasureString(selectDifficulty);
                spriteBatch.DrawString(myFont, selectDifficulty, new Vector2(center.X - stringLen.X / 2, 170), Color.White);

                if (currentDifficulty == GameDifficulties.Easy)
                {
                    spriteBatch.Draw(buttonTextures[0], new Vector2(120, 200), Color.White);
                }
                else
                {
                    spriteBatch.Draw(buttonTextures[1], new Vector2(120, 200), Color.White);
                }
                if (currentDifficulty == GameDifficulties.Medium)
                {
                    spriteBatch.Draw(buttonTextures[2], new Vector2(320, 200), Color.White);
                }
                else
                {
                    spriteBatch.Draw(buttonTextures[3], new Vector2(320, 200), Color.White);
                }
                if (currentDifficulty == GameDifficulties.Hard)
                {
                    spriteBatch.Draw(buttonTextures[4], new Vector2(520, 200), Color.White);
                }
                else
                {
                    spriteBatch.Draw(buttonTextures[5], new Vector2(520, 200), Color.White);
                }





                string selectSize = "Select Universe Size";
                stringLen = myFont.MeasureString(selectSize);
                spriteBatch.DrawString(myFont, selectSize, new Vector2(center.X - stringLen.X / 2, 300), Color.White);

                if (currentSize == GameSize.Small)
                {
                    spriteBatch.Draw(buttonTextures[6], new Vector2(120, 330), Color.White);
                }
                else
                {
                    spriteBatch.Draw(buttonTextures[7], new Vector2(120, 330), Color.White);
                }
                if (currentSize == GameSize.Medium)
                {
                    spriteBatch.Draw(buttonTextures[2], new Vector2(320, 330), Color.White);
                }
                else
                {
                    spriteBatch.Draw(buttonTextures[3], new Vector2(320, 330), Color.White);
                }
                if (currentSize == GameSize.Large)
                {
                    spriteBatch.Draw(buttonTextures[8], new Vector2(520, 330), Color.White);
                }
                else
                {
                    spriteBatch.Draw(buttonTextures[9], new Vector2(520, 330), Color.White);
                }


                string beginSize = "Select with Mouse, Enter to Continue!";
                stringLen = myFont.MeasureString(beginSize);
                spriteBatch.DrawString(myFont, beginSize, new Vector2(center.X - stringLen.X / 2, 430), Color.White);

                spriteBatch.End();
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            if (currentState == GameStates.Controls)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(menuTexture, screen, Color.White);
                spriteBatch.Draw(controlText, screen, Color.White);
                spriteBatch.End();
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }

            if (currentState == GameStates.Playing)
            {


                //world = Matrix.CreateRotationY(MathHelper.Pi) *
                //       Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z) * Matrix.CreateTranslation(position);
                world = Matrix.CreateRotationX(-MathHelper.Pi) * Matrix.CreateFromQuaternion(player.rotation) * Matrix.CreateTranslation(player.position);

                //draw the player
                DrawModel(player.model, world, view, projection);

                //draw all our asteroids a bit bigger than normal
                foreach (Asteroid a in allAsteroids)
                {
                    //Matrix.CreateFromYawPitchRoll(a.rotation.X, a.rotation.Y, a.rotation.Z) rotate the stuff. DONT FORGET TO DO THIS IN COLLISION CHECK TOO!
                    if (a.isActive)
                    {
                        DrawModel(a.getModel(), a.getWorldMatrix(), view, projection);

                    }
                }

                //Draw the cylinder longer because it's a stupid bullet not a cylinder
                //DrawModel(bullet, Matrix.CreateScale(1, 1, 5) * Matrix.CreateTranslation(new Vector3(1, 1, 1)), view, projection);

                DrawSkybox(skyboxModel, Matrix.CreateScale(400, 400, 400), view, projection);


                //draw all the bullets
                foreach (Bullet b in player.bulletSupply)
                {
                    if (b.isActive)
                    {
                        DrawModel(bullet, b.getWorldMatrix(), view, projection);
                    }
                }

                foreach (Powerup p in allItems)
                {
                    if (p.isActive)
                    {
                        DrawModel(p.getModel(), p.getWorldMatrix(), view, projection);
                    }
                }


                int gameMins = (int)TimeSpan.FromSeconds(gameTimer).TotalMinutes;
                int gameSecs = (int)TimeSpan.FromSeconds(gameTimer).TotalSeconds % 60;
                int gameMilli = (int)TimeSpan.FromSeconds(gameTimer).TotalMilliseconds;

                //String gametimeString = "" + gameMins.ToString("D2") + ":" + gameSecs.ToString("D2") + ":" + gameMilli.ToString("D3");
                String gametimeString = "Time Elapsed: " + gameMins.ToString("D2") + ":" + gameSecs.ToString("D2");

                spriteBatch.Begin();
                //spriteBatch.DrawString(myFont, "Asteroids Left: " + asteroidsLeft, new Vector2(20, 20), Color.White);
                spriteBatch.DrawString(myFont, "Lives Left: " + lives, new Vector2(100, GraphicsDevice.Viewport.Height - 20), Color.White);
                //spriteBatch.DrawString(myFont, "Player Pos: " + player.position.ToString(), new Vector2(20, 60), Color.Black);
                //spriteBatch.DrawString(myFont, "" + asteroidsStart *.25, new Vector2(20, 60), Color.Black);

                spriteBatch.DrawString(myFont, gametimeString, new Vector2(GraphicsDevice.Viewport.Width - 250, GraphicsDevice.Viewport.Height - 20), Color.White);

                spriteBatch.DrawString(myFont, "Fuel Remaining: " + Math.Ceiling(player.fuel), new Vector2(100, 20), Color.White);

                String upgradesString = "Upgrades Active: ";
                if (player.doubleSpeed) upgradesString += "Double Speed ";
                if (player.multiBullet) upgradesString += "Multi Bullets ";
                spriteBatch.DrawString(myFont, upgradesString, new Vector2(GraphicsDevice.Viewport.Width - 250, 20), Color.White);



                spriteBatch.End();


                //draw explosions if any
                foreach (Explosion e in allExplosions)
                {
                    if (e.isExploding == true)
                    {
                        //Pretty sure this came from https://blogs.msdn.microsoft.com/shawnhar/2011/01/12/spritebatch-billboards-in-a-3d-world/
                        BasicEffect basicEffect = new BasicEffect(GraphicsDevice);

                        Vector3 camPos = new Vector3(0, 0, 0.5F);
                        camPos = Vector3.Transform(camPos, Matrix.CreateFromQuaternion(player.rotation));
                        camPos += player.position;

                        basicEffect.World = Matrix.CreateConstrainedBillboard(e.position, camPos, Vector3.Down, null, null);
                        basicEffect.View = view;
                        basicEffect.Projection = projection;
                        basicEffect.TextureEnabled = true;

                        spriteBatch.Begin(0, null, null, null, null, basicEffect);
                        spriteBatch.Draw(explosion, new Rectangle(((int)e.position.X - explosion.Width / 2) / 40, ((int)e.position.Y - explosion.Height / 2) / 40, 5, 5), Color.White);
                        spriteBatch.End();

                    }
                }



                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }

            if (currentState == GameStates.Dead)
            {
                spriteBatch.Begin();
                string livesSize = "Lives Remaining: " + lives;
                Vector2 stringLen = myFont.MeasureString(livesSize);
                spriteBatch.Draw(menuTexture, screen, Color.White);
                spriteBatch.DrawString(myFont, livesSize, new Vector2(center.X - stringLen.X / 2, center.Y - stringLen.Y / 2), Color.White);
                string EnterSize = "Press Enter to Respawn";
                stringLen = myFont.MeasureString(EnterSize);
                spriteBatch.DrawString(myFont, EnterSize, new Vector2(center.X - stringLen.X / 2, 100 + center.Y - stringLen.Y / 2), Color.White);


                spriteBatch.End();
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            }

            if (currentState == GameStates.End)
            {
                if (lives == 0)
                {
                    //You lose!
                    spriteBatch.Begin();
                    spriteBatch.Draw(menuTexture, screen, Color.White);
                    string lose = "You lose!";
                    Vector2 stringLen = myFont.MeasureString(lose);
                    spriteBatch.DrawString(myFont, lose, new Vector2(center.X - stringLen.X / 2, center.Y - stringLen.Y / 2), Color.White);

                    string enter = "Enter to Quit";
                    stringLen = myFont.MeasureString(enter);
                    spriteBatch.DrawString(myFont, enter, new Vector2(center.X - stringLen.X / 2, 200 + center.Y - stringLen.Y / 2), Color.White);


                    spriteBatch.End();
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                }
                else
                {
                    //You Win!

                    int gameMins = (int)TimeSpan.FromSeconds(gameTimer).TotalMinutes;
                    int gameSecs = (int)TimeSpan.FromSeconds(gameTimer).TotalSeconds % 60;
                    int gameMilli = (int)TimeSpan.FromSeconds(gameTimer).TotalMilliseconds;

                    //String gametimeString = "" + gameMins.ToString("D2") + ":" + gameSecs.ToString("D2") + ":" + gameMilli.ToString("D3");
                    String gametimeString = "Time Elapsed: " + gameMins.ToString("D2") + ":" + gameSecs.ToString("D2");

                    spriteBatch.Begin();
                    spriteBatch.Draw(menuTexture, screen, Color.White);
                    string win = "You win!";
                    Vector2 stringLen = myFont.MeasureString(win);
                    spriteBatch.DrawString(myFont, win, new Vector2(center.X - stringLen.X / 2, center.Y - stringLen.Y / 2), Color.White);
                    stringLen = myFont.MeasureString(gametimeString);
                    spriteBatch.DrawString(myFont, gametimeString, new Vector2(center.X - stringLen.X / 2, 100 + center.Y - stringLen.Y / 2), Color.White);

                    string enter = "Enter to Quit";
                    stringLen = myFont.MeasureString(enter);
                    spriteBatch.DrawString(myFont, enter, new Vector2(center.X - stringLen.X / 2, 200 + center.Y - stringLen.Y / 2), Color.White);


                    spriteBatch.End();
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                }


            }

            // TODO: Add your drawing code here

            // spriteBatch.Begin();
            // spriteBatch.Draw(skyboxBack, new Rectangle(0, 0, 400, 240), Color.White);
            //  spriteBatch.End();






            base.Draw(gameTime);
        }

        /// <summary>
        /// 
        /// Draws a given model at a given location from a given view/projection. Took it from
        /// http://rbwhitaker.wikidot.com/basic-effect-lighting 
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="world"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.LightingEnabled = true;

                    effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 0.2f, 0.2f); // a reddish light
                    effect.DirectionalLight0.Direction = new Vector3(1, 0, 0);  // coming along the x-axis
                    effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0); // with green highlights

                    effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f); // Add some overall ambient light.

                }

                mesh.Draw();
            }
        }

        /// <summary>
        /// 
        /// Pretty much the exact same as above but no lighting (for skybox)
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="world"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        private void DrawSkybox(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }



        /// <summary>
        /// Taken from https://msdn.microsoft.com/en-us/library/bb203906.aspx (BUT THEN I HAD TO MODIFY IT!)
        /// Will check for collision between two objects, then bounce them back if they are colliding
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        private void CheckForCollision(Asteroid c1, Asteroid c2)
        {

            // Check whether the bounding boxes of the two objects intersect.
            //there is only 1 mesh for my asteroids so this is easier to check
            BoundingSphere c1BoundingSphere = c1.getModel().Meshes[0].BoundingSphere.Transform(c1.getWorldMatrix());
            BoundingSphere c2BoundingSphere = c2.getModel().Meshes[0].BoundingSphere.Transform(c2.getWorldMatrix());
            if (c1BoundingSphere.Intersects(c2BoundingSphere))
            {
                c1.CollisionHit();
                c2.CollisionHit();
                //System.Diagnostics.Debug.Write("Ouch!\n");
                return;
            }
        }


        private void updateCamera()
        {
            Vector3 camPos = new Vector3(0, 0, 0.5F);
            camPos = Vector3.Transform(camPos, Matrix.CreateFromQuaternion(player.rotation));
            camPos += player.position;

            Vector3 cameraUp = new Vector3(0, 1, 0);
            cameraUp = Vector3.Transform(cameraUp, Matrix.CreateFromQuaternion(player.rotation));

            view = Matrix.CreateLookAt(camPos, player.position, cameraUp);

        }

        private void createExplosion(Vector3 location)
        {
            allExplosions[currentExplosion].CreateExplosion(location);
            currentExplosion++;
            currentExplosion %= (allExplosions.Length);
        }


        private void checkCollisions()
        {

            //checks to see if the player is touching an aseroid
            BoundingSphere playerBoundingSphere = player.model.Meshes[0].BoundingSphere.Transform(Matrix.CreateTranslation(player.position));
            foreach (Asteroid a in allAsteroids)
            {
                if (a.isActive)
                {
                    BoundingSphere aBoundSphere = a.getModel().Meshes[0].BoundingSphere.Transform(a.getWorldMatrix());
                    if (playerBoundingSphere.Intersects(aBoundSphere))
                    {
                        //System.Diagnostics.Debug.Write("MAN DOWN\n");
                        //DIE!
                        currentState = GameStates.Dead;
                        lives--;
                    }
                }
            }

            //Is the player hitting an item?
            foreach (Powerup p in allItems)
            {
                if (p.isActive)
                {
                    BoundingSphere aBoundSphere = p.getModel().Meshes[0].BoundingSphere.Transform(p.getWorldMatrix());
                    if (playerBoundingSphere.Intersects(aBoundSphere))
                    {
                        //System.Diagnostics.Debug.Write("Got a powerup!\n");
                        //Find Item type
                        powerupSound.Play();
                        Constants.PowerUpType thisType = p.PickUp();
                        if (thisType == Constants.PowerUpType.Fuel)
                        {
                            player.addFuel(25);
                        }
                        if (thisType == Constants.PowerUpType.MultiShot)
                        {
                            player.multiBullet = true;
                        }
                        if (thisType == Constants.PowerUpType.Speed)
                        {
                            player.doubleSpeed = true;
                        }
                    }
                }
            }

            //checks to see if an asteroid is colliding with another asteroid
            foreach (Asteroid a in allAsteroids)
            {
                foreach (Asteroid b in allAsteroids)
                {
                    if (!a.Equals(b) && a.isActive && b.isActive)
                    {
                        CheckForCollision(a, b);
                    }
                }
            }

            //checks if a bullet is hitting an asteroid
            foreach (Bullet b in player.bulletSupply)
            {
                if (b.isActive)
                {
                    BoundingBox bulletBox = UpdateBoundingBox(bullet, b.getWorldMatrix());
                    foreach (Asteroid a in allAsteroids)
                    {
                        if (a.isActive)
                        {
                            BoundingSphere aBoundSphere = a.getModel().Meshes[0].BoundingSphere.Transform(a.getWorldMatrix());
                            if (bulletBox.Intersects(aBoundSphere))
                            {
                                //System.Diagnostics.Debug.Write("Asteroid Shot Down!\n");
                                //Destroy asteroid!!
                                if (a.hit()) asteroidsLeft--; //destroy the asteroid
                                b.isActive = false; //destory the bullet
                                //asteroidsLeft--;
                                //TODO MAKE AN EXPLOSION HERE! 
                                explosionSound.Play();
                                createExplosion(a.position);
                            }
                        }
                    }
                }
            }


        }


        /// <summary>
        /// Taken from http://gamedev.stackexchange.com/questions/2438/how-do-i-create-bounding-boxes-with-xna-4-0 
        /// by user tunnez.
        /// </summary>
        /// <param name="model">The model that is being used as a base for the boundingBox</param>
        /// <param name="worldTransform">The location of the object in world space</param>
        /// <returns></returns>

        protected BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
        }




    }
}
