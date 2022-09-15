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
    private float catchDistance = 0f;  //총 잡는 범위의 거리




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 만약 마우스 왼쪽 버튼을 누르고 && 잡은 물체가 없는 경우

        //vr 전용으로 수정하기 
        //oculus controller 왼쪽 PrimaryHandTrigger 을 눌렀다 / 뗐다 

        //if (Input.GetButtonDown("Fire") && catchObj == null)
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) && catchObj == null)
        {
            CatchObject(); //총을 잡는다
        }


        //else if (Input.GetButtonDown("Fire") && catchObj != null) //잡은 물체가 있는 경우
        else if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) && catchObj != null)
        {
            DropObject(); //총을 놓는다
        }

        if (catchObj != null) // 잡은게 있다면 
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
        Ray ray = new Ray(leftHand.position, leftHand.forward); //레이를 생성 ( 위치 , 방향)

        int layerMask = 1 << LayerMask.NameToLayer("CleaningTool");

        //LayerMask.NameToLayer("Gun"); --> Gun 이란 이름을 가진 레이어의 숫자를 반환

        RaycastHit[] hitInfos = Physics.SphereCastAll(ray, catchRadius, catchDistance, layerMask);



        if (hitInfos != null && hitInfos.Length > 0) //부딪힌 경우가 있는 경우 실행
        {
            hitInfos[0].transform.parent = leftHand; // 부딪힌것을 Hand의 자식으로 ==부딪힌것의 부모를 Hand 로 설정

            //hitInfos[0].transform.position = leftHand.position;
            //hitInfos[0].transform.forward = leftHand.forward;

            /* hitInfos[0].transform.rotation = leftHand.rotation; *///손의 회전각도와 총의 회전각도 맞춰줌 

            //hitInfos[0].collider.GetComponent<Rigidbody>().isKinematic = true; // 물리연산 off

            catchObj = hitInfos[0].collider.gameObject; // 물체를 잡는다  catchObj에 넣어준다


            if (catchObj.GetComponentInChildren<ReturnPosition>() != null) //잡은것에 ReturnPosition 컴포넌트(스크립)이 있다면
            {
                catchObj.GetComponentInChildren<ReturnPosition>().enabled = false; //그 스크립 비활성화 ->총이 제자리로 돌아가는 스크립을 비활성화
            }

            //내가 잡은물체가 컴포넌트 ( GunFire) 가 있으면 
            if (catchObj.GetComponentInChildren<Sponge>() != null)
            {
                catchObj.GetComponentInChildren<Sponge>().enabled = true; //그스크립을 활성화
            }

            //leftHandModel.SetActive(false); //물체를 잡으면 HandModel 안보이도록

        }



    }

    void DropObject()
    {
        catchObj.transform.parent = null;
        //catchObj.GetComponent<Rigidbody>().isKinematic = false; //물리연산 작용 (o)
        //catchObj.GetComponent<Rigidbody>().useGravity = false; //중력적용 x

        //leftHandModel.SetActive(true);  //물체를 놓으면 HandModel 보이도록

        if (catchObj.GetComponentInChildren<ReturnPosition>() != null)
        {
            catchObj.GetComponentInChildren<ReturnPosition>().enabled = true;
        }

        if (catchObj.GetComponentInChildren<Sponge>() != null)
        {
            catchObj.GetComponentInChildren<Sponge>().enabled = false; //그스크립을 비활성화
        }

        catchObj = null; //순서중요 맨마지막 에 해야됨
    }

    public bool IsTakeGun()
    {
        //내가 현재 총(=catchObj) 이 있는지 를 확인하는 함수
        //있으면 true , 없으면 false

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
