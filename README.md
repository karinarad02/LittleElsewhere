# LittleElsewhere 🌲

**LittleElsewhere** is a peaceful, mysterious, top-down forest exploration game built with Unity (Universal Render Pipeline - URP). Featuring a distinctive hand-painted, watercolor-inspired aesthetic inspired by top-down winter illustrations, the player controls a tiny character in an orange coat exploring a vast, procedurally generated winter forest filled with wildlife, neighbours, subtle supernatural occurrences, and interconnected stories.

---

## 🎨 Core Vision & Visual Style

- **Relaxing & Curiosity-Driven:** No combat, hunger, stamina pressure, or mandatory deadlines.
- **Top-Down Orthographic Perspective:** Fixed high-overhead view emphasizing scale, negative space, and environmental silhouette.
- **Signature Aesthetics:**
  - Soft, muted winter palette with near-white snow textures.
  - Hand-painted top-down pine trees with star/radiating branch clusters and snow dusting.
  - Long, soft cool-blue shadows (`#3B5B88`) pointing diagonally across the snow.
  - Distinctive tiny character wearing a bright orange coat.
  - Organic, painterly silhouettes over conventional low-poly 3D graphics.

---

## 🌲 Key Features & Gameplay Mechanics

### 1. Procedural World & Seamless Wrapping
- **Seed-Based Generation:** Deterministic world generation creates a persistent, unique forest layout per save file.
- **Seamless Toroidal (Wraparound) Topology:** Walking east brings the player around from the west; walking north wraps around from the south seamlessly without artificial camera rotation or directional changes.
- **Chunk-Based Streaming:** Efficient rendering and memory management for a large, continuous world.
- **Anchored Story Points:** Handcrafted story locations and landmarks are placed deterministically based on geographical rules to ensure reachability.

### 2. Atmospheric Exploration & Wayfinding
- **No Minimap / HUD Clutter:** Exploration relies on environmental clues, landmarks, animal footprints, soundscapes, and personal memory.
- **Find Your Way Home:** Subtle, optional environmental cues guiding the player back to their cabin.
- **Return Home:** Convenient, gentle fast-travel back to resting points without penalties.
- **Shelters & Rest:** Discover campsites, forest shelters, and friendly neighbours' cabins. Rest advances time and allows choosing wakeup periods.

### 3. Time & Seasons
- **24-Hour Day/Night Cycle:** Dynamic atmospheric lighting, shifting shadow angles/lengths, and peaceful nighttime environments.
- **4 Dynamic Seasons:** Winter, Spring, Summer, Autumn.
  - Affects foliage, snow depth, water/ice status, wildlife behavior, weather, and neighbour routines.
  - Configurable timing parameters (default: 48 real minutes per day, 12 in-game days per season).

### 4. Organic Discovery & Storytelling
- **Interconnected Narratives:** Uncover wildlife secrets, community stories, personal discoveries, and subtle supernatural mysteries.
- **Clue Chains & Multiple Routes:** Flexible discovery paths ensure main story accessibility regardless of world seed.
- **Memory Journal:** Automatically logs observations, conversations, weather conditions, and personal notes as memories rather than quest checkboxes.
- **Multiple Endings:** Organic story conclusions with post-game exploration supported.

---

## 📁 Project Directory Structure

```
Assets/
├── Audio/                 # Environmental wind, footsteps in snow, soundscapes
├── Editor/                # Custom Unity Editor tools
├── Game/                  # Core game assets (Animals, Effects, Environment, Player, Terrain, Trees)
├── Menu/                  # Menu backgrounds, UI components & mockups
├── References/            # Concept artwork & reference sheets
├── Scenes/                # MainGameScene and project scenes
├── Scripts/               # Core, Player, World, Lighting, Time, Navigation scripts
├── Settings/              # URP & Input settings
└── Tests/                 # Playmode and Editmode unit tests
```

---

## 🚀 Development Roadmap & Prototype Implementation Plan

### Phase 1: Visual Prototype & Movement System (Current Focus)
- [x] Unity Project Initialization & Git Remote Setup
- [x] Folder Structure Migration directly under `Assets/`
- [x] **Snow Ground & Shader:** Soft textured snow surface with subtle painterly finish.
- [x] **Illustrated Tree System:** Billboarded or top-down pine trees casting soft blue directional shadows.
- [x] **Player Controller:** Smooth 8-direction top-down movement for the orange-coated character.
- [x] **Footprint System:** Dynamic footprint trails left in the snow that fade over time.
- [x] **Orthographic Camera:** Fixed overhead camera with smooth dampening.
- [x] **Day/Night Cycle Lighting:** Atmospheric URP directional light, color gradients, and time period presets.

### Phase 2: World Generation & Seamless Wrapping
- [ ] Seed-based deterministic chunk generation.
- [ ] Toroidal coordinate wrapping algorithm (seamless east-west and north-south borders).
- [ ] Geographic distribution rules for landmarks, clearings, and rivers.

### Phase 3: Time, Seasons & Environment
- [ ] Full 24-hour clock and 4-season transition system.
- [ ] Season-driven sprite/material property swaps (snow melt, autumn colors, spring blooms).
- [ ] Weather effects (gentle snowfall, wind, rain).

### Phase 4: Discovery, Journal & Story Engine
- [ ] Interactive environmental clues and animal tracking.
- [ ] Memory Journal UI and automatic discovery logging.
- [ ] Handcrafted story node placer & clue chain evaluation engine.
- [ ] Save/Load persistence system.

---

## 🛠️ Technical Requirements & Setup

- **Engine Version:** Unity 6 / Unity 2022.3 LTS or newer
- **Render Pipeline:** Universal Render Pipeline (URP)
- **Target Platform:** PC / Mac / Console

---

## 📄 License

This project is proprietary and confidential. All rights reserved.
