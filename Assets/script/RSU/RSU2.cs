using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSU2 : MonoBehaviour
{
    private int current_RSU = 2;        // 현재 RSU
    private float RSU_effectRange = 20f;        // RSU 영향 범위

    private Collider[] carList;     // RSU 영향 범위 내의 차량 리스트, 배열 내의 모든 오브젝트가 차량이 아님!
    private int carListNum;     // 차량 리스트 내의 차량 수, 배열 내의 모든 오브젝트가 차량이 아님!

    private const int stateNum = 25;     // state(destination RSU) 수
    private const int actionNum = 3;        // action(neighbor RSU) 수

    public int dest_RSU;       // destination RSU, 차량이 넘겨주는 정보
    private int actionIndex;        // Q-table에서 해당 action(neighbor RSU)의 index
    public int demandLevel;     // Demand Level, 차량이 넘겨주는 정보
    public int safetyLevel;        // Safety Level, 차량이 넘겨주는 정보
    public int prev_RSU;       // 이전 RSU
    private int next_RSU; // 다음 RSU
    private int line_num; // 차량 차선 번호

    private float epsilon;       // ϵ-greedy의 epsilon 값
    private int epsilonDecimalPointNum = 1;     // ϵ(epsilon) 소수점 자리수

    // [state(destination RSU) 수, action(neighbor RUS) 수], Demand Level [time, energy]
    public float[,,] Q_table = new float[5, stateNum, actionNum];       // Demand Level 1, [100, 0] / Demand Level 2, [75, 25] / Demand Level 3, [50, 50] / Demand Level 4, [25, 75] / Demand Level 5, [0, 100]

    // [action(neighbor RSU) 수], 각각의 action의 safety level을 저장
    private int[] actions_SL = new int[actionNum] { 1, 1, 1 };

    // [action(neightbor RSU) 수], {각각의 action에 대응되는 RSU 번호를 저장}
    private int[] actions_RSU = new int[actionNum] { 1, 3, 7 };

    // RSU1방향 좌표 저장
    private Vector3[] forward_RSU1 = new Vector3[3] { new Vector3(0, 0, 0), new Vector3(-7.18f, 0.427f, -313.79f), new Vector3(-7.18f, 0.427f, -311.54f) };

    // RSU3방향 좌표 저장
    private Vector3[] forward_RSU3 = new Vector3[3] { new Vector3(0, 0, 0), new Vector3(7.14f, 0.427f, -316.25f), new Vector3(7.14f, 0.427f, -318.4f) };

    // RSU7방향 좌표 저장
    private Vector3[] forward_RSU7 = new Vector3[3] { new Vector3(0, 0, 0), new Vector3(1.23f, 0.427f, -307.81f), new Vector3(3.36f, 0.427f, -307.81f) };

    // Start is called before the first frame update
    void Start()
    {
        // Q-table 초기화(float 최솟값), 0으로 초기화 시 필요 X
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < stateNum; j++)
            {
                // state(destination RSU)가 자기 자신인 경우 스킵, 0으로 초기화 시 필요 X, RSU마다 수정 필요!
                if(j == current_RSU - 1)
                {
                    continue;
                }

                for (int k = 0; k < actionNum; k++)
                {
                    Q_table[i, j, k] = -15.0f;
                }
            }
        }
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
            if (!carList[i].CompareTag("Q_car"))
            {
                continue;
            }

            // 차량 오브젝트의 state(destination) RSU가 현재 RSU인 경우
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
                    next_RSU = getNextAction();
                    carList[i].GetComponent<Car>().direction = getNextDirection(next_RSU);
                    carList[i].GetComponent<Car>().position = getPosition(next_RSU);
                    carList[i].GetComponent<Car>().lineNum = line_num;
                    carList[i].GetComponent<Car>().curActionIndex = actionIndex;       // Q-table에서 해당 action(neighbor RSU)의 index를 Car script로 넘겨줌
                    carList[i].GetComponent<Car>().cur_RSU = current_RSU;        // 현재 RSU 번호로 초기화
                    for (int j = 0; j < 5; j++)      // state(destination RSU) 별 max Q-value를 넘겨줌
                    {
                        carList[i].GetComponent<Car>().nextMaxQ_value[j] = getMaxQ_value(j);
                    }
                }
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
        // 차량이 RSU 1에서 온 경우
        if(prev_RSU == 1)
        {
            switch (RSU_num)
            {
                case 3:
                    return "straight";
                case 7:
                    return "left";
                default:
                    return "null";
            }
        }
        //차량이 RSU 3에서 온 경우
        else if (prev_RSU == 3)
        {
            switch (RSU_num)
            {
                case 1:
                    return "straight";
                case 7:
                    return "right";
                default:
                    return "null";
            }
        }
        // 그 이외의 경우(차량이 RSU 7에서 온 경우)
        else
        {
            switch (RSU_num)
            {
                case 1:
                    return "right";
                case 3:
                    return "left";
                default:
                    return "null";
            }
        }
    }

    private Vector3 getPosition(int RSU_num)
    {
        // 차량이 RSU 1에서 온 경우
        if (prev_RSU == 1)
        {
            switch (RSU_num)
            {
                case 3:
                    return forward_RSU3[line_num];
                case 7:
                    return forward_RSU7[line_num];
                default:
                    return forward_RSU3[line_num];
            }
        }
        //차량이 RSU 3에서 온 경우
        else if (prev_RSU == 3)
        {
            switch (RSU_num)
            {
                case 1:
                    return forward_RSU1[line_num];
                case 7:
                    return forward_RSU7[line_num];
                default:
                    return forward_RSU1[line_num];
            }
        }
        // 그 이외의 경우(차량이 RSU 7에서 온 경우)
        else
        {
            switch (RSU_num)
            {
                case 1:
                    return forward_RSU1[line_num];
                case 3:
                    return forward_RSU3[line_num];
                default:
                    return forward_RSU1[line_num];
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
