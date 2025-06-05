using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScript : MonoBehaviour
{
    bool isBarrier = false;
    [SerializeField] GameObject barrier;

    [SerializeField] float barrierHP = 100;//仮のバリアHP
    public float playerHP = 500;//仮のプレイヤーHP

    //下記3つのダメージは別の別の人が作ったダメージに応じて変更
    [SerializeField] float bulletAtack = 200;//仮の実弾兵器のダメージ
    [SerializeField] float energyAtack = 300;//仮のエネルギー兵器のダメージ
    [SerializeField] float missileAtack = 400;//仮のミサイル兵器のダメージ
    [SerializeField] float contactAtack = 300;//仮の接触攻撃のダメージ
    [SerializeField] float contactBonus = 100;//接触攻撃のボーナスポイント（仕様書に書かれていたため）

    [SerializeField] Transform playerTR;
    [SerializeField] float sizeMultiplier = 1.2f;

    GameObject currentBarrier;

    [SerializeField,Header("プレイヤーの位置")] Vector3 detectionCenter;  // 検出の中心点（プレイヤーの位置)）
    [SerializeField,Header("変出範囲の半径")] float detectionRadius;
    [SerializeField,Header("球のレイヤー")] LayerMask bulletLayer;

    Collider[] bullets;

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
        if(bullets.Length > 0)
        {
            isBarrier = true;
            currentBarrier.SetActive(true);
        }
        else if(bullets.Length < 0)
        {
            isBarrier = false;
            currentBarrier.SetActive(false);
        }
    }


    //プレイヤーの大きさに合わせたバリアを生成
    void CreateBarrier()
    {
        Renderer playerRenderer = playerTR.GetComponentInChildren<Renderer>();

        Bounds bounds = playerRenderer.bounds;
        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);//一番大きいところをmaxSizeに設定

        // バリア生成
        currentBarrier = Instantiate(barrier, playerTR.position, Quaternion.identity, playerTR);

        // バリアのサイズ調整（元のサイズを基準にスケーリング）
        currentBarrier.transform.localScale = Vector3.one * maxSize * sizeMultiplier;
    }

    
    //ダメージを計算するためにtriggerを使う（playerのisTriggerをtrue）
    private void OnTriggerEnter(Collider other)
    {
        //実弾兵器から発射される球のタグ
        if (other.gameObject.tag == "Bullet")
        {
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
        if (other.gameObject.tag == "EnergyBullet")
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

        //接触攻撃を検知するためのタグ（場合によっては消す）
        if(other.gameObject.tag == "Contact")
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
        }
    }
}

