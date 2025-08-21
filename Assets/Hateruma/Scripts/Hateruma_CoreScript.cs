using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.Purchasing;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Hateruma_CoreScript : CoreScript
{

    public List<RadarSensorObj> radarEnemyObjList = new();
    public List<OpSensorObj> opticalEnemyObjList = new();

    GameObject nearestTargetObj;//一番近い距離の敵
    GameObject nowTargetObj;//今のターゲット
    float updateTimer;//ターゲット更新用

    void Awake()
    {
        energySC = gameObject.GetComponent<EnergyScript>();

        robotMoveScript.coreScript = this;
        if (energyGear != null)
        {
            energyGear.energyPool = energySC;
        }

        rSensor.energyScript = energySC;
        oSensor.energyScript = energySC;

        foreach (var li in LiveGuns)
        {
            li.coreSC = this;
            li.energySC = energySC;
        }
    }

    void Update()
    {
        if (opticalEnemyObjList.Count > 0)
        {
            updateTimer += Time.deltaTime;
            if (updateTimer > 10 || nowTargetObj == null)
            {
                UpdateTarget();
                updateTimer = 0;
            }

            if (LiveGuns.Count > 0)
            {
                foreach (var gun in LiveGuns)
                {
                    StartCoroutine(gun.Fire(nowTargetObj));
                }
            }
            if (energyGear != null)
            {
                energyGear.ShotEnergy(9, nowTargetObj);
            }
        }

        if (hp <= 0)
        {
            gameObject.tag = "Untagged";
            gameObject.SetActive(false);
        }
    }


    void UpdateTarget()
    {
        float nearestDistance = float.MaxValue;//一番近い敵との距離

        for (var i = 0; i < opticalEnemyObjList.Count; i++)
        {
            var dis = opticalEnemyObjList[i].GetDistance(gameObject.transform);
            if (dis < nearestDistance)
            {
                nearestTargetObj = opticalEnemyObjList[i].TargetOBj();
                nearestDistance = dis;
            }
        }

        if (nearestTargetObj != null)
        {
            nowTargetObj = nearestTargetObj;
        }
        else
        {
            nowTargetObj = opticalEnemyObjList[0].TargetOBj();
        }
    }


    /// <summary>
    /// ダメージを受けた際の処理
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    public override void Damage(int damage)
    {
        hp -= damage;
        if (damage >= 5)
        {
            barrierManager.SetBarrier(1f);
        }
    }

    /// <summary>
    /// 重量増加
    /// </summary>
    /// <param name="amount">重量</param>
    public override void AddWeight(int amount)
    {
        weight += amount;
        robotMoveScript.SetMass();
    }

    /// <summary>
    /// 重量減少
    /// </summary>
    /// <param name="amount">重量</param>
    public override void ReduceWeight(int amount)
    {
        weight -= amount;
        robotMoveScript.SetMass();
    }

    public override void OnRadarSensor(List<RadarSensorObj> getTargets, bool isEnter)
    {
        radarEnemyObjList.Clear();

        foreach (var data in getTargets)
        {
            if (data == null) continue;

            try
            {
                if (data.IsEnemy())
                {
                    radarEnemyObjList.Add(data);
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
        opticalEnemyObjList.Clear();

        foreach (var data in getTargets)
        {
            if (data == null) continue;

            try
            {
                if (data.IsEnemy())
                {
                    opticalEnemyObjList.Add(data);
                }
            }
            catch (MissingReferenceException)
            {
                continue;
            }
        }
    }
}
