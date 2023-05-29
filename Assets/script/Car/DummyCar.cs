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
    private static int init_speed = 10;     // 초기 속도(10m/s)
    public int start_speed;     // 차량 출발 시 속도
    public int current_speed;      // 현재 차량의 위치에 따른 속도가 다름 => current speed 변수 선언
    private int speedLimit;     // 속도 제한

    //public bool signal;     // 차량 정지 및 직진 신호

    public int beforeRotation;      // 회전 이전의 rotation

    public string direction = "null";       // 차량 이동방향
    public Vector3 position;        // 월드 좌표를 기준으로 이동시킬 차량의 위치
    private GameObject carBack;     // 차량 뒷면 트리거

    private int currentLineNum;      // 현재 위치한 도로에서의 차선
    public int lineNum;     // 차량이 위치한 차선
    //private bool isCrossroad = false;       // 차량이 exit 트리거를 통과하여 교차로로 이동하였는지 여부

    //public int[] routeList;       // dummy 차량의 경로 저장, 다음 RSU에서 이동할 RSU 번호부터 시작
    //private int routeListLength;        // routeList의 길이
    //public int routeIndex = 0;     // dummy 차량이 따르는 경로 index

    public int cur_RSU;        // 현재 RSU
    public int prev_RSU;        // 이전 RSU
    public int next_RSU;        // 다음 RSU
    public int prev_lineNum;

    private bool isCarInfoUpdateNeeded = true;     // 차량의 RSU정보 update 필요 여부

    //public int curRoadNum;      // 현재 차선의 개수
    //public int nextRoadNum;     // 다음 차선의 개수

    //private crossroadMove DummyCarMoveDecision = new();     // 교차로에서 DummyCar의 이동 결정

    // Start is called before the first frame update
    //void Start()
    //{
    //    carBack = transform.GetChild(8).gameObject;     // carBack 게임 오브젝트 가져오기
    //    BackTriggerSettingBySpeed(start_speed);      // 초기 속도(10m/s)로 CarBack 트리거 설정
    //    beforeRotation = (int)transform.eulerAngles.y;
    //    start_speed = init_speed;
    //    current_speed = init_speed;
    //    //routeListLength = routeList.Length;
    //}

    private void Awake()
    {
        carBack = transform.GetChild(8).gameObject;     // carBack 게임 오브젝트 가져오기
        BackTriggerSettingBySpeed(start_speed);      // 초기 속도(10m/s)로 CarBack 트리거 설정
        beforeRotation = (int)transform.eulerAngles.y;
        start_speed = init_speed;
        current_speed = init_speed;
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * current_speed * Time.deltaTime;       // 차량 이동        

        // Delay 시간 측정
        //if (delayTimer_on)
        //{
        //    delayTimer += Time.deltaTime;

        //    if (delayTimer >= currentLineNum)
        //    {
        //        checkCarCanMove();
        //        delayTimer = 0.0f;
        //    }
        //}
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
            direction = "null";
        }

        // 교차로 탈출
        //if (other.CompareTag("NarrowRoadEnterAngleO") || other.CompareTag("NarrowRoadEnterAngleX") || other.CompareTag("WideRoadEnter"))
        //{
        //    isCrossroad = false;
        //}

        if (other.CompareTag("CrossRoad"))
        {
            //isCrossroad = true;

            // 이동방향에 따른 좌표를 받아옴
            transform.position = position;
            if (direction.Contains("left") || direction.Contains("right"))
            {
                new_drive(direction);
            }
        }

        if(other.CompareTag("NarrowRoadExit") || other.CompareTag("WideRoadExit"))
        {
            isCarInfoUpdateNeeded = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 좁은 도로에서 탈출하는 경우 - 속도 0km/h
        //if (other.CompareTag("NarrowRoadExit") || other.CompareTag("WideRoadExit")) // 이미 CrossRoad와 만났다면 신호 무시
        //{
        //    if (isCrossroad)
        //    {
        //        isCarInfoUpdateNeeded = true;
        //        BackTriggerSettingBySpeed(init_speed);
        //    }
        //    else
        //    {
        //        BackTriggerSettingBySpeed(0);
        //    }
        //}

        //if (other.CompareTag("NarrowRoadEnterAngleO") || other.CompareTag("NarrowRoadEnterAngleX"))
        //{
        //    BackTriggerSettingBySpeed(15);
        //}

        //if (other.CompareTag("WideRoadEnter"))
        //{
        //    BackTriggerSettingBySpeed(20);
        //}

        if (other.CompareTag("NarrowRoadExit") || other.CompareTag("WideRoadExit"))
        {
            // RSU로 부터 현재 RSU에 대한 정보를 받은 경우에만 실행
            //if(cur_RSU != 0)
            //{
            //    DummyCarMoveDecision.getNextDirection(prev_RSU, cur_RSU, ref lineNum, ref direction, ref position);
            //}

            //방향(direction)이 null이 아닌 경우에만 이동하도록 설정
            if (direction == "null")
            {
                BackTriggerSettingBySpeed(0);
            }
            else
            {
                BackTriggerSettingBySpeed(init_speed);
            }

            if (other.GetComponent<TrafficLight>().isLightOn == false) // red light
            {
                BackTriggerSettingBySpeed(0);
            }
            else // blue light
            {
                // direction 방향의 도로에 car 오브젝트가 있는지 확인 - 있으면 true, 없으면 false
                if (detector()) // 이동하려는 direction 도로에 차량이 있음
                {
                    BackTriggerSettingBySpeed(0);
                }
                else
                {
                    if (other.GetComponent<TrafficLight>().lightOn_lineNum.Equals(prev_lineNum))
                    {
                        BackTriggerSettingBySpeed(speedLimit);
                    }
                    else
                    {
                        BackTriggerSettingBySpeed(0);
                    }
                }
            }
        }
    }

    private bool detector()
    {
        return GameObject.Find("DetectTrigger" + cur_RSU + "-" + next_RSU)
            .GetComponent<DetectTrigger>().detected;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("carBack"))
        {
            BackTriggerSettingBySpeed(speedLimit);
        }

        // speed 15 제한 도로에 진입하는 경우, 경사각 X
        if (isCarInfoUpdateNeeded && other.CompareTag("NarrowRoadEnterAngleX"))
        {
            BackTriggerSettingBySpeed(15);
            prev_RSU = cur_RSU;
            cur_RSU = 0;
            currentLineNum = lineNum;
            //curRoadNum = nextRoadNum;
            //nextRoadNum = 0;
            isCarInfoUpdateNeeded = false;
        }

        // speed 15 제한 도로에 진입하는 경우, 경사각 O
        if (isCarInfoUpdateNeeded && other.CompareTag("NarrowRoadEnterAngleO"))
        {
            BackTriggerSettingBySpeed(15);
            prev_RSU = cur_RSU;
            cur_RSU = 0;
            currentLineNum = lineNum;
            //curRoadNum = nextRoadNum;
            //nextRoadNum = 0;
            isCarInfoUpdateNeeded = false;
        }

        // speed 20 제한 도로에 진입하는 경우, 경사각 X
        if (isCarInfoUpdateNeeded && other.CompareTag("WideRoadEnter"))
        {
            BackTriggerSettingBySpeed(20);
            prev_RSU = cur_RSU;
            cur_RSU = 0;
            currentLineNum = lineNum;
            //curRoadNum = nextRoadNum;
            //nextRoadNum = 0;
            isCarInfoUpdateNeeded = false;
        }
    }

    // 차량 후면 트리거(차량간 거리 조절) 위치 조정
    public void BackTriggerSettingBySpeed(int speed_)
    {
        // 초기 속도(init_speed)인 경우
        if (speed_ == init_speed)
        {
            //carBack.transform.localPosition = new Vector3(0, 0, -5f);
            current_speed = init_speed;
            speedLimit = init_speed;
        }
        // 속도가 15인 경우
        else if (speed_ == 15)
        {
            //carBack.transform.localPosition = new Vector3(0, 0, -7.5f);
            current_speed = speed[0];
            speedLimit = speed[0];
        }
        // 속도가 30인 경우
        else if (speed_ == 20)
        {
            //carBack.transform.localPosition = new Vector3(0, 0, -10f);
            current_speed = speed[1];
            speedLimit = speed[1];

        }
        else // 속도가 0인 경우
        {
            //carBack.transform.localPosition = new Vector3(0, 0, -4f);
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