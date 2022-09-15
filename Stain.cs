using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stain : MonoBehaviour
{

    [SerializeField] private Texture2D dirtMaskTextureBase;   //�ʷϻ� uv
    [SerializeField] private Texture2D dirtBrush;
    [SerializeField] private Material material;

    private Texture2D dirtMaskTexture;
    private Vector2Int lastPaintPixelPosition;

    private float dirtAmountTotal;       // ���� ���� ����� ��
    private float removedDirtAmount;  // ���� ������ ���� ����� ��
    float PlusRemovedAmount = 0;     // ���� ������ ���� ����� �ѷ�
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
        // �ʷϻ� uv�� SetPixels  :Ư�� �ȼ��� �� ����
        // GetPixel(int x, int y)  ( x,y) �� �÷� ��ȯ 

        dirtMaskTexture.Apply();

        //material.SetTexture("_DirtMask", dirtMaskTexture);
        GetComponent<MeshRenderer>().material.SetTexture("_DirtMask", dirtMaskTexture);
        //GetComponent<MeshRenderer>().material. : ���� ���� �ϸ� ���� material �� �����ϴ°� �ƴ϶� material �� instance �� ����
        // ������ �������� ������  material �� ����Ѵٰ� �ص� ����� ��� ������  material  �� instance�� ����ϴ� �� 

        //SetTexture ( propertyname , texture)  => �ش� material�� ������Ƽ�� �����ִ� �ؽ�ó�� ������  texture ���� ���� 
        // SetTexture �ؽ�ó�� ���� => _DirtMask ������ ������ �ؽ�ó�� dirtMaskTexture  �ؽ�ó�� ���� 
        //�׷��� �÷����ϸ� ���ٰ� �ʷϻ� uv �� ���°� 

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
        // ������ �Ͻ����� ���¶��.. ��������� ���ϰ� ��..(=UI���)
        //if(GameManager.Instance.isPlaying == false) { return; }

        // 1. ���콺 ��ư�� Ŭ���ϸ�
        if (Input.GetMouseButton(0))
        {
            // 2. ���콺 ��ư�� Ŭ������ ���� ���콺 ��ġ�� Screen���� Ray �߻�
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
            {
                //�ڡڳ��� �� ������ raycastHit �浹ü�� ���� �̽�ũ���� �޸� ������(���ڽ�) �� ���� ���� 
                if (raycastHit.collider.gameObject.Equals(gameObject))
                {
                    // �ش� ��ũ������ �������� ������Ʈ�� ��ġ�� �ִ� ��� ����
                    EraseStain(raycastHit, dirtBrush);
                }
            }
        }
    }


    // �ؽ����� ����� �����ִ� �Լ�
    public void EraseStain(RaycastHit raycastHit, Texture2D dirtBrush)
    {
        Vector2 textureCoord = raycastHit.textureCoord;
        //.textureCoord  =>�浹��ġ������ uv texture ��ǥ 

        int pixelX = (int)(textureCoord.x * dirtMaskTexture.width);
        int pixelY = (int)(textureCoord.y * dirtMaskTexture.height);

        // ������� ��ǥ�� �������� ����ε� ��ǥ���� ���ϴ°� 


        Vector2Int paintPixelPosition = new Vector2Int(pixelX, pixelY);
        //Debug.Log("UV: " + textureCoord + "; Pixels: " + paintPixelPosition);


        // ��� �ڵ� 
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
                //�������� �� : �ʷϻ� uv -( �ʷϻ� uv * �귯���� ������ �κ���  g ���� 0 ) 
                UISystem.instance.RemovedDirtAmount += removedDirtAmount;
                //UISystem Ŭ������ DirtAmount �Լ� value �� ***�����ؼ�*** ���� ����


                dirtMaskTexture.SetPixel(
                    pixelXOffset + x,
                    pixelYOffset + y,
                    new Color(0, pixelDirtMask.g * pixelDirt.g, 0)
                );
                //SetPixel (int x, int y, Color color)  => (x,y) �÷��� ����
                //���콺Ŭ��=> �ʷϻ� uv �� ���������� ��ȭ => ���̴�  multiply���� ������ 0�� , �׷��� �Ⱥ��̰Ե� 




                PlusRemovedAmount += removedDirtAmount;
                //���1�� �� ���ؼ� ����ϱ� (��ü�� �ƴ϶� ) 
                //�� ��ũ���� �޸� ��ü�� ���ؼ��� ���� !
                StainRemovePercent = PlusRemovedAmount / dirtAmountTotal * 100;

                if (StainRemovePercent >= 40f && dropFloor)
                {
                    Debug.Log(" ��� 1���� 90% Ŭ���� " + StainRemovePercent);
                    //levelStainRemoveClear.text = "clear";     
                    oneFloorStainRemovePercent.text = "clear";
                    audioSource.Play();
                    dropFloor = false;
                    /* iscleared = false;*/ //false �� 90%�̻� Ŭ�����ǹ� 
                }
                else if (StainRemovePercent < 40f)
                {
                    //Debug.Log("else" + StainRemovePercent);
                    oneFloorStainRemovePercent.text = $" {(int)StainRemovePercent} % ";
                    //oneFloorStainRemovePercentImage.fillAmount=StainRemovePercent;
                }

                //if (PlusRemovedAmount >= dirtAmountTotal * 0.5f)
                //{ Debug.Log(" ��� 1���� 50% Ŭ���� "); }
            }
        }
        dirtMaskTexture.Apply();
    }


    void GetTotalDirtAmount()
    {
        // 1. ���� �������� ����� �ѷ� ���ϱ�
        dirtAmountTotal = 0f;
        for (int x = 0; x < dirtMaskTexture.width; x++)
        {
            for (int y = 0; y < dirtMaskTexture.height; y++)
            {
                Color dirtMaskTextureBase = dirtMaskTexture.GetPixel(x, y);

                dirtAmountTotal += dirtMaskTextureBase.g;
                //�ʷϻ��� ����̴ϱ�  �� ��� ���̸�   �÷��߿� �ʷϻ��� �����ؼ� ���ؼ� ���ϱ�
                //Debug.Log("������ �ѳ��̱��ϱ�");
                // 2. ���� �ѷ��� UISystem�� ����


            }
            
        }
        UISystem.instance.AddDirtAmount(dirtAmountTotal);
    }

}
