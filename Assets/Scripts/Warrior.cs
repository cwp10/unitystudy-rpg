using UnityEngine;
using System.Collections;

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

        /*if (Input.GetKeyDown(KeyCode.A)) {
            _animator.SetTrigger("attack");
        }*/


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

    void OnTriggerEnter(Collider other) {
        if (other.tag != "Monsters") {
            return;
        }

        targetEnemy = other.transform;
    }

    void OnTriggerExit(Collider other) {
        if (other.tag != "Monsters") {
            return;
        }

        targetEnemy = null;
    }

    void OnAttack() {
        if (targetEnemy == null) {
            return;
        }

        targetEnemy.SendMessage("OnDamage", SendMessageOptions.RequireReceiver);
    }
}
