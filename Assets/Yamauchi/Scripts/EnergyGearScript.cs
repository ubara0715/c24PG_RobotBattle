using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class EnergyGearScript : MonoBehaviour
{
    [SerializeField,Header("�G�l���M�[�e��Prefab�����Ă�������")] GameObject energyBullet;
    [Header("�G�l���M�[�v�[�����t����Object�����Ă�������")] public EnergyScript energyPool;
    [Header("��������Object���܂Ƃ߂Ă������Object�����Ă�������")] public GameObject bulletGroup;
    CoreScript core;

    [HideInInspector]public float usedMax = 100.0f;
    [HideInInspector]public float usedMin = 10.0f;
    public float speed = 100.0f;

    // ���\�b�h
    void Start()
    {
        core = transform.parent.GetComponent<CoreScript>();
        core.AddWeight((int)usedMax);
    }

    // public�֐�
    public void ShotEnergy(int usedEnergy,GameObject target)
    {
        // �_���[�W�v�Z�A���Z��Bullet�̕��ōs��
        // �l��n����AddForce�������邾��

        if(usedEnergy < usedMin || usedEnergy > usedMax)
        {
            return; // �͈͊O�Ȃ猂���Ȃ�
        }

        // �����v�Z���ăG�l���M�[�g�p�ʂ��Œ���ɗ}������
        float two_distance = Vector3.Distance(gameObject.transform.position, target.transform.position);

        // �����]��
        transform.LookAt(target.transform);
        if(transform.eulerAngles.y < -90.0f || transform.eulerAngles.y > 90.0f)
        {
            energyPool.energyAmount += usedEnergy; // �����]����90�x�ȏ�Ȃ�G�l���M�[������Ȃ�
            return; // �����]����90�x�ȏ�Ȃ猂���Ȃ�
        }

        // �N���[���쐬�Ɛݒ�AComponent�̎擾
        GameObject clone = EnergyBullet_clone();

        Rigidbody rb_clone = clone.GetComponent<Rigidbody>();
        EnergyBulletScript bulletSc = clone.GetComponent<EnergyBulletScript>();
        bulletSc.usedEnergy_clone = usedEnergy;
        bulletSc.speed_clone = speed;
        clone.transform.parent = bulletGroup.transform;

        // ��������
        Shot(rb_clone);
    }

    // privert�֐�
    public GameObject EnergyBullet_clone()
    {
        //Vector3 instPos = new Vector3(transform.parent.localPosition.x + transform.position.x, transform.parent.localPosition.y + transform.position.y, transform.parent.localPosition.z + instantiatePos);
        Vector3 instPos = (transform.position + transform.forward);

         GameObject energyBullet_clone =
            Instantiate(
                energyBullet,
                instPos, //AddForce�Ȃ̂Ő����ʒu�Ƀ��m������Ƃ��܂����˂ł��Ȃ��A�̂ł������˂��Ȃ��Ȃ��Ǝv�����璲�����Ă�
                Quaternion.identity
                );

        return energyBullet_clone;
    }

    void Shot(Rigidbody rb)
    {
        rb.AddForce(transform.forward * (speed / Time.fixedDeltaTime), ForceMode.Force);
    }
}
