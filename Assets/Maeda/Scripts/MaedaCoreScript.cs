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
    private float gunSelectInterval = 7f;  // 何秒ごとに銃を切り替えるか
    private float gunSelectTimer = 0f;
    private Rigidbody rb;
    [SerializeField] float maxHp = 500;

    private void Awake()
    {
        //Application.targetFrameRate = 60;

        barrierManager.coreScript = GetComponent<CoreScript>();
        robotMoveScript.coreScript = GetComponent<CoreScript>();
        rSensor.coreScript= GetComponent<CoreScript>();//レーダーセンサー
        oSensor.coreScript= GetComponent<CoreScript>();//光学センサー

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
        // HP30%以下かつバリア未展開かつセンサーで弾を検知したとき
        if (hp <= maxHp * 0.5f && !barrierManager.isBarrier /*&& radarBulletList.Count > 0*/)
        {
            barrierManager.SetBarrier();
        }
        if (/*rSensor.IsEnemy() &&*/ opticalEnemyList.Count > 0)
        {
            var target = opticalEnemyList[0].TargetOBj(); // とりあえず最初の敵
            int usedEnergy = Random.Range(energyGear.usedMin + 1, energyGear.usedMax);
            energyGear.ShotEnergy(usedEnergy, target);
        }
        if (opticalEnemyList.Count == 0 || LiveGuns.Count == 0) return;
        gunSelectTimer -= Time.deltaTime;
        // タイマー切れたら銃をランダムで切り替え
        if (gunSelectTimer <= 0f)
        {
            currentGun = LiveGuns[Random.Range(0, LiveGuns.Count)];
            //currentGun = LiveGuns[Random.Range(3,3)];
            gunSelectTimer = gunSelectInterval;
        }
        // 現在選択中の銃にターゲット渡して撃たせる
        if (currentGun != null)
        {
            var target = opticalEnemyList[0].TargetOBj();
            // Fire内で撃てないなら自動でスキップするので問題なし
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
