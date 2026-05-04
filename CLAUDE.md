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
