What?
=====

This is primarily just a list of things I tend to forget when
I haven't worked on the engine in a while.  This can also be
useful for anyone who is intending to fork/modify the engine
OR to anyone who just wants to set it up properly.

Drawing Conventions
-------------------

In isometric mode, tiles are little diamonds.  It is very
important that (a) all the tiles are the same size, and
(b) you set the fields in the Tile class when you instantiate
the game.  The default values given correspond exclusively
to the tileset I've included (which is a fairly standard size!)
and must be changed for other sized tiles.

Tiles are drawn from their "upper left" corner; that is, where
x and y are minimized.  That means, assuming a diamond of
roughly the shape <>, we're talking about the LEFT corner.
This requires (since your texture will NOT have this as its
upper left corner) that the Tile.TileVisualOffsetX and
Tile.TileVisualOffsetY be set appropriately.