#pragma warning disable CS0108
#pragma warning disable CS0414

using System.Collections;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{   
    private AudioSource audio;
    private Animator animator;

    private Transform playerTr;

    private Transform enemyTr;

    private readonly int hashFire = Animator.StringToHash("Fire");
    private readonly int hashReload = Animator.StringToHash("Reload");

    private float nextFire = 0.1f;
    private readonly float fireRate = 0.3f;
    private readonly float damping = 10.0f;

    private readonly float reloadTime = 2.0f;
    private readonly int maxBullet = 10;
    private int currBullet = 10;
    private bool isReload = false;
    private WaitForSeconds wsReload;


    public bool isFire = false;
    public AudioClip fireSfx;
    public AudioClip reloadSfx;

    public GameObject Bullet;
    public Transform firePos;


    public MeshRenderer muzzleFlash;

    private SC2PlayerDamage playerDamage;



    // Start is called before the first frame update
    void Start()
    {   
        playerDamage = GameObject.Find("Handgun_01_FPSController").GetComponent<SC2PlayerDamage>();
        muzzleFlash.enabled = false;
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();

        wsReload = new WaitForSeconds(reloadTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isReload && isFire){
            if(Time.time >= nextFire){
                Fire();
                playerDamage.AttackedByBullet();
                nextFire = Time.time + fireRate + 2f;  // 공속딜레이
            }
            
        Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
        enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }
    
    private void Fire(){
        animator.SetTrigger(hashFire);
        audio.PlayOneShot(fireSfx, 1.0f);
        StartCoroutine(ShowMuzzleFlash());

        isReload = (--currBullet % maxBullet ==0);

        if(isReload){
            StartCoroutine(Reloading());
        }

        GameObject _bullet = Instantiate(Bullet, firePos.position, firePos.rotation);
        Destroy(_bullet, 3.0f);
    }

    IEnumerator Reloading(){
        muzzleFlash.enabled = false;
        animator.SetTrigger(hashReload);
        audio.PlayOneShot(reloadSfx, 1.0f);

        yield return wsReload;
        currBullet = maxBullet;
        isReload = false;
    }

    IEnumerator ShowMuzzleFlash(){

        muzzleFlash.enabled = true;

        Quaternion rot = Quaternion.Euler(Vector3.forward * Random.Range(0,360));
        muzzleFlash.transform.localRotation = rot;
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1.0f,2.0f);

        Vector2 offset = new Vector2(Random.Range(0,2), Random.Range(0,2) * 0.5f);
        muzzleFlash.material.SetTextureOffset("_MainTex", offset);

        yield return new WaitForSeconds(Random.Range(0.05f,0.2f));
        muzzleFlash.enabled = false;
    }
}