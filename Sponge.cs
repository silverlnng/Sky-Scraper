using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sponge : MonoBehaviour
{
   
    public GameObject brush;              // ���� ��� ����� ������ ������Ʈ
    public float spongeRadius = 1f;      // ������ ��� ����
    public ParticleSystem bubbleParticle;
    [SerializeField] private Texture2D dirtBrush;
    public Transform RightHand;
    public Transform LeftHand;

    public AudioSource audioSource;

    /*public Stain stain;  */       // ��� ����� �Լ� ���� => public ���� �����ؼ� �������ֱ� 

    void Start()
    {
       
        //enabled = false;
    }
    void Update()
    {
        //if (GameManager.Instance.isPlaying == false) { return; } //ui ��忡���� �۵��̾ȵǵ��� ����� 

        //if (Input.GetMouseButton(2))
        if ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) && transform.parent == RightHand) ||
            (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) && transform.parent == LeftHand))
        {
            // �������� ������Ʈ �귯������ ���� ����
            Ray eraseRay = new Ray(brush.transform.position, brush.transform.forward);
            RaycastHit raycastHit;

            //if (Physics.SphereCast(ray, brushRadius, out raycastHit, 0))  => SphereCast�� �ϸ� raycastHit�� ��ȯ��ǥ�� ���� �̻��� = ���������ϴµ� �������� ��ġ�� �̻��� 

            int layerMask = 1 << LayerMask.NameToLayer("Stain");
            // ==> Raycast �� �ؾ�  raycastHit�� ��ȯ��ǥ�� ����� ���� => ����� ������ 
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
             
                // ��� ����
                /* stain.EraseStain(raycastHit); */// waterGun.EraseStain �� ������ �������� �Լ�
                Debug.Log("�������� ����� ������ ! �浹!");

            }
        }
        //[[VR]] : ��ŧ���� ��Ʈ�ѷ���  INDEXTRIGGER ��ư�� ������
        //if (Input.GetMouseButtonDown(1)) : NON - VR
    }


    private void OnDrawGizmos() //OnDrawGizmos() => DrawGizmos �� ���õ� �Լ� �� �ֱ� !!!! 
    {
        Ray ray = new Ray(brush.transform.position, brush.transform.forward);
        //Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        Gizmos.color = Color.red;

        Gizmos.DrawRay(brush.transform.position, brush.transform.forward*spongeRadius);
       

    }

}
