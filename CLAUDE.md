# Peak Graphics Game — Claude Instructions

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

**Engine:** Unity 2022.3 LTS (Built-in Render Pipeline — upgrade to URP later for final graphics)

### First-time setup
1. Open **Unity Hub → Open → Add project from disk** and select this folder.
2. Let Unity import everything (first launch takes a few minutes).
3. In the menu bar go to **Peak Game → Build City Scene**.
4. Unity will generate `Assets/Scenes/Level01_City.unity` and open it.
5. Press **Play** — you spawn outside the south city gate. WASD to move, mouse to look, Space to jump, Shift to sprint.

### Project structure
```
Assets/
  Scripts/
    Core/          GameManager, InnerChamberTrigger
    Player/        FirstPersonController, CharacterClass, CharacterStats
    Character/     PeakCharacterAssembler (primitive character model)
    Roguelike/     RunData
  Editor/          CitySceneBuilder (builds the scene via menu)
  Scenes/          Level01_City.unity (generated — do not hand-edit)
Packages/          manifest.json
ProjectSettings/   ProjectVersion.txt
```

### Character classes
| Class   | Health | Speed | Jump | Role |
|---------|--------|-------|------|------|
| Warrior | 150    | 4.5   | 1.2  | Tank |
| Rogue   | 90     | 7.5   | 1.8  | Speed |
| Mage    | 80     | 5.0   | 1.4  | Magic |
| Ranger  | 110    | 6.0   | 1.6  | Ranged |

### Rebuilding the scene
If the scene gets corrupted or you want a fresh start:
`Peak Game → Build City Scene` — it always creates a clean scene from code.

### Target graphics style
Peak (the game) — stylized realism with a focus on **character models**.
Puffy jacket silhouette, chunky readable proportions, warm/cool lighting contrast.
Character model is assembled from Unity primitives in `PeakCharacterAssembler.cs`
and will be replaced with a proper mesh when the game concept is locked.
