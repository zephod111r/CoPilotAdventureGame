# CoPilotAdventureGame

## Setup

- Install Visual Studio 2022 (Enterprise)
- Install Azure SDK tools

## Configuration
In `Game.Launcher.Console` project, open `local.settings.json` and update the following settings:
```json
{
  "Application": {
    "OpenAIKey": "<OPEN-API-KEY>"
  }
}
```

## Running the game
Run with the command:
```
dotnet run --project Game.Launcher.Console
```

Press 'Ctrl + C' to exit the game.