using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{
    public GameObject Player;
    public GameObject HP;
    private const string bulletTag = "Bullet";
    // Start is called before the first frame update
    

    private void OnTriggerEnter(Collider coll)
    {
        Debug.Log("?");
        Player.SendMessage("Heal");
        HP.SetActive(false);
    }
}
