using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PixelPerfectCamera : MonoBehaviour {

    public Camera camera;
    public float unitSize;
    public float smoothingSpeed = 5f;

    [HideInInspector]
    public MainModel mainModel;
    
    public float PixelsPerUnit { get; private set; } 
    public float Scale { get; private set; }

    public float orthographicSize { get 
        {
            return camera.orthographicSize;
        }
    }

    private Text DEBUG;

    void Awake() 
    {
        if (mainModel == null) mainModel = GetComponent<MainModel>();
        //DEBUG = GameObject.Find("DEBUG").GetComponent<Text>();
    }

    public void UpdateSettings()
    {
        PixelsPerUnit = 1.0f;
        Scale = PixelsPerUnit;
    }

    public void ZoomToFieldLength(bool forced=true)
    {
        float targetWidth = mainModel.GameSettings.fieldLength * unitSize;
        float targetHeight = Mathf.RoundToInt(targetWidth / (float)Screen.width * Screen.height);
        float targetSize = targetHeight / PixelsPerUnit / 2;
        Vector3 targetPosition = new Vector3(targetWidth / 2.0f / PixelsPerUnit, (float)(mainModel.GameSettings.fieldWidth * unitSize) / PixelsPerUnit, camera.transform.position.z);

        AdjustCamera(targetSize, targetPosition, forced);
    }

    public void ZoomToFieldWidth()
    {
       FollowUserCharacter(Vector3.zero, false);
    }

    public void FollowUserCharacter(Vector3 position, bool forced=true)
    {
        float targetHeight = (mainModel.GameSettings.fieldWidth + 2) * unitSize;
        float targetSize = targetHeight / PixelsPerUnit / 2;
        float targetX = position.x;               
        Vector3 targetPosition = new Vector3(targetX, (float)((mainModel.GameSettings.fieldWidth + 2) * unitSize) / 2.0f / PixelsPerUnit, camera.transform.position.z);

        AdjustCamera(targetSize, targetPosition, forced);
    }

    private void AdjustCamera(float targetSize, Vector3 targetPosition, bool forced=true)
    {
        if (!camera.orthographic) return;
        StopAllCoroutines();

        if (forced)
        {
            camera.orthographicSize = targetSize;
            camera.transform.position = ClampCameraPosition(targetPosition);
        }
        else
        {
            StartCoroutine(ZoomCameraSmoothly(targetSize));
            StartCoroutine(MoveCameraSmoothly(targetPosition));
        }
    }

    private Vector3 ClampCameraPosition(Vector3 position)
    {
        float screenRatio = (float)Screen.width / Screen.height;
        float cameraViewWidthHalf = screenRatio * camera.orthographicSize;

        if (position.x < cameraViewWidthHalf)
        {
            position.x = cameraViewWidthHalf;
        }
        else if (position.x > mainModel.GameSettings.fieldLength * unitSize - cameraViewWidthHalf)
        {
            position.x = mainModel.GameSettings.fieldLength * unitSize - cameraViewWidthHalf;
        }

        return position;
    }

    private IEnumerator ZoomCameraSmoothly(float newSize)
    {
        float currentSize = camera.orthographicSize;
        float totalRange = Mathf.Round(camera.orthographicSize - newSize);

        if (totalRange == 0) yield break;

        for (float step = totalRange / smoothingSpeed, remainingRange = step; Mathf.Abs(remainingRange) != totalRange; remainingRange += step)
        {
            float percentage = remainingRange / totalRange;
            camera.orthographicSize = Mathf.Lerp(currentSize, newSize, Mathf.Abs(percentage));       
            yield return null;
        }
    }

    private IEnumerator MoveCameraSmoothly(Vector3 newPosition)
    {
        Vector3 currentPosition = camera.transform.position;
        float totalDistance = Vector3.Distance(camera.transform.position, newPosition);

        if (totalDistance == 0) yield break;

        for (float step = totalDistance / smoothingSpeed, passedDistance = step; passedDistance <= totalDistance; passedDistance += step)
        {
            float percentage = passedDistance / totalDistance;
            Vector3 lerpedPosition = Vector3.Lerp(camera.transform.position, newPosition, percentage);
            Vector3 clampedPosition = ClampCameraPosition(lerpedPosition);
            camera.transform.position = clampedPosition;
            yield return null;
        }

        camera.transform.position = ClampCameraPosition(newPosition);
    }
}
