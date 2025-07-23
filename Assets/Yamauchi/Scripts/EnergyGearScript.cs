using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class EnergyGearScript : MonoBehaviour
{
    [SerializeField,Header("エネルギー弾のPrefabをアタッチしてください")] GameObject energyBullet;
    [Header("エネルギープールが付いたObjectをアタッチしてください")] public EnergyScript energyPool;
    [Header("上手く発射できないなぁと思ったら調整してください")] public float instantiatePos = 1.1f;
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

        // 距離計算してエネルギー使用量を最低限に抑えたい
        float two_distance = Vector3.Distance(gameObject.transform.position, target.transform.position);

        // クローン作成と設定、Componentの取得
        GameObject clone = EnergyBullet_clone();

        Rigidbody rb_clone = clone.GetComponent<Rigidbody>();
        EnergyBulletScript bulletSc = clone.GetComponent<EnergyBulletScript>();
        bulletSc.usedEnergy_clone = usedEnergy;
        bulletSc.speed_clone = speed;
        clone.transform.parent = gameObject.transform;

        // 撃ちだす
        Shot(rb_clone);
    }

    // privert関数
    GameObject EnergyBullet_clone()
    {
        Vector3 instPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + instantiatePos);

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
