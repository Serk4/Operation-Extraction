# Operation Extraction
# Core Rules of Play
# Version 0.1 MVP

---

# Overview

Operation Extraction is a single-player turn-based extraction card game.

The objective is simple:

1. Select a Hero
2. Spawn at a random Infil location
3. Travel across the map
4. Gather loot and upgrades
5. Survive encounters
6. Reach an Exfil location
7. Extract successfully

---

# Match Setup

## Step 1: Select Hero

The player chooses one Hero from 12 available operators.

Hero selection should NOT be random.

Reason:

The hero choice is the player's first strategic decision.

Different heroes can later support different playstyles.

Example:

- Heavy = High Health
- Sniper = High Damage
- Scout = Fast movement
- Medic = Additional healing

For MVP only:

Each hero has:

- Name
- Health
- Gun Strength

---

# Step 2: Generate Map

Create:

- 6 random infil points
- 3 random exfil points
- 3 random event locations

Fixed locations remain constant.

---

# Step 3: Assign Spawn

Randomly choose 1 of the 6 infil locations.

This becomes the player start position.

---

# Step 4: Choose Exfil

Show all 3 exfil locations.

The player chooses one.

This becomes their objective.

---

# Hero Stats

Each Hero contains:

- Health
- Gun Strength

Health:

Determines survivability.

Gun Strength:

Determines base combat damage.

Example:

Ghost

Health = 80

Gun Strength = 7

---

# Core Concept

The game revolves around MOVEMENT.

If the player does not move,
they cannot:

- Loot
- Fight
- Reach POIs
- Reach Exfil
- Win

Movement is the most important resource.

---

# Card Types

The MVP should use only three card categories.

1. Movement Cards
2. Combat Cards
3. Utility Cards

---

# Starting Deck

Each player starts every match with the same deck.

No deck building in MVP.

---

# Starting Deck List

## Movement Cards

Walk x10

Move 1 tile

Sprint x6

Move 2 tiles

Advance x4

Move 3 tiles

Vehicle x2

Move 4 tiles

Movement Total:

22 Cards

---

## Combat Cards

Aim Shot x4

+2 Damage

Full Auto x4

+4 Damage

Grenade x2

+5 Damage

Combat Total:

10 Cards

---

## Utility Cards

Med Kit x2

Restore 10 Health

Armor Plate x2

Prevent 5 Damage

Utility Total:

4 Cards

---

# Total Deck

36 Cards

Every Hero uses the same deck.

Hero differences come from statistics.

---

# Starting Hand

At match start:

Draw 5 cards.

Example:

Walk
Walk
Sprint
Aim Shot
Med Kit

---

# Turn Structure

Each turn follows this sequence:

1. Draw 1 card
2. Play cards
3. Resolve encounters
4. End turn

---

# Movement Rules

Player chooses a destination tile.

The distance determines required movement.

Example:

Current Position:
B2

Destination:
E2

Distance:
3

Possible choices:

- Advance
- Sprint + Walk

After movement resolves:

Encounter roll occurs.

---

# Encounter System

Every movement action triggers a roll.

Roll:

D100

Results:

1-40

Nothing

41-70

Enemy Encounter

71-90

Loot Discovery

91-100

Special Event

---

# Loot Events

Player may discover:

Cash Bundle

Weapon Upgrade

Medical Supplies

Armor Supplies

---

# Weapon Upgrade

Permanently increases:

Gun Strength +1

For the rest of the match.

---

# Enemy System

Enemy types:

Tier 1

Health:
10

Damage:
2

Tier 2

Health:
20

Damage:
4

Boss

Health:
50

Damage:
8

---

# Combat Rules

Combat is automatic once an enemy is encountered.

Player may play Combat Cards.

Formula:

Damage =
Hero Gun Strength
+
Combat Card Bonus

Example:

Ghost:
7 Gun Strength

Aim Shot:
+2

Total Damage:
9

Enemy takes 9 Damage

---

# Enemy Retaliation

If enemy survives:

Enemy attacks.

Player loses enemy damage from Health.

Example:

Enemy Damage:
4

Player Health:
80 -> 76

---

# Utility Cards

Utility cards may be played at any time.

---

## Med Kit

Restore:

10 Health

Maximum health cannot exceed starting value.

---

## Armor Plate

Block:

5 incoming damage

Single use.

---

# Points of Interest

Entering a POI immediately grants:

- Loot Roll
- Event Roll

POIs are risk/reward locations.

The player may skip them.

---

# Dynamic POIs

Examples:

Mobile Command Post

Weapons Cache

Crash Site

These provide:

Better loot

Higher enemy chance

---

# Hand Limit

Maximum Hand:

7 Cards

If draw exceeds:

Discard down to 7.

---

# Empty Deck

When deck reaches zero:

Shuffle discard pile.

Create new draw deck.

Continue game.

---

# Victory

Player wins when:

Current Position equals selected Exfil.

AND

Health > 0

---

# Defeat

Player loses when:

Health <= 0

---

# Scoring

Extraction Bonus:
100

Cash Bundle:
+10 each

Weapon Upgrade:
+20 each

Boss Kill:
+50

Final Score:

Extraction Bonus
+
Loot
+
Kills

---

# MVP Win Condition

The MVP should focus on only this gameplay loop:

Move
→ Encounter
→ Loot
→ Upgrade
→ Reach Exfil

Everything else can be added later.

---

# Future Expansion

Version 0.2

Add:

- Hero abilities
- Armor stat
- Enemy patrols
- Contracts
- Fog of war

Version 0.3

Add:

- Inventory space
- Backpack management
- Multiple maps
- Factions

Version 1.0

Add:

- Co-op
- PvP extraction
- Persistent progression