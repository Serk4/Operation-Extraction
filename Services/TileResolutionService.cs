namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Resolves map tile outcomes in a fixed order after movement completes.
/// </summary>
public class TileResolutionService : ITileResolutionService
{
    private readonly IEncounterDeckService _encounterDeckService;
    private readonly ILootDeckService _lootDeckService;
    private readonly ICombatService _combatService;

    public TileResolutionService(
        IEncounterDeckService encounterDeckService,
        ILootDeckService lootDeckService,
        ICombatService combatService)
    {
        _encounterDeckService = encounterDeckService;
        _lootDeckService = lootDeckService;
        _combatService = combatService;
    }

    /// <inheritdoc />
    public TileResolutionResult ResolveTile(GameSession session, MapTile tile)
    {
        var result = new TileResolutionResult
        {
            TileType = tile.TileType
        };

        result.EventLogEntries.Add($"Tile check: {tile.TileType} at {ToCoordinate(tile.X, tile.Y)}.");

        var shouldDrawLoot = false;
        if (tile.POI is not null)
        {
            result.PoiTriggered = true;
            result.EventLogEntries.Add($"POI resolved: {tile.POI.Name}.");

            var coordinateKey = $"{tile.X}:{tile.Y}";
            if (session.VisitedPoiCoordinates.Add(coordinateKey))
            {
                if (tile.TileType == TileType.DynamicPOI)
                {
                    session.Stats.DynamicPoisVisited++;
                }
                else if (tile.TileType == TileType.POI)
                {
                    session.Stats.PoisVisited++;
                }
            }

            shouldDrawLoot = true;
        }

        var encounterCard = _encounterDeckService.DrawEncounter(session);
        _encounterDeckService.DiscardEncounter(session, encounterCard);

        if (encounterCard.Type != EncounterCardType.Nothing)
        {
            result.EncounterTriggered = true;
            result.EventLogEntries.Add($"Encounter draw: {encounterCard.Name}.");
        }
        else
        {
            result.EventLogEntries.Add("Encounter draw: Nothing.");
        }

        switch (encounterCard.Type)
        {
            case EncounterCardType.EnemyPatrol:
                ResolveEnemyEncounter(session, result, "Enemy Patrol", health: 10, damage: 2, isBoss: false);
                shouldDrawLoot = true;
                break;
            case EncounterCardType.MilitiaSquad:
                ResolveEnemyEncounter(session, result, "Militia Squad", health: 16, damage: 3, isBoss: false);
                shouldDrawLoot = true;
                break;
            case EncounterCardType.MercenarySquad:
                ResolveEnemyEncounter(session, result, "Mercenary Squad", health: 22, damage: 5, isBoss: false);
                shouldDrawLoot = true;
                break;
            case EncounterCardType.BossEncounter:
                ResolveEnemyEncounter(session, result, "Boss Encounter", health: 50, damage: 8, isBoss: true);
                shouldDrawLoot = true;
                break;
            case EncounterCardType.SupplyCache:
                result.EncounterResult = "Supply cache secured.";
                result.EventLogEntries.Add(result.EncounterResult);
                shouldDrawLoot = true;
                break;
            case EncounterCardType.SafeHouse:
                session.Player.Health = Math.Min(session.SelectedHero.MaxHealth, session.Player.Health + 8);
                result.EncounterResult = "Safe house recovered 8 health.";
                result.EventLogEntries.Add(result.EncounterResult);
                shouldDrawLoot = true;
                break;
        }

        if (!result.MatchEnded && shouldDrawLoot)
        {
            var lootCard = _lootDeckService.DrawLoot(session);
            _lootDeckService.DiscardLoot(session, lootCard);
            result.LootAwarded = lootCard;
            ApplyLoot(session, lootCard, result.EventLogEntries);
        }

        result.EventLogEntries.Add("Turn resolved.");
        return result;
    }

    private void ResolveEnemyEncounter(
        GameSession session,
        TileResolutionResult result,
        string enemyName,
        int health,
        int damage,
        bool isBoss)
    {
        session.Phase = GamePhase.Combat;
        var enemy = new Enemy
        {
            Name = enemyName,
            Health = health,
            Damage = damage,
            IsBoss = isBoss
        };

        var combatResult = _combatService.ResolveCombat(session.Player, enemy);
        session.Stats.DamageDealt += combatResult.DamageDealt;
        session.Stats.DamageTaken += combatResult.DamageTaken;

        if (combatResult.Victory)
        {
            session.Stats.EnemiesDefeated++;
            if (combatResult.BossDefeated)
            {
                session.Stats.BossesDefeated++;
            }

            result.EncounterResult = $"{enemyName} defeated.";
            result.EventLogEntries.Add(result.EncounterResult);
        }
        else
        {
            result.EncounterResult = $"{enemyName} overran the operator.";
            result.EventLogEntries.Add(result.EncounterResult);
            session.IsDefeated = true;
            session.Phase = GamePhase.MatchComplete;
            result.MatchEnded = true;
        }

        if (!result.MatchEnded)
        {
            session.Phase = GamePhase.Exploration;
        }
    }

    private static void ApplyLoot(GameSession session, LootDeckCard lootCard, ICollection<string> eventLogEntries)
    {
        switch (lootCard.Type)
        {
            case LootType.Cash:
                session.Resources.Cash += 10;
                eventLogEntries.Add("Loot: +10 Cash.");
                break;
            case LootType.Materials:
                session.Resources.Materials++;
                eventLogEntries.Add("Loot: +1 Materials.");
                break;
            case LootType.MedicalSupplies:
                session.Resources.MedicalSupplies++;
                session.Player.Health = Math.Min(session.SelectedHero.MaxHealth, session.Player.Health + 10);
                eventLogEntries.Add("Loot: +1 Medical Supplies and restored 10 health.");
                break;
            case LootType.ArmorSupplies:
                session.Resources.ArmorSupplies++;
                session.Player.Armor += 5;
                eventLogEntries.Add("Loot: +1 Armor Supplies and gained 5 armor.");
                break;
            case LootType.WeaponUpgrade:
                session.Resources.WeaponUpgrades++;
                session.Player.GunStrength += 1;
                eventLogEntries.Add("Loot: +1 Weapon Upgrade and +1 gun strength.");
                break;
            case LootType.Intel:
                session.Resources.Intel++;
                eventLogEntries.Add("Loot: +1 Intel.");
                break;
            case LootType.ClassifiedIntel:
                session.Resources.ClassifiedIntel++;
                session.Resources.Intel++;
                eventLogEntries.Add("Loot: +1 Classified Intel.");
                break;
        }
    }

    private static string ToCoordinate(int x, int y)
    {
        var column = (char)('A' + x);
        return $"{column}{y + 1}";
    }
}
