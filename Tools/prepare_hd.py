"""Slice the newly generated production source sheets, without upscaling any pixel art.
Input: Assets/ArtSources. Output: Resources/HD and a truthful native-size manifest.
Requires Pillow and numpy. ArtSources are bundled, so no image generation is needed to rerun.
"""
from pathlib import Path
from PIL import Image,ImageFilter,ImageOps
import numpy as np,json,math
ROOT=Path(__file__).resolve().parents[1];SRC=ROOT/'Assets/ArtSources';OUT=ROOT/'Assets/Resources/HD';OUT.mkdir(exist_ok=True,parents=True)
report={}
def write(name,im,source,tight=True,box=None):
 im=im.convert('RGBA')
 if tight:
  alpha=im.getchannel('A');mask=alpha.point(lambda x:255 if x>=12 else 0);bbox=mask.getbbox()
  if bbox:
   pad=max(4,round(max(bbox[2]-bbox[0],bbox[3]-bbox[1])*.025))
   region=im.crop(bbox);new=Image.new('RGBA',(region.width+pad*2,region.height+pad*2));new.paste(region,(pad,pad));im=new
 im.save(OUT/(name+'.png'))
 report[name]={'size':list(im.size),'source':source,'crop':box,'upscaled':False}
def source(name):return Image.open(SRC/(name+'.png')).convert('RGBA')
def atlas(name,columns,rows,names):
 im=source(name)
 for i,key in enumerate(names):
  x=i%columns;y=i//columns
  box=[round(x*im.width/columns),round(y*im.height/rows),round((x+1)*im.width/columns),round((y+1)*im.height/rows)]
  write(key,im.crop(box),name,box=box)
# New sources, never magnified from the original 1536x1024 reference boards.
for i in range(5):
 im=source('pineA' if i%2==0 else 'pineB')
 if i>=2:im=ImageOps.mirror(im)
 write('pine_'+str(i),im,'pineA' if i%2==0 else 'pineB')
for name in ['cabin','camp','lake']:write(name,source(name),name)
atlas('props',4,3,['shed','woodstore','bench','dock','boat','rock_0','berries','logs','sign','lantern','bare_0','bush'])
for name,base in [('rock_1','rock_0'),('rock_2','rock_0'),('bare_1','bare_0')]:
 im=Image.open(OUT/(base+'.png'));write(name,ImageOps.mirror(im),report[base]['source'],False)
atlas('wildlife',3,2,[s+'_0' for s in ['fox','deer','rabbit','squirrel','bird','owl']])
atlas('icons',4,2,['icon_pack','icon_map','icon_journal','icon_fish','icon_lantern','icon_crane','icon_logs','icon_rock'])
# Preserve exact cell alignment for animation. Never tightly crop separate player frames.
im=source('player');cell=math.ceil(im.width/4)
for row,direction in enumerate(['down','up','left','right']):
 for col in range(4):
  box=[round(col*im.width/4),round(row*im.height/4),round((col+1)*im.width/4),round((row+1)*im.height/4)]
  frame=im.crop(box);canvas=Image.new('RGBA',(cell,cell));canvas.paste(frame,((cell-frame.width)//2,cell-frame.height))
  write(f'player_{direction}_{col}',canvas,'player',False,box)
  if direction=='down':write(f'player_idle_{col}',canvas,'player',False,box)
write('landscape',source('menu'),'menu',False)
# Mirroring creates mathematically matching edges. This grows the swatch with new mirrored
# pixels rather than resampling or claiming extra source resolution.
im=source('snow');tile=Image.new('RGBA',(im.width*2,im.height*2));tile.paste(im,(0,0));tile.paste(ImageOps.mirror(im),(im.width,0));tile.paste(ImageOps.flip(tile.crop((0,0,im.width*2,im.height))),(0,im.height));write('snow',tile,'snow (mirrored seamless arrangement)',False)
(ROOT/'Documentation/HDAssetManifest.json').write_text(json.dumps(report,indent=2))
print('Prepared',len(report),'HD resources, no upscaling.')
for name in ['pine_0','pine_1','cabin','camp','lake','player_down_0','rock_0','landscape','snow']:print(name,report[name]['size'])
