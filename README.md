EnglishWidgetApp
================

Overview
--------

EnglishWidgetApp is a WPF desktop application built with .NET 8 that displays English conversational sentences along with their Vietnamese translations. The sentences are updated every 5 minutes, and users can listen to the English text using text-to-speech functionality.

Features
--------

-   Fetches 5 common English conversational sentences daily from Azure OpenAI.

-   Displays a new sentence every 5 minutes.

-   Supports text-to-speech (TTS) for English sentences using Azure Speech Services.

-   Allows users to manually trigger speech via a "Speak" button.

-   Runs in the system tray with options to hide/show and exit.

-   Supports window dragging and remains always on top.

Requirements
------------

-   .NET 8 SDK

-   Azure OpenAI API key

-   Azure Speech Services API key

Installation
------------

1.  Clone the repository:

    ```
    git clone https://github.com/your-repo/EnglishWidgetApp.git
    ```

2.  Navigate to the project folder:

    ```
    cd EnglishWidgetApp
    ```

3.  Install dependencies:

    ```
    dotnet restore
    ```

Configuration
-------------

Set up your Azure API keys in `appsettings.json`:

```
{
  "AzureOpenAIKey": "YOUR_AZURE_OPENAI_KEY",
  "AzureEndpoint": "YOUR_AZURE_OPENAI_ENDPOINT",
  "AzureSpeechKey": "YOUR_AZURE_SPEECH_KEY",
  "AzureSpeechRegion": "YOUR_AZURE_SPEECH_REGION"
}
```

Usage
-----

1.  Run the application:

    ```
    dotnet run
    ```

2.  The widget will appear on the desktop, displaying sentences.

3.  Click the **"🔊 Speak"** button to listen to the English sentence.

4.  Access the system tray menu to hide or exit the application.

Development
-----------

-   Modify `MainWindow.xaml` for UI changes.

-   Modify `MainWindow.xaml.cs` for event handling and logic.

-   Use `Azure.AI.OpenAI` for fetching sentences and `Microsoft.CognitiveServices.Speech` for TTS.

License
-------

This project is licensed under the MIT License.
