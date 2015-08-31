using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace aspectstar2
{
    public class AdventureEnemy : AdventureObject
    {
        protected BestiaryEntry definition;
        protected int health;
        protected int flickerCount;
        protected bool ghost;

        public AdventureEnemy()
        {
        }

        public AdventureEnemy(BestiaryEntry definition)
        {
            this.definition = definition;
            this.texture = Master.texCollection.texEnemies;
            this.health = definition.health;
            this.graphicsRow = definition.graphicsRow;
            this.offset = new Vector2(definition.xOffset, definition.yOffset);
            this.width = definition.width;
            this.height = definition.height;
            this.ghost = definition.ghost;
        }

        public override void Update()
        {
            switch (definition.movementType)
            {
                default:
                    // Stationary
                    break;
                case BestiaryEntry.MovementTypes.random:
                    if (stallCount % (definition.speed) != 0)
                    {
                        if (Master.globalRandom.Next(0, 10) > definition.decisiveness)
                            faceDir = (Master.Directions)Master.globalRandom.Next(0, 4);
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
                    break;
                case BestiaryEntry.MovementTypes.intelligent:
                    if (stallCount % (definition.speed) != 0)
                    {
                        if (Master.globalRandom.Next(0, 10) > definition.decisiveness)
                            if (Master.globalRandom.Next(0, 10) < definition.intelligence)
                            {
                                // Code from Aspect Star 1
                                int x_tar = (int)(location.X - parent.player.location.X);
                                int y_tar = (int)(location.Y - parent.player.location.Y);
                                Master.Directions targetDir;
                                if (Math.Abs(x_tar) > Math.Abs(y_tar))
                                {
                                    if (x_tar > 0)
                                        targetDir = Master.Directions.Left;
                                    else
                                        targetDir = Master.Directions.Right;
                                }
                                else
                                {
                                    if (y_tar > 0)
                                        targetDir = Master.Directions.Up;
                                    else
                                        targetDir = Master.Directions.Down;
                                }
                                this.faceDir = targetDir;
                            }
                            else
                            {
                                /*Master.Directions newDir = (Master.Directions)Master.globalRandom.Next(0, 3);
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
                                faceDir = newDir;*/
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
                    break;
            }

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            bool drawGhost = true;

            if (flickerCount > 0)
            {
                flickerCount--;
                if (flickerCount % 7 == 0)
                    mask = Color.Red;
                drawGhost = false;
            }

            if (ghost && Master.globalRandom.Next(0, 35) == 9)
                flickerCount = 6;

            if (!ghost || drawGhost)
                base.Draw(spriteBatch, mask);
        }

        public virtual void Hurt(bool ghostly)
        {
            if (definition.dependent != "" && !parent.GetFlag(definition.dependent))
                return; // Flag "dependent" must be on

            if (ghostly == ghost && this.flickerCount == 0)
            {
                PlaySound.Boom();
                health = health - 1;
                flickerCount = 40;
                if (health == 0)
                {
                    active = false;
                    parent.addObject(new AdventureExplosion(this.location));
                    if (Master.globalRandom.Next(0, 10) <= 2)
                    {
                        AdventureItem aI = game.getRandomItem();
                        aI.location = location;
                        parent.addObject(aI);
                    }
                }
            }
        }

        public override bool inRange(AdventurePlayer player)
        {
            Vector2 playerloc = player.location;

            if (Math.Sqrt( Math.Pow(location.X - playerloc.X, 2) + Math.Pow(location.Y - playerloc.Y, 2) ) <= Math.Max(width, height))
            {
                player.Hurt();
                player.Recoil(location);
                return true;
            }
            return false;
        }

        public override void Touch()
        {
            // Enemies are defined by their dependence on inRange
        }
    }
}
