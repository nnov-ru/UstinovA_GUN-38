using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTopDownMotion : MonoBehaviour
{
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Tilemap _map;
    private Camera _camera;

    private void Start()
    {
        if (_map == null)
            _map = GetComponent<Tilemap>();
        _camera = Camera.main;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = -_camera.transform.position.z;
                Vector3 worldPos = _camera.ScreenToWorldPoint(mousePos);
                Vector3Int cellPos = _map.WorldToCell(worldPos);
                Vector3 cellCenter = _map.GetCellCenterWorld(cellPos);

                if (_map.HasTile(cellPos))
                {
                    _player.position = cellCenter;
                }
        }
    }
}
