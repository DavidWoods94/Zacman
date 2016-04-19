//************************************************
//
// (c) Copyright 2015 Dr. Thomas Fernandez
//
// All rights reserved.
//
//************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace WPFStartupDemo
{

    // https://evanl.wordpress.com/2009/12/06/efficient-optimal-per-frame-eventing-in-wpf/

    // http://stackoverflow.com/questions/5812384/why-is-frame-rate-in-wpf-irregular-and-not-limited-to-monitor-refresh
    // http://stackoverflow.com/questions/7733990/constant-framerate-in-wpf-for-game
    internal static class CompositionTargetEx 
    { 
        internal static TimeSpan _last = TimeSpan.Zero; 
        private static event EventHandler<RenderingEventArgs> _FrameUpdating; 
        internal static event EventHandler<RenderingEventArgs> FrameUpdating 
        { 
            add 
            { 
                if (_FrameUpdating == null)
                    CompositionTarget.Rendering += CompositionTarget_Rendering; _FrameUpdating += value; 
            } 
            remove 
            { 
                _FrameUpdating -= value; 
                if (_FrameUpdating == null)
                    CompositionTarget.Rendering -= CompositionTarget_Rendering; 
            } 
        } 
        static void CompositionTarget_Rendering(object sender, EventArgs e) 
        { 
            RenderingEventArgs args = (RenderingEventArgs)e; 
            if (args.RenderingTime == _last) return; 
            _last = args.RenderingTime; 
            _FrameUpdating(sender, args);
        } 
    }


    internal static class G
    {
        
        static List<GameObject> displayList = new List<GameObject>();
        
        static internal bool[] keyPressed = new bool[256];
        public static bool gameOver = false;
        static internal double MouseX;
        static internal double MouseY;
        public static double score;
        public static ScoreText scoreText;
        public static HiScoreText hiScoreText;
        static internal bool MouseDown;


        public static void RestartGame()
        {
            scoreAdded = false;
            SetUpAllGameObjects();
            gameOver = false;
        }
        static internal void startup()
        {
            // Alternative (better) way to update everything between frame renders
            // https://evanl.wordpress.com/2009/12/06/efficient-optimal-per-frame-eventing-in-wpf/
            // http://stackoverflow.com/questions/5812384/why-is-frame-rate-in-wpf-irregular-and-not-limited-to-monitor-refresh
            // http://stackoverflow.com/questions/7733990/constant-framerate-in-wpf-for-game

            CompositionTargetEx.FrameUpdating += CompositionTargetEx_FrameUpdating;

            G.SetUpAllGameObjects(true);
        }

        static void CompositionTargetEx_FrameUpdating(object sender, RenderingEventArgs e)
        {
            UpdateGame();
        }

        static bool started = false;
        static int spawnRateF = 75;
        static int spawnRateE = 200;
        static private void UpdateGame()
        {
            if (G.isKeyPressed(Key.Enter))
            { 
                started = true;
            }
            if(!started)
            {
                 hiScoreText.Show();
            }
            else if (started)
            {
                hiScoreText.makeInactive();
                gameOverT.makeInactive();
                if (!gameOver)
                {
                    if(Player.numCollected > 15)
                    {
                        spawnRateF = 30;
                        spawnRateE = 100;
                    }
                    if(Player.numCollected > 40)
                    {
                        spawnRateF = 10;
                        spawnRateE = 50;
                    }
                    if (G.isKeyPressed(Key.Q)) Quit();

                    if ((G.chance(1.0 / (spawnRateE))) || (G.isKeyPressedOnce(Key.D)))
                    {
                        placeBombs();
                    }
                    if (G.isKeyPressedOnce(Key.D0)) clearFramePercent();

                    foreach (GameObject obj in displayList)
                    {
                        obj.update();
                    }


                    if (G.chance(1.0 / spawnRateF) || (G.isKeyPressedOnce(Key.Z)))
                    {

                        if (Nit.activeNits < numberOfEnemies)
                        {
                            Nit z = Nit.getNextAvailable();
                            if (z != null)
                            {
                                z.SetRandom();

                                if (score > 200)
                                {
                                    numberOfEnemies = 5;
                                }
                                if (score > 1000)
                                {
                                    numberOfEnemies = 10;
                                }
                            }

                        }
                    }
                    
                }

                else if (gameOver)
                {
                    gameOverT.Show();
                    if(!scoreAdded)
                    {
                        scoreAdded = true;
                        G.hiScoreText.addNewScore(G.score);
                    }
                    hiScoreText.Show();
                    if (G.isKeyPressedOnce(Key.Escape))
                    {
                        RestartGame();
                    }
                }
            }
        }
        static bool scoreAdded = false;
        static private void placeBombs()
        {
            
                BigNit z = BigNit.getNextAvailable();
                if (z != null)
                {
                    z.SetRandom();

                    if (score > 200)
                    {
                        numberOfEnemies = 5;
                    }
                    if (score > 1000)
                    {
                        numberOfEnemies = 10;
                    }
                }
            
        }
        static private void clearFramePercent()
        {
            FrameRateText.totalFrameCount = 0.0;
            FrameRateText.badFrameCount = 0.0;
        }

        
        static internal void addToDisplayList(GameObject obj)
        {
            displayList.Add(obj);
        }

        static internal void clearGame()
        {
            displayList.Clear();
            Nit.listNits.Clear();
            Player.numCollected = 0;
            Player.poweredUp = false;
            Player.powerMove = 1;

            PowerUp.powerList.Clear();
            Nit.activeNits = 0;
            BigNit.listBigNits.Clear();
            Melt.list.Clear();
            Explosion.list.Clear();
            G.canvas.Children.Clear();
            G.canvas.Background = Brushes.LightSeaGreen;
            G.score = 0;
            spawnRateF = 75;
            spawnRateE = 200;
        }

        static Background1 background1;
        static int numberOfEnemies = 3;
        public static Player player;
        static internal void SetUpAllGameObjects(bool isFirstTime=false)
        {
 
            clearGame();
            
            //add objects to the game 

            background1 = new Background1();

            player = new Player();
            
            for (int i = 0; i < 15; i++)
            {
                Nit nit = new Nit();
                BigNit big = new BigNit();
                Explosion exp = new Explosion();

            }
            for (int i = 0; i < 30; i++)
            {
                Nit nit = new Nit();
                BigNit big = new BigNit();
            }
            G.scoreText = new ScoreText();
            G.hiScoreText = new HiScoreText();
            G.gameOverT = new GameOverText();
            
            FrameRateText frameRateText = new FrameRateText("", 0.96, 0.998, 30, Colors.Black);
 


        }
        static GameOverText gameOverT = null;

        static private void Quit()
        {
            
            Application.Current.Shutdown();
        }

        internal static double gameWidth
        {
            get
            {
                return canvas.Width;
            }
        }
        internal static double gameHeight
        {
            get
            {
                return canvas.Height;
            }
        }
        static internal bool checkCollision(GameObject obj1, GameObject obj2)
        {
            if (obj2.isActive)
            {
                if (Math.Abs(obj1.X - obj2.X) < ((obj1.ScaledWidth + obj2.ScaledWidth) / 2.0))
                {
                    if (Math.Abs(obj1.Y - obj2.Y) < ((obj1.ScaledHeight + obj2.ScaledHeight) / 2.0))                   
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        internal  static void CheckForAlternateContentDir(string fileName)
        {
            if (!File.Exists(G.ContentDir + fileName))
            {
                G.ContentDir = @"Content\";
            }
        }

        internal static string ContentDir = @"..\..\..\Content\";

        internal static Canvas canvas;

        internal static Random random = new Random();
        internal static Color randColor()
        {
            return Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        }
        
        internal static Color randDarkColor()
        {
            return Color.FromRgb((byte)random.Next(64), (byte)random.Next(64), (byte)random.Next(64));
        }

        internal static Color randLightColor()
        {
            Color c;
            do
            {
                c = randColor();
            } while (!isLight(c));
            return c;
        }

        internal static bool isLight(Color c)
        {
            if (c.R >= 200) return true;
            if (c.G >= 200) return true;
            if (c.B >= 200) return true;
            return false;
        }

        internal static byte randByte()
        {
            return (byte)(random.Next(256));
        }

        internal static double randDRange(double range)
        {
            return randD(-range,range);
        }
        internal static bool chance(double probability)
        {
            return randD() < probability;
        }

        internal static double randD()
        {
            return random.NextDouble();
        }

        internal static double randD(double max)
        {
            return randD() * max;
        }

        internal static double randD(double min,double max)
        {
            return randD(max - min) + min;
        }

        internal static bool isKeyPressed(Key k)
        {
            return G.keyPressed[(int)k];
        }
        internal static bool isKeyPressedOnce(Key k)
        {
            bool result = G.keyPressed[(int)k];
            G.keyPressed[(int)k] = false;
            return result;
        }

        internal static double randAngle()
        {
            return randD(360.0);
        }

        internal static int randI()
        {
            return random.Next();
        }
        internal static int randI(int p1)
        {
            return random.Next(p1);
        }
        internal static int randI(int p1, int p2)
        {
            return random.Next(p1, p2);
        }

        internal static void SetupSound(ref MediaPlayer gameSound, string filename)
        {
            if (gameSound == null)
            {
                gameSound = new MediaPlayer();
                G.CheckForAlternateContentDir(filename);
                Uri uri = new Uri(G.ContentDir + filename, UriKind.Relative);
                gameSound.Open(uri);
            }
        }

        internal static double degreeToRadian(double angle)
        {
            return(angle / 360.0)*(Math.PI * 2.0);
        }

        internal static Color getRandomBlueRedColor()
        {
            int r = G.randI(40,120);
            int g = G.randI(40, 120);
            int b = G.randI(40, 120);
            g = 0;

            return Color.FromRgb((byte)r, (byte)g, (byte)b);

        }
        internal static Color randomizeBlueRedColor(Color color, int randomness)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            r = changeColorElement(randomness, r);
            b = changeColorElement(randomness, b);
            g = changeColorElement(randomness, g);

            g = 0;

            return Color.FromRgb((byte)r, (byte)g, (byte)b);

        }

        private static int changeColorElement(int randomness, int r)
        {
            r += G.randI(-randomness, randomness);
            if (r > 120) r = 120;
            if (r < 40) r = 40;
            return r;
        }

        internal static Color randTransparentColor()
        {
            return Color.FromArgb((byte) 180, (byte)random.Next(150), (byte)random.Next(150), (byte)random.Next(150));
        }
    }
}
