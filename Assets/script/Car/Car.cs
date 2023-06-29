using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

public class Car : MonoBehaviour
{
    public bool isEnd = false;     // 도착지 여부 저장

    public int start_RSU;       // start RSU
    public int dest_RSU;        // destination RSU
    public int demandLevel;     // Demand Level
    public int safetyLevel;        // Safety Level

    public int curActionIndex;        // Q-table에서 해당 action(neighbor RSU)의 index
    public int prevActionIndex;        // Q-table에서 해당 action(neighbor RSU)의 index

    public int cur_RSU;        // 현재 RSU
    public int prev_RSU;        // 이전 RSU
    public int next_RSU; // 다음 RSU

    public float totalTime = 0.0f;      // 차량이 출발지에서 목적지까지 총 소요된 시간
    public float timer = 0.0f;      // 교차로 사이에 이동 시간 측정
    private bool timer_on = false;      // 시간 측정 시작 여부

    public float totalEnergy = 0.0f;        // 차량이 출발지에서 목적지까지 총 소모된 에너지
    public float energy = 0.0f;        // 교차로 사이를 이동하는데 필요한 에너지
    private int m = 1500;       // mass of vehicle(kg)
    private float g = 9.81f;        // acceleration of gravity(m/s^2)
    public int theta = 0;      // 현재 선택한 경로의 경사각도
    private float p = 1.28f;        // mass of air(kg/m^3)
    private float Cw = 0.35f;       // drag coefficient
    private float A = 1.8f;     // frontal area of the car(m^2)
    private float u = 0.005f;       // rolling resistance coefficient
    private int Nt = 5;       // normalization for time
    private int Ne = 5000;       // normalization for energy
    private float SRP_decrease_factor = 0.1f;       // SRP(Slope Resistance Power) 감소 인자

    private float alpha = 0.1f;     // Q-learning의 learning rate
    private float gamma = 0.9f;     // Q-learning의 discount factor
    private float road_length;

    private float[] RSU_Q_table = new float[5];       // 이전 RSU의 특정 state(destination RSU)에서의 Q-table
    public float[] nextMaxQ_value = new float[5];       // 현재 RSU의 특정 state(destination RSU)에서의 max Q-value
    private float arrivalReward = 50.0f;        // 차량이 목적지에 도착했을 때 reward
    private GameObject spawnObject;     // SpawnCar script를 컨포넌트로 가지고 있는 오브젝트

    // 차량 속도 선언 - 10, 15, 20
    private int[] speed = new int[] { 10, 20 };
    private static int init_speed = 10;     // 초기 속도(10m/s)
    public int avg_speed = init_speed;      // 현재 차량의 위치에 따른 속도가 다름 => current speed 변수 선언

    public int beforeRotation;      // 회전 이전의 rotation
    Vector3 startPosition, startRotation;
    public string[][] driveSelect;
    public string[][] rsuSelect;
    public string[] a;
    private int speedLimit;     // 속도 제한

    public string direction = "null";       // 차량 이동방향
    public Vector3 position;        // 월드 좌표를 기준으로 이동시킬 차량의 위치

    public int prev_lineNum;
    public int lineNum;     // 차량이 위치한 차선

    public bool isCarInfoUpdateNeeded = false;     // 차량의 RSU, index 정보 update 필요 여부

