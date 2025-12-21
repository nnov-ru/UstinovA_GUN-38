using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerSideMotion : MonoBehaviour
{
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Tilemap _tilemap;
    private Camera _camera;
    private Rigidbody2D _playerRb;
    private Bounds _tilemapBounds;

    private void Start()
    {
        if (_tilemap == null)
            _tilemap = GetComponent<Tilemap>();
        _camera = Camera.main;
        _playerRb = _player.GetComponent<Rigidbody2D>();
        _playerRb.bodyType = RigidbodyType2D.Dynamic;
        _playerRb.gravityScale = 3f;
        _playerRb.freezeRotation = true;

        _tilemap.CompressBounds();
        _tilemapBounds = _tilemap.localBounds;
        _tilemapBounds.center = _tilemap.transform.position;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -_camera.transform.position.z;
            Vector3 worldPos = _camera.ScreenToWorldPoint(mousePos);

            if (_tilemapBounds.Contains(worldPos))
            {
                worldPos.z = _player.position.z;
                _player.position = worldPos;
            }
            else return;
        }
    } 
}
