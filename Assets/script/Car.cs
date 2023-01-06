using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rigid; // 물리기능 추가 - 코드 상에서 rigid를 사용한다고 생각하지만 진짜 존재하는 Rigidbody를 사용하는 것
    [Range(0, 360)]
    
    // 차량 속도 선언 - 10, 15, 20
    private int[] speed = new int[] { 10, 15, 20 };
    // 현재 차량의 위치에 따른 속도가 다름 => current speed 변수 선언
    private static int init_speed = 5;
    public int current_speed = init_speed; // 초기 속도 5

    // 차량 정지 및 직진 신호
    bool forward_signal = false;

    float timer;
    float Passtime;
    int waitingTime = 200;
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
    private float carDist; // 차량 사이의 거리
    private float distLimit = 10.0f; // 차량 사이의 거리 제한
    private int speedLimit = 30; // 속도 제한
    private bool isRoad_30 = false; // 속도 제한 30km/h의 도로인 경우
    private bool isRoad_60 = false; // 속도 제한 60km/h의 도로인 경우
    private GameObject carBack;

    void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        rigid = GetComponent<Rigidbody>();
        carBack = transform.GetChild(8).gameObject;
        //carBack.transform.localPosition = new Vector3(0, 0, -2);
        //carBack.transform.Translate(new Vector3(0, 0, -2), Space.Self);
    }

    private void Start()
    {
        InvokeRepeating("InvokeTrafficSignal", 3f, 5f);
    }

    private void InvokeTrafficSignal()
    {
        if (forward_signal)
        {
            forward_signal = false;
            Debug.Log("red light");
        }
        else
        {
            forward_signal = true;
            Debug.Log("green light");
        }
    }

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
        // 앞 차와 충돌하는 경우
        //if (collision.collider.CompareTag("SportCar2"))
        //{
        //    Debug.Log("충돌!");
        //    speed = 0;
        //}
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        // speed 10 제한 도로에 진입하는 경우
        if (other.CompareTag("Limit10RoadEnter"))
        {
            Debug.Log("10 제한 도로 진입!");
            //isRoad_30 = true;
            BackTriggerSettingBySpeed(10);
        }
        // speed 15 제한 도로에 진입하는 경우
        else if (other.CompareTag("Limit15RoadEnter"))
        {
            Debug.Log("15 제한 도로 진입!");
            //isRoad_30 = true;
            BackTriggerSettingBySpeed(15);
        }
        // 좁은 도로에서 탈출하는 경우 - 속도 0km/h
        else if(other.CompareTag("NarrowRoadExit"))
        {
            Debug.Log("신호대기");
            //isRoad_30 = false;
            if (forward_signal)
            {
                BackTriggerSettingBySpeed(init_speed);
            }
            else
            {
                BackTriggerSettingBySpeed(0);
            }
        }

        // carBack과 충돌하는 경우
        if (other.CompareTag("carBack"))
        {
            current_speed = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NarrowRoadExit"))
        {
            if (forward_signal && current_speed == 0)
            {
                BackTriggerSettingBySpeed(init_speed);
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
        if (other.CompareTag("carBack"))
        {
            BackTriggerSettingBySpeed(init_speed);
        }
    }

    private void BackTriggerSettingBySpeed(int speed_)
    {
        if (speed_ == init_speed)
        {
            carBack.transform.localPosition = new Vector3(0, 0, -3);
            current_speed = init_speed;
            speedLimit = init_speed;
        }
        else if (speed_ == 10)
        {
            carBack.transform.localPosition = new Vector3(0, 0, -4);
            current_speed = speed[0];
            speedLimit = speed[0];
        }
        else if (speed_ == 15)
        {
            carBack.transform.localPosition = new Vector3(0, 0, -5);
            current_speed = speed[1];
            speedLimit = speed[1];
        }
        else if (speed_ == 20)
        {
            carBack.transform.localPosition = new Vector3(0, 0, -6);
            current_speed = speed[2];
            speedLimit = speed[2];

        }
        else
        {
            carBack.transform.localPosition = new Vector3(0, 0, -3);
            current_speed = 0;
            speedLimit = 0;
        }
    }

    // 신호
    private void trafficLight()
    {

    }
}