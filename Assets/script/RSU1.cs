using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSU1 : MonoBehaviour
{
    private float RSU_effectRange = 20f;        // RSU 영향 범위

    private Collider[] carList;     // RSU 영향 범위 내의 차량 리스트, 배열 내의 모든 오브젝트가 차량이 아님!
    private int carListNum;     // 차량 리스트 내의 차량 수, 배열 내의 모든 오브젝트가 차량이 아님!

    private const int stateNum = 24;     // state(destination RSU) 수 - 1 [자기 자신 제외]
    private const int actionNum = 2;        // action(neighbor RSU) 수

    private int dest_RSU;       // destination RSU, 차량이 넘겨주는 정보
    private int demandLevel;     // Demand Level, 차량이 넘겨주는 정보
    private int safetyLevel;        // Safety Level, 차량이 넘겨주는 정보
    private int prev_RSU;       // 이전 RSU

    private float epsilon = 0.3f;       // ϵ-greedy의 epsilon 값

    // [state(destination RSU) 수, action(neighbor RUS) 수], Demand Level [time, energy]
    private float[,,] Q_table = new float[5, stateNum, actionNum];       // Demand Level 1, [100, 0] / Demand Level 2, [75, 25] / Demand Level 3, [50, 50] / Demand Level 4, [25, 75] / Demand Level 5, [0, 100]

    // [action(neighbor RSU) 수], 각각의 action의 safety level을 저장
    public int[] actions_SL = new int[actionNum] {1, 1};

    // [action(neightbor RSU) 수], {각각의 action에 대응되는 RSU 번호를 저장}
    private int[] actions_RSU = new int[actionNum] { 2, 6 };

    // [state(destination RSU) 수], {각각의 state에 대응되는 RSU 번호를 저장}
    private int[] state_RSU = new int[stateNum] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };

    // Start is called before the first frame update
    void Start()
    {
        // Q-table 초기화(float 최솟값)
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < stateNum; j++)
            {
                for(int k = 0; k < actionNum; k++)
                {
                    Q_table[i, j, k] = 
                }
            }
        }

        //Q_table[0, 4, 1] = -0.5f;
    }

    // Update is called once per frame
    void Update()
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
            if (carList[i].GetComponent<Car>().dest_RSU == 1)
            {

            }
            else
            {
                dest_RSU = carList[i].GetComponent<Car>().dest_RSU;
                demandLevel = carList[i].GetComponent<Car>().demandLevel;
                safetyLevel = carList[i].GetComponent<Car>().safetyLevel;
                prev_RSU = carList[i].GetComponent<Car>().prev_RSU;
                carList[i].GetComponent<Car>().direction = getNextDirection(getNextAction());
            }
        }
    }

    private int getNextAction()
    {
        // 해당 state의 2차원 배열(Q_table)에서의 index를 구함
        int stateIndex = 0;
        for(int i = 0; i < stateNum; i++)
        {
            if (state_RSU[i] == dest_RSU)
            {
                stateIndex = i;
                break;
            }
        }

        // maxQ 값 저장, 가장 작은 float 값으로 초기화
        float maxQ = float.MinValue;

        // 해당 action의 index 값 저장
        int actionIndex = 0;

        for (int i = 0; i < actionNum; i++)
        {
            // Safety Level을 만족하지 못하는 action 선택X
            if (actions_SL[i] < safetyLevel || actions_RSU[i] == prev_RSU)
            {
                continue;
            }

            if (maxQ < Q_table[demandLevel, stateIndex, i])
            {
                maxQ = Q_table[demandLevel, stateIndex, i];
                actionIndex = i;
            }
        }

        return actions_RSU[actionIndex];
    }

    private string getNextDirection(int RSU_num)
    {
        switch (RSU_num)
        {
            case 2:
                return "left";
            case 6:
                return "right";
            default:
                return "straight";
        }
    }
}
