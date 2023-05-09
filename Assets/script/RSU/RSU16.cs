using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSU16 : MonoBehaviour
{
    private int current_RSU = 16;
    private float RSU_effectRange = 30f;        // RSU 영향 범위

    private Collider[] carList;     // RSU 영향 범위 내의 차량 리스트, 배열 내의 모든 오브젝트가 차량이 아님!
    private int carListNum;     // 차량 리스트 내의 차량 수, 배열 내의 모든 오브젝트가 차량이 아님!

    private const int stateNum = 25;     // state(destination RSU) 수
    private const int actionNum = 3;        // action(neighbor RSU) 수

    private int dest_RSU;       // destination RSU, 차량이 넘겨주는 정보
    private int actionIndex;        // Q-table에서 해당 action(neighbor RSU)의 index
    private int demandLevel;     // Demand Level, 차량이 넘겨주는 정보
    private int safetyLevel;        // Safety Level, 차량이 넘겨주는 정보
    public int prev_RSU;       // 이전 RSU
    private int next_RSU; // 다음 RSU
    private int line_num; // 차량 차선 번호

    private float epsilon;

    // [state(destination RSU) 수, action(neighbor RUS) 수], Demand Level [time, energy]
    public float[,,] Q_table = new float[5, stateNum, actionNum];       // Demand Level 1, [100, 0] / Demand Level 2, [75, 25] / Demand Level 3, [50, 50] / Demand Level 4, [25, 75] / Demand Level 5, [0, 100]

    // [action(neighbor RSU) 수], 각각의 action의 safety level을 저장
    private int[] actions_SL = new int[actionNum] { 1, 1, 1 };

    // [action(neightbor RSU) 수], {각각의 action에 대응되는 RSU 번호를 저장}
    private int[] actions_RSU = new int[actionNum] { 11, 17, 21 };

    // RSU21방향 좌표 저장
    private Vector3[] forward_RSU21 = new Vector3[3] { new Vector3(0, 0, 0), new Vector3(-318.76f, 0.427f, 622.29f), new Vector3(-316.54f, 0.427f, 622.29f) };
    
    // RSU17방향 좌표 저장
    private Vector3[] forward_RSU17 = new Vector3[3] { new Vector3(0, 0, 0), new Vector3(-312.72f, 0.427f, 613.76f), new Vector3(-312.72f, 0.427f, 611.49f) };
    
    // RSU11방향 좌표 저장
    private Vector3[] forward_RSU11 = new Vector3[3] { new Vector3(0, 0, 0), new Vector3(-321.2f, 0.427f, 607.71f), new Vector3(-323.42f, 0.427f, 607.71f) };

    private crossroadMove DummyCarMove = new();     // DummyCar의 교차로에서 이동을 결정

    // Start is called before the first frame update
    void Start()
    {
        // Q-table 초기화(float 최솟값), 0으로 초기화 시 필요 X
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < stateNum; j++)
            {
                // state(destination RSU)가 자기 자신인 경우 스킵, 0으로 초기화 시 필요 X, RSU마다 수정 필요!
                if (j == current_RSU - 1)
                {
                    continue;
                }

                for (int k = 0; k < actionNum; k++)
                {
                    Q_table[i, j, k] = -15.0f;
                }
            }
        }

        Q_table[4, 17, 2] = float.MinValue; // RSU21로 이동하지 못하게 설정
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        carList = Physics.OverlapSphere(transform.position, RSU_effectRange);       // RSU_effectRange 범위 내의 모든 오브젝트(Collider)를 가져옴
        carListNum = carList.Length;        // carList 배열의 크기
        ifDistInRange();
    }

    // RSU_effectRange 범위 내로 들어온 차량들에게 적용
    private void ifDistInRange()
    {
        // carList 배열에 속해있는 모든 오브젝트(Collider)에 대하여
        for (int i = 0; i < carListNum; i++)
        {
            // 차량 오브젝트에 대해서만 실행
            if (carList[i].CompareTag("Q_car"))
            {
                // 차량 오브젝트의 state(destination) RSU가 현재 RSU인 경우, 각각의 RSU에서 수정
                if (carList[i].GetComponent<Car>().dest_RSU == current_RSU)
                {
                    carList[i].GetComponent<Car>().isEnd = true;
                    carList[i].GetComponent<Car>().cur_RSU = current_RSU;        // 현재(목적지) RSU 번호로 초기화
                }
                else
                {
                    if (carList[i].GetComponent<Car>().direction == "null")
                    {
                        dest_RSU = carList[i].GetComponent<Car>().dest_RSU;
                        demandLevel = carList[i].GetComponent<Car>().demandLevel;
                        safetyLevel = carList[i].GetComponent<Car>().safetyLevel;
                        prev_RSU = carList[i].GetComponent<Car>().prev_RSU;
                        line_num = carList[i].GetComponent<Car>().lineNum;
                        carList[i].GetComponent<Car>().prev_lineNum = line_num; // 차량의 이전 차선 저장
                        next_RSU = getNextAction();
                        carList[i].GetComponent<Car>().direction = getNextDirection(next_RSU);
                        carList[i].GetComponent<Car>().position = getPosition(next_RSU);
                        carList[i].GetComponent<Car>().lineNum = line_num; // 방향 이동 후 car의 line_num 저장
                        carList[i].GetComponent<Car>().curActionIndex = actionIndex;
                        carList[i].GetComponent<Car>().cur_RSU = current_RSU;        // 현재 RSU 번호로 초기화
                        carList[i].GetComponent<Car>().next_RSU = next_RSU;
                        for (int j = 0; j < 5; j++)      // state(destination RSU) 별 max Q-value를 넘겨줌
                        {
                            carList[i].GetComponent<Car>().nextMaxQ_value[j] = getMaxQ_value(j);
                        }
                    }
                }
            }
            else if (carList[i].CompareTag("DummyCar"))
            {
                if (carList[i].GetComponent<DummyCar>().direction == "null")
                {
                    prev_RSU = carList[i].GetComponent<DummyCar>().prev_RSU;
                    //next_RSU = carList[i].GetComponent<DummyCar>().routeList[carList[i].GetComponent<DummyCar>().routeIndex];
                    line_num = carList[i].GetComponent<DummyCar>().lineNum;
                    carList[i].GetComponent<DummyCar>().prev_lineNum = line_num; // 차량의 이전 차선 저장
                    next_RSU = DummyCarMove.DecideNextRSU(prev_RSU, current_RSU);
                    carList[i].GetComponent<DummyCar>().direction = getNextDirection(next_RSU);
                    carList[i].GetComponent<DummyCar>().position = getPosition(next_RSU);
                    carList[i].GetComponent<DummyCar>().lineNum = line_num;     // 방향 이동 후 car의 line_num 저장
                    carList[i].GetComponent<DummyCar>().cur_RSU = current_RSU;        // 현재 RSU 번호로 초기화
                    carList[i].GetComponent<DummyCar>().next_RSU = next_RSU;
                }
            }
            else
            {
                continue;
            }
        }
    }

    // ϵ-greedy 방법에 따라 Q-table에서 다음 action(neighbor RSU)을 선택
    public int getNextAction()
    {
        // 해당 action의 index 값 저장
        actionIndex = 0;
        epsilon = GameObject.Find("RSU1").GetComponent<RSU1>().epsilon;

        // ϵ 확률로 무작위 action(negibor RSU)을 선택
        if (Random.Range(0.00000f, 1.00000f) < epsilon)
        {
            // 무작위로 선택한 action(neighbor RSU)이 이전 RSU가 아닌 경우
            do
            {
                actionIndex = Random.Range(0, actionNum);
            }
            while (actions_RSU[actionIndex] == prev_RSU);
        }
        // 그 이외의 경우(1 - ϵ 확률로 가장 큰 Q 값을 가지고 있는 action을 선택)
        else
        {
            // maxQ 값 저장, 가장 작은 float 값으로 초기화
            float maxQ = float.MinValue;

            for (int i = 0; i < actionNum; i++)
            {
                // Safety Level을 만족하지 못하는 action 선택X
                if (actions_SL[i] < safetyLevel || actions_RSU[i] == prev_RSU)
                {
                    continue;
                }

                if (maxQ < Q_table[demandLevel - 1, dest_RSU - 1, i])
                {
                    maxQ = Q_table[demandLevel - 1, dest_RSU - 1, i];
                    actionIndex = i;
                }
            }
        }

        return actions_RSU[actionIndex];
    }

    // 이전 RSU에 따라 getNextAction() 함수에서 반환되는 다음 RSU로 가기 위한 direction을 반환, 각각의 RSU에서 수정 필요
    public string getNextDirection(int RSU_num)
    {
        if (prev_RSU == 11)
        {
            switch (RSU_num)
            {
                case 21:
                    return "straight";
                case 17:
                    return "right";
                default:
                    return "null";
            }
        }
        else if (prev_RSU == 17)
        {
            switch (RSU_num)
            {
                case 11:
                    return "left";
                case 21:
                    return "right";
                default:
                    return "null";
            }
        }
        else
        {
            switch (RSU_num)
            {
                case 11:
                    return "straight";
                case 17:
                    return "left";
                default:
                    return "null";
            }
        }
    }

    private Vector3 getPosition(int RSU_num)
    {
        if (prev_RSU == 11)
        {
            switch (RSU_num)
            {
                case 21:
                    return forward_RSU21[line_num];
                case 17:
                    return forward_RSU17[line_num];
                default:
                    return forward_RSU21[line_num];
            }
        }
        else if (prev_RSU == 17)
        {
            switch (RSU_num)
            {
                case 11:
                    return forward_RSU11[line_num];
                case 21:
                    return forward_RSU21[line_num];
                default:
                    return forward_RSU11[line_num];
            }
        }
        else
        {
            switch (RSU_num)
            {
                case 11:
                    return forward_RSU11[line_num];
                case 17:
                    return forward_RSU17[line_num];
                default:
                    return forward_RSU11[line_num];
            }
        }
    }

    // Demand Level 별 max Q-value, DL은 실제 demand level - 1
    private float getMaxQ_value(int DL)
    {
        float maxQ = float.MinValue;

        for (int i = 0; i < actionNum; i++)
        {
            if (maxQ < Q_table[DL, dest_RSU - 1, i])
            {
                maxQ = Q_table[DL, dest_RSU - 1, i];
            }
        }

        return maxQ;
    }
}
