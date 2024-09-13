using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private WeaponAssaultRifle weapon; //현재 정보가 출력되는 무기

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName; //무기이름
    [SerializeField]
    private Image imageWeaponIcon; //무기 아이콘
    [SerializeField]
    private Sprite[] spriteWeaponIcons; //무기 아이콘에 사용되는 sprite 배열

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo; //현재/최대 탄 수 출력 text

    private void Awake()
    {
        SetupWeapon();

        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
    }

    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
    }

    private void UpdateAmmoHUD(int currenAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currenAmmo}/</size>{maxAmmo}";
    }
}
