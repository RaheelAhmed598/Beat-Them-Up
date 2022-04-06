using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //Login UI Panel
    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public InputField PlayerNameInput;

    //Connecting loading panel 
    [Header("Connecting Info Panel")]
    public GameObject ConnectingInfoUIPanel;

    //create room panel 
    [Header("Creating Room Info Panel")]
    public GameObject CreatingRoomInfoUIPanel;

    //Game Option panel 
    [Header("GameOptions  Panel")]
    public GameObject GameOptionsUIPanel;

    //Create the Room panel 
    [Header("Create Room Panel")]
    public GameObject CreateRoomUIPanel;
    public InputField RoomNameInputField;
    public string GameMode;

    //Room panel 
    [Header("Inside Room Panel")]
    public GameObject InsideRoomUIPanel;
    //public Text RoomInfoText;
    //public GameObject PlayerListPrefab;
    //public GameObject PlayerListContent;
    //public GameObject StartGameButton;
    //public Text GameModeText;
    //public Image PanelBackground;
    //public Sprite RacingBackground;
    //public Sprite DeathRaceBackground;
    //public GameObject[] PlayerSelectionUIGameObjects;
    //public DeathRacePlayer[] DeathRacePlayers;
    //public RacingPlayer[] RacingPlayers;

    //Random join room panel
    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomUIPanel;


    //using region for code Readability 
    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(LoginUIPanel.name);
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion

    //On Login Button click check that PlayerName
    //Text Field is on the Text Feild or not, If yes
    //show the player name 
    #region UI Callback Methods
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(ConnectingInfoUIPanel.name);
           if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        else
        {
            Debug.Log("Player name is Invalid");
        }
    }
    #endregion

    //go back to the GameOption panel
    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    //creating the room
    public void OnCreateRoomButtonClicked()
    {
        ActivatePanel(CreatingRoomInfoUIPanel.name);

        if (GameMode != null)
        {
            string roomName = RoomNameInputField.text;
            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Room" + Random.Range(1000, 10000);

            }

            //two game modes
            //1. racing = "rc"
            //2. death race = "dr"

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 3;

            string[] roomPropsInLobby = { "gm" }; //gm = game mode

            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } };

            roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;

            PhotonNetwork.CreateRoom(roomName, roomOptions);

        }

    }


    
    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        ActivatePanel(GameOptionsUIPanel.name);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "is Connected to Internet");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name /*+ "Player count:" + PhotonNetwork.CurrentRoom.PlayerCount*/);

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            object gameModeName;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm", out gameModeName))
            {
                Debug.Log(gameModeName.ToString());
            }
        }
    }

    #endregion

    //set active condition of the different panel   
    #region Public Methods
    public void ActivatePanel(string panelNameToBeActivated)
    {

        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
        ConnectingInfoUIPanel.SetActive(ConnectingInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreatingRoomInfoUIPanel.SetActive(CreatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelNameToBeActivated));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        JoinRandomRoomUIPanel.SetActive(JoinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));
    }

    public void SetGameMode(string _gameMode)
    {
      GameMode = _gameMode;
    }

    #endregion
}
