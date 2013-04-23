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

namespace kinect
{
    public static class gestures
    {
        public static void handup( Skeleton[] skeldata, Boolean right, out Boolean player1isup, out Boolean player2isup)
        {
            player1isup = false;
            player2isup = false;

            JointType joint1, joint2;

            

            if (right)
            {
                joint1 = JointType.HandRight;
                joint2 = JointType.ShoulderRight;
            }
            else
            {
                joint1 = JointType.HandLeft;
                joint2 = JointType.ShoulderLeft;
            }

            Vector2 joint1pos = new Vector2(0, 0);
            Vector2 joint2pos = new Vector2(0, 0);

            float posx1 = 3;
            float posx2 = 3;

            foreach (Skeleton data in skeldata)
            {
                if (data.TrackingState == SkeletonTrackingState.Tracked)
                {
                    foreach (Joint joint in data.Joints)
                    {
                        if (joint.JointType == joint1)
                        {
                            joint1pos.X = joint.Position.X;
                            joint1pos.Y = joint.Position.Y;
                        }
                        if (joint.JointType == joint2)
                        {
                            joint2pos.X = joint.Position.X;
                            joint2pos.Y = joint.Position.Y;
                        }
                    }

                    // Check if hand is up
                    Boolean handup = ((joint1pos.Y - joint2pos.Y) > 0.02f);

                    // Decide which joint is p1 and p2
                    if (joint2pos.X < posx1)
                    {
                        player2isup = player1isup;
                        player1isup = handup;
                        posx2 = posx1;
                        posx1 = joint2pos.X;
                    }
                    else
                    {
                        player2isup = handup;
                        posx2 = joint1pos.X;
                    }
                }
            }
        }

    }

    public static class kfunctions
    {
        
        public static Texture2D video2texture(GraphicsDeviceManager graphics, ColorImageFrame videoframe)
        {
            Texture2D frame = new Texture2D(graphics.GraphicsDevice, videoframe.Width, videoframe.Height);
            Color[] colour = new Color[videoframe.Height * videoframe.Width];
            Byte[] pixeldata = new Byte[videoframe.PixelDataLength];

            videoframe.CopyPixelDataTo(pixeldata);

            int index = 0;
            for (int y = 0; y < videoframe.Height; y++)
            {
                for (int x = 0; x < videoframe.Width; x++, index += 4)
                {
                    colour[y * videoframe.Width + x] = new Color(pixeldata[index + 2], pixeldata[index + 1], pixeldata[index + 0]);
                }
            }

            // Set pixeldata from the ColorImageFrame to a Texture2D
            frame.SetData(colour);

            return frame;
        }

        public static Texture2D video2texture(GraphicsDeviceManager graphics, ColorImageFrame videoframe, Texture2D frame, Color[] colour)
        {
            Byte[] pixeldata = new Byte[videoframe.PixelDataLength];
            videoframe.CopyPixelDataTo(pixeldata);

            int index = 0;
            for (int y = 0; y < videoframe.Height; y++)
            {
                for (int x = 0; x < videoframe.Width; x++, index += 4)
                {
                    colour[y * videoframe.Width + x] = new Color(pixeldata[index + 2], pixeldata[index + 1], pixeldata[index + 0]);
                }
            }

            // Set pixeldata from the ColorImageFrame to a Texture2D
            frame.SetData(colour);

            return frame;
        }


        public static void readjoints(Skeleton[] skeldata, JointType jointname, int dwidth, int dheight, float scalefactor, out Vector3 p1pos, out Vector3 p2pos)
        {
            p1pos = new Vector3(2000, 0, 0);
            p2pos = new Vector3(-2000, 0, 0);
            foreach (Skeleton data in skeldata)
            {
                if (data.TrackingState == SkeletonTrackingState.Tracked)
                {
                    foreach (Joint joint in data.Joints)
                    {
                        if (joint.JointType == jointname)
                        {
                            Vector3 temppos;
                            temppos.X = (joint.Position.X + scalefactor) * (dwidth / (2*scalefactor));
                            temppos.Y = (scalefactor - joint.Position.Y) * (dheight / (2*scalefactor));
                            temppos.Z = (joint.Position.Z) * (dwidth/ (2*scalefactor));

                            if (temppos.X < p1pos.X)
                            {
                                p2pos = p1pos;
                                p1pos = temppos;
                            }
                            else
                            {
                                p2pos = temppos;
                            }
                        }
                    }
                }
            }
        }

