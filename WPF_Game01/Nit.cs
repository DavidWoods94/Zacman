
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
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFStartupDemo
{
    class Nit : GameObject
    {
        static BitmapImage bMap1 = null;
        static BitmapImage bMap2 = null;
        static BitmapImage bMap3 = null;
        static BitmapImage bMap4 = null;
        static BitmapImage bMap5 = null;
        public static MediaPlayer meltSound = null;
        static MediaPlayer pistolSound = null;
        static MediaPlayer bigExploSound = null;
        static public MediaPlayer gameOverSound = null;
        public static List<Nit> listNits = new List<Nit>();
        public static int activeNits = 0;
        public static int numFruits = 0;
        public int hp;
        public int MAXHP = 1;
        public Nit()
        {
            G.SetupSound(ref pistolSound, "pistolshotSoft.wav");
            G.SetupSound(ref bigExploSound, "implosion3.wav");
            G.SetupSound(ref gameOverSound, "GameOver.wav");
            G.SetupSound(ref meltSound, "meltsound.wav");

            UseImage("raspberry.png", ref bMap1);
            UseImage("watermelon.png", ref bMap3);
            UseImage("strawberry.png", ref bMap4);
            UseImage("cherry.png", ref bMap5);
            UseImage("tomato.png", ref bMap2);
            
            if (numFruits % 5 == 0)
            {
                ((Image)element).Source = bMap1;
            }
            else if (numFruits % 5 == 1)
            {
                ((Image)element).Source = bMap2;
            }
            else if (numFruits % 5 == 2)
            {
                ((Image)element).Source = bMap3;
            }
            else if (numFruits % 5 == 3)
            {
                ((Image)element).Source = bMap4;
            }
            else
            {
                ((Image)element).Source = bMap5;
            }
            hp = MAXHP;
            
            Scale = 0.15;

            
            Random rand = new Random();
            int num = rand.Next(2);

            SetSpeed();
            if(G.chance(.5))
            {
                SetRight();
                //movingRight = false;
            }
            else 
            {
                SetLeft();
                //movingRight = true;
            }

            //dX = 300;
            listNits.Add(this);
            dY = 0.0;
            //if(G.chance(.5))
            {
                makeInactive();
               
            }
            //else 
           
            //activeNits++; 
            numFruits++;
            AddToGame();
        }

        public static Nit getNextAvailable()
        {
            foreach (Nit b in listNits)
            {
                if (!b.isActive)
                {

                    return b;
                }
            }
            return null;
        }

        public void Kill()
        {
            Explosion exp = Explosion.getNextAvailable();
            
            if (exp != null)
            {
                exp.X = this.X;
                exp.Y = this.Y;
            }
           
            activeNits--;
            makeInactive();
        }

        public void SetSpeed()
        {
            if(G.chance(.3))
            {
                acceleration = 0;
            }
            else if(G.chance(.3))
            {
                acceleration = 0;
            }
            else
            {
                acceleration = 0;
            }
        }

        public void SetInMiddle()
        {
            X = G.gameWidth / 2.0;
            Y = G.gameHeight - (ScaledHeight / 2.0) - 10.0;
        }
        public void SetRandom()
        {
            Random r = new Random();
            X = r.Next(0, (int) G.gameWidth);
            Y = r.Next(0, (int)G.gameHeight);
        }
        public void SetRight()
        {
            X = G.gameWidth ;
            Y = G.gameHeight - (ScaledHeight / 2.0) - 10.0;
            movingRight = false;
        }

        public void SetLeft()
        {
            X = -20;
            Y = G.gameHeight - (ScaledHeight / 2.0) - 10.0;
            movingRight = true;
        }

        double gravity = 1.0;
        double friction = 0.0;
        double acceleration = 1.0;

        int frameCount = 0;
        public bool movingRight = false;
        public int sheildFrameCount = 0;
        public bool isSheildOn = false;

        internal override void update()
        {
            if (isActive)
            {
                X += dX;
                if (dY != 0.0) dY += gravity;
                //dY += gravity;
                Y += dY;
                dX *= friction;
                //sheildFrameCount--;
                frameCount++;

                if (Y > G.gameHeight - (ScaledHeight / 2.0)-10.0)
                {
                    Y = G.gameHeight - (ScaledHeight / 2.0) -10.0;
                    dY = 0.0;
                }
                else if (Math.Abs(Y-(G.gameHeight - (ScaledHeight / 2.0)-10.0))<0.00000001)
                {
                }
                if (X > G.gameWidth - Width) dX = -Math.Abs(dX);
                if (X < 0) dX = Math.Abs(dX);

                if (movingRight == false)
                {
                    dX -= acceleration;
                    if(X <= -20)
                    {
                        makeInactive();
                        activeNits--;
                        hp = MAXHP;
                    }
                }
                else
                {
                    dX += acceleration;
                    if (X >= G.gameWidth + 20)
                    {
                        makeInactive();
                        activeNits--;
                        hp = MAXHP;
                    }
                }

                if (hp <= 0)
                {
                    
                    makeInactive();
                    hp = MAXHP;
                    activeNits--;
                    G.score += 10;
                }
                    

                double maxSpeed = 20.0;

                if (dX > maxSpeed) dX = maxSpeed;
                if (dX < -maxSpeed) dX = -maxSpeed;
            }
        }
    }
}
