using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncherSc : MonoBehaviour
{
    public GameObject missilePf;
    MissileBulletSc bulletCon;
    CoreScript core;

    [Header("生成した弾のまとめ先(Noneでも大丈夫)"), SerializeField] GameObject bulletGroup;
    [Header("以下の変数はデバッグ以外で変更しないで(装弾数は変えていいかも)")]
    [Header("装弾数"), SerializeField] int bulletCount;
    [Header("一回の射撃で発射する弾数"),SerializeField] int bulletNum;
    [Header("連射間隔"), SerializeField] float continuousFiringInterval;
    [Header("発射後クールタイム"),SerializeField] float cooldownTime;
    [Header("ランチャーの重さ"), SerializeField] int weight;

    [Header("二対"), SerializeField] bool isTwin = false;
    [Header("複数"), SerializeField] bool isMultiple = false;

    int sumWeight;
    int bulletWeight;
    int bulletCountNow;
    bool isCooldown = false;
    bool isEmpty = false;

    private void Awake()
    {
        //コアスクリプト取得
        transform.parent.TryGetComponent(out core);
        //初期弾数を残弾数に代入
        bulletCountNow = bulletCount;
    }

    private void Start()
    {
        //総重量をコアに伝達
        core.AddWeight(GetSumWight());
    }

    int GetSumWight()
    {
        //弾を含めた重さを計算
        sumWeight = weight;
        bulletWeight = missilePf.GetComponent<MissileBulletSc>().GetWeght();
        sumWeight += bulletWeight * bulletCount;
        return sumWeight;
    }

    /// <summary>
    /// 今の残弾数を確認
    /// </summary>
    /// <returns></returns>
    public int GetBulletsCount()
    {
        return bulletCountNow;
    }

    /// <summary>
    /// 弾が空かどうか
    /// </summary>
    /// <returns></returns>
    public bool IsEmptyBullets()
    {
        return isEmpty;
    }

    /// <summary>
    /// クールダウン中かどうか
    /// </summary>
    /// <returns></returns>
    public bool IsCooldown()
    {
        return isCooldown;
    }

    /// <summary>
    /// ミサイルを生成して発射
    /// </summary>
    /// <param name="target">レーダーで見つけた対象のオブジェクト</param>
    public void Fire(OpSensorObj targetObj)
    {
        //弾が無ければ発射できない
        if (isEmpty) return;
        //クールタイム中は発射できない
        if (isCooldown) return;

        //対象無しの場合返却
        if (targetObj == null)
        {
            Debug.Log("発射対象が存在しません");
            return;
        }
        if (isTwin)
        {
            for (int i = 0; i < 2; i++)
            {
                if (bulletCountNow <= 0) break;
                //ミサイル弾の生成
                if(bulletGroup == null)
                {
                    bulletCon =
                    Instantiate(missilePf, gameObject.transform.position, transform.rotation)
                        .GetComponent<MissileBulletSc>();
                }
                else
                {
                    bulletCon =
                    Instantiate(missilePf, gameObject.transform.position, transform.rotation, bulletGroup.transform)
                        .GetComponent<MissileBulletSc>();
                }
                //ミサイル弾スクリプトにターゲットオブジェクトを指定
                bulletCon.SetBullet(targetObj.TargetOBj(), core.playerName, twin: true);
                //ランダムに向きをずらす
                if (i == 0) bulletCon.transform.Rotate(UnityEngine.Random.Range(0f, 3f), UnityEngine.Random.Range(-35f, -20f), 0);
                else bulletCon.transform.Rotate(UnityEngine.Random.Range(0f, 3f), UnityEngine.Random.Range(35f, 20f), 0);
                //残弾を減らす
                bulletCountNow--;
                //重量からミサイル一発分の重量を減らす
                core.ReduceWeight(bulletWeight);
            }
        }
        else
        {
            //ミサイル弾の生成
            if (bulletGroup == null)
            {
                bulletCon =
                Instantiate(missilePf, gameObject.transform.position, transform.rotation)
                    .GetComponent<MissileBulletSc>();
            }
            else
            {
                bulletCon =
                Instantiate(missilePf, gameObject.transform.position, transform.rotation, bulletGroup.transform)
                    .GetComponent<MissileBulletSc>();
            }
            //ミサイル弾スクリプトにターゲットオブジェクトを指定
            bulletCon.SetBullet(targetObj.TargetOBj(), core.playerName);
            //ランダムに向きをずらす
            bulletCon.transform.Rotate(UnityEngine.Random.Range(0f, 3f), UnityEngine.Random.Range(-3f, 3f), 0);
            //残弾を減らす
            bulletCountNow--;
            //重量からミサイル一発分の重量を減らす
            core.ReduceWeight(bulletWeight);
        }

        //クールタイム経過後にフラグを解除
        isCooldown = true;
        Invoke(new Action(() => { isCooldown = false; }).Method.Name, cooldownTime);
        //残弾が無くなったら空フラグをオン
        if(bulletCountNow <= 0) isEmpty = true;
    }

    /// <summary>
    /// 複数の対象にミサイルを発射
    /// </summary>
    /// <param name="target">レーダーで見つけた対象のオブジェクトリスト</param>
    public void MultiFire(List<OpSensorObj> targetList)
    {
        //複数フラグ確認
        if (!isMultiple) return;
        //弾が無ければ発射できない
        if (isEmpty) return;
        //クールタイム中は発射できない
        if (isCooldown) return;
        //実行
        StartCoroutine(MultiFireIEnum(targetList));
    }
    IEnumerator MultiFireIEnum(List<OpSensorObj> targetList)
    {
        //先にロックしている敵を保存
        List<OpSensorObj> targetList_sumple = new(targetList);

        //連射間隔を先に定義
        WaitForSeconds wait = new(continuousFiringInterval);

        for (int i = 0; i < bulletNum; i++)
        {
            //途中で弾がなくなったら終了
            if (isEmpty) break;

            //対象無しの場合返却
            if (targetList_sumple.Count == 0)
            {
                Debug.Log("発射対象が存在しません");
                break;
            }

            //ターゲットを順番に指定
            int index = i % targetList_sumple.Count;
            GameObject target = targetList_sumple[index].TargetOBj();

            //ミサイル弾の生成
            if (bulletGroup == null)
            {
                bulletCon =
                Instantiate(missilePf, gameObject.transform.position, transform.rotation)
                    .GetComponent<MissileBulletSc>();
            }
            else
            {
                bulletCon =
                Instantiate(missilePf, gameObject.transform.position, transform.rotation, bulletGroup.transform)
                    .GetComponent<MissileBulletSc>();
            }
            //ミサイル弾スクリプトにターゲットオブジェクトを指定
            bulletCon.SetBullet(target, core.playerName, multiple: true);
            //ランダムに向きをずらす
            bulletCon.transform.Rotate(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-15f, 15f), 0);

            //残弾を減らす
            bulletCountNow--;
            //重量からミサイル一発分の重量を減らす
            core.ReduceWeight(bulletWeight);

            //残弾が無くなったら空フラグをオン
            if (bulletCountNow < 1) isEmpty = true;

            //次の連射可能時間まで待機
            yield return wait;
        }

        //クールタイム経過後にフラグを解除
        isCooldown = true;
        Invoke(new Action(() => { isCooldown = false; }).Method.Name, cooldownTime);
    }
}
