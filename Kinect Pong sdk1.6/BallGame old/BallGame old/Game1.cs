using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;
using kinect;
using System.IO;


namespace BallGame_old
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Boolean errornokinect = true;
        KeyboardState keys;

        // Screen Size
        int displaywidth = 800; 
        int displayheight = 600;

        // Variable to generate random numbers
        Random randomiser = new Random();

        const int maxscore = 100;
        Boolean gameover = false;

        SoundEffect lightsabre, yoda, darth;
        float vib1, vib2; // Amount of vibration on pads

        // Structure for moving 2d graphics
        struct graphics2d
        {
            public Texture2D image;         // Holds 2d graphic
            public Vector3 position;        // On screen position
            public Vector3 oldposition;     // Position of graphic before collision occurs
            public Rectangle rect;          // Holds position and dimensions of graphic
            public Vector2 origin;          // Position on image which the position points to
            public float size;              // Size ratio to scale the image up to
            public Vector3 velocity;        // Direction & Speed of graphic
            public BoundingSphere bsphere;  // Bounding sphere for graphic
            public BoundingBox bbox;        // Bounding box for graphic
            public float power;             // Power of ship with regards to speed
            public float rotation;          // Amount of rotation to apply
            public float rotationspeed;     // Speed at which the ship can turn
            public int score;               // Score
            public Boolean kinectactive;
        }

        graphics2d background;                 // Background picture
        Boolean drawbackground = false;        // Should the backgrond image be drawn or the video feed
        graphics2d[] bat = new graphics2d[2];  // Array of bats
        graphics2d ball;                       // Ball graphic

        SpriteFont mainfont;        // Font for drawing text on the screen
        SpriteFont secondaryfont;

        float move_marginoferror = 1;

        // Variables for Kinect
        KinectSensor kinectSensor;
        Texture2D kinectRGBVideo;
        ColorImageFrame videoframe;
        int cameraangle = 0;
        SkeletonFrame body;

        const int framespersec =24;
        float milliperframe = 1000f / (float)framespersec;
        float framecount = 0;

        float kinectscale = 0.3f;    // Kinect movement scale

        int actualFPS = 0;
        int framecounter = 0;
        float frametimer = 0;
        float cameracounter = 0;

        Boolean keyschanged = true;

        // Create a new render target for rendering images for saving
