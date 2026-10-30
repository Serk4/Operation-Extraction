# Operation Extraction
# Card and Deck System v0.3

## Overview

This iteration transitions from roll-heavy logic to **deck-driven flow**.

Three in-memory decks are active during a match:

1. Movement Deck
2. Encounter Deck
3. Loot Deck

---

## Movement Deck

Movement cards are the playable hand.

| Card    | Effect       | Copies |
| ----    | ------       | ------:|
| Walk    | Move 1 tile  | 10     |
| Sprint  | Move 2 tiles | 6      |
| Advance | Move 3 tiles | 4      |
| Vehicle | Move 4 tiles | 2      |

Rules:

- Opening hand: 5 cards
- Draw 1 card at end of each turn
- Hand limit: 7
- Discard reshuffle when deck is empty

---

## Encounter Deck

Encounter cards are drawn during tile resolution.

| Card            | Result                                |
| ----            | ------                                |
| Nothing         | No combat, no direct encounter effect |
| Enemy Patrol    | Low-threat enemy combat               |
| Militia Squad   | Medium-threat enemy combat            |
| Mercenary Squad | High-threat enemy combat              |
| Supply Cache    | Loot-focused encounter                |
| Safe House      | Recovery + loot opportunity           |
| Boss Encounter  | Boss combat                           |

Notes:

- Encounter cards are drawn from deck and discarded.
- Encounter deck reshuffles from discard when empty.

---

## Loot Deck

Loot cards are drawn during loot resolution.

| Card             | Primary Effect                                 |
| ---              | ---                                            |
| Cash             | Adds cash resources                            |
| Materials        | Adds materials resources                       |
| Medical Supplies | Adds resource + restores health                |
| Armor Supplies   | Adds resource + grants armor                   |
| Weapon Upgrade   | Adds resource + increases gun strength         |
| Intel            | Adds intel resource                            |
| Classified Intel | Adds classified intel (high-value intel track) |

Notes:

- Loot cards are drawn from deck and discarded.
- Loot deck reshuffles from discard when empty.

---

## Card Flow Summary

Player Selects Destination  
→ Player Plays Movement Cards  
→ Movement Completes  
→ Tile Resolution  
→ Encounter Card Draw  
→ Loot Card Draw (if triggered)  
→ Turn Ends

---

## Assumptions

1. Movement cards remain the only player-clicked card category in this iteration.
2. Encounter and loot cards are system-resolved for consistency and speed.
3. Deck composition is intentionally deterministic and in-memory for MVP stability.
