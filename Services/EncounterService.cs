namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Resolves deterministic encounter checks when players move across the map.
/// </summary>
public class EncounterService : IEncounterService
{
    public Encounter? RollEncounter(GameSession session, int x, int y)
    {
        var seed = (session.TurnNumber * 97 + x * 13 + y * 7) % 100;

        if (seed < 18)
        {
            return new Encounter
            {
                Title = "Hostile patrol",
                Description = "An enemy team has cut off the path.",
                Type = EncounterType.Enemy,
                Enemy = new Enemy { Name = "Patrol Unit", Health = 18, Damage = 7 },
                RiskLevel = 2
            };
        }

        if (seed < 28)
        {
            return new Encounter
            {
                Title = "Supply cache",
                Description = "A forgotten stash is lying nearby.",
                Type = EncounterType.SupplyCache,
                Loot = new LootItem { Name = "Med Pack", Description = "Combat supply bundle.", Value = 10 },
                RiskLevel = 1
            };
        }

        if (seed < 35)
        {
            return new Encounter
            {
                Title = "Extraction risk",
                Description = "The route is under heavy observation.",
                Type = EncounterType.ExtractionRisk,
                RiskLevel = 2
            };
        }

        return null;
    }
}
