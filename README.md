# Ninja vs Monster

A 2D endless runner built with C#, Windows Forms, and .NET 8.

## Requirements
- Windows and .NET 8
- Visual Studio 2022 with the .NET desktop development workload

## Run
1. Open `My2DGame.sln` in Visual Studio 2022.
2. Press **F5**.
3. Select **START GAME** or press **Enter**.

## Controls
- **Space**: Jump (grounded only; no double jump)
- **Esc**: Pause / resume
- **Enter**: Start or restart

## Rules
Avoid purple monsters. Passing a monster adds one point. Monster speed rises as score increases, while the spawn interval stays fixed. A collision ends the run immediately. The high score is stored in `%LOCALAPPDATA%\NinjaVsMonster\highscore.txt`. No BGM is used; Windows system sounds provide optional effects.

## Build
```powershell
dotnet restore My2DGame.sln
dotnet build My2DGame.sln --configuration Release
```
