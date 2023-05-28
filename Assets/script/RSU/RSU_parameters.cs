using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RSU_parameters : MonoBehaviour
{
    public static float RSU_effectRange = 30f;     // RSU 영향 범위
    public static float epsilon = 0.0f;       // ϵ-greddy의 epsilon 값

    //private static string txtFileName = "test.txt";
    //private static string txtFilePath = Application.dataPath + "/" + txtFileName;

    //public static void writeMaxQValue(int startRSU, int destRSU, int demandLevel)
    //{
    //    if (File.Exists(txtFilePath) == false)
    //    {
    //        File.Create(txtFilePath);
    //    }

    //    GameObject sourceRSU = GameObject.Find("RSU" + startRSU);
    //    float maxQValueOfSource;
    //    int destCount;

    //    // maxQValueOfSource를 각각의 RSU 스크립트에서 가져옴
    //    switch (startRSU)
    //    {
    //        case 1:
    //            destCount = sourceRSU.GetComponent<RSU1>().dest_count;      // maxQvalueOfSource() 함수에서 dest_count 값을 1 증가시키므로 먼저 호출해야 함
    //            maxQValueOfSource = sourceRSU.GetComponent<RSU1>().maxQvalueOfSource(destRSU, demandLevel);
    //            break;
    //        default:        // 오류 처리
    //            destCount = int.MinValue;
    //            maxQValueOfSource = float.MinValue;
    //            break;
    //    }

    //    File.AppendAllText(txtFilePath, "\n" + destCount + " " + maxQValueOfSource);   
    //}
}
