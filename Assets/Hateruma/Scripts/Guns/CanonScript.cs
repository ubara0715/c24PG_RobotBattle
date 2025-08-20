using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : LiveGunOriginScript
{
    void Start()
    {
        //Cannon用にパラメーターを設定
        bulletAmount = 1;
        bulletSpeed = 5;
        fireRate = 1;
        fireRange = 150;
        reloadTime = 5;
        fireEnergyReq = 50;
        reloadEnergyReq = bulletAmount * fireEnergyReq / 3;
        gunWeight = 3;

        Preparation();
    }

    void Update()
    {
        AngleCheck();
    }

}
