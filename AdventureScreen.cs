using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Jint;

namespace aspectstar2
{
    public class AdventureScreen : Screen
    {
        Game game;

        public bool fromMap;
        public bool beaten;

        public int tileset;

        public AdventurePlayer player;
        List<AdventureObject> objects = new List<AdventureObject>();
        List<AdventureObject> newobjects = new List<AdventureObject>();
        Adventure adventure;
        int roomX, roomY;
        Vector2 first_pos;
        int[] tileMap, key;
        Engine jintEngine;

        public int Keys = 0;

        int animCount, stallCount, lag;
        string textString;
        adventureModes currentMode = adventureModes.runMode;
        enum adventureModes
        {
            runMode,
            drowning,
            scrollingX,
            scrollingY,
            deathFade,
            fadeOut,
            textBox,
        }

        enum tileType
        {
            Clear = 0,
            Solid = 1,
            Crevasse = 2,
            Injury = 3,
            Warp = 4,
            Heal = 5,
            Lock = 6
        }

        public AdventureScreen(Game game, int dest, int destroomX, int destroomY, int x, int y, bool beaten)
        {
            this.game = game;
            this.master = game.master;
            this.player = new AdventurePlayer(this, game);

            this.adventure = Master.currentFile.adventures[dest].Clone();
            this.tileset = adventure.tileset;
            this.key = adventure.key;
            this.beaten = beaten;
            LoadRoom(destroomX, destroomY);

            this.first_pos = new Vector2(x * 32, y * 32);
            player.location = new Vector2(x * 32, y * 32);
        }

        public bool isSolid(Vector2 dest, int z, int width, int height, Master.Directions faceDir)
        {
            // Checks for the solidity of a rectangle; width and height are measured from the center of the rectangle

            int i, x, y;

            if ((faceDir == Master.Directions.Up) || (faceDir == Master.Directions.Left))
            {
                x = (int)Math.Floor((dest.X - width) / 32);
                y = (int)Math.Floor((dest.Y - height) / 32);
                i = x + (y * 25);
                if (((tileType)key[tileMap[i]] == tileType.Solid) || ((tileType)key[tileMap[i]] == tileType.Lock) ||
                    (((tileType)key[tileMap[i]] == tileType.Crevasse) && z == 0))
                    return true;
            }

            if ((faceDir == Master.Directions.Up) || (faceDir == Master.Directions.Right))
            {
                x = (int)Math.Floor((dest.X + width) / 32);
                y = (int)Math.Floor((dest.Y - height) / 32);
                i = x + (y * 25);
                if (((tileType)key[tileMap[i]] == tileType.Solid) || ((tileType)key[tileMap[i]] == tileType.Lock) ||
                    (((tileType)key[tileMap[i]] == tileType.Crevasse) && z == 0))
                    return true;
            }

            if ((faceDir == Master.Directions.Down) || (faceDir == Master.Directions.Left))
            {
                x = (int)Math.Floor((dest.X - width) / 32);
                y = (int)Math.Floor((dest.Y + height) / 32);
                i = x + (y * 25);
                if (((tileType)key[tileMap[i]] == tileType.Solid) || ((tileType)key[tileMap[i]] == tileType.Lock) ||
                    (((tileType)key[tileMap[i]] == tileType.Crevasse) && z == 0))
                    return true;
            }

            if ((faceDir == Master.Directions.Down) || (faceDir == Master.Directions.Right))
            {
                x = (int)Math.Floor((dest.X + width) / 32);
                y = (int)Math.Floor((dest.Y + height) / 32);
                i = x + (y * 25);
                if (((tileType)key[tileMap[i]] == tileType.Solid) || ((tileType)key[tileMap[i]] == tileType.Lock) ||
                    (((tileType)key[tileMap[i]] == tileType.Crevasse) && z == 0))
                    return true;
            }

            return false;
        }

