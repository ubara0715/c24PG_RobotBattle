using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsAtackScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Bulletなどの攻撃する球を消すための処理（必要に応じて別スクリプトに移動してほしい）
        if(other.gameObject.tag == "Barrier" || other.gameObject.tag == "Player")
        {
            gameObject.SetActive(false);
        }
    }
}
