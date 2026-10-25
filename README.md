# Little Elsewhere — HD / 24-hour edition

A cozy winter exploration game project with newly generated larger artwork, a high fixed overhead perspective inspired by `walking.jpg` and `camping.jpg`, a continuous 24-hour lighting cycle, three independent saves, player movement, and dedicated menu/loading/gameplay scenes.

## Open the project

1. Install **Unity Hub** and **Unity 6000.6.2f1**.
2. Extract the ZIP. In Unity Hub choose **Add project from disk** and select the `LittleElsewhere` folder containing `Assets`, `Packages`, and `ProjectSettings`.
3. Let Unity finish importing and compiling. No paid assets, additional input package, or Python setup is required to play.
4. Open **Assets/Scenes/MainMenu.unity**, or choose **Little Elsewhere → Open Game Scene**.
5. Press **Play**. Choose **New Walk**, select one of the three slots, and confirm if replacing an occupied slot. The Loading scene preloads artwork/audio and then opens gameplay.

Open `MainMenu.unity` as the normal entry point. Gameplay objects are created at runtime. The editor scene is intentionally a lightweight bootstrap rather than a fully populated forest. At 16:9, the camera shows a small player in a broad snowy clearing, matching the elevated viewpoint of your references. The camera is fixed, without orbit or zoom controls.

## Three real scenes

| Scene | Purpose |
| --- | --- |
| `Assets/Scenes/MainMenu.unity` | Title, new/load slot selection, settings, quit |
| `Assets/Scenes/Loading.unity` | Asynchronous resource loading, progress bar/percentage, tips, scene activation |
| `Assets/Scenes/LittleElsewhere.unity` | Player, forest, 24-hour clock, activities, pause/save menus, ending |

All three are included in build order, with MainMenu first. Starting and continuing a slot goes through Loading. Saving and returning from gameplay opens the actual MainMenu scene. The loading percentage tracks resource preload and scene loading; it is not a decorative fixed timer. Missing required artwork/audio shows an error and a return button.

## Controls and menus

| Key | Action |
| --- | --- |
| WASD / arrows | Walk; diagonal speed is normalized |
| Shift | Move faster |
| E | Interact with the object named on screen |
| M | Map |
| J / Tab | Journal |
| Escape | Pause / close a panel |
| Space | Catch a fish while its float crosses the blue band |
| F5 | Quick-save to the active slot |

Four facing directions use the newly generated four-frame walk cycles. Movement includes trunk/building collision, footsteps, footprints, camera follow, and continuous world wrapping. Mouse controls operate menus. Controller/touch input is not implemented.

The title includes Continue, New Walk, Settings, and Quit. Slot selection shows three separate slots with location, day/time, playtime, and saved timestamp. The pause menu includes Resume, Settings, Save Game (choose a slot), Save & Main Menu, and Save & Quit. Settings control music, sound, brightness, fullscreen, snowfall, and footprints.

## A full 24-hour day

The clock covers **00:00–23:59**, with dawn, day, dusk, and night. By default **one game day takes 24 real hours of active play** (`dayLengthSeconds = 86400`). It pauses in menus, dialogs, and when the application is suspended. The clock continues during walking and fishing. Saved games retain their own accumulated world time and day number.

To accelerate the cycle, select `Assets/Resources/GameConfig.asset` outside Play Mode and change **Day Length Seconds**:

| Value | One complete game day takes |
| --- | --- |
| `86400` | 24 real hours — default |
| `3600` | 1 real hour |
| `1440` | 24 real minutes |
| `120` | 2 minutes — useful for testing |

The campsite southwest of the cabin lets you rest and advance eight game hours. For immediate editor previews, start/load a walk and use **Little Elsewhere → Preview Lighting → Dawn / Noon / Dusk / Night**. This changes the active session's clock and pauses it; resume to inspect the world.

The artwork has neutral local lighting. Unity applies continuous palette changes to the snow, props, player, and wildlife. Separate sun and moon shadow renderers project the actual sprite silhouettes, changing direction, length, and opacity with time. Cabin, campfire, and lit lantern glows become more prominent at night. These are stylized 2D projected shadows, not 3D geometry-based ray tracing or normal-mapped relighting. Day phases blend continuously instead of swapping between a few painted frames.

