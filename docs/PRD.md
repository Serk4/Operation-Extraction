# Operation Extraction
## Product Requirements Document (PRD)
### Version 0.1 - MVP

---

# 1. Project Overview

Operation Extraction is a mobile-first web-based turn-based tactical card game inspired by extraction shooters such as Call of Duty: DMZ.

Players select a Hero Operator, infiltrate a hostile map, navigate between Points of Interest (POIs), fight enemies, collect loot, and successfully extract before being eliminated.

The MVP is intentionally simple and focused on learning the mechanics of extraction gameplay through a card system. All game state is maintained in memory and resets between matches.

---

# 2. Vision

Create an accessible extraction game that combines:

- Tactical map movement
- Card-driven decisions
- Risk versus reward gameplay
- Random encounters
- Loot acquisition
- Extraction objectives

The game should feel like a lightweight DMZ session that can be completed in 5 to 15 minutes on a mobile device.

---

# 3. MVP Goals

## Included

- Single-player gameplay
- Hero selection
- Fixed map
- Random infil locations
- Random exfil locations
- Random encounter generation
- Card-based movement
- Basic combat
- Loot system
- Extraction victory condition
- Mobile-first UI

## Excluded

- Multiplayer
- PvP
- Character progression
- Persistent inventory
- Database storage
- Trading
- Crafting
- Deck building
- Missions
- Contracts
- Matchmaking

---

# 4. Core Gameplay Loop

```text
Choose Hero
     ↓
Random Infil Spawn
     ↓
Select Exfil Destination
     ↓
Use Movement Cards
     ↓
Encounter Enemies
     ↓
Acquire Loot
     ↓
Move Across Map
     ↓
Reach Exfil Zone
     ↓
Extract Successfully
```

---

# 5. Technical Requirements

## Frontend

- ASP.NET Core 9
- Blazor Server
- Razor Components
- Bootstrap 5
- Mobile-first responsive design
- SVG map rendering

## Backend

- C#
- Service-based architecture
- Dependency Injection
- In-memory game state

## Storage

MVP storage requirements:

```text
None
```

All data exists only during gameplay.

---

# 6. Game State Flow

```text
Lobby
   ↓
Hero Selection
   ↓
Map Generation
   ↓
Infil
   ↓
Exploration
   ↓
Combat
   ↓
Loot
   ↓
Extraction
   ↓
End Game
```

---

# 7. Map Design

## Map Size

```text
10 x 10 Grid
```

Coordinates:

```text
A1 through J10
```

Example:

```text
     A B C D E F G H I J

1    . . . . . . . . . .
2    . . . . . . . . . .
3    . . . . . . . . . .
4    . . . . . . . . . .
5    . . . . . . . . . .
6    . . . . . . . . . .
7    . . . . . . . . . .
8    . . . . . . . . . .
9    . . . . . . . . . .
10   . . . . . . . . . .
```

---

# 8. Fixed Points of Interest (POIs)

These locations never change.

| POI | Coordinate |
|------|------|
| Police Station | C3 |
| Gas Station | H2 |
| Hospital | B8 |
| Shopping Mall | F7 |
| Radio Tower | I6 |

Map layout:

```text
     A B C D E F G H I J

1    . . . . . . . . . .
2    . . . . . . . G . .
3    . . P . . . . . . .
4    . . . . . . . . . .
5    . . . . . . . . . .
6    . . . . . . . . R .
7    . . . . . M . . . .
8    . H . . . . . . . .
9    . . . . . . . . . .
10   . . . . . . . . . .
```

Legend:

```text
P = Police Station
G = Gas Station
H = Hospital
M = Shopping Mall
R = Radio Tower
```

---

# 9. Random Infiltration System

Each match generates:

```text
6 random infiltration points
```

Requirements:

- Perimeter only
- No duplicates
- Randomized each match

Example:

```text
A3
A8
C10
G10
J4
J9
```

After generation:

```text
One location is randomly assigned to the player
```

---

# 10. Random Extraction System

Each match generates:

```text
3 extraction points
```

Requirements:

- Perimeter only
- No duplicates
- Cannot overlap infil points

Example:

```text
A10
F1
J10
```

The player selects their preferred exfil location.

---

# 11. Dynamic Event Locations

Each match generates:

```text
3 random special locations
```

Generated from:

- Mobile Command Post
- Weapons Cache
- Helicopter Crash Site
- Supply Convoy
- UAV Station
- Rogue Operator Camp

These are randomly placed on the map.

---

# 12. Hero System

The player chooses one Hero before deployment.

## Hero Properties

```csharp
public class Hero
{
    public string Name { get; set; }

    public int Health { get; set; }

    public int GunStrength { get; set; }
}
```

---

# 13. Hero Roster

| Hero | Health | Gun Strength |
|--------|--------|--------|
| Ranger | 100 | 5 |
| Ghost | 80 | 7 |
| Heavy | 120 | 4 |
| Scout | 80 | 4 |
| Sniper | 70 | 8 |
| Medic | 90 | 4 |
| Viking | 110 | 5 |
| Hunter | 85 | 6 |
| Reaper | 75 | 8 |
| Marshal | 95 | 5 |
| Outlaw | 90 | 6 |
| Sentinel | 115 | 4 |

---

# 14. Card System

The card system is the primary gameplay mechanic.

Players draw cards and use them to:

- Move
- Fight
- Defend
- Recover
- Improve equipment

---

# 15. Deck Rules

Starting hand:

```text
5 Cards
```

Maximum hand:

```text
7 Cards
```

At the beginning of each turn:

