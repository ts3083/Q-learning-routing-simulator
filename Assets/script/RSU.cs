using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSU : MonoBehaviour
{
    private List<GameObject> carList;   // 자동차 GameObject 리스트
    private float RSU_effectRange = 20;  // RSU 영향 범위

    // Start is called before the first frame update
    void Start()
    {
        carList = new List<GameObject>(GameObject.FindGameObjectsWithTag("SportCar2"));
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
                            car.GetComponent<Car>().lRotateFactor = 2.77f;
                            car.GetComponent<Car>().rRotateFactor = 1.65f;
                            break;
                        case 2:
                            car.GetComponent<Car>().lRotateFactor = 1.44f;
                            car.GetComponent<Car>().rRotateFactor = 3.88f;
                            break;
                    }
                }
            }
            else
            {
                car.GetComponent<Car>().RSU_type = "unknown";
            }
        }
    }
}
