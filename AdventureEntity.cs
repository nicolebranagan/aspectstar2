using Jint;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    public class AdventureEntity : AdventureObject
    {
        public string name;

        Engine jintEngine;
        string code;

        bool floating = false;
        bool solid = true;

        public AdventureEntity(string name, string code)
        {
            this.name = name;
            this.code = code;
        }

        public override void Initialize(AdventureScreen parent, Game game)
        {
            base.Initialize(parent, game);
            jintEngine = ActivateEngine(parent.ActivateEngine(code));
        }

        Engine ActivateEngine(Engine engine)
        {
            return engine
                .SetValue("setDimensions", new Action<int,int>(SetDimensions))
                .SetValue("overrideSettings", new Action<bool, bool>(OverrideSettings))
                .SetValue("move", new Action<int, int>(CommandMove))
                .SetValue("hurtPlayer", new Action(parent.player.Hurt))
                .Execute("onLoad()");
        }

        public override bool inRange(AdventurePlayer player)
        {
            Vector2 playerloc = player.location;

            if (Math.Sqrt(Math.Pow(location.X - playerloc.X, 2) + Math.Pow(location.Y - playerloc.Y, 2)) <= Math.Max(width, height))
            {
                jintEngine.Execute("inRange()");
                return solid;
            }
            return false;
        }

        public override void Touch()
        {
            jintEngine.Execute("touch()");
        }

        public void Execute(string exec)
        {
            jintEngine.Execute(exec);
        }

        void HurtPlayer()
        {
            parent.player.Hurt();
            parent.player.Recoil(location);
        }

        void SetDimensions(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        void OverrideSettings(bool floating, bool solid)
        {
            this.floating = floating;
            this.solid = solid;
        }

        void CommandMove(int x, int y)
        {
            Vector2 move_dir = new Vector2(x, y);
            if (floating)
                location = location + move_dir;
            else
                Move(move_dir);
        }
    }
}
