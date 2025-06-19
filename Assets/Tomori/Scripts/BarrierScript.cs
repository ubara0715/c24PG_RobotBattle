using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScript : MonoBehaviour
{
    [SerializeField] GameObject barrier;

    [SerializeField] int barrierHP = 15;//バリアHP

    [SerializeField] Transform playerTR;
    [SerializeField] float sizeMultiplier = 2f;

    [SerializeField, Header("プレイヤーの位置")] Vector3 detectionCenter;  // 検出の中心点（プレイヤーの位置)）
    [SerializeField, Header("検出範囲の半径")] float detectionRadius;
    [SerializeField, Header("球のレイヤー")] LayerMask bulletLayer;

    [SerializeField] EnergyScript energyScript;

    public CoreScript coreScript;

    Collider[] bullets;

    GameObject currentBarrier;

    float barrierDuration = 2.0f; // バリアを維持する秒数
    float barrierTimer = 0f;

    bool isBarrier = false;

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

        // バリアON中はタイマー減少
        if (isBarrier)
        {
            barrierTimer -= Time.deltaTime;
            if (barrierTimer <= 0f)
            {
                isBarrier = false;
                currentBarrier.SetActive(false);
            }
        }
    }


    //バリアをtrueにする
    void SetBarrier()
    {
        float diameter = currentBarrier.transform.localScale.x;
        float cost = diameter * barrierHP;

        if (bullets.Length > 0)
        {
            if (!isBarrier && energyScript.UseEnergy(cost + 50))
            {
                isBarrier = true;
                currentBarrier.SetActive(true);
                barrierTimer = barrierDuration; // バリアを一定時間維持
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
    private void OnCollisionEnter(Collision collision)
    {
        int damage = 0;

        //実弾兵器から発射される球のタグ
        if (collision.gameObject.tag == "Bullet")
        {
            damage = collision.gameObject.GetComponent<BulletScript>().GetDamage();
            if (isBarrier)
            {
                damage = damage - barrierHP;
                damage = damage < 0 ? 0 : damage;
                coreScript.Damage(damage);
            }
            else
            {
                coreScript.Damage(damage);
            }

            collision.gameObject.SetActive(false);
        }

        //エネルギー兵器から発射される球のタグ
        if (collision.gameObject.tag == "Energy")
        {
            damage = collision.gameObject.GetComponent<EnergyBulletScript>().EnergyDamege();

            if (isBarrier)
            {
                coreScript.Damage(0);
            }
            else
            {
                coreScript.Damage(damage);
            }

            collision.gameObject.SetActive(false);
        }

        //ミサイル兵器から発射されるミサイルのタグ
        if (collision.gameObject.tag == "Missile")
        {
            damage = collision.gameObject.GetComponent<MissileBulletSc>().GetDamage();

            if (isBarrier)
            {
                damage = damage - barrierHP;
                damage = damage < 0 ? 0 : damage;
                coreScript.Damage(damage);
            }
            else
            {
                coreScript.Damage(damage);
            }

            collision.gameObject.SetActive(false);
        }
    }
}

