using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

namespace Empty_CSharp_Application
{
    class Person
    {
        public Person()
        {
            LoadGraphics();
            RemovePersonFromList = false;
        }

        public void Update(float deltaT)
        {
            if (!this.RemovePersonFromList)
            {
                actualFrameTime += deltaT;
                if (actualFrameTime >= frameChangeTime)
                {
                    actualFrameTime = 0.0f;
                    actualFrame++;
                    if (actualFrame >= 4)
                    {
                        actualFrame = 1;
                    }
                }

                float deltaX = BoatPosition.X - Position.X;
                float deltaY = BoatPosition.Y - Position.Y;

                float distanceSquared = (deltaX * deltaX) + (deltaY * deltaY);

                if (distanceSquared <= 20000)
                {
                    float distance = (float)(Math.Sqrt(distanceSquared));
                    deltaX /= distance / 0.0010f * deltaT;
                    deltaY /= distance / 0.0010f * deltaT;

                    Position += new Vector2f(deltaX, deltaY);

                }
            }
        }


        public static SFML.Window.Vector2f BoatPosition { get; set; }


        public void Draw(SFML.Graphics.RenderWindow rw, SFML.Window.Vector2f camPosition)
        {
            if (actualFrame == 1)
            {
                spritePerson1.Position -= camPosition;
                rw.Draw(spritePerson1);
                spritePerson1.Position += camPosition;
            }
            else if (actualFrame == 2)
            {
                spritePerson2.Position -= camPosition;
                rw.Draw(spritePerson2);
                spritePerson2.Position += camPosition;
            }
            else if (actualFrame == 3)
            {
                spritePerson3.Position -= camPosition;
                rw.Draw(spritePerson3);
                spritePerson3.Position += camPosition;
            }

            
        }


        private SFML.Window.Vector2f _position = new Vector2f(0,0);

        public SFML.Window.Vector2f Position 
        {
            get{return _position;}
            set { _position = value; spritePerson1.Position = value; spritePerson2.Position = value; spritePerson3.Position = value; }
        }

        private SFML.Graphics.Texture texturePerson1;
        private SFML.Graphics.Texture texturePerson2;
        private SFML.Graphics.Texture texturePerson3;
        private SFML.Graphics.Sprite spritePerson1;
        private SFML.Graphics.Sprite spritePerson2;
        private SFML.Graphics.Sprite spritePerson3;

        private void LoadGraphics()
        {
            try
            {
                texturePerson1 = new Texture("../gfx/person0.png");
                spritePerson1 = new Sprite(texturePerson1);
                spritePerson1.Origin = new Vector2f(texturePerson1.Size.X / 2.0f, texturePerson1.Size.Y / 2.0f);

                texturePerson2 = new Texture("../gfx/person1.png");
                spritePerson2 = new Sprite(texturePerson2);
                spritePerson2.Origin = new Vector2f(texturePerson1.Size.X / 2.0f, texturePerson1.Size.Y / 2.0f);

                texturePerson3 = new Texture("../gfx/person2.png");
                spritePerson3 = new Sprite(texturePerson2);
                spritePerson3.Origin = new Vector2f(texturePerson1.Size.X / 2.0f, texturePerson1.Size.Y / 2.0f);
            }
            catch (SFML.LoadingFailedException ex)
            {
                System.Console.Out.WriteLine("error loading the person graphics");
                System.Console.Out.WriteLine(ex.Message);
            }
        }

        public bool RemovePersonFromList { get; set; }

        int actualFrame = 1;

        float frameChangeTime= 0.25f;
        float actualFrameTime = 0.0f;

    }
}
