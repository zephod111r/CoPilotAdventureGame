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

- Run with the command:
    ```
    dotnet run --project Game.Launcher.Console
    ```
- Press 'Ctrl + C' or type command `exit` to exit the game.

### Local Web version

- Run locally with the command:
    ```
    dotnet run --project Game.Functions
    ```
- Open in browser http://localhost:7071/
