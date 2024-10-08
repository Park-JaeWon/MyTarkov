using System.Collections.Generic;
using UnityEngine;

public class MemoryPool
{
    // 메모리 풀로 관리되는 오브젝트 정보
    private class PoolItem
    {
        public bool isActive; // "gameObject"의 활성화/비활성화 정보
        public GameObject gameObject; //화면에 보이는 실제 게임오브젝트
    }

    private int increaseCount = 5; //오브젝트가 부족할 때 Instantiate()로 추가 생성되는 오브젝트 개수
    private int maxCount; //현재 리스트에 등록되어 있는 오브젝트 개수
    private int activeCount; //현재 게임에 사용되고 있는(활성화) 오브젝트 개수

    private GameObject poolObject; //오브젝트 풀링에서 관리하는 게임 오브젝트 프리팹
    private List<PoolItem> poolItemList; //관리되는 모든 오브젝트를 저장하는 리스트

    public int MaxCount => maxCount; //외부테서 현재 리스트에 등록되어 있는 오브젝트 개수 확인을 위한 프로퍼티
    public int ActiveCount => activeCount; //외부에서 현재 활성화 되어 있는 오브젝트 개수 확인을 위한 프로퍼티

    public MemoryPool(GameObject poolObject)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObject = poolObject;

        poolItemList = new List<PoolItem>();

        InstantiateObjects(); //최초 5개 생성
    }

    //increseCount 개수만큼 오브젝트를 생성
    public void InstantiateObjects()
    {
        maxCount += increaseCount;

        for(int i = 0; i < increaseCount; ++i)
        {
            PoolItem poolItem = new PoolItem();

            poolItem.isActive = false;
            poolItem.gameObject = GameObject.Instantiate(poolObject);
            poolItem.gameObject.SetActive(false); //우선 사용하지 않을 수도 있기때문에 false로 생성

            poolItemList.Add(poolItem);
        }
    }

    //현재 관리중인(활성/비활성) 모든 오브젝트를 삭제
    public void DestroyObjects()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            GameObject.Destroy(poolItemList[i].gameObject);
        }

        poolItemList.Clear();
    }

    public GameObject ActivatePoolItem()
    {
        if (poolItemList == null) return null;

        //현재 생성해서 관리하는 모든 오브젝트 개수와 현재 활성화 상태인 오브젝트 개수 비교
        //모든 오브젝트가 활성화 상태이면 새로운 오브젝트 필요
        if(maxCount == activeCount)
        {
            InstantiateObjects();
        }

        int count = poolItemList.Count;
        for(int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if(poolItem.isActive == false)
            {
                activeCount++;

                poolItem.isActive = true;
                poolItem.gameObject.SetActive(true);

                return poolItem.gameObject;
            }
        }

        return null;
    }

    //현재 사용이 완료된 오브젝트를 비활성화 상태로 설정
    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (poolItemList == null || removeObject == null) return;

        int count = poolItemList.Count;
        for(int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if(poolItem.gameObject == removeObject)
            {
                activeCount--;

                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return;
            }
        }
    }
    
    //게임에 사용중인 모든 오브젝트를 비활성화 상태로 설정
    public void DeactivateAllPoolItems()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for(int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if(poolItem.gameObject != null && poolItem.isActive == true)
            {
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }

        activeCount = 0;
    }
}
