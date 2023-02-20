using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSU1 : MonoBehaviour
{
    private const int stateNum = 24;     // state(destination RSU) 수 - 1 [자기 자신 제외]
    private const int actionNum = 2;        // action(neighbor RSU) 수

    public int dest_RSU;       // destination RSU, 차량이 넘겨주는 정보
    public int demandLevel;     // Demand Level, 차량이 넘겨주는 정보
    public int safetyLevel;        // Safety Level, 차량이 넘겨주는 정보

    // [state(destination RSU) 수, action(neighbor RUS) 수], Demand Level [time, energy]
    private float[,,] Q_table = new float[5, stateNum, actionNum];       // Demand Level 1, [100, 0] / Demand Level 2, [75, 25] / Demand Level 3, [50, 50] / Demand Level 4, [25, 75] / Demand Level 5, [0, 100]

    // [action(neighbor RSU) 수], 각각의 action의 safety level을 저장
    public int[] actions_SL = new int[actionNum];

    // [action(neightbor RSU) 수], {각각의 action에 대응되는 RSU 번호를 저장}
    private int[] actions_RSU = new int[actionNum] { 2, 6 };

    // [state(destination RSU) 수], {각각의 state에 대응되는 RSU 번호를 저장}
    private int[] state_RSU = new int[stateNum] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };

    // Start is called before the first frame update
    void Start()
    {
        Q_table[0, 2, 0] = 0.9f;
        Q_table[0, 2, 1] = 0.8f;

        Debug.Log(getNextAction());
    }

    // Update is called once per frame
    void Update()
    {
        
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
            if (actions_SL[i] < safetyLevel)
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
}
