using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OpSensorObj : RadarSensorObj
{
    public OpSensorObj(GameObject lockOnObj) : base(lockOnObj) { }

    /// <summary>
    /// �^�[�Q�b�g�Ƃ̋����̑傫�����߂�
    /// </summary>
    /// <param name="myTf">���g�̃g�����X�t�H�[��</param>
    /// <param name="isSqr">Sqr�̃}�O�j�`���[�h�ɂ��邩</param>
    /// <returns>�^�[�Q�b�g�Ƃ̋����̑傫��</returns>
    public float GetDistance(Transform myTf, bool isSqr = false)
    {
        Vector3 diffvec = GetVector(myTf);

        if (isSqr)
        {
            return diffvec.sqrMagnitude;
        }
        else
        {
            return diffvec.magnitude;
        }
    }

    /// <summary>
    /// �^�[�Q�b�g���������������Ă��邩�ǂ���
    /// </summary>
    /// <param name="myTf">�����̃g�����X�t�H�[��</param>
    /// <param name="targetOutLookAngle"></param>
    /// <returns></returns>
    public bool IsLooking(Transform myTf, float targetOutLookAngle = 90)
    {
        const float MAX_ANGLE = 180;//�^�[�Q�b�g�̎���̍ő�l
        targetOutLookAngle = Mathf.Min(MAX_ANGLE, targetOutLookAngle);//�ő�l���傫��������ő�l�ɂ���

        Vector3 targetForward = -targetObj.transform.forward;//�^�[�Q�b�g�̔w��̕���
        Vector3 toTargetVec = GetVector(myTf);//��������^�[�Q�b�g�ւ̕���

        float vecDiff = Vector3.Angle(targetForward, toTargetVec);//�O�q�̓�̕����̊p�x��
        //�p�x���ƃ^�[�Q�b�g�̎�����r����
        return vecDiff * 2 <= targetOutLookAngle;
    }

    /// <summary>
    /// �^�[�Q�b�g�Ǝ����̋����𑪂�֐�
    /// </summary>
    /// <param name="myTf"></param>
    /// <returns>�^�[�Q�b�g�Ǝ����̋���</returns>
    public Vector3 GetVector(Transform myTf)
    {
        Vector3 diffvec = targetObj.transform.position - myTf.position;

        return diffvec;
    }

    public GameObject TargetOBj()
    {
        return targetObj;
    }
}

public class OpticalSensor : MonoBehaviour
{
    [Header("����p"), Range(1, 180)]
    public float angle = 90;

    [Header("�Z���T�[�̑傫��"), Range(40, 400)]
    public float sensorSize = 50;

    public CoreScript coreScript;

    private Transform coreTf;

    public List<OpSensorObj> targets = new();

    public List<string> tags = new();

    public EnergyScript energyScript;

    private List<OpSensorObj> dummyTargets = new();

    private bool isEnergy = true;

    private bool isReset = true;

    private void Awake()
    {
        coreTf = transform.parent.GetComponent<Transform>();//�{�̂̃g�����X�t�H�[�����擾
        transform.localScale = Vector3.one * sensorSize;//�T�C�Y��ݒ�
        transform.localPosition = Vector3.zero;//�|�W�V������������

    }

    private void Update()
    {
        SensorEnergy();

        dummyTargets = new(targets);
        foreach (OpSensorObj target in dummyTargets)
        {
            if (target.IsBullet() && !target.IsActive())
            {
                targets.Remove(target);
            }
        }
        dummyTargets.Clear();
    }

    /// <summary>
    /// ��ԋ߂��^�[�Q�b�g���w�肷��
    /// </summary>
    /// <param name="targets">�^�[�Q�b�g�̃��X�g</param>
    /// <param name="myTf">�����̃g�����X�t�H�[��</param>
    /// <returns>��ԋ߂��^�[�Q�b�g</returns>
    public OpSensorObj GetNearestEnemy(List<OpSensorObj> targets, Transform myTf)
    {

        if (targets.Count == 0) return null;
        OpSensorObj sample = targets[0];

        foreach (OpSensorObj targetObj in targets)
        {
            if (!targetObj.IsEnemy()) continue;

            if (sample.GetDistance(myTf, isSqr: true) > targetObj.GetDistance(myTf, isSqr: true))
            {
                sample = targetObj;
            }
        }

        return sample;
    }

