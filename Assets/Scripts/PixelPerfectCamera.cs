using UnityEngine;
using System;

public class PixelPerfectCamera : MonoBehaviour {

    public Camera camera;
    public float unitSize;

    [HideInInspector]
    public MainModel mainModel;
    
    public float PixelsPerUnit { get; private set; } 
    public float Scale { get; private set; }

	void Awake() 
    {
        if (mainModel == null) mainModel = GetComponent<MainModel>();
    }

    public void UpdateSettings()
    {
        PixelsPerUnit = 1.0f;
        Scale = PixelsPerUnit;

        if (camera.orthographic)
        {
            float targetWidth = mainModel.GameSettings.fieldLength * unitSize;
            int height = Mathf.RoundToInt(targetWidth / (float)Screen.width * Screen.height);
            camera.orthographicSize = height / PixelsPerUnit / 2;
            camera.transform.position = new Vector3(targetWidth / 2.0f / PixelsPerUnit, (float)(mainModel.GameSettings.fieldWidth * unitSize) / 2.0f / PixelsPerUnit, -10.0f);
        }
    }
}
