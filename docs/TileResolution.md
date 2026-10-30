# Operation Extraction
# Tile Resolution System v0.3

## Purpose

Tile resolution is the primary gameplay engine that executes after successful movement.

---

## Resolution Order

1. Check tile type
2. Resolve POI if present
3. Resolve Dynamic POI if present
4. Resolve Encounter
5. Resolve Loot
6. Record Event Log entry
7. End Turn

---

## TileType Enum

- Empty
- POI
- DynamicPOI
- Infil
- Exfil

---

## Service Contract

`TileResolutionService` returns a `TileResolutionResult` containing:

- Tile Type
- Encounter Triggered
- Encounter Result
- Loot Awarded
- POI Triggered
- Event Log Entries

---

## Sequence Diagram (Markdown)

Player Plays Movement Card  
↓  
Movement Resolved  
↓  
Tile Resolution  
↓  
Encounter Draw  
↓  
Loot Draw (if triggered)  
↓  
Event Log Updated  
↓  
Turn Ends

---

## Combat Branch

Encounter Card = Enemy/Boss  
↓  
Enter Combat Phase  
↓  
Resolve Combat Loop  
↓  
Update Damage/Kill Stats  
↓  
If health <= 0 → MatchComplete (Failed)

---

## Extraction Branch

Tile Resolution Complete  
↓  
Check Current Tile == Selected Exfil  
↓  
If true and health > 0  
↓  
MatchComplete (Success)  
↓  
End Result Screen

---

## Assumptions

1. POI and Dynamic POI can both trigger loot opportunities.
2. Encounter and loot are deck-driven; percentages are not the primary source.
3. Event log entries from tile resolution are appended in-order for deterministic replay.