//        RenderTarget2D renderTarget;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = false;
//            this.IsMouseVisible = true;

            this.graphics.PreferredBackBufferWidth = displaywidth;
            this.graphics.PreferredBackBufferHeight = displayheight;
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
//            renderTarget = new RenderTarget2D(GraphicsDevice, displaywidth, displayheight, false,
  //              GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            base.Initialize();
        }

        void setupkinect()
        {
            // Initialise Kinect
            try
            {
                // Enable the Kinect Sensor for Video and Skeletal data
                kinectSensor = KinectSensor.KinectSensors[0];
                kinectSensor.SkeletonStream.Enable();
                kinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                kinectSensor.Start();
                kinectSensor.ElevationAngle = cameraangle;
              //  kinectSensor.DepthStream.Range = DepthRange.Near;

                errornokinect = false;
            }
            catch
            {
                errornokinect = true;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            background.image = Content.Load<Texture2D>("fullmoon43");   // Load bullet image
            background.rect.X = 0;
            background.rect.Y = 0;
            background.rect.Width = displaywidth;
            background.rect.Height = displayheight;

            bat[0].image = Content.Load<Texture2D>("obisaber"); // Load bat graphic for player1
            bat[1].image = Content.Load<Texture2D>("vadersaber"); // Load bat graphic for player2

            for (int count = 0; count < bat.Count(); count++)
            {
                bat[count].size = 0.2f;                                     // Set size ratio
                bat[count].origin.X = bat[count].image.Width / 2;           // Set origin to center of image
                bat[count].origin.Y = bat[count].image.Height / 2;          // as above
                bat[count].rect.Width = (int)(bat[count].image.Width * bat[count].size);     // Set size of rectangle based on size ratio
                bat[count].rect.Height = (int)(bat[count].image.Height * bat[count].size);   // as above
                bat[count].rotation = 0;                                    // Set initial rotation 
                bat[count].power = 0.5f;                                    // Set Power of bat with regards to speed
            }
            
            //hit = Content.Load<SoundEffect>("ballhit");
            lightsabre = Content.Load<SoundEffect>("Saberblk");
            yoda = Content.Load<SoundEffect>("strongam");
            darth = Content.Load<SoundEffect>("darkside");

            ball.image = Content.Load<Texture2D>("starwarsremoteball");   // Load ball image
            ball.size = 0.15f;                                  // Set size ratio
            ball.origin.X = ball.image.Width / 2;     // Set origin to center of image
            ball.origin.Y = ball.image.Height / 2;    // as above
            ball.rect.Width = (int)(ball.image.Width * ball.size);     // Set size of rectangle based on size ratio
            ball.rect.Height = (int)(ball.image.Height * ball.size);   // as above
            ball.rotationspeed = 0.1f;                           // How fast the ball can spin
 
            
            mainfont = Content.Load<SpriteFont>("quartz4");  // Load the quartz4 font
            secondaryfont = Content.Load<SpriteFont>("miramonte9"); 

            resetgame();

            if (File.Exists(@"options.txt"))
            {
                StreamReader sr = new StreamReader(@"options.txt");	// Open the file
                String line;		// Create a string variable to read each line into
                if (!sr.EndOfStream)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    cameraangle = Convert.ToInt32(line);	// This converts line to numeric
                }
                sr.Close();			// Close the file
            }

            // Set up Kinect to stream video
            kinectRGBVideo = new Texture2D(GraphicsDevice, 640, 480);


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            if (!errornokinect)
                kinectSensor.Stop();

            StreamWriter sw = new StreamWriter(@"options.txt");
            sw.WriteLine(cameraangle.ToString("000"));
            sw.Close();


        }

        void resetgame()
        {
            // Set intial positions of the 2 bats
            bat[0].position = new Vector3(50, displayheight / 2, 0);   
            bat[1].position = new Vector3(displaywidth - 50, displayheight / 2, 0);
            bat[0].kinectactive = false;
            bat[1].kinectactive = false;

            vib1 = 0;
            vib2 = 0;
            gameover = false;
            bat[0].score = 0;
            bat[1].score = 0;
            resetball();
        }

 

        void resetball()
        {
            // Set initial position of ball in middle of screen
            ball.position = new Vector3(displaywidth/2, displayheight / 2, 0);    

            // Generate random velocities for the ball
            do
            {
                ball.velocity.X = (randomiser.Next(3)-1)*(5+randomiser.Next(3));
                ball.velocity.Y = randomiser.Next(5) - 2;
            } while (ball.velocity.X == 0 || ball.velocity.Y == 0);
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            keyschanged = (keys != Keyboard.GetState());    // Check if keyboard input has changed at all
            keys = Keyboard.GetState();                     // Read keyboard
            if (keyschanged && keys.IsKeyDown(Keys.F11)) graphics.ToggleFullScreen(); // Toggle full screen mode

            float gtime = (float)gameTime.ElapsedGameTime.TotalMilliseconds; // Time between updates

            // Work out Frames Per Second
            frametimer += gtime;
            if (frametimer >= 1000)
            {
                actualFPS = framecounter;
                framecounter = 0;
                frametimer = 0;
            }

            if (errornokinect)
            {
                setupkinect();
            }
            else
            {
                // Allow user to alter the Kinect Camera Angle
                if (cameracounter < 0)
                {
                    cameracounter = 1500;
                    cameraangle = kfunctions.cameramove(kinectSensor, cameraangle);
                }

                // Count timers down
                cameracounter -= gtime;

                // Toggle video feed on and off
                if (keyschanged && keys.IsKeyDown(Keys.Space))
                {
                    drawbackground = !drawbackground;
                }
                // Read the joypads
                GamePadState[] pad = new GamePadState[2];
                pad[0] = GamePad.GetState(PlayerIndex.One); // Read gamepad 1
                pad[1] = GamePad.GetState(PlayerIndex.Two); // Read gamepad 2

                const float friction = 0.96f;   // Amount of friction to apply

                if (!gameover)
                {
                    if (keys.IsKeyDown(Keys.Escape))
                        gameover = true;

                    Vector3 p1handpos, p2handpos;

                    // show video feed 
                    framecount += gtime;
                    if (framecount > milliperframe && !drawbackground)
                    {
                        // Stream video from Kinect into imageframes
                        using (videoframe = kinectSensor.ColorStream.OpenNextFrame(0))
                        {
                            if (videoframe != null)
                                kinectRGBVideo = kfunctions.video2texture(graphics, videoframe);
                        }
                        framecount = 0;
                    }

                    
                    // Read Kinect Body sensor
                    using (body = kinectSensor.SkeletonStream.OpenNextFrame(0))
                    {
                        if (body != null)
                        {
                            // Get skeleton body data
                            Skeleton[] bodyskel = new Skeleton[body.SkeletonArrayLength];
                            body.CopySkeletonDataTo(bodyskel);

                            // Read Kinect and look for right hand movements on both players
                            int[] angles = new int[2];
                            kfunctions.readjoints(bodyskel, JointType.HandRight, displaywidth, displayheight, kinectscale, out p1handpos, out p2handpos);

                            if (p1handpos.X != 2000)
                            {
                                if (Math.Abs(p1handpos.Y - bat[0].position.Y) > move_marginoferror)
                                {
                                    bat[0].position.Y = p1handpos.Y;
                                }
                                bat[0].kinectactive = true;
                            }

                            if (Math.Abs(p2handpos.X) != 2000)
                            {
                                if (Math.Abs(p2handpos.Y - bat[1].position.Y) > move_marginoferror)
                                {
                                    bat[1].position.Y = p2handpos.Y;
                                }
                                bat[1].kinectactive = true;
                            }

                            for (int i = 0; i < bat.Count(); i++)
                            {
                                // Set the bat rectangle to the correct position
                                bat[i].rect.X = (int)bat[i].position.X;
                                bat[i].rect.Y = (int)bat[i].position.Y; 
                                bat[i].bbox = new BoundingBox(new Vector3(bat[i].position.X - bat[i].rect.Width / 2, bat[i].position.Y - bat[i].rect.Height / 2, 0), new Vector3(bat[i].position.X + bat[i].rect.Width / 2, bat[i].position.Y + bat[i].rect.Height / 2, 0));
                            }
                        }
                    }

                    // Move the two bats
                    for (int i = 0; i < bat.Count(); i++)
                    {
                            // Set the velocity of the bats based on gamepad input
                            bat[i].velocity.Y -= pad[i].ThumbSticks.Left.Y * bat[i].power;

                            // Move bats based on velocity
                            bat[i].position += bat[i].velocity;
                            // Apply friction
                            bat[i].velocity *= friction;

                            // Alter bat direction when bats hit boundaries
                            // Bat hits the bottom of the screen
                            if (bat[i].position.Y > displayheight - bat[i].rect.Height / 2)
                            {
                                //                        bat[i].velocity.Y = -Math.Abs(bat[i].velocity.Y);
                                bat[i].position.Y = displayheight - bat[i].rect.Height / 2;
                                bat[i].velocity.Y = 0;
                            }
                            // Bat hits the top of the screen
                            if (bat[i].position.Y < bat[i].rect.Height / 2)
                            {
                                //                        bat[i].velocity.Y = Math.Abs(bat[i].velocity.Y);
                                bat[i].position.Y = bat[i].rect.Height / 2;
                                bat[i].velocity.Y = 0;
                            }

                            // Set the bat rectangle to the correct position
                            bat[i].rect.X = (int)bat[i].position.X;
                            bat[i].rect.Y = (int)bat[i].position.Y;

                            // Create bounding box and then check for collisions
                            bat[i].bbox = new BoundingBox(new Vector3(bat[i].position.X - bat[i].rect.Width / 2, bat[i].position.Y - bat[i].rect.Height / 2, 0), new Vector3(bat[i].position.X + bat[i].rect.Width / 2, bat[i].position.Y + bat[i].rect.Height / 2, 0));

                            // Add AI
                            if (!pad[i].IsConnected && !bat[i].kinectactive)
                                if (bat[i].position.Y < ball.position.Y)
                                    bat[i].velocity.Y += bat[i].power;
                                else
                                    bat[i].velocity.Y -= bat[i].power;
                    }

                    ball.position += ball.velocity;     // Move the ball
                    ball.rotation += ball.rotationspeed;// Spin ball
                    ball.velocity *= 1.0005f;            // Speed up the ball over time

                    // If the ball hits the top or the bottom of the screen reverse its direction on the Y axis
                    if (ball.position.Y < ball.rect.Width / 2)
                    {
                        ball.velocity.Y = -ball.velocity.Y;
                        ball.position.Y = ball.rect.Width / 2;
                    }
                    if (ball.position.Y > displayheight - ball.rect.Width / 2)
                    {
                        ball.velocity.Y = -ball.velocity.Y;
                        ball.position.Y = displayheight - ball.rect.Width / 2;
                    }

                    // Check if the ball goes out either side
                    if (ball.position.X > displaywidth + ball.rect.Width)
                    {
                        resetball();
                        bat[0].score += 5;
                        yoda.Play();
                    }
                    if (ball.position.X < -ball.rect.Width)
                    {
                        resetball();
                        bat[1].score += 5;
                        darth.Play();
                    }

                    float ballspeed = ball.velocity.Length(); // Store the speed the ball was moving at
                    ball.velocity.Normalize();

                    Boolean hitball = false;
                    // Create boundingsphere around the ball
                    ball.bsphere = new BoundingSphere(ball.position, ball.rect.Width / 2);

                    // Bounce ball off bat0
                    if (bat[0].bbox.Intersects(ball.bsphere) && bat[0].position.X <= ball.position.X)
                    {
                        hitball = true;
                        ball.velocity.X = Math.Abs(ball.velocity.X);    // Force the ball to go right
                        bat[0].position = bat[0].oldposition;
                        //ball.velocity.Y += (ball.position.Y - bat[0].position.Y) / 20f;   // Adjust the Y velocity depending on where on the bat the ball hits
                        ball.velocity.Y = (ball.position.Y - bat[0].position.Y) / 40f;
                        
                        if (ball.velocity.X < 0.01) ball.velocity.X = 0.1f;

                        lightsabre.Play();     // Play the hit sound
                        vib1 = 1;       // Start pad vibration
                    }

                    // Bounce ball off bat1
                    if (bat[1].bbox.Intersects(ball.bsphere) && bat[1].position.X >= ball.position.X)
                    {
                        hitball = true;
                        ball.velocity.X = -Math.Abs(ball.velocity.X);   // Force the ball to go left
                        bat[1].position = bat[1].oldposition;
                        //ball.velocity.Y += (ball.position.Y - bat[1].position.Y) / 20f;   // Adjust the Y velocity depending on where on the bat the ball hits
                        ball.velocity.Y = (ball.position.Y - bat[1].position.Y) / 40f;

                        if (ball.velocity.X > -0.01) ball.velocity.X = -0.1f;

                        lightsabre.Play();     // Play the hit sound
                        vib2 = 1;       // Start pad vibration
                    }

                    if (!hitball)
                    {
                        ball.oldposition = ball.position;
                        bat[0].oldposition = bat[0].position;
                        bat[1].oldposition = bat[1].position;
                    }
                    else
                    {
                        ball.position = ball.oldposition;
                    }

                    // Normalise the temp velocity and apply the current ball speed back onto it and then reapply it to the ball
                    ball.velocity.Normalize();
                    ball.velocity *= ballspeed;

                    // Set position of ball rectangle to match its position for drawing purposes
                    ball.rect.X = (int)ball.position.X;
                    ball.rect.Y = (int)ball.position.Y;

                    // Vibrate pads
                    GamePad.SetVibration(PlayerIndex.One, vib1, vib1);
                    GamePad.SetVibration(PlayerIndex.Two, vib1, vib2);

                    // Reduce vibration
                    if (vib1 > 0.1f)
                        vib1 -= 0.01f;
                    else
                        vib1 = 0;
                    if (vib2 > 0.1f)
                        vib2 -= 0.01f;
                    else
                        vib2 = 0;

                    // If either player reaches maxscore, end game
                    if (bat[0].score >= maxscore || bat[1].score >= maxscore)
                    {
                        gameover = true;
                    }
                }
                else
                {
                    GamePad.SetVibration(PlayerIndex.One, 0, 0);
                    GamePad.SetVibration(PlayerIndex.Two, 0, 0);

                    // Start game again when the user presses start
                    if (keys.IsKeyDown(Keys.Enter))
                        resetgame();
                    if (keys.IsKeyDown(Keys.Escape))
                        this.Exit();

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
            framecounter++; // Count the frames being drawn

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            if (errornokinect)
            {
                spriteBatch.DrawString(secondaryfont, "Error please connect Kinect to this PC", new Vector2(20, 100),
                    Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(secondaryfont, "Also ensure Kinect SDK v1.5 is installed", new Vector2(20, 200),
                    Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);
            }
            else
            {
                // Draw video
                if (drawbackground)
                    spriteBatch.Draw(background.image, background.rect, Color.White);
                else
                    spriteBatch.Draw(kinectRGBVideo, new Rectangle(0, 0, displaywidth, displayheight), Color.White);  // Draw video from kinect camera

                for (int i = 0; i < bat.Count(); i++)
                {
                    spriteBatch.Draw(bat[i].image, bat[i].rect, null, Color.White, bat[i].rotation, bat[i].origin, SpriteEffects.None, 0);
                }

                spriteBatch.Draw(ball.image, ball.rect, null, Color.White, ball.rotation, ball.origin, SpriteEffects.None, 0);
                spriteBatch.DrawString(mainfont, "P1 Score " + bat[0].score.ToString() , new Vector2(50, 20), Color.White);
                spriteBatch.DrawString(mainfont, "P2 Score " + bat[1].score.ToString(), new Vector2(displaywidth - 250, 20), Color.White);
                spriteBatch.DrawString(mainfont, "FPS " + actualFPS.ToString("0"), new Vector2(displaywidth/2 - 50, displayheight - 20), Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);

                if (bat[0].score >= maxscore)
                    spriteBatch.DrawString(mainfont, "P1 WINS", new Vector2(displaywidth / 2 - 70, displayheight / 2 - 50), Color.Red);

                if (bat[1].score >= maxscore)
                    spriteBatch.DrawString(mainfont, "P2 WINS", new Vector2(displaywidth / 2 - 70, displayheight / 2 - 50), Color.Red);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