## Higher-resolution artwork

The main gameplay artwork is **newly generated**, not enlarged crops from the old board. Eleven production sources are preserved in `Assets/ArtSources`, and 59 sliced resources in `Assets/Resources/HD` are preferred automatically at runtime. No upscaling was used. Exact delivered dimensions are recorded in `Documentation/HDAssetManifest.json`.

| Asset | Included raster size |
| --- | --- |
| Tall pine | 996 × 1470 px |
| Compact pine | 911 × 1053 px |
| Cabin | 1015 × 1124 px |
| Campsite | 974 × 760 px |
| Frozen pond | 1151 × 1145 px |
| Player animation | 314 × 314 px per cell; 16 walking cells plus four reused idle cells |
| Main menu background | 1672 × 941 px |
| Mirrored seamless snow | 2508 × 2508 px, assembled from a native 1254 px source |

These are the actual native/sliced dimensions, not a claim that every sprite is a native 4K image. Small props and wildlife come from separate larger atlases; see the manifest for each. The project can render at high display resolutions, while the camera scale keeps gameplay subjects much smaller than their source images. The retained legacy board crops supply small paper/effect graphics and remain available for reference.

The image-generation prompt set and method are included in `Documentation/GenerationPrompts.json`. Requested prompt sizes sometimes differ from the tool's delivered sizes; the dimension manifest is authoritative. The original four reference images are preserved. The optional `Tools/prepare_hd.py` reslices the included production sources without requiring image generation.

## Your walk

Collect five pieces of fallen wood and three berry bushes. Observe four kinds of wildlife, find three letters, light three trail lanterns, and catch two fish. Complete these in any order, return to the cabin to fold a paper crane, carry it to the High Clearing, and return home for the ending. Continue exploring afterwards if you like. The campsite is an optional place to rest.

There is no combat, hunger, failure timer, or inventory limit. The forest wraps in both axes. This is a compact game with one complete objective chain, rather than a multi-hour campaign. Cabin interactions use narrative panels; there is no separate walkable cabin interior. See `Documentation/Walkthrough.md` for locations.

## Saves

Each of the three slots has its own progress, player position, clock, day, and backup. Autosave updates only the active slot every minute and after important interactions. Overwriting another slot asks for confirmation. Loading an unreadable primary file tries that slot's backup. An original one-slot save from the previous project is supported as a legacy Slot 1 import; it is not deleted.

Typical Windows save directory:

`%USERPROFILE%\AppData\LocalLow\LittleElsewhere\Little Elsewhere\`

Files are `little-elsewhere-slot-1.json`, `little-elsewhere-slot-2.json`, and `little-elsewhere-slot-3.json`, each with a `.bak` backup. Settings use PlayerPrefs. A menu preview never writes a gameplay save.

## Validation and building

**Unity was not available in the creation environment. C# syntax and offline file integrity checks passed, but editor compilation, shader compilation, scene transitions, actual playtesting, and builds have not been executed.** Run these checks after importing:

1. **Little Elsewhere → Validate Project** checks resources, goals, all scenes, clock/shadow phase rules, slot paths, wrapping, and save serialization.
2. **Little Elsewhere → Run Play Mode Smoke Check** exercises menu → loading → gameplay, all three save slots, backup recovery, clock progression, midnight rollover, objectives, ending, and reloading. It preserves and restores existing slot files. It bypasses travel and fishing timing.
3. Manually play the game and verify rendering, movement, collision, all four lighting phases, menus, loading progress, and save/continue on your machine.

Install your target's Build Support module through Unity Hub, then use **Little Elsewhere → Build Windows x64 / Build macOS / Build Linux x64**. The commands build all three scenes into `Builds/`. Share the whole resulting build folder. This ZIP contains editable project source, not a prebuilt executable or the Unity editor.

## Rights and dependencies

No Asset Store purchases or third-party runtime plugins are required. Rendering uses Unity's built-in pipeline, legacy keyboard input, and IMGUI. Newly written game code/story/tooling, original synthesized audio, and generated artwork are included for you to use and modify for this game. Supplied reference artwork retains its own applicable rights. The bundled DejaVu Serif font's license is in `Documentation/DejaVu-Font-License.txt`.

Editor release reference: https://unity.com/releases/editor/whats-new/6000.6.2f1
