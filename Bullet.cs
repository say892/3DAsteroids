using Microsoft.Xna.Framework;


namespace _3DAsteroids
{
    class Bullet
    {

        public Quaternion rotation;
        public Vector3 position;
        Vector3 velocity;

        public bool isActive;

        private int outLimit;

        public Bullet()
        {
            rotation = Quaternion.Identity;
            position = Vector3.Zero;
            isActive = false;
        }

        public void setBullet(Vector3 pos, Quaternion rot, int num)
        {
            //velocity = Matrix.CreateFromQuaternion(rot) * Matrix.CreateTranslation(new Vector3(0, 0, 1));

            velocity = Vector3.Transform(new Vector3(0, 0, -2), rot);

            if (num == 1)
            {
                velocity = Vector3.Transform(new Vector3(-1, 0, -2), rot);
                rot *= Quaternion.CreateFromYawPitchRoll(9.9F, 0, 0); //I made these numbers up. They work.
            }
            else  if(num == 2)
            {
                velocity = Vector3.Transform(new Vector3(1, 0, -2), rot);
                rot *= Quaternion.CreateFromYawPitchRoll(-9.9F, 0, 0);
            }
       
            position = (pos + velocity); //move it a bit in front of the model so it's not half in the ship
            rotation = rot;
            isActive = true;
        }


        public void updateBullet()
        {
            position += velocity;
            //position = Vector3.Transform(position, velocity); velocity is a matrix
            
        }

        public void setBounds(int limit)
        {
            outLimit = limit;
        }

        public bool outOfBounds()
        {
            //check if the position is ever outside of the universe
            if (position.X > outLimit)  return true;
            if (position.X < -outLimit) return true;
            if (position.Y > outLimit)  return true;
            if (position.Y < -outLimit) return true;
            if (position.Z > outLimit)  return true;
            if (position.Z < -outLimit) return true;
            
            return false;


        }

        public Matrix getWorldMatrix()
        {
            return Matrix.CreateScale(0.5F, 0.5F, 2) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
        }


    }


}
