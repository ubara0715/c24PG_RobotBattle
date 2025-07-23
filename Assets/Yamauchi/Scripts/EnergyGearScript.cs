using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class EnergyGearScript : MonoBehaviour
{
    [SerializeField,Header("�G�l���M�[�e��Prefab���A�^�b�`���Ă�������")] GameObject energyBullet;
    [Header("�G�l���M�[�v�[�����t����Object���A�^�b�`���Ă�������")] public EnergyScript energyPool;
    [Header("��肭���˂ł��Ȃ��Ȃ��Ǝv�����璲�����Ă�������")] public float instantiatePos = 1.1f;
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

        // �����v�Z���ăG�l���M�[�g�p�ʂ��Œ���ɗ}������
        float two_distance = Vector3.Distance(gameObject.transform.position, target.transform.position);

        // �N���[���쐬�Ɛݒ�AComponent�̎擾
        GameObject clone = EnergyBullet_clone();

        Rigidbody rb_clone = clone.GetComponent<Rigidbody>();
        EnergyBulletScript bulletSc = clone.GetComponent<EnergyBulletScript>();
        bulletSc.usedEnergy_clone = usedEnergy;
        bulletSc.speed_clone = speed;
        clone.transform.parent = gameObject.transform;

        // ��������
        Shot(rb_clone);
    }

    // privert�֐�
    GameObject EnergyBullet_clone()
    {
        Vector3 instPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + instantiatePos);

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
