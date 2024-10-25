using UnityEngine;

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();//HPEvent클래스 인스턴스

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;

    [Header("HP")]
    [SerializeField]
    private int maxHP = 100;
    private int currentHP;

    public float WalkSpeed => walkSpeed;//외부 열람용 프로퍼티
    public float RunSpeed => runSpeed;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public bool DecreseHP(int damage)
    {
        int previousHP = currentHP;

        currentHP = currentHP - damage > 0 ? currentHP - damage : 0;
        //현재 체력 - 데미지 > 0 이면 currentHP - damage를 그렇지 않다면 0
        onHPEvent.Invoke(previousHP, currentHP);

        if(currentHP == 0)
        {
            return true;
        }

        return false;
    }
}
