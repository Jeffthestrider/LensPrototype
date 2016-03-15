using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public GameObject target;
    public Text cameraDebug;
    public float cameraAdjustSpeed;

    float horizontalBounds = .3f;
    float verticalBounds = .4f;
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


        // Camera moves to follow player when approaching edge of screen.
        ShiftPosition(targetPositionRelativeToScreen);
        // Camera focuses of player.
        //FollowPlayer();
    }

    void ShiftPosition(Vector3 targetPositionRelativeToScreen)
    {
        // Amount to adjust camera's position
        float? xAdjust = null;
        float? yAdjust = null;
        
        // Boosts camera speed when target gets closer to or past edge of screen.  Careful adjustment here or you'll get jitters.
        var cameraCatchUpVelocity = 15f;

        // Figure out how much, if at all, the X position of the camera needs to change.
        if (targetPositionRelativeToScreen.x < horizontalBounds)
        {
            var speedIncrease = cameraCatchUpVelocity * (horizontalBounds + .1f - targetPositionRelativeToScreen.x);
            xAdjust = transform.position.x - (cameraAdjustSpeed * Time.deltaTime * speedIncrease);
        }
        else if (targetPositionRelativeToScreen.x > 1 - horizontalBounds)
        {
            var speedIncrease = cameraCatchUpVelocity * (targetPositionRelativeToScreen.x - (1 - horizontalBounds - .1f));
            xAdjust = transform.position.x + (cameraAdjustSpeed * Time.deltaTime * speedIncrease);
        }

        // Figure out how much, if at all, the Y position of the camera needs to change.
        if (targetPositionRelativeToScreen.y < verticalBounds)
        {
            var speedIncrease = cameraCatchUpVelocity * (verticalBounds + .1f - targetPositionRelativeToScreen.y);
            yAdjust = transform.position.y - (cameraAdjustSpeed * Time.deltaTime * speedIncrease);
        }
        else if (targetPositionRelativeToScreen.y > 1 - verticalBounds)
        {
            var speedIncrease = cameraCatchUpVelocity * (targetPositionRelativeToScreen.y - (1 - verticalBounds - .1f));
            yAdjust = transform.position.y + (cameraAdjustSpeed * Time.deltaTime * speedIncrease);
        }

        // Adjust the camera's position.
        transform.position = new Vector3(
            xAdjust ?? transform.position.x,
            yAdjust ?? transform.position.y,
            transform.position.z); ;
    }

    void FollowPlayer()
    {
        Vector3 playerpos = targetTransform.position;
        playerpos.z = transform.position.z;
        transform.position = playerpos;
    }
}
