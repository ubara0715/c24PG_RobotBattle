using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMoveScript : MonoBehaviour
{
    public CoreScript coreScript;

    [SerializeField, Header("�G�l���M�[�Ǘ��X�N���v�g")]
    EnergyScript energyScript;

    [SerializeField]
    GameObject sphere;//�e�X�g�p

    //���{�b�g��Rigidbody
    Rigidbody robotRB;
    
    [SerializeField, Header("�W�����v��")]
    float jumpForce = 1.0f;
    [SerializeField, Header("�ړ���")]
    float moveForce = 1.0f;
    [SerializeField, Header("��]��")]
    float rotateForce = 1.0f;
    [SerializeField, Header("�ō����x")]
    float maxSpeed = 1.0f;
    
    //��]����bool
    bool isRotate = false;

    void Start()
    {
        robotRB = GetComponent<Rigidbody>();
        
        SetMass();

        //�e�X�g�p
        StartCoroutine(MoveTarget(sphere));
    }

    void Update()
    {
        //�e�X�g�p
        if (Input.GetKey(KeyCode.Space)) MoveUp();
    }

    //�@�̂̎��ʂ�ݒ�(�@�̂̏d�ʂ��ς�邽�т�CoreScript����Ă�)
    public void SetMass()
    {
        //CoreScript�̏d�ʕϐ������{�b�g�̎��ʂƂ���
        robotRB.mass = coreScript.weight;
    }

    public void MoveUp()//�W�����v
    {
        robotRB.AddForce(transform.up * jumpForce, ForceMode.Force);

        if (robotRB.velocity.y > maxSpeed)
        {
            robotRB.velocity = new Vector3(
                robotRB.velocity.x,
                maxSpeed,
                robotRB.velocity.z
                );
        }

    }

    //�^�[�Q�b�g�ւ̈ړ�
    public IEnumerator MoveTarget(GameObject targetOBJ)
    {
        //�����͌�X�ύX
        while (true)
        {
            if (transform.position.y - targetOBJ.transform.position.y <= -1
                && energyScript.UseEnergy(0.1f)) 
            {
                MoveUp();
            }

            Vector3 direction = targetOBJ.transform.position - transform.position;

            //�^�[�Q�b�g�ɋ߂Â�����I��(������)
            if (Mathf.Abs(direction.x) < (transform.localScale.x + targetOBJ.transform.localScale.x) / 2
                && Mathf.Abs(direction.y) < (transform.localScale.y + targetOBJ.transform.localScale.y) / 2
                && Mathf.Abs(direction.z) < (transform.localScale.z + targetOBJ.transform.localScale.z) / 2)
                break;

            direction.y = 0;//xz���ʂňړ�����

            if (Mathf.Abs(direction.x) > (transform.localScale.x + targetOBJ.transform.localScale.x) / 2
                || Mathf.Abs(direction.z) > (transform.localScale.z + targetOBJ.transform.localScale.z) / 2
                && energyScript.UseEnergy(0.01f))
            {
                //�������瑊��̕�����AddForce
                robotRB.AddForce(direction.normalized * moveForce, ForceMode.Force);
                
                //���x�`�F�b�N
                CheckVelocity();

                //��]���łȂ��Ƃ�
                if (!isRotate)
                {
                    isRotate = true;

                    StartCoroutine(RotateRobot(targetOBJ));
                }
            }
            else
            {
                robotRB.velocity = new Vector3(0, robotRB.velocity.y, 0);
            }

            yield return null;
        }

        robotRB.velocity = Vector3.zero;
    }

    //�ړ����x�̒���
    void CheckVelocity()
    {
        //���{�b�g��xy���ʂ̑��x���v�Z
        float speed = Mathf.Sqrt(Mathf.Pow(robotRB.velocity.x, 2) + Mathf.Pow(robotRB.velocity.z, 2));
        
        //�ړ����x����`�F�b�N
        if (speed > maxSpeed)
        {
            robotRB.velocity = new Vector3(
                robotRB.velocity.x / (speed / maxSpeed),
                robotRB.velocity.y,
                robotRB.velocity.z / (speed / maxSpeed)
                );
        }
    }

    //���{�b�g�̉�]
    IEnumerator RotateRobot(GameObject targetOBJ)
    {
        while (true) 
        {
            Vector3 direction = targetOBJ.transform.position - transform.position;

            direction.y = 0;//xz���ʂŉ�]

            if (energyScript.UseEnergy(0.01f))
            {
                float angle = Vector3.SignedAngle(transform.forward, direction, transform.up);
                
                //�قڐ��ʂ���������I��
                if (Mathf.Abs(angle) < 5) break;

                //�p�x�����������ɉ�]
                robotRB.AddTorque(transform.up * Mathf.Sign(angle) * rotateForce, ForceMode.Force);
            }

            yield return null;
        }

        robotRB.angularVelocity = Vector3.zero;

        isRotate = false;
    }
}
