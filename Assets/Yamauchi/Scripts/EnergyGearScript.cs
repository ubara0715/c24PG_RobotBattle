using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class EnergyGearScript : MonoBehaviour
{
    [SerializeField,Header("�G�l���M�[�e��Prefab")] GameObject energyBullet;
    [Header("�G�l���M�[�v�[�����t����Object")] public EnergyScript energyPool;
    [Header("��������Object���܂Ƃ߂Ă������Object")] public GameObject bulletGroup;
    CoreScript core;

    //[HideInInspector]
    public int usedMax = 50;
    //[HideInInspector]
    public int usedMin = 10;
    public float speed = 100.0f;

    [SerializeField, Header("�N�[���^�C��")]
    float coolTime_initial = 1.0f;
    [SerializeField]
    float coolTime;
    public bool isCoolDown = false;

    // ���\�b�h
    void Start()
    {
        core = transform.parent.GetComponent<CoreScript>();
        core.AddWeight((int)(usedMax * 0.1));
        coolTime = coolTime_initial;
    }

    void Update()
    {
        if(isCoolDown)
        {
            coolTime += 1.0f * Time.deltaTime;
            if(coolTime >= coolTime_initial)
            {
                isCoolDown = false;
            }
        }
    }

    // public�֐�
    /// <summary>
    /// �G�l���M�[�e�𔭎˂���֐��A�З͕ύX�\
    /// </summary>
    /// <param name="usedEnergy">�З́A���l���̂܂܈З͂ɂȂ�</param>
    /// <param name="target">�^�[�Q�b�g�A�Z���T�[�Ŋ��m�����I�u�W�F�N�g������z��</param>
    public void ShotEnergy(int usedEnergy,GameObject target)
    {
        // �w��͈̔͊O�̃G�l���M�[�ʂȂ猂���Ȃ�
        if (usedEnergy < usedMin || usedEnergy > usedMax) return;

        // �����]��
        transform.LookAt(target.transform);
        if(transform.rotation.y <= -90.0f || transform.rotation.y >= 90.0f) return;

        // �N�[���^�C�����Ȃ猂���Ȃ�
        if (isCoolDown) return;

        // �G�l���M�[���Ȃ��Ȃ猂���Ȃ��A����Ȃ猸�炷
        if (!energyPool.UseEnergy(usedEnergy)) return;

        // �N���[���쐬�Ɛݒ�AComponent�̎擾
        GameObject clone = EnergyBullet_clone();

        Rigidbody rb_clone = clone.GetComponent<Rigidbody>();
        EnergyBulletScript bulletSc = clone.GetComponent<EnergyBulletScript>();

        bulletSc.usedEnergy_clone = usedEnergy;
        bulletSc.speed_clone = speed;
        bulletSc.masterName = core.playerName;

        clone.transform.parent = bulletGroup.transform;
        clone.name = "EnergyBullet_" + core.playerName;
        
        // ��������
        Shot(rb_clone);

        // �N�[���^�C��
        coolTime = 0.0f;
        isCoolDown = true;
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
