namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Defines the read/write contract for the application game state.
/// </summary>
public interface IGameStateService
{
    List<Hero> HeroRoster { get; }
    GameSession? Session { get; }
    void InitializeHeroes();
    void StartNewMatch(Hero hero);
    void RestartMatch();
    void SelectDestination(int x, int y);
    void PlayCard(Card card);
    int GetDistanceToSelectedTarget();
    int GetRequiredCardsForSelectedTarget();
    int CalculateFinalScore();
    string GetFinalRating();
}

/// <summary>
/// Defines the map generation contract used by the state service.
/// </summary>
public interface IMapService
{
    GameMap GenerateMap();
    List<POI> GenerateFixedPOIs(GameMap map);
    List<InfilPoint> GenerateInfilPoints(GameMap map);
    List<ExfilPoint> GenerateExfilPoints(GameMap map);
    List<POI> GenerateDynamicPOIs(GameMap map, List<InfilPoint>? infilPoints = null, List<ExfilPoint>? exfilPoints = null);
}

/// <summary>
/// Defines movement deck setup and draw behavior.
/// </summary>
public interface IMovementDeckService
{
    Deck CreateMovementDeck();
    List<Card> DrawCards(GameSession session, int count);
    void DiscardCard(GameSession session, Card card);
}

/// <summary>
/// Defines encounter deck setup and draw behavior.
/// </summary>
public interface IEncounterDeckService
{
    List<EncounterDeckCard> CreateEncounterDeck();
    EncounterDeckCard DrawEncounter(GameSession session);
    void DiscardEncounter(GameSession session, EncounterDeckCard card);
}

/// <summary>
/// Defines loot deck setup and draw behavior.
/// </summary>
public interface ILootDeckService
{
    List<LootDeckCard> CreateLootDeck();
    LootDeckCard DrawLoot(GameSession session);
    void DiscardLoot(GameSession session, LootDeckCard card);
}

/// <summary>
/// Defines deterministic combat resolution.
/// </summary>
public interface ICombatService
{
    CombatResult ResolveCombat(Player player, Enemy enemy);
}

/// <summary>
/// Defines tile resolution behavior after successful movement.
/// </summary>
public interface ITileResolutionService
{
    TileResolutionResult ResolveTile(GameSession session, MapTile tile);
}
