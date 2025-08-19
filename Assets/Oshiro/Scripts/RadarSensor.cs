using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSensorObj
{

    protected GameObject targetObj = null;//ターゲットのオブジェクト

    public RadarSensorObj(GameObject lockOnObj)
    {
        this.targetObj = lockOnObj;
    }

    /// <summary>
    /// ターゲットの位置を取得
    /// </summary>
    /// <returns>ターゲットの位置</returns>
    public Vector3 GetPos()
    {
        return targetObj.transform.position;
    }

    /// <summary>
    /// 機体かどうかを調べる
    /// </summary>
    /// <returns>機体であればtrue</returns>
    public bool IsEnemy()
    {
        return targetObj.CompareTag("Player");
    }

    /// <summary>
    /// 弾かどうかを調べる
    /// </summary>
    /// <returns>弾であればtrue</returns>
    public bool IsBullet()
    {
        List<string> tags = new() { "Bullet", "Energy", "Missile" };
        return tags.Contains(targetObj.tag);
    }

    /// <summary>
    /// ターゲットのタグを取得
    /// </summary>
    /// <returns>ターゲットのタグ</returns>
    public string GetTag()
    {
        return targetObj.tag;
    }

    /// <summary>
    /// ターゲットオブジェクトの比較
    /// </summary>
    /// <param name="lossObj">比較対象</param>
    /// <returns>ターゲットと引数が一致するならtrue</returns>
    public bool EqualGameObj(GameObject lossObj)
    {
        return targetObj == lossObj;
    }
}

public class RadarSensor : MonoBehaviour
{

    [Header("センサーの大きさ"), Range(40, 400)]
    public int sensorSize = 40;

    private List<RadarSensorObj> targets = new();

    public CoreScript coreScript;

    public List<string> tags = new();

    public EnergyScript energyScript;

    private List<RadarSensorObj> dummyTargets = new();

    private bool isEnergy = true;

    private bool isReset = true;

    private void Awake()
    {
        transform.localScale = Vector3.one * sensorSize;//サイズを設定
        transform.localPosition = Vector3.zero;//ポジションを初期化
    }

    private void Update()
    {
        SensorEnergy();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        //設定したタグにオブジェクトが含まれていたら
        if (tags.Contains(other.gameObject.tag) && transform.parent.gameObject != other.gameObject)
        {
            //自身の弾丸かどうか
            RadarSensorObj sampleObj = new RadarSensorObj(other.gameObject);
            if (sampleObj.IsBullet())
            {
                if (IsMyBullet(other.gameObject)) return;
            }
            //ターゲットリストに含まれ、本体のスクリプトに伝える
            targets.Add(sampleObj);
            if (isEnergy)
            {
                coreScript.OnRadarSensor(targets, isEnter: true);
            }
            else
            {
                coreScript.OnRadarSensor(dummyTargets, isEnter: true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        RadarSensorObj sampleObj = new RadarSensorObj(other.gameObject);
        if (sampleObj.IsBullet())
        {
            if (IsMyBullet(other.gameObject)) return;
        }

        //設定したタグにオブジェクトが含まれていたら
        if (tags.Contains(other.gameObject.tag) && transform.parent.gameObject != other.gameObject)
        {

            //ターゲットリストから除外し、本体のスクリプトに伝える
            targets.RemoveAt(targets.FindIndex(x => x.EqualGameObj(other.gameObject)));
            if (isEnergy)
            {
                coreScript.OnRadarSensor(targets, isEnter: false);
            }
            else
            {
                coreScript.OnRadarSensor(dummyTargets, isEnter: false);
            }
        }


    }


    /// <summary>
    /// 自身の弾丸かどうかを調べる
    /// </summary>
    /// <param name="otherObj">接触したオブジェクト</param>
    /// <returns>自身の弾丸かどうか</returns>
    private bool IsMyBullet(GameObject otherObj)
    {
        if (otherObj.CompareTag("Bullet"))
        {
            string sampleName = "";
            if (otherObj.TryGetComponent<BulletScript>(out BulletScript bullet))
            {
                //sampleName = bullet.masterName;
            }
            else if (otherObj.TryGetComponent<EnergyBulletScript>(out EnergyBulletScript energy))
            {
                //sampleName = energy.masterName;
            }
            else if (otherObj.TryGetComponent<MissileBulletSc>(out MissileBulletSc missile))
            {
                //sampleName = missile.masterName;
            }
            if (coreScript.playerName == sampleName)
            {
                return true;
            }
        }
        return false;
    }

    private void SensorEnergy()
    {
        if (energyScript.UseEnergy(sensorSize/40f*Time.deltaTime))
        {
            isEnergy = true;
            isReset = true;
        }
        else
        {
            isEnergy = false;
            if (isReset)
            {
                coreScript.OnRadarSensor(dummyTargets, isEnter: false);
                isReset = false;
            }
        }
    }

}
