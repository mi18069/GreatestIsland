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
    }

    private void CenterCameraToTilemap(Camera camera, Tilemap tilemap)
    {
        Vector3 tilemapCenter = tilemap.transform.position + tilemap.cellBounds.center;
        camera.transform.position = new Vector3(tilemapCenter.x, tilemapCenter.y, -10f);
        camera.transform.rotation = Quaternion.Euler(0, 0, 90);
    }
}
