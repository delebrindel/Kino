using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Input.Touch; // Include for Windows Phone games
using Microsoft.Xna.Framework.Media;
using System.IO;


namespace gamelib1
{
    // Class for 2D graphics
    public class graphic2d
    {
        public Texture2D image;                 // Texture to hold image
        public Rectangle rect;                  // Rectangle to hold position & size of the image

        public graphic2d() { }  // Empty constructor to avoid crashes

        // Constructor which loads image and fits the background to fill the width of the screen
        public graphic2d(ContentManager content, string spritename, int dwidth, int dheight)
        {
            image = content.Load<Texture2D>(spritename);
            float ratio = ((float)dwidth / image.Width);
            rect.Width = dwidth;
            rect.Height = (int)(image.Height * ratio);
            rect.X = 0;
            rect.Y = (dheight - rect.Height) / 2;
        }

        public graphic2d(GraphicsDeviceManager graphics, ContentManager content, string spritename, int dwidth, int dheight)
        {
            Stream picstream = File.Open(spritename, FileMode.Open);
            image = Texture2D.FromStream(graphics.GraphicsDevice, picstream); // Load an image from the HDD not in the content pipeline
            picstream.Close();
            float ratio = ((float)dwidth / image.Width);    // Work out the ratio for the image depending on screen size
            rect.Width = dwidth;                            // Set image width to match the screen width
            rect.Height = (int)(image.Height * ratio);      // Work out new height based on the screen aspect ratio
            rect.X = 0;
            rect.Y = (dheight - rect.Height) / 2;           // Put image in the middle of the screen on the Y axis
        }

        public void stretch2fit(int dwidth, int dheight)
        {
            rect.Width = dwidth;
            rect.Height = dheight;
            rect.X = 0;
            rect.Y = 0;
        }

        // Use this method to draw the image
        public void drawme(ref SpriteBatch spriteBatch2)
        {
            spriteBatch2.Draw(image, rect, Color.White);
        }
    }


    // Class for 2D sprites
    public class sprite2d
    {
        public Texture2D image;         		// Texture which holds image
        public Vector3 position; 		 	    // Position on screen
        public Vector3 oldposition;             // Old position before collisions
        public Rectangle rect;          		// Rectangle to hold size and position
        public Vector2 origin;          		// Centre point
        public float rotation = 0;          	// Amount of rotation to apply
        public float rotspeed = 0.05f;          // Speed they should spin at
        public Vector3 velocity;        		// Velocity (Direction and speed)
        public BoundingSphere bsphere;  		// Bounding sphere
        public BoundingBox bbox;                // Bounding box
        public Boolean visible = true;    		// Should object be drawn true or false
        public Color colour = Color.White;      // Holds colour to draw the image in
        public float size;                      // Size ratio of object

        public sprite2d() { }                   // Empty constructor to avoid crashes

        // Constructor which initialises the sprite2D
        public sprite2d(ContentManager content, string spritename, int x, int y, float msize, Color mcolour, Boolean mvis)
        {
            image = content.Load<Texture2D>(spritename);    // Load image into texture
            position = new Vector3((float)x, (float)y, 0);  // Set position
            rect.X = x;                                     // Set position of draw rectangle x
            rect.Y = y;                                     // Set position of draw rectangle y
            origin.X = image.Width / 2;               	    // Set X origin to half of width
            origin.Y = image.Height / 2;              	    // Set Y origin to half of height
            rect.Width = (int)(image.Width * msize);  	    // Set the new width based on the size ratio 
            rect.Height = (int)(image.Height * msize);	    // Set the new height based on the size ratio
            colour = mcolour;                               // Set colour
            visible = mvis;                                 // Image visible TRUE of FALSE? 
            size = msize;                                   // Store size ratio
            oldposition = position;
            updateobject();
        }

        public void automove(int dwidth, int dheight, float gtime)
        {
            // Add code here for when the game is running
            rotation += rotspeed; // Spin Ball
            position += velocity * gtime; // Add current velocity to the position of the ball

            // Check if the ball hits any of the four sides and bounce it off them
            if ((position.X + rect.Width / 2) >= dwidth)
            {
                velocity.X = -velocity.X;
                position.X = dwidth - rect.Width / 2;
            }
            if ((position.X - rect.Width / 2) <= 0)
            {
                velocity.X = -velocity.X;
                position.X = rect.Width / 2;
            }
            if ((position.Y + rect.Height / 2) >= dheight)
            {
                velocity.Y = -velocity.Y;
                position.Y = dheight - rect.Height / 2;
            }
            if ((position.Y - rect.Height / 2) <= 0)
            {
                velocity.Y = -velocity.Y;
                position.Y = rect.Height / 2;
            }
            updateobject();
        }

