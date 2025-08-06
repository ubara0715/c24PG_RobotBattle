using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class EnergyGearScript : MonoBehaviour
{
    [SerializeField,Header("エネルギー弾のPrefabを入れてください")] GameObject energyBullet;
    [Header("エネルギープールが付いたObjectを入れてください")] public EnergyScript energyPool;
    [Header("生成したObjectをまとめておく空のObjectを入れてください")] public GameObject bulletGroup;
    CoreScript core;

    [HideInInspector]public float usedMax = 100.0f;
    [HideInInspector]public float usedMin = 10.0f;
    public float speed = 100.0f;

    // メソッド
    void Start()
    {
        core = transform.parent.GetComponent<CoreScript>();
        core.AddWeight((int)usedMax);
    }

    // public関数
    public void ShotEnergy(int usedEnergy,GameObject target)
    {
        // ダメージ計算、減算はBulletの方で行う
        // 値を渡してAddForceを加えるだけ

        if(usedEnergy < usedMin || usedEnergy > usedMax)
        {
            return; // 範囲外なら撃たない
        }

        // 距離計算してエネルギー使用量を最低限に抑えたい
        float two_distance = Vector3.Distance(gameObject.transform.position, target.transform.position);

        // 方向転換
        transform.LookAt(target.transform);
        if(transform.eulerAngles.y < -90.0f || transform.eulerAngles.y > 90.0f)
        {
            energyPool.energyAmount += usedEnergy; // 方向転換が90度以上ならエネルギーを消費しない
            return; // 方向転換が90度以上なら撃たない
        }

        // クローン作成と設定、Componentの取得
        GameObject clone = EnergyBullet_clone();

        Rigidbody rb_clone = clone.GetComponent<Rigidbody>();
        EnergyBulletScript bulletSc = clone.GetComponent<EnergyBulletScript>();
        bulletSc.usedEnergy_clone = usedEnergy;
        bulletSc.speed_clone = speed;
        clone.transform.parent = bulletGroup.transform;

        // 撃ちだす
        Shot(rb_clone);
    }

    // privert関数
    public GameObject EnergyBullet_clone()
    {
        //Vector3 instPos = new Vector3(transform.parent.localPosition.x + transform.position.x, transform.parent.localPosition.y + transform.position.y, transform.parent.localPosition.z + instantiatePos);
        Vector3 instPos = (transform.position + transform.forward);

         GameObject energyBullet_clone =
            Instantiate(
                energyBullet,
                instPos, //AddForceなので生成位置にモノがあるとうまく発射できない、のでもし発射しないなぁと思ったら調整してね
                Quaternion.identity
                );

        return energyBullet_clone;
    }

    void Shot(Rigidbody rb)
    {
        rb.AddForce(transform.forward * (speed / Time.fixedDeltaTime), ForceMode.Force);
    }
}
