using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMoveScript : MonoBehaviour
{
    public CoreScript coreScript;

    [SerializeField, Header("エネルギー管理スクリプト")]
    EnergyScript energyScript;

    [SerializeField]
    GameObject sphere;//テスト用

    //ロボットのRigidbody
    Rigidbody robotRB;
    
    [SerializeField, Header("ジャンプ力")]
    float jumpForce = 1.0f;
    [SerializeField, Header("移動力")]
    float moveForce = 1.0f;
    [SerializeField, Header("回転力")]
    float rotateForce = 1.0f;
    [SerializeField, Header("最高速度")]
    float maxSpeed = 1.0f;

    //回転制御bool
    bool isRotate = false;

    void Start()
    {
        robotRB = GetComponent<Rigidbody>();

        SetMass();

        //テスト用
        StartCoroutine(MoveTarget(sphere));
        //StartCoroutine(RotateRobot(sphere));
    }

    void Update()
    {
        //テスト用
        if (Input.GetKeyDown(KeyCode.Space)) MoveUp();
    }

    //機体の質量を設定
    void SetMass()
    {
        //CoreScriptの重量変数をロボットの質量とする
        robotRB.mass = coreScript.weight;
    }

    public void MoveUp()//ジャンプ
    {
        if (energyScript.UseEnergy(1.0f))
        {
            robotRB.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    //ターゲットへの移動
    public IEnumerator MoveTarget(GameObject targetOBJ)
    {
        //条件は後々変更
        while (Mathf.Abs(targetOBJ.transform.position.x - transform.position.x) > 1
            || Mathf.Abs(targetOBJ.transform.position.z - transform.position.z) > 1)
        {
            if (energyScript.UseEnergy(0.01f))
            {
                Vector3 direction = targetOBJ.transform.position - transform.position;

                //自分から相手の方向にAddForce
                robotRB.AddForce(direction.normalized * moveForce, ForceMode.Force);
                
                //速度チェック
                CheckVelocity();
                
                //回転中でないとき
                if (!isRotate)
                {
                    isRotate = true;

                    StartCoroutine(RotateRobot(targetOBJ));
                }
            }  

            yield return null;
        }

        robotRB.velocity = Vector3.zero;
    }

    //移動速度の調整
    void CheckVelocity()
    {
        //ロボットのxy平面の速度を計算
        float speed = Mathf.Sqrt(Mathf.Pow(robotRB.velocity.x, 2) + Mathf.Pow(robotRB.velocity.z, 2));
        
        //移動速度上限チェック
        if (speed > maxSpeed)
        {
            robotRB.velocity = new Vector3(
                robotRB.velocity.x / (speed / maxSpeed),
                robotRB.velocity.y,
                robotRB.velocity.z / (speed / maxSpeed)
                );
        }
    }

    //ロボットの回転
    IEnumerator RotateRobot(GameObject targetOBJ)
    {
        Vector3 direction = targetOBJ.transform.position - transform.position;
        
        while (Vector3.Angle(transform.forward, direction) > 10)
        {
            if (energyScript.UseEnergy(0.01f))
            {
                direction = targetOBJ.transform.position - transform.position;

                float angle = Vector3.SignedAngle(transform.forward, direction, transform.up);
                
                //角度が小さい方に回転
                robotRB.AddTorque(direction * -Mathf.Sign(angle) * rotateForce, ForceMode.Force);   
            }

            yield return null;
        }

        robotRB.angularVelocity = Vector3.zero;

        isRotate = false;
    }
}
