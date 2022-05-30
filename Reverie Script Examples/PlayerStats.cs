using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine.Events;

public class PlayerStats : CreatureStats
{
    public PlayerStats instance = null;
    public int score;
    public int displayScore;
    public int scoretoAdd;
    public int scrollingScoreAdd;
    [Range(0,6)]
    public int ammo;
    public Camera shootingCamera;
    public float range;
    public Transform checkPoint;
    public GameObject checkPointObject;
    public DeathScreenController myController;
    public GradualCanvasAlpha myDamageAlpha;
    public GradualCanvasAlpha myPickupAlpha;
    public GradualTextAlpha checkpointAlpha;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scorePlusText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI ammoText;
    public bool scrolling;
    public PoolManager bulletHolePool;
    public PoolManager damageGlobPool;
    public PoolManager DamageParticlePool;
    public PoolManager DustParticlePool;
    public PoolManager BulletLinePool;
    public PoolManager StrongHitPool;
    public LayerMask targetMask;
    public Slider healthBar;
    public List<string> heldKeys;
    public Transform gunTransform;
    public PoolManager gunFlashPool;
    public PoolManager gunSmokePool;
    public PoolManager gunLightPool;
    public UnityEvent shootEvent;
    public UnityEvent reloadEvent;
    public bool canFire;
    public GameObject reloadText;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
       