    /// <summary>
    /// ���[�_�[���^�[�Q�b�g�����m
    /// </summary>
    /// <param name="other"></param>
    /// <returns>�^�[�Q�b�g�����[�_�[�Ɉ��������������ǂ���</returns>
    private bool CheckIfItCanFire(Collider other)
    {

        RadarSensorObj sampleObj = new RadarSensorObj(other.gameObject);
        if (sampleObj.IsBullet())
        {
            return !IsMyBullet(other.gameObject);
        }

        if (tags.Contains(other.gameObject.tag) && transform.parent.gameObject != other.gameObject)
        {

            Vector3 posDelta = other.transform.position - coreTf.position;

            float target_angle = Vector3.Angle(transform.forward, posDelta);

            if (target_angle < angle / 2)
            {
                Debug.DrawRay(coreTf.position, posDelta, Color.red);
                if (Physics.Raycast(coreTf.position, posDelta, out RaycastHit hit, sensorSize))
                {
                    if (hit.collider == other)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void OnTriggerStay(Collider other)
    {

        if (CheckIfItCanFire(other))
        {
            if (targets.Find(x => x.EqualGameObj(other.gameObject)) == null)
            {
                //�^�[�Q�b�g���X�g�Ɋ܂܂�A�{�̂̃X�N���v�g�ɓ`����
                targets.Add(new OpSensorObj(other.gameObject));
                if (isEnergy)
                {
                    coreScript.OnOpticalSensor(targets, isVisible: true);
                }
                else
                {
                    coreScript.OnOpticalSensor(dummyTargets, isVisible: true);
                }
            }
        }
        else if (tags.Contains(other.gameObject.tag))
        {
            if (targets.Find(x => x.EqualGameObj(other.gameObject)) != null)
            {
                //�^�[�Q�b�g���X�g���珜�O���A�{�̂̃X�N���v�g�ɓ`����
                targets.RemoveAt(targets.FindIndex(x => x.EqualGameObj(other.gameObject)));
                if (isEnergy)
                {
                    coreScript.OnOpticalSensor(targets, isVisible: false);
                }
                else
                {
                    coreScript.OnOpticalSensor(dummyTargets, isVisible: false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (tags.Contains(other.gameObject.tag))
        {

            if (CheckIfItCanFire(other))
            {
                targets.Add(new OpSensorObj(other.gameObject));

                if (isEnergy)
                {
                    coreScript.OnOpticalSensor(targets, isVisible: true);
                }
                else
                {
                    coreScript.OnOpticalSensor(dummyTargets, isVisible: true);
                }
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

        if (tags.Contains(other.gameObject.tag) && transform.parent.gameObject != other.gameObject)
        {
            if (targets.Find(x => x.EqualGameObj(other.gameObject)) != null)
            {
                targets.RemoveAt(targets.FindIndex(x => x.EqualGameObj(other.gameObject)));

                if (isEnergy)
                {
                    coreScript.OnOpticalSensor(targets, isVisible: false);
                }
                else
                {
                    coreScript.OnOpticalSensor(dummyTargets, isVisible: false);
                }
            }
        }
    }

    /// <summary>
    /// �����̒e�ۂ��ǂ����𒲂ׂ�
    /// </summary>
    /// <param name="otherObj">�ڐG�����I�u�W�F�N�g</param>
    /// <returns>���g�̒e�ۂ��ǂ���</returns>
    private bool IsMyBullet(GameObject otherObj)
    {

        string sampleName = "";
        if (otherObj.TryGetComponent<BulletScript>(out BulletScript bullet))
        {
            sampleName = bullet.masterName;
        }
        else if (otherObj.TryGetComponent<EnergyBulletScript>(out EnergyBulletScript energy))
        {
            sampleName = energy.masterName;
        }
        else if (otherObj.TryGetComponent<MissileBulletSc>(out MissileBulletSc missile))
        {
            sampleName = missile.masterName;
        }
        if (coreScript.playerName == sampleName)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// �G�l���M�[����
    /// </summary>
    private void SensorEnergy()
    {
        if (energyScript.UseEnergy(sensorSize / 40f * (angle / 90) * Time.deltaTime))
        {
            isEnergy = true;
        }
        else
        {
            isEnergy = false;
            if (isReset)
            {
                coreScript.OnOpticalSensor(dummyTargets, isVisible: false);
                isReset = false;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rayAngle"></param>
    /// <returns></returns>
    public bool CheckWall(float rayAngle)
    {
        Vector3 direction = Quaternion.Euler(new Vector3(0, rayAngle, 0)) * coreTf.forward;
        Debug.DrawRay(coreTf.position, direction * sensorSize / 2, Color.blue);
        if (Physics.Raycast(coreTf.position, direction, out RaycastHit hit, sensorSize / 2))
        {
            if (hit.collider.tag == "Ground")
            {
                return true;
            }
        }
        return false;
    }
}
