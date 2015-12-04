# Aspect Star 2
## Introduction
*Aspect Star 2* is a new retro-styled action-adventure game, primarily but not solely taking its inspiration from games of the 8-bit console era. Particular effort is placed in providing not only a game but a game engine, along with editor, that can be taken up and used to make any sort of game. To that end, the game engine (but not the game assets themselves) are placed under the MIT open-source license. All game assets (except music) are released under the Creative Commons Attribution 4.0 license.

## Get the game
The game is now released for Windows. You can download it from the Release/ directory, or from [itch.io](http://nicole-express.itch.io/aspect-star-2). Note that with the music included, the game is released under the Creative Commons Attribution-NonCommercial-ShareAlike 4.0 license.

At the moment there are no builds for Linux or Mac OS X, however no Windows-specific code is used and releases using Mono on those platforms are planned. There are no plans for a mobile release.

## Technical details
*Aspect Star 2* is written in C# and uses the Monogame framework for controls, 2D graphics, sound effects, and music. Keyboard controls and gamepads which use XInput are supported for controlling the game. Until Monogame 3.5 is released, use of the Monogame development builds is recommended due to resolving some music issues. Additionally, [Jint](https://jint.codeplex.com/) is used to provide Javascript scripting capabilities for in-game objects and rooms.

The game engine uses the Monogame Content Pipeline to load graphics and sound, and loads level layouts, room data, specifics of enemy behavior, and Javascript scripts from an external XML "worldfile" produced by the editor. This file is produced and parsed using the built-in .NET XML serializer.
