using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMoveScript : MonoBehaviour
{
    public CoreScript coreScript;

    [SerializeField, Header("�G�l���M�[�Ǘ��X�N���v�g")]
    EnergyScript energyScript;

    [SerializeField,Header("�e�X�g�p�̓G�I�u�W�F�N�g")]//��ŏ���
    GameObject enemy;
    [SerializeField, Header("�����_���ړ��^�[�Q�b�g")]
    GameObject targetObj;

    //���{�b�g��Rigidbody
    Rigidbody robotRB;
    
    [SerializeField, Header("�ő县��")]
    float maxMass = 0;
    //���{�b�g�̎���
    int mass = 0;

    [SerializeField, Header("�W�����v��")]
    float jumpForce = 1.0f;
    [SerializeField, Header("�ړ���")]
    float moveForce = 1.0f;
    [SerializeField, Header("��]��")]
    float rotateForce = 1.0f;

    [Header("�������x�͈̔�")]
    [SerializeField, Header("���")]
    float maxSpeed = 1.0f;
    [SerializeField, Header("����")]
    float minSpeed = 1.0f;
    //���{�b�g�̐������x
    float limitSpeed = 0;

    [Header("�^�[�Q�b�g�؂�ւ��Ԋu")]
    [SerializeField, Header("�ő�")]
    float maxTime = 0;
    [SerializeField, Header("�ŏ�")]
    float minTime = 0;

    //��]����bool
    bool isRotate = false;
    //�Ǐ]����bool
    bool isTarget = false;

    //�ړ����R���[�`��
    Coroutine _moveTarget;

    private void Awake()
    {
        robotRB = GetComponent<Rigidbody>();
    }

    void Start()
    {
        //�X�P�[��x+y+z�����ʂƂ���
        mass = (int)(transform.localScale.x + transform.localScale.y + transform.localScale.z);
        coreScript.AddWeight(mass);
        coreScript.hp = mass * 100;

        //���ʂɉ����Ĉړ����x���v�Z
        limitSpeed = Mathf.Lerp(maxSpeed, minSpeed, mass / maxMass);

        SetTarget();
    }

    void Update()
    {
        //�e�X�g�p(���[�_�[�ɓG���ڂ����z��)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isTarget = true;
            StopCoroutine(_moveTarget);
            _moveTarget = StartCoroutine(MoveTarget(enemy));    
        }

        //�^�[�Q�b�g�����Ȃ��Ƃ�
        if (!isTarget)
        {
            SetTarget();
        }
    }

    //�����_���ړ��̃^�[�Q�b�g�����߂�
    void SetTarget()
    {
        targetObj.transform.position = new Vector3(
                Random.Range(-50, 50),
                Random.Range(1, 3),
                Random.Range(-50, 50)
                );

        isTarget = true;
        _moveTarget = StartCoroutine(MoveTarget(targetObj));
    }

    //�@�̂̎��ʂ�ݒ�(�@�̂̏d�ʂ��ς�邽�т�CoreScript����Ă�)
    public void SetMass()
    {
        //CoreScript�̏d�ʕϐ������{�b�g�̎��ʂƂ���
        robotRB.mass = coreScript.weight;
    }
 
    public void MoveUp()//��ړ�
    {
        robotRB.AddForce(Vector3.up * jumpForce, ForceMode.Force);

        if (robotRB.velocity.y > limitSpeed)
        {
            robotRB.velocity = new Vector3(
                robotRB.velocity.x,
                limitSpeed,
                robotRB.velocity.z
                );
        }
    }

    //�^�[�Q�b�g�ւ̈ړ�
    public IEnumerator MoveTarget(GameObject targetOBJ)
    {
        float waitTime = Random.Range(minTime, maxTime);
        float startTime = Time.time;

        //�����͌�X�ύX
        while (true)
        {
            //�����ƃ^�[�Q�b�g�Ԃ̃x�N�g�����v�Z
            Vector3 direction = targetOBJ.transform.position - transform.position;

            //����ړ�
            if (transform.position.y - targetOBJ.transform.position.y < -1
                && energyScript.UseEnergy(2f * Time.deltaTime)) 
            {
                MoveUp();
            }

            //���B�`�F�b�N(������)
            bool reached = Mathf.Abs(direction.x) <= (transform.localScale.x + targetOBJ.transform.localScale.x) / 2
                && Mathf.Abs(direction.y) <= (transform.localScale.y + targetOBJ.transform.localScale.y) / 2
                && Mathf.Abs(direction.z) <= (transform.localScale.z + targetOBJ.transform.localScale.z) / 2;

            //�^�C���`�F�b�N
            bool timeOut = Time.time - startTime >= waitTime;

            if (reached || timeOut) break;

            //xz���ʂ̈ړ��x�N�g�����v�Z
            Vector3 horizontalDir = new Vector3(direction.x, 0, direction.z);

            if (Mathf.Abs(horizontalDir.x) > (transform.localScale.x + targetOBJ.transform.localScale.x) / 2
                || Mathf.Abs(horizontalDir.z) > (transform.localScale.z + targetOBJ.transform.localScale.z) / 2
                && energyScript.UseEnergy(1f*Time.deltaTime))
            {
                robotRB.AddForce(horizontalDir.normalized * moveForce, ForceMode.Force);

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
                //robotRB.velocity = new Vector3(0, robotRB.velocity.y, 0);
            }

            yield return null;
        }

        robotRB.velocity = Vector3.zero;
        isTarget = false;
    }

    //�ړ����x�̒���
    void CheckVelocity()
    {
        //���{�b�g��xy���ʂ̑��x���v�Z
        float speed = Mathf.Sqrt(Mathf.Pow(robotRB.velocity.x, 2) + Mathf.Pow(robotRB.velocity.z, 2));
        
        //�ړ����x����`�F�b�N
        if (speed > limitSpeed)
        {
            robotRB.velocity = new Vector3(
                robotRB.velocity.x / (speed / limitSpeed),
                robotRB.velocity.y,
                robotRB.velocity.z / (speed / limitSpeed)
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

            if (energyScript.UseEnergy(0))
            {
                float angle = Vector3.SignedAngle(transform.forward, direction, transform.up);
                
                //�قڐ��ʂ���������I��
                if (Mathf.Abs(angle) < 5) break;

                //�p�x�����������ɉ�]
                robotRB.AddTorque(transform.up * Mathf.Sign(angle) * rotateForce, ForceMode.Acceleration);
            }

            yield return null;
        }

        robotRB.angularVelocity = Vector3.zero;
        isRotate = false;
    }
}
