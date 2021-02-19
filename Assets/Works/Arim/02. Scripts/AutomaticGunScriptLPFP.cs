﻿//필요없는 부분은 주석처리함
//원래 에셋에서는 우클릭이 눌러진 상태가 줌in인 상태인데 이것을 PlayerGun의 default상태로 하려함.
//	-> 총의 에임이 카메라가 가르키는 곳과 일치하기 위해
//        필요한것 : Gizmos(카메라가 가르키는 곳 표시)
//스크립트가 너무 길어서 좀 나누고 싶었으나 객체들이 다 엮여있어서 함부로하니깐 잘 안됨..
//Bullet만 이 스크립트에서 분리하고 싶은데 여러 가지 시도를 해봐야할거같음.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Leap;

public class AutomaticGunScriptLPFP : MonoBehaviour
{

    //Animator component attached to weapon
    Animator anim;

    [Header("Gun Camera")]  //카메라 이동
                            //Main gun camera
    public Camera gunCamera;

    [Header("Gun Camera Options")]
    //How fast the camera field of view changes when aiming 
    [Tooltip("How fast the camera field of view changes when aiming.")]
    public float fovSpeed = 15.0f;
    //Default camera field of view
    [Tooltip("Default value for camera field of view (40 is recommended).")]
    public float defaultFov = 40.0f;

    public float aimFov = 25.0f;

    [Header("UI Weapon Name")]
    [Tooltip("Name of the current weapon, shown in the game UI.")]
    public string weaponName;
    private string storedWeaponName;

    [Header("Weapon Sway")] //카메라 이동에 따라 무기도 이동
                            //Enables weapon sway
    [Tooltip("Toggle weapon sway.")]
    public bool weaponSway;

    public float swayAmount = 0.02f;
    public float maxSwayAmount = 0.06f;
    public float swaySmoothValue = 4.0f;

    private Vector3 initialSwayPosition;

    //Used for fire rate
    private float lastFired;
    [Header("Weapon Settings")]
    //How fast the weapon fires, higher value means faster rate of fire
    [Tooltip("How fast the weapon fires, higher value means faster rate of fire.")]
    public float fireRate;
    //Eanbles auto reloading when out of ammo
    [Tooltip("Enables auto reloading when out of ammo.")]
    public bool autoReload;
    //Delay between shooting last bullet and reloading
    public float autoReloadDelay;
    //Check if reloading
    private bool isReloading;

    //Holstering weapon
    //private bool hasBeenHolstered = false;
    //If weapon is holstered
    //private bool holstered;
    //Check if running
    //private bool isRunning;
    //Check if aiming
    //private bool isAiming;
    //Check if walking
    //private bool isWalking;
    //Check if inspecting weapon
    //private bool isInspecting;

    //How much ammo is currently left
    private int currentAmmo;
    //Totalt amount of ammo
    [Tooltip("How much ammo the weapon should have.")]
    public int ammo;
    //Check if out of ammo
    private bool outOfAmmo;

    /* ---------------------------------------------- */
    // Used LeapMotion
    Controller controller;
    List<Finger> fingers;
    public GameObject cube;

    Hand hand;
    Hand previous_hand;

    Vector handPalmPosition;
    Vector prehandPalmPosition;
    /* ---------------------------------------------- */

    [Header("Bullet Settings")]
    //Bullet
    [Tooltip("How much force is applied to the bullet when shooting.")]
    public float bulletForce = 400.0f;
    [Tooltip("How long after reloading that the bullet model becomes visible " +
        "again, only used for out of ammo reload animations.")]
    public float showBulletInMagDelay = 0.6f;
    [Tooltip("The bullet model inside the mag, not used for all weapons.")]
    public SkinnedMeshRenderer bulletInMagRenderer;

    [Header("Grenade Settings")]
    public float grenadeSpawnDelay = 0.35f;

    [Header("Muzzleflash Settings")]
    public bool randomMuzzleflash = false;
    //min should always bee 1
    private int minRandomValue = 1;

    [Range(2, 25)]
    public int maxRandomValue = 5;

