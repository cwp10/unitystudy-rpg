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

    void Awake() {
        anim = GetComponent<Animator>();
        enemyState = ENEMYSTATE.IDLE;
    }

    void OnEnable() {
        damageParticle.SetActive(false);
        InitSkeleton();
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
    }

    Transform target = null;
    void FindPlayer() {
        GameObject findObject = GameObject.FindGameObjectWithTag("Player");

        if (findObject != null) {
            target = findObject.transform;
        }
    }
	
	// Update is called once per frame
	void Update () {
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
            transform.position += dir * moveSpeed * Time.deltaTime;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
        } else {
            stateTime = attackStateMaxTime;
            enemyState = ENEMYSTATE.ATTACK;
            anim.SetTrigger("attack");
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

    void Damage() {
        enemyState = ENEMYSTATE.IDLE;
    }

    void Dead() {
        anim.SetBool("dead", true);
        enemyState = ENEMYSTATE.NONE;

        StartCoroutine(DeadProcess());
    }

    public float healthPoint = 10;

    public void OnDamage() {
        damageParticle.SetActive(true);
        --healthPoint;

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
}
