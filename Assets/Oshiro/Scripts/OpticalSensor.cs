using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OpSensorObj : RadarSensorObj
{
    public OpSensorObj(GameObject lockOnObj) : base(lockOnObj) { }

    /// <summary>
    /// ターゲットとの距離の大きさ求める
    /// </summary>
    /// <param name="myTf">自身のトランスフォーム</param>
    /// <param name="isSqr">Sqrのマグニチュードにするか</param>
    /// <returns>ターゲットとの距離の大きさ</returns>
    public float GetDistance(Transform myTf, bool isSqr = false)
    {
        Vector3 diffvec = GetVector(myTf);

        if (isSqr)
        {
            return diffvec.sqrMagnitude;
        }
        else
        {
            return diffvec.magnitude;
        }
    }

    /// <summary>
    /// ターゲットがこっちを向いているかどうか
    /// </summary>
    /// <param name="myTf">自分のトランスフォーム</param>
    /// <param name="targetOutLookAngle"></param>
    /// <returns></returns>
    public bool IsLooking(Transform myTf, float targetOutLookAngle = 90)
    {
        const float MAX_ANGLE = 180;//ターゲットの視野の最大値
        targetOutLookAngle = Mathf.Min(MAX_ANGLE, targetOutLookAngle);//最大値より大きかったら最大値にする

        Vector3 targetForward = -targetObj.transform.forward;//ターゲットの背後の方向
        Vector3 toTargetVec = GetVector(myTf);//自分からターゲットへの方向

        float vecDiff = Vector3.Angle(targetForward, toTargetVec);//前述の二つの方向の角度差
        //角度差とターゲットの視野を比較する
        return vecDiff * 2 <= targetOutLookAngle;
    }

    /// <summary>
    /// ターゲットと自分の距離を測る関数
    /// </summary>
    /// <param name="myTf"></param>
    /// <returns>ターゲットと自分の距離</returns>
    public Vector3 GetVector(Transform myTf)
    {
        Vector3 diffvec = targetObj.transform.position - myTf.position;

        return diffvec;
    }

    public GameObject TargetOBj()
    {
        return targetObj;
    }
}

public class OpticalSensor : MonoBehaviour
{
    [Header("視野角"), Range(1, 180)]
    public float angle = 90;

    [Header("センサーの大きさ"), Range(40, 400)]
    public float sensorSize = 50;

    public CoreScript coreScript;

    private Transform coreTf;

    public List<OpSensorObj> targets = new();

    public List<string> tags = new();

    public EnergyScript energyScript;

    private List<OpSensorObj> dummyTargets = new();

    private bool isEnergy = true;

    private bool isReset = true;

    private void Awake()
    {
        coreTf = transform.parent.GetComponent<Transform>();//本体のトランスフォームを取得
        transform.localScale = Vector3.one * sensorSize;//サイズを設定
        transform.localPosition = Vector3.zero;//ポジションを初期化

    }

    private void Update()
    {
        SensorEnergy();

        dummyTargets = new(targets);
        foreach (OpSensorObj target in dummyTargets)
        {
            if (target.IsBullet() && !target.IsActive())
            {
                targets.Remove(target);
            }
        }
        dummyTargets.Clear();
    }

    /// <summary>
    /// 一番近いターゲットを指定する
    /// </summary>
    /// <param name="targets">ターゲットのリスト</param>
    /// <param name="myTf">自分のトランスフォーム</param>
    /// <returns>一番近いターゲット</returns>
    public OpSensorObj GetNearestEnemy(List<OpSensorObj> targets, Transform myTf)
    {

        if (targets.Count == 0) return null;
        OpSensorObj sample = targets[0];

        foreach (OpSensorObj targetObj in targets)
        {
            if (!targetObj.IsEnemy()) continue;

            if (sample.GetDistance(myTf, isSqr: true) > targetObj.GetDistance(myTf, isSqr: true))
            {
                sample = targetObj;
            }
        }

        return sample;
    }

