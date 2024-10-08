﻿namespace Flappy.Animations
{
    public static class EasingFunctions
    {
        public static float EaseQuinticOut(float k)
        {
            return 1f + ((k -= 1f) * (float)Math.Pow(k, 4));
        }

        public static float EaseQuinticIn(float k)
        {
            return k * k * k * k * k;
        }


        public static float EaseOutElastic(float k)
        {
            const float c4 = (2f * (float)Math.PI) / 3f;

            return k == 0f ? 0f : k == 1f ? 1f : (float)Math.Pow(2f, -10f * k) * (float)Math.Sin((k * 10f - 0.75f) * c4) + 1f;
        }
    }
}
