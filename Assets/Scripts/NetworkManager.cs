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
    public Text RoomInfoText;
    public GameObject PlayerListPrefab;
    public GameObject PlayerListContent;
    public GameObject StartGameButton;
    public Text GameModeText;
    public Image PanelBackground;
    public Sprite RacingBackground;
    public Sprite DeathRaceBackground;
    //public GameObject[] PlayerSelectionUIGameObjects;
    //public DeathRacePlayer[] DeathRacePlayers;
    //public RacingPlayer[] RacingPlayers;

    //Random join room panel
    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomUIPanel;

    private Dictionary<int, GameObject> playerListGameObjects;
    //using region for code Readability 
    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(LoginUIPanel.name);
        PhotonNetwork.AutomaticallySyncScene = true;
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

    //joining the random rooms with expected max Players in game mode 
    public void OnJoinRandomRoomButtonClicked(string _gameMode)
    {
        GameMode = _gameMode;

        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", _gameMode } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    //back to gameobject panel
    public void OnBackButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }


    public void OnLeaveGameButtonClicked() 
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                //Racing game mode
                PhotonNetwork.LoadLevel("RacingScene");


            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
            {
                //Death race mode
                PhotonNetwork.LoadLevel("DeathRaceScene");
            }
        }
    }
    #endregion




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
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + "Player count:" + PhotonNetwork.CurrentRoom.PlayerCount);

        ActivatePanel(InsideRoomUIPanel.name);


        // after connecting the game mode the player get the choice of the Racing and death race game 
        // after which shows the player room name with total no of player in that room are show
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + " Players/Max.Players: " +
            PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;



            //Racing game Mode
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                //Racing game mode
                GameModeText.text = "LET'S RACE!";
                PanelBackground.sprite = RacingBackground;


                //for (int i = 0; i < PlayerSelectionUIGameObjects.Length; i++)
                //{
                //    PlayerSelectionUIGameObjects[i].transform.Find("PlayerName").GetComponent<Text>().text = RacingPlayers[i].playerName;
                //    PlayerSelectionUIGameObjects[i].GetComponent<Image>().sprite = RacingPlayers[i].playerSprite;
                //    PlayerSelectionUIGameObjects[i].transform.Find("PlayerProperty").GetComponent<Text>().text = "";

                //}


            }


            //Death Race game mode 
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
            {
                //Death race game mode
                GameModeText.text = "DEATH RACE!";
                PanelBackground.sprite = DeathRaceBackground;

                //for (int i = 0; i < PlayerSelectionUIGameObjects.Length; i++)
                //{
                //    PlayerSelectionUIGameObjects[i].transform.Find("PlayerName").GetComponent<Text>().text = DeathRacePlayers[i].playerName;
                //    PlayerSelectionUIGameObjects[i].GetComponent<Image>().sprite = DeathRacePlayers[i].playerSprite;
                //    PlayerSelectionUIGameObjects[i].transform.Find("PlayerProperty").GetComponent<Text>().text = DeathRacePlayers[i].weaponName +
                //        ": " + "Damage: " + DeathRacePlayers[i].damage + " FireRate: " + DeathRacePlayers[i].fireRate;

                //}
            }





            if (playerListGameObjects == null)
            {
                playerListGameObjects = new Dictionary<int, GameObject>();

            }



            //player ready prefab works that check the player list and are they ready or not
            //player list generate/update whenever the player join the room
            //if player player is ready the id and name shown in panel
            //and checking the ready condition of the player 
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                GameObject playerListGameObject = Instantiate(PlayerListPrefab);
                playerListGameObject.transform.SetParent(PlayerListContent.transform);
                playerListGameObject.transform.localScale = Vector3.one;
                playerListGameObject.GetComponent<PlayerListEntryInitializer>().Initialize(player.ActorNumber, player.NickName);//actornumber is player id 


                object isPlayerReady;
                if (player.CustomProperties.TryGetValue(MultiplayerRacingGame.PLAYER_READY, out isPlayerReady))
                {

                    playerListGameObject.GetComponent<PlayerListEntryInitializer>().SetPlayerReady((bool)isPlayerReady);


                }

                
                playerListGameObjects.Add(player.ActorNumber, playerListGameObject);


            }
        }

        StartGameButton.SetActive(false);
    }


    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject playerListGameObject;
        //playerlistGameObject and actor reference through playerlistgameobjectdictionary
        if (playerListGameObjects.TryGetValue(target.ActorNumber, out playerListGameObject))
        {
            //if the existing player dictionary update it condition local player for see the update
            object isPlayerReady;
            if (changedProps.TryGetValue(MultiplayerRacingGame.PLAYER_READY, out isPlayerReady))
            {

            playerListGameObject.GetComponent<PlayerListEntryInitializer>().SetPlayerReady((bool)isPlayerReady);


            }
        }

        StartGameButton.SetActive(CheckPlayersReady());
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " +" Players/Max.Players: " +
        PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        GameObject playerListGameObject = Instantiate(PlayerListPrefab);
        playerListGameObject.transform.SetParent(PlayerListContent.transform);
        playerListGameObject.transform.localScale = Vector3.one;
        playerListGameObject.GetComponent<PlayerListEntryInitializer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListGameObjects.Add(newPlayer.ActorNumber, playerListGameObject);

        StartGameButton.SetActive(CheckPlayersReady());

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + " Players/Max.Players: " +
        PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        //remove the name if player leave the room
        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);

        StartGameButton.SetActive(CheckPlayersReady());

    }

    public override void OnLeftRoom()
    {
        //if player left navigate him to the game option panel
        ActivatePanel(GameOptionsUIPanel.name);

        //destroy player list object after left the room
        foreach (GameObject playerListGameobject in playerListGameObjects.Values)
        {
            Destroy(playerListGameobject);

        }

        // clear the player name on list
        playerListGameObjects.Clear();
        playerListGameObjects = null;




    }



    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.SetActive(CheckPlayersReady());
        }
    }



    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);

        //if no room, create one 
        if (GameMode != null)
        {
            string roomName = RoomNameInputField.text;
            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Room" + Random.Range(1000, 10000);

            }
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 3;
            string[] roomPropsInLobby = { "gm" }; //gm = game mode

            //two game modes
            //1. racing = "rc"
            //2. death race = "dr"

            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } };

            roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
        
    }

    #endregion

    //set active condition of the different panel/ UI Navigation   
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




    //check if all player is ready or not
    #region Private Methods
    private bool CheckPlayersReady()
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {

            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(MultiplayerRacingGame.PLAYER_READY, out isPlayerReady))
            {

                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    #endregion



}
