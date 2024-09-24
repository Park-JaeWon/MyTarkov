using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AmmoEvent : UnityEvent<int, int> { }
[System.Serializable]
public class MagazineEvent : UnityEvent<int> { }
public class WeaponAssaultRifle : MonoBehaviour
{
    [HideInInspector] //public변수인데 인스펙터창에 보이지 않게 한다.
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent onMagazineEvent = new MagazineEvent();

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
    [SerializeField]
    private AudioClip audioClipReload; //재장전 사운드
    [SerializeField]
    private AudioClip audioClipChamberReload; //약실재장전 사운드

    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting; //무기 설정

    private float lastAttackTime = 0; //마지막 발사시간 체크
    private bool isReload = false;// 재장전 중인지 체크

    private AudioSource audioSource; //사운드 재생 컴포넌트
    private PlayerAnimatorController animator; //애니메이션 재생 제어
    private CasingMemoryPool casingMemoryPool; //탄피 생성 후 활성/비활성 관리

    //외부에서 필요한 정보를 열람하기 위해 정의한 Get Property
    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<PlayerAnimatorController>();
        casingMemoryPool = GetComponent<CasingMemoryPool>();

        //시작시 최대 탄수로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        //시작시 최대 탄창 수로 설정
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
    }

    private void OnEnable()
    {
        PlaySound(audioClipTakeOutWeapon);
        muzzleFlashEffect.SetActive(false);
        //무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신한다.
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
        //탄창 정보 갱신
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
    }

    public void StartWeaponAction(int type = 0)
    {
        //재장전 중 공격 불가
        if (isReload == true) return; 
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

    private IEnumerator OnReload()
    {
        bool reloadSound = false; //약실에 탄 유무
        isReload = true; //장전 중
        animator.OnReload();

        if (weaponSetting.currentAmmo > 0)//탄약이 남아있으면
        {
            animator.Ammo = 0;//노리쇠 후퇴고정X 애니메이션 및 사운드
            reloadSound = false;
        }
        else // 탄약이 0이면
        {
            animator.Ammo = 1;//노리쇠 후퇴고정O
            reloadSound = true;
        }

        if (reloadSound)
            PlaySound(audioClipChamberReload);
        else
            PlaySound(audioClipReload);

        while (true)
        {
            if (audioSource.isPlaying == false && animator.CurrentAnimIs("Movement"))
            {
                isReload = false;

                weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
    }

    public void StartReload()
    {
        //재장전 중이거나 탄창수가 0이면 재장전 불가능
        if (isReload == true || weaponSetting.currentMagazine <= 0) return;

        StopWeaponAction();

        StartCoroutine("OnReload");
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
