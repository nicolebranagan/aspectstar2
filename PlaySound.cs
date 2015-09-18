using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;

namespace aspectstar2
{
    public static class PlaySound
    {
        static SoundEffect die;
        static SoundEffect aspect;
        static SoundEffect jump;
        static SoundEffect hurt;
        static SoundEffect enter;
        static SoundEffect drown;
        static SoundEffect key;
        static SoundEffect boom;
        static SoundEffect pew;
        static SoundEffect leave;
        static SoundEffect pause;
        static SoundEffect special;

        static SoundEffectInstance hurtInst;
        static SoundEffectInstance jumpInst;
        static SoundEffectInstance drowInst;
        static SoundEffectInstance keysInst;
        static SoundEffectInstance pewwInst;

        public static void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            PlaySound.die = Content.Load<SoundEffect>("die");
            PlaySound.aspect = Content.Load<SoundEffect>("aspect");
            PlaySound.jump = Content.Load<SoundEffect>("jump");
            PlaySound.enter = Content.Load<SoundEffect>("enter");
            PlaySound.hurt = Content.Load<SoundEffect>("hurt");
            PlaySound.drown = Content.Load<SoundEffect>("drown");
            PlaySound.key = Content.Load<SoundEffect>("key");
            PlaySound.pew = Content.Load<SoundEffect>("pew");
            PlaySound.boom = Content.Load<SoundEffect>("boom");
            PlaySound.leave = Content.Load<SoundEffect>("leave");
            PlaySound.pause = Content.Load<SoundEffect>("pause");
            PlaySound.special = Content.Load<SoundEffect>("computer");

            hurtInst = hurt.CreateInstance();
            jumpInst = jump.CreateInstance();
            drowInst = drown.CreateInstance();
            keysInst = key.CreateInstance();
            pewwInst = pew.CreateInstance();
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

        public static void Key()
        {
            if (keysInst.State != SoundState.Playing)
                keysInst.Play();
        }

        public static void Boom()
        {
            boom.Play();
        }

        public static void Pew()
        {
            if (pewwInst.State != SoundState.Playing)
                pewwInst.Play();
        }

        public static void Leave()
        {
            leave.Play();
            MediaPlayer.Stop();
        }

        public static void Pause()
        {
            pause.Play();
        }

        public static void Special()
        {
            special.Play();
        }
    }
}
