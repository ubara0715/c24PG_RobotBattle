using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Physical_Armor: MonoBehaviour
{
    //public float scaleX =1;
    //public float scaleY =1;
    //public float scaleZ = 1;

    public int defensePoint;
    public int defenseValue;

    public float weightPoint;
    public float armorWeight;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        // transform.localScale = new Vector3(2, 3, 1);
        float scaleX = transform.localScale.x;
        float scaleY = transform.localScale.y;
        float scaleZ = transform.localScale.z;

        defenseValue =
            (int) (scaleZ * (float)defensePoint);

        armorWeight = (int) (scaleX * scaleY * scaleZ * weightPoint);

        if (weightPoint<0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag== "Bullet")
        {
            defensePoint-=1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
