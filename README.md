
# Role Play with ChatGPT 

Within this Unity-based role-playing game, a player engages with various non-playable characters (NPCs). The player poses questions and receives responses that are dynamically generated by ChatGPT rather than pre-scripted. The game connects to OpenAI through a REST API architecture, and retrieves generated texts in real time. The demo showcases ChatGPT's capacity to simulate multiple NPCs concurrently in a game.

## Demo Link 
[Demo Video](https://youtu.be/M1LPVVolahU)

## How to Run
The repo contains source files for Unity. 
- If you would like to run the project on your machine, please create a new Unity project. 
- Then, copy the folder in this repo to overwrite the initial folder with the same name. 
- Open the project with Unity. In your project folder inspector, find the file `Assets/PlayerInteract.cs`
- You will need to have a OpenAI API_KEY and setup the key as the following code. 

```
    void Start()
    {
        // Get player's transform and color
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            avatar = player.transform;
        }

        // *****************************
        // Setup your ChatGPT API_KEY here
        // *****************************
        api = new OpenAIAPI("your_API_Key");


        InitializeRole(); 
    }
```


### OpenAI C# Wrapper 
- Note, the codes requires a OpenAI C# library that can be installed through [https://github.com/OkGoDoIt/OpenAI-API-dotnet](https://github.com/OkGoDoIt/OpenAI-API-dotnet)
- Go the Releases page.
- The codes uses the release: v1.6.

