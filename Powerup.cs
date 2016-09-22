using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DAsteroids
{
    class Powerup
    {

        public Model fuelModel;
        public Model speedModel;
        public Model multiShotModel;

        public Constants.PowerUpType type;

        public Quaternion rotation;
        public Vector3 position;
        Vector3 velocity;
        Vector3 rotVelocity;

        public bool isActive;

        public Random rand;

        public int outerLimit;

        public Powerup(Random rand, int outLimit)
        {
            isActive = false;

            this.rand = rand;

            outerLimit = outLimit;

            float Tempx = (float)rand.NextDouble() * 360; //Range from 0 to 360
            float Tempy = (float)rand.NextDouble() * 360;
            float Tempz = (float)rand.NextDouble() * 360;

            rotation = Quaternion.CreateFromYawPitchRoll(Tempx, Tempy, Tempz);

            velocity.X = (float)rand.NextDouble() * .05F - .025F; //Range from -0.025 to 0.025
            velocity.Y = (float)rand.NextDouble() * .05F - .025F; 
            velocity.Z = (float)rand.NextDouble() * .05F - .025F;

            rotVelocity.X = (float)rand.NextDouble() * 0.05F - 0.025F; //Range from -0.025 to 0.025
            rotVelocity.Y = (float)rand.NextDouble() * 0.05F - 0.025F;
            rotVelocity.Z = (float)rand.NextDouble() * 0.05F - 0.025F;


        }



        public void spawnPowerup()
        {
            position.X = (float)rand.NextDouble() * outerLimit * 2 - outerLimit; //In the playing area
            position.Y = (float)rand.NextDouble() * outerLimit * 2 - outerLimit;
            position.Z = (float)rand.NextDouble() * outerLimit * 2 - outerLimit;

            type = (Constants.PowerUpType) rand.Next(0, 3);
           
            isActive = true;
        }

        public void updatePosition(float delta)
        {
            position += velocity * delta;
            checkOutOfBounds();
            updateRotation();
        }

        public void updateRotation()
        {
            rotation *= Quaternion.CreateFromYawPitchRoll(rotVelocity.X, rotVelocity.Y, rotVelocity.Z);
        }

        public void checkOutOfBounds()
        {


            if (position.X > outerLimit)
            {
                position -= velocity;
                velocity = Vector3.Reflect(velocity, Vector3.Right);
            }
            if (position.X < -outerLimit)
            {
                position -= velocity;
                velocity = Vector3.Reflect(velocity, Vector3.Left);
            }

            if (position.Y > outerLimit)
            {
                position -= velocity;
                velocity = Vector3.Reflect(velocity, Vector3.Up);
            }
            if (position.Y < -outerLimit)
            {
                position -= velocity;
                velocity = Vector3.Reflect(velocity, Vector3.Down);
            }

            if (position.Z > outerLimit)
            {
                position -= velocity;
                velocity = Vector3.Reflect(velocity, Vector3.Backward);
            }
            if (position.Z < -outerLimit)
            {
                position -= velocity;
                velocity = Vector3.Reflect(velocity, Vector3.Forward);
            }


        }

        public Matrix getWorldMatrix()
        {
            return Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
        }

        public void setModel(Model fuel, Model multi, Model speed)
        {
            this.fuelModel = fuel;
            this.multiShotModel = multi;
            this.speedModel = speed;
        }

        public Model getModel()
        {
            if (type == Constants.PowerUpType.Fuel) return fuelModel;
            else if (type == Constants.PowerUpType.MultiShot) return multiShotModel;
            return speedModel;
            //if (type == Constants.PowerUpType.Speed) return speedModel;
        }

        public Constants.PowerUpType PickUp()
        {
            isActive = false;
            return type;
        }


    }
}
