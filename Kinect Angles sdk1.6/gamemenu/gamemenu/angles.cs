using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace angles
{
    public static class makeangles
    {
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
            return (int)(Math.Round((float)numerator / (float)denominator * 100f, 0));
        }

        // Converts a decimal to a percentage
        public static int decimal2percentage(float decnumber)
        {
            return (int)(Math.Round(decnumber * 100, 0));
        }

        // Generate Bearing
        public static string generatebearing(int skill, out int round2, out float angles2)
        {
            string bearing = "";
            round2 = 0;
            angles2 = 0;
            Random randomiser = new Random();       // Variable to generate random numbers
            if (skill == 0)
            {
                round2 = 45;
                int choice = randomiser.Next(4);
                if (choice == 0) { bearing = "North"; angles2 = 360;}
                if (choice == 1) { bearing = "East"; angles2 = 90; }
                if (choice == 2) { bearing = "South"; angles2 = 180; }
                if (choice == 3) { bearing = "West"; angles2 = 270; }
            }
            else if (skill == 1)
            {
                round2 = 45;
                int choice = randomiser.Next(8);
                if (choice == 0) { bearing = "North"; angles2 = 360; }
                if (choice == 1) { bearing = "East"; angles2 = 90; }
                if (choice == 2) { bearing = "South"; angles2 = 180; }
                if (choice == 3) { bearing = "West"; angles2 = 270; }
                if (choice == 4) { bearing = "NE"; angles2 = 45; }
                if (choice == 5) { bearing = "SE"; angles2 = 135; }
                if (choice == 6) { bearing = "SW"; angles2 = 225; }
                if (choice == 7) { bearing = "NW"; angles2 = 315; }

            }
            else if (skill ==2)
            {
                round2 = 1;
                int choice = randomiser.Next(8);
                if (choice == 0) { bearing = "North"; angles2 = 360; }
                if (choice == 1) { bearing = "East"; angles2 = 90; }
                if (choice == 2) { bearing = "South"; angles2 = 180; }
                if (choice == 3) { bearing = "West"; angles2 = 270; }
                if (choice == 4) { bearing = "NE"; angles2 = 45; }
                if (choice == 5) { bearing = "SE"; angles2 = 135; }
                if (choice == 6) { bearing = "SW"; angles2 = 225; }
                if (choice == 7) { bearing = "NW"; angles2 = 315; }
            }
            return bearing;
        }

        // Generate random fraction
        public static void generatefraction(out int numerator, out int denominator, int skill, out int round2)
        {
            Random randomiser = new Random();       // Variable to generate random numbers
            round2 = 1;

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
        public static int generatepercentage(int skill, out int round2)
        {
            Random randomiser = new Random();       // Variable to generate random numbers
            if (skill == 0)
            {
                round2 = 45;
                return ((randomiser.Next(4) + 1) * 25);
            }
            else if (skill == 1)
            {
                round2 = 9;
                return ((randomiser.Next(20) + 1) * 5);
            }
            else
            {
                round2 = 1;
                return (randomiser.Next(100) + 1);
            }
        }

        // Generate random angle in degrees
        public static int generateangle(int skill, out int round2)
        {
            Random randomiser = new Random();       // Variable to generate random numbers
            if (skill == 0)
            {
                round2 = 45;
                return (randomiser.Next(4) + 1) * 90;
            }
            else if (skill == 1)
            {
                round2 = 5;
                return (randomiser.Next(36) + 1) * 10;
            }
            else
            {
                round2 = 1;
                return (randomiser.Next(360) + 1);
            }
        }

        // Generate random decimal number
        public static float generatedecimal(int skill, out int round2)
        {
            Random randomiser = new Random();       // Variable to generate random numbers
            float decimalvalue = 0;
            round2 = 0;
            if (skill == 0)
            {
                round2 = 45;
                decimalvalue = ((randomiser.Next(4) + 1) * 25);
            }
            else if (skill == 1)
            {
                round2 = 9;
                decimalvalue = ((randomiser.Next(20) + 1) * 5);
            }
            else if (skill == 2)
            {
                round2 = 1;
                decimalvalue = (randomiser.Next(100) + 1);
            }

            return (decimalvalue / 100f);
        }


    }
}