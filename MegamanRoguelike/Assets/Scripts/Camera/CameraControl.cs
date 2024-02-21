using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Transform player;
    private Camera cam;
    private BoxCollider2D cameraBox;
    private BoxCollider2D boundary;
    private string aspectRatioSelected;
    private Vector3 defaultAspectRatio;

    private float defaultCameraSizeX, defaultCameraSizeY;

    [Header("Follow Settings")]
    public Vector3 cameraOffSet; //0 0 18
    public float cameraLayerPerDistance; //-10
    public float smoothCoefficient; //0.3
    private Vector3 followVector;

    void Awake()
    {
        player = GameObject.Find("Player").transform;
        cam = Camera.main;
        cameraBox = GetComponent<BoxCollider2D>();
        DetectAspectRatio();
    }

    void Start()
    {
        defaultCameraSizeX = cameraBox.bounds.max.x - cameraBox.bounds.min.x;
        defaultCameraSizeY = cameraBox.bounds.max.y - cameraBox.bounds.min.y;
    }

    void Update()
    {
    }
    private void FixedUpdate()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        cam.orthographicSize = Mathf.Abs(cameraOffSet.z);

        if (GameObject.Find("Boundary"))
        {
            boundary = GameObject.Find("Boundary").GetComponent<BoxCollider2D>();

            followVector = new Vector3(Mathf.Clamp(player.position.x + cameraOffSet.x, boundary.bounds.min.x + cameraBox.size.x / 2, boundary.bounds.max.x - cameraBox.size.x / 2),
                                       Mathf.Clamp(player.position.y + cameraOffSet.y, boundary.bounds.min.y + cameraBox.size.y / 2, boundary.bounds.max.y - cameraBox.size.y / 2), 
                                       cameraLayerPerDistance);

            transform.position = Vector3.Lerp(transform.position, followVector, smoothCoefficient);

            AutoResizeCamBox();
        }
    }
    void DetectAspectRatio()
    {
        //16:10 ratio
        if (cam.aspect >= (1.6f) && cam.aspect < 1.7f)
        {
            aspectRatioSelected = "16:10";
            defaultAspectRatio = new Vector2(((cameraOffSet.z * 23) / 7.1f), (cameraOffSet.z * 14.3f) / 7.1f);
            cameraBox.size = new Vector2(((cameraOffSet.z * 23)/7.1f), (cameraOffSet.z * 14.3f)/7.1f);
        }
        //16:9 ratio
        if (cam.aspect >= (1.7f) && cam.aspect < 1.8f)
        {
            aspectRatioSelected = "16:9";
            defaultAspectRatio = new Vector2(((cameraOffSet.z * 25.47f) / 7.1f), (cameraOffSet.z * 14.3f) / 7.1f);
            cameraBox.size = new Vector2(((cameraOffSet.z * 25.47f)/7.1f), (cameraOffSet.z * 14.3f)/7.1f);
        }
        //5:4 ratio
        if (cam.aspect >= (1.25f) && cam.aspect < 1.3f)
        {
            aspectRatioSelected = "5:4";
            defaultAspectRatio = new Vector2(((cameraOffSet.z * 18) / 7.1f), (cameraOffSet.z * 14.3f) / 7.1f);
            cameraBox.size = new Vector2(((cameraOffSet.z * 18) / 7.1f), (cameraOffSet.z * 14.3f) / 7.1f);
        }
        //4:3 ratio
        if (cam.aspect >= (1.3f) && cam.aspect < 1.4f)
        {
            aspectRatioSelected = "4:3";
            defaultAspectRatio = new Vector2(((cameraOffSet.z * 19.13f) / 7.1f), (cameraOffSet.z * 14.3f) / 7.1f);
            cameraBox.size = new Vector2(((cameraOffSet.z * 19.13f) / 7.1f), (cameraOffSet.z * 14.3f) / 7.1f);
        }
        //3:2 ratio
        if (cam.aspect >= (1.5f) && cam.aspect < 1.6f)
        {
            aspectRatioSelected = "3:2";
            defaultAspectRatio = new Vector2(((cameraOffSet.z * 21.6f) / 7.1f), (cameraOffSet.z * 14.3f) / 7.1f);
            cameraBox.size = new Vector2(((cameraOffSet.z * 21.6f) / 7.1f), (cameraOffSet.z * 14.3f) / 7.1f);
        }
    }

    void AutoResizeCamBox()
    {
        if (defaultCameraSizeX > (boundary.bounds.max.x - boundary.bounds.min.x)/* || defaultCameraSizeY > (boundary.bounds.max.y - boundary.bounds.min.y)*/)
        {
            float newX = 1;
            //float newY = 1;

            if (defaultCameraSizeX > (boundary.bounds.max.x - boundary.bounds.min.x))
            {
                newX = ((defaultCameraSizeX - (boundary.bounds.max.x - boundary.bounds.min.x)) / defaultCameraSizeX);
                cam.rect = new Rect(newX / 2, cam.rect.y, (1 - newX), cam.rect.height);
            }
            else
            {
                newX = 1;
            }
            //if(defaultCameraSizeY > (boundary.bounds.max.y - boundary.bounds.min.y))
            //{
            //    newY = ((defaultCameraSizeY - (boundary.bounds.max.y - boundary.bounds.min.y)) / defaultCameraSizeY);
            //    cam.rect = new Rect(cam.rect.x, newY / 2, cam.rect.width, (1 - newY));
            //}
            //else
            //{
            //    newY = 1;
            //}

            ResizeCamRect((-cam.rect.x * 2) + 1, 1);
        }
        else
        {
            cam.rect = new Rect(0, 0, 1, 1);
            ResizeCamRect(1, 1);
        }
    }

    void ResizeCamRect(float multiplierX, float multiplierY)
    {
        //16:10 ratio
        if (aspectRatioSelected == "16:10")
        {
            cameraBox.size = new Vector2(((cameraOffSet.z * 23) / 7.1f) * multiplierX, (cameraOffSet.z * 14.3f) / 7.1f) * multiplierY;
        }
        //16:9 ratio
        if (aspectRatioSelected == "16:9")
        {
            cameraBox.size = new Vector2(((cameraOffSet.z * 25.47f) / 7.1f) * multiplierX, (cameraOffSet.z * 14.3f) / 7.1f) * multiplierY;
        }
        //5:4 ratio
        if (aspectRatioSelected == "5:4")
        {
            cameraBox.size = new Vector2(((cameraOffSet.z * 18) / 7.1f) * multiplierX, (cameraOffSet.z * 14.3f) / 7.1f) * multiplierY;
        }
        //4:3 ratio
        if (aspectRatioSelected == "4:3")
        {
            cameraBox.size = new Vector2(((cameraOffSet.z * 19.13f) / 7.1f) * multiplierX, (cameraOffSet.z * 14.3f) / 7.1f) * multiplierY;
        }
        //3:2 ratio
        if (aspectRatioSelected == "3:2")
        {
            cameraBox.size = new Vector2(((cameraOffSet.z * 21.6f) / 7.1f) * multiplierX, (cameraOffSet.z * 14.3f) / 7.1f) * multiplierY;
        }
    }

}
