using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;

namespace Empty_CSharp_Application
{
    class Program
    {

        static World myGameWorld;

        static SFML.Audio.Music backgroundMusic;
        
        private static float highScoretimer = 0.0f;

        static bool hasPlayedIntro = false;

        static float IntroTimer = 0.0f;
        static bool isWatchingCredits = false;

        static SFML.Graphics.Texture textureIntroImage;
        static SFML.Graphics.Sprite spriteeIntroImage;


        static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        static SFML.Graphics.RenderWindow rw;

        static void HandleInpt()
        {
            // not the esc key
            if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {
                if (!hasPlayedIntro)
                {
                    hasPlayedIntro = true;
                }
                else
                {

                    if (!myGameWorld.HasQuitGame)
                    {
                        myGameWorld.QuitGame();
                    }
                    else
                    {
                        if (highScoretimer >= 0.45f)
                        {
                            if (isWatchingCredits)
                            {
                                isWatchingCredits = false;
                                highScoretimer = 0.0f;
                            }
                            else
                            {
                                highScoretimer = 0.0f;
                                rw.Close();
                            }
                            
                        }
                    }
                }
            }
            else if (AnyKeyPressed())
            {
                
                if (!hasPlayedIntro)
                {
                    if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Return))
                    {
                        hasPlayedIntro = true;
                    }
                }
                else
                {
                    if (myGameWorld.HasQuitGame)
                    {
                        if (highScoretimer >= 0.5f)
                        {
                            if (!isWatchingCredits)
                            {
                                if (Keyboard.IsKeyPressed(Keyboard.Key.Space) || Keyboard.IsKeyPressed(Keyboard.Key.Return))
                                {
                                    myGameWorld.StartNewGame();
                                    highScoretimer = 0.0f;
                                }
                                else if (Keyboard.IsKeyPressed(Keyboard.Key.C))
                                {
                                    isWatchingCredits = true;
                                }
                            }
                            else
                            {
                                // we are watching Credits
                            }
                        }
                    }
                }
            }
        }

        private static bool AnyKeyPressed()
        {
            bool ret = false;
            if (
                SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.C) ||
                SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.A) ||
                SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.D) ||
                SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.W) ||
                SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Return) ||
                SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Space) ||
                SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Up) ||
                SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Left) ||
                SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Right)          )
            {
                ret = true;
            }

            return ret;
        }

        



        static void Main(string[] args)
        {

            rw = new SFML.Graphics.RenderWindow(new SFML.Window.VideoMode(800, 600), "Body Count");
            rw.SetFramerateLimit(60);

            myGameWorld = new World();

            backgroundMusic = new Music("../sfx/lampedusa_music.wav");
            backgroundMusic.Volume = 10.5f;
            backgroundMusic.Loop = true;

            textureIntroImage = new Texture("../gfx/intro_ship.png");
            spriteeIntroImage = new Sprite(textureIntroImage);
            spriteeIntroImage.Position = new Vector2f(500, 100);
            spriteeIntroImage.Scale = new Vector2f(2.25f, 2.25f);

            rw.SetActive();
            rw.Closed += OnClose;
            rw.SetActive();
            backgroundMusic.Play();

            int startTime = Environment.TickCount;
            int endTime = startTime;
            float time = 16.7f;

            SFML.Graphics.Text framerateText = new Text("", GameSettings.GameFont());
            framerateText.Position = new Vector2f(5, 5);
            framerateText.Scale = new Vector2f(0.5f, 0.5f);

            while (rw.IsOpen())
            {
                if (startTime != endTime)
                {
                    time = (float)(endTime - startTime) / 1000.0f;
                    framerateText.DisplayedString = "" + 1.0/time;
                }
                startTime = Environment.TickCount;
                rw.DispatchEvents();

                HandleInpt();

                myGameWorld.GetInput();

                rw.Clear(new SFML.Graphics.Color(GameSettings.GetBackgroundColor()));


                if (!hasPlayedIntro)
                {
                    ShowIntro(rw);
                    IntroTimer += 16.7f / 1000.0f;
                }
                else
                {

                    if (!myGameWorld.HasQuitGame)
                    {
                        myGameWorld.Update(time);
                        myGameWorld.Draw(rw);
                    }
                    else
                    {
                        highScoretimer += time;
                        if (!isWatchingCredits)
                        {
                            ShowHighScores(rw);
                        }
                        else
                        {
                            ShowCredits(rw);
                        }
                    }

                }

                //rw.Draw(framerateText);
                rw.Display();

                endTime = Environment.TickCount;
            }
        }

       

        private static void ShowIntro(RenderWindow rw)
        {

            rw.Clear(new SFML.Graphics.Color(0,0,0));

            SFML.Graphics.Text GameTitleText = new Text("Body Count", GameSettings.GameFont());
            GameTitleText.Scale = new Vector2f(1.25f, 1.25f);
            GameTitleText.Position = new SFML.Window.Vector2f(400 - (float)(GameTitleText.GetGlobalBounds().Width/2.0), 10);
            rw.Draw(GameTitleText);

            SFML.Graphics.Text IntroText = new Text("Lampedusa", GameSettings.GameFont());
            IntroText.Scale = new Vector2f(1, 1);
            IntroText.Position = new SFML.Window.Vector2f(20, 50);
            rw.Draw(IntroText);

            IntroText.DisplayedString = "Vor der italienischen Küste ereignete sich erneut ein Flüchtlingsdrama.";
            IntroText.Scale = new Vector2f(0.75f, 0.75f);
            IntroText.Position = new SFML.Window.Vector2f(20, 85);
            rw.Draw(IntroText);

            IntroText.DisplayedString = "100 Migranten kamen dabei ums Leben.";
            IntroText.Scale = new Vector2f(0.75f, 0.75f);
            IntroText.Position = new SFML.Window.Vector2f(20, 110);
            rw.Draw(IntroText);

            IntroText.DisplayedString = "Nur mit Hilfe der Fischer konnten einige Überlebende gerettet werden.";
            IntroText.Scale = new Vector2f(0.75f, 0.75f);
            IntroText.Position = new SFML.Window.Vector2f(20, 135);
            rw.Draw(IntroText);

            rw.Draw(spriteeIntroImage);


            if (IntroTimer >= 4.0f)        // Stage 1
            {

                SFML.Graphics.Text IntroInfoText = new Text("Rette mit deinem kleinen Boot so viele ", GameSettings.GameFont());
                IntroInfoText.Scale = new Vector2f(0.75f, 0.75f);
                IntroInfoText.Position = new SFML.Window.Vector2f(20, 190);
                rw.Draw(IntroInfoText);

                IntroInfoText.DisplayedString = "wie möglich und bringe sie sicher auf die Insel.";
                IntroInfoText.Scale = new Vector2f(0.75f, 0.75f);
                IntroInfoText.Position = new SFML.Window.Vector2f(20, 215);
                rw.Draw(IntroInfoText);


                IntroInfoText.DisplayedString = "Achte dabei auf deinen Treibstoff.";
                IntroInfoText.Scale = new Vector2f(0.75f, 0.75f);
                IntroInfoText.Position = new SFML.Window.Vector2f(20, 240);
                rw.Draw(IntroInfoText);
            }

            if (IntroTimer >= 8.0f)        // Stage 2
            {
                SFML.Graphics.Text IntroControlText = new Text("Steuerug", GameSettings.GameFont());
                IntroControlText.Scale = new Vector2f(1, 1);
                IntroControlText.Position = new SFML.Window.Vector2f(400 - IntroControlText.GetGlobalBounds().Width /2.0f, 400);
                rw.Draw(IntroControlText);

                IntroControlText.DisplayedString = "[A], [<-] \t \t Nach links";
                IntroControlText.Scale = new Vector2f(0.75f, 0.75f);
                IntroControlText.Position = new SFML.Window.Vector2f(400 - IntroControlText.GetGlobalBounds().Width / 2.0f, 435);
                rw.Draw(IntroControlText);

                IntroControlText.DisplayedString = "[D], [->] \t \t Nach rechts";
                IntroControlText.Scale = new Vector2f(0.75f, 0.75f);
                IntroControlText.Position = new SFML.Window.Vector2f(400 - IntroControlText.GetGlobalBounds().Width / 2.0f, 460);
                rw.Draw(IntroControlText);

                IntroControlText.DisplayedString = "[W], [^] \t \t Beschleunigen";
                IntroControlText.Scale = new Vector2f(0.75f, 0.75f);
                IntroControlText.Position = new SFML.Window.Vector2f(400 - IntroControlText.GetGlobalBounds().Width / 2.0f, 485);
                rw.Draw(IntroControlText);

                IntroControlText.DisplayedString = "[Enter]\t \t Start";
                IntroControlText.Scale = new Vector2f(0.75f, 0.75f);
                IntroControlText.Position = new SFML.Window.Vector2f(400 - IntroControlText.GetGlobalBounds().Width / 2.0f, 510);
                rw.Draw(IntroControlText);

            }
        }

        private static void ShowHighScores(RenderWindow rw)
        {
            SFML.Graphics.Text HighScoreText = new Text("You rescued", GameSettings.GameFont());
            HighScoreText.Scale = new Vector2f(0.75f, 0.75f);
            HighScoreText.Position = new SFML.Window.Vector2f(400 - HighScoreText.GetGlobalBounds().Width/2.0f, 150);
            rw.Draw(HighScoreText);

            HighScoreText.DisplayedString = " " + myGameWorld.LastRescuedPersons;
            HighScoreText.Scale = new Vector2f(1, 1);
            HighScoreText.Position = new SFML.Window.Vector2f(400 - HighScoreText.GetGlobalBounds().Width / 2.0f, 200);
            rw.Draw(HighScoreText);

            HighScoreText.DisplayedString = "Migrants";
            HighScoreText.Scale = new Vector2f(1, 1);
            HighScoreText.Position = new SFML.Window.Vector2f(400 - HighScoreText.GetGlobalBounds().Width / 2.0f, 240);
            rw.Draw(HighScoreText);

            HighScoreText.DisplayedString = "[C]redits";
            HighScoreText.Scale = new Vector2f(0.75f, 0.75f);
            HighScoreText.Position = new SFML.Window.Vector2f(10 , 600 - 2 * HighScoreText.GetGlobalBounds().Height);
            rw.Draw(HighScoreText);

        }

        private static void ShowCredits(RenderWindow rw)
        {
            SFML.Graphics.Text CreditsText = new Text("Body Count", GameSettings.GameFont());
            CreditsText.Scale = new Vector2f(1.5f, 1.5f);
            CreditsText.Position = new SFML.Window.Vector2f(400 - (float)(CreditsText.GetGlobalBounds().Width / 2.0), 20);
            rw.Draw(CreditsText);

            CreditsText = new Text("Ein Spiel von ", GameSettings.GameFont());
            CreditsText.Scale = new Vector2f(0.75f, 0.75f);
            CreditsText.Position = new SFML.Window.Vector2f(400 - (float)(CreditsText.GetGlobalBounds().Width / 2.0), 100);
            rw.Draw(CreditsText);

            CreditsText = new Text("Simon Weis", GameSettings.GameFont());
            CreditsText.Scale = new Vector2f(1, 1);
            CreditsText.Position = new SFML.Window.Vector2f(400 - (float)(CreditsText.GetGlobalBounds().Width / 2.0), 135);
            rw.Draw(CreditsText);

            CreditsText = new Text("Visual Studio 2010 \t C# \t SFML.Net 2.1 ", GameSettings.GameFont());
            CreditsText.Scale = new Vector2f(0.75f, 0.75f);
            CreditsText.Position = new SFML.Window.Vector2f(400 - (float)(CreditsText.GetGlobalBounds().Width / 2.0), 170);
            rw.Draw(CreditsText);

            CreditsText = new Text("aseprite \t LMMS \t ChronoLapse", GameSettings.GameFont());
            CreditsText.Scale = new Vector2f(0.75f, 0.75f);
            CreditsText.Position = new SFML.Window.Vector2f(400 - (float)(CreditsText.GetGlobalBounds().Width / 2.0), 200);
            rw.Draw(CreditsText);

            CreditsText = new Text("Gedenkt der Opfer der Katastrophe!", GameSettings.GameFont());
            CreditsText.Scale = new Vector2f(0.75f, 0.75f);
            CreditsText.Position = new SFML.Window.Vector2f(400 - (float)(CreditsText.GetGlobalBounds().Width / 2.0), 270);
            rw.Draw(CreditsText);

            CreditsText = new Text("Ich danke meiner Familie & allen lieben Menschen", GameSettings.GameFont());
            CreditsText.Scale = new Vector2f(0.75f, 0.75f);
            CreditsText.Position = new SFML.Window.Vector2f(400 - (float)(CreditsText.GetGlobalBounds().Width / 2.0), 350);
            rw.Draw(CreditsText);

            CreditsText = new Text("die ihren Teil beigetragen haben.", GameSettings.GameFont());
            CreditsText.Scale = new Vector2f(0.75f, 0.75f);
            CreditsText.Position = new SFML.Window.Vector2f(400 - (float)(CreditsText.GetGlobalBounds().Width / 2.0), 375);
            rw.Draw(CreditsText);

            CreditsText = new Text("Ein Beitrag für die 5. Devmania, 6. Oktober 2013", GameSettings.GameFont());
            CreditsText.Scale = new Vector2f(0.75f, 0.75f);
            CreditsText.Position = new SFML.Window.Vector2f(400 - (float)(CreditsText.GetGlobalBounds().Width / 2.0), 500);
            rw.Draw(CreditsText);
        }
    }

}
