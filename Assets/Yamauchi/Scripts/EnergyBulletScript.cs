using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBulletScript : MonoBehaviour
{
    // privert関数

    // 距離減衰をリアルタイムで減少させたい

    float duration = 0;

    Vector3 startPos;
    float distance = 0;

    [HideInInspector] public float usedEnergy;

    Color color;
    ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        startPos = transform.position;
    }

    public int EnergyDamege()
    {
        if ((int)(distance / 100) >= 1)
        {
            for (int _ = 0; _ <= (int)(distance / 100); _++)
            {
                DistanceDecay();
            }
        }

        return (int)usedEnergy;
    }

    void Update()
    {
        duration += Time.deltaTime;

        if(usedEnergy <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        distance = (int)(Vector3.Distance(startPos, transform.position));
        Destroy(gameObject);
    }

    //テスト用、距離減衰はできた
    void OnDestroy()
    {
        //Debug.Log(EnergyDamege());
    }

    // 距離減衰用の関数、動くのは確認済み
    void DistanceDecay()
    {
        usedEnergy = (int)(usedEnergy * 0.8f);
        //Debug.Log("ダメージ減少！");
    }
}