    private int randomMuzzleflashValue;

    public bool enableMuzzleflash = true;
    public ParticleSystem muzzleParticles;
    public bool enableSparks = true;
    public ParticleSystem sparkParticles;
    public int minSparkEmission = 1;
    public int maxSparkEmission = 7;

    [Header("Muzzleflash Light Settings")]
    public Light muzzleflashLight;
    public float lightDuration = 0.02f;

    [Header("Audio Source")]
    //Main audio source
    public AudioSource mainAudioSource;
    //Audio source used for shoot sound
    public AudioSource shootAudioSource;

    [Header("UI Components")]
    public Text timescaleText;
    public Text currentWeaponText;
    public Text currentAmmoText;
    public Text totalAmmoText;

    [System.Serializable]
    public class prefabs
    {
        [Header("Prefabs")]
        public Transform bulletPrefab;
        public Transform casingPrefab;
        public Transform grenadePrefab;
    }
    public prefabs Prefabs;

    [System.Serializable]
    public class spawnpoints
    {
        [Header("Spawnpoints")]
        //Array holding casing spawn points 
        //(some weapons use more than one casing spawn)
        //Casing spawn point array
        public Transform casingSpawnPoint;
        //Bullet prefab spawn from this point
        public Transform bulletSpawnPoint;

        public Transform grenadeSpawnPoint;
    }
    public spawnpoints Spawnpoints;

    [System.Serializable]
    public class soundClips
    {
        public AudioClip shootSound;
        public AudioClip takeOutSound;
        public AudioClip holsterSound;
        public AudioClip reloadSoundOutOfAmmo;
        public AudioClip reloadSoundAmmoLeft;
        public AudioClip aimSound;
    }
    public soundClips SoundClips;

    private void Awake()
    {

        //Set the animator component
        anim = GetComponent<Animator>();
        //Set current ammo to total ammo value
        currentAmmo = ammo;

        muzzleflashLight.enabled = false;
    }

    private void Start()
    {

        //Save the weapon name
        storedWeaponName = weaponName;
        //Get weapon name from string to text
        currentWeaponText.text = weaponName;
        //Set total ammo text from total ammo int
        totalAmmoText.text = ammo.ToString();

        //Weapon sway
        initialSwayPosition = transform.localPosition;

        //Set the shoot sound to audio source
        shootAudioSource.clip = SoundClips.shootSound;

        /* ---------------------------------------------- */
        controller = new Controller();
        cube = GameObject.FindGameObjectWithTag("Cube"); // 임시 물체
        /* ---------------------------------------------- */
    }

    private void LateUpdate() // LateUpdate()가 여기 있어야 맞나?
    {
        //Weapon sway
        if (weaponSway == true)
        {
            float movementX = -Input.GetAxis("Mouse X") * swayAmount;
            float movementY = -Input.GetAxis("Mouse Y") * swayAmount;
            //Clamp movement to min and max values
            movementX = Mathf.Clamp
                (movementX, -maxSwayAmount, maxSwayAmount);
            movementY = Mathf.Clamp
                (movementY, -maxSwayAmount, maxSwayAmount);
            //Lerp local pos
            Vector3 finalSwayPosition = new Vector3
                (movementX, movementY, 0);
            transform.localPosition = Vector3.Lerp
                (transform.localPosition, finalSwayPosition +
                    initialSwayPosition, Time.deltaTime * swaySmoothValue);
        }
    }

    private void Update()
    {

        if (!controller.IsConnected) Debug.Log("not connected"); // 스크립트 멈추는 거 추가하기 (return;)

        Frame frame = controller.Frame();           // The latest frame
        Frame previous = controller.Frame(1);       // The previous frame

        // anim.SetBool("Aim", true);
        gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView, aimFov, fovSpeed * Time.deltaTime); //줌 더 땡겨 주는건데 할지 말지 고민해봐야할듯

        // 항상 Aiming한 상태이니까 if에 해당되는 것을 사용해야 할 것 같음

