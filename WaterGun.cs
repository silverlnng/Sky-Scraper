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
    /*  public Stain stain;   */      // 얼룩 지우는 함수 관리 => public 으로 설정해서 연결해주기 

    private void Start()
    {
        //gameObject.SetActive(false);
        sound=GetComponent<AudioSource>();
    }

    void Update()
    {
        /* if (GameManager.Instance.isPlaying == false) { return; } *///ui 모드에서는 작동이안되도록 만들기 

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

            Debug.Log("물총 조건달성");
            sound.Stop();
            Debug.Log("물총 사운드초기화");
            sound.Play();
            Debug.Log("물총 사운드플레이");
            //소리가 짧으면 loop 걸어주기 
            waterSprayParticleSystem.Stop();
            waterSprayParticleSystem.Play();

            waterSprayParticleSystem.transform.position = firePos.transform.position;
            waterSprayParticleSystem.transform.forward = firePos.transform.forward;

            Ray eraseRay = new Ray(firePos.transform.position, firePos.transform.forward);

            //RaycastHit raycastHit;
            //if (Physics.SphereCast(ray, brushRadius, out raycastHit, 0))  => SphereCast로 하면 raycastHit의 반환좌표가 조금 이상함 = 지워지긴하는데 지워지는 위치가 이상함 

            //내가 레이를 stain 스크립이 달린 오브젯을 향해 쏜다
            // 내 가 쏜 레이의 충돌체

            // ==> Raycast 로 해야  raycastHit의 반환좌표가 제대로 도출 => 제대로 지워짐 
            if (Physics.Raycast(eraseRay, out RaycastHit raycastHit))
            {
                if (raycastHit.collider.GetComponent<Stain>())
                    raycastHit.collider.GetComponent<Stain>().EraseStain(raycastHit, dirtBrush);
                /*stain.EraseStain(raycastHit); */// 얼룩 제거 waterGun.EraseStain 가 실제로 지워지는 함수
                Debug.Log("물총으로 얼룩을 지우자 ! 충돌!");
            }
        }
        if ((OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)&& transform.parent == RightHand)
            || (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) && transform.parent == LeftHand))
        //if(Input.GetMouseButtonUp(1))
        {
            waterSprayParticleSystem.Stop();
            sound.Stop();
        }

        //[[VR]] : 오큘러스 컨트롤러의  INDEXTRIGGER 버튼을 누르면
        //if (Input.GetMouseButtonDown(1)) : NON - VR
    }


    private void OnDrawGizmos() //OnDrawGizmos() => DrawGizmos 와 관련된 함수 만 넣기 !!!! 
    {
        //Ray Ray = new Ray(brush.transform.position, brush.transform.forward);
        Ray ray = new Ray(firePos.transform.position, firePos.transform.forward);


        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            float distance = Vector3.Distance(firePos.transform.position, hitInfo.transform.position);
            //distance 라는 지역변수로 설정
            //-부딪힌 지점까지
           
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(firePos.transform.position, firePos.transform.forward * distance);
            //레이를 그려준다 (시작점,시작점으로부터 방향*거리)

            //2-1부딪힌 지점까지 레이를 그려준다 (시작점 , 부딪힌 지점)

        }

        else
        //3.만일 Ray를 발사해서 뭔사에 부딪히지 않았다면
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(firePos.transform.position, firePos.transform.forward * 1000f);
            //-Ray를 저멀리까지 쏜다.
        }


    }

}

 

