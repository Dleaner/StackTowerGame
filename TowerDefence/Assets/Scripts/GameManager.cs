using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action OnCubeSpawned = delegate {};
    public Color CubeColor { get; private set; }
    [SerializeField] private GameObject _camera; 

    private CubeSpawner[] _cubeSpawners;
    private int _spawnerIndex;
    private float _colorMultiplier = 0.1f;
    private float _cameraMove;

    private void Start()
    {
        CubeColor = GetRandomColor();
        _cubeSpawners = FindObjectsOfType<CubeSpawner>();
        _cubeSpawners[0].SpawnCube();
        _cameraMove = _camera.transform.position.y;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawColor();

            if (MovingCube.CurrentCube != null)
                MovingCube.CurrentCube.Stop();

            _spawnerIndex = _spawnerIndex == 0 ? 1 : 0;
            _cubeSpawners[_spawnerIndex].SpawnCube();
            OnCubeSpawned();

            MoveCamera();
        }
    }

    private void MoveCamera()
    {
        _cameraMove += MovingCube.LastCube.transform.localScale.y; 
        _camera.transform.position = new Vector3(_camera.transform.position.x, _cameraMove, _camera.transform.position.z);
    }

    private void DrawColor()
    {
        _colorMultiplier += 0.1f;
        CubeColor = new Color(CubeColor.r, CubeColor.g, _colorMultiplier);
        if (CubeColor.b >= 1f)
        {
            CubeColor = GetRandomColor();
            _colorMultiplier = 0.1f;
        }
    }

    private Color GetRandomColor()
    {
        return new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), 0.1f);
    }
}
