using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

public class MaedaCoreScript : CoreScript
{
    public List<RadarSensorObj> radarEnemyList = new();
    public List<OpSensorObj> opticalEnemyList = new();
    public List<RadarSensorObj> radarBulletList = new();
    private LiveGunOriginScript currentGun = null;
    private float gunSelectInterval = 7f;  // ���b���Ƃɏe��؂�ւ��邩
    private float gunSelectTimer = 0f;
    private Rigidbody rb;
    [SerializeField] float maxHp = 500;

    private void Awake()
    {
        //Application.targetFrameRate = 60;

        barrierManager.coreScript = GetComponent<CoreScript>();
        robotMoveScript.coreScript = GetComponent<CoreScript>();
        rSensor.coreScript= GetComponent<CoreScript>();//���[�_�[�Z���T�[
        oSensor.coreScript= GetComponent<CoreScript>();//���w�Z���T�[

        foreach (var li in LiveGuns)
        {
            li.coreSC = this;
            li.energySC = energySC;
        }
    }

    void Start()
    {
        hp = hp / 2;
        maxHp = hp;
    }

    void Update()
    {
        // HP30%�ȉ����o���A���W�J���Z���T�[�Œe�����m�����Ƃ�
        if (hp <= maxHp * 0.5f && !barrierManager.isBarrier /*&& radarBulletList.Count > 0*/)
        {
            barrierManager.SetBarrier();
        }
        if (/*rSensor.IsEnemy() &&*/ opticalEnemyList.Count > 0)
        {
            var target = opticalEnemyList[0].TargetOBj(); // �Ƃ肠�����ŏ��̓G
            int usedEnergy = Random.Range(energyGear.usedMin + 1, energyGear.usedMax);
            energyGear.ShotEnergy(usedEnergy, target);
        }
        if (opticalEnemyList.Count == 0 || LiveGuns.Count == 0) return;
        gunSelectTimer -= Time.deltaTime;
        // �^�C�}�[�؂ꂽ��e�������_���Ő؂�ւ�
        if (gunSelectTimer <= 0f)
        {
            currentGun = LiveGuns[Random.Range(0, LiveGuns.Count)];
            //currentGun = LiveGuns[Random.Range(3,3)];
            gunSelectTimer = gunSelectInterval;
        }
        // ���ݑI�𒆂̏e�Ƀ^�[�Q�b�g�n���Č�������
        if (currentGun != null)
        {
            var target = opticalEnemyList[0].TargetOBj();
            // Fire���Ō��ĂȂ��Ȃ玩���ŃX�L�b�v����̂Ŗ��Ȃ�
            StartCoroutine(currentGun.Fire(target));
        }
        if (hp <= 0)
        {
            transform.position = new Vector3(1000000000000, 100000000, 10000000000000000);
            gameObject.SetActive(false);
        }  
    }

    public override void Damage(int damage)
    {
        hp -= damage;
    }
    public override void AddWeight(int add)
    {
        weight += add;

        robotMoveScript.SetMass();
    }
    public override void ReduceWeight(int reduce)
    {
        weight -= reduce;

        robotMoveScript.SetMass();
    }

    public override void OnRadarSensor(List<RadarSensorObj> getTargets, bool isEnter)
    {
        radarEnemyList.Clear();
        radarBulletList.Clear();
        foreach (var data in getTargets)
        {
            if (data == null) continue;

            try
            {
                if (data.IsEnemy())
                {
                    radarEnemyList.Add(data);
                }
                if (data.IsBullet())
                {
                    radarBulletList.Add(data);
                }
            }
            catch (MissingReferenceException)
            {
                continue;
            }
        }
    }
    public override void OnOpticalSensor(List<OpSensorObj> getTargets, bool isVisible)
    {
        foreach (var data in getTargets)
        {
            if (data == null) continue;

            try
            {
                if (data.IsEnemy())
                {
                    if (isVisible)
                        opticalEnemyList.Add(data);
                    else
                        opticalEnemyList.Remove(data);
                }
            }
            catch (MissingReferenceException)
            {
                continue;
            }
        }
    }
}
