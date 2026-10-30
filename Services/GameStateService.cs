namespace Operation_Extraction.Services;

using Operation_Extraction.Data;
using Operation_Extraction.GameEngine;
using Operation_Extraction.Models;

/// <summary>
/// Central state service for hero lifecycle and in-memory match orchestration.
/// </summary>
public class GameStateService : IGameStateService
{
    private readonly IMapService _mapService;
    private readonly IMovementDeckService _movementDeckService;
    private readonly IEncounterDeckService _encounterDeckService;
    private readonly ILootDeckService _lootDeckService;
    private readonly ITileResolutionService _tileResolutionService;

    public GameStateService(
        IMapService mapService,
        IMovementDeckService movementDeckService,
        IEncounterDeckService encounterDeckService,
        ILootDeckService lootDeckService,
        ITileResolutionService tileResolutionService)
    {
        _mapService = mapService;
        _movementDeckService = movementDeckService;
        _encounterDeckService = encounterDeckService;
        _lootDeckService = lootDeckService;
        _tileResolutionService = tileResolutionService;
    }

    /// <inheritdoc />
    public List<Hero> HeroRoster { get; private set; } = new();

    /// <inheritdoc />
    public GameSession? Session { get; private set; }

    /// <inheritdoc />
    public void InitializeHeroes()
    {
        if (HeroRoster.Count == 0)
        {
            HeroRoster = HeroCatalog.CreateHeroes().ToList();
        }
    }

    /// <inheritdoc />
    public void StartNewMatch(Hero hero)
    {
        var map = _mapService.GenerateMap();
        var fixedPois = _mapService.GenerateFixedPOIs(map);
        var infilPoints = _mapService.GenerateInfilPoints(map);
        var exfilPoints = _mapService.GenerateExfilPoints(map);
        var dynamicPois = _mapService.GenerateDynamicPOIs(map, infilPoints, exfilPoints);

        var spawnPoint = infilPoints[Random.Shared.Next(0, infilPoints.Count)];
        var player = new Player
        {
            Hero = hero,
            X = spawnPoint.X,
            Y = spawnPoint.Y,
            Health = hero.MaxHealth,
            Armor = hero.Armor,
            GunStrength = hero.GunStrength
        };

        var session = new GameSession
        {
            SelectedHero = hero,
            Map = map,
            Player = player,
            Phase = GamePhase.Infil,
            MovementDeck = _movementDeckService.CreateMovementDeck(),
            EncounterDeck = _encounterDeckService.CreateEncounterDeck(),
            LootDeck = _lootDeckService.CreateLootDeck(),
            InfilPoints = infilPoints,
            ExfilPoints = exfilPoints,
            FixedPOIs = fixedPois,
            DynamicPOIs = dynamicPois,
            CurrentTargetExfil = null,
            SelectedDestination = null,
            PendingMovement = 0,
            TurnNumber = 1,
            IsStarted = true,
            LastMessage = $"{hero.Name} deployed at {ToCoordinate(spawnPoint.X, spawnPoint.Y)}.",
            Stats = new MatchStatistics
            {
                StartingHealth = hero.MaxHealth,
                TurnsPlayed = 0
            }
        };

        session.EventLog.Add(session.LastMessage);
        session.EventLog.Add("Select an Exfil tile to set your extraction objective.");
        Session = session;
    }

    /// <inheritdoc />
    public void RestartMatch()
    {
        if (Session is null)
        {
            return;
        }

        StartNewMatch(Session.SelectedHero);
    }

    /// <inheritdoc />
    public void SelectDestination(int x, int y)
    {
        if (Session is null || Session.Phase == GamePhase.MatchComplete)
        {
            return;
        }

        var tile = Session.Map.GetTile(x, y);
        if (tile.IsBlocked)
        {
            Session.LastMessage = "Blocked tile selected. Choose a different destination.";
            Session.EventLog.Add(Session.LastMessage);
            return;
        }

        if (Session.Phase == GamePhase.Infil)
        {
            if (tile.POI is not ExfilPoint exfilPoint)
            {
                Session.LastMessage = "Select one of the Exfil markers to begin the mission.";
                Session.EventLog.Add(Session.LastMessage);
                return;
            }

            Session.CurrentTargetExfil = exfilPoint;
            Session.Phase = GamePhase.Exploration;
            Session.CurrentHand = _movementDeckService.DrawCards(Session, 5);
            Session.EventLog.Add($"Exfil objective selected: {exfilPoint.Name} ({ToCoordinate(x, y)}).");
            Session.EventLog.Add("Opening hand drawn (5 movement cards).");
            return;
        }

        if (Session.Phase != GamePhase.Exploration)
        {
            return;
        }

        Session.SelectedDestination = tile;
        Session.PendingMovement = 0;
        var distance = GameplayEngine.CalculateDistance(Session.Player.X, Session.Player.Y, x, y);
        var requiredCards = GameplayEngine.CalculateRequiredMovementCards(distance);
        Session.LastMessage = $"Destination selected: {ToCoordinate(x, y)} ({distance} tiles, ~{requiredCards} cards).";
        Session.EventLog.Add(Session.LastMessage);
    }

