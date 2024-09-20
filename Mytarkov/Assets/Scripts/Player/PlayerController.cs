using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keyCodeRun = KeyCode.LeftShift; //달리기 키
    [SerializeField]
    private KeyCode keyCodeJump = KeyCode.Space; //점프 키
    [SerializeField]
    private KeyCode keyCodeReload = KeyCode.R; //재장전 키

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk; //걷기 사운드
    [SerializeField]
    private AudioClip audioClipRun; //달리기 사운드

    private RotateToMouse rotateToMouse; //마우스 이동 카메라 회전
    private MovementCharacterController movement; //키보드 입력 플레이어 이동, 점프
    private Status status; //이동속도 등 플렝이어 정보
    private PlayerAnimatorController animator; //애니메이션 재생 제어
    private AudioSource audioSource; //사운드 재생 제어
    private WeaponAssaultRifle weapon; //무기를 이용한 공격 제어

    private void Awake()
    {
        //마우스 커서를 보이지 않게 설정하고, 현재 위치에 고정시킨다.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>();
        movement = GetComponent<MovementCharacterController>();
        status = GetComponent<Status>();
        animator = GetComponent<PlayerAnimatorController>();
        audioSource = GetComponent<AudioSource>();
        weapon = GetComponentInChildren<WeaponAssaultRifle>();
    }

    private void Update()
    {
        UpdateRotate();
        UpdateMove();
        UpdateJump();
        UpdateWeaponAction();
        UpdateReload();
    }

    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }
    
    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        //이동중 일 때
        if( x != 0 || z != 0 )
        {
            bool isRun = false;

            //옆이나 뒤로 이동할 때는 달릴 수 없다
            if (z > 0) isRun = Input.GetKey(keyCodeRun); //쉬프트를 누르면 isRun이 트루가 됨

            movement.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
            animator.MoveSpeed = isRun == true ? 1 : 0.5f;
            audioSource.clip = isRun == true ? audioClipRun : audioClipWalk;

            //방향키 입력 여부는 매 프레임 확인하기 때문에
            //재생중일 때는 다시 재생하지 않도록 isPlaying으로 체크해서 재생
            if(audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        //제자리에 있을 때
        else
        {
            movement.MoveSpeed = 0;
            animator.MoveSpeed = 0;

            if(audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }

        movement.MoveTo(new Vector3(x, 0, z));
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(keyCodeJump))
        {
            movement.Jump();
        }
    }

    private void UpdateWeaponAction()
    {
        if(Input.GetMouseButtonDown(0))
        {
            weapon.StartWeaponAction();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            weapon.StopWeaponAction();
        }
    }

    private void UpdateReload()
    {
        if (Input.GetKeyDown(keyCodeReload))
        {
            weapon.StartReload();
        }
        
    }
}
