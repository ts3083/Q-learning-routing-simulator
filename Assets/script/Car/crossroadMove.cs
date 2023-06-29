using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crossroadMove : MonoBehaviour
{
    // 해당 RSU에서 이동할 수 있는 다음 RSU(action) 리스트
    private static int[,] action_RSUList = new int[25, 5]
    {
        { 2, 6, 0, 0, 0 },
        { 1, 3, 7, 0, 0  },
        { 2, 4, 8, 0, 0 },
        { 3, 5, 8, 9, 0 },
        { 4, 10, 0, 0, 0 },
        { 1, 7, 11, 12, 0 },        // RSU6
        { 2, 6, 8, 12, 0 }, // RSU7
        { 3, 4, 7, 9, 13 },
        { 4, 8, 10, 14, 0 },
        { 5, 9, 15, 0, 0 },
        { 6, 12, 16, 0, 0 },
        { 6, 7, 11, 13, 17 }, // RSU12
        { 8, 12, 14, 18, 0 },
        { 9, 13, 15, 19, 20 },
        { 10, 14, 20, 0, 0 },
        { 11, 17, 21, 0, 0 },
        { 12, 16, 18, 22, 0 },
        { 13, 17, 19, 22, 23 },
        { 14, 18, 20, 24, 0 },
        { 14, 15, 19, 25, 0 },
        { 16, 22, 0, 0, 0 },
        { 17, 18, 21, 23, 0 },
        { 18, 22, 24, 0, 0 },
        { 19, 23, 25, 0, 0 },
        { 20, 24, 0, 0, 0 }
    };
    // 해당 RSU에서 다음 RSU(action)을 선택할 확률 리스트, 누적 확률(1 ~ 10)
    private static int[,] probabilityList = new int[25, 5]
    {
        //{5, 10, 0, 0, 0 },      // RSU1
        //{1, 2, 10, 0, 0 },      // RSU2 - 0이상 2미만, 2이상 4미만, 4이상 10미만
        //{1, 2, 10, 0, 0 },      // RSU3
        //{3, 4, 7, 10, 0 },      // RSU4
        //{5, 10, 0, 0, 0 },      // RSU5
        //{1, 8, 9, 10, 0 },      // RSU6
        //{1, 2, 9, 10, 0 },      // RSU7
        //{1, 2, 5, 6, 10 },      // RSU8
        //{1, 5, 6, 10, 0 },      // RSU9
        //{3, 6, 10, 0, 0 },      // RSU10
        //{4, 9, 10, 0, 0 },      // RSU11
        //{1, 4, 5, 9, 10 },      // RSU12
        //{4, 5, 9, 10, 0 },      // RSU13
        //{1, 4, 5, 6, 10 },      // RSU14
        //{1, 9, 10, 0, 0 },      // RSU15
        //{8, 9, 10, 0, 0 },      // RSU16
        //{4, 5, 9, 10, 0 },      // RSU17
        //{6, 7, 8, 9, 10 },      // RSU18
        //{4, 5, 9, 10, 0 },      // RSU19
        //{7, 8, 9, 10, 0 },      // RSU20
        //{5, 10, 0, 0, 0 },      // RSU21
        //{2, 5, 7, 10, 0 },      // RSU22
        //{4, 7, 10, 0, 0 },      // RSU23
        //{4, 7, 10, 0, 0 },      // RSU24
        //{5, 10, 0, 0, 0 }       // RSU25
        {5, 10, 0, 0, 0 },//1
        {1, 2, 10, 0, 0 },//2 - 0이상 2미만, 2이상 4미만, 4이상 10미만
        {1, 2, 10, 0, 0 },//3
        {1, 0, 9, 10, 0 },//4
        {5, 10, 0, 0, 0 },//5
        {1, 9, 10, 0, 0 },// RSU6
        {1, 2, 9, 10, 0 },// RSU7
        {0, 0, 5, 0, 10 },//8
        {0, 5, 0, 10, 0 },//9
        {0, 10, 0, 0, 0 },//10
        {5, 10, 0, 0, 0 },//11
        {0, 5, 0, 10, 0 },// RSU12
        {5, 0, 10, 0, 0 },//13
        {1, 9, 10, 0, 0 },//14
        {5, 10, 0, 0, 0 },//15
        {4, 8, 10, 0, 0 },//16
        {1, 5, 8, 10, 0 },//17
        {1, 6, 7, 8, 10 },//18
        {6, 7, 8, 10, 0 },//19
        {3, 6, 8, 10, 0 },//20
        {5, 10, 0, 0, 0 },//21
        {3, 6, 8, 10, 0 },//22
        {3, 6, 10, 0, 0 },//23
        {6, 8, 10, 0, 0 },//24
        {5, 10, 0, 0, 0 }//25
    };
    private static int[] RSURoadNum = new int[25] { 2, 3, 3, 4, 2, 4, 4, 5, 4, 3, 3, 5, 4, 5, 3, 3, 4, 5, 4, 4, 2, 4, 3, 3, 2 };       // 해당 RSU가 있는 교차로에 연결된 도로 갯수

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static int DecideNextRSU(int prevRSU, int curRSU)
    {
        int selectedRSU = 0;     // 선택된 RSU 번호에 해당하는 index 저장
        int randNum = -1;        // Random.Range() 함수를 통해 생성되는 난수

        // 확률로 다음에 이동할 RSU 번호에 해당하는 index 찾기
        while (selectedRSU == 0)
        {
            randNum = Random.Range(0, 10);
            for (int i = 0; i < RSURoadNum[curRSU - 1]; i++)
            {
                if (randNum < probabilityList[curRSU - 1, i])
                {
                    if(prevRSU == action_RSUList[curRSU - 1, i])
                    {
                        break;
                    }
                    else
                    {
                        selectedRSU = action_RSUList[curRSU - 1, i];
                        break;
                    }
                }
            }
        }

        return selectedRSU;
    }
}