        public static void readarmangle(Skeleton[] skeldata, JointType joint1, JointType joint2 ,out int ang1, out int ang2)
        {
            ang1 = 0; ang2 = 0;

            Vector2 joint1pos = new Vector2(0, 0);
            Vector2 joint2pos = new Vector2(0,0);
            float posx1 = 2000;
            float posx2 = 0;
            
            int ang=0;
            
            foreach (Skeleton data in skeldata)
            {
                if (data.TrackingState == SkeletonTrackingState.Tracked)
                {
                    foreach (Joint joint in data.Joints)
                    {
                        if (joint.JointType == joint1)
                        {
                            joint1pos.X = joint.Position.X;
                            joint1pos.Y = joint.Position.Y;
                        }
                        if (joint.JointType == joint2)
                        {
                            joint2pos.X = joint.Position.X;
                            joint2pos.Y = joint.Position.Y;
                        }
                    }

                    Vector2 anglevec = joint1pos - joint2pos;
                    ang = (int)MathHelper.ToDegrees((float)Math.Atan2(anglevec.X, anglevec.Y));
                    if (ang < 0) ang += 360;

                    // Decide if the angle is for player 1 or player 2 based on the position of both users rightshoulders
                    if (joint2pos.X < posx1)
                    {
                        ang2 = ang1;
                        ang1 = ang;
                        posx2 = posx1;
                        posx1 = joint2pos.X;
                    }
                    else
                    {
                        ang2 = ang;
                        posx2 = joint2pos.X;
                    }
                }
            }
        }

        public static int cameramove(KinectSensor kinectSensor, int cameraangle)
        {
            KeyboardState keys = Keyboard.GetState();                     // Read keyboard

            // Allow the user to move the kinect camera tilt up and down
            if (keys.IsKeyDown(Keys.Up))
            {
                cameraangle += 3;
                if (cameraangle > 27) cameraangle = 27;
                kinectSensor.ElevationAngle = (int)cameraangle;
            }
            if (keys.IsKeyDown(Keys.Down))
            {
                cameraangle -= 3;
                if (cameraangle < -27) cameraangle = -27;
                kinectSensor.ElevationAngle = (int)cameraangle;
            }
            return cameraangle;
        }


        public static Vector2 readjoint(Skeleton[] skeldata, JointType jointname, int dwidth, int dheight)
        {
            Vector2 pos = new Vector2(0, 0);
            foreach (Skeleton data in skeldata)
            {
                if (data.TrackingState == SkeletonTrackingState.Tracked)
                {
                    foreach (Joint joint in data.Joints)
                    {
                        if (joint.JointType == jointname)
                        {
                            pos.X = (joint.Position.X + 1)*(dwidth/2);
                            pos.Y = (1 - joint.Position.Y)*(dheight/2);
                        }
                    }
                }
            }
            return pos;
        }


        public static void readkinect(Skeleton[] skeldata, JointType jointname, ref Vector3 play1, ref Vector3 play2, ref Boolean p1ai, ref Boolean p2ai, int displaywidth, int displayheight)
        {
            Vector2 p1 = new Vector2(100, 0);
            Vector2 p2 = new Vector2(100, 0);
            foreach (Skeleton data in skeldata)
            {
                if (data.TrackingState == SkeletonTrackingState.Tracked)
                {
                    foreach (Joint joint in data.Joints)
                    {
                        if (joint.JointType == jointname)
                        {
                            if (p1.X == 100)
                            {
                                p1.Y = -joint.Position.Y * 1.2f;
                                p1.X = joint.Position.X;
                            }
                            else
                            {
                                p2.Y = -joint.Position.Y * 1.2f;
                                p2.X = joint.Position.X;
                            }
                        }
                    }
                }
            }
            if (p1.X < p2.X)
            {
                if (p1.X != 100)
                {
                    play1.X = ScaleVector(displaywidth, p1.X);
                    play1.Y = ScaleVector2(displayheight, p1.Y);
                }
                if (p2.X != 100)
                {
                    play2.X = ScaleVector(displaywidth, p2.X);
                    play2.Y = ScaleVector2(displayheight, p2.Y);
                }
                p1ai = (p1.X == 100);
                p2ai = (p2.X == 100);
            }
            else
            {
                if (p2.X != 100)
                {
                    play1.X = ScaleVector(displaywidth, p2.X);
                    play1.Y = ScaleVector2(displayheight, p2.Y);
                }
                if (p1.X != 100)
                {
                    play2.X = ScaleVector(displaywidth, p1.X);
                    play2.Y = ScaleVector2(displayheight, p1.Y);
                }
                p2ai = (p1.X == 100);
                p1ai = (p2.X == 100);
            }
        }

        private static float ScaleVector(int length, float position)
        {
            float newposition = (position * ((float)length / 2f) + (length / 2));

            if (newposition < 0)
                return 0;
            if (newposition > length)
                return length;
            return newposition;
        }
        private static float ScaleVector2(int length, float position)
        {
            float newposition = (position * (float)length) + (length);

            if (newposition < 0)
                return 0;
            if (newposition > length)
                return length;
            return newposition;
        }

        //public static int GetDistanceAndPlayerIndex(ImageFrame image, int x, int y, out int PlayerIndex)
        //{
        //    PlayerIndex = 0;
        //    int distance = 0;
        //    int index = (image.Image.Width * y) + x;
        //    index *= 2;
        //    distance = (int)((image.Image.Bits[index] >> 3) | (image.Image.Bits[index + 1] << 5));
        //    PlayerIndex = (image.Image.Bits[index] & 7);
        //    return distance;
        //}


    }
}