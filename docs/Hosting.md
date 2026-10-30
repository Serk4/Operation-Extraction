# Operation Extraction Hosting

## Hosting Model Analysis

The original implementation used **Blazor Server**:

- `Operation-Extraction.csproj` used `Microsoft.NET.Sdk.Web`
- `Program.cs` used `WebApplication` + `AddInteractiveServerComponents`
- pages used `@rendermode InteractiveServer`
- app host used `blazor.web.js`

That model is not compatible with static-only hosting (GitHub Pages).

## Migration Plan (Blazor Server -> Blazor WebAssembly)

### Goals

- Single-player only
- No authentication
- No database
- Preserve current gameplay services and rules
- Deploy as static assets on GitHub Pages

### Plan

1. Convert project SDK/runtime to **Blazor WebAssembly**
2. Move host shell to static `wwwroot/index.html`
3. Replace server startup with `WebAssemblyHostBuilder`
4. Remove server-only render mode directives
5. Keep all existing in-memory services and game logic in client DI
6. Ensure app navigation works under GitHub Pages base path
7. Add GitHub Actions workflow for publish/deploy
8. Document local run and deployment process

## Refactor Summary Implemented

### 1) Runtime and startup conversion

- Converted project SDK to `Microsoft.NET.Sdk.BlazorWebAssembly`
- Updated startup to client-side host (`WebAssemblyHostBuilder`)
- Registered root components (`#app`, `HeadOutlet`)
- Preserved all game services via existing service registration

### 2) Static host shell

- Added `wwwroot/index.html`
- Switched script to `_framework/blazor.webassembly.js`
- Kept Bootstrap and existing app CSS loading

### 3) Component routing updates

- Removed `@rendermode InteractiveServer`
- Converted hard-root links (`/path`) to base-aware links (`path`)
- Added router NotFound fallback view

### 4) GitHub Pages deployment

- Added workflow: `.github/workflows/deploy-pages.yml`
- Workflow:
  - restore + publish
  - set base href to repo path
  - copy `index.html` to `404.html` for deep-link refresh routing
  - deploy artifact via GitHub Pages actions

## State Management

State remains fully client-side and in-memory:

- `GameStateService` (scoped DI in WASM runtime)
- in-memory map/session/deck state
- no persistence layer

## GitHub Pages Notes

- GitHub Pages serves static content only.
- Base href is rewritten to `/<repo-name>/` during CI deploy.
- A `404.html` copy of `index.html` supports client-side route refresh.