        public void moveme(GamePadState gpad, float gtime, int dwidth, int dheight)
        {
            if (visible)
            {
                // Basic Movement Left, Right, Up, Down
                velocity.X = gpad.ThumbSticks.Left.X;
                velocity.Y = -gpad.ThumbSticks.Left.Y;

                float speed = 0.5f;
                position += velocity * gtime * speed;   // Set position based on velocity, time between updates and speed

                // Set screen limits for object
                if (position.X < rect.Width / 2) position.X = rect.Width / 2;
                if (position.X > dwidth - rect.Width / 2) position.X = dwidth - rect.Width / 2;
                if (position.Y < rect.Height / 2) position.Y = rect.Height / 2;
                if (position.Y > dheight - rect.Height / 2) position.Y = dheight - rect.Height / 2;

                updateobject();
            }
        }

        public void updateobject()
        {
            // Set position of object into the rectangle from the position Vector2
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;
            // Create Boundingsphere around the object
            bsphere = new BoundingSphere(position, rect.Width / 2);
            // Create Boundingbox around the object
            bbox = new BoundingBox(new Vector3(position.X - rect.Width / 2, position.Y - rect.Height / 2, 0),
                                    new Vector3(position.X + rect.Width / 2, position.Y + rect.Height / 2, 0));
        }

        // Use this method to draw the image
        public void drawme(ref SpriteBatch sbatch)
        {
            if (visible)
                sbatch.Draw(image, rect, null, colour, rotation, origin, SpriteEffects.None, 0);
        }

        // Use this method to draw the image at a specified position
        public void drawme(ref SpriteBatch sbatch, Vector3 newpos)
        {
            if (visible)
            {
                Rectangle newrect = rect;
                newrect.X = (int)newpos.X;
                newrect.Y = (int)newpos.Y;

                sbatch.Draw(image, newrect, null, colour, rotation, origin, SpriteEffects.None, 0);
            }
        }
    }

    public static class sfunctions
    {
        public static char getnextkey()
        {
            // Read keyboard
            KeyboardState keys = Keyboard.GetState();                    
            if (keys.IsKeyDown(Keys.A))
                return 'A';
            else if (keys.IsKeyDown(Keys.B))
                return 'B';
            else if (keys.IsKeyDown(Keys.C))
                return 'C';
            else if (keys.IsKeyDown(Keys.D))
                return 'D';
            else if (keys.IsKeyDown(Keys.E))
                return 'E';
            else if (keys.IsKeyDown(Keys.F))
                return 'F';
            else if (keys.IsKeyDown(Keys.G))
                return 'G';
            else if (keys.IsKeyDown(Keys.H))
                return 'H';
            else if (keys.IsKeyDown(Keys.I))
                return 'I';
            else if (keys.IsKeyDown(Keys.J))
                return 'J';
            else if (keys.IsKeyDown(Keys.K))
                return 'K';
            else if (keys.IsKeyDown(Keys.L))
                return 'L';
            else if (keys.IsKeyDown(Keys.M))
                return 'M';
            else if (keys.IsKeyDown(Keys.N))
                return 'N';
            else if (keys.IsKeyDown(Keys.O))
                return 'O';
            else if (keys.IsKeyDown(Keys.P))
                return 'P';
            else if (keys.IsKeyDown(Keys.Q))
                return 'Q';
            else if (keys.IsKeyDown(Keys.R))
                return 'R';
            else if (keys.IsKeyDown(Keys.S))
                return 'S';
            else if (keys.IsKeyDown(Keys.T))
                return 'T';
            else if (keys.IsKeyDown(Keys.U))
                return 'U';
            else if (keys.IsKeyDown(Keys.V))
                return 'V';
            else if (keys.IsKeyDown(Keys.W))
                return 'W';
            else if (keys.IsKeyDown(Keys.X))
                return 'X';
            else if (keys.IsKeyDown(Keys.Y))
                return 'Y';
            else if (keys.IsKeyDown(Keys.Z))
                return 'Z';
            else if (keys.IsKeyDown(Keys.Space))
                return ' ';
            else
                return '!';
        }

    }


}