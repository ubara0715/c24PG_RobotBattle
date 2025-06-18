using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsAtackScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //Bulletなどの攻撃する球を消すための処理（必要に応じて別スクリプトに移動してほしい）
        if (collision.gameObject.tag == "Barrier" || collision.gameObject.tag == "Player")
        {
            gameObject.SetActive(false);
        }
    }
}
