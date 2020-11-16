using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private new Camera camera;
    private bool isHit = false;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    Vector2 mousePrePos;

    // Update is called once per frame
    void Update()
    {
        Vector2 mp = Input.mousePosition;

        // Rotate for LMB
        if (Input.GetMouseButtonDown(0))
        {
            mousePrePos = mp;

            Ray ray = camera.ScreenPointToRay(mousePrePos);
            RaycastHit hit;
            isHit = Physics.Raycast(ray, out hit);
        }

        if (!isHit && Input.GetMouseButton(0))
        {
            var mouseDeltaPos = mp - mousePrePos;
            mouseDeltaPos *= (Time.deltaTime * speed);
            //var mx = transform.localEulerAngles.y - mouseDeltaPos.x;
            //var my = transform.localEulerAngles.x + mouseDeltaPos.y;

            //mx = mx > 180f ? -(360f - mx) : mx;
            //my = my > 180f ? -(360f - my) : my;

            //mx = Mathf.Clamp(mx, -180, 180);
            //my = Mathf.Clamp(my, -180, 180);

            //transform.localEulerAngles = new Vector3(my, mx, 0);

            transform.Rotate(new Vector3(-mouseDeltaPos.y, mouseDeltaPos.x, 0));
            mousePrePos = mp;
        }

        if (isHit && Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                var animator = hit.transform.parent.GetComponent<Animator>();
                if (animator.GetBool("isPlay"))
                {
                    animator.SetBool("isPlay", false);
                    animator.SetTrigger("Reverse");
                }
                else
                {
                    animator.SetBool("isPlay", true);
                    animator.SetTrigger("Play");
                }
            }
            isHit = false;
        }

        // Pinch Zoom for RMB
        if (Input.GetMouseButtonDown(1))
        {
            mousePrePos = mp;
        }

        if (Input.GetMouseButton(1))
        {
            var mouseDeltaPos = mp - mousePrePos;
            mouseDeltaPos *= (Time.deltaTime * speed);

            camera.fieldOfView += (mouseDeltaPos.x + mouseDeltaPos.y) * -Time.deltaTime * speed;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 20f, 60f);
        }
    }

//    void PlayerRotation()
//    {

//#if !UNITY_ANDROID
//        var mp = Input.mousePosition;

//        if (mp.x > Camera.main.pixelWidth / 2)
//        {
//            if (Input.GetMouseButtonDown(0))
//            {
//                mousePrePos = mp;
//            }

//            if (Input.GetMouseButton(0))
//            {
//                var mouseDeltaPos = (mp - mousePrePos) / 2;

//                var angle = transform.eulerAngles;
//                my = camera.transform.localEulerAngles.x - mouseDeltaPos.y; //고치기 
//                //Debug.Log(my);
//                my = my > 180f ? -(360f - my) : my;
//                my = Mathf.Clamp(my, -58.0f, 58.0f);
//                camera.transform.localEulerAngles = new Vector3(my, 0, 0);
//                transform.eulerAngles = new Vector3(0, angle.y + mouseDeltaPos.x, angle.z);


//                mousePrePos = mp;
//            }
//        }
//#elif UNITY_ANDROID

//        if (Input.touchCount > 1)
//        {
//            var touches = Input.touches;
//            foreach (var touch in touches)
//            {
//                if (touch.position.x > Camera.main.pixelWidth / 2 )
//                {
//                    if (touch.phase == TouchPhase.Moved)
//                    {
//                        //var mouseDeltaPos = (mp - mousePrePos) / 2;

//                        //var angle = transform.eulerAngles;
//                        //my = camera.transform.localEulerAngles.x - mouseDeltaPos.y; //고치기 
//                        //                                                            //Debug.Log(my);
//                        //my = my > 180f ? -(360f - my) : my;
//                        //my = Mathf.Clamp(my, -58.0f, 58.0f);
//                        //camera.transform.localEulerAngles = new Vector3(my, 0, 0);
//                        //transform.eulerAngles = new Vector3(0, angle.y + mouseDeltaPos.x, angle.z);


//                        //mousePrePos = mp;
//                    }
//                }
//            }
//        }
//#endif
//    }
}