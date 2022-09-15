using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISystem : MonoBehaviour
{
    public static UISystem instance;

    //오염 제거를 퍼센트화 만들기
    private float dirtAmountTotal;                  // 전체 얼룩의 양
    private float removedDirtAmount;             // 전체에서 지워낸 현재 얼룩의 양

    public TextMeshProUGUI stainRemovePercent;
    public TextMeshProUGUI stainRemoveAmount;
    public Image stainRemoveImage;
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI downTimer;

    public TextMeshProUGUI isCurrentFloor;

    public Text clearStainRemovePoint; // // 최종 게임 클리어 할때 뜨는 포인트  UI

    public float RemovedDirtAmount
    {
        get { return removedDirtAmount; }
        set {
            removedDirtAmount = value;
            //stain 클래스  에서이미 누적된 값을 전달 받고 
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

        // 최종 게임 클리어 할때 뜨는 포인트  UI
        clearStainRemovePoint.text = $"{(int)removedDirtAmount} point";
    }

    // 내가 지워야할 얼룩값 총량을 누적하는 함수
    public void AddDirtAmount(float amountTotal)
    {
        dirtAmountTotal += amountTotal;
    }

    public void OneFloorTimer(int oneFloorTimer)
    {
        Timer.text =$"{oneFloorTimer.ToString() }" ;
        //카운트 다운 ui
        float downTime = LiftUp.instance.autoLiftTime - oneFloorTimer;
        downTimer.text = $"{downTime.ToString() }";
    }
    
    public void CurrentFloor(int currentFloor)
    {
        isCurrentFloor.text =$"{currentFloor.ToString()} ";
    }
}
