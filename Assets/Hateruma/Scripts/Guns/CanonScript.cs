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

        Preparation();
    }

    void Update()
    {
        Vector3 angle = gunObj.transform.localEulerAngles;//銃本体の回転を取得

        //0～360になっているのを-180～180にする
        if (angle.y > 180)
        {
            angle.y = angle.y - 360;
        }
        if (angle.x > 180)
        {
            angle.x = angle.x - 360;
        }

        //X軸とY軸の回転を45度の範囲で制限
        angle.x = Mathf.Clamp(angle.x, -gunAngleLimit, gunAngleLimit);
        angle.y = Mathf.Clamp(angle.y, -gunAngleLimit, gunAngleLimit);

        gunObj.transform.localRotation = Quaternion.Euler(angle);//制限された角度を入れる


        if (Input.GetButton("Fire1"))
        {
            StartCoroutine(Fire());
        }
    }

}
