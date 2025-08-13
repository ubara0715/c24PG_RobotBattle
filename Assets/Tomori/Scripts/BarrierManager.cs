using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    GameObject currentBarrier;

    public int barrierHP = 15;
    public bool isBarrier = false;
    [SerializeField] EnergyScript energyScript;

    Transform playerTR;
    [SerializeField] GameObject barrier;
    float sizeMultiplier = 2f;
    [SerializeField, Header("検出範囲の半径")] float detectionRadius;
    [SerializeField] BarrierScript barrierScript;

    public CoreScript coreScript;

    public const int TISEBA = 10;

    [SerializeField] float barrierTimer = 0f;
    [SerializeField] float barrierCoolTime = 3f;

    private void Awake()
    {
        playerTR = transform.parent;
        energyScript = transform.parent.GetComponent<EnergyScript>();
        coreScript = transform.parent.GetComponent<CoreScript>();
    }

    void Start()
    {
        CreateBarrier();
        currentBarrier.SetActive(false);
    }

    void Update()
    {
        if (!isBarrier)
        {
            barrierTimer += Time.deltaTime;
        }

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

    public void SetBarrier(float time = 2f)
    {
        // クールタイム中は発生できない
        if (barrierTimer < barrierCoolTime)
            return;

        float diameter = currentBarrier.transform.localScale.x;
        float initialCost = diameter * barrierHP / TISEBA;

        if (energyScript.UseEnergy(initialCost))
        {
            isBarrier = true;
            currentBarrier.SetActive(true);
            Invoke(nameof(FalseBarrier), time);
        }
        else
        {
            isBarrier = false;
        }
    }

    void KeepBarrier()
    {
        barrierTimer += Time.deltaTime;
        float diameter = currentBarrier.transform.localScale.x;
        float costPerSecond = diameter * barrierHP / TISEBA;

        if (energyScript.UseEnergy(costPerSecond * Time.deltaTime))
        {
            isBarrier = true;
        }
        else
        {
            FalseBarrier();
        }
    }

    void CreateBarrier()
    {
        // 全Renderer取得
        Renderer[] allRenderers = playerTR.GetComponentsInChildren<Renderer>();
        List<Renderer> filteredRenderers = new List<Renderer>();

        // 「RadarSensor」「OpticalSensor」配下は除外
        foreach (var rend in allRenderers)
        {
            Transform current = rend.transform;
            bool ignore = false;

            while (current != null && current != playerTR)
            {
                string name = current.gameObject.name;
                if (name == "RadarSensor" || name == "OpticalSensor")
                {
                    ignore = true;
                    break;
                }
                current = current.parent;
            }

            if (!ignore)
            {
                filteredRenderers.Add(rend);
            }
        }

        if (filteredRenderers.Count == 0) return;

        // Boundsを合成
        Bounds combinedBounds = filteredRenderers[0].bounds;
        for (int i = 1; i < filteredRenderers.Count; i++)
        {
            combinedBounds.Encapsulate(filteredRenderers[i].bounds);
        }

        // 最大サイズを取得してバリアサイズ計算
        float maxSize = Mathf.Max(combinedBounds.size.x, combinedBounds.size.y, combinedBounds.size.z);
        float uniformScale = maxSize * sizeMultiplier;

        float hpMultiplier = 1.1f;
        barrierHP = Mathf.Clamp((int)(uniformScale * hpMultiplier), 1, 30);

        currentBarrier = Instantiate(barrier, playerTR.position, Quaternion.identity);
        currentBarrier.transform.localScale = Vector3.one * uniformScale;

        detectionRadius = uniformScale * 1.1f;

        barrierScript = currentBarrier.GetComponent<BarrierScript>();
        barrierScript.coreScript = coreScript;
        barrierScript.barrierManager = this;
    }

    void FalseBarrier()
    {
        barrierTimer = 0f;
        isBarrier = false;
        currentBarrier.SetActive(false);
    }

    public bool CheckBarrier()
    {
        return isBarrier;
    }
}
