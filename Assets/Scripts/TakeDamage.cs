using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TakeDamage : MonoBehaviourPun
{
    //player health and health bar 
    public float starthHealth = 100f;
    private float health;
    public Image healthBar;

    Rigidbody rb;

    public GameObject PlayerGraphics;
    public GameObject PlayerUI;
    public GameObject PlayerWeaponHolder;
    public GameObject DeathPanelUIPrefab;
    private GameObject DeathPanelUIGameObject;

    // Start is called before the first frame update
    void Start()
    {

        health = starthHealth;

        //changes te health after hitting player 
        healthBar.fillAmount = health / starthHealth;

        rb = GetComponent<Rigidbody>();

    }


    //player taken damage 
    [PunRPC]
    public void DoDamage(float _damage)
    {
        health -= _damage;
        Debug.Log(health);

        healthBar.fillAmount = health / starthHealth;

        if (health <= 0f)
        {
            //Die
            Die();
        }
    }


    //this method is used to stop functioning of player after death
    void Die()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;


        PlayerGraphics.SetActive(false);
        PlayerUI.SetActive(false);
        PlayerWeaponHolder.SetActive(false);


        if (photonView.IsMine)
        {
            //respawn
            StartCoroutine(ReSpawn());
        }
    }

    //count down for respawn
    IEnumerator ReSpawn()
    {
        GameObject canvasGameObject = GameObject.Find("Canvas");

        //show panel to player who died in game 
        if (DeathPanelUIGameObject==null)
        {
            DeathPanelUIGameObject = Instantiate(DeathPanelUIPrefab, canvasGameObject.transform);

        }
        else
        {
            DeathPanelUIGameObject.SetActive(true);
        }

        //death panel count down timer text 
        Text respawnTimeText = DeathPanelUIGameObject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;

        respawnTimeText.text = respawnTime.ToString(".00");

        while (respawnTime> 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(".00f");

            //during count down disable the following
            GetComponent<CarMovement>().enabled = false;
            GetComponent<Shooting>().enabled = false;
        }

        //after respawning death panel UI disappear
        DeathPanelUIGameObject.SetActive(false);

        //enable following scripts after respawn
        GetComponent<CarMovement>().enabled = true;
        GetComponent<Shooting>().enabled = true;

        //respawn at random point
        int randomPoint = Random.Range(-20,20);
        transform.position = new Vector3(randomPoint,0,randomPoint);

        photonView.RPC("Reborn",RpcTarget.AllBuffered);
    }

    //resapwn method
    [PunRPC]
    public void Reborn()
    {
        health = starthHealth;
        healthBar.fillAmount = health / starthHealth;

        PlayerGraphics.SetActive(true);
        PlayerUI.SetActive(true);
        PlayerWeaponHolder.SetActive(true);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
