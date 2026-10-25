# Verification — HD / 24-hour / three-scene edition

## Actually run here

- 337 offline integrity assertions passed.
- All 13 C# files parsed without grammar errors.
- All 59 HD PNG resources decode, have visible pixels and match the delivered dimension manifest. No source was upscaled. The player cells share a 314 × 314 canvas.
- Five 22,050 Hz 16-bit PCM clips decode correctly.
- Scene/configuration GUID references resolve. Build order contains MainMenu, Loading, LittleElsewhere, all enabled.
- The asynchronous loading catalog resolves all 63 texture paths, without duplicates.
- The default 24-hour duration is recorded; save version/migration and three-slot path bounds are present.
- Projected-shadow shader source, serif font/license, eleven production sources/prompts, and four reference images are included.
- Production source contact sheets were visually inspected for camera angle, transparency, subject placement and atlas layout. Extracted runtime texture dimensions are documented truthfully.

## NOT run here

Unity is not installed in the creation environment. C# semantic compilation, shader compilation, engine import, real menu/loading scene transitions, the included Unity validation/smoke checks, manual movement, lighting rendering, save behavior in Unity, and standalone builds have NOT been executed. Syntax/file checks are not a substitute for those runtime checks. No prebuilt executable is included.

Run Little Elsewhere → Validate Project, then Run Play Mode Smoke Check. The latter is designed to enter through MainMenu, observe Loading, start gameplay, exercise the three slots/backup, clock/midnight, progression/ending and reload, and restore existing slot files. It bypasses travel and fishing timing. Then manually verify visuals, controls, loading progress, four lighting phases and save/continue on your machine.

## Implementation boundaries

The high-angle perspective is prepainted 2D artwork in an orthographic sprite world. Time uses a real 24-hour default, configurable in GameConfig; it advances only during active gameplay or an eight-hour campsite rest. Shadows are stylized projected sprite silhouettes. The current game has one objective chain and ending, with no separate walkable cabin interior. The manifest gives actual native/cropped resolutions; this is not a claim that every PNG is native 4K.
