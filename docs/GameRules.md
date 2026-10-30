# Operation Extraction
# Game Rules v0.3 (Playable MVP Iteration)

## Gameplay Loop

Hero Selection  
→ Random Infil Assignment  
→ Player Selects Exfil  
→ Draw Opening Hand  
→ Exploration Turns  
→ Tile Resolution  
→ Combat/Loot  
→ Extraction  
→ End Results Screen

## Match Phases

- **Lobby**
- **HeroSelection**
- **Infil**
- **Exploration**
- **Combat**
- **Extraction**
- **MatchComplete**

## Turn Start

During **Exploration**:

1. If hand is empty or below max, draw 1 movement card at end of previous turn.
2. Player selects a destination tile.
3. Player plays movement cards from hand.

## Card Draw Rules

- Opening hand is drawn **after Exfil is selected**.
- Opening hand size: **5 movement cards**.
- End of each resolved turn: **draw 1 movement card**.
- Hand limit: **7 cards** (excess discarded).
- If movement deck is empty: reshuffle movement discard into deck.

## Card Play Rules

- This iteration uses **movement cards as the playable hand**.
- Cards add movement points toward selected destination.
- A turn continues until movement points meet/exceed destination distance.
- Utility and combat outcomes are now resolved through encounter/loot/tile systems.

## Movement Resolution

1. Player selects destination.
2. Distance is Manhattan distance.
3. Movement card values accumulate.
4. When accumulated movement reaches distance:
   - position updates to destination
   - tile resolution starts immediately

## Tile Resolution Order (Authoritative)

When movement succeeds to a new tile:

1. Check tile type
2. Resolve POI if present
3. Resolve Dynamic POI if present
4. Resolve Encounter (encounter deck draw)
5. Resolve Loot (loot deck draw when applicable)
6. Record Event Log entries
7. End Turn

## Encounter Resolution

Encounter deck cards:

- Nothing
- Enemy Patrol
- Militia Squad
- Mercenary Squad
- Supply Cache
- Safe House
- Boss Encounter

Enemy cards start **Combat** phase and resolve combat immediately.

## Combat Resolution

- Combat is deterministic for this MVP iteration.
- Hero deals gun-strength damage each round.
- Enemy retaliates each round until defeated or hero reaches 0 health.
- Armor absorbs incoming damage first.
- Results tracked:
  - enemies defeated
  - bosses defeated
  - damage dealt
  - damage taken

## Loot Resolution

Loot deck cards:

- Cash
- Materials
- Medical Supplies
- Armor Supplies
- Weapon Upgrade
- Intel
- Classified Intel

Loot can modify resources and hero state:

- Medical Supplies: restore health
- Armor Supplies: add armor
- Weapon Upgrade: +1 gun strength

## Extraction Resolution

Extraction succeeds when:

- player enters chosen Exfil tile
- player health > 0

On success:

- phase transitions to **MatchComplete**
- mission success results shown
- gameplay UI hidden

## Match Completion

Mission ends on either:

1. **Extraction success**
2. **Hero eliminated (health <= 0)**

MatchComplete replaces the game board and controls with the end results screen.

## Scoring

- Extraction Bonus: **100**
- Enemy Kill: **5**
- Boss Kill: **50**
- Cash: **1 point each**
- Materials: **5 points each**
- Intel: **25 points each**
- Weapon Upgrade: **10 points each**

## Rating Bands

- 0-99: Poor
- 100-199: Operator
- 200-299: Veteran
- 300-399: Elite
- 400+: Legend

## Assumptions

1. Opening hand is delayed until Exfil selection to reinforce mission setup flow.
2. Combat resolves in the tile-resolution pipeline (service-driven), not as a separate manual card minigame.
3. Classified Intel is tracked separately and also contributes to intel scoring.
4. Movement remains the only manually played card category for this iteration.
5. End result screen is authoritative once phase reaches MatchComplete.
