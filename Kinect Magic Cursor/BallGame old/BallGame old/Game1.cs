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
using System.Runtime.InteropServices;
using System.IO;

namespace magiccursor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Boolean errornokinect = false;

        // Screen Size
        int displaywidth = 320;
        int displayheight = 240;

        float mousescale = 0.3f;    // Mouse movement scale

        int screenresx, screenresy; // Windows screen resolution
        Vector2 mousepos = new Vector2(0, 0);   // Position of mouse

        float delaybetweenclicks = 1000;    // Delay between left mouse clicks

        Boolean videofeed = true;       // Video feed on or off
        Boolean kinectactive = true;    // Kinect mouse control on or off
        Boolean kclicked = false;       // Check for K button being released before toggling control


        // Variable to generate random numbers
        Random randomiser = new Random();

        SpriteFont mainfont;        // Font for drawing text on the screen
        SpriteFont secondfont;      // Font for drawing text on the screen

        // Variables for Kinect
        KinectSensor kinectSensor;
        ColorImageFrame videoframe; // Variable to hold latest video feed frame
        Texture2D kinectRGBVideo;   // Holds current video frame as a Texture2D for displaying
        SkeletonFrame body;         // Variable to hold body positional data from Kinect
        int cameraangle = 0;        // Current Kinect camera angle
        float cameracounter = 1000; // Delay between moving the camera angle to avoid breaking it :)

        const int framespersec =22; // Video frame rate
        float milliperframe = 1000f / (float)framespersec;  // Milliseconds per frame
        float framecount = 0;       // Counter between video frames


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true; // Show windows mouse pointer

            // Get the screen resolution from Windows
            screenresx = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenresy = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            // Set the current game window size
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

            
            mainfont = Content.Load<SpriteFont>("miramonte3");  // Load the font
            secondfont = Content.Load<SpriteFont>("miramonte9");  // Load the font

            // Load in settings
            if (File.Exists(@"settings.txt")) // This checks to see if the file exists
            {
                StreamReader sr = new StreamReader(@"settings.txt");	// Open the file
                String line;		// Create a string variable to read each line into
                line = sr.ReadLine();	// Read the first line in the text file
                mousescale = Convert.ToSingle(line);	// This converts line to numeric
                line = sr.ReadLine();	// Read the first line in the text file
                cameraangle = Convert.ToInt32(line);	// This converts line to numeric
                sr.Close();			// Close the file
            }


            // Set up Kinect to stream video
            kinectRGBVideo = new Texture2D(GraphicsDevice, 640, 480);

            setupkinect();
        }

        void setupkinect()
        {
            // Initialise Kinect
            try
            {
                // Enable the Kinect Sensor for Video and Skeletal data
                kinectSensor = KinectSensor.KinectSensors[0];
                kinectSensor.SkeletonStream.Enable();
                kinectSensor.ColorStream.Enable(ColorImageFormat.YuvResolution640x480Fps15);
                kinectSensor.Start();
                kinectSensor.ElevationAngle = cameraangle;                // Set the kinect camera angle
                //  kinectSensor.DepthStream.Range = DepthRange.Near;

                errornokinect = false;
            }
            catch
            {
                errornokinect = true;
            }
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            if (!errornokinect)
                kinectSensor.Stop();    // Stop the kinect sensor

            // Save settings
            StreamWriter sw = new StreamWriter(@"settings.txt");
            sw.WriteLine(mousescale.ToString("0.0"));
            sw.WriteLine(cameraangle.ToString("0"));
            sw.Close();
        }




        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            float gtime = (float)gameTime.ElapsedGameTime.TotalMilliseconds; // Time between updates
            KeyboardState keys = Keyboard.GetState();                     // Read keyboard
            
            // Keyboard controls
            if (keys.IsKeyDown(Keys.Escape)) this.Exit();

            // Allow user to alter the mouse movement scale from kinect
            if (keys.IsKeyDown(Keys.Left)) mousescale -= gtime/240; 
            if (keys.IsKeyDown(Keys.Right)) mousescale += gtime/240;
            if (mousescale < 0.01f) mousescale = 0.01f;
            if (mousescale > 2f) mousescale = 2f;

            if (keys.IsKeyDown(Keys.Space)) videofeed=false;    // Turn off video feed
            if (keys.IsKeyDown(Keys.S)) videofeed = true;       // Start video feed

            // Toggle Kinect mouse simulation on or off using the K key on keyboard
            if (keys.IsKeyDown(Keys.K))
            {
                if (!kclicked)
                {
                    kinectactive = !kinectactive;
                    kclicked = true;
                }
            }
            else
                kclicked = false;

            if (!errornokinect)
            {

                // Allow user to alter the Kinect Camera Angle
                if (cameracounter < 0)
                {
                    cameracounter = 1500;
                    cameraangle = kfunctions.cameramove(kinectSensor, cameraangle);
                }

                // Count timers down
                cameracounter -= gtime;
                delaybetweenclicks -= gtime;

                // If the Kinect device is set to active
                if (kinectactive)
                {
                    Vector3 p1handpos, p2handpos; // Variables to read hand positions for 2 people (we are only really using p1handpos in this application)

                    // show video feed 
                    framecount += gtime;
                    if (framecount > milliperframe && videofeed)
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

                            // Read Kinect and look for right hand movement
                            kfunctions.readjoints(bodyskel, JointType.HandRight, screenresx, screenresy, mousescale, out p1handpos, out p2handpos);
                            if (p1handpos.X != 2000)
                            {
                                // Set the Mouse position based on the position of the users right hand
                                mousepos.X = p1handpos.X - this.Window.ClientBounds.X;
                                mousepos.Y = p1handpos.Y - this.Window.ClientBounds.Y;

                                // Keep the mouse within the screen limits
                                if (mousepos.X < -this.Window.ClientBounds.X) mousepos.X = -this.Window.ClientBounds.X;
                                if (mousepos.X > screenresx - this.Window.ClientBounds.X) mousepos.X = screenresx - this.Window.ClientBounds.X;
                                if (mousepos.Y < -this.Window.ClientBounds.Y) mousepos.Y = -this.Window.ClientBounds.Y;
                                if (mousepos.Y > screenresy - this.Window.ClientBounds.Y) mousepos.Y = screenresy - this.Window.ClientBounds.Y;

                                // Set the mouse position based on Kinect data
                                Mouse.SetPosition((int)mousepos.X, (int)mousepos.Y);

                                // Check for left hand being raised for Left-Click simulation
                                Boolean ishandup1 = false;
                                Boolean ishandup2 = false;
                                gestures.handup(bodyskel, false, out ishandup1, out ishandup2);

                                // If left hand is up and delay between clicks has reached 0
                                if (ishandup1 && delaybetweenclicks < 0)
                                {
                                    delaybetweenclicks = 1500;  // Set the delay to 1500 milliseconds before another left click can be performed
                                    // Simulate a left click
                                    mouse_event((uint)(MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP), (uint)mousepos.X, (uint)mousepos.Y, 0, 0);
                                }
                            }
                        }
                    }
                }
            }
            else
                setupkinect();

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
           
            // TODO: Add your drawing code here
            spriteBatch.Begin();

            if (errornokinect)
            {
                spriteBatch.DrawString(secondfont, "Error please connect Kinect to this PC", new Vector2(20, 50),
                    Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(secondfont, "Also ensure Kinect SDK v1.5 is installed", new Vector2(20, 100),
                    Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }
            else
            {

                // Draw video feed
                spriteBatch.Draw(kinectRGBVideo, new Rectangle(0, 0, displaywidth, displayheight), Color.White);  // Draw video from kinect camera

                // Indicate that left mouse click is being simulated
                if (delaybetweenclicks > 1000)
                    spriteBatch.DrawString(mainfont, "LEFT-CLICK", new Vector2(40, 10),
                        Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 1.2f, SpriteEffects.None, 0);

                // Display value of mouse scale
                spriteBatch.DrawString(mainfont, "Mouse Scale " + mousescale.ToString("0.0"), new Vector2(40, displayheight / 2 - 12),
                    Color.WhiteSmoke, MathHelper.ToRadians(0), new Vector2(0, 0), 0.8f, SpriteEffects.None, 0);
                //            spriteBatch.DrawString(mainfont, "Cursor X" + mousepos.X.ToString("0") + " Y" + mousepos.Y.ToString("0"), new Vector2(20, displayheight / 2),
                //              Color.WhiteSmoke, MathHelper.ToRadians(0), new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);

                // Take credit for Magic Cursor
                spriteBatch.DrawString(secondfont, "Magic Cursor created by David Renton", new Vector2(4, 180),
                Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(secondfont, "http://drenton72.wordpress.com", new Vector2(4, 195),
                Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(secondfont, "Email: drenton@reidkerr.ac.uk", new Vector2(4, 210),
                Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(secondfont, "Twitter: @drenton72", new Vector2(4, 225),
                Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        //[DllImport("user32.dll")]
        //static extern bool SetCursorPos(int X, int Y);  

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bvk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        /// <summary>
        /// Generates a simulated keyboard event
        /// <remarks>
        /// For more details, see http://msdn2.microsoft.com/en-us/library/ms645540.aspx and 
        /// http://pinvoke.net/default.aspx/user32/keybd_event.html
        /// </remarks>
        /// </summary>
        /// <param name="keyCode"></param>
        void PressKey(byte keyCode)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            keybd_event(keyCode, (byte)0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            keybd_event(keyCode, (byte)0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
        }

        /// <summary>
        /// http://pinvoke.net/default.aspx/user32/mouse_event.html
        /// </summary>
        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        [Flags]
        public enum KeyboardEventFlags
        {
            VK_RETURN = 0x0000000D,
            VK_ESCAPE = 0x0000001B,
            VK_LEFT = 0x00000025,
            VK_UP = 0x00000026,
            VK_RIGHT = 0x00000027,
            VK_DOWN = 0x00000028,
            VK_VOLUME_UP = 0x000000AF,
            VK_VOLUME_DOWN = 0x000000AE,
            VK_MEDIA_NEXT_TRACK = 0x000000B0,
            VK_MEDIA_PREVIOUS_TRACK = 0x000000B1,
            VK_MEDIA_STOP = 0x000000B2,
            VK_MEDIA_PLAY_PAUSE = 0x000000B3
        }

        //Set cursor position  
        //void SetCursorPos(10, 50);
        //Mouse Right Down and Mouse Right Up
        //mouse_event((uint)MouseEventFlags.RIGHTDOWN,0,0,0,0); 
        //mouse_event((uint)MouseEventFlags.RIGHTUP,0,0,0,0);   
        
    }
}
