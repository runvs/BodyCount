using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

namespace Empty_CSharp_Application
{
    class GameSettings
    {
        public static SFML.Window.Vector2f GetBoatStartingPosition (  ) 
        {
            SFML.Window.Vector2f vec = new Vector2f(400, 300);

            return vec;
        }

        public static SFML.Graphics.Color GetBackgroundColor()
        {
            return new SFML.Graphics.Color(4,4, 22);
        }

        public static float BoatFuelPercentageDrainPerSecond()
        {
            return 3.5f;
        }

        public static float BoatAngularVelocityOnSteering()
        {
            return 3.0f;
        }

        public static float BoatAngularVelocityDampingFactor()
        {
            return 0.95f;
        }

        public static float BoatAcceleration()
        {
            return 60.0f;
        }

        public static float BoatLinearVelocityDampingFactor()
        {
            return 0.99f;
        }

        public static int BoatMaximumNumberOfPersonsToCarry()
        {
            return 3;
        }

        static private SFML.Graphics.Font gameFont = new Font("../gfx/font.ttf");

        static public SFML.Graphics.Font GameFont()
        {
            return gameFont;
        }

        static public SFML.Window.Vector2f GetWorldSize()
        {
            return new Vector2f(800, 200);
        }

        static public float ScrollMargin()
        {
            return 250.0f;
        }

        static public float FasterScrollMargin()
        {
            return 150.0f;
        }



        internal static float BoatPersonCrushingVelocitySquared()
        {
            return 2000.0f;
        }

        internal static Vector2f GetCloudMovement()
        {
            return new SFML.Window.Vector2f(-10.0f, -5.0f );
        }
    }
}
