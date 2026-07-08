# Module Map

## Core Modules

| Module | Main path | Purpose |
| --- | --- | --- |
| `Application` | `Assets/Scripts/Modules/Core/ApplicationModule` | Coroutines, extensions, math/color/camera helpers. |
| `AssetLoader` | `Assets/Scripts/Modules/Core/AssetLoaderModule` | `IAssetLoader` backed by `Resources.Load<GameObject>`. |
| `Camera` | `Assets/Scripts/Modules/Core/CameraModule` | Camera positioning/follow/control service. |
| `Config` | `Assets/Scripts/Modules/Core/ConfigModule` | `ConfigsLoader`, config data models, enums/types. |
| `DI` | `Assets/Scripts/Modules/Core/DIModule` | Custom DI container, aggregator, scene context update. |
| `ECS` | `Assets/Scripts/Modules/Core/ECSModule` | ECS helpers, prefab registry, scene entity/destroy components. |
| `Entity` | `Assets/Scripts/Modules/Core/EntityModule` | Base entity abstraction. |
| `Environment` | `Assets/Scripts/Modules/Core/EnvironmentModule` | Environment lifecycle, setup, environment state machine. |
| `Event` | `Assets/Scripts/Modules/Core/EventModule` | Type-based event dispatcher and handler base. |
| `LoadingScreen` | `Assets/Scripts/Modules/Core/LoadingScreenModule` | Loading screen window/service. |
| `Location` | `Assets/Scripts/Modules/Core/LocationModule` | Current location provider. |
| `Mvvm` | `Assets/Scripts/Modules/Core/MvvmModule` | View/view-model factories and base binding helpers. |
| `Pathing` | `Assets/Scripts/Modules/Core/PathingModule` | Bezier path data used by sector traversal. |
| `StateMachine` | `Assets/Scripts/Modules/Core/StateMachineModule` | Generic state machine and state factory. |
| `Window` | `Assets/Scripts/Modules/Core/WindowModule` | Window presenter, manager, UI root/layers. |

## Gameplay Modules

| Module | Main path | Purpose |
| --- | --- | --- |
| `Bank` | `Assets/Scripts/Modules/Gameplay/BankModule` | Summon flow and summon window. |
| `Battle` | `Assets/Scripts/Modules/Gameplay/BattleModule` | Battle states, squad UI, battle UI, ECS systems/components/config bakers. |
| `Character` | `Assets/Scripts/Modules/Gameplay/CharacterModule` | Character models, factory/service, upgrade/ascend/skills windows. |
| `Character.Infrastructure` | `Assets/Scripts/Modules/Gameplay/CharacterModule/Infrastructure` | Character profile/event data shared by modules. |
| `Cheat` | `Assets/Scripts/Modules/Gameplay/CheatModule` | Cheat service/window and map selection shortcuts. |
| `Events` | `Assets/Scripts/Modules/Gameplay/EventsModule` | Game event models, event list/window, event item UI. |
| `Lobby` | `Assets/Scripts/Modules/Gameplay/LobbyModule` | Tavern scene environment and lobby UI/state. |
| `Meta` | `Assets/Scripts/Modules/Gameplay/MetaModule` | Meta/roster style window and item UI. |
| `Profile` | `Assets/Scripts/Modules/Gameplay/ProfileModule` | Profile load/save and profile event handlers. |
| `Profile.Infrastructure` | `Assets/Scripts/Modules/Gameplay/ProfileModule/Infrastructure` | Profile interfaces, data, and profile events. |
| `Quests` | `Assets/Scripts/Modules/Gameplay/QuestsModule` | Quest factory/registry, quest event handlers, quest item UI. |
| `Resource` | `Assets/Scripts/Modules/Gameplay/ResourceModule` | Runtime resource service and resource UI element. |
| `Resource.Infrastructure` | `Assets/Scripts/Modules/Gameplay/ResourceModule/Infrastructure` | Resource enum/data shared by other modules. |
| `Reward` | `Assets/Scripts/Modules/Gameplay/RewardModule` | Reward factory/service and reward windows/items. |
| `Sector` | `Assets/Scripts/Modules/Gameplay/SectorModule` | Sector scene environment, map service, player path traversal, sector UI. |

## Environment Composition

`CoreEnvironment` registers foundational modules once:

- `AssetLoaderModuleDependency`
- `EntityModuleDependency`
- `EventModuleDependency`
- `CameraModuleDependency`
- `MvvmModuleDependency`
- `WindowModuleDependency`
- `EnvironmentModuleDependency`
- `LocationModuleDependency`

`LobbyEnvironment` registers:

- loading, lobby, cheat, profile, resource, sector, character, reward, bank, meta, quests, events.

`SectorEnvironment` registers:

- loading, sector, profile, resource, character, cheat.

`BattleEnvironment` registers:

- loading, battle, profile, resource, character, sector, reward, quests, events, cheat.

## Common Dependency Files

Look for `*ModuleDependency.cs` first when a type is not resolving:

- `BattleModuleDependency.cs`: battle states, services, squad/battle/result windows.
- `CharacterModuleDependency.cs`: character service/factory and character-related windows.
- `ProfileModuleDependency.cs`: profile service and profile event handlers.
- `QuestsModuleDependency.cs`: quest factory/registry and quest handlers.
- `RewardModuleDependency.cs`: reward factory/service and reward UI.
- `SectorModuleDependency.cs`: sector state machine/window/service.

## UI Files Pattern

Most windows follow this shape:

- `FeatureWindow.cs`: presenter and public interface.
- `ViewModel/FeatureWindowViewModel.cs`: view model and actions.
- `View/FeatureWindowView.cs`: binding from view model to hierarchy.
- `Hierarchy/FeatureWindowHierarchy.cs`: MonoBehaviour references to Unity UI objects.
- Optional `Factory/*ViewFactory.cs` for repeated item elements.

## Battle Folder Guide

Important battle subfolders:

- `States`: battle state machine states.
- `Groups`: ECS system group order.
- `Character`: character ECS components/systems/configs.
- `Core`: movement, collisions, cooldown, lifetime, time, physics, VFX.
- `Location`: grid, squad placement, map/static/flow enemy initialization.
- `Skills`: skill trigger logic, armaments, effects, statuses, stats.
- `Events`: ECS event components/systems bridging simulation outcomes to UI/state.
- `UI`: battle, squad, victory, defeat windows.

