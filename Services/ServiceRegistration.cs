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

        services.AddSingleton<CardService>();
        services.AddSingleton<ICardService>(sp => sp.GetRequiredService<CardService>());

        services.AddSingleton<EncounterService>();
        services.AddSingleton<IEncounterService>(sp => sp.GetRequiredService<EncounterService>());

        services.AddSingleton<CombatService>();
        services.AddSingleton<ICombatService>(sp => sp.GetRequiredService<CombatService>());

        services.AddScoped<GameStateService>();
        services.AddScoped<IGameStateService>(sp => sp.GetRequiredService<GameStateService>());
    }
}
