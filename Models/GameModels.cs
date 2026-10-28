using System.Collections.Generic;

namespace Operation_Extraction.Models;

/// <summary>
/// Represents the playable hero archetype in the extraction game.
/// </summary>
public class Hero
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string AccentColor { get; set; } = "#5eead4";
    public int MaxHealth { get; set; } = 100;
    public int Armor { get; set; } = 0;
    public int Movement { get; set; } = 3;
}

/// <summary>
/// Contains the current in-match state for a human-controlled operator.
/// </summary>
public class Player
{
    public Hero Hero { get; set; } = new();
    public int X { get; set; }
    public int Y { get; set; }
    public int Health { get; set; }
    public int Armor { get; set; }
    public List<LootItem> Inventory { get; set; } = new();
}

/// <summary>
/// Represents a single extractable combat or utility card in the current hand.
/// </summary>
public class Card
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CardType Type { get; set; }
    public int MovementValue { get; set; }
    public int Damage { get; set; }
    public int HealValue { get; set; }
    public int ArmorValue { get; set; }
}

/// <summary>
/// A stack of cards used for turn resolution and draw logic.
/// </summary>
public class Deck
{
    public List<Card> Cards { get; set; } = new();
}

/// <summary>
/// Represents one tile on the tactical 10x10 map.
/// </summary>
public class MapTile
{
    public int X { get; set; }
    public int Y { get; set; }
    public TileType Type { get; set; }
    public bool IsBlocked { get; set; }
    public POI? POI { get; set; }
}

/// <summary>
/// Base class for world points of interest that can appear on the map.
/// </summary>
public class POI
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsDynamic { get; set; }
}

/// <summary>
/// A map point that indicates a safe zone for entering the operation.
/// </summary>
public class InfilPoint : POI
{
    public string Marker => "INFIL";
}

/// <summary>
/// A map point that marks a vehicle or extraction objective.
/// </summary>
public class ExfilPoint : POI
{
    public string Marker => "EXFIL";
}

/// <summary>
/// Represents the full tactical grid for a match.
/// </summary>
public class GameMap
{
    public const int GridSize = 10;
    public MapTile[,] Tiles { get; set; } = new MapTile[GridSize, GridSize];
    public int Width => GridSize;
    public int Height => GridSize;

    public MapTile GetTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= GridSize || y >= GridSize)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "Tile coordinates must stay within the 10x10 map.");
        }

        return Tiles[x, y];
    }
}

/// <summary>
/// Enemy profile used for deterministic encounter composition.
/// </summary>
public class Enemy
{
    public string Name { get; set; } = string.Empty;
    public int Health { get; set; }
    public int Damage { get; set; }
    public bool IsBoss { get; set; }
}

/// <summary>
/// Encapsulates the result of a tile encounter event.
/// </summary>
public class Encounter
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EncounterType Type { get; set; }
    public Enemy? Enemy { get; set; }
    public LootItem? Loot { get; set; }
    public int RiskLevel { get; set; }
}

/// <summary>
/// A loot item that can be picked up during the match.
/// </summary>
public class LootItem
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Value { get; set; }
}

/// <summary>
/// Stores the current in-memory game session for the active match.
/// </summary>
public class GameSession
{
    public Hero SelectedHero { get; set; } = new();
    public GameMap Map { get; set; } = new();
    public Player Player { get; set; } = new();
    public List<Card> CurrentHand { get; set; } = new();
    public Deck Deck { get; set; } = new();
    public List<Card> DiscardPile { get; set; } = new();
    public List<InfilPoint> InfilPoints { get; set; } = new();
    public List<ExfilPoint> ExfilPoints { get; set; } = new();
    public List<POI> DynamicPOIs { get; set; } = new();
    public ExfilPoint? CurrentTargetExfil { get; set; }
    public List<LootItem> LootInventory { get; set; } = new();
    public List<string> EventLog { get; set; } = new();
    public MapTile? SelectedDestination { get; set; }
    public int TurnNumber { get; set; } = 1;
    public string LastMessage { get; set; } = string.Empty;
    public bool IsStarted { get; set; }
}

public enum CardType
{
    Movement,
    Combat,
    Loot
}

public enum TileType
{
    Open,
    Forest,
    Ruins,
    Street,
    Water
}

public enum EncounterType
{
    None,
    Enemy,
    SupplyCache,
    ExtractionRisk
}
