Infinite Isometric Tile Engine
==============================

What?
-----

This project aims to be a re-usable tile
engine for any game that wants to live in
a 2D isometric perspective.

Since that's too easy to be worth establishing
a new project for (in fact there is an
excellent tutorial, complete with usable sample
code, at [XNA Resources][xnaresources]), I'm going to take
the engine a step up, and make it infinite
and procedurally generated.

[xnaresources]: http://www.xnaresources.com/default.asp?page=Tutorial:TileEngineSeries:4
	
Currently that doesn't mean a whole lot,
but you can wander in every direction forever,
over a persistent (but pretty boring) world.

How?
----

This project is written in C# using the XNA
framework.  To add it to your project, just
grab the source, get it noticed by VisualStudio
(however you like), and add a reference to the
Tile Engine project in whatever you're doing.

You'll be able to freely use the existing classes,
and the methods for doing that should be made
fairly clear by the demo project, packaged with
the engine itself.

Status?
-------

Currently the world allows for unlimited scrolling
with persistence and no repeating (until you
overflow your integers, so it's actually a huge
torus).

The tiles are all at the same elevation
and so forth, but visually, they choose from one
of several pre-drawn isometric tiles, drawn from
[OpenGameArt][oga].

[oga]: http://opengameart.org/content/isometric-64x64-outside-tileset