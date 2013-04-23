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
using gamelib1;
using System.IO;
using kinect;

namespace gamemenu
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Screen height and width
        int displaywidth=800;
        int displayheight=600;
        SpriteFont mainfont, fontwhite;        // Main font for drawing in-game text
        SpriteFont digfont;         // Font for numbers

        float gameruntime = 0;      // Time since game started

        graphic2d background;       // Background image
        graphic2d gamebackground1, gamebackground2, gamebackground3;       // Background image
        sprite2d gamelogo;

        int gamestate = -1;         // Current game state
        int ingamestate = 0;        // Current state when game is being played

        GamePadState[] pad = new GamePadState[4];       // Array to hold gamepad states
        KeyboardState keys;                             // Variable to hold keyboard state
        MouseState mouse;                               // Variable to hold mouse state
        Boolean mousereleased = true;                   // Check for mouse button released

        sprite2d mousepointer1, mousepointer2;          // Sprite to hold a mouse pointer
        const int numberofoptions=5;                    // Number of main menu options
        sprite2d[,] menuoptions = new sprite2d[numberofoptions,2]; // Array of sprites to hold the menu options
        int optionselected = 0;                         // Current menu option selected
        int levelselected = 0;

        const int numberofhighscores = 10;                              // Number of high scores to store
        int[] highscores = new int[numberofhighscores];                 // Array of high scores
        string[] highscorenames = new string[numberofhighscores];       // Array of high score names

        // Declare variables to hold menu option buttons
        const int numberoflevels = 3;
        sprite2d[,] leveloptions = new sprite2d[numberoflevels, 2];
        const int numberoftypes = 5;
        sprite2d[,] typeoptions = new sprite2d[numberoftypes, 2];
        sprite2d[,] lefthanded = new sprite2d[2,4];
        sprite2d questionsto;
        sprite2d[] questionspinner = new sprite2d[3];
        sprite2d[,] savepicbutton = new sprite2d[2,2];
        sprite2d[,] fullscreenbutton = new sprite2d[2, 2];
        Boolean fullscreenstate = false;

        Boolean onback = false;
        sprite2d[] backbutton = new sprite2d[2];

        sprite2d[] hand = new sprite2d[2];  // Declare two hand graphics

        KeyboardState lastkeystate;
        Boolean keyboardreleased = true;
        int keyfocus = 0;

        // Class wide variables
        float angletoguess = 0;
        int numer,denom;
        int handconfirmtime = 2000;
        float countdown = 0;
        int round = 0;
        int questiontype = 0;
        char specialcharacter;
        int numberofplayers = 1;

        int delaybetween = 8000;
        int roundlength = 30000;
        int questions2ask = 6;
        int bonuspoints = 10;
        int gamemode = 0;
        int skilllevel = 2;
        Boolean saveallpics = false;

        int round2nearest = 1;

        // Class for players
        public class players
        {
            public Boolean handisup = false;
            public int score = 0;
            public float angle = 0;
            public float countdown;
            public Boolean roundover;           // Is the round over?
            public Boolean lefthanded = false;  // Is the player left handed?
            public string playername;           // Player's Names

            public players()
            {
                countdown = 0;
                roundover = false;
                playername = "";
                lefthanded = false;
            }
        }

        players[] gamer = new players[2];


        // Convert an angle (in degrees) to a decimal
        public static float angle2decimal(int angle)
        {
            return (float)(Math.Round((float)angle / 360f, 2));
        }

        // Convert an angle (in degrees) to a decimal
        public static int decimal2angle(float dec)
        {
            return (int)(Math.Round(dec * 360f, 0));
        }

        // Convert an angle (in degrees) to a percentage
        public static int angle2percentage(int angle)
        {
            return (int)(Math.Round((float)angle / 3.6f, 0));
        }

        // Convert a percentage to an angle (in degrees)
        public static int percentage2angle(int percent)
        {
            return (int)(Math.Round((float)percent * 3.6f, 0));
        }

        // Convert a fraction to a percentage
        public static int fraction2percentage(int numerator, int denominator)
        {
            return (int)(Math.Round((float)numerator/(float)denominator * 100f, 0));
        }

        // Converts a decimal to a percentage
        public static int decimal2percentage(float decnumber)
        {
            return (int)(Math.Round(decnumber * 100, 0));
        }

        // Generate random fraction
        public void generatefraction(out int numerator, out int denominator, int skill)
        {
            Random randomiser = new Random();       // Variable to generate random numbers
            round2nearest = 1;

            if (skill == 1 || skill == 2)
                denominator = randomiser.Next(12) + 1;
            else
                denominator = randomiser.Next(4) + 1;
            do
            {
                numerator = randomiser.Next(12) + 1;
            } while (numerator > denominator);

            if (randomiser.Next(2) == 1 && skill == 2)
            {
                int factor = randomiser.Next(4) + 1;
                numerator *= factor;
                denominator *= factor;
            }
        }

        // Generate random percentage
        public int generatepercentage(int skill)
        {
            Random randomiser = new Random();       // Variable to generate random numbers
            if (skill == 0)
            {
                round2nearest = 45;
                return ((randomiser.Next(4) + 1) * 25);
            }
            else if (skill == 1)
            {
                round2nearest = 9;
                return ((randomiser.Next(20) + 1) * 5);
            }
            else
            {
                round2nearest = 1;
                return (randomiser.Next(100) + 1);
            }
        }

        // Generate random angle in degrees
        public int generateangle(int skill)
        {
            Random randomiser = new Random();       // Variable to generate random numbers
            if (skill == 0)
            {
                round2nearest = 45;
                return (randomiser.Next(4) + 1) * 90;
            }
            else if (skill == 1)
            {
                round2nearest = 5;
                return (randomiser.Next(36) + 1) * 10;
            }
            else
            {
                round2nearest = 1;
                return (randomiser.Next(360) + 1);
            }
        }

        // Generate random decimal number
        public float generatedecimal(int skill)
        {
            return (((float)generatepercentage(skill))/100f);
        }


        // Class for 2D sprites
        public class angleart
        {
            private Texture2D image;         		// Texture which holds image
            public Vector3 position; 		 	    // Position on screen
            public Rectangle rect;          		// Rectangle to hold size and position
            private Vector2 origin;          		// Centre point
            public float rotation = 0;          	// Amount of rotation to apply
            private Boolean visible = true;    		// Should object be drawn true or false
            private Color colour = Color.White;      // Holds colour to draw the image in
            private float size;                      // Size ratio of object

            public angleart() { }                   // Empty constructor to avoid crashes

            // Constructor which initialises the sprite2D
            public angleart(ContentManager content, string spritename, int x, int y, float msize, Color mcolour, Boolean mvis)
            {
                image = content.Load<Texture2D>(spritename);    // Load image into texture
                position = new Vector3((float)x, (float)y, 0);  // Set position
                rect.X = x;                                     // Set position of draw rectangle x
                rect.Y = y;                                     // Set position of draw rectangle y
                origin.X = 0;               	    // Set X origin to half of width
                origin.Y = 0;              	        // Set Y origin to half of height
                rect.Width = (int)(image.Width * msize);  	    // Set the new width based on the size ratio 
                rect.Height = (int)(image.Height * msize);	    // Set the new height based on the size ratio
                colour = mcolour;                               // Set colour
                visible = mvis;                                 // Image visible TRUE of FALSE? 
                size = msize;                                   // Store size ratio
            }

            // Use this method to draw the image
            public void drawme(ref SpriteBatch sbatch)
            {
                if (visible)
                    sbatch.Draw(image, rect, null, colour, rotation, origin, SpriteEffects.None, 0);
            }

            public void drawangle(ref SpriteBatch sbatch, int angle)
            {
                for (int i = 0; i <= angle; i++)
                {
                    rotation = MathHelper.ToRadians(i + 180);
                    drawme(ref sbatch);
                }
            }

            public void drawangle(ref SpriteBatch sbatch, int angle, Color col)
            {
                float ang = 0;
                while (ang <= angle)
                {
                    rotation = MathHelper.ToRadians(ang + 180);
                    if (visible)
                        sbatch.Draw(image, rect, null, col, rotation, origin, SpriteEffects.None, 0);
                    ang += 0.1f;
                }
            }
        }

        // Variables for Kinect
        KinectSensor kinectSensor = KinectSensor.KinectSensors[0];
        Texture2D kinectRGBVideo;
        ColorImageFrame videoframe;
        int cameraangle = 0;
        float cameracounter = 1000;
        Texture2D[] funnypics;
        float piccounter = 0;
        SkeletonFrame body;
        angleart aline1, aline2;
      //  Vector3 p1position, p2position;

        const int numberofsounds = 6;
        SoundEffect[] applause = new SoundEffect[numberofsounds];

        // Create a new render target for rendering images for saving
        RenderTarget2D renderTarget;