```text
Draw 1 Card
```

When deck reaches zero cards:

```text
Reshuffle discard pile
```

---

# 16. Movement Cards

Movement is the most important resource in the game.

## Walk

```text
Move 1 Tile
```

Copies:

```text
10
```

---

## Sprint

```text
Move 2 Tiles
```

Copies:

```text
6
```

---

## Tactical Advance

```text
Move 3 Tiles
```

Copies:

```text
4
```

---

## Off-Road Vehicle

```text
Move 4 Tiles
```

Copies:

```text
2
```

---

# 17. Movement Gameplay

Player Flow:

1. Select target location
2. Select destination exfil
3. Play movement cards
4. Move across grid
5. Trigger encounter checks

Example:

```text
Current Position: B2

Target Position: E2

Distance: 3

Use:
Tactical Advance

Result:
Player moves to E2
```

---

# 18. Combat Cards

## Aim Shot

```text
+2 Damage
```

---

## Full Auto

```text
+4 Damage
```

---

## Grenade

```text
+5 Damage
```

---

## Armor Up

```text
Gain Armor
```

---

# 19. Loot Cards

## Medical Kit

```text
Restore Health
```

---

## Armor Plate

```text
Restore Armor
```

---

## Cash Bundle

```text
Extraction Score
```

---

## Weapon Upgrade

```text
+1 Gun Strength
```

---

## Rare Weapon

```text
+2 Gun Strength
```

---

# 20. Enemy Types

## Tier 1 Militia

Health:

```text
10
```

Damage:

```text
2
```

---

## Tier 2 Mercenary

Health:

```text
20
```

Damage:

```text
4
```

---

## Boss Commander

Health:

```text
50
```

Damage:

```text
8
```

Usually found at:

- Mobile Command Post
- High Value POI

---

# 21. Enemy Encounters

Movement may generate encounters.

Roll percentages:

| Result | Chance |
|----------|----------|
| Nothing | 40% |
| Enemy | 30% |
| Loot | 20% |
| Special Event | 10% |

Every successful movement card triggers a roll.

---

# 22. Combat Resolution

Combat is deterministic for MVP.

Formula:

```text
Damage =
Hero Gun Strength
+
Card Bonus
```

Example:

```text
Hero Gun Strength = 7

Aim Shot Bonus = 2

Total Damage = 9
```

Enemy health is reduced accordingly.

If enemy survives:

```text
Enemy attacks player
```

---

# 23. Loot System

Loot can be obtained from:

- Encounters
- POIs
- Dynamic POIs
- Bosses

Loot rarity:

| Rarity | Chance |
|----------|----------|
| Common | 60% |
| Rare | 25% |
| Epic | 10% |
| Legendary | 5% |

---

# 24. Inventory

For MVP:

Inventory is unlimited.

Properties:

```text
Loot Count
Cash Count
Weapon Upgrades
Consumables
```

Future versions may introduce:

- Weight
- Backpack slots
- Capacity limits

---

# 25. Victory Conditions

Player wins when:

```text
Current Position == Exfil Position
```

AND

```text
Health > 0
```

Extraction ends the match.

---

# 26. Defeat Conditions

Player loses when:

```text
Health <= 0
```

OR

Unable to continue progression.

---

# 27. User Interface Layout

## Top Panel

```text
Hero Name
Health
Armor
Gun Strength
Inventory Count
```

---

## Middle Panel

```text
SVG Map
10 x 10 Grid
```

Display:

- Player
- POIs
- Dynamic POIs
- Infil
- Exfil

---

## Bottom Panel

Player hand:

```text
Movement Cards
Combat Cards
Consumables
```

---

## Event Log

Tracks:

```text
Movement
Damage
Loot
Combat
Extraction
```

Example:

```text
Moved to D4

Encountered Mercenary

Used Aim Shot

Enemy Defeated

Found Medical Kit
```

---

# 28. Architecture

## Models

```text
Hero
Player
Card
Deck
MapTile
GameMap
POI
Enemy
Encounter
LootItem
GameSession
```

---

## Services

```text
GameStateService
MapService
EncounterService
CombatService
CardService
HeroService
LootService
```

---

## Game Engine

```text
GameEngine
GameStateMachine
MapGenerator
CombatResolver
EncounterResolver
LootGenerator
```

---

# 29. MVP Deliverables

## Required

✅ Hero Selection

✅ Game Initialization

✅ Fixed Map

✅ Dynamic POIs

✅ Random Infil Generation

✅ Random Exfil Generation

✅ SVG Map Rendering

✅ Movement Cards

✅ Random Encounters

✅ Combat System

✅ Loot Drops

✅ Extraction System

✅ End Game Screens

---

# 30. Future Releases

## Version 0.2

- Fog of War
- Backpack Capacity
- Weight System
- Weapon Attachments
- Contracts
- Radiation Zone
- AI Patrol Movement
- Higher Difficulty Levels

## Version 0.3

- Multiple Maps
- Hero Abilities
- Permanent Unlocks
- Quest System
- Factions
- Daily Challenges

## Version 1.0

- Progression System
- Persistent Inventory
- Marketplace
- Statistics
- Co-op Play
- Multiplayer Extraction Matches

---

# 31. Design Principles

1. Movement drives gameplay.
2. Every decision should involve risk versus reward.
3. Reaching extraction is the primary objective.
4. The map should be readable on a mobile phone.
5. Complexity should be introduced gradually over future releases.
6. MVP should remain simple, enjoyable, and easily extensible.

---

# End of PRD
# Operation Extraction v0.1