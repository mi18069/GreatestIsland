using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraManipulation : MonoBehaviour
{
    public void AdjustCameraToTilemap(Camera camera, Tilemap tilemap)
    {

        BoundsInt bounds = tilemap.cellBounds;
        Vector3 tilemapSize = new Vector3(bounds.size.x, bounds.size.y, bounds.size.z);

        float cameraDimensions;

        // Bigger size as a reference for shrinking map
        if (tilemapSize.x < tilemapSize.y)
            cameraDimensions = tilemapSize.y / 2f;
        else
            cameraDimensions = tilemapSize.x / 2f;

        // In order to have distance between map and screen
        cameraDimensions *= 1.2f;

        if (cameraDimensions == 0f)
        {
            Debug.Log("Camera dimensions is 0");
            return;
        }

        camera.orthographicSize = cameraDimensions;

        CenterCameraToTilemap(camera, tilemap);
        RotateCameraRandom(camera);
    }

    private void CenterCameraToTilemap(Camera camera, Tilemap tilemap)
    {
        Vector3 tilemapCenter = tilemap.transform.position + tilemap.cellBounds.center;
        camera.transform.position = new Vector3(tilemapCenter.x, tilemapCenter.y, -10f);
    }

    // This method create feeling of rotated map
    // Rotate camera for x * 90 degrees
    private void RotateCameraRandom(Camera camera)
    {
        int rotationSteps = Random.Range(0, 4);
        int zAxisRotateAngle = rotationSteps * 90;
        camera.transform.rotation = Quaternion.Euler(0, 0, zAxisRotateAngle);

    }

    public IEnumerator Shake (float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
