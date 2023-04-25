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
    public int carMoveSpeed = 10;       // 신호를 받은 차량의 이동 속도(Car.cs or DummyCar.cs의 init_speed)
    public int lineNum;

    // Start is called before the first frame update
    void Start()
    {
        startLightOnDelay = (signalTurn - 1) * lightOnTime;
        nextLightDelay = roadNum * lightOnTime;

        // 신호가 꺼진 상태로 시작
        isLightOn = false;

        // 신호 반복 시작
        StartCoroutine("makeSignal");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator makeSignal()
    {
        // lightOnDelay만큼 대기 후 시작
        yield return new WaitForSeconds(startLightOnDelay);

        while (true)
        {
            // 신호 켬
            isLightOn = true;

            // 신호 유지 시간만큼 대기
            yield return new WaitForSeconds(lightOnTime);

            // 신호 끔
            isLightOn = false;

            // nextLightDelay만큼 대기
            yield return new WaitForSeconds(nextLightDelay);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Q_car"))
        {
            if (isLightOn)
            {
                // 신호 On -> 이동
                other.GetComponent<Car>().BackTriggerSettingBySpeed(carMoveSpeed);
            }
            else
            {
                // 신호 Off -> 정지
                other.GetComponent<Car>().BackTriggerSettingBySpeed(0);
            }
        }

        if (other.CompareTag("DummyCar"))
        {
            if(isLightOn)
            {
                // 신호 On -> 이동
                other.GetComponent<DummyCar>().BackTriggerSettingBySpeed(carMoveSpeed);
            }
            else
            {
                // 신호 Off -> 정지
                other.GetComponent<DummyCar>().BackTriggerSettingBySpeed(0);
            }
        }
    }
}
