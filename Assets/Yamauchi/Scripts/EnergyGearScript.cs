using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class EnergyGearScript : MonoBehaviour
{
    [SerializeField,Header("エネルギー弾のPrefabをアタッチしてください")] GameObject energyBullet;
    [Header("エネルギープールが付いたObjectをアタッチしてください")] public EnergyScript energyPool;
    [Header("上手く発射できないなぁと思ったら調整してください")] public float instantiatePos = 1.1f;

    // public関数

    public void ShotEnergy(float usedEnergy,GameObject target)
    {
        // ダメージ計算、減算はBulletの方
        // 値を渡してAddForceを加えるだけか？
        //Debug.Log("起動！");

        // 距離計算してエネルギー使用量を最低限に抑えたい
        float two_distance = Vector3.Distance(gameObject.transform.position, target.transform.position);

        // クローン作成と設定、Componentの取得
        GameObject clone = EnergyBullet_clone();

        Rigidbody rb_clone = clone.GetComponent<Rigidbody>();
        EnergyBulletScript bulletSc = clone.GetComponent<EnergyBulletScript>();
        bulletSc.usedEnergy = usedEnergy;
        clone.transform.parent = gameObject.transform;

        // 撃ちだす
        Shot(rb_clone);
    }


    // privert関数

    // ただのInstantiate、コード長くなるから別で書いた
    GameObject EnergyBullet_clone()
    {
        GameObject energyBullet_clone =
            Instantiate(
                energyBullet,
                gameObject.transform.forward * 1.1f, //AddForceなので生成位置にモノがあるとうまく発射できない、のでもし発射しないなぁと思ったら調整してね
                Quaternion.identity
                );

        return energyBullet_clone;
    }

    void Shot(Rigidbody rb)
    {
        rb.AddForce(transform.forward * (10000.0f/*←遅いと思ったらここの値を増やしてもらって*/ / (1.0f/Time.deltaTime)), ForceMode.Impulse);
    }
}
