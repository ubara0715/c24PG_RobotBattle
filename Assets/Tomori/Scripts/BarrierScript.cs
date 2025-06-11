using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScript : MonoBehaviour
{
    bool isBarrier = false;
    [SerializeField] GameObject barrier;

    [SerializeField] int barrierHP = 150;//仮のバリアHP
    [SerializeField] float energyPool = 10000;//仮のエネルギープール量
    public float playerHP = 500;//仮のプレイヤーHP

    //下記3つのダメージは別の別の人が作ったダメージに応じて変更
    [SerializeField] int bulletAtack = 200;//仮の実弾兵器のダメージ
    [SerializeField] int energyAtack = 300;//仮のエネルギー兵器のダメージ
    [SerializeField] int missileAtack = 400;//仮のミサイル兵器のダメージ
    [SerializeField] int contactAtack = 300;//仮の接触攻撃のダメージ
    [SerializeField] int contactBonus = 100;//接触攻撃のボーナスポイント（仕様書に書かれていたため）

    [SerializeField] Transform playerTR;
    [SerializeField] float sizeMultiplier = 1.2f;

    GameObject currentBarrier;

    [SerializeField,Header("プレイヤーの位置")] Vector3 detectionCenter;  // 検出の中心点（プレイヤーの位置)）
    [SerializeField,Header("検出範囲の半径")] float detectionRadius;
    [SerializeField,Header("球のレイヤー")] LayerMask bulletLayer;

    Collider[] bullets;

    public enum AttackType
    {
        Bullet,　　  //担当者：波照間煌斗
        Energy,　　　//担当者：山内亜音
        Missile,　　 //担当者：新垣大空
        Contact      //担当者：エイデン
    }

    void Start()
    {
        CreateBarrier();//バリアを生成
        currentBarrier.SetActive(false);//バリアをfalse
    }


    void Update()
    {
        detectionCenter = playerTR.localPosition;

        // 指定した範囲内のすべての敵コライダーを検出
        bullets = Physics.OverlapSphere(detectionCenter, detectionRadius, bulletLayer);

        SetBarrier();
    }


    //バリアをtrueにする
    void SetBarrier()
    {
        float diameter = currentBarrier.transform.localScale.x;
        float cost = diameter * barrierHP;

        if (bullets.Length > 0)
        {
            if (!isBarrier && energyPool >= cost)
            {
                energyPool -= cost;
                isBarrier = true;
                currentBarrier.SetActive(true);
            }
        }
        else
        {
            if (isBarrier)
            {
                isBarrier = false;
                currentBarrier.SetActive(false);
            }
        }
    }


    //プレイヤーの大きさに合わせたバリアを生成
    void CreateBarrier()
    {
        Renderer[] renderers = playerTR.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0) return;

        Bounds combinedBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        float maxSize = Mathf.Max(combinedBounds.size.x, combinedBounds.size.y, combinedBounds.size.z);

        currentBarrier = Instantiate(barrier, playerTR.position, Quaternion.identity, playerTR);
        currentBarrier.transform.localScale = Vector3.one * maxSize * sizeMultiplier;

        detectionRadius = maxSize * sizeMultiplier * 1.1f;//バリアの大きさに合わせて検出範囲の半径も大きく
    }


    /*public int ReceiveDamage(AttackType attackType, int damageAmount)
    {
        int actualDamage = 0;

        switch (attackType)
        {
            case AttackType.Bullet:
                if (isBarrier)
                {
                    if (barrierHP <= damageAmount)
                    {
                        actualDamage = damageAmount - barrierHP;
                        playerHP -= actualDamage;
                    }
                    // else → バリアで完全防御（ダメージ0）
                }
                else
                {
                    actualDamage = damageAmount;
                    playerHP -= actualDamage;
                }
                break;

            case AttackType.Energy:
                if (!isBarrier)
                {
                    actualDamage = damageAmount;
                    playerHP -= actualDamage;
                }
                break;

            case AttackType.Missile:
                if (isBarrier)
                {
                    if (barrierHP <= damageAmount)
                    {
                        actualDamage = damageAmount - barrierHP;
                        playerHP -= actualDamage;
                    }
                }
                else
                {
                    actualDamage = damageAmount;
                    playerHP -= actualDamage;
                }
                break;

            case AttackType.Contact:
                int totalContactDamage = damageAmount + contactBonus;
                if (isBarrier)
                {
                    if (barrierHP <= totalContactDamage)
                    {
                        actualDamage = totalContactDamage - barrierHP;
                        playerHP -= actualDamage;
                    }
                }
                else
                {
                    actualDamage = totalContactDamage;
                    playerHP -= actualDamage;
                }
                break;
        }

        return actualDamage;
    }*/


    //ダメージを計算するためにtriggerを使う（playerのisTriggerをtrue）
    private void OnTriggerEnter(Collider other)
    {
        //実弾兵器から発射される球のタグ
        if (other.gameObject.tag == "Bullet")
        {
            var core = other.GetComponent<CoreScript>();

            if (isBarrier)
            {
                if (barrierHP <= bulletAtack)
                {
                    playerHP = playerHP + (barrierHP - bulletAtack);
                }
                else if (barrierHP >= bulletAtack)
                {
                    //playerHP = playerHP;（何もしない）
                }
            }
            else
            {
                playerHP = playerHP - bulletAtack;
            }
        }

        //エネルギー兵器から発射される球のタグ
        if (other.gameObject.tag == "Energy")
        {
            if (isBarrier)
            {
                //playerHP = playerHP（何もしない）;
            }
            else
            {
                playerHP = playerHP - energyAtack;
            }
        }

        //ミサイル兵器から発射されるミサイルのタグ
        if(other.gameObject.tag == "Missile")
        {
            if (isBarrier)
            {
                if (barrierHP <= missileAtack)
                {
                    playerHP = playerHP + (barrierHP - missileAtack);
                }
                else if (barrierHP >= missileAtack)
                {
                    //playerHP = playerHP;（何もしない）
                }
            }
            else
            {
                playerHP = playerHP - missileAtack;
            }
        }

        //接触攻撃を検知するためのタグ（場合によっては消す or 変更）
        /*if(other.gameObject.tag == "Contact")
        {
            if (isBarrier)
            {
                if (barrierHP <= contactAtack + contactBonus)
                {
                    playerHP = playerHP + (barrierHP - (contactAtack + contactBonus));
                }
                else if (barrierHP >= contactAtack + contactBonus)
                {
                    //playerHP = playerHP;（何もしない）
                }
            }
            else
            {
                playerHP = playerHP - missileAtack;
            }
        }*/
    }
}

