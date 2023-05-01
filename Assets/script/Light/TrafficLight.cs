using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public int signalTurn;      // 자신의 신호가 켜지는 순서
    private float startLightOnDelay;      // 시작 후 신호 대기 시간
    public float lightOnTime;       // 신호 유지 시간
    public int roadNum;     // 교차로에 연결된 도로 갯수
    private float nextLightDelay;        // 다음 번 신호 대기 시간
    public bool isLightOn;      // 신호가 켜져 있는지 여부 저장
    public int lightOn_lineNum = 0;
    public int carMoveSpeed = 10;       // 신호를 받은 차량의 이동 속도(Car.cs or DummyCar.cs의 init_speed)
    public int lineNum;
    public float blueLightTerm;

    // Start is called before the first frame update
    void Start()
    {
        lightOnTime = 20;
        blueLightTerm = 20 / 4;

        startLightOnDelay = (signalTurn - 1) * lightOnTime;
        nextLightDelay = (roadNum - 1) * lightOnTime;

        // 신호가 꺼진 상태로 시작
        isLightOn = false;

        // 신호 반복 시작
        StartCoroutine("makeSignal");
    }

    IEnumerator makeSignal()
    {
        // lightOnDelay만큼 대기 후 시작
        yield return new WaitForSeconds(startLightOnDelay);

        while (true)
        {
            // 신호 켬
            isLightOn = true;

            // 2차선 : 신호가 켜지면 1번과 2번 lineNum 번갈아 2번 켜지도록 설정
            // 4차선 : 1번부터 4번까지 돌아가면서 켜지도록 설정
            if (lineNum == 2)
            {
                lightOn_lineNum = 1;
                yield return new WaitForSeconds(lightOnTime / 4);

                lightOn_lineNum = 2;
                yield return new WaitForSeconds(lightOnTime / 4);

                lightOn_lineNum = 1;
                yield return new WaitForSeconds(lightOnTime / 4);

                lightOn_lineNum = 2;
                yield return new WaitForSeconds(lightOnTime / 4);
            } 
            else if(lineNum == 4)
            {
                for (int i = 1; i <= 4; i++)
                {
                    lightOn_lineNum = i;
                    yield return new WaitForSeconds(lightOnTime / 4);
                }
            }

            // 신호 끔
            isLightOn = false;

            // nextLightDelay만큼 대기
            yield return new WaitForSeconds(nextLightDelay);
        }
    }
}
