# AppTiles
An application to help you remember important files or links you use semi-regularly.

Be even more scatterbrained than before!

![AppTiles example](https://raw.githubusercontent.com/Szune/AppTiles/3217b3e4ef40e91b1afc11770a7f469350f75743/Example.png)

## Available tile types
- App, which just opens:
  - a program (with arguments if specified)
  - a website (if a url is specified as path)
- Container, which contains even more tiles!
- Note, which is just a place to keep a small amount of information

## How to use
* Left click to perform the tile's action, whether it be opening its container, starting a program, etc.
* Right click to change the type of the tile, edit its layout or reset it, etc.
* Press F6 on the main AppTiles window to save (or save when closing AppTiles).
* Notes can be saved by pressing ctrl + enter.
* If you happen to remove the "Settings" button, simply create a new app tile with the path set to "{AppSettings}" (without the quotes).
* Using "{AppFolder}" (without the quotes) inside a path will replace that part with the folder that you ran AppTiles in.
* To quickly set the path of an app tile, you can drag a file from the file explorer onto a tile.
* When editing an app tile, you can specify command line arguments that will always be used when executing the program, and/or check "is taking input" (under Advanced) if you need to specify different arguments every time.

## TODO
Add search functionality