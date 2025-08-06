using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMacineGunScript : LiveGunOriginScript
{
    void Start()
    {
        //SubMacineGun用にパラメーターを設定
        bulletAmount = 30;
        bulletSpeed = 15;
        fireRate = 10;
        fireRange = 250;
        reloadTime = 1.5f;
        fireEnergyReq = 1;
        reloadEnergyReq = bulletAmount * fireEnergyReq / 3;

        Preparation();
    }

    void Update()
    {
        AngleCheck();
    }

}