        public void tileAction(Vector2 dest, int width, int height)
        {
            int i, x, y;
            //Center
            x = (int)Math.Floor((dest.X) / 32);
            y = (int)Math.Floor((dest.Y) / 32);
            i = x + (y * 25);
            tileStepAction(i);

            //Top Left
            x = (int)Math.Floor((dest.X - width) / 32);
            y = (int)Math.Floor((dest.Y - height) / 32);
            i = x + (y * 25);
            tileTouchAction(i);

            // Bottom Left
            x = (int)Math.Floor((dest.X + width) / 32);
            y = (int)Math.Floor((dest.Y - height) / 32);
            i = x + (y * 25);
            tileTouchAction(i);

            // Top Right
            x = (int)Math.Floor((dest.X - width) / 32);
            y = (int)Math.Floor((dest.Y + height) / 32);
            i = x + (y * 25);
            tileTouchAction(i);

            // Bottom Right
            x = (int)Math.Floor((dest.X + width) / 32);
            y = (int)Math.Floor((dest.Y + height) / 32);
            i = x + (y * 25);
            tileTouchAction(i);
        }

        void tileStepAction(int i)
        {
            tileType tile = (tileType)key[tileMap[i]];
            if (tile == tileType.Warp)
            {
                PlaySound.Leave();
                currentMode = adventureModes.fadeOut;
                animCount = 250;
            }
            else if (tile == tileType.Heal)
            {
                if (game.life != game.possibleLife)
                {
                    game.life = game.possibleLife;
                    PlaySound.Aspect();
                    tileMap[i] = 0;
                }
            }
        }

        void tileTouchAction(int i)
        {
            tileType tile = (tileType)key[tileMap[i]];
            if (tile == tileType.Lock)
            {
                if (Keys > 0)
                {
                    Keys--;
                    PlaySound.Aspect();
                    tileMap[i] = 0;
                }
            }

            int x, y, j;
            foreach (AdventureObject obj in objects)
            {
                x = (int)Math.Floor(obj.location.X / 32);
                y = (int)Math.Floor(obj.location.Y / 32);
                j = x + (y * 25);
                if (j == i)
                    obj.Touch();
            }
        }

        public bool Collide()
        {
            bool collided = false;
            foreach (AdventureObject obj in objects)
            {
                if (obj.inRange(player))
                    collided = true;
            }
            return collided;
        }

        public bool isInjury(Vector2 dest, int width, int height)
        {
            // Checks for the solidity of a rectangle; width and height are measured from the center of the rectangle

            int i, x, y;
            //Top Left
            x = (int)Math.Floor((dest.X - width) / 32);
            y = (int)Math.Floor((dest.Y - height) / 32);
            i = x + (y * 25);
            if ((tileType)key[tileMap[i]] == tileType.Injury)
                return true;

            // Bottom Left
            x = (int)Math.Floor((dest.X + width) / 32);
            y = (int)Math.Floor((dest.Y - height) / 32);
            i = x + (y * 25);
            if ((tileType)key[tileMap[i]] == tileType.Injury)
                return true;

            // Top Right
            x = (int)Math.Floor((dest.X - width) / 32);
            y = (int)Math.Floor((dest.Y + height) / 32);
            i = x + (y * 25);
            if ((tileType)key[tileMap[i]] == tileType.Injury)
                return true;


            // Bottom Right
            x = (int)Math.Floor((dest.X + width) / 32);
            y = (int)Math.Floor((dest.Y + height) / 32);
            i = x + (y * 25);
            if ((tileType)key[tileMap[i]] == tileType.Injury)
                return true;

            return false;
        }

