# Star Platformer — Claude Instructions

## Git Workflow

After every meaningful unit of work, commit and push to GitHub. Never end a session with uncommitted changes.

### What counts as a commit point
- A new file or asset is added
- A feature or system is implemented or completed
- A bug is fixed
- A significant refactor or reorganization is done
- Any configuration or tooling change

### Commit message style
Use the imperative mood, short and descriptive (50 chars or less for the subject):

```
Add player movement controller
Fix collision detection on slope edges
Refactor shader uniform bindings
Remove unused texture loader
```

Do not use vague messages like "update stuff", "fix", or "wip".

### Push policy
After every commit, push immediately:

```bash
git push
```

The remote is `https://github.com/Iefts/Peak-graphics-game` (`origin/main`).

### Goal
Every meaningful state of the project should be recoverable from GitHub history. If something breaks, we can always revert to a known-good commit.

---

## Unity Project Setup

**Engine:** Unity 6 (6000.4.3f1), Universal Render Pipeline (URP)

### First-time setup
1. Open **Unity Hub → Open → Add project from disk** and select this folder.
2. Let Unity import everything (first launch takes a few minutes).
3. In the menu bar go to **Star Game → Build Level 01**.
4. Unity will generate `Assets/Scenes/Level01_StarPlatformer.unity` and save it.
5. Open the scene, press **Play** — the star spawns on the left of the ground. A/D to roll, Space or W to jump.

### Game concept
2D physics platformer similar to Getting Over It / Jump King.
- Player is a 5-pointed star with long thin arms and a small body
- A/D applies rolling torque; Space/W jumps (impulse-based, must be grounded)
- Must balance the star's tips on small circular notches and jump between them
- Falling is the punishment — no checkpoints

### Project structure
```
Assets/
  Scripts/
    Core/          CameraFollow (smooth orthographic follow)
    Player/        StarController (input/physics), StarMesh (procedural star mesh + collider)
  Editor/          StarPlatformerBuilder (builds scene via menu)
  Scenes/          Level01_StarPlatformer.unity (generated — do not hand-edit)
Packages/          manifest.json
ProjectSettings/   ProjectVersion.txt
```

### Level 01 layout
- Flat ground: x=-6 to x=6, y=0
- 5 notches (radius 0.28) rising gradually: x=7.5→17.5, y=2.2→7.4 (each +1.3 units)
- End platform at x≈23, y=8.1

### Rebuilding the scene
If the scene gets corrupted or you want a fresh start:
`Star Game → Build Level 01` — rebuilds clean from code.

### Unity 6 API notes
Use `Rigidbody2D.linearVelocity` (not `.velocity`) and `linearDamping`/`angularDamping` (not `.drag`/`.angularDrag`).
