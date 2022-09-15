using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//곤돌라의 높이가 올라갈수록 새의 공격이 심해진다
// - 높이에 따라 자주 공격을 한다
// ㄴ 높아질수록 많이 배치 하도록 (이 스크립이 달린 오브젯을 많이 배치)  


// - 곤돌라가 일정 높이에 오면 새가 날아오고  공격을 시작한다
// ㄴ 새가  나에게 날아온다 . 발견범위안에 있으면  Idle =>Move
// ㄴMove 중에는 공격을 당할수있음  Move =>Damaged


//새가 공격을 한다. 공격범위 안으로 Target 이 들어오면 공격
//- 공격 범위 : attack range ( 나와 target 사이의 거리) 
//- 공격에는 2종류 : 카메라에 새똥 공격 / 창문에 새똥 오염 
//-

public class Bird : MonoBehaviour
{
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Damaged
    }

    public Transform target;  // 플레이어 

    public float attackRange = 2f;
    public float detectRange = 5f;

    public float moveSpeed = 5f;  //-이동속도 , 방향
    public float rotateSpeed = 5f;    //- player 에게 몸을 회전하고 이동 , 그 회전하는 속도 
    public float runwaySpeed = 5f;    // 도망가는 속도 
    float distance;
    Vector3 direction;
    Animator animator;

    float currentTime = 0;
    AudioSource audioSource;
    SkinnedMeshRenderer skinnedMeshRenderer;

    public Image imgHit;            // -> 화면을 번쩍거리는 효과 UI 로 
    public float damageSpeed = 1f;    // -> 번쩍거리는 속도

    [Range(0.2f, 0.8f)] // 바로 아래값에 적용 , 이범위 안에서 인스펙터에서 조절할수있도록
    public float originDamageAlpha = 0.6f; // damage ui 의 투명도 의 농도 ->공격당했을때의 농도

    EnemyState enemyState = EnemyState.Idle;

    void Start()
    {
        audioSource=GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();

        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        skinnedMeshRenderer.enabled = false;

        DisplayHitUI(0f); // 입력값 파라미터 로 초기화 해주기 
    }

    // Update is called once per frame
    void Update()
    {
        //Quaternion offsetAngle = target.transform.rotation;

        direction = (target.transform.position - transform.position);

       
        //direction.y = 0;

        distance = Vector3.Distance(transform.position, target.position);

        switch (enemyState) //FSM 에 다른 함수 실행
        {
            case EnemyState.Idle: Idle(); break;
            case EnemyState.Move: Move(); break;
            case EnemyState.Attack: Attack(); break;
            case EnemyState.Damaged: Damaged(); break;
        }

        //만약 공격당했을때  damage ui  에서 맞은 효과 만들기
        // imgHit 의 alpha 값이 불투명한 상태라면 ( alpha 값이 0보다 큰 경우 )
        //imgHit 의 Alpha 값이 점점 투명 (0으로 만들기 )
        // damageSpeed 속도로
        // imgHit의 맞을때 (damage 당할때)  색상 농도는 originDamageAlpha 값의 농도

        if (imgHit.color.a > 0)  // imgHit 의 alpha 값이 불투명한 상태라면 ( alpha 값이 0보다 큰 경우 )
        {
            Color c = imgHit.color;

            c.a -= damageSpeed * Time.deltaTime; //c.a  -> 컬러중 Alpha 값만 접근

            imgHit.color = c; //변화된 컬러값 c 를 다시 적용 

        }

    }

    
    void ChangeState(EnemyState state)
    {
        if (state == EnemyState.Idle) { }
        else if (state == EnemyState.Move) { }
        else if (state == EnemyState.Attack) { }
        else if (state == EnemyState.Damaged) { }

        animator.SetTrigger(state.ToString()); //이걸로 일일이 애니메이션 설정을 안해도 됨 state 값을 문자열로 변환

        enemyState = state; //enemyState 를 변경 ->Update() 의 switch (enemyState) 을 작동 

    }
    
    void Idle()
    {

        if (distance < detectRange)
        {
            Debug.Log("Player  발견");
            skinnedMeshRenderer.enabled = true;
            audioSource.Play();
            ChangeState(EnemyState.Move);
        }

    }

    void Move()
    {

        /* transform.LookAt(direction); *///lookat 함수 회전값을 맞춰줌.
                                          //lookat 함수는 target을 바로 바라보아서 서서히 도는 회전이 아님

        //3. Target 이 있는 방향으로
        Quaternion Qdirection = Quaternion.LookRotation(direction); //Vector3 방향으로 Quaternion방향으로 변환

        //3-1 천천히 Target 방향으로 서서히 회전한다
        //transform.rotation(a,b,c,) a에서  B 를 보는 방향으로 c의 속도로 회전을 하는 함수
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Qdirection, rotateSpeed * Time.deltaTime);

        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;   //1. Target 을 향해 이동//2.moveSpeed 속도로


        if (distance < attackRange)
        {
            Debug.Log("Player  공격");
            ChangeState(EnemyState.Attack);
        }

    }
    void Attack()
    {
        // 애니메이터 실행 중인 애니메이션 클립의 총 재생 시간 / (0)은 레이어, / [0]은 순서
        float playTime = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        Debug.Log(playTime);
     
        currentTime += Time.deltaTime;

        if (currentTime >= playTime)
        {
            ChangeState(EnemyState.Damaged);
            DisplayHitUI(originDamageAlpha);
            currentTime = 0;
        }
    }
    void Damaged()
    {
        direction.Normalize();
        Quaternion Qdirection = Quaternion.LookRotation(-direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Qdirection, rotateSpeed * Time.deltaTime);

        transform.position -= direction * runwaySpeed * Time.deltaTime;

        //공격을 당했을때 애니가 없어서 , 그냥 다른 방향을 틀어서 이동하는걸로하기 
        //공격후에도 그냥 다른 방향을 틀어서 이동하기 
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void DisplayHitUI(float alpha)   // 입력값 파라미터 로 초기화 해주기 
                                                          //타겟이  공격당했을때  damage ui  에서 맞은 효과 만들기
    {
        Color c = imgHit.color;
        c.a = alpha;   // imgHit의 damage 당할때 색상 농도는 originDamageAlpha 값의 농도

        imgHit.color = c; //변화된 Color c 을 적용 
    }


}
