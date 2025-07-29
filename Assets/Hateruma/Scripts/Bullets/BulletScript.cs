using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    bool hit = false;//�����蔻��p

    [SerializeField] float mass = 0;//�e�ێ���
    int damage = 0;//�_���[�W�p
    float attenuation = 0;//�����p

    [SerializeField] MeshRenderer meshRen;//��\���p

    [SerializeField] Collider col;//�R���C�_�[

    AnimationCurve speedAnimC;//�����p�̃A�j���[�V�����J�[�u
    AnimationCurve heightAnimC;//�e�����p�̃A�j���[�V�����J�[�u

    private void Start()
    {
        meshRen = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
    }

    /// <summary>
    /// �e�̋�������
    /// </summary>
    /// <param name="speed">�e��</param>
    /// <param name="range">�˒�</param>
    public IEnumerator Shot(float speed, float range)
    {
        meshRen.enabled = true;//�e�\��
        col.enabled = true;//����\��

        //�����ɍ��킹�Č����p�A�e�����̃A�j���[�V�����J�[�u���쐬
        if (speedAnimC == null)
        {
            speedAnimC = new AnimationCurve();

            speedAnimC.AddKey(new Keyframe(range * 0.75f, speed));
            speedAnimC.AddKey(new Keyframe(range, speed * 0.5f, 0, 0));
        }

        if (heightAnimC == null)
        {
            heightAnimC = new AnimationCurve();

            heightAnimC.AddKey(new Keyframe(range * 0.5f, 0, 0, 0));
            heightAnimC.AddKey(new Keyframe(range, -0.5f * mass, 0, 0));
        }

        hit = true;//�����蔻��ON

        float distance = 0f;//����

        while (hit)
        {
            float eval = speedAnimC.Evaluate(distance);//���݈ʒu�̒l

            attenuation = eval;

            float moveAmount = speed * eval * Time.deltaTime;//�i�ދ���

            transform.position += transform.forward * moveAmount + new Vector3(0, heightAnimC.Evaluate(distance));//�|�W�V�����ړ�

            distance += moveAmount;//�������Z

            //�������˒��͈͂𒴂�����~�߂�
            if (distance >= range)
                break;

            yield return null;//1�t���[���҂�
        }

        BulletHit();//�����ڂƓ����蔻���\��
    }

    /// <summary>
    /// �_���[�W�l���v�Z���ĕԂ�
    /// </summary>
    public int GetDamage()
    {
        damage = (int)(mass * attenuation);
        return damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        BulletHit();//�����ڂƓ����蔻���\��
    }

    /// <summary>
    /// ���e���̏���(�����ڂƓ����蔻���\��)
    /// </summary>
    void BulletHit()
    {
        hit = false;
        meshRen.enabled = false;
        col.enabled = false;
    }
}
