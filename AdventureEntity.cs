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

        bool firstLoad = false;

        bool floating = false;
        public bool solid = true;
        bool wander = false;

        int touchLag = 0;
        Color filter = Color.White;

        public AdventureEntity(EntityData data)
        {
            name = data.name;
            this.data = data;
        }

        public override void Initialize(AdventureScreen parent, Game game)
        {
            base.Initialize(parent, game);
            if (!firstLoad)
            {
                firstLoad = true;
                jintEngine = ActivateEngine(parent.ActivateEngine(data.code));
            }
            
            switch (data.gfxtype)
            {
                case EntityData.GraphicsType.Maptile:
                    graphicsRow = data.graphics;
                    texture = Master.texCollection.adventureTiles[parent.tileset];
                    this.width = 16; this.height = 16;
                    break;
                case EntityData.GraphicsType.Enemies:
                    texture = Master.texCollection.texEnemies;
                    graphicsRow = data.graphics;
                    break;
                case EntityData.GraphicsType.Characters:
                    texture = Master.texCollection.texCharacters;
                    graphicsRow = data.graphics;
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            // Overwrite mask
            if (filter != Color.White)
                mask = filter;

            if (data.gfxtype == EntityData.GraphicsType.Maptile)
            {
                Rectangle source, dest;
                Vector2 sourceTile;
                spriteBatch.Begin();
                sourceTile = Master.getMapTile(graphicsRow, texture);
                source = new Rectangle((int)sourceTile.X, (int)sourceTile.Y, 32, 32);
                dest = new Rectangle((int)location.X - 16, (int)location.Y - 16, 32, 32);
                spriteBatch.Draw(texture, dest, source, mask);
                spriteBatch.End();
            }
            else if (data.gfxtype != EntityData.GraphicsType.Null)
                base.Draw(spriteBatch, mask);
        }

        Engine ActivateEngine(Engine engine)
        {
            return engine
                .SetValue("setDimensions", new Action<int, int>(SetDimensions))
                .SetValue("overrideSettings", new Action<bool, bool>(OverrideSettings))
                .SetValue("move", new Action<int, int>(CommandMove))
                .SetValue("hurtPlayer", new Action(parent.player.Hurt))
                .SetValue("die", new Action(Die))
                .SetValue("questionBox", new Action<string, string, string>(QuestionBox))
                .SetValue("setLocation", new Action<int, int>(SetLocation))
                .SetValue("setWander", new Action<bool>(SetWander))
                .SetValue("setColor", new Action<int, int, int, int>(SetColor))
                .SetValue("becomeEnemy", new Action<int, bool>(BecomeEnemy))
                .SetValue("spawnShooter", new Action<int, int, bool>(SpawnShooter))
                .SetValue("changeFace", new Action<int>(ChangeFace))
                .SetValue("changeDirection", new Action<int>(ChangeDirection))
                .SetValue("getThisX", new Func<int>(GetThisX))
                .SetValue("getThisY", new Func<int>(GetThisY))
                .SetValue("explode", new Action<bool>(Explode))
                .Execute("onLoad()");
        }

        public override void Update()
        {
            if (touchLag > 0)
                touchLag = touchLag - 1;
            if (wander)
            {
                if (stallCount % (3) == 0)
                {
                    if (Master.globalRandom.Next(0, 10) > 8)
                    {
                        Master.Directions newDir = (Master.Directions)Master.globalRandom.Next(0, 3);
                        switch (faceDir)
                        {
                            case Master.Directions.Down:
                                if (newDir == Master.Directions.Up)
                                    newDir = Master.Directions.Right;
                                break;
                            case Master.Directions.Up:
                                if (newDir == Master.Directions.Down)
                                    newDir = Master.Directions.Right;
                                break;
                            case Master.Directions.Right:
                                if (newDir == Master.Directions.Left)
                                    newDir = Master.Directions.Right;
                                break;
                        }
                        faceDir = newDir;
                    }
                    else
                    {
                        Vector2 move_dist = new Vector2(0, 0);
                        switch (faceDir)
                        {
                            case Master.Directions.Down:
                                move_dist = new Vector2(0, 2);
                                break;
                            case Master.Directions.Up:
                                move_dist = new Vector2(0, -2);
                                break;
                            case Master.Directions.Left:
                                move_dist = new Vector2(-2, 0);
                                break;
                            case Master.Directions.Right:
                                move_dist = new Vector2(2, 0);
                                break;
                            default:
                                break; // Something has gone wrong
                        }
                        this.Move(move_dist);
                    }
                }
            }
            base.Update();
            jintEngine.Execute("update()");
        }

        public override bool inRange(AdventurePlayer player)
        {
            Vector2 playerloc = player.location;
            //double del = Math.Sqrt(Math.Pow(location.X - playerloc.X, 2) + Math.Pow(location.Y - playerloc.Y, 2));
            //if (del <= Math.Max(width, height))
            if (doesOverlap(player))
            {
                jintEngine.Execute("inRange()");
                if (solid) player.Recoil(this.location, this);
                return solid;
            }
            else
                return false;
        }

        public void enemyInRange(List<AdventureObject> objects)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                AdventureObject obj = objects[i];
                if (obj is AdventureEnemy && doesOverlap(obj))
                {
                    jintEngine.Execute(String.Concat(
                        "if (typeof enemyInRange == 'function') { enemyInRange(", i.ToString(),"); }"
                        ));
                }
            }
        }

        public override void Touch()
        {
            if (touchLag == 0)
            {
                jintEngine.Execute("touch()");
                touchLag = 40;
            }
        }

        public void Hurt(int damage)
        {
            jintEngine.Execute(String.Concat("hurt(",damage.ToString(),")"));
        }

        public void Execute(string exec)
        {
            jintEngine.Execute(exec);
        }

        void HurtPlayer()
        {
            parent.player.Hurt();
            parent.player.Recoil(location, this);
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

        void QuestionBox(string text, string callYes, string callNo)
        {
            parent.QuestionBox(text, getChooser(this, callYes, callNo));
        }

        void SetLocation(int x, int y)
        {
            location = new Vector2(x * 32 + 16, y * 32 + 16);
        }

        void SetWander(bool wander)
        {
            this.wander = wander;
        }

        void SetColor(int r, int g, int b, int a)
        {
            filter = Color.FromNonPremultiplied(r, g, b, a);
        }

        void BecomeEnemy(int enemy, bool fatal)
        {
            AdventureEnemy aE = new AdventureEnemy(Master.currentFile.bestiary[enemy], enemy);
            aE.location = location;
            parent.addObject(aE);
            if (fatal)
                active = false;
        }

        void SpawnShooter(int x, int y, bool inline)
        {
            AdventureShooter shoot = new AdventureShooter(inline);
            shoot.location = new Vector2(x * 32 + 16, y * 32 + 16);
            parent.addObject(shoot);
        }

        void ChangeFace(int face)
        {
            graphicsRow = face;
        }

        void ChangeDirection(int dir)
        {
            faceDir = (Master.Directions)dir;
        }

        int GetThisX()
        {
            return (int)Math.Floor(location.X / 32);
        }

        int GetThisY()
        {
            return (int)Math.Floor(location.Y / 32);
        }

        void Explode(bool fatal)
        {
            AdventureExplosion aE = new AdventureExplosion(location);
            parent.addObject(aE);
            if (fatal)
                active = false;
        }

        static Action<bool> getChooser(AdventureEntity ent, string callYes, string callNo)
        {
            return delegate (bool x)
            {
                if (x)
                    ent.jintEngine.Execute(callYes);
                else
                    ent.jintEngine.Execute(callNo);
            };
        }
    }
}
