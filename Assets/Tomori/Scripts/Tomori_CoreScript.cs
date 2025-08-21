using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Tomori_CoreScript : CoreScript
{
    public RadarSensor radar;
    public OpticalSensor opticalSensor;
    public EnergyGearScript energyGearScript;
    public HealthAndArmor healthAndArmorScript;
    public EnemyHealth enemyHealthScript;

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
        robotMoveScript.coreScript = GetComponent<CoreScript>();
        var scale = gameObject.transform.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        maxHp = hp;
        
    }

    // Update is called once per frame
    void Update()
    {
        // HP50%�ȉ����o���A���W�J���Z���T�[�Œe�����m�����Ƃ�
        if (hp <= maxHp * 0.8f && !barrierManager.isBarrier /*&& radarBulletList.Count > 0*/)
        {
            barrierManager.SetBarrier();
        }


        if (opticalEnemyList.Count > 0)
        {
            var target = opticalEnemyList[0].TargetOBj(); // �Ƃ肠�����ŏ��̓G

            // usedMin+1 �` usedMax-1 �͈̔͂Ń����_���Ɍ��߂�
            int usedEnergy = Random.Range(energyGearScript.usedMin + 1, energyGearScript.usedMax);

            energyGearScript.ShotEnergy(usedEnergy, target);
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

        if(hp <= 0)
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

    public override void ReduceWeight(int reWeight)
    {
        weight -= reWeight;
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
            catch(MissingReferenceException)
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
