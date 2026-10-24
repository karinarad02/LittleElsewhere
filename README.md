# LittleElsewhere 🌲

**LittleElsewhere** is a peaceful, mysterious, top-down forest exploration game built with Unity (Universal Render Pipeline - URP). Featuring a distinctive hand-painted, watercolor-inspired aesthetic, the player controls a tiny character in an orange coat exploring a vast, procedurally generated winter forest filled with wildlife, neighbours, subtle supernatural occurrences, and interconnected stories.

---

## 🎨 Core Vision & Visual Style

- **Relaxing & Curiosity-Driven:** No combat, hunger, stamina pressure, or mandatory deadlines.
- **Top-Down Orthographic Perspective:** High-overhead view emphasizing scale, negative space, and environmental silhouette.
- **Signature Aesthetics:**
  - Soft, muted winter palette with near-white snow textures.
  - Detailed, hand-painted/illustrated pine trees and foliage.
  - Long, soft blue shadows that adjust dynamically with the day/night cycle.
  - Distinctive tiny character wearing a bright orange coat.
  - Organic, painterly silhouettes over conventional low-poly 3D aesthetics.

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

## 📁 Recommended Unity Project Structure

```
Assets/
├── Core/
│   ├── GameState/         # Game Loop, Save/Load System, Scene Management
│   ├── Events/            # Event Channels and ScriptableObject Messaging
│   └── Utilities/         # Math, Serialization, Chunk Helpers
├── Scripts/
│   ├── Player/            # Top-down Controller, Movement, Footprints Engine
│   ├── World/             # Procedural Generation, Chunk Manager, World Wrapping
│   ├── Environment/       # Day/Night Cycle, Season Controller, Weather
│   ├── Story/             # Narrative Engine, Clue Chains, Journal System
│   └── UI/                # Minimal HUD, Journal Interface, Transition Screens
├── ScriptableObjects/
│   ├── WorldGen/          # Biome Rules, Feature Distribution Rules
│   ├── Items/             # Discoverables, Artifacts, Letters
│   └── Stories/           # Story Node Definitions, Journal Entry Templates
├── Prefabs/
│   ├── Environment/       # Pine Trees, Boulders, Shelters, Water Tiles
│   ├── Characters/        # Player (Orange Coat), Wildlife, Neighbours
│   └── Effects/           # Footprint Emitter, Weather Particles
├── Settings/              # URP Pipeline Assets, Render Features
├── Materials/             # Custom Shaders (Painterly Snow, Soft Shadow, Leaves)
└── Textures/              # Hand-painted Textures, Sprites, Noise Maps
```

---

## 🚀 Development Roadmap & Prototype Implementation Plan

### Phase 1: Visual Prototype & Movement System (Current Focus)
- [x] Unity Project Initialization & Git Setup
- [ ] **Snow Ground & Shader:** Soft textured snow surface with subtle painterly finish.
- [ ] **Illustrated Tree System:** Billboarded or hybrid 2D pine trees cast soft blue directional shadows.
- [ ] **Player Controller:** Smooth top-down movement for the orange-coated character.
- [ ] **Footprint System:** Dynamic footprint trails left in the snow that fade over time.
- [ ] **Orthographic Camera:** Fixed overhead camera with smooth dampening.
- [ ] **Day/Night Cycle Lighting:** Basic atmospheric URP directional light and ambient gradient transitions.

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
