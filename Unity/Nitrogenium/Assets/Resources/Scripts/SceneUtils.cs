using System.Collections.Generic;
using UnityEngine;

public class SceneUtils {

    public static SceneUtils instance {
        get {
            return new SceneUtils(
                Screen.width,
                Screen.height,
                Camera.main.orthographicSize
            );
        }
    }

    float screenWidth;
    float screenHeight;
    float cameraSize;

    public SceneUtils (float screenWidth, float screenHeight, float cameraSize) {
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        this.cameraSize = cameraSize;
    }

    public float maxX {
        get {
            return screenWidth / screenHeight * cameraSize;
        }
    }

    public float minX {
        get {
            return -maxX;
        }
    }

    public float maxY {
        get {
            return cameraSize;
        }
    }

    public float minY {
        get {
            return -maxY;
        }
    }

    public bool inBound (Vector3 position) {
        if (
            position.x > maxX ||
            position.x < minX ||
            position.y > maxY ||
            position.y < minY
        ) {
            return false;
        }
        return true;
    }
}
