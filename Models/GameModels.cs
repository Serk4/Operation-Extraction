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
    public int Armor { get; set; }
    public int Movement { get; set; } = 3;
    public int GunStrength { get; set; } = 5;
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
    public int GunStrength { get; set; }
}

/// <summary>
/// Represents one movement card available to the player.
/// </summary>
public class Card
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MovementValue { get; set; }
}

/// <summary>
/// A stack of movement cards used for draw and discard flow.
/// </summary>
public class Deck
{
    public List<Card> Cards { get; set; } = new();
}

/// <summary>
/// Represents one encounter deck card.
/// </summary>
public class EncounterDeckCard
{
    public EncounterCardType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Represents one loot deck card.
/// </summary>
public class LootDeckCard
{
    public LootType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Represents one tile on the tactical 10x10 map.
/// </summary>
public class MapTile
{
    public int X { get; set; }
    public int Y { get; set; }
    public TerrainType Terrain { get; set; }
    public TileType TileType { get; set; } = TileType.Empty;
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
/// Summary of a combat resolution against one enemy.
/// </summary>
public class CombatResult
{
    public int DamageDealt { get; set; }
    public int DamageTaken { get; set; }
    public int ArmorAbsorbed { get; set; }
    public bool Victory { get; set; }
    public bool BossDefeated { get; set; }
}

/// <summary>
/// Describes the full outcome of resolving a newly-entered tile.
/// </summary>
public class TileResolutionResult
{
    public TileType TileType { get; set; }
    public bool EncounterTriggered { get; set; }
    public string? EncounterResult { get; set; }
    public LootDeckCard? LootAwarded { get; set; }
    public bool PoiTriggered { get; set; }
    public List<string> EventLogEntries { get; set; } = new();
    public bool MatchEnded { get; set; }
}

/// <summary>
/// Tracks resource totals collected during a match.
/// </summary>
public class ResourceLedger
{
    public int Cash { get; set; }
    public int Materials { get; set; }
    public int Intel { get; set; }
    public int ClassifiedIntel { get; set; }
    public int MedicalSupplies { get; set; }
    public int ArmorSupplies { get; set; }
    public int WeaponUpgrades { get; set; }
}

/// <summary>
/// Tracks outcome statistics for the active match.
/// </summary>
public class MatchStatistics
{
    public int StartingHealth { get; set; }
    public int TurnsPlayed { get; set; }
    public int TilesTraveled { get; set; }
    public int PoisVisited { get; set; }
    public int DynamicPoisVisited { get; set; }
    public int EnemiesDefeated { get; set; }
    public int BossesDefeated { get; set; }
    public int DamageDealt { get; set; }
    public int DamageTaken { get; set; }
}

/// <summary>
/// Stores the current in-memory game session for the active match.
/// </summary>
public class GameSession
{
    public Hero SelectedHero { get; set; } = new();
    public GameMap Map { get; set; } = new();
    public Player Player { get; set; } = new();
    public GamePhase Phase { get; set; } = GamePhase.Lobby;

    public Deck MovementDeck { get; set; } = new();
    public List<Card> MovementDiscard { get; set; } = new();
    public List<Card> CurrentHand { get; set; } = new();
    public List<EncounterDeckCard> EncounterDeck { get; set; } = new();
    public List<EncounterDeckCard> EncounterDiscard { get; set; } = new();
    public List<LootDeckCard> LootDeck { get; set; } = new();
    public List<LootDeckCard> LootDiscard { get; set; } = new();

    public List<InfilPoint> InfilPoints { get; set; } = new();
    public List<ExfilPoint> ExfilPoints { get; set; } = new();
    public List<POI> FixedPOIs { get; set; } = new();
    public List<POI> DynamicPOIs { get; set; } = new();
    public ExfilPoint? CurrentTargetExfil { get; set; }

    public HashSet<string> VisitedPoiCoordinates { get; set; } = new();
    public ResourceLedger Resources { get; set; } = new();
    public MatchStatistics Stats { get; set; } = new();
    public List<string> EventLog { get; set; } = new();
    public MapTile? SelectedDestination { get; set; }
    public int PendingMovement { get; set; }
    public int TurnNumber { get; set; } = 1;
    public bool IsExtracted { get; set; }
    public bool IsDefeated { get; set; }
    public string LastMessage { get; set; } = string.Empty;
    public bool IsStarted { get; set; }
}

public enum TerrainType
{
    Open,
    Forest,
    Ruins,
    Street,
    Water
}

public enum TileType
{
    Empty,
    POI,
    DynamicPOI,
    Infil,
    Exfil
}

public enum EncounterCardType
{
    Nothing,
    EnemyPatrol,
    MilitiaSquad,
    MercenarySquad,
    SupplyCache,
    SafeHouse,
    BossEncounter
}

public enum LootType
{
    Cash,
    Materials,
    MedicalSupplies,
    ArmorSupplies,
    WeaponUpgrade,
    Intel,
    ClassifiedIntel
}

public enum GamePhase
{
    Lobby,
    HeroSelection,
    Infil,
    Exploration,
    Combat,
    Extraction,
    MatchComplete
}
