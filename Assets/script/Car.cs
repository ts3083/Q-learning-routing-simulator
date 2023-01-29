using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rigid; // 물리기능 추가 - 코드 상에서 rigid를 사용한다고 생각하지만 진짜 존재하는 Rigidbody를 사용하는 것
    [Range(0, 360)]
    
    // 차량 속도 선언 - 10, 15, 20
    private int[] speed = new int[] { 30, 60 };
    // 현재 차량의 위치에 따른 속도가 다름 => current speed 변수 선언
    private static int init_speed = 10;
    public int current_speed = init_speed; // 초기 속도 10

    // 차량 정지 및 직진 신호
    public bool signal;
    public bool moveDirection = false;
    //public List<string> signal_str;
    //int temp = 0;

    public float timer = 0.0f;
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
    
    public string direction;
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
    public bool getDirection; // 자동차가 RSU로부터 방향을 결정 받았는지 여부
    public int lineNum; // 자동차가 위치한 차선

    int CarLayerName;
    int RotateLayerName;

    // 교차로에서 차량에 적용되는 레이어 - 현재 사용X
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
        getDirection = false;
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
        // speed 30 제한 도로에 진입하는 경우
        if (other.CompareTag("NarrowRoadEnter"))
        {
            BackTriggerSettingBySpeed(30);
            setLayerCar();
            getDirection = false;
            Debug.Log(direction);
        }
        // spped 60 제한 도로에 진입하는 경우
        if (other.CompareTag("WideRoadEnter"))
        {
            BackTriggerSettingBySpeed(60);
            setLayerCar();
            getDirection = false;
            Debug.Log(direction);
        }

        // carBack과 충돌하는 경우
        if (other.CompareTag("carBack"))
        {
            current_speed = 0;
        }

        if (other.CompareTag("CrossRoadReady"))
        {
            BackTriggerSettingBySpeed(init_speed);
        }
        
        if (other.CompareTag("CrossRoad") || other.CompareTag("Corner"))
        {
            moveDirection = true;
            isCrossRoad = true;
            BackTriggerSettingBySpeed(init_speed);
            setLayerRotateCar();
            getDirection = true;
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
        if (other.CompareTag("CrossRoad"))
        {
            Debug.Log("교차로 탈출!");
            isCrossRoad = false;
        }

        if (other.CompareTag("carBack"))
        {
            current_speed = speedLimit;
        }
    }

    // 좌회전 & 우회전
    private void drive(string direction)
    {
        if(moveDirection) // 길 건너는 중이면 실행
        {
            //Debug.Log(rotation.y);
            if (beforeRotation == 0)
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
        // 속도가 30인 경우
        else if (speed_ == 30) 
        {
            carBack.transform.localPosition = new Vector3(0, 0, -5f);
            current_speed = speed[0];
            speedLimit = speed[0];
        }
        // 속도가 60인 경우
        else if (speed_ == 60)
        {
            carBack.transform.localPosition = new Vector3(0, 0, -10f);
            current_speed = speed[1];
            speedLimit = speed[1];

        }
        // 그 이외의 경우
        else
        {
            if(speedLimit == 30)
            {
                carBack.transform.localPosition = new Vector3(0, 0, -5.0f);
            }
            else if(speedLimit == 60)
            {
                carBack.transform.localPosition = new Vector3(0, 0, -10.0f);
            }
            current_speed = 0;
            speedLimit = 0;
        }
    }
}