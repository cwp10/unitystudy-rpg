using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skeleton : MonoBehaviour {

    public enum ENEMYSTATE {
        NONE = -1,
        IDLE = 0,
        MOVE,
        ATTACK,
        DAMAGE,
        DEAD
    }

    public ENEMYSTATE enemyState = ENEMYSTATE.IDLE;

    Animator anim = null;

    delegate void FsmFunc();
    Dictionary<ENEMYSTATE, FsmFunc> dicState = new Dictionary<ENEMYSTATE, FsmFunc>();

    public GameObject damageParticle;

    Transform target = null;
    NavMeshAgent _agent = null;
    
    void Awake() {
        anim = GetComponent<Animator>();
        
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;
        enemyState = ENEMYSTATE.IDLE;
    }

    void OnEnable() {
        if (damageParticle != null) {
            damageParticle.SetActive(false);
        }
            
        InitSkeleton();

        if (uiHudBar) {
            uiHudBar.gameObject.SetActive(true);
        }
    }

    void OnDisable() {
        /*if (uiHudBar) {
            uiHudBar.gameObject.SetActive(false);
        }*/
    }

	// Use this for initialization
	void Start () {
        dicState[ENEMYSTATE.NONE] = None;
        dicState[ENEMYSTATE.IDLE] = Idle;
        dicState[ENEMYSTATE.MOVE] = Move;
        dicState[ENEMYSTATE.ATTACK] = Attack;
        dicState[ENEMYSTATE.DAMAGE] = Damage;
        dicState[ENEMYSTATE.DEAD] = Dead;

        FindPlayer();
        StartCoroutine(CreateHpBar());

       
    }

    UiHudBar uiHudBar = null;
    public GameObject hudObject = null;

    IEnumerator CreateHpBar() {
        while (true) {
            GameObject uiRootObject = GameObject.FindGameObjectWithTag("UiRoot");

            if (uiRootObject != null) {
                Transform uiRoot = uiRootObject.transform;

                if (uiRootObject != null) {
                    GameObject obj = Instantiate(hudObject) as GameObject;
                    obj.transform.SetParent(uiRoot, false);
                    uiHudBar = obj.GetComponent<UiHudBar>();
                    uiHudBar.targetTransform = transform;
                    obj.SetActive(gameObject.activeSelf);
                    break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void InitSkeleton() {
        Renderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (Renderer currentRenderer in renderers) {
            currentRenderer.material.shader = Shader.Find("Legacy Shaders/Diffuse");
        }

        healthPoint = 5.0f;
        enemyState = ENEMYSTATE.IDLE;
        stateTime = 0.0f;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = false;

        isKnockBackState = false;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(transform.position, out hit, 99f, NavMesh.AllAreas) == true) {
            transform.position = hit.position;
        }

        _agent.enabled = true;
    }

    void FindPlayer() {
        GameObject findObject = GameObject.FindGameObjectWithTag("Player");

        if (findObject != null) {
            target = findObject.transform;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (isKnockBackState == true) {
            return;
        }
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        dicState[enemyState]();
	}

    void None() {
    }

    float stateTime = 0.0f;
    public float idleStateMaxTime = 2.0f;

    void Idle() {
        stateTime += Time.deltaTime;

        if (stateTime > idleStateMaxTime) {
            stateTime = 0.0f;
            enemyState = ENEMYSTATE.MOVE;
            anim.SetTrigger("move");
        }

        float distance = (target.position - transform.position).magnitude;

        if (distance <= attackRange) {
            stateTime = attackStateMaxTime;
            enemyState = ENEMYSTATE.ATTACK;
            anim.SetTrigger("attack");
        }
    }

    public float moveSpeed = 3.0f;
    public float rotationSpeed = 10.0f;
    public float attackRange = 1.0f;
    public float attackStateMaxTime = 2.0f;

    void Move() {
        float distance = (target.position - transform.position).magnitude;

        if (distance > attackRange) {
            Vector3 dir = target.position - transform.position;
            dir.y = 0.0f;
            dir.Normalize();
            //transform.position += dir * moveSpeed * Time.deltaTime;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);

     
            _agent.Resume();
            _agent.updatePosition = true;
            _agent.SetDestination(target.position);
     
        } else {
            stateTime = attackStateMaxTime;
            enemyState = ENEMYSTATE.ATTACK;
            anim.SetTrigger("attack");

            _agent.Stop();
            _agent.updatePosition = false;
        }
    }

    public AnimationClip attackClip;

    void Attack() {
        stateTime += Time.deltaTime;

        if (stateTime > attackStateMaxTime) {
            stateTime = -attackClip.length;
            anim.SetTrigger("attack");
        }

        float distance = (target.position - transform.position).magnitude;

        if (distance > attackRange) {
            stateTime = 0.0f;
            enemyState = ENEMYSTATE.IDLE;
        }
    }

    void OnAttack() {
        if (target == null) {
            return;
        }

        target.SendMessage("OnDamage", Random.Range(1.0f, 2.0f), SendMessageOptions.RequireReceiver);
    }

    void Damage() {
        enemyState = ENEMYSTATE.IDLE;
    }

    void Dead() {
        anim.SetBool("dead", true);
        enemyState = ENEMYSTATE.NONE;
        
        StartCoroutine(DeadProcess());

        target.SendMessage("ResetTarget", SendMessageOptions.DontRequireReceiver);
    }

    public float healthPoint = 5.0f;
    public float maxHelth = 5.0f;

    public void OnDamage() {
        _agent.updatePosition = false;
        _agent.Stop();

        damageParticle.SetActive(true);
        --healthPoint;

        if (uiHudBar) {
            uiHudBar.UpdateHpBar(healthPoint / maxHelth);
        }

        if (healthPoint > 0) {
            enemyState = ENEMYSTATE.DAMAGE;
            anim.SetFloat("damage", Random.Range(1, 3));
        } else {
            enemyState = ENEMYSTATE.DEAD;
        }
    }

    public void ResetDamage() {
        anim.SetFloat("damage", 0.0f);
    }

    public float fadeOutSpeed = 0.5f;
    public float fadeWaitTime = 2.0f;

    IEnumerator DeadProcess() {
        yield return new WaitForSeconds(fadeWaitTime);

        if (uiHudBar) {
            uiHudBar.gameObject.SetActive(false);
        }

        Renderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        Shader transparentShader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
        renderers[0].material.shader = transparentShader;
        //renderers[1].material.shader = transparentShader;

        Color fadeOutColor = Color.white;

        while (fadeOutColor.a > 0.0f) {
            yield return new WaitForEndOfFrame();

            fadeOutColor.a -= fadeOutSpeed * Time.deltaTime;

            renderers[0].material.color = fadeOutColor;
            //renderers[1].material.color = fadeOutColor;
        }

        fadeOutColor.a = 0.0f;
        renderers[0].material.color = fadeOutColor;
        //renderers[1].material.color = fadeOutColor;

        enemyState = ENEMYSTATE.NONE;
        //gameObject.SetActive(false);

        ObjectPool.instance.PoolObject(gameObject);

        yield return null;
    }

    bool isKnockBackState = false;

    public void KnockBack(float explosionForce, Vector3 pos) {
        isKnockBackState = true;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().AddExplosionForce(explosionForce, pos, 10.0f, 1.0f, ForceMode.Impulse);
        OnDamage();

        StartCoroutine(DamageOver());
    }

    IEnumerator DamageOver() {
        while (true) {
            yield return new WaitForFixedUpdate();

            if (GetComponent<Rigidbody>().velocity.y == 0) {
                Vector3 tempPos = transform.position;
                tempPos.y = 0.0f;
                transform.position = tempPos;

                isKnockBackState = false;
            }
        }
    }

    bool isJump = false;

    void OnCollisionEnter(Collision col) {
        if (isJump == true && col.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            isJump = false;

            Vector3 tempPos = transform.position;
            tempPos.y = 0.0f;
            transform.position = tempPos;

            isKnockBackState = false;
        }
    }

    void OnCollisionExit(Collision col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            isJump = true;
        }
    }
}
