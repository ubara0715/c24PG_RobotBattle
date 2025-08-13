using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncherSc : MonoBehaviour
{
    public GameObject missilePf;
    MissileBulletSc bulletCon;

    [Header("���e��"), SerializeField] int bulletCount;
    [Header("���˂���e��"),SerializeField] int bulletNum;
    [Header("�A�ˊԊu"), SerializeField] float continuousFiringInterval;
    [Header("���ˌ�N�[���^�C��"),SerializeField] float cooldownTime;


    /// <summary>
    /// �~�T�C���𐶐����Ĕ���
    /// </summary>
    /// <param name="target">���[�_�[�Ō������Ώۂ̃I�u�W�F�N�g</param>
    public void Fire(OpSensorObj targetObj)
    {
        //�Ώۖ����̏ꍇ�ԋp
        if (targetObj == null)
        {
            Debug.Log("���ˑΏۂ����݂��܂���");
            return;
        }

        //�~�T�C���e�̐���
        bulletCon = 
            Instantiate(missilePf, gameObject.transform.position, transform.rotation)
                .GetComponent<MissileBulletSc>();
        //�~�T�C���e�X�N���v�g�Ƀ^�[�Q�b�g�I�u�W�F�N�g���w��
        bulletCon.SetTargetObj(targetObj.TargetOBj());
    }

    /// <summary>
    /// �����̑ΏۂɃ~�T�C���𔭎�
    /// </summary>
    /// <param name="target">���[�_�[�Ō������Ώۂ̃I�u�W�F�N�g���X�g</param>
    public void MultiFire(List<GameObject> targetList)
    {
        StartCoroutine(MultiFireIEnum(targetList));
    }
    IEnumerator MultiFireIEnum(List<GameObject> targetList)
    {
        //�A�ˊԊu���ɒ�`
        WaitForSeconds wait = new WaitForSeconds(continuousFiringInterval);

        for (int i = 0; i < bulletNum; i++)
        {
            //�Ώۖ����̏ꍇ�ԋp
            if (targetList.Count == 0)
            {
                Debug.Log("���ˑΏۂ����݂��܂���");
                break;
            }

            //�^�[�Q�b�g�����ԂɎw��
            int index = i % targetList.Count;
            GameObject target = targetList[index];

            //�~�T�C���e�̐���
            bulletCon =
                Instantiate(missilePf, gameObject.transform.position, transform.rotation)
                    .GetComponent<MissileBulletSc>();
            //�~�T�C���e�X�N���v�g�Ƀ^�[�Q�b�g�I�u�W�F�N�g���w��
            bulletCon.SetTargetObj(target);

            //���̘A�ˉ\���Ԃ܂őҋ@
            yield return wait;
        }
    }
}
