using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBulletScript : MonoBehaviour
{
    public string masterName;

    // 武器スクリプト
    EnergyGearScript energyGear;

    // ダメージ計算
    float range = 0;
    int damege = 0;
    [HideInInspector] public float usedEnergy_clone;
    [HideInInspector] public float speed_clone;

    void Awake()
    {
        energyGear = GetComponentInParent<EnergyGearScript>();
        FilghtDistance();
    }

    void Update()
    {
        // エネルギーなくなったら消滅
        if(usedEnergy_clone <= 0)
        {
            Destroy(gameObject);
        }

        // 距離減衰
        if(range / speed_clone >= 1)
        {
            DistanceDecay();
            range = 0;
        }
    }

    // 振れたら消滅、ダメージを表示
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        Debug.Log("ダメージ(エネルギー弾)："+EnergyDamege());
    }

    // 関数

    // ダメージ算出
    public int EnergyDamege()
    {
        damege = (int)usedEnergy_clone;
        return damege;
    }

    // 距離減衰用の関数、動くのは確認済み
    void DistanceDecay()
    {
        usedEnergy_clone = (int)(usedEnergy_clone * 0.8f);
    }

    // 飛距離計算
    void FilghtDistance()
    {
        range += speed_clone * Time.fixedDeltaTime;
        Invoke("FilghtDistance", Time.deltaTime);
    }
}
