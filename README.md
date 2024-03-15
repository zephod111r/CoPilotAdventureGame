# CoPilotAdventureGame

## Setup

- Install Visual Studio 2022 (Enterprise)
- Install Azure SDK tools

## Configuration
In `Game.Launcher.Console` (Console version) or `Game.Functions` (Web version) project, open `local.appsettings.json` and update the following settings:
```json
{
    "OpenAIKey": "<OPEN-API-KEY>",
    "AzureWebJobsStorage": "<AZURE-STORAGE-CONNECTION-STRING>",
    "StaticFiles": "local"
}
```

Setting `StaticFiles` to `local` will use the local static files. Otherwise it will use the static files from Azure Blob Storage.

## Running the game

### Console version

Run with the command:
```
dotnet run --project Game.Launcher.Console
```

Press 'Ctrl + C' or type command `exit` to exit the game.

### Web version
Open in browser:
- Local dev version: http://localhost:7071/
- Deployed version: https://copilotadvgamefunctions.azurewebsites.net/

You can add a query parameter `?theme=<game theme>` to the URL to change the game theme e.g. https://copilotadvgamefunctions.azurewebsites.net/?theme=StarWars
