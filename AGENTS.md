# AutoBattler Agent Guide

This file is the entry point for coding agents working in this Unity project.
The local wiki lives in [docs/agent-wiki/README.md](docs/agent-wiki/README.md).

## Read First

1. Start with [docs/agent-wiki/README.md](docs/agent-wiki/README.md) for the project map.
2. Read [docs/agent-wiki/architecture.md](docs/agent-wiki/architecture.md) before changing startup, DI, windows, events, configs, or battle ECS.
3. Use [docs/agent-wiki/module-cookbook.md](docs/agent-wiki/module-cookbook.md) for common feature-change recipes.
4. Use [docs/agent-wiki/operations.md](docs/agent-wiki/operations.md) for Unity version, packages, scenes, assets, and validation notes.

## Agent Rules

- This is a Unity 6000.4.0f1 project. Prefer opening/validating through Unity when behavior or serialization matters.
- Main code is under `Assets/Scripts/Modules`; generated IDE files (`*.csproj`, `*.sln`) are ignored and should not be hand-edited.
- Keep `.meta` files with Unity assets. For docs-only changes, no `.meta` is needed outside `Assets`.
- Preserve existing folder spellings such as `Sctipts` and `Conponents`; they are real paths in this repo.
- Follow existing module patterns: register dependencies in `*ModuleDependency`, open UI through `WindowPresenter`, and route state changes through the relevant state machine.
- Avoid broad refactors while fixing a feature. Many modules are coupled through the custom DI aggregator and Unity serialized assets.

