using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyScript : MonoBehaviour
{
    [SerializeField, Header("毎秒の増加量")]
    float incAmount = 1.0f;
    [SerializeField, Header("エネルギーの最大容量")]
    float maxAmount = 1000f;
    //現在のエネルギー量
    public float energyAmount = 0;

    void Start()
    {
        //初期エネルギーをマックスにする
        energyAmount = maxAmount;

        IncEnergy();
    }

    void Update()
    {

    }

    //一定時間ごとにエネルギー量を増やす
    void IncEnergy()
    {
        energyAmount += incAmount;
        
        if (energyAmount > maxAmount)//最大量を超えるとき
        {
            energyAmount = maxAmount;
        }
        
        Invoke("IncEnergy", 1);
    }

    //エネルギー消費関数
    public bool UseEnergy(float useAmount)//関数を呼ぶ側で消費量を指定
    {
        if (energyAmount >= useAmount)//必要エネルギーがあるとき
        {
            energyAmount -= useAmount;

            return true;
        }
        else //ないとき
        {
            return false;
        }
    }
}
