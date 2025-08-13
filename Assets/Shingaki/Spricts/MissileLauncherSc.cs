using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncherSc : MonoBehaviour
{
    public GameObject missilePf;
    MissileBulletSc bulletCon;

    [Header("装弾数"), SerializeField] int bulletCount;
    [Header("発射する弾数"),SerializeField] int bulletNum;
    [Header("連射間隔"), SerializeField] float continuousFiringInterval;
    [Header("発射後クールタイム"),SerializeField] float cooldownTime;


    /// <summary>
    /// ミサイルを生成して発射
    /// </summary>
    /// <param name="target">レーダーで見つけた対象のオブジェクト</param>
    public void Fire(OpSensorObj targetObj)
    {
        //対象無しの場合返却
        if (targetObj == null)
        {
            Debug.Log("発射対象が存在しません");
            return;
        }

        //ミサイル弾の生成
        bulletCon = 
            Instantiate(missilePf, gameObject.transform.position, transform.rotation)
                .GetComponent<MissileBulletSc>();
        //ミサイル弾スクリプトにターゲットオブジェクトを指定
        bulletCon.SetTargetObj(targetObj.TargetOBj());
    }

    /// <summary>
    /// 複数の対象にミサイルを発射
    /// </summary>
    /// <param name="target">レーダーで見つけた対象のオブジェクトリスト</param>
    public void MultiFire(List<GameObject> targetList)
    {
        StartCoroutine(MultiFireIEnum(targetList));
    }
    IEnumerator MultiFireIEnum(List<GameObject> targetList)
    {
        //連射間隔を先に定義
        WaitForSeconds wait = new WaitForSeconds(continuousFiringInterval);

        for (int i = 0; i < bulletNum; i++)
        {
            //対象無しの場合返却
            if (targetList.Count == 0)
            {
                Debug.Log("発射対象が存在しません");
                break;
            }

            //ターゲットを順番に指定
            int index = i % targetList.Count;
            GameObject target = targetList[index];

            //ミサイル弾の生成
            bulletCon =
                Instantiate(missilePf, gameObject.transform.position, transform.rotation)
                    .GetComponent<MissileBulletSc>();
            //ミサイル弾スクリプトにターゲットオブジェクトを指定
            bulletCon.SetTargetObj(target);

            //次の連射可能時間まで待機
            yield return wait;
        }
    }
}
