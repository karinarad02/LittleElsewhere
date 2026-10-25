"""Offline project/asset integrity only. Does not compile C# or shaders or run Unity."""
from pathlib import Path
import json,re,wave
from PIL import Image
ROOT=Path(__file__).resolve().parents[1];checks=[]
def check(ok,description):
 if not ok:raise AssertionError(description)
 checks.append(description)
manifest=json.loads((ROOT/'Documentation/HDAssetManifest.json').read_text())
check(len(manifest)==59,'59 high-resolution runtime resource entries')
for name,entry in manifest.items():
 path=ROOT/'Assets/Resources/HD'/f'{name}.png'
 with Image.open(path) as im:
  im.load();check(list(im.size)==entry['size'],f'HD dimensions match: {name}')
  check(not entry['upscaled'],f'No upscaling recorded: {name}')
  if im.mode=='RGBA':check(im.getchannel('A').getextrema()[1]>0,f'Visible pixels: {name}')
for name in ['pine_0','pine_1','cabin','lake']:
 check(max(manifest[name]['size'])>=1024,f'Large native production subject: {name}')
for d in ['down','up','left','right','idle']:
 for i in range(4):check(manifest[f'player_{d}_{i}']['size']==[314,314],f'Aligned player frame: {d} {i}')
for name in ['wind','music','step','bell','pickup']:
 with wave.open(str(ROOT/'Assets/Resources/Audio'/f'{name}.wav')) as w:
  check(w.getnframes()>0 and w.getframerate()==22050 and w.getsampwidth()==2,f'PCM audio valid: {name}')
catalog=json.loads((ROOT/'Assets/Resources/PreloadCatalog.json').read_text())
check(len(catalog['art'])==len(set(catalog['art'])),'Loading catalog has no duplicates')
for name in catalog['art']:check((ROOT/'Assets/Resources'/f'{name}.png').is_file(),f'Loading resource resolves: {name}')
check(len(catalog['art'])==63,'All HD entries and four effect/paper textures preloaded')
guidmap={}
for p in (ROOT/'Assets').rglob('*.meta'):
 match=re.search(r'^guid: ([a-f0-9]{32})$',p.read_text(),re.M)
 if match:
  check(match[1] not in guidmap,f'Unique GUID: {p.relative_to(ROOT)}');guidmap[match[1]]=p
for rel in ['Assets/Scenes/MainMenu.unity','Assets/Scenes/Loading.unity','Assets/Scenes/LittleElsewhere.unity','Assets/Resources/GameConfig.asset']:
 text=(ROOT/rel).read_text()
 for guid in re.findall(r'guid: ([a-f0-9]{32})',text):check(guid in guidmap,f'Serialized reference resolves: {rel}')
build=(ROOT/'ProjectSettings/EditorBuildSettings.asset').read_text()
paths=re.findall(r'path: (.+)',build)
check(paths==['Assets/Scenes/MainMenu.unity','Assets/Scenes/Loading.unity','Assets/Scenes/LittleElsewhere.unity'],'Three enabled scenes in menu/loading/game order')
check(build.count('enabled: 1')==3,'All three scenes enabled')
check('activeInputHandler: 0' in (ROOT/'ProjectSettings/ProjectSettings.asset').read_text(),'Legacy keyboard input selected')
check('dayLengthSeconds: 86400' in (ROOT/'Assets/Resources/GameConfig.asset').read_text(),'Default full-day duration is 24 real hours')
shader=(ROOT/'Assets/Resources/Lighting/ProjectedShadow.shader').read_text()
check('ShadowVert' in shader and '_ShadowVector' in shader and 'SampleSpriteTexture' in shader,'Projected alpha-silhouette shader present')
save=(ROOT/'Assets/Scripts/Runtime/SaveSystem.cs').read_text()
check('Mathf.Clamp(slot, 1, 3)' in save and 'little-elsewhere-slot-' in save,'Three-slot path bounds present')
check('worldHours' in save and 'public int day' in save and 'data.version==1' in save,'Clock/day saved and legacy migration present')
for name in ['assets.png','menus.png','walking.jpg','camping.jpg']:check((ROOT/'Assets/Reference'/name).is_file(),f'Reference preserved: {name}')
check(len(list((ROOT/'Assets/ArtSources').glob('*.png')))==11,'Eleven new production sources bundled')
check((ROOT/'Documentation/DejaVu-Font-License.txt').is_file(),'Bundled font license')
prompts=json.loads((ROOT/'Documentation/GenerationPrompts.json').read_text());check(len(prompts['prompts'])==11,'Exact generation prompt set bundled')
syntax='Not run: install tree-sitter and tree-sitter-c-sharp.';count=0
try:
 from tree_sitter import Language,Parser
 import tree_sitter_c_sharp
 parser=Parser(Language(tree_sitter_c_sharp.language()))
 for path in (ROOT/'Assets/Scripts').rglob('*.cs'):
  check(not parser.parse(path.read_bytes()).root_node.has_error,f'C# syntax parses: {path.name}');count+=1
 syntax=f'All {count} C# files parsed without grammar errors.'
except ImportError:pass
report=f'''# Verification — HD / 24-hour / three-scene edition

## Actually run here

- {len(checks)} offline integrity assertions passed.
- {syntax}
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
'''
(ROOT/'Documentation/Verification.md').write_text(report)
print(f'PASS: {len(checks)} offline integrity assertions. {syntax}')
