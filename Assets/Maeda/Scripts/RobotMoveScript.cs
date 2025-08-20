using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMoveScript : MonoBehaviour
{
    public CoreScript coreScript;

    [SerializeField, Header("エネルギー管理スクリプト")]
    EnergyScript energyScript;

    [SerializeField,Header("テスト用の敵オブジェクト")]//後で消す
    GameObject enemy;
    [SerializeField, Header("ランダム移動ターゲット")]
    GameObject targetObj;

    //ロボットのRigidbody
    Rigidbody robotRB;
    
    [SerializeField, Header("最大質量")]
    float maxMass = 0;
    //ロボットの質量
    int mass = 0;

    [SerializeField, Header("ジャンプ力")]
    float jumpForce = 1.0f;
    [SerializeField, Header("移動力")]
    float moveForce = 1.0f;
    [SerializeField, Header("回転力")]
    float rotateForce = 1.0f;

    [Header("制限速度の範囲")]
    [SerializeField, Header("上限")]
    float maxSpeed = 1.0f;
    [SerializeField, Header("下限")]
    float minSpeed = 1.0f;
    //ロボットの制限速度
    float limitSpeed = 0;

    [Header("ターゲット切り替え間隔")]
    [SerializeField, Header("最大")]
    float maxTime = 0;
    [SerializeField, Header("最小")]
    float minTime = 0;

    //回転制御bool
    bool isRotate = false;
    //追従制御bool
    bool isTarget = false;

    //移動中コルーチン
    Coroutine _moveTarget;

    private void Awake()
    {
        robotRB = GetComponent<Rigidbody>();
    }

    void Start()
    {
        //スケールx+y+zを質量とする
        mass = (int)(transform.localScale.x + transform.localScale.y + transform.localScale.z);
        coreScript.AddWeight(mass);
        coreScript.hp = mass * 100;

        //質量に応じて移動速度を計算
        limitSpeed = Mathf.Lerp(maxSpeed, minSpeed, mass / maxMass);

        SetTarget();
    }

    void Update()
    {
        //テスト用(レーダーに敵が移った想定)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isTarget = true;
            StopCoroutine(_moveTarget);
            _moveTarget = StartCoroutine(MoveTarget(enemy));    
        }

        //ターゲットがいないとき
        if (!isTarget)
        {
            SetTarget();
        }
    }

    //ランダム移動のターゲットを決める
    void SetTarget()
    {
        targetObj.transform.position = new Vector3(
                Random.Range(-50, 50),
                Random.Range(1, 3),
                Random.Range(-50, 50)
                );

        isTarget = true;
        _moveTarget = StartCoroutine(MoveTarget(targetObj));
    }

    //機体の質量を設定(機体の重量が変わるたびにCoreScriptから呼ぶ)
    public void SetMass()
    {
        //CoreScriptの重量変数をロボットの質量とする
        robotRB.mass = coreScript.weight;
    }
 
    public void MoveUp()//上移動
    {
        robotRB.AddForce(Vector3.up * jumpForce, ForceMode.Force);

        if (robotRB.velocity.y > limitSpeed)
        {
            robotRB.velocity = new Vector3(
                robotRB.velocity.x,
                limitSpeed,
                robotRB.velocity.z
                );
        }
    }

    //ターゲットへの移動
    public IEnumerator MoveTarget(GameObject targetOBJ)
    {
        float waitTime = Random.Range(minTime, maxTime);
        float startTime = Time.time;

        //条件は後々変更
        while (true)
        {
            //自分とターゲット間のベクトルを計算
            Vector3 direction = targetOBJ.transform.position - transform.position;

            //上方移動
            if (transform.position.y - targetOBJ.transform.position.y < -1
                && energyScript.UseEnergy(2f * Time.deltaTime)) 
            {
                MoveUp();
            }

            //到達チェック(仮条件)
            bool reached = Mathf.Abs(direction.x) <= (transform.localScale.x + targetOBJ.transform.localScale.x) / 2
                && Mathf.Abs(direction.y) <= (transform.localScale.y + targetOBJ.transform.localScale.y) / 2
                && Mathf.Abs(direction.z) <= (transform.localScale.z + targetOBJ.transform.localScale.z) / 2;

            //タイムチェック
            bool timeOut = Time.time - startTime >= waitTime;

            if (reached || timeOut) break;

            //xz平面の移動ベクトルを計算
            Vector3 horizontalDir = new Vector3(direction.x, 0, direction.z);

            if (Mathf.Abs(horizontalDir.x) > (transform.localScale.x + targetOBJ.transform.localScale.x) / 2
                || Mathf.Abs(horizontalDir.z) > (transform.localScale.z + targetOBJ.transform.localScale.z) / 2
                && energyScript.UseEnergy(1f*Time.deltaTime))
            {
                robotRB.AddForce(horizontalDir.normalized * moveForce, ForceMode.Force);

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
                //robotRB.velocity = new Vector3(0, robotRB.velocity.y, 0);
            }

            yield return null;
        }

        robotRB.velocity = Vector3.zero;
        isTarget = false;
    }

    //移動速度の調整
    void CheckVelocity()
    {
        //ロボットのxy平面の速度を計算
        float speed = Mathf.Sqrt(Mathf.Pow(robotRB.velocity.x, 2) + Mathf.Pow(robotRB.velocity.z, 2));
        
        //移動速度上限チェック
        if (speed > limitSpeed)
        {
            robotRB.velocity = new Vector3(
                robotRB.velocity.x / (speed / limitSpeed),
                robotRB.velocity.y,
                robotRB.velocity.z / (speed / limitSpeed)
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

            if (energyScript.UseEnergy(0))
            {
                float angle = Vector3.SignedAngle(transform.forward, direction, transform.up);
                
                //ほぼ正面を向いたら終了
                if (Mathf.Abs(angle) < 5) break;

                //角度が小さい方に回転
                robotRB.AddTorque(transform.up * Mathf.Sign(angle) * rotateForce, ForceMode.Acceleration);
            }

            yield return null;
        }

        robotRB.angularVelocity = Vector3.zero;
        isRotate = false;
    }
}
