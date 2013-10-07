using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using SFML.Graphics;

namespace Empty_CSharp_Application
{
    class World
    {

        public World()
        {
            LoadGraphics();
            LoadSounds();

            StartNewGame();
        }



        private System.Random myRandomGenerator = new Random();

        private Boat playerBoat = new Boat();

        private System.Collections.IList personList = new System.Collections.Generic.List<Person>();

        private Island lampedusa = new Island();

        public bool HasQuitGame { get; set; }


        private SFML.Window.Vector2f CamPosition { get; set; }
        private int personsRescued = 0;

        private int personsDEAD = 0;

        private float timeTilNextPersonDeat = 3.0f;

        private float timeTilNextPersonSpawns = 12.0f;

        private SFML.Graphics.Texture textureWaterTile;
        private SFML.Graphics.Sprite spriteWaterTile;

        private SFML.Graphics.Texture textureBar;
        private SFML.Graphics.Sprite spriteBar;

        private SFML.Graphics.Shape fuelShape;


        private SFML.Graphics.Text personsRescuedText;
        private SFML.Graphics.Text personsDeadText;
        private SFML.Graphics.Text fuelText;

        private SFML.Graphics.Texture texturePlaceholderEmpty;
        private SFML.Graphics.Texture texturePlaceholderFull;

        private SFML.Graphics.Sprite spritePlaceholderEmpty;
        private SFML.Graphics.Sprite spritePlaceholderFull;

        private SFML.Audio.SoundBuffer soundBufferPickUp;
        private SFML.Audio.Sound soundPickUp;


        private SFML.Audio.SoundBuffer soundBufferRelief;
        private SFML.Audio.Sound soundRelief;

        private SFML.Graphics.Texture textureCloud;
        System.Collections.Generic.IList<SFML.Graphics.Sprite> listClouds = new System.Collections.Generic.List<SFML.Graphics.Sprite>();

        private float arrowDisplayTime = -7.0f;
        private bool firstPickUp = true;
        private SFML.Graphics.Texture textureArrow;
        private SFML.Graphics.Sprite spriteArrow;
        private Text textBringThemHere;


        private void ResetCameraPosition()
        {
            CamPosition = new Vector2f(0, 0);
        }


        public void GetInput()
        {
            playerBoat.GetInput();
        }

        public void Update(float deltaT)
        {
            DoScrolling();

            UpdateClouds(deltaT);


            playerBoat.Update(deltaT);
            foreach (Person p in personList)
            {
                if (!p.RemovePersonFromList)
                {
                    DoColissionHandlingPersons(p);
                    p.Update(deltaT);
                }
            }
            DoColissionHandlingIsland();
            Person.BoatPosition = playerBoat.GetPosition();

            UpdateDeathCount(deltaT);

            UpdateMigrantSpawner(deltaT);

            UpdateHud();

            if (personsDEAD >= 100)
            {
                QuitGame();
            }

            CheckPlayerIsOnMap();



            if (playerBoat.FuelPercentage <= 0.0f)
            {
                playerBoat.ReliefPersons();
                playerBoat.ResetBoatPosition();
                ResetCameraPosition();
                playerBoat.RefuelBoat();
            }

            if (arrowDisplayTime >= 0)
            {
                arrowDisplayTime -= deltaT;
            }
        }

        private void UpdateClouds(float deltaT)
        {
            foreach (SFML.Graphics.Sprite spr in listClouds)
            {
                spr.Position += GameSettings.GetCloudMovement() * deltaT;

                SFML.Window.Vector2f newPosition = spr.Position;

                if (newPosition.X <= -500)
                {
                    newPosition.X = GameSettings.GetWorldSize().X + 900;
                }
                if (newPosition.Y <= -500)
                {
                    newPosition.Y = GameSettings.GetWorldSize().Y + 700;
                }

                spr.Position = newPosition;


            }
        }

        private void CheckPlayerIsOnMap()
        {
            if (playerBoat.GetPosition().X <= 20 || playerBoat.GetPosition().X >= GameSettings.GetWorldSize().X + 800 - 20)
            {

                playerBoat.ReliefPersons(); // but dont add them to the rescued Persons counter
                playerBoat.ResetBoatPosition();
                ResetCameraPosition();
            }

            if (playerBoat.GetPosition().Y <= 20 || playerBoat.GetPosition().Y >= GameSettings.GetWorldSize().Y + 600 - 20)
            {
                playerBoat.ReliefPersons();
                playerBoat.ResetBoatPosition();
                ResetCameraPosition();
            }
        }


