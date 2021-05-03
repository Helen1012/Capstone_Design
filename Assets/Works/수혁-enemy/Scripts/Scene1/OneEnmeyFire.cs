using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneEnmeyFire : MonoBehaviour
{   
    private AudioSource audio;
    public AudioClip fireSfx;

    private Animator animator;

    private Transform playerTr;

    private Transform enemyTr;

    private readonly int hashFire = Animator.StringToHash("isFire");


    private float nextFire = 0.1f;
    private readonly float fireRate = 0.3f;
    private readonly float damping = 10.0f;

    private readonly float reloadTime = 2.0f;
    private readonly int maxBullet = 10;
    private int currBullet = 10;

    private WaitForSeconds wsReload;


    public bool isFire = false;
    public GameObject Bullet;

    public GameObject bulletSpawn;     // 총구( 총알의 발사 위치)
    //public MeshRenderer muzzleFlash;



    // Start is called before the first frame update
    void Start()
    {   
        enemyTr = GetComponent<Transform>();
        //muzzleFlash.enabled = false;
        
        audio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        wsReload = new WaitForSeconds(reloadTime);
    }

    void Update(){
        if( isFire){
            if(Time.time >= nextFire){
                animator.SetTrigger(hashFire);
                Fire();
                //animator.SetBool(hashFire,false);
                nextFire = Time.time + fireRate + Random.Range(0.0f,1.0f);  // 공속딜레이
            }
        }
    }
    void Fire()
    {  
        audio.PlayOneShot(fireSfx, 1.0f);
        Instantiate(Bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
    }
}
