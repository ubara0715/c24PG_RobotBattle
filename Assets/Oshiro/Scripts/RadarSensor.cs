using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSensorObj
{

    protected GameObject targetObj = null;//�^�[�Q�b�g�̃I�u�W�F�N�g

    public RadarSensorObj(GameObject lockOnObj)
    {
        this.targetObj = lockOnObj;
    }

    /// <summary>
    /// �^�[�Q�b�g�̈ʒu���擾
    /// </summary>
    /// <returns>�^�[�Q�b�g�̈ʒu</returns>
    public Vector3 GetPos()
    {
        return targetObj.transform.position;
    }

    /// <summary>
    /// �@�̂��ǂ����𒲂ׂ�
    /// </summary>
    /// <returns>�@�̂ł����true</returns>
    public bool IsEnemy()
    {
        return targetObj.CompareTag("Player");
    }

    /// <summary>
    /// �e���ǂ����𒲂ׂ�
    /// </summary>
    /// <returns>�e�ł����true</returns>
    public bool IsBullet()
    {
        List<string> tags = new() { "Bullet", "Energy", "Missile" };
        return tags.Contains(targetObj.tag);
    }

    /// <summary>
    /// �^�[�Q�b�g�̃^�O���擾
    /// </summary>
    /// <returns>�^�[�Q�b�g�̃^�O</returns>
    public string GetTag()
    {
        return targetObj.tag;
    }

    /// <summary>
    /// �^�[�Q�b�g�I�u�W�F�N�g�̔�r
    /// </summary>
    /// <param name="lossObj">��r�Ώ�</param>
    /// <returns>�^�[�Q�b�g�ƈ�������v����Ȃ�true</returns>
    public bool EqualGameObj(GameObject lossObj)
    {
        return targetObj == lossObj;
    }
}

public class RadarSensor : MonoBehaviour
{

    [Header("�Z���T�[�̑傫��"), Range(40, 400)]
    public int sensorSize = 40;

    private List<RadarSensorObj> targets = new();

    public CoreScript coreScript;

    public List<string> tags = new();

    public EnergyScript energyScript;

    private List<RadarSensorObj> dummyTargets = new();

    private bool isEnergy = true;

    private bool isReset = true;

    private void Awake()
    {
        transform.localScale = Vector3.one * sensorSize;//�T�C�Y��ݒ�
        transform.localPosition = Vector3.zero;//�|�W�V������������
    }

    private void Update()
    {
        SensorEnergy();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        //�ݒ肵���^�O�ɃI�u�W�F�N�g���܂܂�Ă�����
        if (tags.Contains(other.gameObject.tag) && transform.parent.gameObject != other.gameObject)
        {
            //���g�̒e�ۂ��ǂ���
            RadarSensorObj sampleObj = new RadarSensorObj(other.gameObject);
            if (sampleObj.IsBullet())
            {
                if (IsMyBullet(other.gameObject)) return;
            }
            //�^�[�Q�b�g���X�g�Ɋ܂܂�A�{�̂̃X�N���v�g�ɓ`����
            targets.Add(sampleObj);
            if (isEnergy)
            {
                coreScript.OnRadarSensor(targets, isEnter: true);
            }
            else
            {
                coreScript.OnRadarSensor(dummyTargets, isEnter: true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        RadarSensorObj sampleObj = new RadarSensorObj(other.gameObject);
        if (sampleObj.IsBullet())
        {
            if (IsMyBullet(other.gameObject)) return;
        }

        //�ݒ肵���^�O�ɃI�u�W�F�N�g���܂܂�Ă�����
        if (tags.Contains(other.gameObject.tag) && transform.parent.gameObject != other.gameObject)
        {

            //�^�[�Q�b�g���X�g���珜�O���A�{�̂̃X�N���v�g�ɓ`����
            targets.RemoveAt(targets.FindIndex(x => x.EqualGameObj(other.gameObject)));
            if (isEnergy)
            {
                coreScript.OnRadarSensor(targets, isEnter: false);
            }
            else
            {
                coreScript.OnRadarSensor(dummyTargets, isEnter: false);
            }
        }


    }


    /// <summary>
    /// ���g�̒e�ۂ��ǂ����𒲂ׂ�
    /// </summary>
    /// <param name="otherObj">�ڐG�����I�u�W�F�N�g</param>
    /// <returns>���g�̒e�ۂ��ǂ���</returns>
    private bool IsMyBullet(GameObject otherObj)
    {
        if (otherObj.CompareTag("Bullet"))
        {
            string sampleName = "";
            if (otherObj.TryGetComponent<BulletScript>(out BulletScript bullet))
            {
                //sampleName = bullet.masterName;
            }
            else if (otherObj.TryGetComponent<EnergyBulletScript>(out EnergyBulletScript energy))
            {
                //sampleName = energy.masterName;
            }
            else if (otherObj.TryGetComponent<MissileBulletSc>(out MissileBulletSc missile))
            {
                //sampleName = missile.masterName;
            }
            if (coreScript.playerName == sampleName)
            {
                return true;
            }
        }
        return false;
    }

    private void SensorEnergy()
    {
        if (energyScript.UseEnergy(sensorSize/40f*Time.deltaTime))
        {
            isEnergy = true;
            isReset = true;
        }
        else
        {
            isEnergy = false;
            if (isReset)
            {
                coreScript.OnRadarSensor(dummyTargets, isEnter: false);
                isReset = false;
            }
        }
    }

}
