using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public int dest_RSU;        // destination RSU
    public int demandLevel;     // Demand Level
    public int safetyLevel;        // Safety Level

    public int curActionIndex;        // Q-table에서 해당 action(neighbor RSU)의 index
    public int prevActionIndex;        // Q-table에서 해당 action(neighbor RSU)의 index

    public int cur_RSU;        // 현재 RSU
    public int prev_RSU;        // 이전 RSU

    public float timer = 0.0f;      // 교차로 사이에 이동 시간 측정
    private bool timer_on = false;      // 시간 측정 시작 여부

    public float energy = 0.0f;        // 교차로 사이를 이동하는데 필요한 에너지
    private int m = 1500;       // mass of vehicle(kg)
    private float g = 9.81f;        // acceleration of gravity(m/s^2)
    public int theta = 0;      // 현재 선택한 경로의 경사각도
    private float p = 1.28f;        // mass of air(kg/m^3)
    private float Cw = 0.35f;       // drag coefficient
    private float A = 1.8f;     // frontal area of the car(m^2)
    private float u = 0.005f;       // rolling resistance coefficient
    private float Nt = 0.1f;       // normalization for time
    private float Ne = 0.0001f;       // normalization for energy

    private float alpha = 0.1f;     // Q-learning의 learning rate
    private float gamma = 0.9f;     // Q-learning의 discount factor

    private float[] RSU_Q_table = new float[5];       // 이전 RSU의 특정 state(destination RSU)에서의 Q-table

    // Start is called before the first frame update
    Rigidbody rigid; // 물리기능 추가 - 코드 상에서 rigid를 사용한다고 생각하지만 진짜 존재하는 Rigidbody를 사용하는 것
    [Range(0, 360)]
    
    // 차량 속도 선언 - 10, 15, 20
    private int[] speed = new int[] { 15, 30 };
    // 현재 차량의 위치에 따른 속도가 다름 => current speed 변수 선언
    private static int init_speed = 10;     // 초기 속도(m/s)
    public int current_speed = init_speed;      // 초기 속도 10(m/s)

    // 차량 정지 및 직진 신호
    public bool signal;
    public bool moveDirection = false;
    //public List<string> signal_str;
    //int temp = 0;

    //public float timer = 0.0f;
    float Passtime;
    int waitingTime = 200;
    public float beforeRotation; // 회전 이전의 rotation
    private Vector3 rotation;
    public int way = 4;
    public int turn = 3;
    public int rsu_row = 5;
    public int rsu_col = 5;
    Vector3 startPosition, startRotation;
    public string[][] driveSelect;
    public string[][] rsuSelect;
    public string[] a;
    private List<GameObject> carList; // scene에 존재하는 차량(SportsCar2) 저장, test
    private int speedLimit = 30; // 속도 제한
    private bool isCrossRoad = false;
    
    public string direction = "null";
    public float lRotateFactor; // 사거리에서 좌회전시 회전량 결정 요소
    public float rRotateFactor; // 사거리에서 우회전시 회전량 결정 요소
    private GameObject carBack; // 차량 뒷면 트리거
    private GameObject BL;
    private GameObject BR;
    private GameObject door_fl;
    private GameObject door_fr;
    private GameObject FL;
    private GameObject FR;
    private GameObject SportCar2;
    private GameObject steering_wheel;
    //public bool getDirection; // 자동차가 RSU로부터 방향을 결정 받았는지 여부
    public int lineNum; // 자동차가 위치한 차선

    int CarLayerName;
    int RotateLayerName;

    // 교차로에서 차량에 적용되는 레이어 - 디폴트
    void setLayerCar()
    {
        this.gameObject.layer = CarLayerName;
        BL.gameObject.layer = CarLayerName;
        BR.gameObject.layer = CarLayerName;
        door_fl.gameObject.layer = CarLayerName;
        door_fr.gameObject.layer = CarLayerName;
        FL.gameObject.layer = CarLayerName;
        FR.gameObject.layer = CarLayerName;
        SportCar2.gameObject.layer = CarLayerName;
        steering_wheel.gameObject.layer = CarLayerName;
        carBack.gameObject.layer = CarLayerName;
    }

    // 교차로에서 차량에 적용되는 레이어 - 현재 사용X
    void setLayerRotateCar()
    {
        this.gameObject.layer = RotateLayerName;
        BL.gameObject.layer = RotateLayerName;
        BR.gameObject.layer = RotateLayerName;
        door_fl.gameObject.layer = RotateLayerName;
        door_fr.gameObject.layer = RotateLayerName;
        FL.gameObject.layer = RotateLayerName;
        FR.gameObject.layer = RotateLayerName;
        SportCar2.gameObject.layer = RotateLayerName;
        steering_wheel.gameObject.layer = RotateLayerName;
        carBack.gameObject.layer = RotateLayerName;
    }

    void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        rigid = GetComponent<Rigidbody>();
        carBack = transform.GetChild(8).gameObject;
        BL = transform.GetChild(0).gameObject;
        BR = transform.GetChild(1).gameObject;
        door_fl = transform.GetChild(2).gameObject;
        door_fr = transform.GetChild(3).gameObject;
        FL = transform.GetChild(4).gameObject;
        FR = transform.GetChild(5).gameObject;
        SportCar2 = transform.GetChild(6).gameObject;
        steering_wheel = transform.GetChild(7).gameObject;
        //carBack.transform.localPosition = new Vector3(0, 0, -2);
        //carBack.transform.Translate(new Vector3(0, 0, -2), Space.Self);
        BackTriggerSettingBySpeed(init_speed);
        //direction = decideDirection(); // 진행방향 랜덤 결정
        CarLayerName = LayerMask.NameToLayer("Car");
        RotateLayerName = LayerMask.NameToLayer("RotateCar");
        //setLayerCar();
        //if (direction.Equals("right"))
        //{
        //    setLayerRotateCar();
        //}
        beforeRotation = transform.eulerAngles.y;
        //getDirection = false;
    }

    private void Start()
    {

    }

    //private void InvokeTrafficSignal()
    //{
    //    if (signal)
    //    {
    //        signal = false;
    //        Debug.Log("red light");
    //        temp++;
    //    }
    //    else
    //    {
    //        signal = true;
    //        Debug.Log("green light");

    //        if (temp % 3 == 0) // 직진
    //        {
    //            signal_str = "straight";
    //        }
    //        else if (temp % 3 == 1) // 좌 
    //        {
    //            signal_str = "left";
    //        }
    //        else if (temp % 3 == 2) // 우
    //        {
    //            signal_str = "right";
    //        }
    //    }
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        // Time.deltaTime은 화면이 한번 깜빡이는 시간 = 한 프레임의 시간
        // 화면을 60번 깜빡이면 (초당 60프레) 1/60이 들어간다
        transform.position += transform.forward * current_speed * Time.deltaTime;

        //timer += Time.deltaTime * 50;
        //Debug.Log(timer);
        //Passtime = Mathf.Floor(timer / waitingTime);
        //Debug.Log(Passtime);
        //rsuSelecter();
        //Debug.Log(rsuSelect);
        rotation = this.transform.eulerAngles;
        //Debug.Log(this.rotation.y);
        //Time.timeScale = 0.02f;

        // 시간 측정
        if (timer_on)
        {
            timer += Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //앞 차와 충돌하는 경우
        //if (collision.collider.CompareTag("carBack"))
        //{
        //    Debug.Log("충돌!");
        //    speed = 0;
        //}
    }

    private void OnTriggerEnter(Collider other) 
    {
        // speed 15 제한 도로에 진입하는 경우, 시간 측정 시작
        if (other.CompareTag("NarrowRoadEnterAngleO") || other.CompareTag("NarrowRoadEnterAngleX"))
        {
            timer_on = true;        // 시간 측정 시작
        }

        // spped 30 제한 도로에 진입하는 경우
        if (other.CompareTag("WideRoadEnter"))
        {
            BackTriggerSettingBySpeed(30);
            setLayerCar();
            //getDirection = false;
        }
        if (other.CompareTag("null"))
        {
            direction = "null";
        }

        // carBack과 충돌하는 경우
        if (other.CompareTag("carBack"))
        {
            BackTriggerSettingBySpeed(0);
        }

        if (other.CompareTag("CrossRoadReady")) // 이건 왜 필요함?
        {
            BackTriggerSettingBySpeed(init_speed);
        }
        
        if (other.CompareTag("CrossRoad") || other.CompareTag("Corner"))
        {
            moveDirection = true;
            isCrossRoad = true;
            BackTriggerSettingBySpeed(init_speed);
            setLayerRotateCar();
            //getDirection = true;
        }

        if (other.CompareTag("NarrowRoadExit") && timer_on)
        {
            timer_on = false;       // 시간 측정 끝

            // 이전 RSU의 Q-table update
            UpdateRSU();

            timer = 0.0f;       // 다시 0으로 초기화
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 좁은 도로에서 탈출하는 경우 - 속도 0km/h
        if (other.CompareTag("NarrowRoadExit") && !isCrossRoad) // 이미 CrossRoad와 만났다면 신호 무시
        {
            if (signal)
            {
                BackTriggerSettingBySpeed(init_speed);
            }
            else
            {
                BackTriggerSettingBySpeed(0);
            }
        }

        if (other.CompareTag("CrossRoad") || other.CompareTag("Corner"))
        {
            //BackTriggerSettingBySpeed(init_speed);
            if (direction.Contains("left") || direction.Contains("right"))
            {
                drive(direction);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CrossRoad") || other.CompareTag("Corner"))
        {
            //Debug.Log("교차로 탈출!");
            isCrossRoad = false;
        }

        if (other.CompareTag("carBack"))
        {
            BackTriggerSettingBySpeed(speedLimit);
        }

        // speed 15 제한 도로에 진입하는 경우, 경사각 X
        if (other.CompareTag("NarrowRoadEnterAngleX"))
        {
            BackTriggerSettingBySpeed(15);
            setLayerCar();
            prevActionIndex = curActionIndex;
            curActionIndex = -1;
            prev_RSU = cur_RSU;
            cur_RSU = 0;
            theta = 0;      // 경사각 0
            //getDirection = false;
        }

        // speed 15 제한 도로에 진입하는 경우, 경사각 O
        if (other.CompareTag("NarrowRoadEnterAngleO"))
        {
            BackTriggerSettingBySpeed(15);
            setLayerCar();
            prevActionIndex = curActionIndex;
            curActionIndex = -1;
            prev_RSU = cur_RSU;
            cur_RSU = 0;
            theta = 10;     // 경사각 10
            //getDirection = false;
        }
    }

    // 좌회전 & 우회전
    private void drive(string direction)
    {
        if(moveDirection) // 길 건너는 중이면 실행
        {
            //Debug.Log(rotation.y);
            if (beforeRotation == 0 || beforeRotation == 360)
            {
                // 좌회전
                if (direction.Contains("left"))
                {
                    transform.eulerAngles -= Vector3.up * lRotateFactor;
                    if (rotation.y >= 180 && rotation.y <= 270)
                    {
                        moveDirection = false;
                        transform.eulerAngles = new Vector3(0, 270, 0);
                        beforeRotation = 270;
                    }
                }
                // 우회전
                else if (direction.Contains("right"))
                {
                    transform.eulerAngles += Vector3.up * rRotateFactor;
                    if (rotation.y >= 90 && rotation.y <= 180)
                    {
                        moveDirection = false;
                        transform.eulerAngles = new Vector3(0, 90, 0);
                        beforeRotation = 90;
                    }
                }
            }
            else if (beforeRotation == 270)
            {
                // 좌회전
                if (direction.Contains("left"))
                {
                    transform.eulerAngles -= Vector3.up * lRotateFactor;
                    if (rotation.y >= 90 && rotation.y <= 180)
                    {
                        moveDirection = false;
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        beforeRotation = 180;
                    }
                }
                // 우회전
                else if (direction.Contains("right"))
                {
                    transform.eulerAngles += Vector3.up * rRotateFactor;
                    if (rotation.y >= 0 && rotation.y <= 90)
                    {
                        moveDirection = false;
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        beforeRotation = 0;
                    }
                }
            }
            else if (beforeRotation == 180)
            {
                // 좌회전
                if (direction.Contains("left"))
                {
                    transform.eulerAngles -= Vector3.up * lRotateFactor;
                    if (rotation.y >= 0 && rotation.y <= 90)
                    {
                        moveDirection = false;
                        transform.eulerAngles = new Vector3(0, 90, 0);
                        beforeRotation = 90;
                    }
                }
                // 우회전
                else if (direction.Contains("right"))
                {
                    transform.eulerAngles += Vector3.up * rRotateFactor;
                    if (rotation.y >= 270 && rotation.y <= 360)
                    {
                        moveDirection = false;
                        transform.eulerAngles = new Vector3(0, 270, 0);
                        beforeRotation = 270;
                    }
                }
            }
            else if (beforeRotation == 90)
            {
                // 좌회전
                if (direction.Contains("left"))
                {
                    transform.eulerAngles -= Vector3.up * lRotateFactor;
                    if (rotation.y >= 270 && rotation.y <= 360)
                    {
                        moveDirection = false;
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        beforeRotation = 0;
                    }
                }
                // 우회전
                else if (direction.Contains("right"))
                {
                    transform.eulerAngles += Vector3.up * rRotateFactor;
                    if (rotation.y >= 180 && rotation.y <= 270)
                    {
                        moveDirection = false;
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        beforeRotation = 180;
                    }
                }
            }
        }
    }

    //private string decideDirection_exceptStraightByRoadNum1()
    //{
    //    string[] s = { "right", "left1-4" };

    //    return s[Random.Range(0, 2)];
    //}

    // 차량 후면 트리거(차량간 거리 조절) 위치 조정
    private void BackTriggerSettingBySpeed(int speed_)
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
        else if (speed_ == 30)
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
        // 그 이외의 경우
        //else
        //{
        //    if(speedLimit == 30)
        //    {
        //        carBack.transform.localPosition = new Vector3(0, 0, -5.0f);
        //    }
        //    else if(speedLimit == 60)
        //    {
        //        carBack.transform.localPosition = new Vector3(0, 0, -10.0f);
        //    }
        //    current_speed = 0;
        //    speedLimit = 0;
        //}
    }

    // 차량에서 이전 RSU의 Q-table update
    private void UpdateRSU()
    {
        float reward;       // Q-learning의 reward
        float Wt, We;       // Demand Level에 따른 가중치

        // 에너지 계산
        CalEnergy();

        GetQ_table();

        Debug.Log("time: " + timer);
        for (int i = 0; i < 5; i++)
        {
            // Demand Level에 따른 가중치 계산
            Wt = 1.0f - 0.25f * i;
            We = 0.25f * i;
            
            reward = -(Wt * timer * Nt + We * energy * Ne);       // Q-learning의 reward 계산
            RSU_Q_table[i] = (1 - alpha) * RSU_Q_table[i] + alpha * (reward + gamma * RSU_Q_table[demandLevel - 1]);
            Debug.Log(i + ": " + RSU_Q_table[i]);
        }

        UpdateQ_table();
    }

    // 에너지 계산
    private void CalEnergy()
    {
        // Slope Resistance Power
        float SRP = m * g * current_speed * Mathf.Sin(theta * Mathf.Deg2Rad);
        Debug.Log("SRP: " + SRP);

        // Air Resistance Power
        float ARP = 0.5f * p * Cw * A * Mathf.Pow(current_speed, 3);
        Debug.Log("ARP: " + ARP);

        // Rolling Resistance Power
        float RRP = u * m * g * current_speed;
        Debug.Log("RRP: " + RRP);

        // Total Energy
        energy = (SRP + ARP + RRP) * timer;
    }

    // update 대상 RSU의 Q-table 가져오기
    private void GetQ_table()
    {
        GameObject RSU = GameObject.Find("RSU" + prev_RSU); 

        switch (prev_RSU)
        {
            case 1:
                break;
            case 2:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU2>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 3:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU3>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            case 11:
                break;
            case 12:
                break;
            case 13:
                break;
            case 14:
                break;
            case 15:
                break;
            case 16:
                break;
            case 17:
                break;
            case 18:
                break;
            case 19:
                break;
            case 20:
                break;
            case 21:
                break;
            case 22:
                break;
            case 23:
                break;
            case 24:
                break;
            case 25:
                break;
            default:
                break;
        }
    }

    // update 대상 RSU의 Q-table 가져오기
    private void UpdateQ_table()
    {
        GameObject RSU = GameObject.Find("RSU" + prev_RSU);

        switch (prev_RSU)
        {
            case 1:
                break;
            case 2:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU2>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 3:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU3>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            case 11:
                break;
            case 12:
                break;
            case 13:
                break;
            case 14:
                break;
            case 15:
                break;
            case 16:
                break;
            case 17:
                break;
            case 18:
                break;
            case 19:
                break;
            case 20:
                break;
            case 21:
                break;
            case 22:
                break;
            case 23:
                break;
            case 24:
                break;
            case 25:
                break;
            default:
                break;
        }
    }
}