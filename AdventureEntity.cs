using Jint;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace aspectstar2
{
    public class AdventureEntity : AdventureObject
    {
        public string name;

        Engine jintEngine;
        EntityData data;

        bool floating = false;
        bool solid = true;

        int touchLag = 0;

        public AdventureEntity(EntityData data)
        {
            name = data.name;
            this.data = data;
        }

        public override void Initialize(AdventureScreen parent, Game game)
        {
            base.Initialize(parent, game);
            jintEngine = ActivateEngine(parent.ActivateEngine(data.code));

            switch (data.gfxtype)
            {
                case EntityData.GraphicsType.Maptile:
                    texture = Master.texCollection.adventureTiles[parent.tileset];
                    break;
                case EntityData.GraphicsType.Enemies:
                    texture = Master.texCollection.texEnemies;
                    graphicsRow = data.graphics;
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            if (data.gfxtype == EntityData.GraphicsType.Maptile)
            {
                Rectangle source, dest;
                Vector2 sourceTile;
                spriteBatch.Begin();
                sourceTile = Master.getMapTile(data.graphics, texture);
                source = new Rectangle((int)sourceTile.X, (int)sourceTile.Y, 32, 32);
                dest = new Rectangle((int)location.X - 16, (int)location.Y - 16, 32, 32);
                spriteBatch.Draw(texture, dest, source, mask);
                spriteBatch.End();
            }
            else
                base.Draw(spriteBatch, mask);
        }

        Engine ActivateEngine(Engine engine)
        {
            return engine
                .SetValue("setDimensions", new Action<int,int>(SetDimensions))
                .SetValue("overrideSettings", new Action<bool, bool>(OverrideSettings))
                .SetValue("move", new Action<int, int>(CommandMove))
                .SetValue("hurtPlayer", new Action(parent.player.Hurt))
                .SetValue("die", new Action(Die))
                .Execute("onLoad()");
        }

        public override void Update()
        {
            if (touchLag > 0)
                touchLag = touchLag - 1;
            base.Update();
            jintEngine.Execute("update()");
        }

        public override bool inRange(AdventurePlayer player)
        {
            Vector2 playerloc = player.location;
            double del = Math.Sqrt(Math.Pow(location.X - playerloc.X, 2) + Math.Pow(location.Y - playerloc.Y, 2));
            if (del <= Math.Max(width, height))
            {
                jintEngine.Execute("inRange()");
                player.Recoil(this.location);
                return solid;
            }
            else
                return false;
        }

        public override void Touch()
        {
            if (touchLag == 0)
            {
                jintEngine.Execute("touch()");
                touchLag = 20;
            }
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

        void Die()
        {
            active = false;
        }
    }
}
