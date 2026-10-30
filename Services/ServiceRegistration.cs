namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Shared helper for service configuration defaults used by the application.
/// </summary>
public static class ServiceRegistration
{
    public static void ConfigureGameServices(IServiceCollection services)
    {
        services.AddSingleton<MapService>();
        services.AddSingleton<IMapService>(sp => sp.GetRequiredService<MapService>());

        services.AddSingleton<MovementDeckService>();
        services.AddSingleton<IMovementDeckService>(sp => sp.GetRequiredService<MovementDeckService>());

        services.AddSingleton<CombatService>();
        services.AddSingleton<ICombatService>(sp => sp.GetRequiredService<CombatService>());

        services.AddSingleton<EncounterDeckService>();
        services.AddSingleton<IEncounterDeckService>(sp => sp.GetRequiredService<EncounterDeckService>());

        services.AddSingleton<LootDeckService>();
        services.AddSingleton<ILootDeckService>(sp => sp.GetRequiredService<LootDeckService>());

        services.AddSingleton<TileResolutionService>();
        services.AddSingleton<ITileResolutionService>(sp => sp.GetRequiredService<TileResolutionService>());

        services.AddScoped<GameStateService>();
        services.AddScoped<IGameStateService>(sp => sp.GetRequiredService<GameStateService>());
    }
}
