using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarList
{
    public GameObject[] car;
}

public class SpawnCar : MonoBehaviour
{
    public CarList[] QCar;       // Q-learning을 수행하는 차량
    private GameObject RSUObject;       // RSU 오브젝트

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 출발지에 Q_car 생성
    public void spawnQCar(int startRSU, int destRSU, int safetyLevel, int demandLevel)
    {
        int carPosIndex = getCarPosIndex(startRSU, destRSU, safetyLevel, demandLevel);

        // 차량의 생성 위치를 특정하지 못한 경우
        if(carPosIndex == -1)
        {
            Debug.Log("Spawn Car Error!");
            return;
        }

        GameObject newQCar = Instantiate(QCar[startRSU - 1].car[carPosIndex], QCar[startRSU - 1].car[carPosIndex].transform.position, QCar[startRSU - 1].car[carPosIndex].transform.rotation);
        newQCar.GetComponent<Car>().prev_RSU = startRSU;
        newQCar.GetComponent<Car>().dest_RSU = destRSU;
        newQCar.GetComponent<Car>().demandLevel = demandLevel;
        newQCar.GetComponent<Car>().safetyLevel = safetyLevel;
    }

    // 출발지에서 action(다음 RSU) 선택
    private int getCarPosIndex(int startRSU, int destRSU, int safetyLevel, int demandLevel)
    {
        RSUObject = GameObject.Find("RSU" + startRSU);      // 출발지 RSU 오브젝트 찾기
        switch (startRSU)
        {
            case 1:
                RSUObject.GetComponent<RSU1>().prev_RSU = startRSU;
                RSUObject.GetComponent<RSU1>().dest_RSU = destRSU;
                RSUObject.GetComponent<RSU1>().safetyLevel = safetyLevel;
                RSUObject.GetComponent<RSU1>().demandLevel = demandLevel;
                //return Random.Range(2, 6);
                switch (RSUObject.GetComponent<RSU1>().getNextAction())
                {
                    case 2:
                        return Random.Range(0, 2);
                    case 6:
                        return Random.Range(2, 6);
                    default:
                        return -1;
                }
            case 2:
                //RSUObject.GetComponent<RSU2>().prev_RSU = startRSU;
                RSUObject.GetComponent<RSU2>().dest_RSU = destRSU;
                RSUObject.GetComponent<RSU2>().safetyLevel = safetyLevel;
                RSUObject.GetComponent<RSU2>().demandLevel = demandLevel;
                switch (RSUObject.GetComponent<RSU2>().getNextAction())
                {
                    case 1:
                        return Random.Range(0, 2);
                    case 3:
                        return Random.Range(2, 4);
                    case 7:
                        return Random.Range(4, 6);
                    default:
                        return -1;
                }
            case 3:
                RSUObject.GetComponent<RSU3>().dest_RSU = destRSU;
                RSUObject.GetComponent<RSU3>().safetyLevel = safetyLevel;
                RSUObject.GetComponent<RSU3>().demandLevel = demandLevel;
                switch (RSUObject.GetComponent<RSU3>().getNextAction())
                {
                    case 2:
                        return Random.Range(0, 2);
                    case 4:
                        return Random.Range(2, 4);
                    case 8:
                        return Random.Range(4, 6);
                    default:
                        return -1;
                }
            case 4:
                RSUObject.GetComponent<RSU4>().dest_RSU = destRSU;
                RSUObject.GetComponent<RSU4>().safetyLevel = safetyLevel;
                RSUObject.GetComponent<RSU4>().demandLevel = demandLevel;
                switch (RSUObject.GetComponent<RSU4>().getNextAction())
                {
                    case 3:
                        return Random.Range(0, 2);
                    case 5:
                        return Random.Range(2, 6);
                    case 8:
                        return Random.Range(6, 8);
                    case 9:
                        return Random.Range(8, 10);
                    default:
                        return -1;
                }
            case 5:
                RSUObject.GetComponent<RSU5>().dest_RSU = destRSU;
                RSUObject.GetComponent<RSU5>().safetyLevel = safetyLevel;
                RSUObject.GetComponent<RSU5>().demandLevel = demandLevel;
                switch (RSUObject.GetComponent<RSU5>().getNextAction())
                {
                    case 4:
                        return Random.Range(0, 4);
                    case 10:
                        return Random.Range(4, 6);
                    default:
                        return -1;
                }
            case 6:
                return RSUObject.GetComponent<RSU6>().getNextAction();
            case 7:
                return RSUObject.GetComponent<RSU7>().getNextAction();
            case 8:
                return RSUObject.GetComponent<RSU8>().getNextAction();
            case 9:
                return RSUObject.GetComponent<RSU9>().getNextAction();
            case 10:
                return RSUObject.GetComponent<RSU10>().getNextAction();
            case 11:
                return RSUObject.GetComponent<RSU11>().getNextAction();
            case 12:
                return RSUObject.GetComponent<RSU12>().getNextAction();
            case 13:
                return RSUObject.GetComponent<RSU13>().getNextAction();
            case 14:
                return RSUObject.GetComponent<RSU14>().getNextAction();
            case 15:
                return RSUObject.GetComponent<RSU15>().getNextAction();
            case 16:
                return RSUObject.GetComponent<RSU16>().getNextAction();
            case 17:
                return RSUObject.GetComponent<RSU17>().getNextAction();
            case 18:
                return RSUObject.GetComponent<RSU18>().getNextAction();
            case 19:
                return RSUObject.GetComponent<RSU19>().getNextAction();
            case 20:
                return RSUObject.GetComponent<RSU20>().getNextAction();
            case 21:
                return RSUObject.GetComponent<RSU21>().getNextAction();
            case 22:
                return RSUObject.GetComponent<RSU22>().getNextAction();
            case 23:
                return RSUObject.GetComponent<RSU23>().getNextAction();
            case 24:
                return RSUObject.GetComponent<RSU24>().getNextAction();
            case 25:
                return RSUObject.GetComponent<RSU25>().getNextAction();
            default:
                return 0;
        }
    }
}
