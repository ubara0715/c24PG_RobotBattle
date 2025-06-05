using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScript : MonoBehaviour
{
    bool isBarrier = false;
    [SerializeField] GameObject barrier;

    public float barrierHP = 100;//仮のバリアHP
    public float playerHP = 500;//仮のプレイヤーHP
    [SerializeField] float bulletAtack = 200;//仮の実弾のダメージ

    public Transform player;
    public float sizeMultiplier = 2.0f;

    private GameObject currentBarrier;

    void Start()
    {
        CreateBarrier();
        currentBarrier.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            isBarrier = true;
            SetBarrier();
        }
    }


    //バリアをtrueにする
    void SetBarrier()
    {
        if (isBarrier)
        {
            currentBarrier.SetActive(true);
        }
    }


    //プレイヤーの大きさに合わせたバリアを生成
    void CreateBarrier()
    {
        Renderer playerRenderer = player.GetComponentInChildren<Renderer>();

        Bounds bounds = playerRenderer.bounds;
        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        // バリア生成
        currentBarrier = Instantiate(barrier, player.position, Quaternion.identity, player);

        // バリアのサイズ調整（元のサイズを基準にスケーリング）
        currentBarrier.transform.localScale = Vector3.one * maxSize * sizeMultiplier;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //実弾兵器から発射される球のタグ
        if (collision.gameObject.tag == "Bullet")
        {
            if (isBarrier)
            {
                if (barrierHP < bulletAtack)
                {
                    playerHP = playerHP + (barrierHP - bulletAtack);
                }
                else if (barrierHP > bulletAtack)
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
        if(collision.gameObject.tag == "EnergyBullet")
        {

        }
    }
}

