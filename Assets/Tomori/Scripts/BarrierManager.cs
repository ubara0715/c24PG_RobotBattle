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
    EnergyScript energyScript;

    Transform playerTR;
    [SerializeField] GameObject barrier;
    float sizeMultiplier = 2f;
    [SerializeField, Header("検出範囲の半径")] float detectionRadius;
    [SerializeField, Header("プレイヤーの位置")] Vector3 detectionCenter;  // 検出の中心点（プレイヤーの位置)）
    [SerializeField, Header("球のレイヤー")] LayerMask bulletLayer;
    BarrierScript barrierScript;

    public CoreScript coreScript;

    public const int TISEBA = 30;

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
        detectionCenter = playerTR.localPosition;

        bullets = Physics.OverlapSphere(detectionCenter, detectionRadius, bulletLayer);

        if (isBarrier)
        {
            KeepBarrier();
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

        float maxSize = Mathf.Max(combinedBounds.size.x, combinedBounds.size.y, combinedBounds.size.z);

        currentBarrier = Instantiate(barrier, playerTR.position, Quaternion.identity, playerTR);
        currentBarrier.transform.localScale = Vector3.one * maxSize * sizeMultiplier;

        detectionRadius = maxSize * sizeMultiplier * 1.1f;//バリアの大きさに合わせて検出範囲の半径も大きく

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
