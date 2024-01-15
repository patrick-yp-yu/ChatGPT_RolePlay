using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;


// OpenAI package
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

public class PlayerInteract : MonoBehaviour
{

    // public GameObject dialogueBox;
    [SerializeField] private GameObject toActivate;

    // OpenAI
    public TMP_Text textField;  // textField for chat message
    public TMP_InputField inputField;    // User input field
    public Button sendButton;

    private OpenAIAPI api;
    private bool[] talk2NPC = new bool[2];

    private List<ChatMessage> messageList; // Chat to chatGPT
    private List<ChatMessage> messageList0; // Record all chat contents for NPC0
    private List<ChatMessage> messageList1; // Record all chat contents for NPC1

    // player
    private GameObject player = null;
    private Transform avatar;

    // Start is called before the first frame update
    void Start()
    {
        // Get player's transform and color
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            avatar = player.transform;
        }

        // Setup ChatGPT API_KEY
        api = new OpenAIAPI("");        
        InitializeRole(); 
    }

    private void InitializeRole()
    {
        messageList0 = new List<ChatMessage> {
            new ChatMessage(ChatMessageRole.System, 
            @"Start a role play. Now, your name is 'Paul'. You are a police and are willing to assist people. 
            You speak formally and politely. You keep your responses short and to the point.
            Your friend, John, is standing next to your left. John is a cowboy.
            ")
        };            

        messageList1 = new List<ChatMessage> {
            new ChatMessage(ChatMessageRole.System, 
            @"Start a role play. Your name is 'John'. You are a friendly cowboy and you speak in a traditional western cowboy way. 
            You are willing to assist people. You only know things that is related to cowboy. You keep your responses short and to the point.
            Your friend, Paul, is standing next to your right. Paul is a police.
            ")
        };

    } 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Check whether encounter NPCs
            float interactRange = 3f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray) 
            {
                // Debug.Log(collider);
                if (collider.CompareTag("NPC0") || collider.CompareTag("NPC1"))
                {
                    Debug.Log(collider);
                    if (collider.CompareTag("NPC0"))
                    {
                        textField.text = "Talk to NPC0:";
                        RoleSetup_NPC0();
                    }
                    else
                    {
                        textField.text = "Talk to NPC1:";
                        RoleSetup_NPC1();
                    }

                    StartChat(); 
                }
            }
        }

    }

    private void StartChat()
    {
        
        // activate DialogueBox
        toActivate.SetActive(true);
        inputField.ActivateInputField();

        // disable player movement when try to type conversations
        avatar.GetComponent<PlayerInput>().enabled = false;         

        // dısplay cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        sendButton.onClick.AddListener(() => GetResponse());                      
    }    

    public void CloseChat()
    {
        toActivate.SetActive(false);
        for (int i = 0; i < talk2NPC.Length; i++) 
        { 
            talk2NPC[i] = false; 
        }
        Debug.Log(talk2NPC);

        // recover player movement
        avatar.GetComponent<PlayerInput>().enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void RoleSetup_NPC0()
    {
        talk2NPC[0] = true;

        inputField.text = "";   // clear inputField
    }

    private void RoleSetup_NPC1()
    {
        talk2NPC[1] = true;

        inputField.text = "";   // clear inputField
    }       

    private async void GetResponse()
    {
        
        if (talk2NPC[0] == true)
        {
            messageList = messageList0;
            Debug.Log("talk2NPC[0] == " + talk2NPC[0]);
        }
        else if (talk2NPC[1] == true)
        {
            messageList = messageList1;
            Debug.Log("talk2NPC[1] == " + talk2NPC[1]);
        }
        else
        {
            Debug.Log("Wrong!!!");
        }

        // Avoid empty (& meaningless) input 
        if (inputField.text.Length < 2)
        {
            return;
        }

        // Disable the button
        sendButton.enabled = false;

        // Fill the user message from the input field
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;    // assign role
        userMessage.Content = inputField.text;      // assign content

        // Avoid long messages  
        if (userMessage.Content.Length > 200)
        {
            userMessage.Content = userMessage.Content.Substring(0, 200);
        }

        Debug.Log(string.Format("{0}: {1}", messageList[0].rawRole, messageList[0].Content));
        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content));
 
        // Add the message to the list
        messageList.Add(userMessage);

        // Update the text field 
        textField.text = string.Format("You: {0}", userMessage.Content);

        // Clear the input field
        inputField.text = "";

        // Send chat to OpenAI
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest() {
            Model = Model.ChatGPTTurbo, 
            Temperature = 0.7,  // 0 = deterministic, 1 = change wildly
            MaxTokens = 100,
            Messages = messageList 
        });

        // Get the response message
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

        // Add the response to the list
        messageList.Add(responseMessage);

        // Udate the text field
        textField.text = string.Format("You:\n\t{0}\n\n\nNPC_1:\n\t{1}", userMessage.Content, responseMessage.Content);

        // Enable the ok button
        sendButton.enabled = true; 
    }

    
}
