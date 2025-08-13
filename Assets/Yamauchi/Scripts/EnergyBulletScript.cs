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

    // �J���[�ύX
    /*
    ParticleSystem.MainModule main;
    ParticleSystem ps;
    */

    // ���\�b�h
    void Start()
    {
        //main = ps.main;
    }

    void Awake()
    {
        energyGear = GetComponentInParent<EnergyGearScript>();
        //ps = GetComponent<ParticleSystem>();
        FilghtDistance();
        //SwitchColor();
    }

    void Update()
    {
        // �G�l���M�[�Ȃ��Ȃ��������
        if(usedEnergy_clone <= 0)
        {
            Destroy(gameObject);
        }

        // ��������
        //Debug.Log("�c��G�l���M�[�F" + usedEnergy_clone + "�򋗗��F" + range);
        if(range / 100 >= 1)
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
        //Debug.Log("�_���[�W�����I");
    }

    // �򋗗��v�Z
    void FilghtDistance()
    {
        range += speed_clone * Time.fixedDeltaTime;
        Invoke("FilghtDistance", Time.deltaTime);
    }


    // �ۗ�
    /*
    void SwitchColor()
    {
        if(usedEnergy_clone <= 25.0f)
        {
            //main.startColor = new ParticleSystem.MinMaxGradient(Color.green);
        }
        else if(usedEnergy_clone <= energyGear.usedMax * 0.3)
        {
            // �F
        }
        else if(usedEnergy_clone <= energyGear.usedMax * 0.6)
        {
            // ���F
        }
        else
        {
            // ���F
        }
    }
    */

    //�J���i�K
    /*
    void OnCollisionEnter(Collision collision)
    {
        distance = (int)(Vector3.Distance(startPos, transform.position));
        Destroy(gameObject);
    }

    //�e�X�g�p�A���������͂ł���
    void OnDestroy()
    {
        //Debug.Log(EnergyDamege());
    }
    */
}
