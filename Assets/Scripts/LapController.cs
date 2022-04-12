using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class LapController : MonoBehaviourPun
{
    private List<GameObject> LapTriggers = new List<GameObject>();

    public enum RaiseEventsCode
    {
        WhoFinishedEventCode = 0
    }

    private int finishOrder = 0;

    // Start is called before the first frame update
    void Start()
    {

        foreach (GameObject lapTrigger in RacingModeGameManager.instance.lapTriggers)
        {
            LapTriggers.Add(lapTrigger);
        }

    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    
    //whenever event race, this method called automatically 
    void OnEvent(EventData photonEvent)
    {
        //showing the finish order number with the player nickname
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoFinishedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string)data[0];

            finishOrder = (int)data[1];

            int viewID = (int)data[2];

            Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);

            GameObject orderUITextGameObject = RacingModeGameManager.instance.FinishOrderUIGameObjects[finishOrder - 1];
            orderUITextGameObject.SetActive(true);


            if (viewID == photonView.ViewID)
            {
                // the player is actually me!
                orderUITextGameObject.GetComponent<Text>().text = finishOrder + ". " + "(YOU)" + nickNameOfFinishedPlayer;
                orderUITextGameObject.GetComponent<Text>().color = Color.red;
            }
            else
            {
                orderUITextGameObject.GetComponent<Text>().text = finishOrder + ". " + nickNameOfFinishedPlayer;

            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (LapTriggers.Contains(other.gameObject))
        {
            int indexOfTrigger = LapTriggers.IndexOf(other.gameObject);

            LapTriggers[indexOfTrigger].SetActive(false);

            if (other.name == "FinishTrigger")
            {
                //game is finished.
                GameFinished();
            }


        }
    }



    void GameFinished()
    {
        //releasing the camera and stop the car movement script 
        GetComponent<PlayerSetup>().PlayerCamera.transform.parent = null;
        GetComponent<CarMovement>().enabled = false;

        //the player who finished the game increase 1, 
        //means those whocame in 1st,2nd,3rd
        finishOrder += 1;


        string nickName = photonView.Owner.NickName;
        int viewID = photonView.ViewID;

        //event data for all the player in current lobby
        object[] data = new object[] { nickName, finishOrder, viewID };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache

        };

        //send options
        SendOptions sendOptions = new SendOptions
        {
            Reliability = false

        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
