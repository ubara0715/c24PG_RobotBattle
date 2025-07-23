using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrierStrength : MonoBehaviour
{
    [SerializeField] Slider barrierSlider;
    [SerializeField] BarrierManager barrierManager;

    // Start is called before the first frame update
    void Start()
    {
        barrierSlider.value = 1;
    }

    // Update is called once per frame
    void Update()
    {
        barrierManager.barrierHP = (int)barrierSlider.value;
    }
}
