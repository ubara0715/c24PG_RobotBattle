using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpSensorObj : RadarSensorObj
{
    public OpSensorObj(GameObject lockOnObj) : base(lockOnObj) { }

    /// <summary>
    /// ターゲットとの距離の大きさ求める
    /// </summary>
    /// <param name="myTf">自身のトランスフォーム</param>
    /// <param name="isSqr">Sqrのマグニチュードにするか</param>
    /// <returns>ターゲットとの距離の大きさ</returns>
    public float GetDistance(Transform myTf,bool isSqr = false)
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
}

public class OpticalSensor : MonoBehaviour
{
    public float angle = 45;

    public CoreScript coreScript;

    private Transform coreTf;

    public float sensorSize = 50;

    private List<OpSensorObj> targets = new();

    public List<string> tags = new();

    private void Awake()
    {
        coreTf = transform.parent.GetComponent<Transform>();//本体のトランスフォームを取得
        transform.localScale = Vector3.one * sensorSize;//サイズを設定
        transform.localPosition = Vector3.zero;//ポジションを初期化
    }

    /// <summary>
    /// 一番近いターゲットを指定する
    /// </summary>
    /// <param name="targets">ターゲットのリスト</param>
    /// <param name="myTf">自分のトランスフォーム</param>
    /// <returns>一番近いターゲット</returns>
    public OpSensorObj GetNearestEnemy(List<OpSensorObj> targets, Transform myTf)
    {

        OpSensorObj sample = targets[0];

        foreach(OpSensorObj targetObj in targets)
        {
            if (!targetObj.IsEnemy()) continue;

            if(sample.GetDistance(myTf,isSqr:true) > targetObj.GetDistance(myTf,isSqr : true))
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
        if (tags.Contains(other.gameObject.tag))
        {

            Vector3 posDelta = other.transform.position - coreTf.position;

            float target_angle = Vector3.Angle(transform.forward, posDelta);

            if (target_angle < angle)
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
            if(targets.Find(x => x.EqualGameObj(other.gameObject)) == null)
            {
                //ターゲットリストに含まれ、本体のスクリプトに伝える
                targets.Add(new OpSensorObj(other.gameObject));
                coreScript.OnOpticalSensor(targets, isVisible: true);
            }
        }
        else if(tags.Contains(other.gameObject.tag))
        {
            if (targets.Find(x => x.EqualGameObj(other.gameObject)) != null)
            {
                //ターゲットリストから除外し、本体のスクリプトに伝える
                targets.RemoveAt(targets.FindIndex(x => x.EqualGameObj(other.gameObject)));
                coreScript.OnOpticalSensor(targets, isVisible: false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (tags.Contains(other.gameObject.tag))
        {
            targets.Add(new OpSensorObj(other.gameObject));

            if(CheckIfItCanFire(other))
            {
                coreScript.OnOpticalSensor(targets, isVisible: true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (tags.Contains(other.gameObject.tag))
        {
            if (targets.Find(x => x.EqualGameObj(other.gameObject)) != null)
            {
                targets.RemoveAt(targets.FindIndex(x => x.EqualGameObj(other.gameObject)));

                coreScript.OnOpticalSensor(targets, isVisible: false);
            }
        }
    }
}
