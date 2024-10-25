using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum Enemy_Sniper_State { None = -1, Idle = 0, Attack, }

public class EnemyFSM_Sniper : MonoBehaviour
{
    [SerializeField]
    private Transform target;//타겟

    [Header("Pursuit")]
    [SerializeField]
    private float targetRecongnitionRange = 8;//인식 범위

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;//발사체 프리팹
    [SerializeField]
    private Transform projectileSpawnPoint;//발사체 생성 위치
    [SerializeField]
    private float attackRange = 100;//공격 범위(이 안에 들어오면 "Attack"상태로 변경)
    [SerializeField]
    private float attackRate = 1;//공격 속도

    private Enemy_Sniper_State enemy_Sniper_State = Enemy_Sniper_State.None;//현재 적 상태
    private float lastAttackTime = 0;//공격 주기 계산

    private Status status;//이동속도 등의 정보
    private Animator animator;


    private void Awake()
    {
        status = GetComponent<Status>();
        animator = GetComponent<Animator>();

        ChangeState(Enemy_Sniper_State.Idle);
    }

    private void OnEnable()
    {
        //적이 활성화될 때 적의 상태를 Idle로 설정
        //ChangeState(Enemy_Sniper_State.Idle);
    }

    private void OnDisable()
    {
        StopCoroutine(enemy_Sniper_State.ToString());//현재 상태 정지

        enemy_Sniper_State = Enemy_Sniper_State.None;
    }

    public void ChangeState(Enemy_Sniper_State newState)
    {
        //현재 상태와 바꿀상태가 같으면 바꿀 필요없기 때문에 return
        if (enemy_Sniper_State == newState) return;

        //이전에 재생중이던 상태 종료
        StopCoroutine(enemy_Sniper_State.ToString());
        //현재 적의 생태를 newState로 설정
        enemy_Sniper_State = newState;
        //새로운 상태 재생
        StartCoroutine(enemy_Sniper_State.ToString());
    }

    private IEnumerator Idle()
    {
        while (true)
        {
            //"대기" 상태일 때 하는 행동
            animator.SetBool("Attack", false);
            //타겟과의 거리에 따라 행동 선택
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            animator.SetBool("Attack", true);
            //타겟 방향 주시
            LookRotationToTarget();

            //타겟과의 거리에 따라 행동 선택
            CalculateDistanceToTargetAndSelectState();

            if (Time.time - lastAttackTime > attackRate)
            {
                //공격주기가 되어야 공격할 수 있도록 현재 시간 저장
                lastAttackTime = Time.time;

                //발사체 생성
                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().Setup(target.position, new Vector3(target.position.x, target.position.y - 90, target.position.z));
            }

            yield return null;
        }
    }

    private void LookRotationToTarget()
    {
        //목표 위치
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        //내 위치
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z); 

        //바로 돌기
        transform.rotation = Quaternion.LookRotation(to - from);

        //서서히 돌기
        //Quaternion rotation = Quaternion.LookRotation(to - from);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
    }

    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return;

        //플레이어(target)와 적의 거리 계산 후 거리에 따라 행동 선택
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= attackRange)
        {
            ChangeState(Enemy_Sniper_State.Attack);
        }
        else
        {
            ChangeState(Enemy_Sniper_State.Idle);
        }
    }

    private void OnDrawGizmos()
    {
        //목표 인식 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecongnitionRange);

        //공격 범위
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void Sniper_TakeDamage(int damage)
    {
        Debug.Log(damage);
        bool isDie = status.DecreseHP(damage);

        if (isDie == true)
        {
            Destroy(this.gameObject);
        }
    }
}
