using UnityEngine;
using UnityEngine.Tilemaps;

public class CoordinateDebugger : MonoBehaviour
{
    [SerializeField]
    private Tilemap _tilemap;
    [SerializeField]
    private Transform _player;
    private Camera _camera;
    void Start()
    {
        if (_tilemap == null)
            _tilemap = FindObjectOfType<Tilemap>();
        _camera = Camera.main;

        Debug.Log("==DEBUGGER STARTED==");
        LogAllPositions();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("==MOUSE CLICK==");
            Vector3 screenMouse = Input.mousePosition;
            Debug.Log($"Screen mouse: {screenMouse}");

            for (int i = 0; i < 3; i++)
            {
                float zValue = i switch
                {
                    0 => 0f,
                    1 => 10f,
                    2 => -_camera.transform.position.z,
                    _ => 0f
                };
                Vector3 mousewithZ = new Vector3(screenMouse.x, screenMouse.y, zValue);
                Vector3 worldPos = _camera.ScreenToWorldPoint(mousewithZ);

                Debug.Log($"Z = {zValue}: World pos = {worldPos}");
            };

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -_camera.transform.position.z;
            Vector3 worldPosFinal = _camera.ScreenToWorldPoint(mousePos);
            Debug.Log($"Using Z={mousePos.z}: World = {worldPosFinal}");

            if (_tilemap != null)
            {
                Vector3Int cellPos = _tilemap.WorldToCell(worldPosFinal);
                Vector3 cellCenter = _tilemap.GetCellCenterWorld(cellPos);
                Debug.Log($"Cell position: {cellPos}");
                Debug.Log($"Cell center: {cellCenter}");
                Debug.Log($"Has tile: {_tilemap.HasTile(cellPos)}");
                Debug.Log($"Diff from (0,26,0): {cellCenter - new Vector3(0, 26, 0)}");

                LogAllPositions();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
            {
                LogAllPositions();
            }
    }
    void LogAllPositions()
    {
        Debug.Log($"==CURRENT POSITIONS==");
        Debug.Log($"Player: {_player.position}");
        Debug.Log($"Tilemap: {_tilemap.transform.position}");
        Debug.Log($"Tilemap local: {_tilemap.transform.localPosition}");
        Debug.Log($"Camera: {_camera.transform.position}");
        Debug.Log($"Camera near clip: {_camera.nearClipPlane}");
        Debug.Log($"Camera far clip: {_camera.farClipPlane}");

        BoundsInt bounds = _tilemap.cellBounds;
        Debug.Log($"Tilemap bounds: {bounds.min} to {bounds.max}");
        Debug.Log($"Tilemap cell size: {_tilemap.cellSize}");

        Vector3Int testCell = _tilemap.WorldToCell(new Vector3(0,26,0));
        Debug.Log($"Position (0,26,0) is in cell: {testCell}");
        Debug.Log($"Cell center at (0,26,0): {_tilemap.GetCellCenterWorld(testCell)}");
    }
    void OnDrawGizmos()
    {
        if (_tilemap == null || _player == null) return;
        Gizmos.color = Color.blue;
        BoundsInt bounds = _tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (_tilemap.HasTile(cell))
                {
                    Vector3 center = _tilemap.GetCellCenterWorld(cell);
                    Gizmos.DrawWireCube(center, _tilemap.cellSize * 0.9f);
                }
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(0, 26, 0), .5f);
        Gizmos.DrawLine(_player.position, new Vector3(0, 26, 0));

        if (Application.isPlaying )
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -_camera.transform.position.z;
            Vector3 worldPos = _camera.ScreenToWorldPoint(mousePos);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(worldPos, .3f);
            Gizmos.DrawLine(_camera.transform.position, worldPos);

        }
    }
}
