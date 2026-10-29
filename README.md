# Operation Extraction

Operation Extraction is a mobile-first ASP.NET Core 9 Blazor MVP for a turn-based extraction card game inspired by DMZ-style tactical gameplay.

## Features
- 10x10 SVG tactical map
- Hero selection with twelve operators
- In-memory match state with no database or auth requirements
- Rules-based 36-card deck (movement, combat, utility)
- Turn flow with encounter rolls, combat, loot effects, and deck reshuffle
- Coordinate-indexed board with tactical markers for infil, exfil, POIs, and dynamic locations
- In-match hero card with base stats and live updated stats
- Responsive mobile-first interfaces using Bootstrap 5

## Prerequisites
- .NET 9 SDK
- A modern browser

## Run locally
1. Restore dependencies:
   dotnet restore
2. Start the app:
   dotnet run
3. Open the local URL printed by the CLI, typically https://localhost:5001 or http://localhost:5000.

## Project structure
- /Components - Razor components and app shell
- /Pages - secondary application page files
- /Models - domain models and game state entities
- /Services - dependency-injected gameplay services
- /GameEngine - deterministic rules and turn logic
- /Data - static game catalog data
- /docs - product and design notes

## Notes
- The application is intentionally in-memory only.
- No authentication, database, or background services are required.
