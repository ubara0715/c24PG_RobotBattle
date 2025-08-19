using System;
using System.Xml.Serialization;
using UnityEngine;


public class MissileBulletSc : MonoBehaviour
{
    public string masterName;   //������̖��O

    Rigidbody rb;               //�e��Rigidbody
    GameObject bulletObj;       //�e��Object
    GameObject explosionObj;    //����Object
    GameObject targetObj;       //�ǔ��Ώ�

    [Header("�����x"),SerializeField, Range(1, 100)] float initalSpeed;
    [Header("�����x"),SerializeField, Range(1, 100)] float accel; 
    [Header("�ō����x"),SerializeField, Range(1, 100)] float accelLimit = 10;
    [Header("�z�[�~���O���\"), SerializeField, Range(0, 3)] float tracking;  
    [Header("�����͈�(m)"),SerializeField, Range(3f, 30)] float explosionArea = 10;
    [Header("�����J�n����"),SerializeField,Range(0, 1)] float startAccelTime = 0.2f;
    [Header("�z�[�~���O�\����"), SerializeField] float trackingLimitDistance = 10;
    [Header("�����܂ł̎���"),SerializeField] float destroyTimeLimit = 10;
    [Header("��������c������"),SerializeField] float explosionTime = 0.4f;

    int attackPow = 0;          //�U����
    float moveSpeed = 0;        //���x
    bool isTracking = false;    //�ǐՒ�
    bool isAccel = false;       //������
    float attackRate = 1;       //�U���͔{��

    public int sum_weight;

    public void SetBullet(GameObject target,string name,bool twin = false,bool multiple = false)
    {
        targetObj = target;
        masterName = name;
        if (twin) attackRate = 0.8f;
        if (multiple) attackRate = 0.5f;
    }

    public int GetWeght()
    {
        //�d����Ԃ� (������*10 + �ǐ՗� + ������ + ���x)/10
        return (int)((explosionArea * 20 + tracking * 33 + accel + initalSpeed)/(10));
    }

    [ContextMenu("SetSumWeight")]
    public void SetSumWeight()
    {
        sum_weight = GetWeght();
    }

    public int GetDamage()
    {
        return attackPow;
    }


    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        //�����܂ł̃J�E���g
        Invoke(nameof(HitObject), destroyTimeLimit);

        //�ǐՎ��s�܂ł̃J�E���g(�t���O���I���ɂ���)
        Invoke(new Action(() => { isTracking = true; }).Method.Name, explosionArea / 30);

        //�����J�n�܂ł̃J�E���g(�t���O���I���ɂ���)
        Invoke(new Action(() => { isAccel = true; }).Method.Name, startAccelTime);

        //���ˏ����x��^����
        rb.velocity = initalSpeed * transform.forward;
    }

    private void FixedUpdate()
    {
        //�ǔ��t���O���I���Ȃ�Βe���^�[�Q�b�g�ɒǔ�������
        if (isTracking) TrackingBullet();

        //�����t���O���I���Ȃ�Βe������
        if (isAccel) AccelBullet();
    }

    void Init()
    {
        //�e��Rigidbody��ǉ�
        rb = GetComponent<Rigidbody>();

        //�q�I�u�W�F�N�g���擾
        bulletObj = transform.GetChild(0).gameObject;
        explosionObj = transform.GetChild(1).gameObject;

        //�I�u�W�F�N�g�̃A�N�e�B�u��������
        bulletObj.SetActive(true);
        explosionObj.SetActive(false);

        //�����I�u�W�F�N�g�͈̔͂��w��̑傫���ɕύX
        explosionObj.transform.localScale = new Vector3(explosionArea, explosionArea, explosionArea);

        //�U���͂𔚔��͂�10�{�Ɏw��(int)
        attackPow = (int)(explosionArea * 10);
    }

    void AccelBullet()
    {
        //�e�̑��x���擾
        moveSpeed = rb.velocity.magnitude;
        //�e�̑��x�ɉ�������ǉ�
        moveSpeed += accel * Time.fixedDeltaTime;
        //�����e�̑��x�����E���x��葁����������E���x����
        moveSpeed = Mathf.Min(moveSpeed, accelLimit);
        //�e�̑O�����ɉ������̕t�^���ꂽ���x�ňړ�������
        rb.velocity = moveSpeed * transform.forward;
    }

    void TrackingBullet()
    {
        //�^�[�Q�b�g�܂ł̃x�N�g�����擾
        Vector3 targetVec = targetObj.transform.position - transform.position;

        //���������ȏ㗣��Ă�����ǐՂ���߂�
        if(targetVec.sqrMagnitude > trackingLimitDistance * trackingLimitDistance)
        {
            isTracking = false;
            return;
        }

        //���K��
        targetVec.Normalize();
        //�ǔ��������v�Z���đ��
        float step = tracking * Time.fixedDeltaTime;
        //�����ɉ����ăx�N�g�����^�[�Q�b�g�Ɋ񂹂�
        Vector3 lookVec = Vector3.MoveTowards(transform.forward, targetVec, step);
        //�e���v�Z�����x�N�g���̕����ɉ�]������
        transform.rotation = Quaternion.LookRotation(lookVec);
    }

    void HitObject()
    {
        //�ړ���������~
        rb.constraints = RigidbodyConstraints.FreezePosition;

        //�I�u�W�F�N�g�̃A�N�e�B�u�𔽓](����)
        bulletObj.SetActive(false);
        explosionObj.SetActive(true);

        //������莞�Ԍ�ɏ���
        Invoke(nameof(DestroyGameObject),explosionTime);
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    public void OnBulletTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //���������G�@�̂�CoreScript���擾
            CoreScript targetCoreSc = other.GetComponent<CoreScript>();
            //���������̋@�̂Ȃ疳��
            if (targetCoreSc.playerName == masterName) return;
            HitObject();
        }
        else if(other.CompareTag("Ground"))
        {
            HitObject();
        }

        //if (collision.collider.CompareTag("���[�U�["))
        //{
        //    HitObject();
        //}
    }

    public void OnExplosionTriggerEnter(Collider other)
    {
        //�_���[�W�t�^
        if (other.CompareTag("Player"))
        {
            //���������G�@�̂�CoreScript���擾
            CoreScript targetCoreSc = other.GetComponent<CoreScript>();
            //�@�̂Ƀo���A�⑕�b�����݂���Ȃ�_���[�W��^���Ȃ�(�ԋp)
            if (CheckBarrier(targetCoreSc) || CheckArmor(targetCoreSc)) return;
            //�����Ȃ��Ȃ璼�ږ{�̂Ƀ_���[�W��^����
            targetCoreSc.Damage((int)(attackPow * attackRate));
        }
    }

    bool CheckBarrier(CoreScript targetCoreSc)
    {
        //�@�̂��o���A�𐶐����Ă��邩�m�F
        if (targetCoreSc.barrierManager != null)
        {
            //�o���A�𐶐��ς݂̎���Ture
            if (targetCoreSc.barrierManager.CheckBarrier()) return true;
        }
        return false;
    }

    bool CheckArmor(CoreScript targetCoreSc)
    {
        //�@�̂ɕ������b�����Ă��邩�m�F
        if (targetCoreSc.barrierManager != null)
        {
            //�@�̂ɕ������b�����Ă��鎞��True
            if (targetCoreSc.barrierManager.CheckBarrier()) return true;
        }
        return false;
    }
}
