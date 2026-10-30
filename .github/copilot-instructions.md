# Copilot instructions for Operation-Extraction

## Build, test, and lint commands

This repo is a single .NET 9 Blazor WebAssembly app. Use the standard .NET workflow from the repository root:

- Restore dependencies: `dotnet restore`
- Build the app: `dotnet build`
- Run locally: `dotnet run`
- Run the default validation command for the project: `dotnet test`
- Run a single test by name when tests are added: `dotnet test --filter "FullyQualifiedName~<TestName>"`

There is no dedicated lint script or custom lint configuration in this repository. Prefer `dotnet build` and `dotnet test` for validation.

## High-level architecture

This is a mobile-first single-player extraction game built with .NET 9 and Blazor WebAssembly.

- `Program.cs` boots the WebAssembly host and configures dependency injection.
- `Services/ServiceRegistration.cs` wires the game services into the container.
- `Models/GameModels.cs` is the domain layer: heroes, cards, map tiles, POIs, and the in-memory `GameSession` state.
- `GameEngine/GameplayEngine.cs` centralizes movement calculations and reusable game rules (for example, distance and required movement-card logic).
- `Services/GameStateService.cs` is the main orchestration layer for hero selection, match setup, turn flow, movement resolution, combat, and extraction outcomes.
- `Services/*` contains the gameplay subsystems for map generation, decks, encounters, loot, combat, and tile resolution.
- `Components/Pages/Game.razor` is the main gameplay screen and renders the tactical 10x10 map, hero stats, phase state, and match result UI.
- `Data/HeroCatalog.cs` defines the static roster shown during hero selection.
- `docs/PRD.md` and `docs/GameRules.md` describe the MVP goals and the intended game loop; keep code aligned with that single-player, in-memory design.

## Key conventions

- The app is intentionally in-memory only. Do not add database persistence, auth, or server-side state stores unless the task explicitly calls for it.
- Game state is centralized around the active `GameSession` and the `GameStateService.Session` singleton-scoped instance. Keep mutations in service and model code rather than spreading state across components.
- Keep map logic coordinate-based: the board is a fixed 10x10 grid with A1 through J10 coordinate conventions already used in the UI and game logic.
- Respect game phases before applying mutations. The flow is explicit (`Lobby`/`Infil`/`Exploration`/`MatchComplete` and related states), and state changes should be gated by the current phase.
- Reuse the movement and rule helpers in `GameplayEngine` instead of reimplementing distance and card calculations in components or services.
- The project is mobile-first and uses Razor Components + Bootstrap styling. Keep UI changes consistent with the existing structure under `Components/` and the current SVG map rendering approach.
- Avoid broad refactors that expand scope beyond the MVP. The repo’s design goal is a lightweight tactical prototype, not a production game platform.

## Working expectations

- Prefer small, service-oriented changes that fit the current architecture.
- If a change affects turn logic, map generation, or rules resolution, validate it through the relevant gameplay flow and `dotnet build`/`dotnet test`.
- New behavior should remain deterministic and in-memory; do not create hidden runtime dependencies.
