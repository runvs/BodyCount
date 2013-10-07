using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

namespace Empty_CSharp_Application
{
    class Island
    {
        public Island()
        {
            LoadGraphics();
        }

        public void Update(float deltaT)
        {

        }

        public void Draw(SFML.Graphics.RenderWindow rw, SFML.Window.Vector2f camPosition)
        {

            spriteIsland.Position -= camPosition;
            rw.Draw(spriteIsland);
            spriteIsland.Position += camPosition;

        }
            private SFML.Graphics.Texture textureIsland;
            private SFML.Graphics.Sprite spriteIsland;

        private void LoadGraphics()
        {
            try
            {
                textureIsland = new Texture("../gfx/island.png");
                spriteIsland = new Sprite(textureIsland);
                //spritePerson.Origin = new Vector2f(texturePerson.Size.X / 2.0f, texturePerson.Size.Y / 2.0f);
            }
            catch (SFML.LoadingFailedException ex)
            {
                System.Console.Out.WriteLine("error loading the island graphics");
                System.Console.Out.WriteLine(ex.Message);
            }
        }
    }
}
