using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class MacineGunScript : LiveGunOriginScript
{
    void Start()
    {
        //MacineGun用にパラメーターを設定
        bulletAmount = 30;
        bulletSpeed = 7.5f;
        fireRate = 6;
        fireRange = 100;
        reloadTime = 2;
        fireEnergyReq = 2;
        reloadEnergyReq = bulletAmount * fireEnergyReq / 3;
        gunWeight = 1;

        Preparation();
    }

    void Update()
    {
        AngleCheck();
    }
}
