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
the engine a step up, and make it infinite.

[xnaresources]: http://www.xnaresources.com/default.asp?page=Tutorial:TileEngineSeries:4
	
Currently that doesn't mean a whole lot,
but you can wander in every direction forever,
over a persistent (but pretty boring) world.

Usage-wise, a main design goal for the engine is that
as a _coder_, you can safely ignore the isometric
perspective of the world.  Instead, design for a square
world, with cartesian blocks, and witness the tilted
perspective just sort of emerge.

Obviously the artist will not have this same experience!

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

If you want to use procedural generation (which
this is designed for), just extend the TileMap
class, and in particular, override the MakeMapCell(x,y)
method to make MapCells of your own design.

Status?
-------

Currently the world allows for unlimited scrolling
with persistence and no repeating (until you
overflow your integers, so it's technically a huge
torus, but that's farther than players should
reasonably go!).

There is support for caching, and both procedural
generation and manual creation can happily coexist.

There is support for mouse action (that is, the engine
automatically tracks the most recently moused-over
square) so the user won't have to.

There is support for in-game objects (with a focus on NPCs).

Sample Code?
------------

Since this is an engine, it seems useful to attach a
toy project which uses the features in a minimal way,
to expose the functionality while remaining small.  With this
in mind, a small demo is attached which gives an example
of manually created content.

Currently, the demo world is an infinite green field
(procedural generation not used much) with a few hand-made
houses and a few random-walking NPCs.

The tiles used in the demo (except the solid color
tiles) were made by Seth Galbraith, and are available
at [OpenGameArt][oga2].  They are intended to be used
in conjunction with [Yar's work][oga1], which was used
in previous versions of this project, but is not in
the current build.

A few (solid color) tiles, as well as the line-art
person, were drawn by me, and are licensed under the
same license as the rest of the project.

[oga1]: http://opengameart.org/content/isometric-64x64-outside-tileset
[oga2]: http://opengameart.org/content/isometric-64x64-medieval-building-tileset

Copyright?
----------

Excepting art assets, this work was done by Richard
Rast, drawing on tutorials to get started and vague
ideas (such as infinite worlds) which I implemented
myself but did not invent.

The code is released under the [CC-BY-NC-3.0][ccbync3]
license, which means you can do whatever you want with it,
so long as you don't sell it.  I don't particularly mind
if you mess up the attribution, either.  If you _do_ want to
sell a derivative work, you'll need to talk to me first.

[ccbync3]:http://creativecommons.org/licenses/by-nc/3.0/

The art assets are licensed separately and are property
of their original creator, as noted above.  They
are both released under the [CC-BY-3.0][ccby3] license,
which means you're free to do whatever with them, so
long as attribution is maintained.

[ccby3]:http://creativecommons.org/licenses/by/3.0/