    /// <inheritdoc />
    public void PlayCard(Card card)
    {
        if (Session is null || Session.Phase != GamePhase.Exploration || Session.IsExtracted || Session.IsDefeated)
        {
            return;
        }

        if (!Session.CurrentHand.Contains(card))
        {
            return;
        }

        if (Session.SelectedDestination is null)
        {
            Session.LastMessage = "Select a destination tile before playing movement cards.";
            Session.EventLog.Add(Session.LastMessage);
            return;
        }

        var distance = GameplayEngine.CalculateDistance(
            Session.Player.X,
            Session.Player.Y,
            Session.SelectedDestination.X,
            Session.SelectedDestination.Y);

        if (distance <= 0)
        {
            Session.LastMessage = "Already on selected tile.";
            Session.EventLog.Add(Session.LastMessage);
            return;
        }

        Session.CurrentHand.Remove(card);
        _movementDeckService.DiscardCard(Session, card);
        Session.PendingMovement += card.MovementValue;

        if (Session.PendingMovement < distance)
        {
            Session.LastMessage = $"{card.Name} played ({Session.PendingMovement}/{distance} movement).";
            Session.EventLog.Add(Session.LastMessage);
            return;
        }

        ResolveMovement(Session, distance);
    }

    /// <inheritdoc />
    public int GetDistanceToSelectedTarget()
    {
        if (Session is null || Session.SelectedDestination is null)
        {
            return 0;
        }

        return GameplayEngine.CalculateDistance(
            Session.Player.X,
            Session.Player.Y,
            Session.SelectedDestination.X,
            Session.SelectedDestination.Y);
    }

    /// <inheritdoc />
    public int GetRequiredCardsForSelectedTarget()
    {
        return GameplayEngine.CalculateRequiredMovementCards(GetDistanceToSelectedTarget());
    }

    /// <inheritdoc />
    public int CalculateFinalScore()
    {
        if (Session is null)
        {
            return 0;
        }

        var extractionBonus = Session.IsExtracted ? 100 : 0;
        var enemyPoints = Session.Stats.EnemiesDefeated * 5;
        var bossPoints = Session.Stats.BossesDefeated * 50;
        var cashPoints = Session.Resources.Cash;
        var materialPoints = Session.Resources.Materials * 5;
        var intelPoints = (Session.Resources.Intel + Session.Resources.ClassifiedIntel) * 25;
        var upgradePoints = Session.Resources.WeaponUpgrades * 10;

        return extractionBonus + enemyPoints + bossPoints + cashPoints + materialPoints + intelPoints + upgradePoints;
    }

    /// <inheritdoc />
    public string GetFinalRating()
    {
        var score = CalculateFinalScore();
        return score switch
        {
            <= 99 => "Poor",
            <= 199 => "Operator",
            <= 299 => "Veteran",
            <= 399 => "Elite",
            _ => "Legend"
        };
    }

    private void ResolveMovement(GameSession session, int traveledDistance)
    {
        var destination = session.SelectedDestination;
        if (destination is null)
        {
            return;
        }

        session.Player.X = destination.X;
        session.Player.Y = destination.Y;
        session.PendingMovement = 0;
        session.SelectedDestination = null;
        session.Stats.TilesTraveled += traveledDistance;
        session.EventLog.Add($"Movement resolved to {ToCoordinate(destination.X, destination.Y)}.");

        var tileResult = _tileResolutionService.ResolveTile(session, destination);
        foreach (var entry in tileResult.EventLogEntries)
        {
            session.EventLog.Add(entry);
        }

        if (session.Player.Health <= 0)
        {
            session.IsDefeated = true;
            session.Phase = GamePhase.MatchComplete;
            session.LastMessage = "Mission failed. Operator eliminated.";
            session.EventLog.Add(session.LastMessage);
            return;
        }

        if (session.CurrentTargetExfil is not null &&
            session.Player.X == session.CurrentTargetExfil.X &&
            session.Player.Y == session.CurrentTargetExfil.Y)
        {
            session.IsExtracted = true;
            session.Phase = GamePhase.MatchComplete;
            session.LastMessage = $"Extraction successful at {session.CurrentTargetExfil.Name}.";
            session.EventLog.Add(session.LastMessage);
            return;
        }

        EndTurn(session);
    }

    private void EndTurn(GameSession session)
    {
        if (session.Phase == GamePhase.MatchComplete)
        {
            return;
        }

        session.Stats.TurnsPlayed++;
        session.TurnNumber++;
        session.CurrentHand.AddRange(_movementDeckService.DrawCards(session, 1));
        TrimHandToLimit(session, 7);
        session.LastMessage = $"Turn {session.TurnNumber} ready.";
    }

    private static void TrimHandToLimit(GameSession session, int handLimit)
    {
        while (session.CurrentHand.Count > handLimit)
        {
            var removedCard = session.CurrentHand[^1];
            session.CurrentHand.RemoveAt(session.CurrentHand.Count - 1);
            session.MovementDiscard.Add(removedCard);
            session.EventLog.Add($"Hand limit reached. Discarded {removedCard.Name}.");
        }
    }

    private static string ToCoordinate(int x, int y)
    {
        var column = (char)('A' + x);
        return $"{column}{y + 1}";
    }
}
