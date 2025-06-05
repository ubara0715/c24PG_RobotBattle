using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsAtackScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Bulletなどの攻撃する球を消すための処理（必要に応じて別スクリプトに移動してほしい）
        if(other.gameObject.tag == "Ballier" || other.gameObject.tag == "Player")
        {
            this.gameObject.SetActive(false);
        }
    }
}
