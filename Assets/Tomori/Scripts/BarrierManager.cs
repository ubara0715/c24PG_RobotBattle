using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    GameObject currentBarrier;

    public int barrierHP = 15;
    Collider[] bullets;
    public bool isBarrier = false;
    [SerializeField] EnergyScript energyScript;

    Transform playerTR;
    [SerializeField] GameObject barrier;
    float sizeMultiplier = 2f;
    [SerializeField, Header("検出範囲の半径")] float detectionRadius;
    //[SerializeField, Header("プレイヤーの位置")] Vector3 detectionCenter;  // 検出の中心点（プレイヤーの位置)）
    //[SerializeField, Header("球のレイヤー")] LayerMask bulletLayer;
    [SerializeField] BarrierScript barrierScript;

    public CoreScript coreScript;

    public const int TISEBA = 100;

    private void Awake()
    {
        playerTR = transform.parent;
        energyScript = transform.parent.GetComponent<EnergyScript>();

        coreScript = transform.parent.GetComponent<CoreScript>();
        
    }

    void Start()
    {
        CreateBarrier();//バリアを生成
        currentBarrier.SetActive(false);//バリアをfalse
    }


    void Update()
    {
        if (isBarrier)
        {
            KeepBarrier();
        }

        if (isBarrier && currentBarrier != null)
        {
            currentBarrier.transform.position = playerTR.position;
            currentBarrier.transform.rotation = playerTR.rotation;
        }
    }

    //バリアをtrueにする
    public void SetBarrier(float time = 2f)
    {
        float diameter = currentBarrier.transform.localScale.x;
        float cost = diameter * barrierHP / TISEBA;

        if(energyScript.energyAmount >= cost)
        {
            isBarrier = true;
            Invoke(nameof(FalseBarrier), time);
        }
    }

    void KeepBarrier()
    {
        float diameter = currentBarrier.transform.localScale.x;
        float cost = diameter * barrierHP / TISEBA;

        if (energyScript.UseEnergy(cost))
        {
            isBarrier = true;
            currentBarrier.SetActive(true);
        }
        else
        {
            FalseBarrier();
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

        // 一番大きい軸方向を取得
        float maxSize = Mathf.Max(combinedBounds.size.x, combinedBounds.size.y, combinedBounds.size.z);

        // サイズ倍率をかけて、バリアのスケールを決定
        float uniformScale = maxSize * sizeMultiplier;

        //バリアの強度を設定(最大30)
        float hpMultiplier = 2.3f;
        barrierHP = Mathf.Clamp((int)(uniformScale * hpMultiplier), 1, 30);

        // バリアを生成
        currentBarrier = Instantiate(barrier, playerTR.position, Quaternion.identity);
        currentBarrier.transform.localScale = Vector3.one * uniformScale;

        // バリアの検出半径を設定
        detectionRadius = uniformScale * 1.1f;

        barrierScript = currentBarrier.GetComponent<BarrierScript>();
        barrierScript.coreScript = coreScript;
        barrierScript.barrierManager = this;
    }


    void FalseBarrier()
    {
        isBarrier = false;
        currentBarrier.SetActive(false);
    }

    public bool CheckBarrier()
    {
        return isBarrier;
    }
}