        public void Draw(SFML.Graphics.RenderWindow rw)
        {
            DrawWater(rw);

            DrawPersons(rw);

            lampedusa.Draw(rw, CamPosition);
            playerBoat.Draw(rw, CamPosition);

            DrawClouds(rw, CamPosition);


            if (arrowDisplayTime >= 0.0f)
            {
                rw.Draw(textBringThemHere);
                rw.Draw(spriteArrow);
            }

            DrawHUD(rw);
        }

        private void DrawClouds(RenderWindow rw, Vector2f CamPosition)
        {
            foreach (SFML.Graphics.Sprite spr in listClouds)
            {
                SFML.Graphics.Color oldColor = spr.Color;

                spr.Color = new Color(0, 0, 0, 50);
                spr.Position -= 1.0f * CamPosition;
                rw.Draw(spr);
                spr.Position += 1.0f * CamPosition;

                spr.Color = oldColor;
                CamPosition.X += 50;
                CamPosition.Y += 30;
                spr.Position -= 1.8f * CamPosition;
                rw.Draw(spr);
                spr.Position += 1.8f * CamPosition;
                CamPosition.X -= 50;
                CamPosition.Y -= 30;
            }
        }



        public void QuitGame()
        {
            LastRescuedPersons = personsRescued;
            HasQuitGame = true;
        }

        private void UpdateDeathCount(float deltaT)
        {
            timeTilNextPersonDeat -= deltaT;

            if (timeTilNextPersonDeat <= 0.0f)
            {
                ResetDeathTimer();
                personsDEAD += 1;
            }
        }

        private void UpdateMigrantSpawner(float deltaT)
        {
            timeTilNextPersonSpawns -= deltaT;
            if (timeTilNextPersonSpawns <= 0.0f)
            {
                ResetSpawnTimer();
                CreateOnePerson();
            }
        }

        private void ResetSpawnTimer()
        {
            // 8 to 13s
            timeTilNextPersonSpawns = (float)(8 + 5.0 * myRandomGenerator.NextDouble());
        }

        private void ResetDeathTimer()
        {
            //  1. to 1.75s
            timeTilNextPersonDeat = (float)(1.0 + 0.75 * myRandomGenerator.NextDouble());
        }

        public int LastRescuedPersons { get; set; }

        private void DoScrolling()
        {
            if (playerBoat.GetPosition().X - CamPosition.X <= GameSettings.ScrollMargin() && CamPosition.X >= 0)
            {
                SFML.Window.Vector2f newPos = CamPosition;
                newPos.X -= 1;
                if (playerBoat.GetPosition().X - CamPosition.X <= GameSettings.FasterScrollMargin())
                {
                    newPos.X -= 1;
                }
                CamPosition = newPos;
            }
            else if (playerBoat.GetPosition().X - CamPosition.X >= 800 - GameSettings.ScrollMargin() && CamPosition.X <= GameSettings.GetWorldSize().X)
            {
                SFML.Window.Vector2f newPos = CamPosition;
                newPos.X += 1;
                if (playerBoat.GetPosition().X - CamPosition.X >= 800 - GameSettings.FasterScrollMargin())
                {
                    newPos.X += 1;
                }
                CamPosition = newPos;
            }

            if (playerBoat.GetPosition().Y - CamPosition.Y <= GameSettings.ScrollMargin() && CamPosition.Y >= 0)
            {
                SFML.Window.Vector2f newPos = CamPosition;
                newPos.Y -= 1;
                if (playerBoat.GetPosition().Y - CamPosition.Y <= GameSettings.FasterScrollMargin())
                {
                    newPos.Y -= 1;
                }
                CamPosition = newPos;
            }
            else if (playerBoat.GetPosition().Y - CamPosition.Y >= 600 - GameSettings.ScrollMargin() && CamPosition.Y <= GameSettings.GetWorldSize().Y)
            {
                SFML.Window.Vector2f newPos = CamPosition;
                newPos.Y += 1;
                if (playerBoat.GetPosition().Y - CamPosition.Y >= 600 - GameSettings.ScrollMargin())
                {
                    newPos.Y += 1;
                }
                CamPosition = newPos;
            }

        }

        private void UpdateHud()
        {
            personsRescuedText.DisplayedString = "Persons Rescued: " + this.personsRescued;
            personsDeadText.DisplayedString = "Body Count: " + this.personsDEAD;


            float scaleFactor = playerBoat.FuelPercentage / 100.0f;
            if (scaleFactor < 0.0f)
            {
                scaleFactor = 0.0f;
            }

            fuelShape.Scale = new SFML.Window.Vector2f(scaleFactor, 1);
        }


        private void DrawWater(RenderWindow rw)
        {
            float tileOffsetX = (float)(CamPosition.X) % 20.0f;
            float tileOffsetY = (float)(CamPosition.Y) % 20.0f;

            for (int i = -2; i != 42; ++i)
            {
                for (int j = -2; j != 62; ++j)
                {
                    spriteWaterTile.Position = new SFML.Window.Vector2f(20 * i, 20 * j);
                    spriteWaterTile.Position -= new Vector2f(tileOffsetX, tileOffsetY);
                    rw.Draw(spriteWaterTile);
                }
            }
        }




