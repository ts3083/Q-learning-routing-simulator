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
        { 2, 6, 7, 12, 0 },
        { 3, 4, 7, 9, 13 },
        { 4, 8, 10, 14, 0 },
        { 5, 9, 15, 0, 0 },
        { 6, 12, 16, 0, 0 },
        { 6, 7, 11, 13, 17 },
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
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {1, 2, 5, 10, 0 },       // RSU6
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0 }
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

    public int DecideNextRSU(int prevRSU, int curRSU)
    {
        int selectedRSU = 0;     // 선택된 RSU 번호에 해당하는 index 저장
        int randNum = -1;        // Random.Range() 함수를 통해 생성되는 난수

        // 확률로 다음에 이동할 RSU 번호에 해당하는 index 찾기
        while(selectedRSU == 0)
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

    // DummyCar에서 다음 경로 결정 함수(이전 RSU, 현재 RSU, DummyCar script의 차선 번호, DummyCar script의 direction, DummyCar script의 position)
    //public void getNextDirection(int prevRSU, int curRSU, ref int lineNum, ref string direction, ref Vector3 position)
    //{
    //    int nextRSU = DecideNextRSU(prevRSU, curRSU);       // 다음으로 이동할 RSU

    //    switch(curRSU)
    //    {
    //        case 1:
    //            RSU1 temp1 = new();
    //            temp1.prev_RSU = prevRSU;
    //            direction = temp1.getNextDirection(nextRSU);
    //            break;
    //        case 2:
    //            RSU2 temp2 = new();
    //            temp2.prev_RSU = prevRSU;
    //            direction = temp2.getNextDirection(nextRSU);
    //            break;
    //        case 3:
    //            RSU3 temp3 = new();
    //            temp3.prev_RSU = prevRSU;
    //            direction = temp3.getNextDirection(nextRSU);
    //            break;
    //        case 4:
    //            RSU4 temp4 = new();
    //            temp4.prev_RSU = prevRSU;
    //            direction = temp4.getNextDirection(nextRSU);
    //            break;
    //        case 5:
    //            RSU5 temp5 = new();
    //            temp5.prev_RSU = prevRSU;
    //            direction = temp5.getNextDirection(nextRSU);
    //            break;
    //        case 6:     // TEST
    //            RSU6 temp6 = new();
    //            temp6.prev_RSU = prevRSU;
    //            temp6.line_num = lineNum;
    //            direction = temp6.getNextDirection(nextRSU);
    //            position = temp6.getPosition(nextRSU);
    //            lineNum = temp6.line_num;
    //            break;
    //        case 7:
    //            RSU7 temp7 = new();
    //            temp7.prev_RSU = prevRSU;
    //            direction = temp7.getNextDirection(nextRSU);
    //            break;
    //        case 8:
    //            RSU8 temp8 = new();
    //            temp8.prev_RSU = prevRSU;
    //            direction = temp8.getNextDirection(nextRSU);
    //            break;
    //        case 9:
    //            RSU9 temp9 = new();
    //            temp9.prev_RSU = prevRSU;
    //            direction = temp9.getNextDirection(nextRSU);
    //            break;
    //        case 10:
    //            RSU10 temp10 = new();
    //            temp10.prev_RSU = prevRSU;
    //            direction = temp10.getNextDirection(nextRSU);
    //            break;
    //        case 11:
    //            RSU11 temp11 = new();
    //            temp11.prev_RSU = prevRSU;
    //            direction = temp11.getNextDirection(nextRSU);
    //            break;
    //        case 12:
    //            RSU12 temp12 = new();
    //            temp12.prev_RSU = prevRSU;
    //            direction = temp12.getNextDirection(nextRSU);
    //            break;
    //        case 13:
    //            RSU13 temp13 = new();
    //            temp13.prev_RSU = prevRSU;
    //            direction = temp13.getNextDirection(nextRSU);
    //            break;
    //        case 14:
    //            RSU14 temp14 = new();
    //            temp14.prev_RSU = prevRSU;
    //            direction = temp14.getNextDirection(nextRSU);
    //            break;
    //        case 15:
    //            RSU15 temp15 = new();
    //            temp15.prev_RSU = prevRSU;
    //            direction = temp15.getNextDirection(nextRSU);
    //            break;
    //        case 16:
    //            RSU16 temp16 = new();
    //            temp16.prev_RSU = prevRSU;
    //            direction = temp16.getNextDirection(nextRSU);
    //            break;
    //        case 17:
    //            RSU17 temp17 = new();
    //            temp17.prev_RSU = prevRSU;
    //            direction = temp17.getNextDirection(nextRSU);
    //            break;
    //        case 18:
    //            RSU18 temp18 = new();
    //            temp18.prev_RSU = prevRSU;
    //            direction = temp18.getNextDirection(nextRSU);
    //            break;
    //        case 19:
    //            RSU19 temp19 = new();
    //            temp19.prev_RSU = prevRSU;
    //            direction = temp19.getNextDirection(nextRSU);
    //            break;
    //        case 20:
    //            RSU20 temp20 = new();
    //            temp20.prev_RSU = prevRSU;
    //            direction = temp20.getNextDirection(nextRSU);
    //            break;
    //        case 21:
    //            RSU21 temp21 = new();
    //            temp21.prev_RSU = prevRSU;
    //            direction = temp21.getNextDirection(nextRSU);
    //            break;
    //        case 22:
    //            RSU22 temp22 = new();
    //            temp22.prev_RSU = prevRSU;
    //            direction = temp22.getNextDirection(nextRSU);
    //            break;
    //        case 23:
    //            RSU23 temp23 = new();
    //            temp23.prev_RSU = prevRSU;
    //            direction = temp23.getNextDirection(nextRSU);
    //            break;
    //        case 24:
    //            RSU24 temp24 = new();
    //            temp24.prev_RSU = prevRSU;
    //            direction = temp24.getNextDirection(nextRSU);
    //            break;
    //        case 25:
    //            RSU25 temp25 = new();
    //            temp25.prev_RSU = prevRSU;
    //            direction = temp25.getNextDirection(nextRSU);
    //            break;
    //        default:
    //            break;
    //    }
    //}
}
