using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace aspectstar2
{
    public class AdventureEnemy : AdventureObject
    {
        BestiaryEntry definition;

        public AdventureEnemy(BestiaryEntry definition)
        {
            this.definition = definition;
        }

        public override void inRange(Vector2 playerloc)
        {
            // TODO: Attack the player
        }

        public override void Touch()
        {
            // Enemies are defined by their dependence on inRange
        }
    }
}
