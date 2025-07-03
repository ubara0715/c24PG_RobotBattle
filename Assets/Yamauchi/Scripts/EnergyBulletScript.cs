using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBulletScript : MonoBehaviour
{
    // パーティクル修正、カラー変更

    // ダメージ計算
    float range = 0;
    int damege = 0;
    [HideInInspector] public float usedEnergy_clone;
    [HideInInspector] public float speed_clone;

    // カラー変更
    Color color;
    ParticleSystem ps;

    // メソッド
    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
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
        //Debug.Log("残りエネルギー：" + usedEnergy_clone + "飛距離：" + range);
        if(range / 100 >= 1)
        {
            DistanceDecay();
            range = 0;
        }
    }

    // 振れたら消滅、ダメージを表示
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        Debug.Log(EnergyDamege());
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
        //Debug.Log("ダメージ減少！");
    }

    // 飛距離計算
    void FilghtDistance()
    {
        range += speed_clone * Time.fixedDeltaTime;
        Invoke("FilghtDistance", Time.deltaTime);
    }

    //開発段階
    /*
    void OnCollisionEnter(Collision collision)
    {
        distance = (int)(Vector3.Distance(startPos, transform.position));
        Destroy(gameObject);
    }

    //テスト用、距離減衰はできた
    void OnDestroy()
    {
        //Debug.Log(EnergyDamege());
    }
    */
}
