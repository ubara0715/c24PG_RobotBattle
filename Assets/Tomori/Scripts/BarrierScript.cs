using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScript : MonoBehaviour
{
    public CoreScript coreScript;
    public BarrierManager barrierManager;

    private void Start()
    {
        int Barrier = LayerMask.NameToLayer("Barrier");
        int Floor = LayerMask.NameToLayer("Floor");
        int Player = LayerMask.NameToLayer("Player");

        Physics.IgnoreLayerCollision(Barrier, Floor);
        Physics.IgnoreLayerCollision(Barrier, Player);
    }

    //ダメージを計算するためにtriggerを使う（playerのisTriggerをtrue）
    private void OnCollisionEnter(Collision collision)
    {
        int damage = 0;

        //実弾兵器から発射される球のタグ
        if (collision.gameObject.tag == "Bullet")
        {
            damage = collision.gameObject.GetComponent<BulletScript>().GetDamage();

            damage = damage - barrierManager.barrierHP;
            damage = damage < 0 ? 0 : damage;
            coreScript.Damage(damage);


            collision.gameObject.SetActive(false);
        }

        //エネルギー兵器から発射される球のタグ
        if (collision.gameObject.tag == "Energy")
        {
            damage = collision.gameObject.GetComponent<EnergyBulletScript>().EnergyDamege();


            coreScript.Damage(0);


            collision.gameObject.SetActive(false);
        }

        //ミサイル兵器から発射されるミサイルのタグ
        if (collision.gameObject.tag == "Missile")
        {
            damage = collision.gameObject.GetComponent<MissileBulletSc>().GetDamage();


            damage = damage - barrierManager.barrierHP;
            damage = damage < 0 ? 0 : damage;
            coreScript.Damage(damage);


            collision.gameObject.SetActive(false);
        }
    }
}

