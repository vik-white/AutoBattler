# Module Cookbook

## Add A New Service

1. Put the service interface and implementation in the closest module.
2. Register it in that module's `*ModuleDependency.cs`.
3. Make sure every environment that needs the service registers that module dependency.
4. Use constructor injection. Avoid service locator calls except in existing base classes that already use `DI.Resolve`.
5. If the service needs per-frame updates, implement `IUpdatable` and ensure it is registered as a singleton interface key.

## Add A New Window

1. Add presenter `FeatureWindow : WindowPresenter<FeatureWindowView, FeatureWindowViewModel>`.
2. Add `IFeatureWindow` with `ShowWindow(...)` and/or `CloseWindow()`.
3. Add `FeatureWindowViewModel`, usually inheriting `WindowViewModel` or `WindowViewModel<TModel>`.
4. Add `FeatureWindowView : WindowView<FeatureWindowHierarchy, FeatureWindowViewModel>` and bind fields/buttons in `UpdateViewModel`.
5. Add `FeatureWindowHierarchy : MonoBehaviour` with serialized UI references.
6. Put prefab under `Assets/Resources/UI/Prefabs/...` and set `AssetName` to the path without `Assets/Resources` and extension.
7. Register presenter, view model, and view in the module dependency.

## Add A Repeated UI Element

1. Create `ElementHierarchy`, `ElementViewModel`, `ElementView`.
2. Add a factory inheriting the existing local factory pattern and give it an `AssetName`.
3. Register factory, view model, and view.
4. Instantiate through the factory from the parent view/view model instead of direct `GameObject.Instantiate`.

## Add A New Event Type

1. Define an event data type near the owning module or an infrastructure module if it crosses module boundaries.
2. Add `class SomeHandler : EventHandler<SomeEvent>`.
3. Register it as `IEventHandler` in the module dependency.
4. Dispatch with `IEventDispatcher.Dispatch(new SomeEvent(...))`.
5. If `EventDispatcher` was already resolved before registration, the new handler will not be included. In practice, register handlers before the first service resolves the dispatcher.

## Change Profile Data

1. Update `ProfileData` or related infrastructure data under `ProfileModule/Infrastructure`.
2. Update default values in `ProfileService.Rest()`.
3. Add or update event types/handlers so runtime services persist changes into `ProfileService.Data`.
4. Check `ProfileService.Save()` and `Load()` compatibility with existing JSON in `Application.persistentDataPath/Profile.json`.
5. Use `ProfileTools` in `ProfileModule/Editor` if the change affects editor profile utilities.

## Add Or Change A Config Column

1. Add a public field to the target config data class under `Core/ConfigModule/Scripts/Data`.
2. Make the Google Sheet column name match the field name exactly.
3. If parsing is not primitive, implement or update `ICustomJsonParser.Parse`.
4. Reload the `ConfigsLoader` asset in Unity through its Odin `Load` button.
5. Commit the changed serialized config asset if Unity marks it dirty.

## Add A New Config Table

1. Add `Config<RowData, IRowData>` private field in `ConfigsLoader`.
2. Add property to `IConfigs` and `ConfigsLoader`.
3. Add row data class/interface under `Core/ConfigModule/Scripts/Data`.
4. The sheet tab name should match the private field name with the first letter capitalized.
5. Update asmdef references only if the data type lives outside `Config`.

## Add A New Environment

1. Add a value to `EnvironmentType`.
2. Create `NewEnvironment : Environment`.
3. Register needed module dependencies in `Register()`.
4. Use `Initialize()` for additive scene loading and initial state switch.
5. Use `Release()` for state exit and scene unload.
6. Register it in `Bootstrap` with `.Add<NewEnvironment>(EnvironmentType.New)`.

## Add A New Local State

1. Add a marker interface such as `IFeatureState : IState` if one does not exist.
2. Add specific state interface `IFeatureStartState : IFeatureState`.
3. Implement `Enter()` and `Exit()`.
4. Register `IStateMachine<IFeatureState>`, `IStateFactory<IFeatureState>`, and the state interface in the module dependency.
5. Switch through `IStateMachine<IFeatureState>.SwitchState<ISpecificState>()`.

## Add A Battle ECS System

1. Put components and systems in the nearest battle subfolder.
2. Choose the right group from `BattleModule/Scripts/Groups/Groups.cs`.
3. Add `[UpdateInGroup(typeof(...))]`, plus `UpdateBefore/UpdateAfter` or `OrderFirst/OrderLast` only when needed.
4. Use `RequireForUpdate<T>()` in `OnCreate` if the system should be dormant without a marker/config component.
5. Use `SceneEntity` for entities that should be cleaned with the battle scene.
6. If a system must run during squad setup while time is paused, it needs to be in one of the groups manually updated by `BattleSystemGroup.AllowSetupWhilePaused`.

## Add A Battle Prefab To ECS

1. Ensure the scene has a `PrefabRegistry` authoring object.
2. In a baker, call `baker.RegisterPrefab(prefab)` to append the prefab entity to the registry buffer.
3. Store the returned index in config/blob/component data.
4. Spawn through the existing prefab buffer patterns in battle create/event systems.

## Change Resource Or Reward Logic

1. Resource enum/data lives under `ResourceModule/Infrastructure`.
2. Runtime amounts are managed by `ResourceService`.
3. Adding or spending resources should dispatch `ChangeResourceEvent`.
4. Profile persistence for resources is handled by `ChangeResourceProfileHandler`.
5. Rewards are created by `RewardFactory` and applied by `RewardService`.

## Change Quest Logic

1. Quest config data lives in `QuestData`.
2. Quest runtime creation goes through `QuestFactory`.
3. Quest progress is event-driven through handlers in `QuestsModule/Scripts/EventHandlers`.
4. Quest profile entries are created through `CreateQuestProfileEvent`.
5. Claim/progress persistence is handled by profile event handlers.

