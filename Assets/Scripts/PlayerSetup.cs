using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

//synchronizing movement of the player across the network
public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera PlayerCamera;
    public TextMeshProUGUI PlayerNameText;

    // Start is called before the first frame update
    void Start()
    {

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            //A PhotonView identifies an object across the network (viewID) and
            //configures how the controlling client updates remote instances.
            //True if the PhotonView is "mine" and can be controlled by this client.

            if (photonView.IsMine)
            {

                //enable carMovement script and camera
                GetComponent<CarMovement>().enabled = true;
                GetComponent<LapController>().enabled = true;
                PlayerCamera.enabled = true;

            }
            else
            {
                //Player is remote. Disable CarMovement script and camera.
                GetComponent<CarMovement>().enabled = false;
                GetComponent<LapController>().enabled = false;
                PlayerCamera.enabled = false;

            }

        }

        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            if (photonView.IsMine)
            {

                //enable carMovement script and camera
                GetComponent<CarMovement>().enabled = true;
                GetComponent<CarMovement>().controlsEnabled = true;
                PlayerCamera.enabled = true;

            }
            else
            {
                //Player is remote. Disable CarMovement script and camera.
                GetComponent<CarMovement>().enabled = false;

                PlayerCamera.enabled = false;

            }
        }




       SetPlayerUI();
    }

    //displaying player name above the car
    private void SetPlayerUI()
    {
        if (PlayerNameText != null)
        {
            PlayerNameText.text = photonView.Owner.NickName;

            if (photonView.IsMine)
            {
                PlayerNameText.gameObject.SetActive(false);
            }
        }



    }

}
