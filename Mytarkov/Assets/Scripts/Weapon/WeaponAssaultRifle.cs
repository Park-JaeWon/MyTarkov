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
    private GameObject muzzleFlashEffect; //총구 이펙트

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint; //탄피 생성 위치

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon; //무기 장착 사운드
    [SerializeField]
    private AudioClip audioClipFire; //공격 사운드

    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting; //무기 설정

    private float lastAttackTime = 0; //마지막 발사시간 체크

    private AudioSource audioSource; //사운드 재생 컴포넌트
    private PlayerAnimatorController animator; //애니메이션 재생 제어
    private CasingMemoryPool casingMemoryPool; //탄피 생성 후 활성/비활성 관리

    //외부에서 필요한 정보를 열람하기 위해 정의한 Get Property
    public WeaponName WeaponName => weaponSetting.weaponName;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<PlayerAnimatorController>();
        casingMemoryPool = GetComponent<CasingMemoryPool>();

        //시작시 최대 탄수로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        PlaySound(audioClipTakeOutWeapon);
        muzzleFlashEffect.SetActive(false);
        //무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신한다.
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    public void StartWeaponAction(int type = 0)
    {
        //마우스 왼쪽 클릭 (공격 시작)
        if(type == 0)
        {
            //연발
            if(weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
            //단발
            else
            {
                OnAttack();
            }
        }
    }

    public void StopWeaponAction(int type = 0)
    {
        //마우스 왼쪽 클릭 (공격 종료)
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
            //뛰고 있을때는 공격 불가
            if(animator.MoveSpeed > 0.5f)
            {
                return;
            }
            //공격주기 체크를 위해 현재시간 저장
            lastAttackTime = Time.time;

            //탄 수가 없으면 공격 불가능
            if(weaponSetting.currentAmmo <= 0)
            {
                return;
            }

            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            animator.Play("Fire", -1, 0);
            StartCoroutine("OnMuzzleFlashEffect");
            PlaySound(audioClipFire);
            //탄피 생성
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
        audioSource.Stop();         //기존 사운드 정지
        audioSource.clip = clip;    //새로운 사운드 clip으로 교체
        audioSource.Play();         //재생
    }
}
