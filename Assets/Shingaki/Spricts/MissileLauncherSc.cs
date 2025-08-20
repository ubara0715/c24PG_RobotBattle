using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncherSc : MonoBehaviour
{
    public GameObject missilePf;
    MissileBulletSc bulletCon;
    CoreScript core;

    [Header("���������e�̂܂Ƃߐ�(None�ł����v)"), SerializeField] GameObject bulletGroup;
    [Header("�ȉ��̕ϐ��̓f�o�b�O�ȊO�ŕύX���Ȃ���(���e���͕ς��Ă�������)")]
    [Header("���e��"), SerializeField] int bulletCount;
    [Header("���̎ˌ��Ŕ��˂���e��"),SerializeField] int bulletNum;
    [Header("�A�ˊԊu"), SerializeField] float continuousFiringInterval;
    [Header("���ˌ�N�[���^�C��"),SerializeField] float cooldownTime;
    [Header("�����`���[�̏d��"), SerializeField] int weight;

    [Header("���"), SerializeField] bool isTwin = false;
    [Header("����"), SerializeField] bool isMultiple = false;

    int sumWeight;
    int bulletWeight;
    int bulletCountNow;
    bool isCooldown = false;
    bool isEmpty = false;

    private void Awake()
    {
        //�R�A�X�N���v�g�擾
        transform.parent.TryGetComponent(out core);
        //�����e�����c�e���ɑ��
        bulletCountNow = bulletCount;
    }

    private void Start()
    {
        //���d�ʂ��R�A�ɓ`�B
        core.AddWeight(GetSumWight());
    }

    int GetSumWight()
    {
        //�e���܂߂��d�����v�Z
        sumWeight = weight;
        bulletWeight = missilePf.GetComponent<MissileBulletSc>().GetWeght();
        sumWeight += bulletWeight * bulletCount;
        return sumWeight;
    }

    /// <summary>
    /// ���̎c�e�����m�F
    /// </summary>
    /// <returns></returns>
    public int GetBulletsCount()
    {
        return bulletCountNow;
    }

    /// <summary>
    /// �e���󂩂ǂ���
    /// </summary>
    /// <returns></returns>
    public bool IsEmptyBullets()
    {
        return isEmpty;
    }

    /// <summary>
    /// �N�[���_�E�������ǂ���
    /// </summary>
    /// <returns></returns>
    public bool IsCooldown()
    {
        return isCooldown;
    }

    /// <summary>
    /// �~�T�C���𐶐����Ĕ���
    /// </summary>
    /// <param name="target">���[�_�[�Ō������Ώۂ̃I�u�W�F�N�g</param>
    public void Fire(OpSensorObj targetObj)
    {
        //�e��������Δ��˂ł��Ȃ�
        if (isEmpty) return;
        //�N�[���^�C�����͔��˂ł��Ȃ�
        if (isCooldown) return;

        //�Ώۖ����̏ꍇ�ԋp
        if (targetObj == null)
        {
            Debug.Log("���ˑΏۂ����݂��܂���");
            return;
        }
        if (isTwin)
        {
            for (int i = 0; i < 2; i++)
            {
                if (bulletCountNow <= 0) break;
                //�~�T�C���e�̐���
                if(bulletGroup == null)
                {
                    bulletCon =
                    Instantiate(missilePf, gameObject.transform.position, transform.rotation)
                        .GetComponent<MissileBulletSc>();
                }
                else
                {
                    bulletCon =
                    Instantiate(missilePf, gameObject.transform.position, transform.rotation, bulletGroup.transform)
                        .GetComponent<MissileBulletSc>();
                }
                //�~�T�C���e�X�N���v�g�Ƀ^�[�Q�b�g�I�u�W�F�N�g���w��
                bulletCon.SetBullet(targetObj.TargetOBj(), core.playerName, twin: true);
                //�����_���Ɍ��������炷
                if (i == 0) bulletCon.transform.Rotate(UnityEngine.Random.Range(0f, 3f), UnityEngine.Random.Range(-35f, -20f), 0);
                else bulletCon.transform.Rotate(UnityEngine.Random.Range(0f, 3f), UnityEngine.Random.Range(35f, 20f), 0);
                //�c�e�����炷
                bulletCountNow--;
                //�d�ʂ���~�T�C���ꔭ���̏d�ʂ����炷
                core.ReduceWeight(bulletWeight);
            }
        }
        else
        {
            //�~�T�C���e�̐���
            if (bulletGroup == null)
            {
                bulletCon =
                Instantiate(missilePf, gameObject.transform.position, transform.rotation)
                    .GetComponent<MissileBulletSc>();
            }
            else
            {
                bulletCon =
                Instantiate(missilePf, gameObject.transform.position, transform.rotation, bulletGroup.transform)
                    .GetComponent<MissileBulletSc>();
            }
            //�~�T�C���e�X�N���v�g�Ƀ^�[�Q�b�g�I�u�W�F�N�g���w��
            bulletCon.SetBullet(targetObj.TargetOBj(), core.playerName);
            //�����_���Ɍ��������炷
            bulletCon.transform.Rotate(UnityEngine.Random.Range(0f, 3f), UnityEngine.Random.Range(-3f, 3f), 0);
            //�c�e�����炷
            bulletCountNow--;
            //�d�ʂ���~�T�C���ꔭ���̏d�ʂ����炷
            core.ReduceWeight(bulletWeight);
        }

        //�N�[���^�C���o�ߌ�Ƀt���O������
        isCooldown = true;
        Invoke(new Action(() => { isCooldown = false; }).Method.Name, cooldownTime);
        //�c�e�������Ȃ������t���O���I��
        if(bulletCountNow <= 0) isEmpty = true;
    }

    /// <summary>
    /// �����̑ΏۂɃ~�T�C���𔭎�
    /// </summary>
    /// <param name="target">���[�_�[�Ō������Ώۂ̃I�u�W�F�N�g���X�g</param>
    public void MultiFire(List<OpSensorObj> targetList)
    {
        //�����t���O�m�F
        if (!isMultiple) return;
        //�e��������Δ��˂ł��Ȃ�
        if (isEmpty) return;
        //�N�[���^�C�����͔��˂ł��Ȃ�
        if (isCooldown) return;
        //���s
        StartCoroutine(MultiFireIEnum(targetList));
    }
    IEnumerator MultiFireIEnum(List<OpSensorObj> targetList)
    {
        //��Ƀ��b�N���Ă���G��ۑ�
        List<OpSensorObj> targetList_sumple = new(targetList);

        //�A�ˊԊu���ɒ�`
        WaitForSeconds wait = new(continuousFiringInterval);

        for (int i = 0; i < bulletNum; i++)
        {
            //�r���Œe���Ȃ��Ȃ�����I��
            if (isEmpty) break;

            //�Ώۖ����̏ꍇ�ԋp
            if (targetList_sumple.Count == 0)
            {
                Debug.Log("���ˑΏۂ����݂��܂���");
                break;
            }

            //�^�[�Q�b�g�����ԂɎw��
            int index = i % targetList_sumple.Count;
            GameObject target = targetList_sumple[index].TargetOBj();

            //�~�T�C���e�̐���
            if (bulletGroup == null)
            {
                bulletCon =
                Instantiate(missilePf, gameObject.transform.position, transform.rotation)
                    .GetComponent<MissileBulletSc>();
            }
            else
            {
                bulletCon =
                Instantiate(missilePf, gameObject.transform.position, transform.rotation, bulletGroup.transform)
                    .GetComponent<MissileBulletSc>();
            }
            //�~�T�C���e�X�N���v�g�Ƀ^�[�Q�b�g�I�u�W�F�N�g���w��
            bulletCon.SetBullet(target, core.playerName, multiple: true);
            //�����_���Ɍ��������炷
            bulletCon.transform.Rotate(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-15f, 15f), 0);

            //�c�e�����炷
            bulletCountNow--;
            //�d�ʂ���~�T�C���ꔭ���̏d�ʂ����炷
            core.ReduceWeight(bulletWeight);

            //�c�e�������Ȃ������t���O���I��
            if (bulletCountNow < 1) isEmpty = true;

            //���̘A�ˉ\���Ԃ܂őҋ@
            yield return wait;
        }

        //�N�[���^�C���o�ߌ�Ƀt���O������
        isCooldown = true;
        Invoke(new Action(() => { isCooldown = false; }).Method.Name, cooldownTime);
    }
}