        public void Drown()
        {
            animCount = 24;
            currentMode = adventureModes.drowning;
            PlaySound.Drown();
            //objects.RemoveAll(isProjectile);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            switch (currentMode)
            {
                case adventureModes.textBox:
                    DrawRoom(spriteBatch, Color.White);
                    List<AdventureObject> sortList = objects.OrderBy(o => o.location.Y).ToList();
                    foreach (AdventureObject obj in sortList)
                    {
                        obj.Draw(spriteBatch, Color.White);
                    }
                    DrawTextBox(spriteBatch, textString);
                    break;
                case adventureModes.drowning:
                    if (animCount == 0)
                    {
                        DrawRoom(spriteBatch, Color.White);
                        drawStatus(spriteBatch);
                        if (game.life > 2)
                        {
                            game.life = game.life - 2;
                            player.location = first_pos;
                            player.Flicker();
                            currentMode = adventureModes.runMode;
                        }
                        else
                        {
                            game.life = 0;
                            Die();
                        }
                        return;
                    }

                    DrawRoom(spriteBatch, Color.White);
                    player.Drown(spriteBatch, animCount);

                    if (stallCount > 0)
                    {
                        stallCount = stallCount - 1;
                    }
                    else
                    {
                        stallCount = 3;
                        animCount--;
                    }
                    break;
                case adventureModes.deathFade:
                    Color mask = Color.White;
                    mask.G = (byte)(animCount / 2);
                    mask.B = (byte)(animCount / 2);
                    mask.R = (byte)(animCount);
                    DrawRoom(spriteBatch, mask);
                    break;
                case adventureModes.fadeOut:
                    Color mask2 = Color.White;
                    mask2.R = (byte)(animCount / 2);
                    mask2.G = (byte)(animCount / 2);
                    mask2.B = (byte)(animCount / 2);
                    DrawRoom(spriteBatch, mask2);
                    player.Draw(spriteBatch, mask2);
                    break;
                case adventureModes.scrollingX:
                case adventureModes.scrollingY:
                    scroll(spriteBatch);
                    break;
                case adventureModes.runMode:
                    DrawRoom(spriteBatch, Color.White);
                    //player.Draw(spriteBatch, Color.White);
                    List<AdventureObject> sortedList = objects.OrderBy(o => o.location.Y).ToList();
                    foreach (AdventureObject obj in sortedList)
                    {
                        obj.Draw(spriteBatch, Color.White);
                    }
                    break;
            }
            drawStatus(spriteBatch);
        }

        public void DrawRoom(SpriteBatch spriteBatch, Color mask)
        {
            int x, y;
            Rectangle source, dest;
            Vector2 sourceTile;
            spriteBatch.Begin();
            for (int i = 0; i < (tileMap.Length); i++)
            {
                x = i % 25;
                y = i / 25;
                sourceTile = Master.getMapTile(tileMap[i], Master.texCollection.adventureTiles[adventure.tileset]);
                source = new Rectangle((int)sourceTile.X, (int)sourceTile.Y, 32, 32);
                dest = new Rectangle(x * 32, y * 32, 32, 32);
                spriteBatch.Draw(Master.texCollection.adventureTiles[adventure.tileset], dest, source, mask);
            }
            spriteBatch.End();
        }

        public void drawStatus(SpriteBatch spriteBatch)
        {
            int x, y;
            Rectangle source, dest;
            spriteBatch.Begin();

            // Controls icons
            x = 18;
            y = 13;
            source = new Rectangle(0, 0, 64, 64);
            dest = new Rectangle(x * 32, y * 32, 64, 64);
            spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.White);

            x = 21;
            y = 13;
            source = new Rectangle(64, 0, 64, 64);
            dest = new Rectangle(x * 32, y * 32, 64, 64);
            spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.White);

            // A icon
            //source = new Rectangle(0, 64, 32, 32);
            //dest = new Rectangle((int)(18.5 * 32), (int)(13.5 * 32), 32, 32);
            //spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.White);


