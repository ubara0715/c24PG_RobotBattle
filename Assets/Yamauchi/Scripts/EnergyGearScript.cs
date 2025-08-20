using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class EnergyGearScript : MonoBehaviour
{
    [SerializeField,Header("エネルギー弾のPrefab")] GameObject energyBullet;
    [Header("エネルギープールが付いたObject")] public EnergyScript energyPool;
    [Header("生成したObjectをまとめておく空のObject")] public GameObject bulletGroup;
    CoreScript core;

    //[HideInInspector]
    public int usedMax = 50;
    //[HideInInspector]
    public int usedMin = 10;
    public float speed = 100.0f;

    [SerializeField, Header("クールタイム")]
    float coolTime_initial = 1.0f;
    [SerializeField]
    float coolTime;
    public bool isCoolDown = false;

    // メソッド
    void Start()
    {
        core = transform.parent.GetComponent<CoreScript>();
        core.AddWeight((int)(usedMax * 0.1));
        coolTime = coolTime_initial;
    }

    void Update()
    {
        if(isCoolDown)
        {
            coolTime += 1.0f * Time.deltaTime;
            if(coolTime >= coolTime_initial)
            {
                isCoolDown = false;
            }
        }
    }

    // public関数
    /// <summary>
    /// エネルギー弾を発射する関数、威力変更可能
    /// </summary>
    /// <param name="usedEnergy">威力、数値そのまま威力になる</param>
    /// <param name="target">ターゲット、センサーで感知したオブジェクトを入れる想定</param>
    public void ShotEnergy(int usedEnergy,GameObject target)
    {
        // 指定の範囲外のエネルギー量なら撃たない
        if (usedEnergy < usedMin || usedEnergy > usedMax) return;

        // 方向転換
        transform.LookAt(target.transform);
        if(transform.rotation.y <= -90.0f || transform.rotation.y >= 90.0f) return;

        // クールタイム中なら撃たない
        if (isCoolDown) return;

        // エネルギーがないなら撃たない、あるなら減らす
        if (!energyPool.UseEnergy(usedEnergy)) return;

        // クローン作成と設定、Componentの取得
        GameObject clone = EnergyBullet_clone();

        Rigidbody rb_clone = clone.GetComponent<Rigidbody>();
        EnergyBulletScript bulletSc = clone.GetComponent<EnergyBulletScript>();

        bulletSc.usedEnergy_clone = usedEnergy;
        bulletSc.speed_clone = speed;
        bulletSc.masterName = core.playerName;

        clone.transform.parent = bulletGroup.transform;
        clone.name = "EnergyBullet_" + core.playerName;
        
        // 撃ちだす
        Shot(rb_clone);

        // クールタイム
        coolTime = 0.0f;
        isCoolDown = true;
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
