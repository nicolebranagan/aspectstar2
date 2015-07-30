using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    public static class PlaySound
    {
        public static SoundEffect die;
        public static SoundEffect aspect;

        public static void Die()
        {
            die.Play();
        }

        public static void Aspect()
        {
            aspect.Play();
        }
    }
}
