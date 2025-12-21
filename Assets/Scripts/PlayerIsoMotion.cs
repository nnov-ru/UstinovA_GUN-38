using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerIsoMotion : MonoBehaviour
{
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Tilemap _map;
    private Camera _camera;
    //private Plane _groundPlane = new Plane(Vector3.forward, 0);

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

            float[] testZValues = {
                Mathf.Abs(_camera.transform.position.z),
                10f, 20f, 30f, 50f
                };
            foreach (float z in testZValues)
            {
                mousePos.z = z;
                Vector3 worldPos = _camera.ScreenToWorldPoint(mousePos);

                //Debug.Log($"Test Z={z}: World pos = {worldPos}");

                if (FindAndMoveToTile(worldPos)) return;
            }
        }
    }
    private bool FindAndMoveToTile(Vector3 worldPos)
    {
        Vector3Int cellPos = _map.WorldToCell(worldPos);
        //Debug.Log($"Testing cell: {cellPos}");
        for (int z = -36; z <= 0; z++)
        {
            Vector3Int checkCell = new Vector3Int(cellPos.x, cellPos.y, z);
            if (_map.HasTile(checkCell))
            {
                Vector3 cellCenter = _map.GetCellCenterWorld(checkCell);
                _player.position = new Vector3(cellCenter.x, cellCenter.y, _player.position.z);
                //Debug.Log($"Found Tile at Z={z}, moved player to {cellCenter.x}");
                return true;
            }
        }
        return false;
    }
}