using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class DeathRaceGameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] PlayerPrefabs;


    // Start is called before the first frame update
    void Start()
    {
        //checking the photon network is connnected and ready then go playerselesctionUI of Death Race 
        if (PhotonNetwork.IsConnectedAndReady)
        {

            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerRacingGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                //x and z random position
                int randomPosition = Random.Range(-15,15);

                //instantiating player character or car based on selection 
                PhotonNetwork.Instantiate(PlayerPrefabs[(int)playerSelectionNumber].name,new Vector3(randomPosition,0,randomPosition), Quaternion.identity);

            }

        }

       

        
    }

    //Leave button of Deathrace game UI to leave room
    public void OnQuitMatchButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    //take you to the lobby scene
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
