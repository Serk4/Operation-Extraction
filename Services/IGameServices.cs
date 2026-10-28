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
    void SelectDestination(int x, int y);
    void PlayCard(Card card);
    int GetDistanceToSelectedTarget();
    int GetRequiredCardsForSelectedTarget();
}

/// <summary>
/// Defines the map generation contract used by the state service.
/// </summary>
public interface IMapService
{
    GameMap GenerateMap();
    List<InfilPoint> GenerateInfilPoints(GameMap map);
    List<ExfilPoint> GenerateExfilPoints(GameMap map);
    List<POI> GenerateDynamicPOIs(GameMap map, List<InfilPoint>? infilPoints = null, List<ExfilPoint>? exfilPoints = null);
}

/// <summary>
/// Defines the contract for the deck and hand lifecycle.
/// </summary>
public interface ICardService
{
    Deck CreateDeck();
    List<Card> DrawOpeningHand(Deck deck, int count);
}

/// <summary>
/// Defines the encounter resolution contract.
/// </summary>
public interface IEncounterService
{
    Encounter? RollEncounter(GameSession session, int x, int y);
}

/// <summary>
/// Defines the combat resolution contract.
/// </summary>
public interface ICombatService
{
    CombatService.CombatResult ResolveCombat(Player player, Enemy enemy, Card card);
}