            // Hearts
            int hearts = (int)Math.Ceiling((double)game.possibleLife / 2);
            for (int i = 1; i <= hearts; i++)
            {
                if (game.life == (i * 2 - 1))
                    source = new Rectangle((128 + 16), 0, 16, 16);
                else if ((i * 2) > game.life)
                    source = new Rectangle((128 + 32), 0, 16, 16);
                else
                    source = new Rectangle(128, 0, 16, 16);
                dest = new Rectangle((int)(1 * 32 + (i * 16)), (int)(13 * 32 + 16), 16, 16);
                spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.White);
            }

            // Keys
            source = new Rectangle((128 + 48), 0, 16, 16);
            dest = new Rectangle((int)(10 * 32), (int)(13 * 32 + 16), 16, 16);
            spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.White);
            source = new Rectangle((128 + 64), 0, 16, 16);
            dest = new Rectangle((int)(10 * 32), (int)(13 * 32 + 32), 16, 16);
            spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.White);
            spriteBatch.End();

            // Name
            WriteText(spriteBatch, adventure.name, new Vector2(48, 14 * 32), Color.White);

            // Key counts
            WriteText(spriteBatch, game.goldKeys.ToString(), new Vector2(10 * 32 + 16, 13 * 32 + 16), Color.White);
            WriteText(spriteBatch, Keys.ToString(), new Vector2(10 * 32 + 16, 14 * 32), Color.White);

            // Weapons
            game.weaponA.Draw(spriteBatch, (int)(18.5 * 32), (int)(13.5 * 32));
            game.weaponB.Draw(spriteBatch, (int)(21.5 * 32), (int)(13.5 * 32));
        }

        void scroll(SpriteBatch spriteBatch)
        {
            bool scrollDown = true;
            if (Math.Sign(animCount) == -1)
                scrollDown = false;

            bool Xscroll = false;
            if (currentMode == adventureModes.scrollingX)
                Xscroll = true;

            Vector2 screenOffset;
            Rectangle source;
            Rectangle dest;
            Vector2 sourceTile, limitOffset;
            int x, y;
            spriteBatch.Begin();

            int width, height;

            if (Xscroll)
            {
                width = 50;
                height = 13;
                screenOffset.Y = 0;
                if (scrollDown)
                    screenOffset.X = ((width / 2) - animCount) * 32;
                else
                    screenOffset.X = -(animCount) * 32;
            }
            else
            {
                width = 25;
                height = 26;
                screenOffset.X = 0;
                if (scrollDown)
                    screenOffset.Y = ((height / 2) - animCount) * 32;
                else
                    screenOffset.Y = -(animCount) * 32;
            }

            int[] newroom;
            int[] oldroom;
            int[] room = new int[width * height];

            if (Xscroll)
            {
                newroom = adventure.rooms[roomX, roomY].tileMap;
                oldroom = adventure.rooms[roomX - Math.Sign(animCount), roomY].tileMap;
                if (scrollDown)
                {
                    room = Master.sumRooms(oldroom, newroom, 25, 13);
                }
                else
                {
                    room = Master.sumRooms(newroom, oldroom, 25, 13);
                }
            }
            else
            {
                newroom = adventure.rooms[roomX, roomY].tileMap;
                oldroom = adventure.rooms[roomX, roomY - Math.Sign(animCount)].tileMap;
                if (scrollDown)
                {
                    oldroom.CopyTo(room, 0);
                    newroom.CopyTo(room, 13 * 25);
                }
                else
                {
                    oldroom.CopyTo(room, 13 * 25);
                    newroom.CopyTo(room, 0);
                }
            }

            for (int i = 0; i < (width * height); i++)
            {
                x = i % width;
                y = i / width;
                limitOffset = new Vector2((float)Math.Floor(screenOffset.X / 32), (float)Math.Floor(screenOffset.Y / 32));
                if ((x >= limitOffset.X) && (x <= limitOffset.X + Master.width) &&
                    (y >= limitOffset.Y) && (y <= limitOffset.Y + (Master.height)))
                {
                    sourceTile = Master.getMapTile(room[i], Master.texCollection.adventureTiles[adventure.tileset]);
                    source = new Rectangle((int)sourceTile.X, (int)sourceTile.Y, 32, 32);
                    dest = new Rectangle(x * 32 - (int)screenOffset.X, y * 32 - (int)screenOffset.Y, 32, 32);
                    if (y - (int)limitOffset.Y < 13)
                        spriteBatch.Draw(Master.texCollection.adventureTiles[adventure.tileset], dest, source, Color.White);
                }
            }
            spriteBatch.End();


            if (animCount == 0)
            {
                currentMode = adventureModes.runMode;
                stallCount = 0;
                return;
            }

            if (stallCount > 0)
            {
                stallCount = stallCount - 1;
            }
            else
            {
                if (Xscroll)
                    stallCount = 2;
                else
                    stallCount = 3;
                animCount = (Math.Abs(animCount) - 1) * Math.Sign(animCount);
            }
        }

        public void enterNewRoom(int del_x, int del_y)
        {
            int x = roomX + del_x;
            int y = roomY + del_y;

            if (x >= 0 && y >= 0 && x < 16 && y < 16)
                if (adventure.rooms[x, y] != null)
                {
                    if (del_x == -1)
                        player.location.X = 25 * 32 - 14 - 2;
                    else if (del_y == -1)
                        player.location.Y = 13 * 32 - 6 - 2;
                    else if (del_x == 1)
                        player.location.X = 14 + 2;
                    else if (del_y == 1)
                        player.location.Y = 6 + 2;
                    this.first_pos = player.location;

                    LoadRoom(x, y);

                    if (del_y != 0)
                    {
                        currentMode = adventureModes.scrollingY;
                        animCount = 12 * del_y;
                    }
                    else if (del_x != 0)
                    {
                        currentMode = adventureModes.scrollingX;
                        animCount = 24 * del_x;
                    }
                }

        }

        public void LoadRoom(int x, int y)
        {
            roomX = x; roomY = y;
            Room newRoom = adventure.rooms[x, y];
            this.tileMap = newRoom.tileMap;
            this.objects = new List<AdventureObject>();
            this.newobjects = new List<AdventureObject>(); 
            objects.Add(player);
            foreach (AdventureObject aO in newRoom.adventureObjects)
            {
                aO.Initialize(this, game);
                objects.Add(aO);
            }

            // Run room code
            ActivateScript();
        }

        public override void Update(GameTime gameTime)
        {
            switch (currentMode)
            {
                case adventureModes.textBox:
                    KeyboardState state = Keyboard.GetState();
                    if (state.IsKeyDown(Master.controls.A) || state.IsKeyDown(Master.controls.B))
                        currentMode = adventureModes.runMode;
                    break;
                case adventureModes.fadeOut:
                    animCount = animCount - 2;
                    if (animCount <= 0)
                        game.exitAdventure(beaten);
                    break;
                case adventureModes.deathFade:
                    animCount = animCount - 3;
                    if (animCount <= 0)
                        game.exitAdventure(beaten);
                    break;
                case adventureModes.runMode:
                    UpdateRoom();
                    break;
                default:
                    break;
            }
        }

        public void UpdateRoom()
        {
            if (jintEngine != null)
            {
                jintEngine.Execute(String.Concat(
                    "playerX = ", Math.Floor(player.location.X / 32),
                    "; playerY = ", Math.Floor(player.location.Y / 32)));
                jintEngine.Execute("update();");
            }

            // Update
            foreach (AdventureObject obj in this.objects)
            {
                obj.Update();
            }
            objects.RemoveAll(isInactive);
            objects = objects.Concat(newobjects).ToList();
            newobjects = new List<AdventureObject>();

            if (currentMode == adventureModes.drowning)
                objects.RemoveAll(isProjectile);

            foreach (AdventureProjectile proj in this.objects.Where(isProjectile))
            {
                proj.projectileUpdate(objects);
            }

            // Get keyboard input
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Master.controls.Up))
            {
                player.moving = true;
                player.faceDir = Master.Directions.Up;
            }
            else if (state.IsKeyDown(Master.controls.Down))
            {
                player.moving = true;
                player.faceDir = Master.Directions.Down;
            }
            else if (state.IsKeyDown(Master.controls.Left))
            {
                player.moving = true;
                player.faceDir = Master.Directions.Left;
            }
            else if (state.IsKeyDown(Master.controls.Right))
            {
                player.moving = true;
                player.faceDir = Master.Directions.Right;
            }
            else
            {
                player.moving = false;
            }

            if (state.IsKeyDown(Master.controls.A))
                game.weaponA.Activate(player, this);
            else if (state.IsKeyDown(Master.controls.B))
                game.weaponB.Activate(player, this);
            else if (state.IsKeyDown(Master.controls.Start) && lag == 0)
            {
                game.Pause();
                lag = 20;
            }
            if (lag != 0)
                lag = lag - 1;

            game.weaponA.Update();
            game.weaponB.Update();
        }

        public void Die()
        {
            animCount = 250;
            currentMode = adventureModes.deathFade;
            PlaySound.Die();
        }

        public void addObject(AdventureObject obj)
        {
            obj.Initialize(this, game);
            newobjects.Add(obj);
        }

        void DrawTextBox(SpriteBatch spriteBatch, string text)
        {
            int height = 4 * 32 + 16;
            spriteBatch.Begin();
            spriteBatch.Draw(Master.texCollection.blank, new Rectangle(0, 0, Master.width, height), Color.Black);
            spriteBatch.End();

            text = text.ToUpper();
            int tiles = 46; // (int)Math.Floor((double)(Master.width - 64) / 16);
            Debug.WriteLine(tiles);
            bool moreText = true; int i = 0; string dummy;
            while (moreText)
            {
                if ((text.Length - (i * tiles)) >= tiles)
                    dummy = text.Substring(i * tiles, tiles);
                else
                {
                    dummy = text.Substring(i * tiles, text.Length - (i * tiles));
                    moreText = false;
                }
                WriteText(spriteBatch, dummy, new Vector2(32, 16 + (i * 32)), Color.White);

                i++;
            }
        }

        public Engine ActivateEngine(string code)
        {
            return new Engine(cfg => cfg.AllowClr())
                .SetValue("spawnKey", new Action<int, int>(this.SpawnKey))
                .SetValue("getFlag", new Func<string, bool>(this.GetFlag))
                .SetValue("setFlag", new Action<string, bool>(this.SetFlag))
                .SetValue("overwriteTile", new Action<int, int, int>(this.OverwriteTile))
                .SetValue("playSoundEffect", new Action<int>(this.PlaySoundEffect))
                .SetValue("anyEnemies", new Func<bool>(this.AnyEnemies))
                .SetValue("clearObjects", new Action(this.ClearObjects))
                .SetValue("TextBox", new Action<string>(this.TextBox))
                .SetValue("callMethod", new Action<string,string>(this.CallMethod))
                .SetValue("giveWeapon", new Action<int>(this.GiveWeapon))
                .Execute(code);
        }

        void ActivateScript()
        {
            string code = adventure.rooms[roomX, roomY].code;
            if (code != null)
            {
                jintEngine = ActivateEngine(code);

                jintEngine.Execute("onLoad()");
            }
        }

        // Relevant to code

        void SpawnKey(int x, int y)
        {
            AdventureKey aK = new AdventureKey();
            aK.Initialize(this, game);
            aK.location.X = 32 * x + 16;
            aK.location.Y = 32 * y + 16;
            newobjects.Add(aK);
            adventure.rooms[roomX, roomY].adventureObjects.Add(aK); // So that the key respawns even if you leave the room, but not the adventure
            PlaySound.Aspect();
            
        }

        void OverwriteTile(int x, int y, int newTile)
        {
            int i = x + (y * 25);
            tileMap[i] = newTile;
            first_pos = new Vector2(player.location.X, player.location.Y);
        }

        void PlaySoundEffect(int i)
        {
            switch (i)
            {
                case 0:
                    PlaySound.Aspect();
                    break;
                case 1:
                    PlaySound.Boom();
                    break;
            }
        }

        bool AnyEnemies()
        {
            foreach (AdventureObject aO in objects)
            {
                if (aO is AdventureEnemy)
                    return true;
            }
            return false;
        }

        void ClearObjects()
        {
            foreach (AdventureObject obj in objects)
            {
                if (!(obj is AdventurePlayer) && !(obj is AdventureExplosion))
                    obj.active = false;
                if ((obj is AdventureEnemy) || (obj is AdventureShooter))
                {
                    addObject(new AdventureExplosion(obj.location));
                }
            }
        }

        public void TextBox(string text)
        {
            textString = text;
            currentMode = adventureModes.textBox;
        }

        void CallMethod(string name, string message)
        {
            AdventureEntity ent;

            foreach (AdventureObject obj in objects)
                if (obj is AdventureEntity)
                {
                    ent = (AdventureEntity)obj;
                    if (ent.name == name)
                        ent.Execute(message);
                }
        }

        void GiveWeapon(int weapon)
        {
            switch (weapon)
            {
                case 0:
                    game.GetWeapon(new ProjectileWeapon());
                    break;
            }
        }

        Dictionary<string, bool> flags = new Dictionary<string, bool>();

        void SetFlag(string flag, bool value)
        {
            if (flags.ContainsKey(flag))
                flags.Remove(flag);
            flags.Add(flag, value);
        }

        bool GetFlag(string flag)
        {
            if (flags.ContainsKey(flag))
                return flags[flag];
            return false;
        }

        // Predicates

        public static bool isInactive(AdventureObject obj)
        {
            return !(obj.active);
        }

        public static bool isProjectile(AdventureObject proj)
        {
            if (proj is AdventureProjectile)
                return true;
            else
                return false;
        }
    }
}
