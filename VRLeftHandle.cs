using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRLeftHandle : MonoBehaviour
{
    //public GameObject leftHandModel;
    public Transform leftHand;
    GameObject catchObj;

    public float catchSpeed = 7f;
    public float catchRadius = 2.5f;
    private float catchDistance = 0f;  //�� ��� ������ �Ÿ�




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // ���� ���콺 ���� ��ư�� ������ && ���� ��ü�� ���� ���

        //vr �������� �����ϱ� 
        //oculus controller ���� PrimaryHandTrigger �� ������ / �ô� 

        //if (Input.GetButtonDown("Fire") && catchObj == null)
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) && catchObj == null)
        {
            CatchObject(); //���� ��´�
        }


        //else if (Input.GetButtonDown("Fire") && catchObj != null) //���� ��ü�� �ִ� ���
        else if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) && catchObj != null)
        {
            DropObject(); //���� ���´�
        }

        if (catchObj != null) // ������ �ִٸ� 
        {
            catchObj.transform.position = Vector3.Lerp(catchObj.transform.position, leftHand.position, catchSpeed * Time.deltaTime);

            //catchObj.transform.forward = Vector3.Lerp(catchObj.transform.forward ,leftHand.transform.forward, catchSpeed * Time.deltaTime);



        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(leftHand.position, catchRadius);

    }

    void CatchObject()
    {
        Ray ray = new Ray(leftHand.position, leftHand.forward); //���̸� ���� ( ��ġ , ����)

        int layerMask = 1 << LayerMask.NameToLayer("CleaningTool");

        //LayerMask.NameToLayer("Gun"); --> Gun �̶� �̸��� ���� ���̾��� ���ڸ� ��ȯ

        RaycastHit[] hitInfos = Physics.SphereCastAll(ray, catchRadius, catchDistance, layerMask);



        if (hitInfos != null && hitInfos.Length > 0) //�ε��� ��찡 �ִ� ��� ����
        {
            hitInfos[0].transform.parent = leftHand; // �ε������� Hand�� �ڽ����� ==�ε������� �θ� Hand �� ����

            //hitInfos[0].transform.position = leftHand.position;
            //hitInfos[0].transform.forward = leftHand.forward;

            /* hitInfos[0].transform.rotation = leftHand.rotation; *///���� ȸ�������� ���� ȸ������ ������ 

            //hitInfos[0].collider.GetComponent<Rigidbody>().isKinematic = true; // �������� off

            catchObj = hitInfos[0].collider.gameObject; // ��ü�� ��´�  catchObj�� �־��ش�


            if (catchObj.GetComponentInChildren<ReturnPosition>() != null) //�����Ϳ� ReturnPosition ������Ʈ(��ũ��)�� �ִٸ�
            {
                catchObj.GetComponentInChildren<ReturnPosition>().enabled = false; //�� ��ũ�� ��Ȱ��ȭ ->���� ���ڸ��� ���ư��� ��ũ���� ��Ȱ��ȭ
            }

            //���� ������ü�� ������Ʈ ( GunFire) �� ������ 
            if (catchObj.GetComponentInChildren<Sponge>() != null)
            {
                catchObj.GetComponentInChildren<Sponge>().enabled = true; //�׽�ũ���� Ȱ��ȭ
            }

            //leftHandModel.SetActive(false); //��ü�� ������ HandModel �Ⱥ��̵���

        }



    }

    void DropObject()
    {
        catchObj.transform.parent = null;
        //catchObj.GetComponent<Rigidbody>().isKinematic = false; //�������� �ۿ� (o)
        //catchObj.GetComponent<Rigidbody>().useGravity = false; //�߷����� x

        //leftHandModel.SetActive(true);  //��ü�� ������ HandModel ���̵���

        if (catchObj.GetComponentInChildren<ReturnPosition>() != null)
        {
            catchObj.GetComponentInChildren<ReturnPosition>().enabled = true;
        }

        if (catchObj.GetComponentInChildren<Sponge>() != null)
        {
            catchObj.GetComponentInChildren<Sponge>().enabled = false; //�׽�ũ���� ��Ȱ��ȭ
        }

        catchObj = null; //�����߿� �Ǹ����� �� �ؾߵ�
    }

    public bool IsTakeGun()
    {
        //���� ���� ��(=catchObj) �� �ִ��� �� Ȯ���ϴ� �Լ�
        //������ true , ������ false

        if (catchObj != null)
        {
            return true;
        }
        else
        {
            return false;
        }


    }
}
