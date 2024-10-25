using UnityEngine;

public enum ImpactType { Normal = 0, Obstacle, Enemy, }
//벽과 바닥은 일반타격, 장애물은 장애물 타격
public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] impactPrefab; //피격이펙트
    private MemoryPool[] memoryPool; //피격이펙트 메모리 풀

    private void Awake()
    {
        // 피격 이펙트가 여러 종류이면 종류별로 memoryPool 생성
        memoryPool = new MemoryPool[impactPrefab.Length];
        for(int i = 0; i < impactPrefab.Length; ++i)
        {
            memoryPool[i] = new MemoryPool(impactPrefab[i]);
        }
    }

    public void SpawnImpact(RaycastHit hit)
    {
        if(hit.transform.CompareTag("ImpactNormal"))
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if(hit.transform.CompareTag("ImpactObstacle"))
        {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if(hit.transform.CompareTag("ImpactEnemy"))
        {
            OnSpawnImpact(ImpactType.Enemy, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactEnemy_Sniper"))
        {
            OnSpawnImpact(ImpactType.Enemy, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    public void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation)
    {
        GameObject item = memoryPool[(int)type].ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = rotation;
        item.GetComponent<Impact>().Setup(memoryPool[(int)type]);
    }
}
