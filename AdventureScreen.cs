﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace aspectstar2
{
    class AdventureScreen : Screen
    {
        Game game;

        AdventureObject player;
        List<AdventureObject> objects = new List<AdventureObject>();
        Adventure adventure;
        int roomX, roomY;
        int[] tileMap, key;

        int animCount, stallCount;
        adventureModes currentMode = adventureModes.runMode;
        enum adventureModes
        {
            runMode,
            scrollingX,
            scrollingY
        }

        enum tileType
        {
            Clear = 0,
            Solid = 1,
            Crevasse = 2
        }

        public AdventureScreen(Game game, int dest, int destroomX, int destroomY, int x, int y)
        {
            this.game = game;
            this.master = game.master;
            this.player = new AdventurePlayer(this);

            this.adventure = Master.currentFile.adventures[dest];
            this.key = Master.currentFile.adventures[dest].key;
            this.roomX = destroomX;
            this.roomY = destroomY;
            this.tileMap = Master.currentFile.adventures[dest].rooms[roomX, roomY].tileMap;

            objects.Add(this.player);
            player.location = new Vector2(x * 32, y * 32);
        }

        public bool isSolid(Vector2 dest, int z, int width, int height)
        {
            // Checks for the solidity of a rectangle; width and height are measured from the center of the rectangle
            
            int i, x, y;
            //Top Left
            x = (int)Math.Floor((dest.X - width) / 32);
            y = (int)Math.Floor((dest.Y - height) / 32);
            i = x + (y * 25);
            if (((tileType)key[tileMap[i]] == tileType.Solid) || (((tileType)key[tileMap[i]] == tileType.Crevasse) && z == 0))
                return true;

            // Bottom Left
            x = (int)Math.Floor((dest.X + width) / 32);
            y = (int)Math.Floor((dest.Y - height) / 32);
            i = x + (y * 25);
            if (((tileType)key[tileMap[i]] == tileType.Solid) || (((tileType)key[tileMap[i]] == tileType.Crevasse) && z == 0))
                return true;

            // Top Right
            x = (int)Math.Floor((dest.X - width) / 32);
            y = (int)Math.Floor((dest.Y + height) / 32);
            i = x + (y * 25);
            if (((tileType)key[tileMap[i]] == tileType.Solid) || (((tileType)key[tileMap[i]] == tileType.Crevasse) && z == 0))
                return true;


            // Bottom Right
            x = (int)Math.Floor((dest.X + width) / 32);
            y = (int)Math.Floor((dest.Y + height) / 32);
            i = x + (y * 25);
            if (((tileType)key[tileMap[i]] == tileType.Solid) || (((tileType)key[tileMap[i]] == tileType.Crevasse) && z == 0))
                return true;

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            switch (currentMode)
            {
                case adventureModes.scrollingX:
                case adventureModes.scrollingY:
                    scroll(spriteBatch);
                    break;
                case adventureModes.runMode:
                    DrawRoom(spriteBatch);
                    foreach (AdventureObject obj in this.objects)
                    {
                        obj.Draw(spriteBatch, Color.White);
                    }
                    break;
            }
        }

        void DrawRoom(SpriteBatch spriteBatch)
        {
            int x, y;
            Rectangle source, dest;
            Vector2 sourceTile;
            spriteBatch.Begin();
            for (int i = 0; i < (tileMap.Length); i++)
            {
                x = i % 25;
                y = i / 25;
                sourceTile = Master.getMapTile(tileMap[i], Master.texCollection.dungeonTiles);
                source = new Rectangle((int)sourceTile.X, (int)sourceTile.Y, 32, 32);
                dest = new Rectangle(x * 32, y * 32, 32, 32);
                spriteBatch.Draw(Master.texCollection.dungeonTiles, dest, source, Color.White);
            }
            spriteBatch.End();
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

            if (Xscroll)
                ;

            for (int i = 0; i < (width * height); i++)
            {
                x = i % width;
                y = i / width;
                limitOffset = new Vector2((float)Math.Floor(screenOffset.X / 32), (float)Math.Floor(screenOffset.Y / 32));
                if ((x >= limitOffset.X) && (x <= limitOffset.X + Master.width) &&
                    (y >= limitOffset.Y) && (y <= limitOffset.Y + (Master.height)))
                {
                    sourceTile = Master.getMapTile(room[i], Master.texCollection.dungeonTiles);
                    source = new Rectangle((int)sourceTile.X, (int)sourceTile.Y, 32, 32);
                    dest = new Rectangle(x * 32 - (int)screenOffset.X, y * 32 - (int)screenOffset.Y, 32, 32);
                    if (y - (int)limitOffset.Y < 13)
                        spriteBatch.Draw(Master.texCollection.dungeonTiles, dest, source, Color.White);
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
                if (adventure.rooms[x,y] != null)
                {
                    roomX = x;
                    roomY = y;
                    this.tileMap = adventure.rooms[x, y].tileMap;
                    if (del_x == -1)
                        player.location.X = 25 * 32 - 14;
                    else if (del_y == -1)
                        player.location.Y = 13 * 32 - 7;
                    else if (del_x == 1)
                        player.location.X = 14;
                    else if (del_y == 1)
                        player.location.Y = 7;

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

        public override void Update(GameTime gameTime)
        {
            switch (currentMode)
            {
                case adventureModes.runMode:
                    UpdateRoom();
                    break;
            }
        }

        public void UpdateRoom()
        {
            // Update
            foreach (AdventureObject obj in this.objects)
            {
                obj.Update();
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
                player.Jump();
        }
    }
}
