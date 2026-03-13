<h1 align="center">⚔️ Bellerophon</h1>
<p align="center">A personal hobby project built with Unity.</p>

---
Overview
Bellerophon is a Unity game project featuring a player character, enemy pool system, and various gameplay systems built with a clean dependency injection architecture using Extenject (Zenject).
---
Screenshots
> *Coming soon*
---
Tech Stack
	Package	Purpose
🎮	Unity	Game engine
💉	Extenject (Zenject)	Dependency injection framework
🎬	DOTween	Tweening & animation
---
Architecture
The project follows a Facade + Object Pool pattern powered by Zenject's `MonoPoolableMemoryPool`.
```
GameInstaller
├── PlayerFacade          (SubContainer per instance)
├── EnemyFacadePool       (MonoPoolableMemoryPool per EnemyType)
│   ├── Satyr
│   ├── Minotaur
│   └── Hydra
└── Signals               (Decoupled event system)
```
Enemy Pool
Each enemy type gets its own `MonoPoolableMemoryPool`. Enemies are spawned and despawned via `IEnemySpawnService`, and each instance lives inside a Zenject SubContainer so its internal dependencies (movement, health, etc.) are properly injected and isolated.
```csharp
// Spawn an enemy anywhere in the codebase
_spawnService.Spawn(EnemyTypes.Satyr, position);

// Enemies can despawn themselves
enemyFacade.Despawn();
```
Settings
Game configuration is driven by ScriptableObjects (`PlayerSettings`, `EnemyPrefabSettings`) that are installed into Zenject's container at startup — no singletons, no static state.
---
Getting Started
Requirements
Unity 6000.x or newer
Extenject
DOTween
Setup
Clone the repository
```bash
   git clone https://github.com/yourusername/bellerophon.git
   ```
Open the project in Unity
Install packages via Unity Package Manager or import from the Asset Store:
Extenject
DOTween (run the setup wizard after import)
Open the main scene and hit Play
---
Project Structure
```
Assets/
├── Bellepron/
│   ├── Enemy/
│   │   ├── EnemyFacade.cs
│   │   ├── EnemyInstaller.cs
│   │   ├── EnemySpawnService.cs
│   │   ├── EnemySpawner.cs
│   │   └── EnemyTypes.cs
│   ├── Player/
│   │   ├── PlayerFacade.cs
│   │   ├── PlayerSettings.cs
│   │   └── Controllers/
│   ├── Installers/
│   │   ├── GameInstaller.cs
│   │   └── GameSignalsInstaller.cs
│   └── Settings/
│       └── EnemyPrefabSettings.cs
```
---
License
Personal hobby project — not licensed for redistribution.
