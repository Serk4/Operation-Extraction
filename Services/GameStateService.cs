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
        var fixedPois = _mapService.GenerateFixedPOIs(map);
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
            GunStrength = hero.GunStrength,
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
            FixedPOIs = fixedPois,
            DynamicPOIs = dynamicPois,
            CurrentTargetExfil = exfilPoints.FirstOrDefault(),
            LootInventory = new List<LootItem>(),
            EventLog = new List<string>(),
            SelectedDestination = null,
            PendingMovement = 0,
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
        if (tile.POI is ExfilPoint exfilPoint)
        {
            Session.CurrentTargetExfil = exfilPoint;
            Session.EventLog.Add($"Extraction objective updated to {exfilPoint.Name}.");
        }

        Session.SelectedDestination = tile;
        Session.PendingMovement = 0;
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

        if (Session.IsExtracted || Session.IsDefeated)
        {
            return;
        }

        if (!Session.CurrentHand.Contains(card))
        {
            return;
        }

        if (Session.ActiveEnemy is not null)
        {
            if (card.Type == CardType.Combat)
            {
                ResolveCombatCard(Session, card);
                return;
            }

            if (card.Type == CardType.Loot)
            {
                ApplyUtilityCard(Session, card);
                EndTurn(Session);
                return;
            }

            Session.LastMessage = "Cannot move while enemies are active. Use a combat card or utility card.";
            Session.EventLog.Add(Session.LastMessage);
            return;
        }

        if (card.Type == CardType.Movement)
        {
            ResolveMovementCard(Session, card);
            return;
        }

        if (card.Type == CardType.Combat)
        {
            Session.LastMessage = $"{card.Name} cannot be used without an active enemy encounter.";
            Session.EventLog.Add(Session.LastMessage);
            return;
        }

        ApplyUtilityCard(Session, card);
        EndTurn(Session);
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

    private void ResolveMovementCard(GameSession session, Card card)
    {
        if (session.SelectedDestination is null)
        {
            session.LastMessage = "Select a destination tile before playing movement cards.";
            session.EventLog.Add(session.LastMessage);
            return;
        }

        if (session.SelectedDestination.IsBlocked)
        {
            session.LastMessage = "Destination is blocked. Select another tile.";
            session.EventLog.Add(session.LastMessage);
            return;
        }

        var distance = GameplayEngine.CalculateDistance(
            session.Player.X,
            session.Player.Y,
            session.SelectedDestination.X,
            session.SelectedDestination.Y);
        if (distance <= 0)
        {
            session.LastMessage = "Already on the selected tile.";
            session.EventLog.Add(session.LastMessage);
            return;
        }

        session.CurrentHand.Remove(card);
        session.DiscardPile.Add(card);
        session.PendingMovement += card.MovementValue;

        if (session.PendingMovement < distance)
        {
            session.LastMessage = $"{card.Name} played. Movement progress {session.PendingMovement}/{distance}.";
            session.EventLog.Add(session.LastMessage);
            return;
        }

        session.Player.X = session.SelectedDestination.X;
        session.Player.Y = session.SelectedDestination.Y;
        session.PendingMovement = 0;
        session.SelectedDestination = null;
        session.LastMessage = $"{card.Name} completed movement to ({session.Player.X}, {session.Player.Y}).";
        session.EventLog.Add(session.LastMessage);

        ResolveTileArrival(session);
        EvaluateMatchOutcome(session);
        EndTurn(session);
    }

    private void ResolveTileArrival(GameSession session)
    {
        var tile = session.Map.GetTile(session.Player.X, session.Player.Y);
        if (tile.POI is not null)
        {
            session.EventLog.Add($"Entered location: {tile.POI.Name}.");
        }

        var encounter = _encounterService.RollEncounter(session, session.Player.X, session.Player.Y);
        if (encounter is null)
        {
            session.EventLog.Add("Encounter roll: clear.");
            return;
        }

        session.EventLog.Add($"Encounter: {encounter.Title} - {encounter.Description}");
        switch (encounter.Type)
        {
            case EncounterType.Enemy when encounter.Enemy is not null:
                session.ActiveEnemy = encounter.Enemy;
                session.EventLog.Add($"{encounter.Enemy.Name} engaged. Health {encounter.Enemy.Health}, Damage {encounter.Enemy.Damage}.");
                break;
            case EncounterType.SupplyCache when encounter.Loot is not null:
                ApplyLootEvent(session, encounter.Loot);
                break;
            case EncounterType.ExtractionRisk:
                session.Player.GunStrength += 1;
                session.EventLog.Add("Special event: Gun Strength increased by 1.");
                break;
        }
    }

    private void ApplyLootEvent(GameSession session, LootItem loot)
    {
        session.LootInventory.Add(loot);

        if (loot.Name == "Medical Supplies")
        {
            session.Player.Health = Math.Min(session.SelectedHero.MaxHealth, session.Player.Health + 10);
            session.EventLog.Add("Loot effect: restored 10 health.");
            return;
        }

        if (loot.Name == "Armor Supplies")
        {
            session.Player.Armor += 5;
            session.EventLog.Add("Loot effect: gained 5 armor.");
            return;
        }

        if (loot.Name == "Weapon Upgrade")
        {
            session.Player.GunStrength += 1;
            session.EventLog.Add("Loot effect: Gun Strength increased by 1.");
            return;
        }

        session.EventLog.Add($"Loot secured: {loot.Name}.");
    }

    private void ApplyUtilityCard(GameSession session, Card card)
    {
        if (card.HealValue > 0)
        {
            session.Player.Health = Math.Min(session.SelectedHero.MaxHealth, session.Player.Health + card.HealValue);
        }

        if (card.ArmorValue > 0)
        {
            session.Player.Armor += card.ArmorValue;
        }

        session.CurrentHand.Remove(card);
        session.DiscardPile.Add(card);
        session.LastMessage = $"{card.Name} used for recovery and support.";
        session.EventLog.Add(session.LastMessage);
    }

    private void ResolveCombatCard(GameSession session, Card card)
    {
        if (session.ActiveEnemy is null)
        {
            return;
        }

        session.CurrentHand.Remove(card);
        session.DiscardPile.Add(card);

        var result = _combatService.ResolveCombat(session.Player, session.ActiveEnemy, card);
        session.EventLog.Add(
            $"Combat: {card.Name} dealt {result.DamageDealt}. Enemy health now {result.EnemyHealthRemaining}.");

        if (result.Victory)
        {
            session.EventLog.Add($"Enemy neutralized: {session.ActiveEnemy.Name}.");
            session.ActiveEnemy = null;
        }
        else
        {
            session.EventLog.Add(
                $"Enemy retaliation: {result.HealthLost} health lost, {result.ArmorAbsorbed} armor blocked.");
        }

        EvaluateMatchOutcome(session);
        EndTurn(session);
    }

    private void EndTurn(GameSession session)
    {
        if (session.IsExtracted || session.IsDefeated)
        {
            return;
        }

        session.TurnNumber++;
        DrawCards(session, 1);
        TrimHandToLimit(session, 7);
    }

    private static void TrimHandToLimit(GameSession session, int handLimit)
    {
        while (session.CurrentHand.Count > handLimit)
        {
            var discarded = session.CurrentHand[^1];
            session.CurrentHand.RemoveAt(session.CurrentHand.Count - 1);
            session.DiscardPile.Add(discarded);
            session.EventLog.Add($"Hand limit reached. Discarded {discarded.Name}.");
        }
    }

    private static void EvaluateMatchOutcome(GameSession session)
    {
        if (session.Player.Health <= 0)
        {
            session.IsDefeated = true;
            session.LastMessage = "Mission failed. Operator eliminated.";
            session.EventLog.Add(session.LastMessage);
            return;
        }

        if (session.CurrentTargetExfil is null)
        {
            return;
        }

        if (session.Player.X == session.CurrentTargetExfil.X &&
            session.Player.Y == session.CurrentTargetExfil.Y &&
            session.Player.Health > 0)
        {
            session.IsExtracted = true;
            session.LastMessage = $"Extraction successful at {session.CurrentTargetExfil.Name}.";
            session.EventLog.Add(session.LastMessage);
        }
    }

    private static void DrawCards(GameSession session, int count)
    {
        for (var drawIndex = 0; drawIndex < count; drawIndex++)
        {
            if (session.Deck.Cards.Count == 0)
            {
                if (session.DiscardPile.Count == 0)
                {
                    return;
                }

                var reshuffled = session.DiscardPile.OrderBy(_ => Random.Shared.Next()).ToList();
                session.Deck.Cards = reshuffled;
                session.DiscardPile = new List<Card>();
                session.EventLog.Add("Deck exhausted. Reshuffled discard pile.");
            }

            var nextCard = session.Deck.Cards[0];
            session.Deck.Cards.RemoveAt(0);
            session.CurrentHand.Add(nextCard);
        }
    }
}
