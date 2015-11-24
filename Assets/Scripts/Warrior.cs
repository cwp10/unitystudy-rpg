using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Warrior : MonoBehaviour {

    Animator _animator;
    Transform _transform;

    public float moveSpeed = 5.0f;
    public float rotationSpeed = 10.0f;
    Vector3 moveToPosition = Vector3.zero;

    public GameObject arrow;
    Transform targetEnemy = null;

    float deltaAttackTime = 0.0f;
    public float attackMaxTime = 3.0f;

    public float currentHealth = 10.0f;
    public float maxHealth = 10.0f;

    public PlayerStateUi playerStateUi = null;

    public List<Transform> monsterList = new List<Transform>();
    public int CompareTransform(Transform value1, Transform value2) {
        float dist1 = (transform.position - value1.position).magnitude;
        float dist2 = (transform.position - value2.position).magnitude;

        return dist1.CompareTo(dist2);
    }

   

    void Awake() {
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();

        moveToPosition = _transform.position;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.A)) {
            DoSkill();
        }

        if (targetEnemy == null) {
            FindBestTarget();
        }


        if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int groundMask = 1 << LayerMask.NameToLayer("Ground");
            RaycastHit hitInfo;
            bool result = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, groundMask);

            if (result) {
                moveToPosition = hitInfo.point;

                if (arrow) {
                    arrow.transform.position = moveToPosition;
                    arrow.SetActive(true);
                }
            }
        }

        MoveProcess();
    }

    void MoveProcess() {
        if (Vector3.Distance(_transform.position, moveToPosition) > 0.05f) {
            _animator.SetBool("run", true);

            Vector3 dir = moveToPosition - _transform.position;
            dir.y = 0.0f;
            dir.Normalize();

            _transform.position += dir * moveSpeed * Time.deltaTime;

            Quaternion from = _transform.rotation;
            Quaternion to = Quaternion.LookRotation(dir);
            _transform.rotation = Quaternion.Lerp(from, to, rotationSpeed * Time.deltaTime);
        } else {
            if (arrow) {
                arrow.SetActive(false);
            }
            _animator.SetBool("run", false);

            AttackToTarget();
        }
    }

    void AttackToTarget() {
        if (targetEnemy == null) {
            return;
        }

        if (targetEnemy.gameObject.activeSelf == false) {
            return;
        }

        if ((targetEnemy.position - transform.position).magnitude > 4.0f) {
            monsterList.Remove(targetEnemy);
            targetEnemy = null;
            return;
        }

        Vector3 dir = targetEnemy.position - transform.position;
        dir.y = 0.0f;
        dir.Normalize();

        Quaternion from = transform.rotation;
        Quaternion to = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(from, to, rotationSpeed * Time.deltaTime);

        deltaAttackTime += Time.deltaTime;

        if (deltaAttackTime > attackMaxTime) {
            deltaAttackTime = 0.0f;
            _animator.SetTrigger("attack");
        }
    }

    void FindBestTarget() {
        if (targetEnemy != null) {
            return;
        }

        if (monsterList.Count == 0) {
            return;
        }

        for (int i = 0; i < monsterList.Count;) {
            if (monsterList[i] == null) {
                monsterList.RemoveAt(i);
            } else {
                ++i;
            }
        }

        monsterList.Sort(CompareTransform);

        if (monsterList.Count > 0) {
            targetEnemy = monsterList[0];
        }
    }

    public void ResetTarget() {
        if (targetEnemy != null) {
            monsterList.Remove(targetEnemy);
        }

        targetEnemy = null;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag != "Monsters") {
            return;
        }

        //targetEnemy = other.transform;
        if (monsterList.Contains(other.transform) == false) {
            monsterList.Add(other.transform);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag != "Monsters") {
            return;
        }

        //targetEnemy = null;

        if (monsterList.Contains(other.transform) == false) {
            return;
        }

        monsterList.Remove(other.transform);

        if (targetEnemy == other.transform) {
            targetEnemy = null;
        }
    }

    void OnAttack() {
        if (targetEnemy == null) {
            return;
        }

        targetEnemy.SendMessage("OnDamage", SendMessageOptions.RequireReceiver);
    }

    public void OnDamage(float damage) {
        currentHealth -= damage;

        float hpRatio = currentHealth / maxHealth;
        playerStateUi.UpdateBar(PlayerStateUi.SLIDERTYPE.HP, hpRatio);
        StartCoroutine(DamageProcess());
    }

    Shader orgShader = null;
    Shader damageShader = null;

    IEnumerator DamageProcess() {
        Renderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (orgShader == null) {
            orgShader = Shader.Find("Standard");
        }

        if (damageShader == null) {
            damageShader = Shader.Find("Unlit/Damage");
        }

        renderers[0].material.shader = damageShader;
        yield return new WaitForSeconds(0.05f);
        renderers[0].material.shader = orgShader;
        yield return null;
    }

    public GameObject skillObject;
    public float skillCoolTime = 2.0f;
    public float explosionForce = 7.0f;

    IEnumerator SkillProcess() {
        _animator.SetTrigger("skill");
        skillObject.SetActive(true);

        int monsterLayer = 1 << LayerMask.NameToLayer("Monsters");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3.0f, monsterLayer);

        foreach (Collider enemyCollider in hitColliders) {
            Skeleton skeleton = enemyCollider.GetComponent<Skeleton>();
            skeleton.KnockBack(explosionForce, transform.position);
        }

        yield return new WaitForSeconds(skillCoolTime);
        skillObject.SetActive(false);
    }

    public void DoSkill() {
        StartCoroutine(SkillProcess());
    }
}