        private void DrawPersons(SFML.Graphics.RenderWindow rw)
        {
            foreach (Person p in personList)
            {
                if (!p.RemovePersonFromList)
                {
                    p.Draw(rw, CamPosition);
                }
            }
        }



        private void DrawHUD(SFML.Graphics.RenderWindow rw)
        {
            rw.Draw(personsRescuedText);
            rw.Draw(personsDeadText);
            rw.Draw(fuelText);


            /// Fuel 
            rw.Draw(fuelShape);
            spriteBar.Position = new Vector2f(20, 530);
            rw.Draw(spriteBar);


            // boat Capacity

            spritePlaceholderEmpty.Position = new Vector2f(610, 520);
            rw.Draw(spritePlaceholderEmpty);
            spritePlaceholderEmpty.Position = new Vector2f(670, 520);
            rw.Draw(spritePlaceholderEmpty);
            spritePlaceholderEmpty.Position = new Vector2f(730, 520);
            rw.Draw(spritePlaceholderEmpty);

            //System.Console.Out.WriteLine("Number Of Persons: " + playerBoat.NumberOfPersonsCarried );
            if (playerBoat.NumberOfPersonsCarried >= 1)
            {
                spritePlaceholderFull.Position = new Vector2f(610, 520);
                rw.Draw(spritePlaceholderFull);
            }

            if (playerBoat.NumberOfPersonsCarried >= 2)
            {
                spritePlaceholderFull.Position = new Vector2f(670, 520);
                rw.Draw(spritePlaceholderFull);
            }

            if (playerBoat.NumberOfPersonsCarried >= 3)
            {
                spritePlaceholderFull.Position = new Vector2f(730, 520);
                rw.Draw(spritePlaceholderFull);
            }



        }


        // this will be HORROR
        private void DoColissionHandlingPersons(Person p)
        {
            SFML.Window.Vector2f boatPosition = playerBoat.GetPosition();

            SFML.Window.Vector2f personPosition = p.Position;

            float boatRadius = 30;
            float personRadius = 14;

            float radiusSumSquared = (boatRadius + personRadius) * (boatRadius + personRadius);


            float deltaX = personPosition.X - boatPosition.X;
            float deltaY = personPosition.Y - boatPosition.Y;

            float distanceSquared = (deltaX * deltaX) + (deltaY * deltaY);

            if (distanceSquared <= radiusSumSquared)
            {
                if (playerBoat.GetAbsoluteVelocitySquared() >= GameSettings.BoatPersonCrushingVelocitySquared())
                {
                    System.Console.Out.WriteLine("Crushing Person with Speed^2: " + playerBoat.GetAbsoluteVelocitySquared());
                    KillPerson(p);
                }
                else
                {
                    //System.Console.Out.WriteLine("Collecting Person");
                    CollectPerson(p);
                    if (firstPickUp)
                    {
                        firstPickUp = false;
                        arrowDisplayTime *= -1;
                    }
                }

            }

        }



        /// <summary>
        /// If You Hit the Island, you relief the persons you are carriyng, you start at the old position and velocity and you get a free load of fuel
        /// </summary>
        private void DoColissionHandlingIsland()
        {
            if ((playerBoat.GetPosition().X <= 160 && playerBoat.GetPosition().Y <= 280) ||
                playerBoat.GetPosition().X <= 245 && playerBoat.GetPosition().Y <= 180)
            {
                soundRelief.Play();
                personsRescued += playerBoat.NumberOfPersonsCarried;
                playerBoat.ReliefPersons();
                playerBoat.ResetBoatPosition();
                playerBoat.RefuelBoat();

                arrowDisplayTime = -5.0f;
            }
        }




        private void CollectPerson(Person p)
        {
            if (playerBoat.HasFreeSpace())
            {
                soundPickUp.Play();
                playerBoat.PickUpPerson(p);
                p.RemovePersonFromList = true;

            }
        }

        private void KillPerson(Person p)
        {
            p.RemovePersonFromList = true;
            personsDEAD += 1;
        }


        private SFML.Window.Vector2f GetNewPersonPosition()
        {
            return new Vector2f(myRandomGenerator.Next(100, (int)(GameSettings.GetWorldSize().X + 800 - 100)), myRandomGenerator.Next(100, (int)(GameSettings.GetWorldSize().Y + 600 - 100)));
        }


