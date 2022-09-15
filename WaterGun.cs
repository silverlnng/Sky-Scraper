using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGun : MonoBehaviour
{
    
    public Transform firePos;
    [SerializeField] private Texture2D dirtBrush;

    public ParticleSystem waterSprayParticleSystem;
    public Transform RightHand;
    public Transform LeftHand;

   
    float  currentTime;

    AudioSource sound;
    /*  public Stain stain;   */      // ��� ����� �Լ� ���� => public ���� �����ؼ� �������ֱ� 

    private void Start()
    {
        //gameObject.SetActive(false);
        sound=GetComponent<AudioSource>();
    }

    void Update()
    {
        /* if (GameManager.Instance.isPlaying == false) { return; } *///ui ��忡���� �۵��̾ȵǵ��� ����� 

        //currentTime +=Time.deltaTime;
        //if (currentTime >= LiftUp.instance.autoLiftTime*7)
        //{ gameObject.SetActive(true); }


        if ((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) && transform.parent == RightHand) ||
            (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) && transform.parent == LeftHand))
        {
            sound.Stop();
            sound.Play();
            waterSprayParticleSystem.Stop();
            waterSprayParticleSystem.Play();

        }

        if ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)&&transform.parent== RightHand)||
            (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) && transform.parent == LeftHand))
            ///*||OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch)*/)
            ////if (Input.GetMouseButton(1))
        {

            Debug.Log("���� ���Ǵ޼�");
            sound.Stop();
            Debug.Log("���� �����ʱ�ȭ");
            sound.Play();
            Debug.Log("���� �����÷���");
            //�Ҹ��� ª���� loop �ɾ��ֱ� 
            waterSprayParticleSystem.Stop();
            waterSprayParticleSystem.Play();

            waterSprayParticleSystem.transform.position = firePos.transform.position;
            waterSprayParticleSystem.transform.forward = firePos.transform.forward;

            Ray eraseRay = new Ray(firePos.transform.position, firePos.transform.forward);

            //RaycastHit raycastHit;
            //if (Physics.SphereCast(ray, brushRadius, out raycastHit, 0))  => SphereCast�� �ϸ� raycastHit�� ��ȯ��ǥ�� ���� �̻��� = ���������ϴµ� �������� ��ġ�� �̻��� 

            //���� ���̸� stain ��ũ���� �޸� �������� ���� ���
            // �� �� �� ������ �浹ü

            // ==> Raycast �� �ؾ�  raycastHit�� ��ȯ��ǥ�� ����� ���� => ����� ������ 
            if (Physics.Raycast(eraseRay, out RaycastHit raycastHit))
            {
                if (raycastHit.collider.GetComponent<Stain>())
                    raycastHit.collider.GetComponent<Stain>().EraseStain(raycastHit, dirtBrush);
                /*stain.EraseStain(raycastHit); */// ��� ���� waterGun.EraseStain �� ������ �������� �Լ�
                Debug.Log("�������� ����� ������ ! �浹!");
            }
        }
        if ((OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)&& transform.parent == RightHand)
            || (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) && transform.parent == LeftHand))
        //if(Input.GetMouseButtonUp(1))
        {
            waterSprayParticleSystem.Stop();
            sound.Stop();
        }

        //[[VR]] : ��ŧ���� ��Ʈ�ѷ���  INDEXTRIGGER ��ư�� ������
        //if (Input.GetMouseButtonDown(1)) : NON - VR
    }


    private void OnDrawGizmos() //OnDrawGizmos() => DrawGizmos �� ���õ� �Լ� �� �ֱ� !!!! 
    {
        //Ray Ray = new Ray(brush.transform.position, brush.transform.forward);
        Ray ray = new Ray(firePos.transform.position, firePos.transform.forward);


        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            float distance = Vector3.Distance(firePos.transform.position, hitInfo.transform.position);
            //distance ��� ���������� ����
            //-�ε��� ��������
           
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(firePos.transform.position, firePos.transform.forward * distance);
            //���̸� �׷��ش� (������,���������κ��� ����*�Ÿ�)

            //2-1�ε��� �������� ���̸� �׷��ش� (������ , �ε��� ����)

        }

        else
        //3.���� Ray�� �߻��ؼ� ���翡 �ε����� �ʾҴٸ�
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(firePos.transform.position, firePos.transform.forward * 1000f);
            //-Ray�� ���ָ����� ���.
        }


    }

}

 

