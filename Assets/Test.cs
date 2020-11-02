using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonUp(0))
        {
            // Ray 객체 생성
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // rayCasting 실행
            rayCasting(ray);
        }
    }

    void rayCasting(Ray ray)
    {
        RaycastHit hitObj;
        if (Physics.Raycast(ray, out hitObj, Mathf.Infinity))
        {
            Debug.Log(hitObj.transform.name);
        }
    }
}
