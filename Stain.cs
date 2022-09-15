using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stain : MonoBehaviour
{

    [SerializeField] private Texture2D dirtMaskTextureBase;   //초록색 uv
    [SerializeField] private Texture2D dirtBrush;
    [SerializeField] private Material material;

    private Texture2D dirtMaskTexture;
    private Vector2Int lastPaintPixelPosition;

    private float dirtAmountTotal;       // 현재 층의 얼룩의 양
    private float removedDirtAmount;  // 현재 층에서 지운 얼룩의 양
    float PlusRemovedAmount = 0;     // 현재 층에서 지운 얼룩의 총량
    float StainRemovePercent = 0;

    public TextMeshProUGUI oneFloorStainRemovePercent;
    //public Image oneFloorStainRemovePercentImage;
    //public TextMeshProUGUI levelStainRemoveClear;

    //public bool iscleared = true;
    public bool dropFloor = true;

    AudioSource audioSource;
    //public AudioClip clearSuccess;
    

    private void Awake()
    {

        dirtMaskTexture = new Texture2D(dirtMaskTextureBase.width, dirtMaskTextureBase.height);

        dirtMaskTexture.SetPixels(dirtMaskTextureBase.GetPixels());
        // 초록색 uv의 SetPixels  :특정 픽셀의 색 추출
        // GetPixel(int x, int y)  ( x,y) 의 컬러 반환 

        dirtMaskTexture.Apply();

        //material.SetTexture("_DirtMask", dirtMaskTexture);
        GetComponent<MeshRenderer>().material.SetTexture("_DirtMask", dirtMaskTexture);
        //GetComponent<MeshRenderer>().material. : 으로 접근 하면 원본 material 에 접근하는게 아니라 material 의 instance 에 접근
        // 각각의 오브젯이 동일한  material 을 사용한다고 해도 사실은 모두 각각의  material  의 instance를 사용하는 것 

        //SetTexture ( propertyname , texture)  => 해당 material에 프로퍼티로 속해있는 텍스처를 지정한  texture 으로 변경 
        // SetTexture 텍스처를 변경 => _DirtMask 변수에 설정된 텍스처를 dirtMaskTexture  텍스처로 변경 
        //그래서 플레이하면 없다가 초록색 uv 가 들어가는것 

        oneFloorStainRemovePercent.text = "%";
        //oneFloorStainRemovePercentImage.fillAmount = 0;
        //levelStainRemoveClear.text = "";
        audioSource=GetComponent<AudioSource>();    
    }


    private void Start()
    {
        GetTotalDirtAmount();
    }



    void Update()
    {
        // 게임이 일시정지 상태라면.. 얼룩지우지 못하게 함..(=UI모드)
        //if(GameManager.Instance.isPlaying == false) { return; }

        // 1. 마우스 버튼을 클릭하면
        if (Input.GetMouseButton(0))
        {
            // 2. 마우스 버튼을 클릭했을 때의 마우스 위치로 Screen에서 Ray 발사
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
            {
                //★★내가 쏜 레이의 raycastHit 충돌체가 오직 이스크립이 달린 오브젯(나자신) 일 때만 적용 
                if (raycastHit.collider.gameObject.Equals(gameObject))
                {
                    // 해당 스크린에서 보여지는 오브젝트의 위치에 있는 얼룩 제거
                    EraseStain(raycastHit, dirtBrush);
                }
            }
        }
    }


    // 텍스쳐의 얼룩을 지워주는 함수
    public void EraseStain(RaycastHit raycastHit, Texture2D dirtBrush)
    {
        Vector2 textureCoord = raycastHit.textureCoord;
        //.textureCoord  =>충돌위치에서의 uv texture 좌표 

        int pixelX = (int)(textureCoord.x * dirtMaskTexture.width);
        int pixelY = (int)(textureCoord.y * dirtMaskTexture.height);

        // 상대적인 좌표에 곱해져서 제대로된 좌표값을 구하는것 


        Vector2Int paintPixelPosition = new Vector2Int(pixelX, pixelY);
        //Debug.Log("UV: " + textureCoord + "; Pixels: " + paintPixelPosition);


        // 방어 코드 
        int paintPixelDistance = Mathf.Abs(paintPixelPosition.x - lastPaintPixelPosition.x) + Mathf.Abs(paintPixelPosition.y - lastPaintPixelPosition.y);
        int maxPaintDistance = 7;
        if (paintPixelDistance < maxPaintDistance)
        {
            // Painting too close to last position
            return;
        }
        lastPaintPixelPosition = paintPixelPosition;


        //* 
        int pixelXOffset = pixelX - (dirtBrush.width / 2);
        int pixelYOffset = pixelY - (dirtBrush.height / 2);

        for (int x = 0; x < dirtBrush.width; x++)
        {
            for (int y = 0; y < dirtBrush.height; y++)
            {
                Color pixelDirt = dirtBrush.GetPixel(x, y);

                Color pixelDirtMask = dirtMaskTexture.GetPixel(pixelXOffset + x, pixelYOffset + y);

                removedDirtAmount = pixelDirtMask.g - (pixelDirtMask.g * pixelDirt.g);
                //지워지는 양 : 초록색 uv -( 초록색 uv * 브러쉬의 검정색 부분의  g 값이 0 ) 
                UISystem.instance.RemovedDirtAmount += removedDirtAmount;
                //UISystem 클래스의 DirtAmount 함수 value 에 ***누적해서*** 값을 전달


                dirtMaskTexture.SetPixel(
                    pixelXOffset + x,
                    pixelYOffset + y,
                    new Color(0, pixelDirtMask.g * pixelDirt.g, 0)
                );
                //SetPixel (int x, int y, Color color)  => (x,y) 컬러를 변경
                //마우스클릭=> 초록색 uv 가 검정색으로 변화 => 쉐이더  multiply으로 검정은 0값 , 그래서 안보이게됨 




                PlusRemovedAmount += removedDirtAmount;
                //얼룩1개 에 대해서 계산하기 (전체가 아니라 ) 
                //이 스크립이 달린 객체에 대해서만 접근 !
                StainRemovePercent = PlusRemovedAmount / dirtAmountTotal * 100;

                if (StainRemovePercent >= 40f && dropFloor)
                {
                    Debug.Log(" 얼룩 1개의 90% 클리어 " + StainRemovePercent);
                    //levelStainRemoveClear.text = "clear";     
                    oneFloorStainRemovePercent.text = "clear";
                    audioSource.Play();
                    dropFloor = false;
                    /* iscleared = false;*/ //false 가 90%이상 클리어의미 
                }
                else if (StainRemovePercent < 40f)
                {
                    //Debug.Log("else" + StainRemovePercent);
                    oneFloorStainRemovePercent.text = $" {(int)StainRemovePercent} % ";
                    //oneFloorStainRemovePercentImage.fillAmount=StainRemovePercent;
                }

                //if (PlusRemovedAmount >= dirtAmountTotal * 0.5f)
                //{ Debug.Log(" 얼룩 1개의 50% 클리어 "); }
            }
        }
        dirtMaskTexture.Apply();
    }


    void GetTotalDirtAmount()
    {
        // 1. 내가 지워야할 얼룩의 총량 구하기
        dirtAmountTotal = 0f;
        for (int x = 0; x < dirtMaskTexture.width; x++)
        {
            for (int y = 0; y < dirtMaskTexture.height; y++)
            {
                Color dirtMaskTextureBase = dirtMaskTexture.GetPixel(x, y);

                dirtAmountTotal += dirtMaskTextureBase.g;
                //초록색만 얼룩이니까  총 얼룩 넓이를   컬러중에 초록색만 누적해서 더해서 구하기
                //Debug.Log("오염의 총넓이구하기");
                // 2. 구한 총량을 UISystem에 전달


            }
            
        }
        UISystem.instance.AddDirtAmount(dirtAmountTotal);
    }

}
