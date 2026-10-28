namespace Operation_Extraction.Services;

using Operation_Extraction.Data;
using Operation_Extraction.GameEngine;
using Operation_Extraction.Models;

/// <summary>
/// Central state service for heroes, session lifecycle, and memory-backed gameplay.
/// </summary>
public class GameStateService : IGameStateService
{
    private readonly MapService _mapService;
    private readonly CardService _cardService;
    private readonly EncounterService _encounterService;
    private readonly CombatService _combatService;

    public GameStateService(MapService mapService, CardService cardService, EncounterService encounterService, CombatService combatService)
    {
        _mapService = mapService;
        _cardService = cardService;
        _encounterService = encounterService;
        _combatService = combatService;
    }

    public List<Hero> HeroRoster { get; private set; } = new();
    public GameSession? Session { get; private set; }

    public void InitializeHeroes()
    {
        if (HeroRoster.Count == 0)
        {
            HeroRoster = HeroCatalog.CreateHeroes().ToList();
        }
    }

    public void StartNewMatch(Hero hero)
    {
        var map = _mapService.GenerateMap();
        var infilPoints = _mapService.GenerateInfilPoints(map);
        var exfilPoints = _mapService.GenerateExfilPoints(map);
        var dynamicPois = _mapService.GenerateDynamicPOIs(map, infilPoints, exfilPoints);
        var spawnIndex = Random.Shared.Next(0, infilPoints.Count);
        var spawnPoint = infilPoints[spawnIndex];

        var player = new Player
        {
            Hero = hero,
            X = spawnPoint.X,
            Y = spawnPoint.Y,
            Health = hero.MaxHealth,
            Armor = hero.Armor,
            Inventory = new List<LootItem>()
        };

        var deck = _cardService.CreateDeck();
        var openingHand = _cardService.DrawOpeningHand(deck, 5);

        var session = new GameSession
        {
            SelectedHero = hero,
            Map = map,
            Player = player,
            Deck = deck,
            CurrentHand = openingHand,
            InfilPoints = infilPoints,
            ExfilPoints = exfilPoints,
            DynamicPOIs = dynamicPois,
            CurrentTargetExfil = exfilPoints.FirstOrDefault(),
            LootInventory = new List<LootItem>(),
            EventLog = new List<string>(),
            SelectedDestination = null,
            IsStarted = true,
            LastMessage = $"{hero.Name} has entered the map."
        };

        session.EventLog.Add($"{hero.Name} deployed to {spawnPoint.Name}.");
        session.EventLog.Add($"Current extraction route is {session.CurrentTargetExfil?.Name ?? "unknown"}.");
        Session = session;
    }

    public void SelectDestination(int x, int y)
    {
        if (Session is null)
        {
            return;
        }

        var tile = Session.Map.GetTile(x, y);
        Session.SelectedDestination = tile;
        var distance = GameplayEngine.CalculateDistance(Session.Player.X, Session.Player.Y, x, y);
        var requiredCards = GameplayEngine.CalculateRequiredMovementCards(distance);
        Session.LastMessage = $"Destination selected: {distance} tiles away. Requires {requiredCards} movement cards.";
        Session.EventLog.Add(Session.LastMessage);
    }

    public void PlayCard(Card card)
    {
        if (Session is null)
        {
            return;
        }

        if (!Session.CurrentHand.Contains(card))
        {
            return;
        }

        if (card.Type == CardType.Movement && Session.SelectedDestination is not null)
        {
            var distance = GameplayEngine.CalculateDistance(Session.Player.X, Session.Player.Y, Session.SelectedDestination.X, Session.SelectedDestination.Y);
            if (!GameplayEngine.CanMoveWithCard(card, distance))
            {
                Session.LastMessage = $"{card.Name} is not enough to reach the selected destination. Distance: {distance}.";
                Session.EventLog.Add(Session.LastMessage);
                return;
            }

            Session.Player.X = Session.SelectedDestination.X;
            Session.Player.Y = Session.SelectedDestination.Y;
            Session.CurrentHand.Remove(card);
            Session.DiscardPile.Add(card);
            Session.SelectedDestination = null;
            Session.LastMessage = $"{card.Name} used. Position updated to ({Session.Player.X}, {Session.Player.Y}).";
            Session.EventLog.Add(Session.LastMessage);

            var encounter = _encounterService.RollEncounter(Session, Session.Player.X, Session.Player.Y);
            if (encounter is not null)
            {
                Session.EventLog.Add($"Encounter: {encounter.Title} - {encounter.Description}");
                if (encounter.Type == EncounterType.Enemy && encounter.Enemy is not null)
                {
                    var result = _combatService.ResolveCombat(Session.Player, encounter.Enemy, card);
                    Session.EventLog.Add($"Combat resolved: {result.DamageDealt} damage, {result.HealthLost} health lost.");
                }
            }

            Session.TurnNumber++;
            return;
        }

        if (card.Type == CardType.Loot)
        {
            if (card.HealValue > 0)
            {
                Session.Player.Health = Math.Min(Session.SelectedHero.MaxHealth, Session.Player.Health + card.HealValue);
            }

            if (card.ArmorValue > 0)
            {
                Session.Player.Armor += card.ArmorValue;
            }

            Session.CurrentHand.Remove(card);
            Session.DiscardPile.Add(card);
            Session.LastMessage = $"{card.Name} used for recovery and support.";
            Session.EventLog.Add(Session.LastMessage);
            return;
        }

        Session.CurrentHand.Remove(card);
        Session.DiscardPile.Add(card);
        Session.LastMessage = $"{card.Name} was played.";
        Session.EventLog.Add(Session.LastMessage);
    }

    public int GetDistanceToSelectedTarget()
    {
        if (Session is null || Session.SelectedDestination is null)
        {
            return 0;
        }

        return GameplayEngine.CalculateDistance(Session.Player.X, Session.Player.Y, Session.SelectedDestination.X, Session.SelectedDestination.Y);
    }

    public int GetRequiredCardsForSelectedTarget()
    {
        return GameplayEngine.CalculateRequiredMovementCards(GetDistanceToSelectedTarget());
    }
}
