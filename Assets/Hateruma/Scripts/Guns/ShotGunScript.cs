using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGunScript : LiveGunOriginScript
{
    void Start()
    {
        //ShotGun用にパラメーターを設定
        bulletAmount = 50;
        bulletSpeed = 6.5f;
        fireRate = 1;
        fireRange = 100;
        reloadTime = 3;
        fireEnergyReq = 7;
        reloadEnergyReq = (bulletAmount / 10) * fireEnergyReq / 3;

        isShotGun = true;

        Preparation();
    }

    void Update()
    {
        AngleCheck();
    }
}
