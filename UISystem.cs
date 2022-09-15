using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISystem : MonoBehaviour
{
    public static UISystem instance;

    //���� ���Ÿ� �ۼ�Ʈȭ �����
    private float dirtAmountTotal;                  // ��ü ����� ��
    private float removedDirtAmount;             // ��ü���� ������ ���� ����� ��

    public TextMeshProUGUI stainRemovePercent;
    public TextMeshProUGUI stainRemoveAmount;
    public Image stainRemoveImage;
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI downTimer;

    public TextMeshProUGUI isCurrentFloor;

    public Text clearStainRemovePoint; // // ���� ���� Ŭ���� �Ҷ� �ߴ� ����Ʈ  UI

    public float RemovedDirtAmount
    {
        get { return removedDirtAmount; }
        set {
            removedDirtAmount = value;
            //stain Ŭ����  �����̹� ������ ���� ���� �ް� 
            StainCalculation();
        }
    }

    private void Awake()
    {
        if (instance == null) { instance = this; }
       
    }

    public void StainCalculation()
    {

        float dirtRemovePercent = removedDirtAmount / dirtAmountTotal * 100;
        stainRemovePercent.text = $"{(int)dirtRemovePercent} %";
        stainRemoveImage.fillAmount = dirtRemovePercent / 100;

        stainRemoveAmount.text = $"{(int)removedDirtAmount}";

        // ���� ���� Ŭ���� �Ҷ� �ߴ� ����Ʈ  UI
        clearStainRemovePoint.text = $"{(int)removedDirtAmount} point";
    }

    // ���� �������� ��谪 �ѷ��� �����ϴ� �Լ�
    public void AddDirtAmount(float amountTotal)
    {
        dirtAmountTotal += amountTotal;
    }

    public void OneFloorTimer(int oneFloorTimer)
    {
        Timer.text =$"{oneFloorTimer.ToString() }" ;
        //ī��Ʈ �ٿ� ui
        float downTime = LiftUp.instance.autoLiftTime - oneFloorTimer;
        downTimer.text = $"{downTime.ToString() }";
    }
    
    public void CurrentFloor(int currentFloor)
    {
        isCurrentFloor.text =$"{currentFloor.ToString()} ";
    }
}
