namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Resolves deterministic encounter checks when players move across the map.
/// </summary>
public class EncounterService : IEncounterService
{
    private readonly Random _random = new();

    public Encounter? RollEncounter(GameSession session, int x, int y)
    {
        var roll = _random.Next(1, 101);
        if (roll <= 40)
        {
            return null;
        }

        if (roll <= 70)
        {
            var enemyTierRoll = _random.Next(1, 101);
            var enemy = enemyTierRoll switch
            {
                <= 60 => new Enemy { Name = "Tier 1 Patrol", Health = 10, Damage = 2 },
                <= 92 => new Enemy { Name = "Tier 2 Mercenary", Health = 20, Damage = 4 },
                _ => new Enemy { Name = "Boss Squad", Health = 50, Damage = 8, IsBoss = true }
            };

            return new Encounter
            {
                Title = "Enemy encounter",
                Description = "Hostiles engage your position.",
                Type = EncounterType.Enemy,
                Enemy = enemy,
                RiskLevel = enemy.IsBoss ? 3 : 2
            };
        }

        if (roll <= 90)
        {
            var lootRoll = _random.Next(1, 101);
            var loot = lootRoll switch
            {
                <= 35 => new LootItem { Name = "Cash Bundle", Description = "Secure funds and intel notes.", Value = 10 },
                <= 65 => new LootItem { Name = "Medical Supplies", Description = "Recover 10 health.", Value = 10 },
                <= 90 => new LootItem { Name = "Armor Supplies", Description = "Reinforce armor by 5.", Value = 10 },
                _ => new LootItem { Name = "Weapon Upgrade", Description = "Increase gun strength by 1.", Value = 20 }
            };

            return new Encounter
            {
                Title = "Loot discovery",
                Description = $"Found {loot.Name}.",
                Type = EncounterType.SupplyCache,
                Loot = loot,
                RiskLevel = 1
            };
        }

        return new Encounter
        {
            Title = "Special event",
            Description = "Field intel improved your weapon handling (+1 Gun Strength).",
            Type = EncounterType.ExtractionRisk,
            RiskLevel = 2
        };
    }
}