        private void CreateOnePerson()
        {
            SFML.Window.Vector2f personPosition = GetNewPersonPosition();

            while (!CheckPersonPosition(personPosition))
            {
                personPosition = GetNewPersonPosition();
            }

            //System.Console.Out.WriteLine("Creating Person at: " + personPosition);

            Person myIncrediblePerson = new Person();
            myIncrediblePerson.Position = personPosition;
            personList.Add(myIncrediblePerson);
        }

        private bool CheckPersonPosition(SFML.Window.Vector2f personPosition)
        {
            bool ret = true;

            if (personPosition.X <= 300 && personPosition.Y <= 400)
            {
                ret = false;
            }
            return ret;
        }

        private void LoadGraphics()
        {
            try
            {
                personsRescuedText = new Text("", GameSettings.GameFont());
                personsRescuedText.Position = new Vector2f(550, 10);
                personsRescuedText.Scale = new Vector2f(0.75f, 0.75f);

                personsDeadText = new Text("", GameSettings.GameFont());
                personsDeadText.Position = new Vector2f(550, 30);
                personsDeadText.Scale = new Vector2f(0.75f, 0.75f);

                fuelText = new Text("Fuel", GameSettings.GameFont());
                fuelText.Position = new Vector2f(24, 500);
                fuelText.Scale = new Vector2f(0.75f, 0.75f);



                textureWaterTile = new Texture("../gfx/watertile.png");
                spriteWaterTile = new Sprite(textureWaterTile);

                textureBar = new Texture("../gfx/Bar.png");
                spriteBar = new Sprite(textureBar);

                fuelShape = new SFML.Graphics.RectangleShape(new SFML.Window.Vector2f(194, 44));
                fuelShape.Position = new Vector2f(24, 558);
                fuelShape.FillColor = new Color(180, 40, 20);
                fuelShape.Origin = new Vector2f(0, 24);

                texturePlaceholderEmpty = new Texture("../gfx/placeholder.png");
                texturePlaceholderFull = new Texture("../gfx/placeholder_full.png");

                spritePlaceholderEmpty = new Sprite(texturePlaceholderEmpty);
                spritePlaceholderFull = new Sprite(texturePlaceholderFull);

                textureCloud = new Texture("../gfx/cloud.png");
                CreateClouds();



                textureArrow = new Texture("../gfx/arrow.png");
                spriteArrow = new Sprite(textureArrow);
                spriteArrow.Position = new Vector2f(280, 180);

                textBringThemHere = new Text("Bring ihn \n nach Lampedusa!", GameSettings.GameFont());
                textBringThemHere.Position = new Vector2f(280, 120);


            }
            catch (SFML.LoadingFailedException ex)
            {
                System.Console.Out.WriteLine("error loading the boat graphics");
                System.Console.Out.WriteLine(ex.Message);
            }
        }

        private void CreateClouds()
        {
            SFML.Graphics.Sprite cloudSprite;
            for (int i = 0; i != 15; ++i)
            {
                cloudSprite = new Sprite(textureCloud);
                cloudSprite.Position = new SFML.Window.Vector2f(myRandomGenerator.Next(-20, (int)(GameSettings.GetWorldSize().X) + 800), myRandomGenerator.Next(-20, (int)(GameSettings.GetWorldSize().Y) + 600));
                float scaleFactor = (float)(1.75 + 3.0 * myRandomGenerator.NextDouble());
                cloudSprite.Scale = new SFML.Window.Vector2f(scaleFactor, scaleFactor);
                SFML.Graphics.Color newColor = cloudSprite.Color;
                newColor.A = 45;
                cloudSprite.Color = newColor;
                listClouds.Add(cloudSprite);
            }
        }




        private void LoadSounds()
        {
            try
            {
                soundBufferPickUp = new SFML.Audio.SoundBuffer("../sfx/pickup.wav");
                soundPickUp = new SFML.Audio.Sound(soundBufferPickUp);
                soundPickUp.Volume = 50.0f;

                soundBufferRelief = new SFML.Audio.SoundBuffer("../sfx/relief.wav");
                soundRelief = new SFML.Audio.Sound(soundBufferRelief);
                soundRelief.Volume = 20.0f;

            }
            catch (SFML.LoadingFailedException ex)
            {
                System.Console.Out.WriteLine("error loading the world sounds");
                System.Console.Out.WriteLine(ex.Message);
            }
        }


        internal void StartNewGame()
        {
            CamPosition = new SFML.Window.Vector2f(0, 0);
            HasQuitGame = false;

            personList.Clear();
            for (int i = 0; i != 4; ++i)
            {
                CreateOnePerson();
            }

            ResetDeathTimer();

            personsRescued = 0;
            personsDEAD = 0;

            playerBoat.ReliefPersons();
            playerBoat.ResetBoatPosition();
            playerBoat.RefuelBoat();
        }


    }
}
