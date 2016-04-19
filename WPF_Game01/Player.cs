//************************************************
//
// (c) Copyright 2015 Dr. Thomas Fernandez
//
// All rights reserved.
//
//************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFStartupDemo
{
    class Player : GameObject
    {

        static BitmapImage bMap1 = null;
        static BitmapImage bMap2 = null;
        static BitmapImage bMap3 = null;
        static BitmapImage bMap4 = null;
        static MediaPlayer scream = null;
        static MediaPlayer bigExploSound = null;
        static public MediaPlayer gameOverSound = null;
        static public List<Player> player = new List<Player>();
        static public int numCollected;
        static public int powerMove = 1;
        static public bool poweredUp = false;
        public Player()
        {
            G.SetupSound(ref scream, "death.wav");
            G.SetupSound(ref bigExploSound, "chomp.wav");
            G.SetupSound(ref gameOverSound, "GameOver.wav");

            UseImage("zacmanPowerO.png", ref bMap3);
            UseImage("zacmanPowerC.png", ref bMap4);
            UseImage("zacmanOpen.png", ref bMap1);
            UseImage("zacmanC.png", ref bMap2);
            player.Add(this);

            
            Scale = 0.5;

            SetInMiddle();

            dX = 0.0;
            dY = 0.0;
            tim.Enabled = true;
            tim.Interval = 100;
            tim.Start();
            numCollected = 0;
            AddToGame();


        }
        public void SetInMiddle()
        {
            X = G.gameWidth / 2.0;
            Y = G.gameHeight - (ScaledHeight / 2.0) - 10.0;
        }
        public void Died()
        {
            scream.Stop();
            scream.Play();
            Explosion explo = Explosion.getNextAvailable();

            makeInactive();

            G.gameOver = true;
        }
        Timer tim = new Timer(1000);
        int ot = 25;
        int ct = 0;

        public void OpenClose()
        {
            if (open && ot == 0)
            {
                ct--;
                ((Image)element).Source = bMap2;
                if (ct == 0)
                {
                    open = false;
                    ot = 25;
                }
            }
            else if (ct == 0 && ot != 0)
            {
                ot--;

                ((Image)element).Source = bMap1;
                if (ot == 0)
                {
                    open = true;
                    ct = 25;
                }
            }
        }
        public void PowerOpenClose()
        {
            if(open&&ot==0)
            {
                ct--;
                ((Image)element).Source = bMap4;
                if (ct == 0)
                {
                    open = false;
                    ot = 25;
                }
            }
            else if(ct == 0 && ot != 0)
            {
                ot--;
                
                ((Image)element).Source = bMap3;
                if(ot == 0)
                {
                    open = true;
                    ct = 25;
                }
            }

        }
        double gravity = 1.0;
        double friction = 0.92;
        double acceleration = 0.4;

        int frameCount = 0;
        bool open = true;
        public int sheildFrameCount = 0;
        public bool speedBoost = false;
        int direction = 1;
        int speedTimer = 0;
        int pTime = 500;
        internal override void update()
        {
            if (isActive)
            {
                if(poweredUp)
                {
                    PowerOpenClose();
                }
                else 
                {
                    OpenClose();
                }
                
                if(direction == 1)
                {
                    rotateTransform.Angle = -90;
                    if(speedBoost)
                    {
                        dY = -10;
                    }
                    else
                    dY = -5;
                }
                else if(direction == 2)
                {
                    rotateTransform.Angle = 90;
                    if (speedBoost)
                    {
                        dY = 10;
                    }
                    else
                    dY = 5;
                }
                else if(direction == 3)
                {
                    rotateTransform.Angle = 180;
                    
                    if (speedBoost)
                    {
                        dX = -10;
                    }
                    else
                    dX = -5;
                }
                else if(direction == 4)
                {
                    rotateTransform.Angle = 0;
                    if (speedBoost)
                    {
                        dX = 10;
                    }
                    else
                    dX = 5;
                }
                X += dX;
                Y += dY;
                   
                
                sheildFrameCount--;
                frameCount++;

                if (Y < 0)
                {
                    dY = 0.0;
                    Died();
                    
                }
                if (Math.Abs(Y-(G.gameHeight - (ScaledHeight / 2.0)-10.0))<0.00000001)
                {
                    Died();
                }
                if (X > G.gameWidth)
                {
                    Died();
                }
                if (X < 0)
                {
                    Died();
                }
                
                if (G.isKeyPressed(Key.Left))
                {
                    
                    direction = 3;
                    dY = 0;
                }
                if (G.isKeyPressed(Key.Right))
                {
                    
                    direction = 4;
                    dY = 0;
                }
                if (G.isKeyPressed(Key.Down))
                {
                    direction = 2;
                    dX = 0;
                }
                if (G.isKeyPressed(Key.Up))
                {
                    direction = 1;
                    dX = 0;
                }
                if (G.isKeyPressed(Key.Space)&& powerMove > 0)
                {
                    pTime= 500;
                    poweredUp = true;
                }
                if(poweredUp)
                {
                    pTime--;
                    if(pTime == 0)
                    {
                        poweredUp = false;
                        powerMove--;
                    }
                }
                if(numCollected %10 == 0 && numCollected != 0)
                {
                    speedBoost = true;
                    speedTimer = 300;
                }
                if(speedBoost)
                {
                    speedTimer--;
                    if(speedTimer == 0)
                    {
                        speedBoost = false;
                    }
                }
                if(powerMove == 0&&numCollected >= 100)
                {
                    powerMove = 1;
                }
                foreach (Nit z in Nit.listNits)
                {
                    if (G.checkCollision(this, z))
                    {
                       
                        bigExploSound.Stop();
                        bigExploSound.Play();
                        Explosion exp = Explosion.getNextAvailable();
                        if (exp != null)
                        {
                            exp.X = z.X;
                            exp.Y = z.Y;
                            
                        }
                        if(speedBoost)
                        {
                            G.score += 100;
                        }
                        else
                        {
                            G.score += 50;
                        }
                        
                        z.makeInactive();
                        numCollected += 1;
                    }
                }
                foreach(BigNit b in BigNit.listBigNits)
                {
                    if (G.checkCollision(this, b))
                    {
                        if (poweredUp)
                        {
                            b.makeInactive();
                            bigExploSound.Stop();
                            bigExploSound.Play();
                            G.score += 200;
                        }
                        else
                        {
                            scream.Stop();
                            scream.Play();
                            Explosion explo = Explosion.getNextAvailable();

                            makeInactive();

                            G.gameOver = true;
                        }
                       
                    }
                }
                double maxSpeed = 20.0;
                if (dX > maxSpeed) dX = maxSpeed;
                if (dX < -maxSpeed) dX = -maxSpeed;
            }
        }

    }
}
