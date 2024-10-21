# Star Wars Copilot

> A developer copilot inspired by stories from a long time ago in a galaxy far away,

This repo contains an example C# copilot created using [Pieces for Developers](https://pieces.app) using the [Pieces OS .NET Client SDK](https://github.com/pieces-app/pieces-os-client-sdk-for-csharp). In particular, this uses [Pieces.Extensions.AI](https://www.nuget.org/packages/Pieces.Extensions.AI/), a nuget package that provides for support for Pieces OS using [Microsoft.Extensions.AI](https://devblogs.microsoft.com/dotnet/introducing-microsoft-extensions-ai-preview/?hide_banner=true).

## Run this copilot

To run the copilot, use the .NET CLI:

```bash
dotnet run
```

You will then get an option to choose which copilot to use - R2 oe Yoda:

```output
__    __   _______  __       __        ______      .___________. __    __   _______ .______       _______  __  
|  |  |  | |   ____||  |     |  |      /  __  \     |           ||  |  |  | |   ____||   _  \     |   ____||  | 
|  |__|  | |  |__   |  |     |  |     |  |  |  |    `---|  |----`|  |__|  | |  |__   |  |_)  |    |  |__   |  | 
|   __   | |   __|  |  |     |  |     |  |  |  |        |  |     |   __   | |   __|  |      /     |   __|  |  | 
|  |  |  | |  |____ |  `----.|  `----.|  `--'  |        |  |     |  |  |  | |  |____ |  |\  \----.|  |____ |__| 
|__|  |__| |_______||_______||_______| \______/         |__|     |__|  |__| |_______|| _| `._____||_______|(__) 
                                                                                                                                                                 
Who do you want as your copilot?
                                
> R2-D2                         
  Yoda    
```

Select your copilot and ask questions!

R2-D2 has been loaded up with the Death Star plans. These come from the data vault on Scarif, and consist of a .NET application with details about the Death Star. Maybe R2 can help to find a weakness?

Yoda can sense the living force, and can answer questions on the activities you have been doing by sensing your feelings.

## How this works

The `Copilot` class provides a base wrapper around a `PiecesCopilotChat`, seeding each conversation with a system prompt set in the derived `R2D2Copilot` and `YodaCopilot` classes. Each conversation also has options set when you run each chat. R2 has options to attach the [GalacticEmpire](./GalacticEmpire/) project, Yoda has options to use Pieces Live Context.
