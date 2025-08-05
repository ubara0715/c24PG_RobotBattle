using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGunScript : LiveGunOriginScript
{
    void Start()
    {
        //ShotGun�p�Ƀp�����[�^�[��ݒ�
        bulletAmount = 50;
        bulletSpeed = 13;
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
        Vector3 angle = gunObj.transform.localEulerAngles;//�e�{�̂̉�]���擾

        //0�`360�ɂȂ��Ă���̂�-180�`180�ɂ���
        if (angle.y > 180)
        {
            angle.y = angle.y - 360;
        }
        if (angle.x > 180)
        {
            angle.x = angle.x - 360;
        }

        //X����Y���̉�]��45�x�͈̔͂Ő���
        angle.x = Mathf.Clamp(angle.x, -gunAngleLimit, gunAngleLimit);
        angle.y = Mathf.Clamp(angle.y, -gunAngleLimit, gunAngleLimit);

        gunObj.transform.localRotation = Quaternion.Euler(angle);//�������ꂽ�p�x������


        if (Input.GetButton("Fire1"))
        {
            StartCoroutine(Fire());
        }
    }
}
