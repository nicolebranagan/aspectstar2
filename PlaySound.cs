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
        public static SoundEffect jump;
        public static SoundEffect hurt;
        public static SoundEffect enter;
        public static SoundEffect drown;

        static SoundEffectInstance hurtInst;
        static SoundEffectInstance jumpInst;
        static SoundEffectInstance drowInst;

        public static void Initialize()
        {
            hurtInst = hurt.CreateInstance();
            jumpInst = jump.CreateInstance();
            drowInst = drown.CreateInstance();
        }

        public static void Die()
        {
            if (hurtInst.State == SoundState.Playing)
                hurtInst.Stop();
            die.Play();
        }

        public static void Aspect()
        {
            aspect.Play();
        }

        public static void Jump()
        {
            if (jumpInst.State != SoundState.Playing)
                jumpInst.Play();
        }

        public static void Enter()
        {
            enter.Play();
        }

        public static void Hurt()
        {
            if (hurtInst.State != SoundState.Playing)
                hurtInst.Play();
        }

        public static void Drown()
        {
            if (drowInst.State != SoundState.Playing)
                drowInst.Play();
        }
    }
}
