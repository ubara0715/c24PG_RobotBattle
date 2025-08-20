using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMacineGunScript : LiveGunOriginScript
{
    void Start()
    {
        //SubMacineGun用にパラメーターを設定
        bulletAmount = 30;
        bulletSpeed = 7.5f;
        fireRate = 10;
        fireRange = 75;
        reloadTime = 1.5f;
        fireEnergyReq = 1;
        reloadEnergyReq = bulletAmount * fireEnergyReq / 3;
        gunWeight = 1;

        Preparation();
    }

    void Update()
    {
        AngleCheck();
    }

}
