# Ninja vs Monster — rules

- Windows Forms, C#, .NET 8 Windows target; 800 × 450 client area.
- Preserve the existing solution and project; no third-party dependencies.
- States: Title, Play, Pause, Over.
- Space jumps only while grounded; no double jump.
- Esc toggles pause and resume.
- One monster approaches from the right; score increases once after it passes the player.
- Monster speed increases with score; spawn interval remains fixed.
- Collision ends the run immediately; no HP/lives.
- High score persists under LocalApplicationData/NinjaVsMonster/highscore.txt.
- Audio is optional; missing sound assets must not block gameplay.
