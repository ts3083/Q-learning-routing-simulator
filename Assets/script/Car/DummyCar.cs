using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.EventSystems.EventTrigger;

public class DummyCar : MonoBehaviour
{
    // 차량 속도 선언 - 10, 15, 20
    private int[] speed = new int[] { 15, 20 };
    private static int init_speed = 0;     // 초기 속도(10m/s)
    public int current_speed = init_speed;      // 현재 차량의 위치에 따른 속도가 다름 => current speed 변수 선언
    private int speedLimit;     // 속도 제한

    public bool signal;     // 차량 정지 및 직진 신호

    public int beforeRotation;      // 회전 이전의 rotation

    public string direction = "null";       // 차량 이동방향
    public Vector3 position;        // 월드 좌표를 기준으로 이동시킬 차량의 위치

    private GameObject carBack;     // 차량 뒷면 트리거

    public int lineNum;     // 차량이 위치한 차

    public int[] routeList;       // dummy 차량의 경로 저장, 다음 RSU에서 이동할 RSU 번호부터 시작
    public int routeIndex = 0;     // dummy 차량이 따르는 경로 index

    public int cur_RSU;        // 현재 RSU
    public int prev_RSU;        // 이전 RSU
    public int next_RSU; // 다음 RSU

    float timer = 0;


    // Start is called before the first frame update
    void Start()
    {
        carBack = transform.GetChild(8).gameObject;     // carBack 게임 오브젝트 가져오기
        BackTriggerSettingBySpeed(init_speed);      // 초기 속도(10m/s)로 CarBack 트리거 설정
        beforeRotation = (int)transform.eulerAngles.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer > 30)
        {
            transform.position += transform.forward * 10 * Time.deltaTime;       // 차량 이동
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // carBack과 충돌하는 경우
        if (other.CompareTag("carBack"))
        {
            BackTriggerSettingBySpeed(0);
        }

        if (other.CompareTag("null"))
        {
            Destroy(gameObject);
            direction = "null";
        }

        if (other.CompareTag("CrossRoad"))
        {
            // 이동방향에 따른 좌표를 받아옴
            transform.position = position;
            if (direction.Contains("left") || direction.Contains("right"))
            {
                new_drive(direction);
            }

            routeIndex++;
            if(routeIndex > routeList.Length - 1)
            {
                routeIndex = 0;
            }
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    // 좁은 도로에서 탈출하는 경우 - 속도 0km/h
    //    if (other.CompareTag("NarrowRoadExit") || other.CompareTag("WideRoadExit")) // 이미 CrossRoad와 만났다면 신호 무시
    //    {
    //        if (signal)
    //        {
    //            BackTriggerSettingBySpeed(init_speed);
    //        }
    //        else
    //        {
    //            BackTriggerSettingBySpeed(0);
    //        }
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("carBack"))
        {
            BackTriggerSettingBySpeed(speedLimit);
        }

        // speed 15 제한 도로에 진입하는 경우, 경사각 X
        if (other.CompareTag("NarrowRoadEnterAngleX"))
        {
            BackTriggerSettingBySpeed(15);
            prev_RSU = cur_RSU;
            cur_RSU = 0;
        }

        // speed 15 제한 도로에 진입하는 경우, 경사각 O
        if (other.CompareTag("NarrowRoadEnterAngleO"))
        {
            BackTriggerSettingBySpeed(15);
            prev_RSU = cur_RSU;
            cur_RSU = 0;
        }

        // speed 20 제한 도로에 진입하는 경우, 경사각 X
        if (other.CompareTag("WideRoadEnter"))
        {
            BackTriggerSettingBySpeed(20);
            prev_RSU = cur_RSU;
            cur_RSU = 0;
        }
    }

    // 차량 후면 트리거(차량간 거리 조절) 위치 조정
    public void BackTriggerSettingBySpeed(int speed_)
    {
        // 초기 속도(init_speed)인 경우
        if (speed_ == init_speed)
        {
            carBack.transform.localPosition = new Vector3(0, 0, -3f);
            current_speed = init_speed;
            speedLimit = init_speed;
        }
        // 속도가 15인 경우
        else if (speed_ == 15)
        {
            carBack.transform.localPosition = new Vector3(0, 0, -5f);
            current_speed = speed[0];
            speedLimit = speed[0];
        }
        // 속도가 30인 경우
        else if (speed_ == 20)
        {
            carBack.transform.localPosition = new Vector3(0, 0, -10f);
            current_speed = speed[1];
            speedLimit = speed[1];

        }
        else // 속도가 0인 경우
        {
            carBack.transform.localPosition = new Vector3(0, 0, -3f);
            current_speed = 0;
            speedLimit = init_speed;
        }
    }

