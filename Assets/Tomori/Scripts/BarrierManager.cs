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
    [SerializeField, Header("���o�͈͂̔��a")] float detectionRadius;
    //[SerializeField, Header("�v���C���[�̈ʒu")] Vector3 detectionCenter;  // ���o�̒��S�_�i�v���C���[�̈ʒu)�j
    //[SerializeField, Header("���̃��C���[")] LayerMask bulletLayer;
    BarrierScript barrierScript;

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
        CreateBarrier();//�o���A�𐶐�
        currentBarrier.SetActive(false);//�o���A��false
    }


    void Update()
    {
        //detectionCenter = playerTR.localPosition;

        //bullets = Physics.OverlapSphere(detectionCenter, detectionRadius, bulletLayer);

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

    //�o���A��true�ɂ���
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


    //�v���C���[�̑傫���ɍ��킹���o���A�𐶐�
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
        float uniformScale = maxSize * sizeMultiplier;

        // �e�Ȃ��Ő���
        currentBarrier = Instantiate(barrier, playerTR.position, Quaternion.identity);
        currentBarrier.transform.localScale = Vector3.one * uniformScale;

        detectionRadius = uniformScale * 1.1f;//�o���A�̌��o�͈͂����킹��

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
