namespace Operation_Extraction.Data;

using Operation_Extraction.Models;

/// <summary>
/// Static catalog for the hero roster shown during match setup.
/// </summary>
public static class HeroCatalog
{
    public static IReadOnlyList<Hero> CreateHeroes()
    {
        return new List<Hero>
        {
            new() { Name = "Rook", Role = "Frontline", Specialty = "Shield Wall", AccentColor = "#42a5f5", MaxHealth = 120, Armor = 15, Movement = 3, GunStrength = 6 },
            new() { Name = "Viper", Role = "Recon", Specialty = "Silent Entry", AccentColor = "#7dd3fc", MaxHealth = 100, Armor = 10, Movement = 4, GunStrength = 7 },
            new() { Name = "Sable", Role = "Assault", Specialty = "Shock Tactics", AccentColor = "#f472b6", MaxHealth = 110, Armor = 8, Movement = 3, GunStrength = 6 },
            new() { Name = "Atlas", Role = "Support", Specialty = "Field Resupply", AccentColor = "#fbbc05", MaxHealth = 130, Armor = 12, Movement = 2, GunStrength = 5 },
            new() { Name = "Vega", Role = "Stealth", Specialty = "Ghost Approach", AccentColor = "#34d399", MaxHealth = 100, Armor = 7, Movement = 4, GunStrength = 7 },
            new() { Name = "Nova", Role = "Engineer", Specialty = "Rapid Breach", AccentColor = "#60a5fa", MaxHealth = 105, Armor = 12, Movement = 3, GunStrength = 6 },
            new() { Name = "Kite", Role = "Scout", Specialty = "Long View", AccentColor = "#a78bfa", MaxHealth = 95, Armor = 6, Movement = 4, GunStrength = 6 },
            new() { Name = "Mara", Role = "Medic", Specialty = "Battlefield Care", AccentColor = "#fca5a5", MaxHealth = 115, Armor = 10, Movement = 3, GunStrength = 5 },
            new() { Name = "Quill", Role = "Tactician", Specialty = "Recon Grid", AccentColor = "#c084fc", MaxHealth = 100, Armor = 9, Movement = 3, GunStrength = 6 },
            new() { Name = "Hawk", Role = "Marksman", Specialty = "Hold the Lane", AccentColor = "#fdba74", MaxHealth = 105, Armor = 8, Movement = 3, GunStrength = 8 },
            new() { Name = "Warden", Role = "Defense", Specialty = "Fortified", AccentColor = "#4ade80", MaxHealth = 135, Armor = 18, Movement = 2, GunStrength = 5 },
            new() { Name = "Echo", Role = "Operator", Specialty = "Fast Exit", AccentColor = "#2dd4bf", MaxHealth = 98, Armor = 7, Movement = 5, GunStrength = 7 }
        };
    }
}
