# Survival Arena RL

Survival Arena RL is a 2D survival game and reinforcement-learning environment built in Unity and C#. The player navigates an arena, avoids pursuing enemies, and uses ranged attacks to survive.

The project explores how game mechanics, enemy navigation, and environment design can be structured for eventual training and evaluation with Unity ML-Agents.

## Current Features

* Physics-based player movement using Unity’s `Rigidbody2D`
* Mouse-based aiming and projectile attacks
* Configurable firing cooldowns
* Enemy spawning and pursuit behavior
* Obstacle-aware enemy navigation
* Flow-field pathfinding over a configurable grid
* Arena walls and movement boundaries
* Modular Unity prefabs and C# components

## Pathfinding

Enemies navigate using a flow field that divides the arena into a grid. Each cell stores information used to direct enemies toward the player while accounting for obstacles.

The current configuration uses a `40 × 20` grid with a cell size of `0.5` Unity units. Nearby obstacles are detected before navigation directions are assigned. This approach allows multiple enemies to reuse the same navigation field rather than independently computing a complete path.

## Technology

* Unity 6
* C#
* Unity Input System
* Unity 2D Physics
* Unity ML-Agents *(planned integration)*

## Project Structure

```text
Assets/
├── Scripts/          # Gameplay, navigation, and spawning logic
├── Prefabs/          # Reusable player, enemy, and projectile objects
└── Scenes/           # Unity scenes

Packages/             # Unity package configuration
ProjectSettings/      # Unity project settings
```

The exact contents of `Assets/` may vary as development continues.

## Running the Project

1. Install Unity Hub.
2. Install the Unity Editor version listed in `ProjectSettings/ProjectVersion.txt`.
3. Clone or download this repository.
4. In Unity Hub, select **Add project from disk** and choose the repository directory.
5. Open the main scene from `Assets/Scenes`.
6. Press **Play** in the Unity Editor.

## Code Sample Notes

I independently designed and implemented the gameplay systems and C# architecture in this repository. The most relevant code for review is located under `Assets/Scripts`, including the player controls, projectile system, enemy behavior, spawning logic, and flow-field navigation system.

This repository is an active personal project. The current version focuses on the game environment and navigation systems that will support future reinforcement-learning experiments.
