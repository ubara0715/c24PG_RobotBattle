using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : LiveGunOriginScript
{
    void Start()
    {
        //Cannon�p�Ƀp�����[�^�[��ݒ�
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
