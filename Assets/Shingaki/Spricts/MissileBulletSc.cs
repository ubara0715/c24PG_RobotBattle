using System;
using System.Xml.Serialization;
using UnityEngine;


public class MissileBulletSc : MonoBehaviour
{
    public string masterName;   //持ち主の名前

    Rigidbody rb;               //弾のRigidbody
    GameObject bulletObj;       //弾丸Object
    GameObject explosionObj;    //爆発Object
    GameObject targetObj;       //追尾対象

    [Header("初速度"),SerializeField, Range(1, 100)] float initalSpeed;
    [Header("加速度"),SerializeField, Range(1, 100)] float accel; 
    [Header("最高速度"),SerializeField, Range(1, 100)] float accelLimit = 10;
    [Header("ホーミング性能"), SerializeField, Range(0, 3)] float tracking;  
    [Header("爆発範囲(m)"),SerializeField, Range(3f, 30)] float explosionArea = 10;
    [Header("加速開始時間"),SerializeField,Range(0, 1)] float startAccelTime = 0.2f;
    [Header("ホーミング可能距離"), SerializeField] float trackingLimitDistance = 10;
    [Header("消失までの時間"),SerializeField] float destroyTimeLimit = 10;
    [Header("爆発判定残留時間"),SerializeField] float explosionTime = 0.4f;

    int attackPow = 0;          //攻撃力
    float moveSpeed = 0;        //速度
    bool isTracking = false;    //追跡中
    bool isAccel = false;       //加速中
    float attackRate = 1;       //攻撃力倍率

    public int sum_weight;

    public void SetBullet(GameObject target,string name,bool twin = false,bool multiple = false)
    {
        targetObj = target;
        masterName = name;
        if (twin) attackRate = 0.8f;
        if (multiple) attackRate = 0.5f;
    }

    public int GetWeght()
    {
        //重さを返す (爆発力*10 + 追跡率 + 加速率 + 速度)/10
        return (int)((explosionArea * 20 + tracking * 33 + accel + initalSpeed)/(10));
    }

    [ContextMenu("SetSumWeight")]
    public void SetSumWeight()
    {
        sum_weight = GetWeght();
    }

    public int GetDamage()
    {
        return attackPow;
    }


    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        //消失までのカウント
        Invoke(nameof(HitObject), destroyTimeLimit);

        //追跡実行までのカウント(フラグをオンにする)
        Invoke(new Action(() => { isTracking = true; }).Method.Name, explosionArea / 30);

        //加速開始までのカウント(フラグをオンにする)
        Invoke(new Action(() => { isAccel = true; }).Method.Name, startAccelTime);

        //発射初速度を与える
        rb.velocity = initalSpeed * transform.forward;
    }

    private void FixedUpdate()
    {
        //追尾フラグがオンならば弾をターゲットに追尾させる
        if (isTracking) TrackingBullet();

        //加速フラグがオンならば弾を加速
        if (isAccel) AccelBullet();
    }

    void Init()
    {
        //弾のRigidbodyを追加
        rb = GetComponent<Rigidbody>();

        //子オブジェクトを取得
        bulletObj = transform.GetChild(0).gameObject;
        explosionObj = transform.GetChild(1).gameObject;

        //オブジェクトのアクティブを初期化
        bulletObj.SetActive(true);
        explosionObj.SetActive(false);

        //爆発オブジェクトの範囲を指定の大きさに変更
        explosionObj.transform.localScale = new Vector3(explosionArea, explosionArea, explosionArea);

        //攻撃力を爆発力の10倍に指定(int)
        attackPow = (int)(explosionArea * 10);
    }

    void AccelBullet()
    {
        //弾の速度を取得
        moveSpeed = rb.velocity.magnitude;
        //弾の速度に加速分を追加
        moveSpeed += accel * Time.fixedDeltaTime;
        //もし弾の速度が限界速度より早かったら限界速度を代入
        moveSpeed = Mathf.Min(moveSpeed, accelLimit);
        //弾の前方向に加速分の付与された速度で移動させる
        rb.velocity = moveSpeed * transform.forward;
    }

    void TrackingBullet()
    {
        //ターゲットまでのベクトルを取得
        Vector3 targetVec = targetObj.transform.position - transform.position;

        //距離が一定以上離れていたら追跡をやめる
        if(targetVec.sqrMagnitude > trackingLimitDistance * trackingLimitDistance)
        {
            isTracking = false;
            return;
        }

        //正規化
        targetVec.Normalize();
        //追尾割合を計算して代入
        float step = tracking * Time.fixedDeltaTime;
        //割合に応じてベクトルをターゲットに寄せる
        Vector3 lookVec = Vector3.MoveTowards(transform.forward, targetVec, step);
        //弾を計算したベクトルの方向に回転させる
        transform.rotation = Quaternion.LookRotation(lookVec);
    }

    void HitObject()
    {
        //移動を強制停止
        rb.constraints = RigidbodyConstraints.FreezePosition;

        //オブジェクトのアクティブを反転(爆発)
        bulletObj.SetActive(false);
        explosionObj.SetActive(true);

        //爆発一定時間後に消滅
        Invoke(nameof(DestroyGameObject),explosionTime);
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    public void OnBulletTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //当たった敵機体のCoreScriptを取得
            CoreScript targetCoreSc = other.GetComponent<CoreScript>();
            //もし自分の機体なら無視
            if (targetCoreSc.playerName == masterName) return;
            HitObject();
        }
        else if(other.CompareTag("Ground"))
        {
            HitObject();
        }

        //if (collision.collider.CompareTag("レーザー"))
        //{
        //    HitObject();
        //}
    }

    public void OnExplosionTriggerEnter(Collider other)
    {
        //ダメージ付与
        if (other.CompareTag("Player"))
        {
            //当たった敵機体のCoreScriptを取得
            CoreScript targetCoreSc = other.GetComponent<CoreScript>();
            //機体にバリアや装甲が存在するならダメージを与えない(返却)
            if (CheckBarrier(targetCoreSc) || CheckArmor(targetCoreSc)) return;
            //何もないなら直接本体にダメージを与える
            targetCoreSc.Damage((int)(attackPow * attackRate));
        }
    }

    bool CheckBarrier(CoreScript targetCoreSc)
    {
        //機体がバリアを生成しているか確認
        if (targetCoreSc.barrierManager != null)
        {
            //バリアを生成済みの時はTure
            if (targetCoreSc.barrierManager.CheckBarrier()) return true;
        }
        return false;
    }

    bool CheckArmor(CoreScript targetCoreSc)
    {
        //機体に物理装甲がついているか確認
        if (targetCoreSc.barrierManager != null)
        {
            //機体に物理装甲がついている時はTrue
            if (targetCoreSc.barrierManager.CheckBarrier()) return true;
        }
        return false;
    }
}
