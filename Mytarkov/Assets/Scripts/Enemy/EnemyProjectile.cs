using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private MovementTransform movement;
    private float projectileDistance = 30;//발사체 최대 발사거리
    private int damage = 5;//발사체 공격력
    
    public void Setup(Vector3 position, Vector3 target)
    {
        movement = GetComponent<MovementTransform>();

        StartCoroutine("OnMove", position);
        movement.RotateTo(target);
    }

    private IEnumerator OnMove(Vector3 targetPosition)
    {
        Vector3 start = transform.position;
        //이동방향 설정
        movement.MoveTo((targetPosition - transform.position).normalized);
        
        while(true)
        {
            if(Vector3.Distance(transform.position, start) >= projectileDistance)
            {
                Destroy(gameObject);

                yield break;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //Debug.Log("Player Hit");
            other.GetComponent<PlayerController>().TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
