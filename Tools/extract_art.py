"""Rebuild extracted assets from the supplied reference boards. Requires Pillow, numpy, scipy.
The game already includes all generated PNGs and WAVs; running this is optional.
"""
from pathlib import Path
import json, math, wave
import numpy as np
from PIL import Image, ImageDraw, ImageFilter
from scipy import ndimage as ndi
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/'Assets/Resources/Art'; OUT.mkdir(parents=True,exist_ok=True)
a=Image.open(ROOT/'Assets/Reference/assets.png').convert('RGB')
m=Image.open(ROOT/'Assets/Reference/menus.png').convert('RGB')
manifest={}
def cut(name,box,solid=False,source=a):
 im=source.crop(box).convert('RGBA')
 if not solid:
  ar=np.array(im); rgb=ar[:,:,:3].astype(float)
  # Remove only paper reachable from the outer edge. Enclosed snow stays white.
  bg=(rgb[:,:,0]>199)&(rgb[:,:,1]>197)&(rgb[:,:,2]>187)&(np.max(rgb,2)-np.min(rgb,2)<31)
  seeds=np.zeros(bg.shape,bool); seeds[0,:]=bg[0,:]; seeds[-1,:]=bg[-1,:]; seeds[:,0]=bg[:,0]; seeds[:,-1]=bg[:,-1]
  paper=ndi.binary_propagation(seeds,mask=bg)
  keep=~paper
  labels,n=ndi.label(keep); sizes=np.bincount(labels.ravel()); sizes[0]=0
  keep=sizes[labels]>=max(5,im.width*im.height*.0005)
  ar[:,:,3]=keep.astype('uint8')*255
  im=Image.fromarray(ar)
 im.save(OUT/(name+'.png')); manifest[name]={'source':'menus.png' if source is m else 'assets.png','rect_xyxy':list(box),'paper_removed':not solid}
# Identically sized animation cells retain their original reference alignment.
for row,key in [(0,'idle'),(1,'down'),(2,'up'),(3,'left'),(4,'right')]:
 for col,x in enumerate([62,120,179,239]):
  y=[49,150,260,375,496][row]
  cut('player_'+key+'_'+str(col),(x-29,y-3,x+29,y+74))
