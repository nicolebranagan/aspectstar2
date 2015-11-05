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
        public enum SoundEffectName
        {
            Aspect = 0,
            Boom = 1,
            Die = 2,
            Jump = 3,
            Hurt = 4,
            Enter = 5,
            Drown = 6,
            Key = 7,
            Pew = 8,
            Leave = 9,
            Pause = 10,
            Special = 11,
            Coin = 12,
        }

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
        static SoundEffect coin;

        static SoundEffectInstance hurtInst;
        static SoundEffectInstance jumpInst;
        static SoundEffectInstance drowInst;
        static SoundEffectInstance keysInst;
        static SoundEffectInstance pewwInst;

        public static bool enabled = true;

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
            PlaySound.coin = Content.Load<SoundEffect>("coin");

            hurtInst = hurt.CreateInstance();
            jumpInst = jump.CreateInstance();
            drowInst = drown.CreateInstance();
            keysInst = key.CreateInstance();
            pewwInst = pew.CreateInstance();
        }

        public static void Play(SoundEffectName name)
        {
            if (!enabled)
                return;

            switch (name)
            {
                case SoundEffectName.Aspect:
                    aspect.Play();
                    break;
                case SoundEffectName.Boom:
                    boom.Play();
                    break;
                case SoundEffectName.Die:
                    if (hurtInst.State == SoundState.Playing)
                        hurtInst.Stop();
                    die.Play();
                    break;
                case SoundEffectName.Jump:
                    if (jumpInst.State != SoundState.Playing)
                        jumpInst.Play();
                    break;
                case SoundEffectName.Hurt:
                    if (hurtInst.State != SoundState.Playing)
                        hurtInst.Play();
                    break;
                case SoundEffectName.Enter:
                    enter.Play();
                    break;
                case SoundEffectName.Drown:
                    if (drowInst.State != SoundState.Playing)
                        drowInst.Play();
                    break;
                case SoundEffectName.Key:
                    if (keysInst.State != SoundState.Playing)
                        keysInst.Play();
                    break;
                case SoundEffectName.Pew:
                    if (pewwInst.State != SoundState.Playing)
                        pewwInst.Play();
                    break;
                case SoundEffectName.Leave:
                    leave.Play();
                    break;
                case SoundEffectName.Pause:
                    pause.Play();
                    break;
                case SoundEffectName.Special:
                    special.Play();
                    break;
                case SoundEffectName.Coin:
                    coin.Play();
                    break;
            }
        }
    }
}
