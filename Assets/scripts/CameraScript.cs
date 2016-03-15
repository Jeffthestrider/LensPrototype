using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public GameObject target;
    public Text cameraDebug;
    public float cameraAdjustSpeed;

    private float horizontalBounds = .3f;
    private float verticalBounds = .4f;
    Transform targetTransform;
    Camera camera;

	// Use this for initialization
	void Start () {
        targetTransform = target.GetComponent<Transform>();
        camera = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        var targetPositionRelativeToScreen = camera.WorldToViewportPoint(target.transform.position);
        cameraDebug.text = "Target position relative to screen" +
            "\nX: " + targetPositionRelativeToScreen.x * 100 + "%" +
            "\nY: " + targetPositionRelativeToScreen.y * 100 + "%";


        ShiftPosition(targetPositionRelativeToScreen);
        //FollowPlayer();
    }

    void ShiftPosition(Vector3 targetPositionRelativeToScreen)
    {
        float? xAdjust = null;
        float? yAdjust = null;

        var catchUpSpeed = 20;
        var idealX = 0f;
        var idealY = 0f;

        if (targetPositionRelativeToScreen.x < horizontalBounds)
        {
            var speedIncrease = catchUpSpeed * (horizontalBounds + .1f - targetPositionRelativeToScreen.x);
            xAdjust = transform.position.x - (cameraAdjustSpeed * Time.deltaTime * speedIncrease);

            cameraDebug.text += "\nxAdj " + xAdjust + "\nidealX: " + GetIdealPositionForX(false);

            if (xAdjust < GetIdealPositionForX(false))
            {
                xAdjust = GetIdealPositionForX(false);
            }
        } else if (targetPositionRelativeToScreen.x > 1 - horizontalBounds)
        {
            var speedIncrease = catchUpSpeed * (targetPositionRelativeToScreen.x - (1 - horizontalBounds));
            xAdjust = transform.position.x + (cameraAdjustSpeed * Time.deltaTime * speedIncrease);
        }

        if (targetPositionRelativeToScreen.y < verticalBounds)
        {
            var speedIncrease = catchUpSpeed * (verticalBounds + .1f - targetPositionRelativeToScreen.y);
            yAdjust = transform.position.y - (cameraAdjustSpeed * Time.deltaTime * speedIncrease);
        }
        else if (targetPositionRelativeToScreen.y > 1 - verticalBounds)
        {
            var speedIncrease = catchUpSpeed * (targetPositionRelativeToScreen.y - (1 - verticalBounds));
            yAdjust = transform.position.y + (cameraAdjustSpeed * Time.deltaTime * speedIncrease);
        }

        var proposed = new Vector3(
            xAdjust ?? transform.position.x,
            yAdjust ?? transform.position.y,
            transform.position.z);
        
        var newViewportPoint = camera.WorldToViewportPoint(target.transform.position);

        transform.position = proposed;

    }

    float GetIdealPositionForX(bool moveRight)
    {
        if (moveRight)
            return camera.ViewportToWorldPoint(new Vector3(1 - horizontalBounds, 0f, 0f)).x;
        else
            return camera.ViewportToWorldPoint(new Vector3(horizontalBounds, 0f, 0f)).x;
    }

    void FollowPlayer()
    {
        Vector3 playerpos = targetTransform.position;
        playerpos.z = transform.position.z;
        transform.position = playerpos;
    }
}
