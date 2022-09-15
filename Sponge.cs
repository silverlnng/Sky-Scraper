using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sponge : MonoBehaviour
{
   
    public GameObject brush;              // 실제 얼룩 지우는 스펀지 오브젝트
    public float spongeRadius = 1f;      // 스펀지 닿는 범위
    public ParticleSystem bubbleParticle;
    [SerializeField] private Texture2D dirtBrush;
    public Transform RightHand;
    public Transform LeftHand;

    public AudioSource audioSource;

    /*public Stain stain;  */       // 얼룩 지우는 함수 관리 => public 으로 설정해서 연결해주기 

    void Start()
    {
       
        //enabled = false;
    }
    void Update()
    {
        //if (GameManager.Instance.isPlaying == false) { return; } //ui 모드에서는 작동이안되도록 만들기 

        //if (Input.GetMouseButton(2))
        if ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) && transform.parent == RightHand) ||
            (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) && transform.parent == LeftHand))
        {
            // 얼룩지우는 오브젝트 브러쉬에서 레이 생성
            Ray eraseRay = new Ray(brush.transform.position, brush.transform.forward);
            RaycastHit raycastHit;

            //if (Physics.SphereCast(ray, brushRadius, out raycastHit, 0))  => SphereCast로 하면 raycastHit의 반환좌표가 조금 이상함 = 지워지긴하는데 지워지는 위치가 이상함 

            int layerMask = 1 << LayerMask.NameToLayer("Stain");
            // ==> Raycast 로 해야  raycastHit의 반환좌표가 제대로 도출 => 제대로 지워짐 
            if (Physics.Raycast(eraseRay, out raycastHit, spongeRadius, layerMask))
            {
                bubbleParticle.Stop();
                bubbleParticle.Play();

                bubbleParticle.transform.position = brush.transform.position;
                bubbleParticle.transform.forward = brush.transform.forward;

                audioSource.Stop();
                audioSource.Play();

                if (raycastHit.collider.GetComponent<Stain>())
                    raycastHit.collider.GetComponent<Stain>().EraseStain(raycastHit, dirtBrush);
             
                // 얼룩 제거
                /* stain.EraseStain(raycastHit); */// waterGun.EraseStain 가 실제로 지워지는 함수
                Debug.Log("스폰지로 얼룩을 지우자 ! 충돌!");

            }
        }
        //[[VR]] : 오큘러스 컨트롤러의  INDEXTRIGGER 버튼을 누르면
        //if (Input.GetMouseButtonDown(1)) : NON - VR
    }


    private void OnDrawGizmos() //OnDrawGizmos() => DrawGizmos 와 관련된 함수 만 넣기 !!!! 
    {
        Ray ray = new Ray(brush.transform.position, brush.transform.forward);
        //Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        Gizmos.color = Color.red;

        Gizmos.DrawRay(brush.transform.position, brush.transform.forward*spongeRadius);
       

    }

}
