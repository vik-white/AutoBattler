# Operations

## Local Environment

- Current project path used while creating this wiki: `D:\Projects\AutoBattler`.
- Unity version: `6000.4.0f1`.
- Main package file: `Packages/manifest.json`.
- Important packages include Unity Entities `6.4.0`, Entities Graphics `6.4.0`, Input System `1.19.0`, URP `17.4.0`, SoftMaskForUGUI, Unity MCP, DOTween, Odin Inspector, UniRx, and Rukhanka animation.

## Files To Ignore

The repo ignores normal Unity generated outputs:

- `Library`, `Temp`, `Obj`, `Build`, `Builds`, `Logs`, `UserSettings`
- generated `*.csproj`, `*.sln`, and IDE files
- build artifacts such as `*.apk`, `*.aab`, `*.unitypackage`

Do not hand-edit generated project files. Unity/Rider will regenerate them.

## Finding Things Fast

Use `rg` first:

```powershell
rg "IResourceService" Assets/Scripts -g "*.cs"
rg --files Assets/Scripts/Modules -g "*.asmdef"
rg "public override string AssetName" Assets/Scripts/Modules -g "*.cs"
```

Useful focused roots:

- Runtime code: `Assets/Scripts/Modules`
- Editor tools: `Assets/Scripts/Editor`, plus selected editor folders under `Assets/Resources/Sector/.../Editor`
- UI prefabs: `Assets/Resources/UI/Prefabs`
- Battle resources: `Assets/Resources/Battle`
- Character resources: `Assets/Resources/Characters`
- Ability resources: `Assets/Resources/Abilities`
- Scenes: `Assets/Scenes`

## Validation

For docs-only changes, checking `git diff` is enough.

For C# or Unity serialized changes, prefer a Unity compile/open check. A typical batchmode smoke command on this machine is:

```powershell
& "C:\Program Files\Unity\6000.4.0f1\Editor\Unity.exe" -batchmode -nographics -quit -projectPath "D:\Projects\AutoBattler" -logFile "D:\Projects\AutoBattler\UnityBatchmode.log"
```

Then inspect `UnityBatchmode.log`. If Unity exits non-zero, the log is the source of truth.

If behavior changed, do a manual play-mode smoke test in `Assets/Scenes/Main.unity`:

- Start into lobby.
- Open sector/adventure flow.
- Enter battle squad setup.
- Start battle and verify victory/defeat windows still close/switch correctly.
- Confirm profile changes persist after save/load if resources, characters, quests, or sector location changed.

## Asset And Meta Rules

- Any file under `Assets` should have a matching `.meta` file managed by Unity.
- Files outside `Assets` such as this wiki do not need Unity `.meta` files.
- Keep `Resources` load paths in sync with presenter `AssetName` strings and data object resource paths.
- When moving Unity assets, do it through Unity when possible so GUID references survive.

## Known Footguns

- Folder typos are real: `Sctipts`, `Conponents`, `Invenotry`, `TweenHendler`.
- `EventDispatcher` captures handlers in its constructor; late handler registrations will not be seen by an already-created dispatcher.
- The DI aggregator keeps a list of containers. Disposed environment containers are cleared, not removed.
- `WindowViewFactory` caches views by asset name and destroys them only when the factory is disposed.
- `ResourceService.Initialize()` assumes a fresh service instance; repeated calls on the same instance can re-add dictionary keys.
- `BattleEnvironment.Initialize()` currently calls `Resolve<IEventsService>().Initialize()` twice; `EventsService` guards against duplicate initialization.
- `Config<T, I>` relies on public fields and exact sheet column names.
- `ProfileService.Rest()` sets default data before load; schema changes should consider existing saved JSON.