for species,y,xs in [('fox',74,[345,420,491,564]),('deer',177,[341,413,491,564]),('rabbit',292,[342,414,491,565]),('squirrel',371,[341,418,496,566]),('bird',485,[340,414,490,564]),('owl',559,[337,419,543])]:
 for i,x in enumerate(xs):
  w=76 if species in ['owl','deer'] else 64
  h={'fox':64,'deer':83,'rabbit':54,'squirrel':75,'bird':51,'owl':77}[species]
  cut(species+'_'+str(i),(x-w//2,y-3,x+w//2,y+h))
for name,box in {
 'pine_0':(633,38,787,286),'pine_1':(776,8,916,223),'pine_2':(892,170,990,326),'pine_3':(625,278,747,449),'pine_4':(620,475,753,672),
 'bare_0':(875,360,985,489),'bare_1':(850,502,947,611),
 'cabin':(1035,116,1300,352),'shed':(1295,146,1387,231),'woodstore':(1317,269,1412,348),
 'bench':(1076,376,1161,435),'dock':(1294,355,1413,440),'boat':(1362,410,1454,477),
 'rock_0':(1004,517,1092,594),'rock_1':(1130,512,1235,623),'rock_2':(1040,576,1126,651),
 'berries':(1247,617,1318,694),'logs':(1380,576,1466,650),'sign':(1260,531,1321,601),
 'stump':(1332,630,1371,666),'lantern':(1441,618,1502,714),'fence':(1330,513,1427,591),
 'bush':(1373,672,1431,736),'lake':(8,771,316,1006),
 }.items(): cut(name,box)
cut('snow',(597,786,682,865),True)
cut('landscape',(0,483,616,702),True,m)
cut('paper',(1095,836,1255,915),True,m)
for name,box in {'icon_pack':(541,815,602,885),'icon_map':(629,814,696,883),'icon_journal':(715,815,788,886),'icon_fish':(810,819,888,879),'icon_lantern':(674,898,723,977),'icon_crane':(818,906,892,979)}.items(): cut(name,box,False,m)
# Seamless mirrored watercolor snow swatch.
im=Image.open(OUT/'snow.png'); tile=Image.new('RGB',(im.width*2,im.height*2))
tile.paste(im,(0,0)); tile.paste(im.transpose(Image.Transpose.FLIP_LEFT_RIGHT),(im.width,0)); tile.paste(tile.crop((0,0,im.width*2,im.height)).transpose(Image.Transpose.FLIP_TOP_BOTTOM),(0,im.height)); tile.save(OUT/'snow.png')
# Small procedural drawing assets used for trails, interaction markers and snowfall.
for name,size in [('flake',16),('footprint',24),('glow',128)]:
 im=Image.new('RGBA',(size,size)); d=ImageDraw.Draw(im)
 if name=='flake': d.ellipse((5,5,11,11),fill=(255,255,255,205))
 elif name=='footprint': d.ellipse((8,3,15,20),fill=(107,146,170,90))
 else:
  yy,xx=np.mgrid[:size,:size]; dist=np.sqrt((xx-size/2)**2+(yy-size/2)**2)/(size/2)
  arr=np.zeros((size,size,4),dtype=np.uint8); arr[:,:,:3]=(255,211,140); arr[:,:,3]=np.clip((1-dist)**2*125,0,125).astype('uint8'); im=Image.fromarray(arr)
 im.save(OUT/(name+'.png'))
(ROOT/'Documentation/ArtSlices.json').write_text(json.dumps(manifest,indent=2))
# Original synthesized ambience and melodic motifs, with deterministic noise.
SR=22050; AUD=ROOT/'Assets/Resources/Audio'; AUD.mkdir(exist_ok=True)
rng=np.random.default_rng(237)
def wav(name,data):
 data=np.asarray(data); data=np.clip(data,-.96,.96)
 with wave.open(str(AUD/(name+'.wav')),'wb') as w:
  w.setnchannels(1); w.setsampwidth(2); w.setframerate(SR); w.writeframes((data*32767).astype('<i2').tobytes())
t=np.arange(SR*18)/SR
noise=ndi.gaussian_filter1d(rng.normal(0,1,len(t)),22)
wind=noise*.9*(.65+.25*np.sin(t*.48))+.012*np.sin(t*2*math.pi*87)
# Crossfade tail into beginning for a seamless loop.
n=SR; wind[-n:]=wind[-n:]*np.linspace(1,0,n)+wind[:n]*np.linspace(0,1,n)
wav('wind',wind)
t=np.arange(int(SR*.24))/SR
wav('step',ndi.gaussian_filter1d(rng.normal(0,1,len(t)),1)*np.exp(-t*28)*.22)
t=np.arange(SR*3)/SR
wav('bell',sum(np.sin(2*math.pi*f*t)*np.exp(-t*(1+i*.7))*.12/(i+1) for i,f in enumerate([523.25,1046.5,1569.75,2100])))
wav('pickup',np.sin(2*math.pi*(660*t+90*t*t))*np.exp(-t*7)*.15)
t=np.arange(SR*32)/SR; song=np.zeros(len(t))
for start,note in enumerate([60,64,67,71,69,67,64,62,60,64,67,72,71,67,64,62]):
 tt=t-start*2; env=np.where(tt>=0,np.exp(-np.maximum(tt,0)*1.25),0)
 freq=440*2**((note-69)/12)
 song+=env*(np.sin(2*math.pi*freq*tt)+.24*np.sin(2*math.pi*freq*2*tt))*.045
wav('music',song)
print('Extracted',len(manifest),'art crops and synthesized five audio clips.')
# Remove disconnected labels and neighboring fragments left by reference-board crops.
for name,meta in manifest.items():
 if not meta['paper_removed']: continue
 path=OUT/(name+'.png');im=Image.open(path).convert('RGBA');ar=np.array(im)
 labels,n=ndi.label(ar[:,:,3]>0);sizes=np.bincount(labels.ravel());sizes[0]=0
 if n:
  largest=int(sizes.argmax());main=labels==largest
  # Detached color flecks are paper texture or neighboring subjects, not animation data.
  near=ndi.binary_dilation(main,iterations=4)
  valid=[i for i in range(1,n+1) if i==largest or (sizes[i]>7 and np.any(near & (labels==i)))]
  ar[:,:,3]=np.isin(labels,valid).astype('uint8')*255;Image.fromarray(ar).save(path)
# Cabin roof snow reaches the paper boundary: use a hand-traced silhouette to preserve it.
def polygon_asset(name,points):
 box=manifest[name]['rect_xyxy'];im=a.crop(box).convert('RGBA');mask=Image.new('L',im.size)
 ImageDraw.Draw(mask).polygon([(x-box[0],y-box[1]) for x,y in points],fill=255)
 im.putalpha(mask);im.save(OUT/(name+'.png'))
polygon_asset('cabin',[(1039,211),(1115,122),(1150,138),(1150,132),(1163,127),(1178,136),(1178,157),(1281,217),(1283,228),(1266,225),(1266,281),(1273,292),(1270,308),(1227,328),(1215,347),(1203,352),(1181,343),(1171,336),(1158,346),(1150,332),(1052,286),(1051,237),(1043,234)])
# Constrain trees to their own silhouettes; no adjacent reference-board subjects.
for name,points in {
 'pine_0':[(710,38),(727,106),(755,157),(778,211),(772,267),(723,286),(647,263),(633,214),(664,162),(690,102)],
 'pine_1':[(845,9),(865,71),(889,119),(916,173),(909,206),(849,223),(785,207),(776,164),(807,115),(825,60)],
 'pine_2':[(943,171),(961,223),(989,285),(984,320),(941,326),(902,310),(892,272),(923,223)],
 'pine_3':[(665,285),(684,341),(719,389),(742,423),(690,449),(636,429),(626,408),(644,362)],
 'pine_4':[(676,476),(695,526),(718,570),(749,639),(728,666),(675,672),(620,649),(620,594),(650,537)]
}.items():
 path=OUT/(name+'.png');im=Image.open(path);mask=Image.new('L',im.size);box=manifest[name]['rect_xyxy']
 ImageDraw.Draw(mask).polygon([(x-box[0],y-box[1]) for x,y in points],fill=255)
 ar=np.array(im);ar[:,:,3]=np.minimum(ar[:,:,3],np.array(mask));Image.fromarray(ar).save(path)
print('Cleaned reference labels and traced cabin snow silhouette.')
# Exclude the separate boat drawing which touches the dock's snowy reference shadow.
p=OUT/'dock.png';im=Image.open(p);arr=np.array(im);h,w=arr.shape[:2];yy,xx=np.mgrid[:h,:w]
arr[(xx>66)&(yy>59),3]=0;Image.fromarray(arr).save(p)
