using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBulletScript : MonoBehaviour
{
    public string masterName;

    // ����X�N���v�g
    EnergyGearScript energyGear;

    // �_���[�W�v�Z
    float range = 0;
    int damege = 0;
    [HideInInspector] public float usedEnergy_clone;
    [HideInInspector] public float speed_clone;

    void Awake()
    {
        energyGear = GetComponentInParent<EnergyGearScript>();
        FilghtDistance();
    }

    void Update()
    {
        // �G�l���M�[�Ȃ��Ȃ��������
        if(usedEnergy_clone <= 0)
        {
            Destroy(gameObject);
        }

        // ��������
        if(range / speed_clone >= 1)
        {
            DistanceDecay();
            range = 0;
        }
    }

    // �U�ꂽ����ŁA�_���[�W��\��
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        Debug.Log("�_���[�W(�G�l���M�[�e)�F"+EnergyDamege());
    }

    // �֐�

    // �_���[�W�Z�o
    public int EnergyDamege()
    {
        damege = (int)usedEnergy_clone;
        return damege;
    }

    // ���������p�̊֐��A�����̂͊m�F�ς�
    void DistanceDecay()
    {
        usedEnergy_clone = (int)(usedEnergy_clone * 0.8f);
    }

    // �򋗗��v�Z
    void FilghtDistance()
    {
        range += speed_clone * Time.fixedDeltaTime;
        Invoke("FilghtDistance", Time.deltaTime);
    }
}
