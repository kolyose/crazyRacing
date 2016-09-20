using UnityEngine;
using System.Collections;

public class PixelPerfectCamera : MonoBehaviour {

    public Camera camera;
    public float unitSize;
    public float smoothingSpeed = 2;

    [HideInInspector]
    public MainModel mainModel;
    
    public float PixelsPerUnit { get; private set; } 
    public float Scale { get; private set; }

    private Coroutine _cameraSizeCoroutine;
    private Coroutine _cameraPositionCoroutine;

    void Awake() 
    {
        if (mainModel == null) mainModel = GetComponent<MainModel>();
    }

    public void UpdateSettings()
    {
        PixelsPerUnit = 1.0f;
        Scale = PixelsPerUnit;
    }

    public void ZoomToFieldLength(bool forced=true)
    {
        if (!camera.orthographic) return;
        StopAllCoroutines();

        float targetWidth = mainModel.GameSettings.fieldLength * unitSize;
        float targetHeight = Mathf.RoundToInt(targetWidth / (float)Screen.width * Screen.height);
        float targetSize = targetHeight / PixelsPerUnit / 2;
        Vector3 targetPosition = new Vector3(targetWidth / 2.0f / PixelsPerUnit, (float)(mainModel.GameSettings.fieldWidth * unitSize) / 2.0f / PixelsPerUnit, -10.0f);

        if (forced)
        {
            camera.orthographicSize = targetSize;
            camera.transform.position = targetPosition;
        }
        else
        { 
            StartCoroutine(UpdateCameraSizeSmoothly(targetSize));
            StartCoroutine(UpdateCameraPositionSmoothly(targetPosition));
        }
    }

    public void FollowUserCharacter(Vector3 characterPosition, bool forced=true)
    {
        if (!camera.orthographic) return;
        StopAllCoroutines();

        float targetHeight = mainModel.GameSettings.fieldWidth * unitSize;
        float targetSize = targetHeight / PixelsPerUnit / 2;
        float targetX = characterPosition.x;
        
        if (targetX < (float)Screen.width / PixelsPerUnit / 2)
        {
            targetX = (float)Screen.width / PixelsPerUnit / 2;
        }
        else if (targetX > mainModel.GameSettings.fieldLength * unitSize - (float)Screen.width/ PixelsPerUnit /2)
        {
            targetX = mainModel.GameSettings.fieldLength * unitSize - (float)Screen.width / PixelsPerUnit / 2;
        }
               
        Vector3 targetPosition = new Vector3(targetX, (float)(mainModel.GameSettings.fieldWidth * unitSize) / 2.0f / PixelsPerUnit, -10);

        if (forced)
        {
            camera.orthographicSize = targetSize;
            camera.transform.position = targetPosition;            
        }
        else
        {            
            StartCoroutine(UpdateCameraSizeSmoothly(targetSize));
            StartCoroutine(UpdateCameraPositionSmoothly(targetPosition));            
        }
    }

    private IEnumerator UpdateCameraSizeSmoothly(float newSize)
    {
        Vector3 oldPosition = camera.transform.position;
        float remainingSize = Mathf.Round(Mathf.Abs(camera.orthographicSize - newSize));
        if (remainingSize == 0) yield break;                

        while (remainingSize > 0)
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, newSize, Time.deltaTime * smoothingSpeed);
            remainingSize = Mathf.Round(Mathf.Abs(camera.orthographicSize - newSize));
            camera.transform.position = oldPosition;
            yield return null;
        }
    }

    private IEnumerator UpdateCameraPositionSmoothly(Vector3 newPosition)
    {
        float remainingDistance = Mathf.Round(Mathf.Abs(camera.transform.position.x - newPosition.x));
        if (remainingDistance == 0) yield break;

        while (remainingDistance > 0)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, newPosition, Time.deltaTime * smoothingSpeed);
            remainingDistance = Mathf.Round(Mathf.Abs(camera.transform.position.x - newPosition.x));
            yield return null;
        }
    }
}
