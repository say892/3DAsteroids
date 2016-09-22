using Microsoft.Xna.Framework;

namespace _3DAsteroids
{
    class Explosion
    {

        public Vector3 position;
        public bool isExploding;
        public float timer;

        public Explosion()
        {
            position = Vector3.Zero;
            isExploding = false;
            timer = 0;
        }

        public void CreateExplosion(Vector3 pos)
        {
            position = pos;
            isExploding = true;
            timer = 0;
        }

        public void updateTimer(float timeElapsed)
        {
            timer += timeElapsed;

            if (timer >= .75F)
            {
                isExploding = false;
            }
        }

    }
}
