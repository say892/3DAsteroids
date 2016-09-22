using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace _3DAsteroids
{

    public class Asteroid
    {

        public Quaternion rotation = Quaternion.Identity;
        public Vector3 position = Vector3.Zero;
        Vector3 velocity = Vector3.Zero;
        Vector3 rotVelocity = Vector3.Zero;

        public Model bigModel;
        public Model medModel;
        public Model smallModel;

        public BoundingBox bounds;

        public bool isActive;

        private int outLimit; //how far an asteroid can go before it bounces back

        private int difficulty; //the odds of an asteroid exploding

        Random rand;
        int size; //3 = large, 2 = medium, 1 = small

        public Asteroid(Random rand, int outerLimit, int diff)
        {

            float Tempx = (float)rand.NextDouble() * 360; //Range from 0 to 360
            float Tempy = (float)rand.NextDouble() * 360;
            float Tempz = (float)rand.NextDouble() * 360;


            rotation = Quaternion.CreateFromYawPitchRoll(Tempx, Tempy, Tempz);

            position.X = (float) rand.NextDouble() * outerLimit * 2 - outerLimit; //Populate asteroids in the
            position.Y = (float) rand.NextDouble() * outerLimit * 2 - outerLimit; //playing area.
            position.Z = (float) rand.NextDouble() * outerLimit * 2 - outerLimit;

            ////position.X = (float)rand.NextDouble() * 100 - 50; //Range from -15 to 15 for testing
            //position.Y = (float)rand.NextDouble() * 100 - 50;
            //position.Z = (float)rand.NextDouble() * 100 - 50;

            velocity.X = (float)rand.NextDouble() * .5F - .25F; //Range from -0.025 to 0.025
            velocity.Y = (float)rand.NextDouble() * .5F - .25F; //range from -3 to 3
            velocity.Z = (float)rand.NextDouble() * .5F - .25F;

            rotVelocity.X = (float)rand.NextDouble() * 0.05F - 0.025F; //Range from -0.025 to 0.025
            rotVelocity.Y = (float)rand.NextDouble() * 0.05F - 0.025F;
            rotVelocity.Z = (float)rand.NextDouble() * 0.05F - 0.025F;

            isActive = true;

            size = 3;

            outLimit = outerLimit;

            this.rand = rand;
            difficulty = diff;


        }

        public void updatePosition(float delta)
        {
            position += velocity * delta;
            checkOutOfBounds();
           // updateBounds();
            updateRotation();
        }

        public void updateRotation()
        {
            //rotation.X += rotVelocity.X;
            //rotation.Y += rotVelocity.Y;
            //rotation.Z += rotVelocity.Z;

            rotation *= Quaternion.CreateFromYawPitchRoll(rotVelocity.X, rotVelocity.Y, rotVelocity.Z);
        }
        public void CollisionHit()
        {
            position -= velocity;
            position -= velocity;
            velocity = -velocity;

            if (rand.Next(0, 99) <= difficulty)
            {
                size--;
                //System.Diagnostics.Debug.Write("I was lowered!");

            }
            //System.Diagnostics.Debug.Write("" + temp + ", " + difficulty + "\n");

            //System.Diagnostics.Debug.Write("ouch!");
        }

        //public void setupBounds()
       // {
       //     bounds = BoundingBox.CreateFromSphere(model.Meshes[0].BoundingSphere);
       // }

        public Model getModel()
        {
            if (size == 3) return bigModel;
            if (size == 2) return medModel;
            else return smallModel;
            //else is size == 1
        }

       // public void updateBounds()
       // {
            //I wonder if this works (it does not)
            //BoundingBox all = BoundingBox.CreateFromSphere(model.Meshes[0].BoundingSphere);
            //int count = 0;
            //foreach(ModelMesh m in model.Meshes)
            //{
            //System.Diagnostics.Debug.Write("" + count + "\n");
            //     BoundingBox.CreateMerged(all, BoundingBox.CreateFromSphere(m.BoundingSphere));
            //count++;         
            // }
            // bounds = all;
            //System.Diagnostics.Debug.Write("" + bounds.GetCorners().ToArray().ToString() + "\n");

            //all = BoundingBox.CreateFromSphere(model.Meshes[0].BoundingSphere);
            //bounds = all;


            //This works if the asteroid isn't rotating
            //bounds.Min += velocity;
            //bounds.Max += velocity;

           // bounds = BoundingBox.CreateFromSphere(model.Meshes[0].BoundingSphere);

       // }

        public void checkOutOfBounds()
        {

           
            if (position.X > outLimit)
            {
                position -= velocity;
                velocity = Vector3.Reflect(velocity, Vector3.Right);
            }
            if (position.X < -outLimit)
            {
                position -= velocity;
                velocity = Vector3.Reflect(velocity, Vector3.Left);
            }

            if (position.Y > outLimit)
            {
                position -= velocity;
                velocity = Vector3.Reflect(velocity, Vector3.Up);
            }
            if (position.Y < -outLimit)
            {
                position -= velocity;
                velocity = Vector3.Reflect(velocity, Vector3.Down);
            }

            if (position.Z > outLimit)
            {
                position -= velocity;
                velocity = Vector3.Reflect(velocity, Vector3.Backward);
            }
            if (position.Z < -outLimit)
            {
                position -= velocity;
                velocity = Vector3.Reflect(velocity, Vector3.Forward);
            }


        }

        public Matrix getWorldMatrix()
        {
            if (size == 3) return Matrix.CreateScale(7, 7, 7) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
            if (size == 2) return Matrix.CreateScale(5, 5, 5) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
            else return Matrix.CreateScale(2, 2, 2) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
            //else is size == 1

        }

        //returns true if the asteroid is destroyed
        public bool hit()
        {
            size--;

            if (size == 0)
            {
                isActive = false;
                return true;
            }

            return false;
        }

    }
}