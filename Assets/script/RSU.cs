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

    // Start is called before the first frame update
    void Start()
    {
        carList = new List<GameObject>(GameObject.FindGameObjectsWithTag("SportCar2"));
        if (!gameObject.CompareTag("RSU_narrow_corner"))
        {
            if (gameObject.CompareTag("RSU_narrow_crossRoad4"))
            {
                isCrossRoad4 = true;
            }
            else
            {
                isCrossRoad4 = false;
            }
            
            InvokeRepeating("InvokeTrafficSignal", 1f, 2f);
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
                if (gameObject.CompareTag("RSU_narrow_corner"))
                {
                    car.GetComponent<Car>().RSU_type = "narrow_corner";
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
                }
                else if (gameObject.CompareTag("RSU_narrow_crossRoad4"))
                {
                    car.GetComponent<Car>().RSU_type = "narrow_crossRoad4";
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
                }
                //else if (gameObject.CompareTag("RSU_wide_crossRoad3"))
                //{
                //    car.GetComponent<Car>().RSU_type = "narrow_crossRoad4";
                //    switch (car.GetComponent<Car>().lineNum)
                //    {
                //        case 1:
                //            car.GetComponent<Car>().lRotateFactor = 0.97f;
                //            car.GetComponent<Car>().rRotateFactor = 1.60f;
                //            break;
                //        case 2:
                //            car.GetComponent<Car>().lRotateFactor = 0.71f;
                //            car.GetComponent<Car>().rRotateFactor = 3.35f;
                //            break;
                //    }

                //    car.GetComponent<Car>().signal = signal;
                //    car.GetComponent<Car>().signal_str = signal_str;
                //}
            }
            else
            {
                car.GetComponent<Car>().RSU_type = "unknown";
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
            if (temp % 2 == 0) // 1에서 3, 3에서 1로 직진 & 1에서 4, 3에서 2로 좌회전, 우회,
            {
                signal_str.Clear();
                signal_str.Add("straight1-3");
                signal_str.Add("left1-4");
                signal_str.Add("left3-2");
                signal_str.Add("right");
            }
            else if (temp % 2 == 1) // 2에서 4, 4에서 2로 직진 & 2에서 1, 4에서 3으로 좌회전 우회
            {
                signal_str.Clear();
                signal_str.Add("straight2-4");
                signal_str.Add("left2-1");
                signal_str.Add("left4-3");
                signal_str.Add("right");
            }
        }
    }
}
