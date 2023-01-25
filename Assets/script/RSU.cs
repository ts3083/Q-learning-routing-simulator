using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSU : MonoBehaviour
{
    private List<GameObject> carList;   // 자동차 GameObject 리스트
    private float RSU_effectRange = 20;  // RSU 영향 범위

    // 차량 통제 신호
    int temp = 0;
    public bool signal = false;
    public List<string> signal_str;
    private bool isCrossRoad4;
    private bool isCorner;

    // Start is called before the first frame update
    void Start()
    {
        carList = new List<GameObject>(GameObject.FindGameObjectsWithTag("SportCar2"));
        if (gameObject.CompareTag("RSU_narrow_crossRoad4"))
        {
            isCrossRoad4 = true;
            InvokeRepeating("InvokeTrafficSignal", 2f, 10f);
        }
        else if(gameObject.CompareTag("RSU_narrow_crossRoad3_1") || gameObject.CompareTag("RSU_narrow_crossRoad3_2") || gameObject.CompareTag("RSU_narrow_crossRoad3_3") || gameObject.CompareTag("RSU_narrow_crossRoad3_4"))
        {
            isCrossRoad4 = false;
            InvokeRepeating("InvokeTrafficSignal", 2f, 10f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ifDistInRange();
    }

    private void ifDistInRange()
    {
        float dist;

        foreach(GameObject car in carList)
        {
            dist = Vector3.Distance(gameObject.transform.position, car.transform.position);

            if(dist < RSU_effectRange)
            {
                //Debug.Log(dist);
                // Corner
                if (gameObject.CompareTag("RSU_narrow_corner1-2"))
                {
                    switch (car.GetComponent<Car>().lineNum)
                    {
                        case 1:
                            car.GetComponent<Car>().lRotateFactor = 2.82f;
                            car.GetComponent<Car>().rRotateFactor = 1.64f;
                            break;
                        case 2:
                            car.GetComponent<Car>().lRotateFactor = 1.44f;
                            car.GetComponent<Car>().rRotateFactor = 3.85f;
                            break;
                    }

                    float beforeRotation = car.GetComponent<Car>().beforeRotation;

                    if(beforeRotation == 90)
                    {
                        car.GetComponent<Car>().direction = "left";
                    }
                    else if(beforeRotation == 180)
                    {
                        car.GetComponent<Car>().direction = "right";
                    }
                }
                else if (gameObject.CompareTag("RSU_narrow_corner2-3"))
                {
                    switch (car.GetComponent<Car>().lineNum)
                    {
                        case 1:
                            car.GetComponent<Car>().lRotateFactor = 2.82f;
                            car.GetComponent<Car>().rRotateFactor = 1.64f;
                            break;
                        case 2:
                            car.GetComponent<Car>().lRotateFactor = 1.44f;
                            car.GetComponent<Car>().rRotateFactor = 3.85f;
                            break;
                    }

                    float beforeRotation = car.GetComponent<Car>().beforeRotation;

                    if (beforeRotation == 0)
                    {
                        car.GetComponent<Car>().direction = "left";
                    }
                    else if (beforeRotation == 90)
                    {
                        car.GetComponent<Car>().direction = "right";
                    }
                }
                else if (gameObject.CompareTag("RSU_narrow_corner3-4"))
                {
                    switch (car.GetComponent<Car>().lineNum)
                    {
                        case 1:
                            car.GetComponent<Car>().lRotateFactor = 2.82f;
                            car.GetComponent<Car>().rRotateFactor = 1.64f;
                            break;
                        case 2:
                            car.GetComponent<Car>().lRotateFactor = 1.44f;
                            car.GetComponent<Car>().rRotateFactor = 3.85f;
                            break;
                    }

                    float beforeRotation = car.GetComponent<Car>().beforeRotation;

                    if (beforeRotation == 270)
                    {
                        car.GetComponent<Car>().direction = "left";
                    }
                    else if (beforeRotation == 0)
                    {
                        car.GetComponent<Car>().direction = "right";
                    }
                }
                else if (gameObject.CompareTag("RSU_narrow_corner4-1"))
                {
                    switch (car.GetComponent<Car>().lineNum)
                    {
                        case 1:
                            car.GetComponent<Car>().lRotateFactor = 2.82f;
                            car.GetComponent<Car>().rRotateFactor = 1.64f;
                            break;
                        case 2:
                            car.GetComponent<Car>().lRotateFactor = 1.44f;
                            car.GetComponent<Car>().rRotateFactor = 3.85f;
                            break;
                    }

                    float beforeRotation = car.GetComponent<Car>().beforeRotation;

                    if (beforeRotation == 180)
                    {
                        car.GetComponent<Car>().direction = "left";
                    }
                    else if (beforeRotation == 270)
                    {
                        car.GetComponent<Car>().direction = "right";
                    }
                }
                // 4거리 또는 3거리
                else if (gameObject.CompareTag("RSU_narrow_crossRoad4"))
                {
                    switch (car.GetComponent<Car>().lineNum)
                    {
                        case 1:
                            car.GetComponent<Car>().lRotateFactor = 0.97f;
                            car.GetComponent<Car>().rRotateFactor = 1.60f;
                            break;
                        case 2:
                            car.GetComponent<Car>().lRotateFactor = 0.71f;
                            car.GetComponent<Car>().rRotateFactor = 3.35f;
                            break;
                    }

                    car.GetComponent<Car>().signal = signal;
                    car.GetComponent<Car>().signal_str = signal_str;

                    if (!car.GetComponent<Car>().getDirection)      // 자동차의 방향 결정
                    {
                        DecideCarDirection(car);
                    }
                }
                else if (gameObject.CompareTag("RSU_narrow_crossRoad3_1"))
                {
                    switch (car.GetComponent<Car>().lineNum)
                    {
                        case 1:
                            car.GetComponent<Car>().lRotateFactor = 0.97f;
                            car.GetComponent<Car>().rRotateFactor = 1.60f;
                            break;
                        case 2:
                            car.GetComponent<Car>().lRotateFactor = 0.71f;
                            car.GetComponent<Car>().rRotateFactor = 3.35f;
                            break;
                    }

                    car.GetComponent<Car>().signal = signal;
                    car.GetComponent<Car>().signal_str = signal_str;

                    if (!car.GetComponent<Car>().getDirection)      // 자동차의 방향 결정
                    {
                        DecideCarDirection(car, 1);
                    }
                }
                else if (gameObject.CompareTag("RSU_narrow_crossRoad3_2"))
                {
                    switch (car.GetComponent<Car>().lineNum)
                    {
                        case 1:
                            car.GetComponent<Car>().lRotateFactor = 0.97f;
                            car.GetComponent<Car>().rRotateFactor = 1.60f;
                            break;
                        case 2:
                            car.GetComponent<Car>().lRotateFactor = 0.71f;
                            car.GetComponent<Car>().rRotateFactor = 3.35f;
                            break;
                    }

                    car.GetComponent<Car>().signal = signal;
                    car.GetComponent<Car>().signal_str = signal_str;

                    if (!car.GetComponent<Car>().getDirection)      // 자동차의 방향 결정
                    {
                        DecideCarDirection(car, 2);
                    }
                }
                else if (gameObject.CompareTag("RSU_narrow_crossRoad3_3"))
                {
                    switch (car.GetComponent<Car>().lineNum)
                    {
                        case 1:
                            car.GetComponent<Car>().lRotateFactor = 0.97f;
                            car.GetComponent<Car>().rRotateFactor = 1.60f;
                            break;
                        case 2:
                            car.GetComponent<Car>().lRotateFactor = 0.71f;
                            car.GetComponent<Car>().rRotateFactor = 3.35f;
                            break;
                    }

                    car.GetComponent<Car>().signal = signal;
                    car.GetComponent<Car>().signal_str = signal_str;

                    if (!car.GetComponent<Car>().getDirection)      // 자동차의 방향 결정
                    {
                        DecideCarDirection(car, 3);
                    }
                }
                else if (gameObject.CompareTag("RSU_narrow_crossRoad3_4"))
                {
                    switch (car.GetComponent<Car>().lineNum)
                    {
                        case 1:
                            car.GetComponent<Car>().lRotateFactor = 0.97f;
                            car.GetComponent<Car>().rRotateFactor = 1.60f;
                            break;
                        case 2:
                            car.GetComponent<Car>().lRotateFactor = 0.71f;
                            car.GetComponent<Car>().rRotateFactor = 3.35f;
                            break;
                    }

                    car.GetComponent<Car>().signal = signal;
                    car.GetComponent<Car>().signal_str = signal_str;

                    if (!car.GetComponent<Car>().getDirection)      // 자동차의 방향 결정
                    {
                        DecideCarDirection(car, 4);
                    }
                }
            }
            else
            {

            }
        }
    }

    private void InvokeTrafficSignal()
    {
        if (signal)
        {
            signal = false;
            signal_str.Clear();
            signal_str.Add("right");
            temp++;
        }
        else
        {
            signal = true;
            if (temp % 4 == 0)      // 1에서 3으로 직진 & 1에서 4로 좌회전, 우회전
            {
                signal_str.Clear();
                signal_str.Add("straight1-3");
                signal_str.Add("left1-4");
                signal_str.Add("right");
            }
            else if (temp % 4 == 1)     // 2에서 4로 직진 & 2에서 1로 좌회전, 우회전
            {
                signal_str.Clear();
                signal_str.Add("straight2-4");
                signal_str.Add("left2-1");
                signal_str.Add("right");
            }
            else if(temp % 4 == 2)      // 3에서 1로 직진 & 3에서 2로 좌회전, 우회전
            {
                signal_str.Clear();
                signal_str.Add("straight3-1");
                signal_str.Add("left3-2");
                signal_str.Add("right");
            }
            else if(temp % 4 == 3)      // 4에서 2로 직진 & 4에서 3으로 좌회, 우회전
            {
                signal_str.Clear();
                signal_str.Add("straight4-2");
                signal_str.Add("left4-3");
                signal_str.Add("right");
            }
        }
    }

    private void DecideCarDirection(GameObject car, int vacantRoadNum = 0)
    {
        float beforeRoatation = car.GetComponent<Car>().beforeRotation;

        if(beforeRoatation == 0)
        {
            car.GetComponent<Car>().direction = DecideDirectionByRoadNum3(vacantRoadNum);
        }
        else if(beforeRoatation == 90)
        {
            car.GetComponent<Car>().direction = DecideDirectionByRoadNum2(vacantRoadNum);
        }
        else if(beforeRoatation == 180)
        {
            car.GetComponent<Car>().direction = DecideDirectionByRoadNum1(vacantRoadNum);
        }
        else if(beforeRoatation == 270)
        {
            car.GetComponent<Car>().direction = DecideDirectionByRoadNum4(vacantRoadNum);
        }
    }

    // 자동차가 위치한 도로 방향에 따라 방향 결정
    private string DecideDirectionByRoadNum1(int vacantRoadNum)
    {
        List<string> s;

        if (isCrossRoad4)
        {
            s = new List<string> { "straight1-3", "right", "left1-4" };
        }
        else
        {
            switch (vacantRoadNum)
            {
                case 2:
                    s = new List<string> { "straight1-3", "left1-4" };
                    break;
                case 3:
                    s = new List<string> { "right", "left1-4" };
                    break;
                case 4:
                    s = new List<string> { "straight1-3", "right", };
                    break;
                default:
                    s = new List<string> { "straight1-3", "right", "left1-4" };
                    break;
            }
        }

        return s[Random.Range(0, s.Count)];
    }

    private string DecideDirectionByRoadNum2(int vacantRoadNum = 0)
    {
        List<string> s;

        if (isCrossRoad4)
        {
            s = new List<string> { "straight2-4", "right", "left2-1" };
        }
        else
        {
            switch (vacantRoadNum)
            {
                case 1:
                    s = new List<string> { "straight2-4", "right" };
                    break;
                case 3:
                    s = new List<string> { "straight2-4", "left2-1" };
                    break;
                case 4:
                    s = new List<string> { "right", "left2-1" };
                    break;
                default:
                    s = new List<string> { "straight2-4", "right", "left2-1" };
                    break;
            }
        }

        return s[Random.Range(0, s.Count)];
    }

    private string DecideDirectionByRoadNum3(int vacantRoadNum = 0)
    {
        List<string> s;
        if (isCrossRoad4)
        {
            s = new List<string> { "straight3-1", "right", "left3-2" };
        }
        else
        {
            switch (vacantRoadNum)
            {
                case 1:
                    s = new List<string> { "right", "left3-2" };
                    break;
                case 2:
                    s = new List<string> { "straight3-1", "right" };
                    break;
                case 4:
                    s = new List<string> { "straight3-1", "left3-2" };
                    break;
                default:
                    s = new List<string> { "straight3-1", "right", "left3-2" };
                    break;
            }
        }

        return s[Random.Range(0, s.Count)];
    }
    private string DecideDirectionByRoadNum4(int vacantRoadNum = 0)
    {
        List<string> s;
        if (isCrossRoad4)
        {
            s = new List<string> { "straight4-2", "right", "left4-3" };

        }
        else
        {
            switch (vacantRoadNum)
            {
                case 1:
                    s = new List<string> { "straight4-2", "left4-3" };
                    break;
                case 2:
                    s = new List<string> { "right", "left4-3" };
                    break;
                case 3:
                    s = new List<string> { "straight4-2", "right" };
                    break;
                default:
                    s = new List<string> { "straight4-2", "right", "left4-3" };
                    break;
            }
        }

        return s[Random.Range(0, s.Count)];
    }
}
