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
    [SerializeField, Header("���o�͈͂̔��a")] float detectionRadius;
    //[SerializeField, Header("�v���C���[�̈ʒu")] Vector3 detectionCenter;  // ���o�̒��S�_�i�v���C���[�̈ʒu)�j
    //[SerializeField, Header("���̃��C���[")] LayerMask bulletLayer;
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
        CreateBarrier();//�o���A�𐶐�
        currentBarrier.SetActive(false);//�o���A��false
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

        // ��ԑ傫�����������擾
        float maxSize = Mathf.Max(combinedBounds.size.x, combinedBounds.size.y, combinedBounds.size.z);

        // �T�C�Y�{���������āA�o���A�̃X�P�[��������
        float uniformScale = maxSize * sizeMultiplier;

        //�o���A�̋��x��ݒ�(�ő�30)
        float hpMultiplier = 2.3f;
        barrierHP = Mathf.Clamp((int)(uniformScale * hpMultiplier), 1, 30);

        // �o���A�𐶐�
        currentBarrier = Instantiate(barrier, playerTR.position, Quaternion.identity);
        currentBarrier.transform.localScale = Vector3.one * uniformScale;

        // �o���A�̌��o���a��ݒ�
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