//        Texture2D vframe;
//        Color[] vcolour;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Set the screen resolution
            this.graphics.PreferredBackBufferWidth = displaywidth;
            this.graphics.PreferredBackBufferHeight = displayheight;

            // Set the game window to match the screen resolution
         //   this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        //    this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
         //   this.graphics.PreferredBackBufferWidth = displaywidth;
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
          //  displayheight = this.GraphicsDevice.Viewport.Height-120;
            //displaywidth = graphics.GraphicsDevice.Viewport.Width;
           // displaywidth = (int)Math.Round((float)displayheight * (4f / 3f), 0);
           // graphics.PreferredBackBufferWidth = displaywidth;
           // graphics.PreferredBackBufferHeight = displayheight;
           // graphics.ApplyChanges();

            //graphics.ToggleFullScreen(); // Put game into full screen mode

            // Initialise Kinect
//            kinectSensor.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseDepth | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);


           // kinectSensor.DepthStream.Enable();
            kinectSensor.SkeletonStream.Enable();
            kinectSensor.ColorStream.Enable(ColorImageFormat.YuvResolution640x480Fps15);
            kinectSensor.Start();

            renderTarget = new RenderTarget2D(GraphicsDevice, displaywidth, displayheight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);


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

//            applause[0] = Content.Load<SoundEffect>("applause-2"); // Loads the sound effect
            applause[0] = Content.Load<SoundEffect>("woof"); // Loads the sound effect
            applause[1] = Content.Load<SoundEffect>("yipee"); // Loads the sound effect
            applause[5] = Content.Load<SoundEffect>("fandab"); // Loads the sound effect
            applause[2] = Content.Load<SoundEffect>("woop"); // Loads the sound effect
            applause[3] = Content.Load<SoundEffect>("yeah"); // Loads the sound effect
            applause[4] = Content.Load<SoundEffect>("yeah2"); // Loads the sound effect

            // TODO: use this.Content to load your game content here
            mainfont = Content.Load<SpriteFont>("andynew"); // Load font
            digfont = Content.Load<SpriteFont>("quartz4"); // Load font
            fontwhite = Content.Load<SpriteFont>("miramonte9"); // Load font

            background = new graphic2d(Content, "kinectbackground", displaywidth, displayheight);
            gamebackground1 = new graphic2d(Content, "game screen 3", displaywidth, displayheight);
            gamebackground1.stretch2fit(displaywidth, displayheight);
            gamebackground2 = new graphic2d(Content, "game screen 4", displaywidth, displayheight);
            gamebackground2.stretch2fit(displaywidth, displayheight);
            gamebackground3 = new graphic2d(Content, "game screen 5", displaywidth, displayheight);
            gamebackground3.stretch2fit(displaywidth, displayheight);


            gamelogo = new sprite2d(Content, "kinect angles", displaywidth / 2, displayheight - 100, 0.4f, Color.White, true);
            gamelogo.rect.Y = displayheight-gamelogo.rect.Height/2;

            mousepointer1 = new sprite2d(Content, "X-Games-Cursor", 0, 0, 0.15f, Color.White, true);
            mousepointer2 = new sprite2d(Content, "X-Games-Cursor-Highlight", 0, 0, 0.15f, Color.White, true);

            hand[0] = new sprite2d(Content, "conniehand", displaywidth/4, 84, 0.5f, Color.White, true);
            hand[1] = new sprite2d(Content, "conniehand", (displaywidth / 4)*3, 84, 0.5f, Color.White, true);

            menuoptions[0, 0] = new sprite2d(Content, "player1", displaywidth / 2, 180, 1, Color.White, true);
            menuoptions[0, 1] = new sprite2d(Content, "player1over", displaywidth / 2, 180, 1, Color.White, true);
            menuoptions[1, 0] = new sprite2d(Content, "player2", displaywidth / 2, 260, 1, Color.White, true);
            menuoptions[1, 1] = new sprite2d(Content, "player2over", displaywidth / 2, 260, 1, Color.White, true);
            menuoptions[2, 0] = new sprite2d(Content, "options", displaywidth / 2, 340, 1, Color.White, true);
            menuoptions[2, 1] = new sprite2d(Content, "optionsover", displaywidth / 2, 340, 1, Color.White, true);
            menuoptions[3, 0] = new sprite2d(Content, "highscores", displaywidth / 2, 420, 1, Color.White, true);
            menuoptions[3, 1] = new sprite2d(Content, "highscoreover", displaywidth / 2, 420, 1, Color.White, true);
            menuoptions[4, 0] = new sprite2d(Content, "exit", displaywidth / 2, 500, 1, Color.White, true);
            menuoptions[4, 1] = new sprite2d(Content, "exitover", displaywidth / 2, 500, 1, Color.White, true);
            for (int i = 0; i < numberofoptions; i++)
                menuoptions[i, 0].updateobject();

            backbutton[0] = new sprite2d(Content, "xboxControllerBack", 60, 60, 1, Color.White, true);
            backbutton[1] = new sprite2d(Content, "backglow", 60, 60, 1, Color.White, true);

            typeoptions[0, 0] = new sprite2d(Content, "angles1", 100, 140, 1, Color.White, true);
            typeoptions[0, 1] = new sprite2d(Content, "angles2", 100, 140, 1, Color.White, true);
            typeoptions[1, 0] = new sprite2d(Content, "percentages1", 250, 140, 1, Color.White, true);
            typeoptions[1, 1] = new sprite2d(Content, "percentages2", 250, 140, 1, Color.Yellow, true);
            typeoptions[2, 0] = new sprite2d(Content, "decimal1", 400, 140, 1, Color.White, true);
            typeoptions[2, 1] = new sprite2d(Content, "decimal2", 400, 140, 1, Color.White, true);
            typeoptions[3, 0] = new sprite2d(Content, "fractions1", 550, 140, 1, Color.White, true);
            typeoptions[3, 1] = new sprite2d(Content, "fractions2", 550, 140, 1, Color.Yellow, true);
            typeoptions[4, 0] = new sprite2d(Content, "all1", 700, 140, 1, Color.White, true);
            typeoptions[4, 1] = new sprite2d(Content, "all2", 700, 140, 1, Color.Yellow, true);
            for (int i = 0; i < numberoftypes; i++)
                typeoptions[i, 0].updateobject();

            leveloptions[0, 0] = new sprite2d(Content, "easy", 200, 220, 1, Color.White, true);
            leveloptions[0, 1] = new sprite2d(Content, "easy2", 200, 220, 1, Color.White, true);
            leveloptions[1, 0] = new sprite2d(Content, "medium", 400, 220, 1, Color.White, true);
            leveloptions[1, 1] = new sprite2d(Content, "medium2", 400, 220, 1, Color.Yellow, true);
            leveloptions[2, 0] = new sprite2d(Content, "hard", 600, 220, 1, Color.White, true);
            leveloptions[2, 1] = new sprite2d(Content, "hard2", 600, 220, 1, Color.White, true);
            for (int i = 0; i < numberoflevels; i++)
                leveloptions[i, 0].updateobject();

            lefthanded[0, 0] = new sprite2d(Content, "p1left", 300, 300, 1, Color.White, true);
            lefthanded[0, 1] = new sprite2d(Content, "p1lefthigh", 300, 300, 1, Color.White, true);
            lefthanded[0, 2] = new sprite2d(Content, "p1right", 300, 300, 1, Color.White, true);
            lefthanded[0, 3] = new sprite2d(Content, "p1rightglow", 300, 300, 1, Color.White, true);
            lefthanded[0, 0].updateobject();
            lefthanded[1, 0] = new sprite2d(Content, "p2left", 500, 300, 1, Color.White, true);
            lefthanded[1, 1] = new sprite2d(Content, "p2lefthigh", 500, 300, 1, Color.White, true);
            lefthanded[1, 2] = new sprite2d(Content, "p2right", 500, 300, 1, Color.White, true);
            lefthanded[1, 3] = new sprite2d(Content, "p2rightglow", 500, 300, 1, Color.White, true);
            lefthanded[1, 0].updateobject();

            questionsto = new sprite2d(Content, "questions2ask", 340, 400, 1, Color.White, true);
            questionspinner[0] = new sprite2d(Content, "spinner", 500, 400, 1, Color.White, true);
            questionspinner[1] = new sprite2d(Content, "spinnerup", 500, 400, 1, Color.White, true);
            questionspinner[2] = new sprite2d(Content, "spinnerdown", 500, 400, 1, Color.White, true);
            questionspinner[1].updateobject();
            questionspinner[2].updateobject();
            questionspinner[1].bbox.Max.Y -= questionspinner[1].rect.Height / 2;
            questionspinner[2].bbox.Min.Y += questionspinner[2].rect.Height / 2;

            savepicbutton[0, 0] = new sprite2d(Content, "savepicson", 400, 500, 1, Color.White, true);
            savepicbutton[0, 1] = new sprite2d(Content, "savepicsonglow", 400, 500, 1, Color.White, true);
            savepicbutton[1, 0] = new sprite2d(Content, "savepicturesoff", 400, 500, 1, Color.White, true);
            savepicbutton[1, 1] = new sprite2d(Content, "savepicturesoffglow", 400, 500, 1, Color.White, true);
            savepicbutton[0, 0].updateobject();

            fullscreenbutton[0, 0] = new sprite2d(Content, "full screen off", 700, 60, 1, Color.White, true);
            fullscreenbutton[0, 1] = new sprite2d(Content, "fullscreenoffglow", displaywidth - (fullscreenbutton[0, 0].rect.Width / 2), fullscreenbutton[0, 0].rect.Height / 2, 1, Color.White, true);
            fullscreenbutton[1, 0] = new sprite2d(Content, "full screen on", fullscreenbutton[0, 1].rect.X, fullscreenbutton[0, 1].rect.Y, 1, Color.White, true);
            fullscreenbutton[1, 1] = new sprite2d(Content, "fullscreenonglow", fullscreenbutton[0, 1].rect.X, fullscreenbutton[0, 1].rect.Y, 1, Color.White, true);
            fullscreenbutton[0, 0].position = fullscreenbutton[0, 1].position;
            fullscreenbutton[0, 0].updateobject();
            
            funnypics = new Texture2D[questions2ask];

            aline1 = new angleart(Content, "line3", displaywidth / 4, displayheight / 2, 1f, Color.White, true);
            aline2 = new angleart(Content, "line4", (displaywidth / 4) * 3, displayheight / 2, 1f, Color.White, true);

            gamer[0] = new players();
            gamer[1] = new players();

            if (File.Exists(@"options.txt"))
            {
                StreamReader sr = new StreamReader(@"options.txt");	// Open the file

                String line;		// Create a string variable to read each line into
                if (!sr.EndOfStream)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    questions2ask = (int)Convert.ToDecimal(line);	// This converts line to numeric
                }
                if (!sr.EndOfStream)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    delaybetween = (int)Convert.ToDecimal(line);	// This converts line to numeric
                } 
                if (!sr.EndOfStream)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    roundlength = (int)Convert.ToDecimal(line);	// This converts line to numeric
                } 
                if (!sr.EndOfStream)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    bonuspoints = (int)Convert.ToDecimal(line);	// This converts line to numeric
                }
                if (!sr.EndOfStream)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    gamemode = (int)Convert.ToDecimal(line);	// This converts line to numeric
                }
                if (!sr.EndOfStream)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    skilllevel = (int)Convert.ToDecimal(line);	// This converts line to numeric
                }
                if (!sr.EndOfStream)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    saveallpics = (Convert.ToDecimal(line)==1);	// This converts line to numeric then boolean
                }
                if (!sr.EndOfStream)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    cameraangle = (int)Convert.ToDecimal(line);	// This converts line to numeric
                }
                
                sr.Close();			// Close the file
            }

            loadhighscores("highscore" + gamemode.ToString("0") + skilllevel.ToString("0") + questions2ask.ToString("00") + ".txt");

            if (!Directory.Exists(Content.RootDirectory + "\\..\\pictures"))
                Directory.CreateDirectory(Content.RootDirectory + "\\..\\pictures");

            // Set up Kinect to stream video
            kinectSensor.ElevationAngle = cameraangle;

            kinectRGBVideo = new Texture2D(GraphicsDevice, 640, 480);
            //vframe = new Texture2D(GraphicsDevice, kinectRGBVideo.Width, kinectRGBVideo.Height);
            //vcolour = new Color[kinectRGBVideo.Height * kinectRGBVideo.Width];
          
        }

        void loadhighscores(string filename)
        {
            // Load in high scores
            if (File.Exists(@filename)) // This checks to see if the file exists
            {
                StreamReader sr;
                sr = new StreamReader(@filename);	// Open the file

                String line;		// Create a string variable to read each line into
                for (int i = 0; i < numberofhighscores && !sr.EndOfStream; i++)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    highscorenames[i] = line.Trim(); // Read high score name

                    if (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();	// Read the first line in the text file
                        line = line.Trim(); 	// This trims spaces from either side of the text
                        highscores[i] = (int)Convert.ToDecimal(line);	// This converts line to numeric
                    }
                }
                sr.Close();			// Close the file
            } 
            else
                for (int i = 0; i < highscorenames.Count(); i++)
                {
                    highscorenames[i] = " ";
                    highscores[i] = 0;
                }
        }


        void savehighscores(string filename)
        {
            // Save high scores
            StreamWriter sw = new StreamWriter(@filename);
            for (int i = 0; i < numberofhighscores; i++)
            {
                sw.WriteLine(highscorenames[i]);
                sw.WriteLine(highscores[i].ToString());
            }
            sw.Close();
        }



        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            kinectSensor.Stop();

            StreamWriter sw = new StreamWriter(@"options.txt");
            sw.WriteLine(questions2ask.ToString("00"));
            sw.WriteLine(delaybetween.ToString("00000"));
            sw.WriteLine(roundlength.ToString("000000"));
            sw.WriteLine(bonuspoints.ToString("000"));
            sw.WriteLine(gamemode.ToString("0"));
            sw.WriteLine(skilllevel.ToString("0"));
            sw.WriteLine(saveallpics ? "1" : "0");
            sw.WriteLine(cameraangle.ToString("000"));
            sw.Close();

            savehighscores("highscore" + gamemode.ToString("0") + skilllevel.ToString("0") + questions2ask.ToString("00") + ".txt");
        }

        // Reset values at the start of a new game
        void reset()
        {
            ingamestate = 1;
            for (int i = 0; i < gamer.Count(); i++)
            {
                gamer[i].score = 0;
                gamer[i].angle = 0;
                gamer[i].playername = "";
            } 
            
            angletoguess = 0;
            round = 0;
            countdown = delaybetween*2;

            // Stream video from Kinect into imageframes
            videoframe = kinectSensor.ColorStream.OpenNextFrame(0);
            if (videoframe != null)
                kinectRGBVideo = kfunctions.video2texture(graphics, videoframe);

            funnypics = new Texture2D[questions2ask];
            for (int i = 0; i < funnypics.Count(); i++)
                funnypics[i] = kinectRGBVideo;
        }


        void savepictures()
        {
            // If it is not the first round store the video feed into a picture and save it
            if (round > 0)
            {
                GraphicsDevice.SetRenderTarget(renderTarget);                // Set the render target
                drawgame();
                GraphicsDevice.SetRenderTarget(null);                // Drop the render target

                DateTime dateandtime = DateTime.Now;
                string filename = Content.RootDirectory + "\\..\\pictures\\" + dateandtime.ToFileTime() + ".jpg";
                FileStream stream = File.OpenWrite(filename);
                renderTarget.SaveAsJpeg(stream, 640, 480);
                stream.Close();

                dateandtime = DateTime.Now;
                filename = Content.RootDirectory + "\\..\\pictures\\" + dateandtime.ToFileTime() + ".jpg";
                stream = File.OpenWrite(filename);
                kinectRGBVideo.SaveAsJpeg(stream, 640, 480);
                stream.Close();
            }
        }
        
        // Reset values at the start of each round
        void resetround(float gtime)
        {
            Random randomiser = new Random();       // Variable to generate random numbers

            countdown = roundlength;
            for (int i = 0; i < gamer.Count(); i++)
            {
                gamer[i].roundover = false;
                gamer[i].countdown = handconfirmtime;
            }

            if (gamemode == 4)
                questiontype = randomiser.Next(4);
            else
                questiontype = gamemode;

            if (questiontype == 0)
            {
                angletoguess = generateangle(skilllevel);
                specialcharacter = '~';
            }
            else if (questiontype == 1)
            {
                angletoguess = generatepercentage(skilllevel);
                specialcharacter = '%';
            }
            else if (questiontype == 2)
            {
                angletoguess = generatedecimal(skilllevel);
                specialcharacter = ' ';
            }
            else if (questiontype == 3)
            {
                generatefraction(out numer, out denom, skilllevel);
                angletoguess = (int)(Math.Round(((float)numer/(float)denom)*100f,0));
                specialcharacter = '%';
            }

            // Advance round counter until end of game
            round++;

            // Check if game is over
            if (round > questions2ask)
            {
                ingamestate = 4;
                countdown = 0;

                // Set to allow the user to enter a highscore name if need be
                keyfocus = 0;
                if (gamer[0].score > highscores[9])
                    keyfocus = 1;
                else if (gamer[1].score > highscores[9])
                    keyfocus = 2;
                
                // Play cheer for getting a high score
                if (gamer[0].score > highscores[9] || gamer[1].score > highscores[9])
                    applause[randomiser.Next(numberofsounds)].Play();
            }
        }



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            pad[0] = GamePad.GetState(PlayerIndex.One);     // Reads gamepad 1
            pad[1] = GamePad.GetState(PlayerIndex.Two);     // Reads gamepad 2
            pad[2] = GamePad.GetState(PlayerIndex.Three);   // Reads gamepad 1
            pad[3] = GamePad.GetState(PlayerIndex.Four);    // Reads gamepad 2
            mouse = Mouse.GetState();                       // Read Mouse
            keys = Keyboard.GetState();                     // Read keyboard
            keyboardreleased = (keys != lastkeystate);      // Has keyboard input changed

            float timebetweenupdates = (float)gameTime.ElapsedGameTime.TotalMilliseconds; // Time between updates
            gameruntime += timebetweenupdates;  // Count how long the game has been running for

            // Read the mouse and set the mouse cursor
            mousepointer1.position.X = mouse.X;
            mousepointer1.position.Y = mouse.Y;
            mousepointer1.updateobject();
            // Set a small bounding sphere at the center of the mouse cursor
            mousepointer1.bsphere = new BoundingSphere(mousepointer1.position, 2);  

            // TODO: Add your update logic here
            switch (gamestate)
            {
                case -1:
                    // Game is on the main menu
                    updatemenu();
                    break;
                case 0:
                    // Game is being played
                    updategame(timebetweenupdates);
                    break;
                case 1:
                    // Game is being played
                    updategame(timebetweenupdates);
                    break;
                case 2:
                    // Options menu
                    updateoptions();
                    break;
                case 3:
                    // High Score table
                    updatehighscore();
                    break;
                default:
                    // Do something if none of the above are selected
                    this.Exit();    // Quit Game
                    break;
            }

            lastkeystate = keys;                     // Read keyboard

            base.Update(gameTime);
        }

        public void updatemenu()
        {
            optionselected = -1;

            // Check for mousepointer being over a menu option
            for (int i = 0; i < numberofoptions; i++)
            {
                // Check for mouse over a menu option
                if (mousepointer1.bsphere.Intersects(menuoptions[i, 0].bbox))
                {
                    optionselected = i;
                    if (mouse.LeftButton == ButtonState.Pressed)
                        gamestate = optionselected;
                }
            }

            // Start 1 player game
            if (gamestate == 0)
            {
                numberofplayers = 1;
                aline1.rect.X = displaywidth / 2;
                gamebackground1.image = Content.Load<Texture2D>("game screen 3b");
                gamebackground2.image = Content.Load<Texture2D>("game screen 4b");
                gamebackground3.image = Content.Load<Texture2D>("game screen 5b");

                reset();
            }
            // Start 2 player game
            if (gamestate == 1)
            {
                numberofplayers = 2;
                aline1.rect.X = displaywidth / 4;
                gamebackground1.image = Content.Load<Texture2D>("game screen 3");
                gamebackground2.image = Content.Load<Texture2D>("game screen 4");
                gamebackground3.image = Content.Load<Texture2D>("game screen 5");
                reset();
            }
            // Load options screen
            if (gamestate == 2)
            {
                mousereleased = false;
                savehighscores("highscore" + gamemode.ToString("0") + skilllevel.ToString("0") + questions2ask.ToString("00") + ".txt");
            }
        }

        public void drawmenu()
        {
            spriteBatch.Begin();
            background.drawme(ref spriteBatch);
            // Draw menu options
            for (int i = 0; i < numberofoptions; i++)
            {
                if (optionselected==i)
                    menuoptions[i, 1].drawme(ref spriteBatch);
                else
                    menuoptions[i, 0].drawme(ref spriteBatch);
            }

            // Draw mouse
            if (optionselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);


            spriteBatch.DrawString(mainfont, "Programming and Design: David Renton ", new Vector2(240, displayheight - 50), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(mainfont, "Twitter @drenton72  Email drenton@reidkerr.ac.uk", new Vector2(200, displayheight - 33), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(mainfont, "Facebook: www.facebook.com/TheKTeamPIL", new Vector2(230, displayheight - 16), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);

            spriteBatch.End();
        }

        public void updategame(float gtime)
        {
            // Variable to generate random numbers
            Random randomiser = new Random();

            countdown -= gtime;

            // Main game code
            // Game is being played
            switch (ingamestate)
            {
                case 1:
                    // Round is about to start

                    // Allow user to tilt camera using arrow keys
                    if (cameracounter < 0)
                    {
                        cameracounter = 1500;
                        cameraangle = kfunctions.cameramove(kinectSensor, cameraangle);
                    }
                    cameracounter -= gtime;

                    // If round 1 is just about to begin then show video feed rather than frozen image
                    if (angletoguess == 0)
                    {
                        videoframe = kinectSensor.ColorStream.OpenNextFrame(0);
                        if (videoframe != null)
                        {
                            kinectRGBVideo = kfunctions.video2texture(graphics, videoframe);
                          //  kinectRGBVideo = kfunctions.video2texture(graphics, videoframe, vframe, vcolour);
                        }
                    }

                    // Once counter has reached zero begin round
                    if (countdown < 0)
                    {
                        if (round>0) funnypics[round - 1] = kinectRGBVideo;
                        if (saveallpics) savepictures();
                        ingamestate = 2;
                        resetround(gtime);
                    }

                    break;

                case 2:
                    // Round is being played

                    // Stream video from Kinect into imageframes
                    videoframe = kinectSensor.ColorStream.OpenNextFrame(0);
                    if (videoframe != null)
                    {
                        kinectRGBVideo = kfunctions.video2texture(graphics, videoframe);
                       // kinectRGBVideo = kfunctions.video2texture(graphics, videoframe, vframe, vcolour);
                    }

                    // Read Kinect Body sensor
                    body = kinectSensor.SkeletonStream.OpenNextFrame(0);
                    if (body != null)
                    {
                        // Get skeleton body data
                        Skeleton[] bodyskel = new Skeleton[body.SkeletonArrayLength];
                        body.CopySkeletonDataTo(bodyskel);

                        // Read Kinect and look for right hand movements on both players
                        int[] angles = new int[2];
                        kfunctions.readarmangle(bodyskel, JointType.HandRight, JointType.ElbowRight, out angles[0], out angles[1]);

                        // Handle lefties
                        int angletemp = 0;
                        if (gamer[0].lefthanded)
                            kfunctions.readarmangle(bodyskel, JointType.HandLeft, JointType.ElbowLeft, out angles[0], out angletemp);
                        if (gamer[1].lefthanded)
                            kfunctions.readarmangle(bodyskel, JointType.HandLeft, JointType.ElbowLeft, out angletemp, out angles[1]);

                        angles[0] = (int)(Math.Round((float)angles[0] / (float)round2nearest, 0) * round2nearest);
                        angles[1] = (int)(Math.Round((float)angles[1] / (float)round2nearest, 0) * round2nearest);

                        for (int i = 0; i < numberofplayers; i++)
                        {
                            if (gamer[i].countdown == handconfirmtime)
                            {
                                gamer[i].angle = angles[i];
                            }
                        }

                        // Check if either player has raised their hand
                        gestures.handup(bodyskel, false, out gamer[0].handisup, out gamer[1].handisup);

                        // Handle lefties
                        Boolean temphandup = false;
                        if (gamer[0].lefthanded)
                            gestures.handup(bodyskel, true, out gamer[0].handisup, out temphandup);
                        if (gamer[1].lefthanded)
                            gestures.handup(bodyskel, true, out temphandup, out gamer[1].handisup);
                    }

                    checkforhands(gtime);

                    if ((gamer[0].roundover && numberofplayers==1) || (gamer[0].roundover && gamer[1].roundover) || countdown < 0)
                    {
                        ingamestate = 1;
                        countdown = delaybetween;
                        for (int i = 0; i < numberofplayers; i++)
                        {
                            if (questiontype == 0)
                            {
                                gamer[i].score += angle2percentage(360 - Math.Abs((int)angletoguess - (int)gamer[i].angle));
                            }
                            else if (questiontype == 1 || questiontype == 3)
                            {
                                gamer[i].angle = angle2percentage((int)gamer[i].angle);
                                gamer[i].score += (100 - Math.Abs((int)angletoguess - (int)gamer[i].angle));
                            }
                            else if (questiontype == 2)
                            {
                                gamer[i].angle = angle2percentage((int)gamer[i].angle);
                                gamer[i].score += (100 - Math.Abs((int)(angletoguess * 100) - (int)gamer[i].angle));
                                gamer[i].angle /= 100f;
                            }

                            // Bonus points if they get the angle exactly right
                            if (gamer[i].angle == angletoguess)
                            {
                                gamer[i].score += bonuspoints;
                                applause[randomiser.Next(numberofsounds)].Play();
                            }
                        }
                    }

                    break;


                default:
                    // Game is over
                    piccounter += gtime;
                    if (piccounter >= questions2ask*2000)
                        piccounter = 0;

                    //if (keyboardreleased && gamer[0].score > highscores[9] && gamer[0].playername.Length < 12)
                    if (keyboardreleased && keyfocus==1)
                    {
                        if (keys.IsKeyDown(Keys.Enter))
                        {
                            if (gamer[1].score > highscores[9])
                                keyfocus = 2;
                            else
                                keyfocus = 0;
                        }
                        else if (keys.IsKeyDown(Keys.Back) && gamer[0].playername.Length > 0)
                        {
                            gamer[0].playername = gamer[0].playername.Substring(0, gamer[0].playername.Length - 1);
                        }
                        else
                        {
                            char nextchar = sfunctions.getnextkey();
                            if (nextchar != '!')
                            {
                                gamer[0].playername += nextchar;
                                if (gamer[0].playername.Length > 15)
                                    gamer[0].playername = gamer[0].playername.Substring(0, 15);
                            }
                        }
                    }
                    else if (keyboardreleased && keyfocus==2)
                    {
                        if (keys.IsKeyDown(Keys.Enter))
                        {
                            keyfocus = 0;
                        }
                        else if (keys.IsKeyDown(Keys.Back) && gamer[1].playername.Length > 0)
                        {
                            gamer[1].playername = gamer[1].playername.Substring(0, gamer[1].playername.Length - 1);
                        }
                        else
                        {
                            char nextchar = sfunctions.getnextkey();
                            if (nextchar != '!')
                            {
                                gamer[1].playername += nextchar;
                                if (gamer[1].playername.Length > 15)
                                    gamer[1].playername = gamer[1].playername.Substring(0, 15);
                            }
                        }
                    }

                    // Allow game to return to the main menu
                    if (keys.IsKeyDown(Keys.Escape))
                    {
                        if (gamer[0].score > highscores[9])
                        {
                            highscores[9] = gamer[0].score;
                            highscorenames[9] = gamer[0].playername.Trim();
                        }
                        // Sort the high score table
                        Array.Sort(highscores, highscorenames);
                        Array.Reverse(highscores);
                        Array.Reverse(highscorenames);

                        if (gamer[1].score > highscores[9])
                        {
                            highscores[9] = gamer[1].score;
                            highscorenames[9] = gamer[1].playername.Trim();
                        }
                        // Sort the high score table
                        Array.Sort(highscores, highscorenames);
                        Array.Reverse(highscores);
                        Array.Reverse(highscorenames);

                        gamestate = -1;
                    }

                    break;
            }

            if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.Back == ButtonState.Pressed)
                ingamestate = 3;    // End Game
        }

        public void checkforhands(float gtime)
        {
            // Count down if hand is up
            for (int i = 0; i < numberofplayers; i++)
            {
                if (!gamer[i].roundover)
                {
                    if (gamer[i].handisup)
                        gamer[i].countdown -= gtime;
                    else
                        gamer[i].countdown = handconfirmtime;
                }

                if (gamer[i].countdown < 0)
                    gamer[i].roundover = true;
            }
        }

        public void drawgame()
        {
            // Draw the in-game graphics
            spriteBatch.Begin();

            // Draw video
            spriteBatch.Draw(kinectRGBVideo, new Rectangle(0, 0, displaywidth, displayheight), Color.White);  // Draw video from kinect camera

            switch (ingamestate)
            {
                case 1:
                    // Round is over
                    gamebackground1.drawme(ref spriteBatch);
    
                    if (angletoguess > 0)
                    {
                        spriteBatch.DrawString(mainfont, angletoguess.ToString() + specialcharacter, new Vector2(displaywidth / 2 - 43, 8),
                           Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 1.8f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(mainfont, gamer[0].angle.ToString() + specialcharacter, new Vector2(80, 8),
                            Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 1.8f, SpriteEffects.None, 0);
                     
                        if (numberofplayers==2)
                            spriteBatch.DrawString(mainfont, gamer[1].angle.ToString() + specialcharacter, new Vector2((displaywidth / 2) + 210, 8),
                                Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 1.8f, SpriteEffects.None, 0);

                        // Display message if they got the exact angle
                        for (int i = 0; i < numberofplayers; i++)
                            if (gamer[i].angle == angletoguess)
                                if (i == 0)
                                    spriteBatch.DrawString(mainfont, "BONUS!", new Vector2(60, 70),
                                        Color.PaleVioletRed, MathHelper.ToRadians(0), new Vector2(0, 0), 2f, SpriteEffects.None, 0);
                                else
                                    spriteBatch.DrawString(mainfont, "BONUS!", new Vector2(displaywidth/2 + 60, 70),
                                        Color.PaleVioletRed, MathHelper.ToRadians(0), new Vector2(0, 0), 2f, SpriteEffects.None, 0);


                        // Draw angles
                        if (questiontype == 0)
                        {
                            drawangles((int)angletoguess, (int)gamer[0].angle, (int)gamer[1].angle);
                        }
                        else if (questiontype == 1 || questiontype==3)
                        {
                            drawangles(percentage2angle((int)angletoguess), percentage2angle((int)gamer[0].angle), percentage2angle((int)gamer[1].angle));
                        }
                        else if (questiontype == 2)
                        {
                            drawangles(decimal2angle(angletoguess), decimal2angle(gamer[0].angle), decimal2angle(gamer[1].angle));
                        }
                    }


                    break;

                case 2:
                    // Round is being played
                    gamebackground2.drawme(ref spriteBatch);

                    if (questiontype != 3)
                    {
                        spriteBatch.DrawString(mainfont, angletoguess.ToString() + specialcharacter, new Vector2(displaywidth / 2 - 43, 8),
                            Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 1.8f, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.DrawString(mainfont, numer.ToString(), new Vector2(displaywidth / 2 - 30, -2),
                            Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 1.2f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(mainfont, denom.ToString(), new Vector2(displaywidth / 2 - 30, 28),
                            Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 1.2f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(mainfont, "_", new Vector2(displaywidth / 2 - 38, -28),
                            Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 2f, SpriteEffects.None, 0);

                    }

                    // Draw angles
                    aline1.drawangle(ref spriteBatch, (int)gamer[0].angle);
                    if (numberofplayers == 2)
                        aline2.drawangle(ref spriteBatch, (int)gamer[1].angle);

                    drawhands();

                    break;

                default:
                    // Game is over
                    spriteBatch.Draw(funnypics[(int)(piccounter/2000)], new Rectangle(0, 0, displaywidth, displayheight), Color.White);  // Draw video from kinect camera
                    gamebackground3.drawme(ref spriteBatch);

                    if (gamer[0].score > gamer[1].score)
                    {
                        if (numberofplayers == 2)
                            spriteBatch.DrawString(mainfont, "Player 1 Wins", new Vector2(106, 10),
                            Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 2.5f, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.DrawString(mainfont, "Player 2 Wins", new Vector2(106, 10),
                        Color.Aquamarine, MathHelper.ToRadians(0), new Vector2(0, 0), 2.5f, SpriteEffects.None, 0);
                    }

                    if (keyfocus == 1)
                    {
                        spriteBatch.DrawString(mainfont, "P1 New High Score", new Vector2(80, 80),
                                Color.PaleVioletRed, MathHelper.ToRadians(0), new Vector2(0, 0), 2f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(mainfont, gamer[0].playername, new Vector2(60, 180),
                                Color.PaleVioletRed, MathHelper.ToRadians(0), new Vector2(0, 0), 2f, SpriteEffects.None, 0);
                    }

                    if (gamer[1].score > highscores[9] && keyfocus>0)
                    {
                        spriteBatch.DrawString(mainfont, "P2 New High Score", new Vector2(80, 300),
                            Color.LightBlue, MathHelper.ToRadians(0), new Vector2(0, 0), 2f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(mainfont, gamer[1].playername, new Vector2(60, 400),
                                Color.LightBlue, MathHelper.ToRadians(0), new Vector2(0, 0), 2f, SpriteEffects.None, 0);
                    }

                    spriteBatch.DrawString(mainfont, "GAME OVER", new Vector2(130, 460),
                        Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 2.5f, SpriteEffects.None, 0);

                    drawhands();

                    break;
            }

            spriteBatch.DrawString(mainfont, "P1:" + gamer[0].score.ToString("0"), new Vector2(32, displayheight - 53),
                        Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1.8f, SpriteEffects.None, 0);
            if (numberofplayers == 2)
                spriteBatch.DrawString(mainfont, "P2:" + gamer[1].score.ToString("0"), new Vector2((displaywidth / 2) + 158, displayheight - 53),
                        Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1.8f, SpriteEffects.None, 0);
//            spriteBatch.DrawString(mainfont, "P1: " + gamer[0].score.ToString("0"), new Vector2(30, 10),
  //                      Color.LightBlue, MathHelper.ToRadians(0), new Vector2(0, 0), 2f, SpriteEffects.None, 0);

            if (countdown > 0)
                spriteBatch.DrawString(digfont, (countdown / 1000).ToString("0"), new Vector2(displaywidth / 2 - 36, displayheight - 64),
                    Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 2f, SpriteEffects.None, 0);
            else
                gamelogo.drawme(ref spriteBatch);

            spriteBatch.End();
        }

        public void drawangles(int angle2draw, int p1ang, int p2ang)
        {
            if (angle2draw!=p1ang)
                aline1.drawangle(ref spriteBatch, angle2draw, Color.Black);
            aline1.drawangle(ref spriteBatch, p1ang);
            if (ingamestate == 1 && p1ang > angle2draw)
                aline1.drawangle(ref spriteBatch, angle2draw, Color.Black);

            if (numberofplayers == 2)
            {
                if (angle2draw != p2ang)
                    aline2.drawangle(ref spriteBatch, angle2draw, Color.Black);
                aline2.drawangle(ref spriteBatch, p2ang);
                if (ingamestate == 1 && gamer[1].angle > angletoguess)
                    aline2.drawangle(ref spriteBatch, angle2draw, Color.Black);
            }
        }

        public void drawhands()
        {
            // Draw hands if held up
            for (int i = 0; i < numberofplayers; i++)
            {
                if (gamer[i].countdown < handconfirmtime)
                {
                    if (gamer[i].countdown < 0)
                        hand[i].colour = Color.Aquamarine;
                    else
                        hand[i].colour = Color.White;
                    hand[i].drawme(ref spriteBatch);
                }
            }
        }


        public void updateoptions()
        {
            // Update code for the options screen
            optionselected = -1;
            for (int i = 0; i < numberoftypes; i++)
            {
                // Check for mouse over a menu option
                if (mousepointer1.bsphere.Intersects(typeoptions[i, 0].bbox))
                {
                    optionselected = i;
                    if (mouse.LeftButton == ButtonState.Pressed)
                        gamemode = optionselected;
                }
            }

            levelselected = -1;
            for (int i = 0; i < numberoflevels; i++)
            {
                // Check for mouse over a menu option
                if (mousepointer1.bsphere.Intersects(leveloptions[i, 0].bbox))
                {
                    levelselected = i;
                    if (mouse.LeftButton == ButtonState.Pressed)
                        skilllevel = levelselected;
                }
            }

            for (int i=0; i<2; i++)
                if (mousepointer1.bsphere.Intersects(lefthanded[i, 0].bbox) && mouse.LeftButton == ButtonState.Pressed && mousereleased)
                {
                    mousereleased = false;
                    gamer[i].lefthanded = !gamer[i].lefthanded;
                }

            // Check for up and down clicked for questions to ask
            if (mousepointer1.bsphere.Intersects(questionspinner[1].bbox) && mouse.LeftButton == ButtonState.Pressed && mousereleased)
            {
                mousereleased = false;
                if (questions2ask < 99)
                    questions2ask++;
            }
            if (mousepointer1.bsphere.Intersects(questionspinner[2].bbox) && mouse.LeftButton == ButtonState.Pressed && mousereleased)
            {
                mousereleased = false;
                if (questions2ask>1)
                    questions2ask--;
            }

            // Allow game to return to the main menu
            onback = mousepointer1.bsphere.Intersects(backbutton[0].bbox);
            if (onback && mouse.LeftButton == ButtonState.Pressed)
            {
                gamestate = -1;
                loadhighscores("highscore" + gamemode.ToString("0") + skilllevel.ToString("0") + questions2ask.ToString("00") + ".txt");
            }

            if (savepicbutton[0, 0].bbox.Intersects(mousepointer1.bsphere))
            {
                onback = true;

                if (mousereleased && mouse.LeftButton == ButtonState.Pressed)
                {
                    mousereleased = false;
                    saveallpics = !saveallpics;
                }
            }

            if (fullscreenbutton[0, 0].bbox.Intersects(mousepointer1.bsphere))
            {
                onback = true;

                if (mousereleased && mouse.LeftButton == ButtonState.Pressed)
                {
                    mousereleased = false;
                    fullscreenstate = !fullscreenstate;
                    graphics.IsFullScreen = fullscreenstate;
                    graphics.ApplyChanges();
                }
            }

            if (mouse.LeftButton != ButtonState.Pressed) mousereleased = true;


            if (mouse.LeftButton != ButtonState.Pressed) mousereleased = true;
        }

        public void drawoptions()
        {
            // Draw graphics for OPTIONS screen
            spriteBatch.Begin();
            background.drawme(ref spriteBatch);

            if (!onback)
                backbutton[0].drawme(ref spriteBatch);
            else
                backbutton[1].drawme(ref spriteBatch);

            for (int i = 0; i < numberoftypes; i++)
            {
                if (gamemode == i)
                {
                    typeoptions[i, 0].colour = Color.Yellow;
                    typeoptions[i, 1].colour = Color.Yellow;
                }
                else
                {
                    typeoptions[i, 0].colour = Color.White;
                    typeoptions[i, 1].colour = Color.White;
                }

                if (optionselected == i)
                    typeoptions[i, 1].drawme(ref spriteBatch);
                else
                    typeoptions[i, 0].drawme(ref spriteBatch);
            }


            for (int i = 0; i < numberoflevels; i++)
            {
                if (skilllevel == i)
                {
                    leveloptions[i, 0].colour = Color.Yellow;
                    leveloptions[i, 1].colour = Color.Yellow;
                }
                else
                {
                    leveloptions[i, 0].colour = Color.White;
                    leveloptions[i, 1].colour = Color.White;
                }

                if (levelselected == i)
                    leveloptions[i, 1].drawme(ref spriteBatch);
                else
                    leveloptions[i, 0].drawme(ref spriteBatch);
            }

            for (int i = 0; i < 2; i++)
                if (mousepointer1.bsphere.Intersects(lefthanded[i, 0].bbox))
                {
                    if (gamer[i].lefthanded)
                        lefthanded[i, 1].drawme(ref spriteBatch);
                    else
                        lefthanded[i, 3].drawme(ref spriteBatch);

                    onback = true;
                }
                else
                    if (gamer[i].lefthanded)
                        lefthanded[i, 0].drawme(ref spriteBatch);
                    else
                        lefthanded[i, 2].drawme(ref spriteBatch);

            questionsto.drawme(ref spriteBatch);
            questionspinner[0].drawme(ref spriteBatch);

            if (mousepointer1.bsphere.Intersects(questionspinner[1].bbox))
            {
                questionspinner[1].drawme(ref spriteBatch);
                onback = true;
            }
            if (mousepointer1.bsphere.Intersects(questionspinner[2].bbox))
            {
                questionspinner[2].drawme(ref spriteBatch);
                onback = true;
            }
            spriteBatch.DrawString(fontwhite, questions2ask.ToString("00"), new Vector2(483, 382), new Color(69, 99, 53), 0, new Vector2(0, 0), 1.2f, SpriteEffects.None, 0);

            // Draw save pictures button
            if (saveallpics)
            {
                if (savepicbutton[0, 0].bbox.Intersects(mousepointer1.bsphere))
                    savepicbutton[0, 1].drawme(ref spriteBatch);
                else
                    savepicbutton[0, 0].drawme(ref spriteBatch);
            }
            else
            {
                if (savepicbutton[0, 0].bbox.Intersects(mousepointer1.bsphere))
                    savepicbutton[1, 1].drawme(ref spriteBatch);
                else
                    savepicbutton[1, 0].drawme(ref spriteBatch);
            }

            if (!fullscreenstate)
            {
                if (fullscreenbutton[0, 0].bbox.Intersects(mousepointer1.bsphere))
                    fullscreenbutton[0, 1].drawme(ref spriteBatch);
                else
                    fullscreenbutton[0, 0].drawme(ref spriteBatch);
            }
            else
            {
                if (fullscreenbutton[0, 0].bbox.Intersects(mousepointer1.bsphere))
                    fullscreenbutton[1, 1].drawme(ref spriteBatch);
                else
                    fullscreenbutton[1, 0].drawme(ref spriteBatch);
            }



            // Draw mouse
            if (optionselected > -1 || onback || levelselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);


            spriteBatch.DrawString(fontwhite, "Programming and Design: David Renton ", new Vector2(290, displayheight - 50), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(fontwhite, "Graphic Artist: Christopher Gillies", new Vector2(290, displayheight - 35), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
//            spriteBatch.DrawString(fontwhite, "Sound Effects: www.SoundJay.com", new Vector2(290, displayheight - 20), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);

            spriteBatch.End();
        }

        public void updatehighscore()
        {
            // Update code for the high score screen
            // Allow game to return to the main menu
            // Allow game to return to the main menu
            onback = mousepointer1.bsphere.Intersects(backbutton[0].bbox);
            if (onback && mouse.LeftButton == ButtonState.Pressed)
            {
                gamestate = -1;
            }
        }

        public void drawhighscore()
        {
            // Draw graphics for High Score table
            spriteBatch.Begin();
            background.drawme(ref spriteBatch);
            if (!onback)
                backbutton[0].drawme(ref spriteBatch);
            else
                backbutton[1].drawme(ref spriteBatch);
            
            // Draw top ten high scores
            for (int i = 0; i < numberofhighscores; i++)
            {
                if (highscorenames[i].Length >= 15)
                    spriteBatch.DrawString(mainfont, (i + 1).ToString("0") + ". " + highscorenames[i].Substring(0, 15), new Vector2(156, 96 + (i * 30)),
                        Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                else
                    spriteBatch.DrawString(mainfont, (i + 1).ToString("0") + ". " + highscorenames[i], new Vector2(156, 96 + (i * 30)),
                        Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);

                spriteBatch.DrawString(mainfont, highscores[i].ToString("0"), new Vector2(displaywidth - 210, 96 + (i * 30)),
                    Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            }



            spriteBatch.DrawString(fontwhite, "Programming and Design: David Renton ", new Vector2(290, displayheight - 50), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(fontwhite, "Graphic Artist: Christopher Gillies", new Vector2(290, displayheight - 35), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
//            spriteBatch.DrawString(fontwhite, "Sound Effects: www.SoundJay.com", new Vector2(290, displayheight - 20), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);

            // Draw mouse
            if (onback)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);

            spriteBatch.End();
        }





        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here


            // Draw stuff depending on the game state
            switch (gamestate)
            {
                case -1:
                    // Game is on the main menu
                    drawmenu();
                    break;
                case 0:
                    // Game is being played
                    drawgame();
                    break;
                case 1:
                    // Game is being played
                    drawgame();
                    break;
                case 2:
                    // Options menu
                    drawoptions();
                    break;
                case 3:
                    // High Score table
                    drawhighscore();
                    break;
                default:
                    break;
            }

            base.Draw(gameTime);
        }
    }
}