        displayScore = score;
        scoreText.text = "Score \n" + displayScore;
        if (checkPoint != null)
        {
            transform.position = checkPoint.position;
        }  
        else
        {
            checkPointObject = new GameObject();
            checkPoint = checkPointObject.transform;
            SetCheckPointFirst(transform);
        }

       
    }
    public void CanFire()
    {
        canFire = true;
    }

    void Update()
    {
        healthText.text = "" + health;
        healthBar.value = health;
        ammoText.text = "Ammo \n" + ammo;
        if (Input.GetButtonDown("Fire1") && canFire && ammo > 0)
        {
            ShootHitScan();
        }
        else if (Input.GetButtonDown("Fire1") && ammo == 0)
        {
            reloadText.SetActive(true); 
        }
        if (Input.GetKeyDown(KeyCode.R) && ammo < 6)
        {
            Reload();
        }
        CheckForInteractable();
    }
    public void Reload()
    {
        reloadEvent.Invoke();
        ammo = 6;
        canFire = false;
    }
    public void ShootHitScan()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(gunTransform.transform.position, shootingCamera.transform.forward, out hit, range, targetMask))
        {
            LineSpawn(BulletLinePool, hit);
            EffectSpawn(gunFlashPool, gunTransform);
            EffectSpawn(gunSmokePool, gunTransform);
            EffectSpawn(gunLightPool, gunTransform);
            shootEvent.Invoke();
            if (hit.transform.tag == "Wall")
            {
                EffectSpawn(bulletHolePool, hit, .001f, hit.transform);
                EffectSpawn(DustParticlePool, hit);
            }
            if (hit.transform.gameObject.GetComponent<TargetPointStats>())
            {
                TargetPointStats myStats = hit.transform.gameObject.GetComponent<TargetPointStats>();
                myStats.TakeDamage(attack, gameObject);
                if (myStats.uniqueDamagePool != null)
                {
                    EffectSpawn(damageGlobPool, hit, .1f);
                    EffectSpawn(myStats.uniqueDamagePool, hit);
                }
                else
                {
                    EffectSpawn(damageGlobPool, hit, .3f);
                    EffectSpawn(DamageParticlePool, hit);
                }
                if (myStats.multiplier > 1f)
                {
                    EffectSpawn(StrongHitPool, hit,.3f);
                }
                else if (myStats.multiplier < 1f)
                {

                }
            }
            canFire = false;
            ammo -= 1;
        }
    }
    public void EffectSpawn(PoolManager spawnPool, Transform spawnPoint)
    {
        for (int i = 0; i < spawnPool.holePool.Count; i++)
        {
            if (spawnPool.holePool[i].activeInHierarchy == false)
            {
                spawnPool.holePool[i].SetActive(true);
                spawnPool.holePool[i].transform.position = spawnPoint.position;
                spawnPool.holePool[i].transform.rotation = spawnPoint.rotation;
                break;
            }
        }
    }
    public void LineSpawn(PoolManager spawnPool, RaycastHit hit)
    {
        for (int i = 0; i < spawnPool.holePool.Count; i++)
        {
            if (spawnPool.holePool[i].activeInHierarchy == false)
            {
                spawnPool.holePool[i].SetActive(true);
                LineRenderer line = spawnPool.holePool[i].GetComponent<LineRenderer>();
                line.SetPosition(0, gunTransform.position);
                line.SetPosition(1, hit.point);
                break;
            }
        }
    }
    public void EffectSpawn(PoolManager spawnPool, RaycastHit hit)
    {
        for (int i = 0; i < spawnPool.holePool.Count; i++)
        {
            if (spawnPool.holePool[i].activeInHierarchy == false)
            {
                spawnPool.holePool[i].SetActive(true);
                spawnPool.holePool[i].transform.position = Vector3.Lerp(hit.point, shootingCamera.transform.position, .001f);
                spawnPool.holePool[i].transform.rotation = Quaternion.LookRotation(hit.normal);
                break;
            }
        }
    }
    public void EffectSpawn(PoolManager spawnPool, RaycastHit hit, float offset)
    {
        for (int i = 0; i < spawnPool.holePool.Count; i++)
        {
            if (spawnPool.holePool[i].activeInHierarchy == false)
            {
                spawnPool.holePool[i].SetActive(true);
                spawnPool.holePool[i].transform.position = Vector3.Lerp(hit.point, shootingCamera.transform.position, offset);
                spawnPool.holePool[i].transform.rotation = Quaternion.LookRotation(hit.normal);
                break;
            }
        }
    }
    public void EffectSpawn(PoolManager spawnPool, RaycastHit hit, float offset, Transform parentTransform)
    {
        for (int i = 0; i < spawnPool.holePool.Count; i++)
        {
            if (spawnPool.holePool[i].activeInHierarchy == false)
            {
                spawnPool.holePool[i].SetActive(true);
                spawnPool.holePool[i].transform.position = Vector3.Lerp(hit.point, shootingCamera.transform.position, offset);
                if (parentTransform.parent != null)
                {
                    spawnPool.holePool[i].transform.parent = parentTransform.parent;
                }
                spawnPool.holePool[i].transform.rotation = Quaternion.LookRotation(hit.normal);
                break;
            }   
        }
    }

    #region Data Stuff
    public override void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        myDamageAlpha.CanvasAlphaDown(.3f);
        print("Player took " + damageAmount + " damage!");
        if (health <= 0)
        {
            PlayerDeath();
            health = maxHealth;
        }
    }

    public override void Heal(int healAmount)
    {

        myPickupAlpha.CanvasAlphaDown(.3f);
        health += healAmount;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        
    }

    public void PlayerDeath()
    {
        myController.Death(checkPoint.position);
        score = score / 2;
    }

    public void gainScore(int scoreAmount)
    {
        score += scoreAmount;
        if (!scrolling)
        {
            StartCoroutine(scoreScrolling(scoreAmount, 1f));
        }
        //this adds to the counter without calling a new coroutine.
        else
        {
            scoretoAdd += scoreAmount;
            scrollingScoreAdd += scoreAmount;
            scorePlusText.text = "+ " + scoretoAdd;
        }

    }

    public void gainPickupScore(int scoreAmount)
    {
        myPickupAlpha.CanvasAlphaDown(.3f);
        score += scoreAmount;
        if (!scrolling)
        {
            StartCoroutine(scoreScrolling(scoreAmount, 1f));
        }
        //this adds to the counter without calling a new coroutine.
        else
        {
            scoretoAdd += scoreAmount;
            scrollingScoreAdd += scoreAmount;
            scorePlusText.text = "+ " + scoretoAdd;
        }
    }
    public void loseScore(int scoreAmount)
    {
        score -= scoreAmount;
    }

    public void SetCheckPoint(Transform checkPointArg)
    {
        myPickupAlpha.CanvasAlphaDown(.3f);
        checkPoint.position = checkPointArg.position;
        print(checkPoint.position);
    }
    public void SetCheckPointFirst(Transform checkPointArg)
    {
        checkPoint.position = checkPointArg.position;
        print(checkPoint.position);
    }
    public void AddKey(string keyName)
    {
        myPickupAlpha.CanvasAlphaDown(.3f);
        heldKeys.Add(keyName);
        scorePlusText.text = keyName;
        scorePlusText.enabled = true;
        Invoke("DelayDisable", 2f);
    }
    public void AddAmmo(int ammoAmount)
    {
        myPickupAlpha.CanvasAlphaDown(.3f);
        ammo += ammoAmount;
    }
    public void LoseAmmo(int ammoAmount)
    {
        ammo -= ammoAmount;
    }
    public void DelayDisable()
    {
        scorePlusText.enabled = false;
    }
    public IEnumerator scoreScrolling(int plusScore, float delay)
    {    
        scrolling = true;
        scoretoAdd += plusScore;
        scrollingScoreAdd += plusScore;
        scorePlusText.enabled = true;
        scorePlusText.text = "+ " + scoretoAdd;
        scoreloop:
        while (scrollingScoreAdd > 0)
        {
            displayScore++;
            scoreText.text = "Score \n" + displayScore;
            scrollingScoreAdd--;
            yield return new WaitForSeconds(.01f);
        }
        for(float i = delay; i > 0; i-= 0.01f)
        {
            if (scrollingScoreAdd > 0)
            {
                goto scoreloop;
            }
            yield return new WaitForSeconds(.01f);
        }
            scrolling = false;
            scoretoAdd = 0;
            scorePlusText.enabled = false;      
    }
    public void CheckForInteractable()
    {
        RaycastHit hit;
        if(Physics.Raycast(shootingCamera.transform.position, shootingCamera.transform.forward, out hit, 3f, targetMask))
        {
            if (hit.transform.gameObject.GetComponent<InteractableScript>())
            {
                InteractableScript myScript = hit.transform.gameObject.GetComponent<InteractableScript>();
                if (messageText.enabled == false)
                {
                    messageText.text = myScript.InteractionMessage;
                }
                if (Input.GetKeyDown("e"))
                {
                    if (myScript.requiresKey)
                    {
                        myScript.Interaction(heldKeys, this);
                    }
                    else
                    {
                        myScript.Interaction();
                    }
                }
                messageText.enabled = true;        
            }
            else
            {
                messageText.enabled = false;
            }
        }
        else
        {
            messageText.enabled = false;
        }
    }
    #endregion
}
