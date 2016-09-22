using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace _3DAsteroids
{
    class Player
    {
        public Vector3 position;
        public Quaternion rotation;
        public Model model;

        public float upDownRot;
        public float leftRightRot;

        public Vector3 forwardThrust; 

        public Bullet[] bulletSupply; //reuse bullets!
        int currentBullet; //the bullet that will be fired next from the pool

        float timer; //time to shoot again

        private int outLimit;

        private SoundEffect missileSound;

        public float fuel;

        public bool multiBullet;
        public float multiTimer;

        public bool doubleSpeed;
        public float doubleTimer;
        //public BoundingBox bounds;

        public Player()
        {
            position = Vector3.Zero;
            rotation = Quaternion.Identity;

            upDownRot = 0;
            leftRightRot = 0;

            forwardThrust = Vector3.Zero;

            bulletSupply = new Bullet[20];

            for (int i = 0; i < bulletSupply.Length; i++)
            {
                bulletSupply[i] = new Bullet();
            }

            currentBullet = 0;

            timer = 0;

            fuel = 100;

            multiBullet = false;
       

            doubleSpeed = false;
        }

        public void updateTimer(float timeElapsed)
        {
            timer += timeElapsed;
            fuel -= timeElapsed /2;

            if (multiBullet) multiTimer += timeElapsed;
            if (doubleSpeed) doubleTimer += timeElapsed;


            if (multiTimer > 10)
            {
                multiBullet = false;
                multiTimer = 0;
            }
            if (doubleTimer > 10)
            {
                doubleSpeed = false;
                doubleTimer = 0;
            }

        }

        public bool canShoot()
        {
            return timer >= Constants.SHOOTTIMER;
        }

        public void fireBullet(Vector3 pos, Quaternion rot)
        {
            timer = 0;
            missileSound.Play();
            if (multiBullet)
            {

                Vector3 up = Vector3.Transform(new Vector3(0, -1, 0), rotation);
               // Quaternion bullet1 = Quaternion.CreateFromYawPitchRoll(forward.X, forward.Y, forward.Z);
                Quaternion bullet1 = rot + Quaternion.CreateFromAxisAngle(up, 2);


                //bulletSupply[currentBullet].setBullet(pos, rot + Quaternion.CreateFromAxisAngle(up, 2));
               // currentBullet++;
                //currentBullet %= (bulletSupply.Length);

                bulletSupply[currentBullet].setBullet(pos, rot, 1);
                currentBullet++;
                currentBullet %= (bulletSupply.Length);

                bulletSupply[currentBullet].setBullet(pos, rot, 2);
                currentBullet++;
                currentBullet %= (bulletSupply.Length);
            }

            bulletSupply[currentBullet].setBullet(pos, rot, 0);
            currentBullet++;
            currentBullet %= (bulletSupply.Length);
        }

        public void setSound(SoundEffect sound)
        {
            missileSound = sound;
        }

        public void setBounds(int limit)
        {
            outLimit = limit;
            for (int i = 0; i < bulletSupply.Length; i++)
            {
                bulletSupply[i].setBounds(outLimit);
            }
        }

        public void updateBounds()
        {
            /*BoundingBox all = new BoundingBox();
            int count = 0;
            foreach (ModelMesh m in model.Meshes)
            {
                System.Diagnostics.Debug.Write("" + count + "\n");
                BoundingBox.CreateMerged(all, BoundingBox.CreateFromSphere(m.BoundingSphere));
                count++;
            }
            bounds = all;*/

            //bounds = BoundingBox.CreateFromSphere(model.Meshes[0].BoundingSphere);
        }

        public void playerFlight()
        {
            leftRightRot = 0;
            upDownRot = 0;
            float rotationSpeed = 0.025F;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                leftRightRot += rotationSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                leftRightRot -= rotationSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                upDownRot += rotationSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                upDownRot -= rotationSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                Vector3 increase = Vector3.Transform(new Vector3(0, 0, -0.1F), rotation);
                forwardThrust += increase;

                if (forwardThrust.LengthSquared() >= Constants.MAXSPEED * Constants.MAXSPEED)
                {
                    forwardThrust.Normalize();
                    forwardThrust *= Constants.MAXSPEED;
                }
               
                //position = Vector3.Transform(position, Matrix.CreateTranslation(0, 0, 0.1F));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                //Create Bullet
                if (canShoot())
                {
                    fireBullet(position, rotation);
                }
            }

            //update position based off of all of the rotating done above
            rotation *= Quaternion.CreateFromYawPitchRoll(leftRightRot, upDownRot, 0);

            if (checkOutOfBounds())
            {
                //bounce if going out of bounds
                forwardThrust *= -1;
                          
            }


            //PROGRESS NEVER STOPS, ALWAYS MOVE FORWARD
            if (doubleSpeed) position += forwardThrust*2;
            else position += forwardThrust;


            //cool the engines bro
            forwardThrust *= .99F;
        }

        bool checkOutOfBounds()
        {
            if (position.X > outLimit)
            {
                return true;
            }
            if (position.X < -outLimit)
            {
                return true;
            }

            if (position.Y > outLimit)
            {
                return true;
            }
            if (position.Y < -outLimit)
            {
                return true;
            }

            if (position.Z > outLimit)
            {
                return true;
            }
            if (position.Z < -outLimit)
            {
                return true;
            }
            return false;
        }

        public void addFuel(float fuel)
        {
            this.fuel += fuel;
            if (this.fuel > 100) this.fuel = 100;
        }

    }

    

}