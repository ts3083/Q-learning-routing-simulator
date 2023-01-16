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
    private int current_speed = init_speed; // 초기 속도 10

    // 차량 정지 및 직진 신호
    bool signal = false;
    bool moveDirection = false;
    public List<string> signal_str;
    int temp = 0;

    public float timer = 0.0f;
    float Passtime;
    int waitingTime = 200;
    private float beforeRotation; // 회전 이전의 rotation
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
    private bool isRoad_30 = false; // 속도 제한 30km/h의 도로인 경우
    private bool isRoad_60 = false; // 속도 제한 60km/h의 도로인 경우
    private bool isCarStopped = false;
    
    private string direction;
    public float lRotateFactor = 0.96f; // 사거리에서 좌회전시 회전량 결정 요소
    public float rRotateFactor = 1.6f; // 사거리에서 우회전시 회전량 결정 요소
    private GameObject carBack; // 차량 뒷면 트리거
    public string RSU_type;
    public int lineNum; // 자동차가 위치한 차선

    void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        rigid = GetComponent<Rigidbody>();
        carBack = transform.GetChild(8).gameObject;
        beforeRotation = transform.eulerAngles.y;
        //carBack.transform.localPosition = new Vector3(0, 0, -2);
        //carBack.transform.Translate(new Vector3(0, 0, -2), Space.Self);
        BackTriggerSettingBySpeed(init_speed);
        direction = decideDirectionByRoadNum1(); // 진행방향 랜덤 결정
    }

    private void Start()
    {
        InvokeRepeating("InvokeTrafficSignal", 1f, 2f);
    }

    private void InvokeTrafficSignal()
    {
        if (signal)
        {
            signal = false;
            temp++;
        }
        else
        {
            signal = true;
            if (temp % 3 == 0) // 1에서 3, 3에서 1로 직진 & 1에서 4, 3에서 2로 좌회전
            {
                signal_str.Clear();
                signal_str.Add("straight1-3");
                signal_str.Add("left1-4");
                signal_str.Add("left3-2");
            }
            else if (temp % 3 == 1) // 2에서 4, 4에서 2로 직진 & 2에서 1, 4에서 3으로 좌회전
            {
                signal_str.Clear();
                signal_str.Add("straight2-4");
                signal_str.Add("left2-1");
                signal_str.Add("left4-3");
            }
            else if (temp % 3 == 2) // 우회전
            {
                signal_str.Clear();
                signal_str.Add("right");
            }
        }
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
        //carList = new List<GameObject>(GameObject.FindGameObjectsWithTag("SportCar2"));
        //foreach (GameObject frontCar in carList)
        //{
        //    if (frontCar.name != gameObject.name)
        //    {
        //        carDist = Vector3.Distance(gameObject.transform.position, frontCar.transform.position); // 차량 사이의 거리 계산
        //        if (carDist <= distLimit)
        //        {
        //            //frontCar.GetComponent<Car>().speed - 다른 스크립트에 있는 변수에 접근
        //            Debug.Log(gameObject.name + " 과 " + frontCar.name + "차량 사이의 거리: " + carDist);
        //            current_speed = 0;
        //        }
        //    }
        //}

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

    private void OnCollisionExit(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other) 
    {
        // speed 30 제한 도로에 진입하는 경우
        if (other.CompareTag("Limit30RoadEnterByRoadNum1"))
        {
            //isRoad_30 = true;
            BackTriggerSettingBySpeed(30);
            direction = decideDirectionByRoadNum1(); // 진행방향 랜덤 결정
            Debug.Log(direction);
        }
        else if (other.CompareTag("Limit30RoadEnterByRoadNum2"))
        {
            BackTriggerSettingBySpeed(30);
            direction = decideDirectionByRoadNum2(); // 진행방향 랜덤 결정
            Debug.Log(direction);
        }
        else if (other.CompareTag("Limit30RoadEnterByRoadNum3"))
        {
            BackTriggerSettingBySpeed(30);
            direction = decideDirectionByRoadNum3(); // 진행방향 랜덤 결정
            Debug.Log(direction);
        }
        else if (other.CompareTag("Limit30RoadEnterByRoadNum4"))
        {
            BackTriggerSettingBySpeed(30);
            direction = decideDirectionByRoadNum4(); // 진행방향 랜덤 결정
            Debug.Log(direction);
        }
        else if (other.CompareTag("OnlyLeft30RoadEnter4-3"))
        {
            BackTriggerSettingBySpeed(30);
            direction = "left4-3";
        }
        else if (other.CompareTag("OnlyLeft30RoadEnter1-4"))
        {
            BackTriggerSettingBySpeed(30);
            direction = "left1-4";
        }
        else if (other.CompareTag("OnlyLeft30RoadEnter2-1"))
        {
            BackTriggerSettingBySpeed(30);
            direction = "left2-1";
        }
        else if (other.CompareTag("OnlyLeft30RoadEnter3-2"))
        {
            BackTriggerSettingBySpeed(30);
            direction = "left3-2";
        }
        else if (other.CompareTag("OnlyRight30RoadEnter"))
        {
            BackTriggerSettingBySpeed(30);
            direction = "right";
        }
        // speed 60 제한 도로에 진입하는 경우
        else if (other.CompareTag("Limit60RoadEnterByRoadNum1"))
        {
            //isRoad_30 = true;
            BackTriggerSettingBySpeed(60);
            direction = decideDirectionByRoadNum1(); // 진행방향 랜덤 결정
            Debug.Log(direction);
        }
        else if (other.CompareTag("Limit60RoadEnterByRoadNum2"))
        {
            //isRoad_30 = true;
            BackTriggerSettingBySpeed(60);
            direction = decideDirectionByRoadNum2(); // 진행방향 랜덤 결정
            Debug.Log(direction);
        }
        else if (other.CompareTag("Limit60RoadEnterByRoadNum3"))
        {
            //isRoad_30 = true;
            BackTriggerSettingBySpeed(60);
            direction = decideDirectionByRoadNum3(); // 진행방향 랜덤 결정
            Debug.Log(direction);
        }
        else if (other.CompareTag("Limit60RoadEnterByRoadNum4"))
        {
            //isRoad_30 = true;
            BackTriggerSettingBySpeed(60);
            direction = decideDirectionByRoadNum4(); // 진행방향 랜덤 결정
            Debug.Log(direction);
        }
        else if (other.CompareTag("OnlyLeft60RoadEnter4-3"))
        {
            BackTriggerSettingBySpeed(60);
            direction = "left4-3";
        }
        else if (other.CompareTag("OnlyLeft60RoadEnter1-4"))
        {
            BackTriggerSettingBySpeed(60);
            direction = "left1-4";
        }
        else if (other.CompareTag("OnlyLeft60RoadEnter2-1"))
        {
            BackTriggerSettingBySpeed(60);
            direction = "left2-1";
        }
        else if (other.CompareTag("OnlyLeft60RoadEnter3-2"))
        {
            BackTriggerSettingBySpeed(60);
            direction = "left3-2";
        }
        else if (other.CompareTag("OnlyRight60RoadEnter"))
        {
            BackTriggerSettingBySpeed(60);
            direction = "right";
        }
        else if (other.CompareTag("ExceptStraight60RoadEnterByRoadNum1"))
        {
            BackTriggerSettingBySpeed(60);
            direction = decideDirection_exceptStraightByRoadNum1();
        }

        // carBack과 충돌하는 경우
        if (other.CompareTag("carBack"))
        {
            current_speed = 0;
        }
        
        if (other.CompareTag("CrossRoad") || other.CompareTag("Corner"))
        {
            moveDirection = true;
            BackTriggerSettingBySpeed(init_speed);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 좁은 도로에서 탈출하는 경우 - 속도 0km/h
        if (other.CompareTag("NarrowRoadExit")) // 이미 CrossRoad와 만났다면 신호 무시
        {
            //isRoad_30 = false;
            if (signal && signal_str.Contains(direction))
            {
                BackTriggerSettingBySpeed(init_speed);
            }
            else
            {
                BackTriggerSettingBySpeed(0);
            }
        }
        //if (other.CompareTag("NarrowRoadExit"))
        //{
        //    if (forward_signal && current_speed == 0)
        //    {
        //        BackTriggerSettingBySpeed(init_speed);
        //    }
        //}

        if (other.CompareTag("CrossRoad") || other.CompareTag("Corner"))
        {
            current_speed = init_speed;
            if (direction.Contains("left") || direction.Contains("right"))
            {
                drive(direction);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("NarrowRoadExit"))
        //{
        //    Debug.Log("도로 탈출!");
        //    speed = 0;
        //}
    }

    // 좌회전 & 우회전
    private void drive(string direction)
    {
        if(moveDirection) // 길 건너는 중이면 실행
        {
            if (beforeRotation == 0)
            {
                // 좌회전
                if (direction.Contains("left"))
                {
                    transform.eulerAngles -= Vector3.up * lRotateFactor;
                    if (rotation.y > 180 && rotation.y <= 270)
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
                    if (rotation.y > 90 && rotation.y <= 180)
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
                    if (rotation.y > 90 && rotation.y <= 180)
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
                    if (rotation.y > 0 && rotation.y <= 90)
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
                    if (rotation.y > 0 && rotation.y <= 90)
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
                    if (rotation.y > 270 && rotation.y <= 360)
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
                    if (rotation.y > 270 && rotation.y <= 360)
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
                    if (rotation.y > 180 && rotation.y <= 270)
                    {
                        moveDirection = false;
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        beforeRotation = 180;
                    }
                }
            }
        }
    }

    private string decideDirectionByRoadNum1()
    {
        string[] s = { "straight1-3", "right", "left1-4" };

        return s[Random.Range(0, 3)];
    }

    private string decideDirectionByRoadNum2()
    {
        string[] s = { "straight2-4", "right", "left2-1" };

        return s[Random.Range(0, 3)];
    }

    private string decideDirectionByRoadNum3()
    {
        string[] s = { "straight1-3", "right", "left3-2" };

        return s[Random.Range(0, 3)];
    }
    private string decideDirectionByRoadNum4()
    {
        string[] s = { "straight2-4", "right", "left4-3" };

        return s[Random.Range(0, 3)];
    }

    private string decideDirection_exceptStraightByRoadNum1()
    {
        string[] s = { "right", "left1-4" };

        return s[Random.Range(0, 2)];
    }

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