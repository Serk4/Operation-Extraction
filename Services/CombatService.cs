namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Applies deterministic combat outcomes for a full enemy engagement.
/// </summary>
public class CombatService : ICombatService
{
    /// <inheritdoc />
    public CombatResult ResolveCombat(Player player, Enemy enemy)
    {
        var result = new CombatResult();

        while (enemy.Health > 0 && player.Health > 0)
        {
            var dealt = Math.Max(1, player.GunStrength);
            enemy.Health = Math.Max(0, enemy.Health - dealt);
            result.DamageDealt += dealt;

            if (enemy.Health <= 0)
            {
                result.Victory = true;
                break;
            }

            var absorbed = Math.Min(player.Armor, enemy.Damage);
            player.Armor -= absorbed;
            var healthLoss = Math.Max(0, enemy.Damage - absorbed);
            player.Health = Math.Max(0, player.Health - healthLoss);

            result.ArmorAbsorbed += absorbed;
            result.DamageTaken += healthLoss;
        }

        result.BossDefeated = result.Victory && enemy.IsBoss;
        return result;
    }
}
