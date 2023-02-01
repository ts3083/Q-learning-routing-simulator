using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSU : MonoBehaviour
{
    private float RSU_effectRange = 20f;  // RSU 영향 범위

    private Collider[] carList;
    private int carListNum;

    // 차량 통제 신호
    private int temp = 0;
    private bool isCrossRoad4;

    // Start is called before the first frame update
    void Start()
    {
        // 4거리인 경우
        if (gameObject.CompareTag("RSU_narrow_crossRoad4"))
        {
            isCrossRoad4 = true;
            InvokeRepeating("InvokeTrafficSignal4", 2f, 2f);
        }
        // 3거리인 경우
        else
        {
            isCrossRoad4 = false;

            // 1번 도로가 없는 경우
            if (gameObject.CompareTag("RSU_narrow_crossRoad3_1"))
            {
                InvokeRepeating("InvokeTrafficSignal3_1", 2f, 2f);
            }
            // 2번 도로가 없는 경우
            else if (gameObject.CompareTag("RSU_narrow_crossRoad3_2"))
            {
                InvokeRepeating("InvokeTrafficSignal3_2", 2f, 2f);
            }
            // 3번 도로가 없는 경우
            else if (gameObject.CompareTag("RSU_narrow_crossRoad3_3"))
            {
                InvokeRepeating("InvokeTrafficSignal3_3", 2f, 2f);
            }
            // 4번 도로가 없는 경우
            else if (gameObject.CompareTag("RSU_narrow_crossRoad3_4"))
            {
                InvokeRepeating("InvokeTrafficSignal3_4", 2f, 2f);
            }
        }

        // RSU에서 영향 범위 내의 차량에게 신호를 줌
        InvokeRepeating("ifDistInRange", 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        carList = Physics.OverlapSphere(transform.position, RSU_effectRange);   // RSU_effectRange 범위 내의 모든 오브젝트(Collider)를 가져옴
        carListNum = carList.Length;    // 배열의 크기
    }

    // RSU_effectRange 범위 내의 모든 차량들에게 적용
    private void ifDistInRange()
    {
        // carList 배열에 속해있는 모든 오브젝트(Collider)에 대하여
        for(int i = 0; i < carListNum; i++)
        {
            // 차량 오브젝트에 대해서만 실행하기 위해서
            if (!carList[i].CompareTag("SportCar2"))
            {
                continue;
            }

            // 1, 2번 도로만 있는 코너, 속도 30 제한 도로
            if (gameObject.CompareTag("RSU_narrow_corner1-2"))
            {
                // 차선(lineNum)에 따라
                switch (carList[i].GetComponent<Car>().lineNum)
                {
                    case 1:
                        carList[i].GetComponent<Car>().lRotateFactor = 2.82f;
                        carList[i].GetComponent<Car>().rRotateFactor = 1.64f;
                        break;
                    case 2:
                        carList[i].GetComponent<Car>().lRotateFactor = 1.44f;
                        carList[i].GetComponent<Car>().rRotateFactor = 3.85f;
                        break;
                }

                float beforeRotation = carList[i].GetComponent<Car>().beforeRotation;

                if (carList[i].GetComponent<Car>().direction.Equals("null")) // 자동차의 방향 결정, 1번만 결정
                {
                    // 2번 도로에서 차량이 오는 경우
                    if (beforeRotation == 90)
                    {
                        carList[i].GetComponent<Car>().direction = "left";
                    }
                    // 1번 도로에서 차량이 오는 경우
                    else if (beforeRotation == 180)
                    {
                        carList[i].GetComponent<Car>().direction = "right";
                    }
                }
            }
            // 2, 3번 도로만 있는 코너
            else if (gameObject.CompareTag("RSU_narrow_corner2-3"))
            {
                // 차선(lineNum)에 따라
                switch (carList[i].GetComponent<Car>().lineNum)
                {
                    case 1:
                        carList[i].GetComponent<Car>().lRotateFactor = 2.82f;
                        carList[i].GetComponent<Car>().rRotateFactor = 1.64f;
                        break;
                    case 2:
                        carList[i].GetComponent<Car>().lRotateFactor = 1.44f;
                        carList[i].GetComponent<Car>().rRotateFactor = 3.85f;
                        break;
                }

                float beforeRotation = carList[i].GetComponent<Car>().beforeRotation;

                if (carList[i].GetComponent<Car>().direction.Equals("null")) // 자동차의 방향 결정, 1번만 결정
                {
                    // 3번 도로에서 차량이 오는 경우
                    if (beforeRotation == 0)
                    {
                        carList[i].GetComponent<Car>().direction = "left";
                    }
                    // 2번 도로에서 차량이 오는 경우
                    else if (beforeRotation == 90)
                    {
                        carList[i].GetComponent<Car>().direction = "right";
                    }
                }
            }
            // 3, 4번 도로만 있는 코너
            else if (gameObject.CompareTag("RSU_narrow_corner3-4"))
            {
                // 차선(lineNum)에 따라
                switch (carList[i].GetComponent<Car>().lineNum)
                {
                    case 1:
                        carList[i].GetComponent<Car>().lRotateFactor = 2.82f;
                        carList[i].GetComponent<Car>().rRotateFactor = 1.64f;
                        break;
                    case 2:
                        carList[i].GetComponent<Car>().lRotateFactor = 1.44f;
                        carList[i].GetComponent<Car>().rRotateFactor = 3.85f;
                        break;
                }

                float beforeRotation = carList[i].GetComponent<Car>().beforeRotation;

                if (carList[i].GetComponent<Car>().direction.Equals("null")) // 자동차의 방향 결정, 1번만 결정
                {
                    // 4번 도로에서 차량이 오는 경우
                    if (beforeRotation == 270)
                    {
                        carList[i].GetComponent<Car>().direction = "left";
                    }
                    // 3번 도로에서 차량이 오는 경우
                    else if (beforeRotation == 0)
                    {
                        carList[i].GetComponent<Car>().direction = "right";
                    }
                }
            }
            // 4, 1번 도로만 있는 코너
            else if (gameObject.CompareTag("RSU_narrow_corner4-1"))
            {
                // 차선(lineNum)에 따라
                switch (carList[i].GetComponent<Car>().lineNum)
                {
                    case 1:
                        carList[i].GetComponent<Car>().lRotateFactor = 2.82f;
                        carList[i].GetComponent<Car>().rRotateFactor = 1.64f;
                        break;
                    case 2:
                        carList[i].GetComponent<Car>().lRotateFactor = 1.44f;
                        carList[i].GetComponent<Car>().rRotateFactor = 3.85f;
                        break;
                }

                float beforeRotation = carList[i].GetComponent<Car>().beforeRotation;

                if (carList[i].GetComponent<Car>().direction.Equals("null")) // 자동차의 방향 결정, 1번만 결정
                {
                    // 1번 도로에서 차량이 오는 경우
                    if (beforeRotation == 180)
                    {
                        carList[i].GetComponent<Car>().direction = "left";
                    }
                    // 4번 도로에서 차량이 오는 경우
                    else if (beforeRotation == 270)
                    {
                        carList[i].GetComponent<Car>().direction = "right";
                    }
                }
            }
            // 4거리
            else if (gameObject.CompareTag("RSU_narrow_crossRoad4"))
            {
                // 차선(lineNum)에 따라
                switch (carList[i].GetComponent<Car>().lineNum)
                {
                    case 1:
                        carList[i].GetComponent<Car>().lRotateFactor = 0.97f;
                        carList[i].GetComponent<Car>().rRotateFactor = 1.60f;
                        break;
                    case 2:
                        carList[i].GetComponent<Car>().lRotateFactor = 0.71f;
                        carList[i].GetComponent<Car>().rRotateFactor = 3.35f;
                        break;
                }

                if (carList[i].GetComponent<Car>().direction.Equals("null")) // 자동차의 방향 결정, 1번만 결정
                {
                    DecideCarDirection(carList[i]);
                }
            }
            // 3거리, 1번 도로가 없는 경우
            else if (gameObject.CompareTag("RSU_narrow_crossRoad3_1"))
            {
                // 차선(lineNum)에 따라
                switch (carList[i].GetComponent<Car>().lineNum)
                {
                    case 1:
                        carList[i].GetComponent<Car>().lRotateFactor = 0.97f;
                        carList[i].GetComponent<Car>().rRotateFactor = 1.60f;
                        break;
                    case 2:
                        carList[i].GetComponent<Car>().lRotateFactor = 0.71f;
                        carList[i].GetComponent<Car>().rRotateFactor = 3.35f;
                        break;
                }

                if (carList[i].GetComponent<Car>().direction.Equals("null")) // 자동차의 방향 결정, 1번만 결정
                {
                    DecideCarDirection(carList[i], 1);
                }
            }
            // 3거리, 2번 도로가 없는 경우
            else if (gameObject.CompareTag("RSU_narrow_crossRoad3_2"))
            {
                // 차선(lineNum)에 따라
                switch (carList[i].GetComponent<Car>().lineNum)
                {
                    case 1:
                        carList[i].GetComponent<Car>().lRotateFactor = 0.97f;
                        carList[i].GetComponent<Car>().rRotateFactor = 1.60f;
                        break;
                    case 2:
                        carList[i].GetComponent<Car>().lRotateFactor = 0.71f;
                        carList[i].GetComponent<Car>().rRotateFactor = 3.35f;
                        break;
                }

                if (carList[i].GetComponent<Car>().direction.Equals("null")) // 자동차의 방향 결정, 1번만 결정
                {
                    DecideCarDirection(carList[i], 2);
                }
            }
            // 3거리, 3번 도로가 없는 경우
            else if (gameObject.CompareTag("RSU_narrow_crossRoad3_3"))
            {
                // 차선(lineNum)에 따라
                switch (carList[i].GetComponent<Car>().lineNum)
                {
                    case 1:
                        carList[i].GetComponent<Car>().lRotateFactor = 0.97f;
                        carList[i].GetComponent<Car>().rRotateFactor = 1.60f;
                        break;
                    case 2:
                        carList[i].GetComponent<Car>().lRotateFactor = 0.71f;
                        carList[i].GetComponent<Car>().rRotateFactor = 3.35f;
                        break;
                }

                if (carList[i].GetComponent<Car>().direction.Equals("null")) // 자동차의 방향 결정, 1번만 결정
                {
                    DecideCarDirection(carList[i], 3);
                }
            }
            // 3거리, 4번 도로가 없는 경우
            else if (gameObject.CompareTag("RSU_narrow_crossRoad3_4"))
            {
                // 차선(lineNum)에 따라
                switch (carList[i].GetComponent<Car>().lineNum)
                {
                    case 1:
                        carList[i].GetComponent<Car>().lRotateFactor = 0.97f;
                        carList[i].GetComponent<Car>().rRotateFactor = 1.60f;
                        break;
                    case 2:
                        carList[i].GetComponent<Car>().lRotateFactor = 0.71f;
                        carList[i].GetComponent<Car>().rRotateFactor = 3.35f;
                        break;
                }

                if (carList[i].GetComponent<Car>().direction.Equals("null")) // 자동차의 방향 결정, 1번만 결정
                {
                    DecideCarDirection(carList[i], 4);
                }
            }
        }
    }
    private void signalFunc(int beforeRotate)
    {
        // carList 배열에 속해있는 모든 오브젝트(Collider)에 대하여
        for (int i = 0; i < carListNum; i++)
        {
            // 차량 오브젝트에 대해서만 실행하기 위해서
            if (!carList[i].CompareTag("SportCar2"))
            {
                continue;
            }

            if (carList[i].GetComponent<Car>().beforeRotation == beforeRotate)
            {
                carList[i].GetComponent<Car>().signal = true;
            }
            else
            {
                carList[i].GetComponent<Car>().signal = false;
            }
        }
    }

    // 4거리 신호 발생
    private void InvokeTrafficSignal4()
    {
        if(temp % 4 == 0)
        {
            // 3번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(0);
            temp++;
        }
        else if(temp % 4 == 1)
        {
            // 2번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(90);
            temp++;
        }
        else if(temp % 4 == 2)
        {
            // 1번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(180);
            temp++;
        }
        else if(temp % 4 == 3)
        {
            // 4번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(270);
            temp = 0;
        }
    }

    // 3거리 신호 발생(1번 도로X)
    private void InvokeTrafficSignal3_1()
    {
        if (temp % 3 == 0)
        {
            // 3번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(0);
            temp++;
        }
        else if (temp % 3 == 1)
        {
            // 2번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(90);
            temp++;
        }
        else if(temp % 3 == 2)
        {
            // 4번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(270);
            temp = 0;
        }
    }

    // 3거리 신호 발생(2번 도로X)
    private void InvokeTrafficSignal3_2()
    {
        if (temp % 3 == 0)
        {
            // 3번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(0);
            temp++;
        }
        else if (temp % 3 == 1)
        {
            // 1번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(180);
            temp++;
        }
        else if (temp % 3 == 2)
        {
            // 4번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(270);
            temp = 0;
        }
    }

    // 3거리 신호 발생(3번 도로X)
    private void InvokeTrafficSignal3_3()
    {
        if (temp % 3 == 0)
        {
            // 2번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(90);
            temp++;
        }
        else if (temp % 3 == 1)
        {
            // 1번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(180);
            temp++;
        }
        else if (temp % 3 == 2)
        {
            // 4번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(270);
            temp = 0;
        }
    }

    // 3거리 신호 발생(4번 도로X)
    private void InvokeTrafficSignal3_4()
    {
        if (temp % 3 == 0)
        {
            // 2번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(90);
            temp++;
        }
        else if (temp % 3 == 1)
        {
            // 1번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(180);
            temp++;
        }
        else if (temp % 3 == 2)
        {
            // 3번 도로 차량의 신호를 켬, 나머지 도로 차량의 신호를 끔
            signalFunc(0);
            temp = 0;
        }
    }

    // 차량 방향 결정 함수
    private void DecideCarDirection(Collider car ,int vacantRoadNum = 0)
    {
        List<string> move;

        // 차량이 4거리에 있는 경우
        if (isCrossRoad4)
        {
            move = new List<string> { "straight", "left", "right" };
        }
        // 차량이 3거리에 있는 경우
        else
        {
            // 3거리에서 없는 도로 번호(vacantRoadNum)
            switch (vacantRoadNum)
            {
                case 1: // 1번 도로가 없는 경우
                    // 3번 도로에서 오는 경우
                    if (car.GetComponent<Car>().beforeRotation == 0 || car.GetComponent<Car>().beforeRotation == 360)
                    {
                        move = new List<string> { "left", "right" };
                    }
                    // 2번 도로에서 오는 경우
                    else if(car.GetComponent<Car>().beforeRotation == 90)
                    {
                        move = new List<string> { "straight", "right" };
                    }
                    // 4번 도로에서 오는 경우
                    else if (car.GetComponent<Car>().beforeRotation == 270)
                    {
                        move = new List<string> { "straight", "left" };
                    }
                    else
                    {
                        move = new List<string> { "straight", "left", "right" };
                    }
                    break;
                case 2: // 2번 도로가 없는 경우
                    // 4번 도로에서 오는 경우
                    if (car.GetComponent<Car>().beforeRotation == 270)
                    {
                        move = new List<string> { "left", "right" };
                    }
                    // 3번 도로에서 오는 경우
                    else if (car.GetComponent<Car>().beforeRotation == 0 || car.GetComponent<Car>().beforeRotation == 360)
                    {
                        move = new List<string> { "straight", "right" };
                    }
                    // 1번 도로에서 오는 경우
                    else if (car.GetComponent<Car>().beforeRotation == 180)
                    {
                        move = new List<string> { "straight", "left" };
                    }
                    else
                    {
                        move = new List<string> { "straight", "left", "right" };
                    }
                    break;
                case 3: // 3번 도로가 없는 경우
                    // 1번 도로에서 오는 경우
                    if (car.GetComponent<Car>().beforeRotation == 180)
                    {
                        move = new List<string> { "left", "right" };
                    }
                    // 4번 도로에서 오는 경우
                    else if (car.GetComponent<Car>().beforeRotation == 270)
                    {
                        move = new List<string> { "straight", "right" };
                    }
                    // 2번 도로에서 오는 경우
                    else if (car.GetComponent<Car>().beforeRotation == 90)
                    {
                        move = new List<string> { "straight", "left" };
                    }
                    else
                    {
                        move = new List<string> { "straight", "left", "right" };
                    }
                    break;
                case 4: // 4번 도로가 없는 경우
                    // 2번 도로에서 오는 경우
                    if (car.GetComponent<Car>().beforeRotation == 90)
                    {
                        move = new List<string> { "left", "right" };
                    }
                    // 1번 도로에서 오는 경우
                    else if (car.GetComponent<Car>().beforeRotation == 180)
                    {
                        move = new List<string> { "straight", "right" };
                    }
                    // 3번 도로에서 오는 경우
                    else if (car.GetComponent<Car>().beforeRotation == 0 || car.GetComponent<Car>().beforeRotation == 360)
                    {
                        move = new List<string> { "straight", "left" };
                    }
                    else
                    {
                        move = new List<string> { "straight", "left", "right" };
                    }
                    break;
                default:
                    move = new List<string> { "straight", "left", "right" };
                    break;
            }
        }

        car.GetComponent<Car>().direction = move[Random.Range(0, move.Count)];  // 차량에게 방향 전달
    }
}
