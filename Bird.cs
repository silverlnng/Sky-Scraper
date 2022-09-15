using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//�ﵹ���� ���̰� �ö󰥼��� ���� ������ ��������
// - ���̿� ���� ���� ������ �Ѵ�
// �� ���������� ���� ��ġ �ϵ��� (�� ��ũ���� �޸� �������� ���� ��ġ)  


// - �ﵹ�� ���� ���̿� ���� ���� ���ƿ���  ������ �����Ѵ�
// �� ����  ������ ���ƿ´� . �߰߹����ȿ� ������  Idle =>Move
// ��Move �߿��� ������ ���Ҽ�����  Move =>Damaged


//���� ������ �Ѵ�. ���ݹ��� ������ Target �� ������ ����
//- ���� ���� : attack range ( ���� target ������ �Ÿ�) 
//- ���ݿ��� 2���� : ī�޶� ���� ���� / â���� ���� ���� 
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

    public Transform target;  // �÷��̾� 

    public float attackRange = 2f;
    public float detectRange = 5f;

    public float moveSpeed = 5f;  //-�̵��ӵ� , ����
    public float rotateSpeed = 5f;    //- player ���� ���� ȸ���ϰ� �̵� , �� ȸ���ϴ� �ӵ� 
    public float runwaySpeed = 5f;    // �������� �ӵ� 
    float distance;
    Vector3 direction;
    Animator animator;

    float currentTime = 0;
    AudioSource audioSource;
    SkinnedMeshRenderer skinnedMeshRenderer;

    public Image imgHit;            // -> ȭ���� ��½�Ÿ��� ȿ�� UI �� 
    public float damageSpeed = 1f;    // -> ��½�Ÿ��� �ӵ�

    [Range(0.2f, 0.8f)] // �ٷ� �Ʒ����� ���� , �̹��� �ȿ��� �ν����Ϳ��� �����Ҽ��ֵ���
    public float originDamageAlpha = 0.6f; // damage ui �� ���� �� �� ->���ݴ��������� ��

    EnemyState enemyState = EnemyState.Idle;

    void Start()
    {
        audioSource=GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();

        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        skinnedMeshRenderer.enabled = false;

        DisplayHitUI(0f); // �Է°� �Ķ���� �� �ʱ�ȭ ���ֱ� 
    }

    // Update is called once per frame
    void Update()
    {
        //Quaternion offsetAngle = target.transform.rotation;

        direction = (target.transform.position - transform.position);

       
        //direction.y = 0;

        distance = Vector3.Distance(transform.position, target.position);

        switch (enemyState) //FSM �� �ٸ� �Լ� ����
        {
            case EnemyState.Idle: Idle(); break;
            case EnemyState.Move: Move(); break;
            case EnemyState.Attack: Attack(); break;
            case EnemyState.Damaged: Damaged(); break;
        }

        //���� ���ݴ�������  damage ui  ���� ���� ȿ�� �����
        // imgHit �� alpha ���� �������� ���¶�� ( alpha ���� 0���� ū ��� )
        //imgHit �� Alpha ���� ���� ���� (0���� ����� )
        // damageSpeed �ӵ���
        // imgHit�� ������ (damage ���Ҷ�)  ���� �󵵴� originDamageAlpha ���� ��

        if (imgHit.color.a > 0)  // imgHit �� alpha ���� �������� ���¶�� ( alpha ���� 0���� ū ��� )
        {
            Color c = imgHit.color;

            c.a -= damageSpeed * Time.deltaTime; //c.a  -> �÷��� Alpha ���� ����

            imgHit.color = c; //��ȭ�� �÷��� c �� �ٽ� ���� 

        }

    }

    
    void ChangeState(EnemyState state)
    {
        if (state == EnemyState.Idle) { }
        else if (state == EnemyState.Move) { }
        else if (state == EnemyState.Attack) { }
        else if (state == EnemyState.Damaged) { }

        animator.SetTrigger(state.ToString()); //�̰ɷ� ������ �ִϸ��̼� ������ ���ص� �� state ���� ���ڿ��� ��ȯ

        enemyState = state; //enemyState �� ���� ->Update() �� switch (enemyState) �� �۵� 

    }
    
    void Idle()
    {

        if (distance < detectRange)
        {
            Debug.Log("Player  �߰�");
            skinnedMeshRenderer.enabled = true;
            audioSource.Play();
            ChangeState(EnemyState.Move);
        }

    }

    void Move()
    {

        /* transform.LookAt(direction); *///lookat �Լ� ȸ������ ������.
                                          //lookat �Լ��� target�� �ٷ� �ٶ󺸾Ƽ� ������ ���� ȸ���� �ƴ�

        //3. Target �� �ִ� ��������
        Quaternion Qdirection = Quaternion.LookRotation(direction); //Vector3 �������� Quaternion�������� ��ȯ

        //3-1 õõ�� Target �������� ������ ȸ���Ѵ�
        //transform.rotation(a,b,c,) a����  B �� ���� �������� c�� �ӵ��� ȸ���� �ϴ� �Լ�
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Qdirection, rotateSpeed * Time.deltaTime);

        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;   //1. Target �� ���� �̵�//2.moveSpeed �ӵ���


        if (distance < attackRange)
        {
            Debug.Log("Player  ����");
            ChangeState(EnemyState.Attack);
        }

    }
    void Attack()
    {
        // �ִϸ����� ���� ���� �ִϸ��̼� Ŭ���� �� ��� �ð� / (0)�� ���̾�, / [0]�� ����
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

        //������ �������� �ִϰ� ��� , �׳� �ٸ� ������ Ʋ� �̵��ϴ°ɷ��ϱ� 
        //�����Ŀ��� �׳� �ٸ� ������ Ʋ� �̵��ϱ� 
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void DisplayHitUI(float alpha)   // �Է°� �Ķ���� �� �ʱ�ȭ ���ֱ� 
                                                          //Ÿ����  ���ݴ�������  damage ui  ���� ���� ȿ�� �����
    {
        Color c = imgHit.color;
        c.a = alpha;   // imgHit�� damage ���Ҷ� ���� �󵵴� originDamageAlpha ���� ��

        imgHit.color = c; //��ȭ�� Color c �� ���� 
    }


}
