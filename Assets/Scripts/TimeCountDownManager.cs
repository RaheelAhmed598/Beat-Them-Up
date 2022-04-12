using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TimeCountDownManager : MonoBehaviourPunCallbacks
{
    //Timer text
    private Text TimeUIText;

    //Timer clock rate set
    private float timeToStartRace = 5.0f;

    private void Awake()
    {
        TimeUIText = RacingModeGameManager.instance.TimeUIText;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeToStartRace >= 0.0f)
            {
                timeToStartRace -= Time.deltaTime;
                // RpcTarget.AllBuffered = player join late, they see the other remaining time 
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartRace);


            }
            else if (timeToStartRace < 0.0f)
            {
                // if the player join late, they see the other remaining time
                photonView.RPC("StartTheRace", RpcTarget.AllBuffered);
            }
        }
    }

    //method-calls on remote clients in the same room.
    [PunRPC]
    public void SetTime(float time)
    {
        if (time > 0.0f)
        {
            TimeUIText.text = time.ToString("F1");
        }
        else
        {
            //The countdown is over
            TimeUIText.text = "";
        }

    }

    [PunRPC]
    public void StartTheRace()
    {
        GetComponent<CarMovement>().controlsEnabled = true;
        this.enabled = false;
    }


}
