namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Applies deterministic combat outcomes for the MVP session.
/// </summary>
public class CombatService : ICombatService
{
    public CombatResult ResolveCombat(Player player, Enemy enemy, Card card)
    {
        var bonusDamage = Math.Max(0, card.Damage);
        var totalDamage = player.GunStrength + bonusDamage;
        enemy.Health = Math.Max(0, enemy.Health - totalDamage);

        var effectiveArmor = 0;
        var healthLoss = 0;
        if (enemy.Health > 0)
        {
            effectiveArmor = Math.Min(player.Armor, enemy.Damage);
            healthLoss = Math.Max(0, enemy.Damage - effectiveArmor);
            player.Armor -= effectiveArmor;
            player.Health = Math.Max(0, player.Health - healthLoss);
        }

        return new CombatResult
        {
            DamageDealt = totalDamage,
            ArmorAbsorbed = effectiveArmor,
            HealthLost = healthLoss,
            EnemyHealthRemaining = enemy.Health,
            Victory = enemy.Health <= 0
        };
    }

    public class CombatResult
    {
        public int DamageDealt { get; set; }
        public int ArmorAbsorbed { get; set; }
        public int HealthLost { get; set; }
        public int EnemyHealthRemaining { get; set; }
        public bool Victory { get; set; }
    }
}
