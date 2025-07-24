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
    }

    void Update()
    {
        //テスト用
        if (Input.GetKey(KeyCode.Space)) MoveUp();
    }

    //機体の質量を設定(機体の重量が変わるたびにCoreScriptから呼ぶ)
    public void SetMass()
    {
        //CoreScriptの重量変数をロボットの質量とする
        robotRB.mass = coreScript.weight;
    }

    public void MoveUp()//ジャンプ
    {
        robotRB.AddForce(transform.up * jumpForce, ForceMode.Force);

        if (robotRB.velocity.y > maxSpeed)
        {
            robotRB.velocity = new Vector3(
                robotRB.velocity.x,
                maxSpeed,
                robotRB.velocity.z
                );
        }

    }

    //ターゲットへの移動
    public IEnumerator MoveTarget(GameObject targetOBJ)
    {
        //条件は後々変更
        while (true)
        {
            if (transform.position.y - targetOBJ.transform.position.y <= -1
                && energyScript.UseEnergy(0.1f)) 
            {
                MoveUp();
            }

            Vector3 direction = targetOBJ.transform.position - transform.position;

            //ターゲットに近づいたら終了(仮条件)
            if (Mathf.Abs(direction.x) < (transform.localScale.x + targetOBJ.transform.localScale.x) / 2
                && Mathf.Abs(direction.y) < (transform.localScale.y + targetOBJ.transform.localScale.y) / 2
                && Mathf.Abs(direction.z) < (transform.localScale.z + targetOBJ.transform.localScale.z) / 2)
                break;

            direction.y = 0;//xz平面で移動する

            if (Mathf.Abs(direction.x) > (transform.localScale.x + targetOBJ.transform.localScale.x) / 2
                || Mathf.Abs(direction.z) > (transform.localScale.z + targetOBJ.transform.localScale.z) / 2
                && energyScript.UseEnergy(0.01f))
            {
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
            else
            {
                robotRB.velocity = new Vector3(0, robotRB.velocity.y, 0);
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
        while (true) 
        {
            Vector3 direction = targetOBJ.transform.position - transform.position;

            direction.y = 0;//xz平面で回転

            if (energyScript.UseEnergy(0.01f))
            {
                float angle = Vector3.SignedAngle(transform.forward, direction, transform.up);
                
                //ほぼ正面を向いたら終了
                if (Mathf.Abs(angle) < 5) break;

                //角度が小さい方に回転
                robotRB.AddTorque(transform.up * Mathf.Sign(angle) * rotateForce, ForceMode.Force);
            }

            yield return null;
        }

        robotRB.angularVelocity = Vector3.zero;

        isRotate = false;
    }
}