    private void new_drive(string direction)
    {
        if (beforeRotation >= 355 || beforeRotation <= 5)
        {
            // 좌회전
            if (direction.Equals("left"))
            {
                leftMove(270);
            }
            else if (direction.Equals("left45"))
            {
                leftMove(315);
            }
            else if (direction.Equals("left135"))
            {
                leftMove(225);
            }
            // 우회전
            else if (direction.Equals("right"))
            {
                rightMove(90);
            }
            else if (direction.Equals("right45"))
            {
                rightMove(45);
            }
            else if (direction.Equals("right135"))
            {
                rightMove(135);
            }
        }
        else if (beforeRotation >= 40 && beforeRotation <= 50)
        {
            // 좌회전
            if (direction.Equals("left"))
            {
                leftMove(315);
            }
            else if (direction.Equals("left45"))
            {
                leftMove(0);
            }
            else if (direction.Equals("left135"))
            {
                leftMove(270);
            }
            // 우회전
            else if (direction.Equals("right"))
            {
                rightMove(135);
            }
            else if (direction.Equals("right45"))
            {
                rightMove(90);
            }
            else if (direction.Equals("right135"))
            {
                rightMove(180);
            }
        }
        else if (beforeRotation >= 85 && beforeRotation <= 95)
        {
            // 좌회전
            if (direction.Equals("left"))
            {
                leftMove(0);
            }
            else if (direction.Equals("left45"))
            {
                leftMove(45);
            }
            else if (direction.Equals("left135"))
            {
                leftMove(315);
            }
            // 우회전
            else if (direction.Equals("right"))
            {
                rightMove(180);
            }
            else if (direction.Equals("right45"))
            {
                rightMove(135);
            }
            else if (direction.Equals("right135"))
            {
                rightMove(225);
                Debug.Log(90);
            }
        }
        else if (beforeRotation >= 130 && beforeRotation <= 140)
        {
            // 좌회전
            if (direction.Equals("left"))
            {
                leftMove(45);
            }
            else if (direction.Equals("left45"))
            {
                leftMove(90);
            }
            else if (direction.Equals("left135"))
            {
                leftMove(0);
            }
            // 우회전
            else if (direction.Equals("right"))
            {
                rightMove(225);
            }
            else if (direction.Equals("right45"))
            {
                rightMove(180);
            }
            else if (direction.Equals("right135"))
            {
                rightMove(270);
            }
        }
        else if (beforeRotation >= 175 && beforeRotation <= 185)
        {
            // 좌회전
            if (direction.Equals("left"))
            {
                leftMove(90);
            }
            else if (direction.Equals("left45"))
            {
                leftMove(135);
            }
            else if (direction.Equals("left135"))
            {
                leftMove(45);
            }
            // 우회전
            else if (direction.Equals("right"))
            {
                rightMove(270);
            }
            else if (direction.Equals("right45"))
            {
                rightMove(225);
            }
            else if (direction.Equals("right135"))
            {
                rightMove(315);
            }
        }
        else if (beforeRotation >= 220 && beforeRotation <= 230)
        {
            // 좌회전
            if (direction.Equals("left"))
            {
                leftMove(135);
            }
            else if (direction.Equals("left45"))
            {
                leftMove(180);
            }
            else if (direction.Equals("left135"))
            {
                leftMove(90);
            }
            // 우회전
            else if (direction.Equals("right"))
            {
                rightMove(315);
            }
            else if (direction.Equals("right45"))
            {
                rightMove(270);
            }
            else if (direction.Equals("right135"))
            {
                rightMove(0);
            }
        }
        else if (beforeRotation >= 265 && beforeRotation <= 275)
        {
            // 좌회전
            if (direction.Equals("left"))
            {
                leftMove(180);
            }
            else if (direction.Equals("left45"))
            {
                leftMove(225);
            }
            else if (direction.Equals("left135"))
            {
                leftMove(135);
            }
            // 우회전
            else if (direction.Equals("right"))
            {
                rightMove(0);
            }
            else if (direction.Equals("right45"))
            {
                rightMove(315);
            }
            else if (direction.Equals("right135"))
            {
                rightMove(45);
            }
        }
        else if (beforeRotation >= 310 && beforeRotation <= 320)
        {
            // 좌회전
            if (direction.Equals("left"))
            {
                leftMove(225);
            }
            else if (direction.Equals("left45"))
            {
                leftMove(270);
            }
            else if (direction.Equals("left135"))
            {
                leftMove(180);
            }
            // 우회전
            else if (direction.Equals("right"))
            {
                rightMove(45);
            }
            else if (direction.Equals("right45"))
            {
                rightMove(0);
            }
            else if (direction.Equals("right135"))
            {
                rightMove(90);
            }
        }
        else
        {
            Debug.Log("아무것도 실행 안됨!");
        }
    }

    private void leftMove(int angle)
    {
        transform.eulerAngles = new Vector3(0, angle, 0);
        beforeRotation = angle;
    }

    private void rightMove(int angle)
    {
        transform.eulerAngles = new Vector3(0, angle, 0);
        beforeRotation = angle;
    }
}
