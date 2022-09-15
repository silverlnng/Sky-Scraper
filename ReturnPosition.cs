using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������Ʈ�� ó���ִ� �ڸ��� ����� ���ڸ��� ��������
//�����ڸ� 
//�󸶸�ŭ �������� �ڸ��� ������� üũ�ϱ�
//�������� �ӵ�

public class ReturnPosition : MonoBehaviour
{
    public Transform originPos;
    public Transform originDir;
    //����Ʈ��- �ڽ� -�������Ʈ ���� ��ġ ����ֱ�

 



    public float returnDistance = 0.1f;

    public float returnSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
        //originPos = transform.position; //���� �����ִ�(����) �ڸ��� �ʱ�ȭ (�Ҵ�)
        //originDir = transform.forward;  //���� �����ִ� (����)������ �ʱ�ȭ (�Ҵ�)

    }

// Update is called once per frame
void Update()
    {
     

        if (Vector3.Distance(originPos.position, transform.position) > returnDistance) //���ڸ����� ���� �������ִٸ�
        {
            transform.position = Vector3.Lerp(transform.position , originPos.position,returnSpeed*Time.deltaTime);
            transform.forward = Vector3.Lerp(transform.forward, originDir.forward, returnSpeed * Time.deltaTime);
        }

    }
}