        /*
		//Aiming
		//Toggle camera FOV when right click is held down
		if(Input.GetButton("Fire2") && !isReloading && !isRunning && !isInspecting) 
		{
			
			isAiming = true;
			//Start aiming
			anim.SetBool ("Aim", true);

			//When right click is released
			gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView,
				aimFov,fovSpeed * Time.deltaTime);

			if (!soundHasPlayed) 
			{
				mainAudioSource.clip = SoundClips.aimSound;
				mainAudioSource.Play ();
	
				soundHasPlayed = true;
			}
		} 
		else 
		{
			//When right click is released
			gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView,
				defaultFov,fovSpeed * Time.deltaTime);

			isAiming = false;
			//Stop aiming
			anim.SetBool ("Aim", false);
				
			soundHasPlayed = false;
		}
		//Aiming end
		*/

        //If randomize muzzleflash is true, genereate random int values
        if (randomMuzzleflash == true)
        {
            randomMuzzleflashValue = Random.Range(minRandomValue, maxRandomValue);
        }

        //Set current ammo text from ammo int
        currentAmmoText.text = currentAmmo.ToString();

        //Continosuly check which animation 
        //is currently playing
        AnimationCheck();

        // 컨트롤러에 손이 인지될 때
        for (int h = 0; h < frame.Hands.Count; h++)
        {
            hand = frame.Hands[0]; // 현재 나타나는 손
            previous_hand = previous.Hands[0]; // 이전 프레임에 나타나는 손

            handPalmPosition = hand.PalmPosition;   // 현재 손의 위치
            prehandPalmPosition = previous_hand.PalmPosition;   // 이전 프레임의 손의 위치

            fingers = hand.Fingers; // 현재 손가락의 개수

            int _extendedFingers = getExtendedFingers();    // 함수를 호출하여 펼쳐진 손가락의 개수를 확인한다


            /* ---------------------- (1) 수류탄; 립모션 사용 ---------------------- */
            
            //Throw grenade when pressing G key

            // [Condition for changing weapons] 무기전환(수류탄)
            //  1. Hands moving from side to side (swipe)

            // if (Input.GetKeyDown (KeyCode.G) && !isInspecting)
            if (hand.IsRight && System.Math.Abs(handPalmPosition.x - prehandPalmPosition.x) > 5)
            {
                StartCoroutine(GrenadeSpawnDelay());
                //Play grenade throw animation
                anim.Play("GrenadeThrow", 0, 0.0f);

                cube.GetComponent<MeshRenderer>().material.color = Color.green;
            }

            

            /* ---------------------- (2) 총쏘기; 립모션 사용 ---------------------- */
            //----------------------Input.GetMouseButton(0) = 좌클릭 -> bool함수로 립모션이랑 연계필요 -------------------------------------

            // if (Input.GetMouseButton (0) && !outOfAmmo && !isReloading && !isInspecting && !isRunning)
            // [Conditions to 'Shoot'] 총쏘기
            //  1. Two straight fingers
            //  2. Hands moving from top to bottom

            else if (hand.IsRight && _extendedFingers == 2 && System.Math.Abs(handPalmPosition.y - prehandPalmPosition.y) > 5 && System.Math.Abs(hand.PalmVelocity.y) > 30 && !outOfAmmo && !isReloading)
            {
                //Shoot automatic
                if (Time.time - lastFired < 1 / fireRate) continue;

                lastFired = Time.time;

                //Remove 1 bullet from ammo
                currentAmmo -= 1;

                shootAudioSource.clip = SoundClips.shootSound;
                shootAudioSource.Play();

                //if (!isAiming) //if not aiming
                //{
                //	anim.Play ("Fire", 0, 0f);
                //	//If random muzzle is false
                //	if (!randomMuzzleflash && 
                //		enableMuzzleflash == true) 
                //	{
                //		muzzleParticles.Emit (1);
                //		//Light flash start
                //		StartCoroutine(MuzzleFlashLight());
                //	} 
                //	else if (randomMuzzleflash == true)
                //	{
                //		//Only emit if random value is 1
                //		if (randomMuzzleflashValue == 1) 
                //		{
                //			if (enableSparks == true) 
                //			{
                //				//Emit random amount of spark particles
                //				sparkParticles.Emit (Random.Range (minSparkEmission, maxSparkEmission));
                //			}
                //			if (enableMuzzleflash == true) 
                //			{
                //				muzzleParticles.Emit (1);
                //				//Light flash start
                //				StartCoroutine (MuzzleFlashLight ());
                //			}
                //		}
                //	}
                //} 
                //else //if aiming
                //{

                // anim.Play("Aim Fire", 0, 0f);

                //If random muzzle is false
                if (!randomMuzzleflash)
                {
                    muzzleParticles.Emit(1);
                    //If random muzzle is true
                }
                else if (randomMuzzleflash == true)
                {
                    //Only emit if random value is 1
                    if (randomMuzzleflashValue == 1)
                    {
                        if (enableSparks == true)
                        {
                            //Emit random amount of spark particles
                            sparkParticles.Emit(Random.Range(minSparkEmission, maxSparkEmission));
                            // Emit의 매개 변수:
                            //   count:
                            //     Number of particles to emit.
                        }
                        if (enableMuzzleflash == true)
                        {
                            muzzleParticles.Emit(1);
                            //Light flash start
                            StartCoroutine(MuzzleFlashLight());
                        }
                    }
                }
                //}

                //Spawn bullet from bullet spawnpoint
                var bullet = (Transform)Instantiate(
                    Prefabs.bulletPrefab,
                    Spawnpoints.bulletSpawnPoint.transform.position,
                    Spawnpoints.bulletSpawnPoint.transform.rotation);

                //Add velocity to the bullet
                bullet.GetComponent<Rigidbody>().velocity =
                    bullet.transform.forward * bulletForce;

                //Spawn casing prefab at spawnpoint
                Instantiate(Prefabs.casingPrefab,
                    Spawnpoints.casingSpawnPoint.transform.position,
                    Spawnpoints.casingSpawnPoint.transform.rotation);

                cube.GetComponent<MeshRenderer>().material.color = Color.red;

            }

            /* ---------------------- (3) 재장전; 립모션 사용 ---------------------- */

            // [Condition to Load] 장전
            //  1. Gripped left hand

            //if (Input.GetKeyDown (KeyCode.R) && !isReloading && !isInspecting) 
            else if (hand.IsLeft && hand.GrabStrength == 1 && !isReloading)
            {
                //Reload
                Reload();
                cube.GetComponent<MeshRenderer>().material.color = Color.yellow;
            }
            else
            {
                cube.GetComponent<MeshRenderer>().material.color = Color.black;
            }

            //If out of ammo 남은 총알수가 0개면 자동 재장전. 
            if (currentAmmo == 0)
            {
                //Show out of ammo text
                currentWeaponText.text = "OUT OF AMMO";
                //Toggle bool
                outOfAmmo = true;
                //Auto reload if true
                if (autoReload == true && !isReloading)
                {
                    StartCoroutine(AutoReload());
                }
            }
            else
            {
                //When ammo is full, show weapon name again
                currentWeaponText.text = storedWeaponName.ToString();
                //Toggle bool
                outOfAmmo = false;
                //anim.SetBool ("Out Of Ammo", false);
            }


            //------------------플레이어의 이동을 어떻게 할지 고민해 봐야할듯
            //Walking when pressing down WASD keys
            //if (Input.GetKey (KeyCode.W) && !isRunning || 
            //	Input.GetKey (KeyCode.A) && !isRunning || 
            //	Input.GetKey (KeyCode.S) && !isRunning || 
            //	Input.GetKey (KeyCode.D) && !isRunning) 
            //{
            //if (Input.GetKey(KeyCode.W) ||
            //	Input.GetKey(KeyCode.A) ||
            //	Input.GetKey(KeyCode.S) ||
            //	Input.GetKey(KeyCode.D))
            //{
            //	anim.SetBool ("Walk", true);
            //} else {
            //	anim.SetBool ("Walk", false);
            //}
        } // end for
    } // end Update()

    // 펼쳐진 손가락의 개수를 확인하는 함수
    private int getExtendedFingers()
    {
        int extendedFingers = 0;

        for (int f = 0; f < fingers.Count; f++)
        {
            Finger digit = fingers[f];
            if (digit.IsExtended)
                extendedFingers++;
        }

        Debug.Log(extendedFingers);
        return extendedFingers;
    }

    //수류탄 생성 관련 함수
    private IEnumerator GrenadeSpawnDelay()
    {

        //Wait for set amount of time before spawning grenade
        yield return new WaitForSeconds(grenadeSpawnDelay);
        //Spawn grenade prefab at spawnpoint
        Instantiate(Prefabs.grenadePrefab,
            Spawnpoints.grenadeSpawnPoint.transform.position,
            Spawnpoints.grenadeSpawnPoint.transform.rotation);
    }

    //자동 재장전 -------------------------------------------------------------------------
    private IEnumerator AutoReload()
    {
        //Wait set amount of time
        yield return new WaitForSeconds(autoReloadDelay);

        if (outOfAmmo == true)
        {
            //Play diff anim if out of ammo
            anim.Play("Reload Out Of Ammo", 0, 0f);

            mainAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
            mainAudioSource.Play();

            //If out of ammo, hide the bullet renderer in the mag
            //Do not show if bullet renderer is not assigned in inspector
            if (bulletInMagRenderer != null)
            {
                bulletInMagRenderer.GetComponent
                <SkinnedMeshRenderer>().enabled = false;
                //Start show bullet delay
                StartCoroutine(ShowBulletInMag());
            }
        }
        //Restore ammo when reloading
        currentAmmo = ammo;
        outOfAmmo = false;
    }

    //Reload 재장전-----------------------------------------------------------------------
    private void Reload()
    {

        if (outOfAmmo == true)
        {
            //Play diff anim if out of ammo
            //anim.Play ("Reload Out Of Ammo", 0, 0f);

            mainAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
            mainAudioSource.Play();

            //If out of ammo, hide the bullet renderer in the mag
            //Do not show if bullet renderer is not assigned in inspector
            if (bulletInMagRenderer != null)
            {
                bulletInMagRenderer.GetComponent
                <SkinnedMeshRenderer>().enabled = false;
                //Start show bullet delay
                StartCoroutine(ShowBulletInMag());
            }
        }
        else
        {
            //Play diff anim if ammo left
            //anim.Play ("Reload Ammo Left", 0, 0f);

            mainAudioSource.clip = SoundClips.reloadSoundAmmoLeft;
            mainAudioSource.Play();

            //If reloading when ammo left, show bullet in mag
            //Do not show if bullet renderer is not assigned in inspector
            if (bulletInMagRenderer != null)
            {
                bulletInMagRenderer.GetComponent
                <SkinnedMeshRenderer>().enabled = true;
            }
        }
        //Restore ammo when reloading
        currentAmmo = ammo;
        outOfAmmo = false;
    }


    //총알관련 함수들(이펙트)
    //Enable bullet in mag renderer after set amount of time
    private IEnumerator ShowBulletInMag()
    {

        //Wait set amount of time before showing bullet in mag
        yield return new WaitForSeconds(showBulletInMagDelay);
        bulletInMagRenderer.GetComponent<SkinnedMeshRenderer>().enabled = true;
    }

    //Show light when shooting, then disable after set amount of time
    private IEnumerator MuzzleFlashLight()
    {

        muzzleflashLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        muzzleflashLight.enabled = false;
    }

    //Check current animation playing
    private void AnimationCheck()
    {

        //Check if reloading
        //Check both animations
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reload Out Of Ammo") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Reload Ammo Left"))
        {
            isReloading = true;
        }
        else
        {
            isReloading = false;
        }

        //Check if inspecting weapon
        //if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Inspect")) 
        //{
        //	isInspecting = true;
        //} 
        //else 
        //{
        //	isInspecting = false;
        //}
    }
}
 