    void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        BackTriggerSettingBySpeed(init_speed);
        beforeRotation = (int)transform.eulerAngles.y;
    }

    private void Start()
    {
        spawnObject = GameObject.Find("ProcessObject");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Time.deltaTime은 화면이 한번 깜빡이는 시간 = 한 프레임의 시간
        // 화면을 60번 깜빡이면 (초당 60프레) 1/60이 들어간다
        Time.timeScale = 3f;
        transform.position += transform.forward * avg_speed * Time.deltaTime;       // 차량 이동

        // 시간 측정
        if (timer_on)
        {
            timer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 시간 측정 시작
        if (other.CompareTag("NarrowRoadEnterAngleO") || other.CompareTag("NarrowRoadEnterAngleX") || other.CompareTag("WideRoadEnter"))
        {
            timer_on = true;        // 시간 측정 시작
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

        if (other.CompareTag("CrossRoad") && timer_on)
        {
            timer_on = false;       // 시간 측정 끝

            // 목적지에 도착한 경우
            if (isEnd)
            {
                UpdateRSU(arrivalReward);

                //Debug.Log("RSU" + start_RSU + " → RSU" + dest_RSU + "의 총 (시간, 에너지): (" + totalTime + ", " + totalEnergy + ")");
                RSU_parameters.writeMaxQValue(start_RSU, dest_RSU, demandLevel);        // source RSU의 MaxQ 값을 기록
                RSU_parameters.decaying_epsilonValue();

                Destroy(gameObject);        // 목적지에 도착한 차량 제거
                spawnObject.GetComponent<SpawnCar>().spawnQCar(start_RSU, dest_RSU, safetyLevel, demandLevel);
            }
            else
            {
                // 이전 RSU의 Q-table update
                UpdateRSU();

                timer = 0.0f;       // 다시 0으로 초기화
                energy = 0.0f;      // 다시 0으로 초기화

                isCarInfoUpdateNeeded = true;

                transform.position = position;
                if (direction.Contains("left") || direction.Contains("right"))
                {
                    new_drive(direction);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NarrowRoadExit") || other.CompareTag("WideRoadExit"))
        {
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
                    if (other.GetComponent<TrafficLight>().lightOn_lineNum.Equals(prev_lineNum) || isEnd)
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
        // cur_RSU 또는 next_RSU 값을 RSU로 부터 받지 못한 경우
        if (cur_RSU == 0)
        {
            return true;
        }

        if (isEnd)
        {
            return false;
        }

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
            BackTriggerSettingBySpeed(speed[0]);
            prevActionIndex = curActionIndex;
            curActionIndex = -1;
            prev_RSU = cur_RSU;
            next_RSU = cur_RSU = 0;
            theta = 0;      // 경사각 0
            isCarInfoUpdateNeeded = false;
            road_length = 300;
        }

        // speed 15 제한 도로에 진입하는 경우, 경사각 O
        if (isCarInfoUpdateNeeded && other.CompareTag("NarrowRoadEnterAngleO"))
        {
            BackTriggerSettingBySpeed(speed[0]);
            prevActionIndex = curActionIndex;
            curActionIndex = -1;
            prev_RSU = cur_RSU;
            next_RSU = cur_RSU = 0;
            theta = 10;     // 경사각 10
            isCarInfoUpdateNeeded = false;
            road_length = 424.3f;
        }

        // speed 20 제한 도로에 진입하는 경우, 경사각 X
        if (isCarInfoUpdateNeeded && other.CompareTag("WideRoadEnter"))
        {
            BackTriggerSettingBySpeed(speed[1]);
            prevActionIndex = curActionIndex;
            curActionIndex = -1;
            prev_RSU = cur_RSU;
            next_RSU = cur_RSU = 0;
            theta = 0;      // 경사각 0
            isCarInfoUpdateNeeded = false;
            road_length = 300;
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
                //Debug.Log(90);
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

    // 차량 후면 트리거(차량간 거리 조절) 위치 조정
    public void BackTriggerSettingBySpeed(int speed_)
    {
        // 초기 속도(init_speed)인 경우
        if (speed_ == init_speed)
        {
            avg_speed = init_speed;
            speedLimit = init_speed;
        }
        // 속도가 15인 경우
        else if (speed_ == speed[0])
        {
            avg_speed = speed[0];
            speedLimit = speed[0];
        }
        // 속도가 30인 경우
        else if (speed_ == speed[1])
        {
            avg_speed = speed[1];
            speedLimit = speed[1];

        }
        else // 속도가 0인 경우
        {
            avg_speed = 0;
            speedLimit = init_speed;
        }
    }

    // 차량에서 이전 RSU의 Q-table update
    private void UpdateRSU(float additionalReward = 0.0f)
    {
        float reward;       // Q-learning의 reward
        float Wt, We;       // Demand Level에 따른 가중치

        // 에너지 계산
        CalEnergy();

        //Debug.Log("RSU" + prev_RSU + " → " + cur_RSU + "의 (시간, 에너지): (" + timer + ", " + energy + ")");

        GetQ_table();

        for (int i = 0; i < 5; i++)
        {
            // Demand Level에 따른 가중치 계산
            Wt = 1.0f - 0.25f * i;
            We = 0.25f * i;

            reward = -(Wt * timer / Nt + We * energy / Ne) + additionalReward;       // Q-learning의 reward 계산
            //Debug.Log("RSU" + prev_RSU + " → " + cur_RSU + "의 reward[DL: " + (i + 1) + "]: " + reward);
            RSU_Q_table[i] = (1 - alpha) * RSU_Q_table[i] + alpha * (reward + gamma * nextMaxQ_value[i]);
        }
        //Debug.Log("RSU" + prev_RSU + " → RSU" + cur_RSU + "(DL 1 ~ 5): " + RSU_Q_table[0] + ", " + RSU_Q_table[1] + ", " + RSU_Q_table[2] + ", " + RSU_Q_table[3] + ", " + RSU_Q_table[4]);

        UpdateQ_table();
    }

    // 에너지 계산
    private void CalEnergy()
    {
        float avg_speed = road_length / timer;

        // Slope Resistance Power
        float SRP = m * g * avg_speed * Mathf.Sin(theta * Mathf.Deg2Rad) * SRP_decrease_factor;
        //float SRP = m * g * avg_speed * Mathf.Sin(theta * Mathf.Deg2Rad);
        //Debug.Log("(RSU" + prev_RSU + " → " + cur_RSU + ") SRP: " + SRP + "(time: " + timer + ")");

        // Air Resistance Power
        float ARP = 0.5f * p * Cw * A * Mathf.Pow(avg_speed, 3);
        //Debug.Log("(RSU" + prev_RSU + " → " + cur_RSU + ") ARP: " + ARP + "(time: " + timer + ")");

        // Rolling Resistance Power
        float RRP = u * m * g * avg_speed;
        //Debug.Log("(RSU" + prev_RSU + " → " + cur_RSU + ") RRP: " + RRP + "(time: " + timer + ")");

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
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU1>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
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
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU4>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 5:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU5>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 6:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU6>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 7:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU7>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 8:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU8>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 9:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU9>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 10:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU10>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 11:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU11>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 12:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU12>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 13:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU13>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 14:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU14>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 15:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU15>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 16:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU16>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 17:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU17>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 18:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU18>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 19:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU19>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 20:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU20>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 21:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU21>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 22:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU22>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 23:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU23>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 24:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU24>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
                break;
            case 25:
                for (int i = 0; i < 5; i++)
                {
                    RSU_Q_table[i] = RSU.GetComponent<RSU25>().Q_table[i, dest_RSU - 1, prevActionIndex];
                }
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
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU1>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
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
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU4>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 5:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU5>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 6:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU6>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 7:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU7>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 8:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU8>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 9:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU9>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 10:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU10>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 11:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU11>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 12:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU12>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 13:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU13>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 14:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU14>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 15:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU15>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 16:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU16>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 17:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU17>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 18:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU18>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 19:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU19>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 20:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU20>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 21:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU21>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 22:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU22>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 23:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU23>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 24:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU24>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            case 25:
                for (int i = 0; i < 5; i++)
                {
                    RSU.GetComponent<RSU25>().Q_table[i, dest_RSU - 1, prevActionIndex] = RSU_Q_table[i];
                }
                break;
            default:
                break;
        }
    }
}