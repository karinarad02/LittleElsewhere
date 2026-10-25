# Developer guide

## Scene flow

The build begins at `MainMenu.unity`. Its lightweight GameSession creates only the menu camera, UI and audio. New Walk and Continue choose one of three slots, then `SceneFlow` opens `Loading.unity`. `LoadingSceneController` asynchronously preloads the resources listed in `Resources/PreloadCatalog.json`, loads `LittleElsewhere.unity` without activating it immediately, displays completion, and activates gameplay. The gameplay GameSession consumes the pending new/load request and constructs one world. Save & Main Menu returns to the actual menu scene.

The loading scene is a separate Unity scene with progress and error handling. It is not a loading graphic drawn over an already running game. The original gameplay scene can still be opened directly for development; without a transition request it presents the title before starting a session.

| Source | Role |
| --- | --- |
| `GameSession.cs` | Scene-aware bootstrap, session/page state, interactions, objectives, autosave |
| `SceneFlow.cs` | Requested slot/action, three scene names, references held during loading |
| `LoadingSceneController.cs` | Async resource preload, scene activation, progress/tips/error UI |
| `WorldBuilder.cs` | Authored landmarks and campsite, seeded forest, collision, world wrapping |
| `PlayerController.cs` | WASD/arrows, normalized diagonal speed, four-facing animation, footprints |
| `DayNightCycle.cs` | 24-hour time, palette transitions, sun/moon silhouettes, nighttime warmth |
| `SaveSystem.cs` | Three independent versioned JSON slots, backups, legacy import, preferences |
| `GameUI.cs` | Title, slot menus, overwrite confirmation, HUD, settings, journal/map, story/fishing |
| `Art.cs` | Prefer `Resources/HD`, fall back to legacy paper/effects, cache runtime sprites |
| `Soundscape.cs` | Original synthesized music, wind, steps and interaction sounds |
| `GameConfig.cs` | Inspector configuration including day length, seed, camera and goals |
| `ProjectTools.cs` | Importer, validation, lighting preview and three-scene desktop builds |
| `SmokeCheck.cs` | End-to-end scene/progression/time/slot check inside Unity |

The built-in render pipeline, legacy `UnityEngine.Input`, SpriteRenderer and IMGUI are used. Active Input Handling must be **Input Manager (Old)** or **Both**. No URP, Input System, Cinemachine, TextMeshPro or third-party runtime dependency is required. No Canvas/EventSystem wiring is needed.

## Camera and coordinates

This is a 2D/2.5D game with prepainted high-angle artwork and an orthographic camera looking straight at the XY sprite plane. Do not rotate that camera into a 3D isometric view: the artwork already contains the elevated viewpoint. The player renders at 0.95 world units tall against a roughly 21-unit-high viewport at the default 16:9 layout. This creates the small-player/broad-snowfield composition in the references.

The world is 80 × 64 units, centered on the cabin. World entities render at the nearest periodic image relative to the player. Wrapped shortest deltas also power interactions/collision. Camera width is capped below half the world width. Long travel periodically recenters player/camera/tracks to avoid floating point drift and sprite sort limits.

Sprites use a bottom-biased pivot, a 64-source-pixels-per-unit Sprite, and an authored world display height. That display-height normalization preserves sizes when source textures become larger. The player animation cells keep a common size/baseline. Ordinary objects use tightly cropped PNGs with transparent padding.

Foot Y determines object draw order. Ground, pond, footprints, sun/moon shadows and glow have separate fixed sort bands. Simple circle obstacles sit at trunks/feet; the artwork silhouette is not the physics shape. `treeCount` is placement attempts; clearings and trail corridors reject some attempts.

## Day/night lighting

`SaveData.worldHours` is an accumulated double-precision hour count. Hour-of-day is modulo 24, and day index is 1 + floor(hours/24). The default real-time day is 86400 active-play seconds; change `GameConfig.dayLengthSeconds` to accelerate it. Time advances while walking/fishing, pauses in menus/dialogs, and advances eight hours at the campsite. Offline time is not simulated.

The palette interpolates continuously across midnight, dawn, noon, dusk and night. Each lightable sprite keeps its original tint; frame updates multiply it by the current tone. Ground stays uniformly toned to avoid square lighting patches at tile boundaries. Smooth warm sprite glows provide the local light pools.

Two child SpriteRenderers per eligible object share the `Resources/Lighting/ProjectedShadow.shader` material. Shader vertex projection shears/flattens the source silhouette from its foot line; a MaterialPropertyBlock supplies direction, length, tint and opacity. Sun and moon projections blend with elevation. This is stylized alpha-silhouette shadowing; it does not model terrain occlusion, physical moon phases, arbitrary point-light cast shadows, or self-relighting through normal maps.

Use the editor lighting preview menu after starting a walk. Preview changes the active session clock and pauses; resuming and saving retains the changed time. The regular settings brightness slider is separate from time-of-day tint.

## Saving and migration

There are exactly three slot paths, each with `.bak` recovery and `.tmp` write staging. Save/load menus preview slots without changing the active slot. New/save overwrite requests name the target and require confirmation; autosave writes only the active slot. The save contains progress, player position, world time/day, location and completion state.

Version-1 saves are migrated to version 2 in memory, starting their new clock at 09:00 on day 1. A previous `little-elsewhere-save.json` is offered as Slot 1 only when the newer slot files do not exist; the legacy file is not removed. Shared settings remain in PlayerPrefs. Add explicit migrations before changing future save semantics.

## Artwork and source regeneration

`Assets/ArtSources` contains eleven new built-in-imagegen production sources. `Resources/HD` contains 59 cropped/derived game textures; the dimension manifest records actual sizes, source identity, crop rectangles, and no-upscaling status. Two tree drawings supply five mirrored/size-varied runtime entries. Wildlife and props use their own atlases. Player walking has four directions × four frames; the four front-facing idle entries reuse the same sheet rather than claiming separately animated idle poses.

The original low-resolution board crops remain under `Resources/Art` for small effects/paper and reference. Every texture used as a major gameplay subject now has an HD override. The loading catalog includes all HD resources plus paper, snowflake, footprint and glow effects. Update that catalog if you add a texture required before gameplay.

Texture importing is uncompressed RGBA, bilinear, no mipmaps, no non-power-of-two rescaling, and a 4096-pixel import cap. Pixel dimensions in the asset manifest are actual source/slice sizes, not native-4K claims.

Optional regeneration requires Python + Pillow; `prepare_hd.py` also imports numpy. Run:

```bash
python Tools/prepare_hd.py
```

This reslices existing production sources without contacting an image service. The original extraction/synthesized-audio tool remains `Tools/extract_art.py`; it updates only the legacy board crops/audio. The exact image generation prompts and tool mode are preserved separately.

## Extend and build

Change authored object positions, display sizes and stable IDs in WorldBuilder. Change goals in GameConfig within the available supply limits, and add story/interaction rules in GameSession. The world is runtime generated; dragging a Play Mode object does not persist its placement. Build commands include all three scenes in order. Distribute each complete standalone build folder.

Run the Unity validation menu, the Play Mode smoke check, then a real manual playthrough. The smoke check goes through all three scenes, tests slot isolation/backup, clock progression/midnight, objectives, ending and reload; it restores pre-existing slot files afterward. It bypasses actual movement and fishing timing, so those still need manual checks.
