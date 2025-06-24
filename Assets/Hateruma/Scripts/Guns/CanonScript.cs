using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : LiveGunOriginScript
{
    void Start()
    {
        //Cannon用にパラメーターを設定
        bulletAmount = 1;
        bulletSpeed = 10;
        fireRate = 1;
        fireRange = 500;
        reloadTime = 5;
        fireEnergyReq = 50;
        reloadEnergyReq = bulletAmount * fireEnergyReq / 3;

        //弾プレハブを装弾数×2個分用意
        unUsedBulletList = BulletInst(bulletAmount);
        usedBulletList = BulletInst(bulletAmount * 5);


        //弾のスクリプト取得
        foreach (var list in unUsedBulletList)
        {
            unUsedBulletSCList.Add(list.GetComponent<BulletScript>());
        }
        foreach (var list in usedBulletList)
        {
            usedBulletSCList.Add(list.GetComponent<BulletScript>());
        }
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            StartCoroutine(Fire());
        }
    }

}
