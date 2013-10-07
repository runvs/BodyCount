using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

namespace Empty_CSharp_Application
{
    class Boat
    {

        public Boat()
        {
            LoadGraphics();
            SetPosition(GameSettings.GetBoatStartingPosition());

            NumberOfPersonsCarried = 0;
        }


        private SFML.Window.Vector2f velocity = new Vector2f(30.0f, 0);

        public SFML.Window.Vector2f GetVelocity()
        {
            return velocity;
        }

        public float GetAbsoluteVelocitySquared()
        {
            return velocity.X * velocity.X + velocity.Y * velocity.Y;
        }

        private SFML.Window.Vector2f acceleration = new Vector2f(0, 0);

        public float FuelPercentage { set; get; }

        private float rotation = 0.0f;
        private float angularVelocity = 90.0f;

        private bool hasAcceleratedThisFrame = false;

        private SFML.Graphics.Texture textureBoat;
        private SFML.Graphics.Sprite spriteBoat;

        public int NumberOfPersonsCarried { get; private set; }

        internal void GetInput()
        {
            if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.A) || SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Left))
            {
                RotateLeft();
            }
            if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.D) || SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Right))
            {
                RotateRight();
            }
            if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.W) || SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Up))
            {
                Accelerate();
            }
        }

        private void RotateLeft()
        {
            angularVelocity -= GameSettings.BoatAngularVelocityOnSteering();
        }
        private void RotateRight()
        {
            angularVelocity += GameSettings.BoatAngularVelocityOnSteering();
        }

        private void Accelerate()
        {
            hasAcceleratedThisFrame = true; // set the state for acceleration
            if (FuelPercentage > 0.0f)
            {
                SFML.Window.Vector2f deltaAcceleration = new Vector2f();

                deltaAcceleration.X = (float)(Math.Cos((rotation - 90.0f) * Math.PI / 180.0f));
                deltaAcceleration.Y = (float)(Math.Sin((rotation - 90.0f) * Math.PI / 180.0f));

                deltaAcceleration *= GameSettings.BoatAcceleration();

                acceleration = deltaAcceleration;

                //System.Console.Out.WriteLine(deltaAcceleration);
            }
            else
            {
                //System.Console.Out.WriteLine("Out Of Fuel");
            }
        }

        public void Update(float deltaT)
        {
            // Movement
            DoBoatMovement(deltaT);

            if (hasAcceleratedThisFrame)
            {
                FuelPercentage -= GameSettings.BoatFuelPercentageDrainPerSecond() * deltaT;
            }
            hasAcceleratedThisFrame = false;
        }

        private void DoBoatMovement(float deltaT)
        {
            
            SFML.Window.Vector2f newPosition = position;
            float newRotation = rotation;
            
            newPosition += velocity * deltaT;

            velocity += acceleration * deltaT;
            velocity *= GameSettings.BoatLinearVelocityDampingFactor();

            newRotation += angularVelocity * deltaT;
            angularVelocity *= GameSettings.BoatAngularVelocityDampingFactor();

            SetPosition(newPosition);
            UpdateSpirteRotation(newRotation);

            acceleration = new Vector2f(0, 0);

            
        }

        public void Draw(SFML.Graphics.RenderWindow rw, SFML.Window.Vector2f camPosition)
        {
            spriteBoat.Position -= camPosition;
            rw.Draw(spriteBoat);
            spriteBoat.Position += camPosition;
        }


        private SFML.Window.Vector2f position;

        public void SetPosition(SFML.Window.Vector2f newPos)
        {
            position = newPos;
            spriteBoat.Position = newPos;
        }

        public SFML.Window.Vector2f GetPosition()
        {
            return position;
        }




        void UpdateSpirteRotation(float newRotation)
        {
            rotation = newRotation;
            spriteBoat.Rotation = newRotation ;
        }




        private void LoadGraphics()
        {
            try
            {
                textureBoat = new Texture("../gfx/boat.png");
                spriteBoat = new Sprite(textureBoat);
                spriteBoat.Origin = new Vector2f(textureBoat.Size.X / 2.0f, 2.0f * textureBoat.Size.Y / 3.0f);
            }
            catch (SFML.LoadingFailedException ex)
            {
                System.Console.Out.WriteLine("error loading the boat graphics");
                System.Console.Out.WriteLine(ex.Message);
            }
        }



        /// <summary>
        /// This Method returns true if the boat has free capacity to take up some passengers
        /// </summary>
        /// <returns></returns>
        public bool HasFreeSpace()
        {
            bool freespace = true;

            if (this.NumberOfPersonsCarried >= GameSettings.BoatMaximumNumberOfPersonsToCarry())
            {
                freespace = false;
            }

            return freespace;
        }


        internal void PickUpPerson(Person p)
        {
            NumberOfPersonsCarried += 1;
           // System.Console.Out.WriteLine("Picking Up Person. Now Carryin " + NumberOfPersonsCarried);
        }

        public void ReliefPersons ( )
        {
            NumberOfPersonsCarried = 0;

        }

        public void ResetBoatPosition()
        {
            SetPosition(GameSettings.GetBoatStartingPosition());
            velocity = new SFML.Window.Vector2f(0, 0);
            acceleration = new SFML.Window.Vector2f(0, 0);
            angularVelocity = 0;
            rotation = 90;
        }

        public void RefuelBoat()
        {
            FuelPercentage = 100.0f;
        }


    }
}
