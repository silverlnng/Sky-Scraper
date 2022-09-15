using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//오브젝트가 처음있던 자리를 벗어나면 제자리로 돌려놓기
//원래자리 
//얼마만큼 떨어져야 자리를 벗어난건지 체크하기
//돌려보낼 속도

public class ReturnPosition : MonoBehaviour
{
    public Transform originPos;
    public Transform originDir;
    //리프트의- 자식 -빈오브젝트 만들어서 위치 잡아주기

 



    public float returnDistance = 0.1f;

    public float returnSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
        //originPos = transform.position; //내가 원래있던(최초) 자리를 초기화 (할당)
        //originDir = transform.forward;  //내가 원래있던 (최초)각도를 초기화 (할당)

    }

// Update is called once per frame
void Update()
    {
     

        if (Vector3.Distance(originPos.position, transform.position) > returnDistance) //제자리에서 현재 떨어져있다면
        {
            transform.position = Vector3.Lerp(transform.position , originPos.position,returnSpeed*Time.deltaTime);
            transform.forward = Vector3.Lerp(transform.forward, originDir.forward, returnSpeed * Time.deltaTime);
        }

    }
}
