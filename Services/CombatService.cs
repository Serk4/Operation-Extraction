namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Applies deterministic combat outcomes for the MVP session.
/// </summary>
public class CombatService : ICombatService
{
    public CombatResult ResolveCombat(Player player, Enemy enemy, Card card)
    {
        var baseDamage = Math.Max(0, card.Damage);
        var finalDamage = baseDamage + (enemy.IsBoss ? 2 : 0);
        var effectiveArmor = Math.Min(player.Armor, finalDamage);
        var healthLoss = Math.Max(0, finalDamage - effectiveArmor);

        player.Armor -= effectiveArmor;
        player.Health = Math.Max(0, player.Health - healthLoss);

        return new CombatResult
        {
            DamageDealt = finalDamage,
            ArmorAbsorbed = effectiveArmor,
            HealthLost = healthLoss,
            Victory = healthLoss >= enemy.Health
        };
    }

    public class CombatResult
    {
        public int DamageDealt { get; set; }
        public int ArmorAbsorbed { get; set; }
        public int HealthLost { get; set; }
        public bool Victory { get; set; }
    }
}
