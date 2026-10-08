# AR Castle Defense

An Augmented Reality (AR) tower defense game developed with Unity and AR Foundation. Track an AR image target to anchor your castle into the real world, then tap the floor to deploy defensive cannons and protect your castle from waves of incoming enemies.

## How to Play

1. Launch the app and tap Play.
2. Scan the AR target card to spawn and position your Castle.
3. Tap detected surfaces/planes on the floor to place defensive Cannons.
   - Note: Each cannon costs 5 coins. You can have a maximum of 3 active cannons on the field.
4. Earn +5 coins per completed wave and defend your castle through all 5 rounds to win.

## Game Mechanics

- Castle Anchor (AR Image Tracking): The central castle spawns and tracks automatically upon scanning the designated physical AR image target.
- Tap-to-Place Cannons: Touch flat surfaces in your AR environment to spawn cannons that automatically detect, aim, and shoot at enemies.
- Enemy Waves: Survive through 5 progressive rounds of increasing difficulty. Enemy skeletons spawn around the castle and pathfind directly to attack it.
- Economy and Progression System:
  - Starting Balance: 10 coins.
  - Cannon Cost: 5 coins per cannon.
  - Active Limit: Maximum of 3 active cannons on the scene at any time.
  - Round Reward: +5 coins awarded upon completing each round.
 
  - # Key Scripts Architecture

| Script | Description |
| :--- | :--- |
| CastleBehavior.cs | Tracks castle health, visual damage states, round progression (1 to 5), difficulty scaling, and grants round-completion coin rewards. |
| CoinManager.cs | Central singleton economy controller. Tracks coin balance, validates cannon placement transactions, and enforces the 3-cannon limit. |
| CannonPlacementManager.cs | Handles touch inputs and AR raycasting against detected planes. Validates economy rules via CoinManager before instantiating a cannon. |
| CannonBehavior.cs | Cannon combat AI: scans for nearby enemies, calculates obstacle-clear trajectories, and fires projectiles. |
| Bullet.cs | Projectile logic: moves forward, deals damage on collision with skeletons, and notifies the UI manager when enemies are defeated. |
| SkeletonBehavior.cs | Enemy AI: pathfinding towards the castle, camera-facing health bar (Billboard), and dealing damage upon arrival. |
| EnemySpawner.cs | Handles skeleton spawning in a radial pattern around the castle, selecting between standard and sword skeleton prefabs. |
| GameUIManager.cs | Controls UI flow (Main Menu, Instructions, In-Game HUD, Game Over / Victory screens) and triggers game resets. |
| ARImageReset.cs | Manages image tracking resets to re-align or re-anchor the castle when needed. |