    /// <summary>
    /// レーダーがターゲットを検知
    /// </summary>
    /// <param name="other"></param>
    /// <returns>ターゲットがレーダーに引っかかったかどうか</returns>
    private bool CheckIfItCanFire(Collider other)
    {

        RadarSensorObj sampleObj = new RadarSensorObj(other.gameObject);
        if (sampleObj.IsBullet())
        {
            return !IsMyBullet(other.gameObject);
        }

        if (tags.Contains(other.gameObject.tag) && transform.parent.gameObject != other.gameObject)
        {

            Vector3 posDelta = other.transform.position - coreTf.position;

            float target_angle = Vector3.Angle(transform.forward, posDelta);

            if (target_angle < angle / 2)
            {
                Debug.DrawRay(coreTf.position, posDelta, Color.red);
                if (Physics.Raycast(coreTf.position, posDelta, out RaycastHit hit, sensorSize))
                {
                    if (hit.collider == other)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void OnTriggerStay(Collider other)
    {

        if (CheckIfItCanFire(other))
        {
            if (targets.Find(x => x.EqualGameObj(other.gameObject)) == null)
            {
                //ターゲットリストに含まれ、本体のスクリプトに伝える
                targets.Add(new OpSensorObj(other.gameObject));
                if (isEnergy)
                {
                    coreScript.OnOpticalSensor(targets, isVisible: true);
                }
                else
                {
                    coreScript.OnOpticalSensor(dummyTargets, isVisible: true);
                }
            }
        }
        else if (tags.Contains(other.gameObject.tag))
        {
            if (targets.Find(x => x.EqualGameObj(other.gameObject)) != null)
            {
                //ターゲットリストから除外し、本体のスクリプトに伝える
                targets.RemoveAt(targets.FindIndex(x => x.EqualGameObj(other.gameObject)));
                if (isEnergy)
                {
                    coreScript.OnOpticalSensor(targets, isVisible: false);
                }
                else
                {
                    coreScript.OnOpticalSensor(dummyTargets, isVisible: false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (tags.Contains(other.gameObject.tag))
        {

            if (CheckIfItCanFire(other))
            {
                targets.Add(new OpSensorObj(other.gameObject));

                if (isEnergy)
                {
                    coreScript.OnOpticalSensor(targets, isVisible: true);
                }
                else
                {
                    coreScript.OnOpticalSensor(dummyTargets, isVisible: true);
                }
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

        if (tags.Contains(other.gameObject.tag) && transform.parent.gameObject != other.gameObject)
        {
            if (targets.Find(x => x.EqualGameObj(other.gameObject)) != null)
            {
                targets.RemoveAt(targets.FindIndex(x => x.EqualGameObj(other.gameObject)));

                if (isEnergy)
                {
                    coreScript.OnOpticalSensor(targets, isVisible: false);
                }
                else
                {
                    coreScript.OnOpticalSensor(dummyTargets, isVisible: false);
                }
            }
        }
    }

    /// <summary>
    /// 自分の弾丸かどうかを調べる
    /// </summary>
    /// <param name="otherObj">接触したオブジェクト</param>
    /// <returns>自身の弾丸かどうか</returns>
    private bool IsMyBullet(GameObject otherObj)
    {

        string sampleName = "";
        if (otherObj.TryGetComponent<BulletScript>(out BulletScript bullet))
        {
            sampleName = bullet.masterName;
        }
        else if (otherObj.TryGetComponent<EnergyBulletScript>(out EnergyBulletScript energy))
        {
            sampleName = energy.masterName;
        }
        else if (otherObj.TryGetComponent<MissileBulletSc>(out MissileBulletSc missile))
        {
            sampleName = missile.masterName;
        }
        if (coreScript.playerName == sampleName)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// エネルギー消費
    /// </summary>
    private void SensorEnergy()
    {
        if (energyScript.UseEnergy(sensorSize / 40f * (angle / 90) * Time.deltaTime))
        {
            isEnergy = true;
        }
        else
        {
            isEnergy = false;
            if (isReset)
            {
                coreScript.OnOpticalSensor(dummyTargets, isVisible: false);
                isReset = false;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rayAngle"></param>
    /// <returns></returns>
    public bool CheckWall(float rayAngle)
    {
        Vector3 direction = Quaternion.Euler(new Vector3(0, rayAngle, 0)) * coreTf.forward;
        Debug.DrawRay(coreTf.position, direction * sensorSize / 2, Color.blue);
        if (Physics.Raycast(coreTf.position, direction, out RaycastHit hit, sensorSize / 2))
        {
            if (hit.collider.tag == "Ground")
            {
                return true;
            }
        }
        return false;
    }
}
