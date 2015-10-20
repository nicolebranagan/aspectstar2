using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspectstar2
{
    public static class PlaySong
    {
        public enum SongName
        {
            None = -1,
            Title = 0,
            WorldMap = 1,
            Town = 2,
            Garrison = 3,
            Boss = 4,
            Item = 5,
            Dungeon1 = 6,
            Dungeon2 = 7,
            Dungeon3 = 8,
            Dungeon4 = 9,
            Dungeon5 = 10,
            Dungeon6 = 11,
            Dungeon7 = 12,
            Dungeon8 = 13,
            Dungeon9a = 14,
            Dungeon9b = 15,
            FinalBoss = 16,
            Credits = 17,
            Silent = 18,
        }

        static Dictionary<SongName, Song> Songs = new Dictionary<SongName, Song>();
        static SongName currentSong = SongName.None;
        static public bool enabled = true;

        public static void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            try
            {
                Songs[SongName.Title] = Content.Load<Song>("music_azureflux06");
                Songs[SongName.WorldMap] = Content.Load<Song>("music_visager14");
                Songs[SongName.Town] = Content.Load<Song>("music_visager10");
                Songs[SongName.Boss] = Content.Load<Song>("music_visager09");
                Songs[SongName.Item] = Content.Load<Song>("music_visager03");
                Songs[SongName.Dungeon1] = Content.Load<Song>("music_visager06");
                Songs[SongName.Dungeon2] = Content.Load<Song>("music_visager02");
                Songs[SongName.Dungeon3] = Content.Load<Song>("music_visager04");
                Songs[SongName.Dungeon4] = Content.Load<Song>("music_visager07");
                Songs[SongName.Dungeon5] = Content.Load<Song>("music_azureflux05");
                Songs[SongName.Dungeon6] = Content.Load<Song>("music_visager08");
                Songs[SongName.Dungeon7] = Content.Load<Song>("music_azureflux03");
                Songs[SongName.Dungeon8] = Content.Load<Song>("music_visager11");
            }
            catch { 
                // No Music
            }
        }

        public static void Play(SongName song)
        {
            if (enabled && currentSong != song)
            {
                MediaPlayer.Stop();
                currentSong = song;
                if (Songs.ContainsKey(song))
                {
                    MediaPlayer.Play(Songs[song]);
                }
                MediaPlayer.Volume = 0.4f;
                MediaPlayer.IsRepeating = true;
            }
        }
    }
}
