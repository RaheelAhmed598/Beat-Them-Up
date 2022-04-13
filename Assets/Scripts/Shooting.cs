using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPun
{
    //bullet and shooting fire position and player camera
    public GameObject BulletPrefab;
    public Transform firePosition;
    public Camera PlayerCamera;

    //scriptable object of the Death race player's data 
    public DeathRacePlayer DeathRacePlayerProperties;

    //to prevent the shooting continuously we used fire rate 
    private float fireRate;
    private float fireTimer = 0.0f;

    //for sprts car player we use the laser as shooting 
    private bool useLaser;
    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {

        //takingthe fire rate of the player through death race scriptable object
        fireRate = DeathRacePlayerProperties.fireRate;

        if (DeathRacePlayerProperties.weaponName== "Laser Gun" )
        {
            useLaser = true;

        }else
        {
            useLaser = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {


        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetKey("space"))
        {
            if (fireTimer>fireRate)
            {
                //fire
                photonView.RPC("Fire",RpcTarget.All, firePosition.position);

                fireTimer = 0.0f;
            }       
        }


        if (fireTimer<fireRate)
        {
            fireTimer += Time.deltaTime;
        }


    }


    [PunRPC]
    public void Fire(Vector3 _firePosition)
    {

        if (useLaser)
        {
            //laser raycast through line renderer
            RaycastHit _hit;
            Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f));

            if (Physics.Raycast(ray,out _hit, 200))
            {

                if (!lineRenderer.enabled)
                {
                    lineRenderer.enabled = true;
                }

                lineRenderer.startWidth = 0.3f;
                lineRenderer.endWidth = 0.1f;



                lineRenderer.SetPosition(0,_firePosition);
                lineRenderer.SetPosition(1,_hit.point);

                if (_hit.collider.gameObject.CompareTag("Player"))
                {
                    if (_hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                    {
                        _hit.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, DeathRacePlayerProperties.damage);
                    }
                
                }



                StopAllCoroutines();
                StartCoroutine(DisableLaserAfterSecs(0.3f));
            }

        }
        else
        {
            //raycasting for shooting through player camera with center of screen 
            Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            //bullet instantiate
            GameObject bullletGameObject = Instantiate(BulletPrefab, _firePosition, Quaternion.identity);

            //bullet fire power of each player through death race scriptable object
            bullletGameObject.GetComponent<BulletScript>().Initialize(ray.direction, DeathRacePlayerProperties.bulletSpeed, DeathRacePlayerProperties.damage);
        }      
    }


    //disable laser after fire 
    IEnumerator DisableLaserAfterSecs(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        lineRenderer.enabled = false;

    }
}
