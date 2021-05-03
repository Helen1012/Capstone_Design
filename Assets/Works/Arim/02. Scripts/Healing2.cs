using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing2 : MonoBehaviour
{
    public GameObject Player;
    public GameObject HP;
    private const string bulletTag = "Bullet";
    // Start is called before the first frame update



    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == bulletTag)
        {
            Debug.Log("?");
            Player.SendMessage("Heal");
            //HP.SetActive(false);
            //Destroy(coll.gameObject);
        }

    }
}
