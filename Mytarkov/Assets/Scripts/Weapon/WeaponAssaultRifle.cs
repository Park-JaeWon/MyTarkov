using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AmmoEvent : UnityEvent<int, int> { }
public class WeaponAssaultRifle : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect; //�ѱ� ����Ʈ

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint; //ź�� ���� ��ġ

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon; //���� ���� ����
    [SerializeField]
    private AudioClip audioClipFire; //���� ����

    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting; //���� ����

    private float lastAttackTime = 0; //������ �߻�ð� üũ

    private AudioSource audioSource; //���� ��� ������Ʈ
    private PlayerAnimatorController animator; //�ִϸ��̼� ��� ����
    private CasingMemoryPool casingMemoryPool; //ź�� ���� �� Ȱ��/��Ȱ�� ����

    //�ܺο��� �ʿ��� ������ �����ϱ� ���� ������ Get Property
    public WeaponName WeaponName => weaponSetting.weaponName;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<PlayerAnimatorController>();
        casingMemoryPool = GetComponent<CasingMemoryPool>();

        //���۽� �ִ� ź���� ����
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        PlaySound(audioClipTakeOutWeapon);
        muzzleFlashEffect.SetActive(false);
        //���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ �����Ѵ�.
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    public void StartWeaponAction(int type = 0)
    {
        //���콺 ���� Ŭ�� (���� ����)
        if(type == 0)
        {
            //����
            if(weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
            //�ܹ�
            else
            {
                OnAttack();
            }
        }
    }

    public void StopWeaponAction(int type = 0)
    {
        //���콺 ���� Ŭ�� (���� ����)
        if(type == 0)
        {
            StopCoroutine("OnAttackLoop");
        }
    }

    private IEnumerator OnAttackLoop()
    {
        while (true)
        {
            OnAttack();

            yield return null;
        }
    }

    public void OnAttack()
    {
        if(Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            //�ٰ� �������� ���� �Ұ�
            if(animator.MoveSpeed > 0.5f)
            {
                return;
            }
            //�����ֱ� üũ�� ���� ����ð� ����
            lastAttackTime = Time.time;

            //ź ���� ������ ���� �Ұ���
            if(weaponSetting.currentAmmo <= 0)
            {
                return;
            }

            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            animator.Play("Fire", -1, 0);
            StartCoroutine("OnMuzzleFlashEffect");
            PlaySound(audioClipFire);
            //ź�� ����
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);
        }
    }

    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         //���� ���� ����
        audioSource.clip = clip;    //���ο� ���� clip���� ��ü
        audioSource.Play();         //���
    }
}
