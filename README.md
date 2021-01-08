## Frost4-Toolkit
Requires .NET Framework 4.8

### Editor

NOTE: The main "Frost4" editor tool is being heavily reworked, the older source code was very unfunctional. The time to rework will vary but the goal is to push out a better tool that is dynamic and user-friendly.

~~Currently very BETA, current purposes are data analysis but the tool can be used to browse the ebx/res filesystem of Frostbite on PS4 specifically for Mirror's Edge Catalyst.~~

~~Requires .NET Framework 4.7.2~~

### Binary File Reader

Is able to view the contents of surface-level Frostbite 3 files (initfs, sb, toc, chunkmanifest, etc.) that utilizes Frostbite 3's Entry and Flag system to store information (does not include cas or cat).

### InitFS Cli

Can (un)pack the InitFS file located in certain (or all) Frostbite 3 games, was built with Mirror's Edge Catalyst in mind but may work on other games that utilize Frostbite 3.

NOTE: Being able to save changes doesn't mean the game will accept a modified InitFS, you may need to bypass any checks the game potentially has. Frostbite 3 modding tools like Frosty implement a patch that bypasses these checks; on consoles the InitFS can be modified without needing to bypass checks. Also, it is recommended to keep backups of the original InitFS and heavily modified files.

### Credits
 - Thanks mirrorsedgefan for creating the awesome Frost4 logo and icons for the tools.
 - [Frosty Tool Suite](https://github.com/GalaxyEham/FrostyToolSuite) for inspiration and work in the Frostbite 3 modding scene.
