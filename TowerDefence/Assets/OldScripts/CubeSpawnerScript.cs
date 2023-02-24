using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawnerScript : MonoBehaviour
{
    public GameObject CubePrefab;
    public float MaxBounds;
    public float CubeSpeed;

    private GameObject PrefabMk2;
    
    private Vector3 _startPos;
    private bool _canSpawn;
    private bool _canDropDown;
    private GameObject _cube;
    private GameObject _previousCube;
    private GameObject _tower;
    private bool _moveDirectionRight;
    private int _direction;

    enum IDirections
    {
        Right,
        Forward
    }

    void Start()
    {
        _tower = GameObject.Find("Tower");
        _previousCube = GameObject.Find("Ground");
        _startPos = Vector3.zero;
        _canDropDown = false;
        _canSpawn = true;

        PrefabMk2 = CubePrefab;
    }

    private void FixedUpdate() 
    {
        // Spawn Cube
        if (_canSpawn)
        {
            SpawnCube();
        }

        if (_cube != null)
        {
            MoveCube();
        }
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && _canDropDown)
        {
            DropDownCube();
        }
    }

    private void SpawnCube()
    {
        _canSpawn = false;
        _canDropDown = true;
        
        _cube = Instantiate(CubePrefab, _startPos, CubePrefab.transform.rotation, _tower.transform);
    }

    private void MoveCube()
    {
        // Move
        if (_direction == (int)IDirections.Right)
        {
            if (_moveDirectionRight)
                _cube.transform.Translate(Vector3.right * Time.deltaTime * CubeSpeed);
            else
                _cube.transform.Translate(Vector3.left * Time.deltaTime * CubeSpeed);
        }
        else if (_direction == (int)IDirections.Forward)
        {
            if (_moveDirectionRight)
                _cube.transform.Translate(Vector3.forward * Time.deltaTime * CubeSpeed);
            else
                _cube.transform.Translate(Vector3.back * Time.deltaTime * CubeSpeed);
        }

        // Bounds check
        if (
            !((_cube.transform.position.x < MaxBounds && _cube.transform.position.x > -MaxBounds) && 
            (_cube.transform.position.z < MaxBounds && _cube.transform.position.z > -MaxBounds))
            )
        {
            _moveDirectionRight = !_moveDirectionRight;
        }
    }

    private Vector3 DistanceBetweenVectorsByXZ(Vector3 firstDot, Vector3 secondDot)
    {
        float xDistance = Mathf.Abs(firstDot.x - secondDot.x);
        float zDistance = Mathf.Abs(firstDot.z - secondDot.z);

        if (zDistance == 0)
            zDistance = 2;
        if (xDistance == 0)
            xDistance = 2;

        return new Vector3(xDistance, 1, zDistance);
    }

    private void DropDownCube()
    {
        _canDropDown = false;

        // Drop Cube
        Vector3 leftBorder = _cube.transform.position + _cube.transform.localScale / 2;
        Vector3 rightBorder = _previousCube.transform.position + _previousCube.transform.localScale / 2;
        Vector3 cubeBorder = _cube.transform.position - _cube.transform.localScale / 2;

        leftBorder.y = 0;
        rightBorder.y = 0;
        cubeBorder.y = 0;

        // Falling part
        Vector3 partPos = (leftBorder - rightBorder) / 2;
        if (partPos.x > 0)
            partPos.x += _previousCube.transform.localScale.x / 2;
        else if (partPos.x < 0)
            partPos.x -= _previousCube.transform.localScale.x / 2;
        else if (partPos.z > 0)
            partPos.z += _previousCube.transform.localScale.z / 2;
        else if (partPos.z < 0)
            partPos.z -= _previousCube.transform.localScale.z / 2;

        Vector3 partScale = Vector3.Scale(DistanceBetweenVectorsByXZ(leftBorder, rightBorder), new Vector3(1, 0.2f, 1));

        GameObject fallingPart = Instantiate(CubePrefab, partPos, CubePrefab.transform.rotation, _tower.transform);
        fallingPart.transform.localScale = partScale;
        fallingPart.GetComponent<Rigidbody>().isKinematic = false;

        // New cube
        Vector3 cubePos = (rightBorder + cubeBorder) / 2;
        Vector3 cubeScale = Vector3.Scale(DistanceBetweenVectorsByXZ(rightBorder, cubeBorder), new Vector3(1, 0.2f, 1));
        if (cubePos.x < 0)
            cubeScale.x -= partScale.x * 2;
        if (cubePos.z < 0)
            cubeScale.z -= partScale.z * 2;

        // _cube.transform.position = cubePos;
        // _cube.transform.localScale = cubeScale;
        GameObject newCube = Instantiate(CubePrefab, cubePos, CubePrefab.transform.rotation, _tower.transform);
        newCube.transform.localScale = cubeScale;
        
        // CubePrefab = newCube;
        // _previousCube = newCube;


        // _startPos = newCube.transform.position;
        // _startPos.y = 0;


        Destroy(_cube);
        StartCoroutine(SpawnDelayRoutine()); 

        // Max bounds refresh

        _direction = Random.Range(0, 2);
    }

    IEnumerator SpawnDelayRoutine()
    {
        _tower.GetComponent<TowerController>().MoveTower();
        yield return new WaitForSeconds(1);
        _canSpawn = true; 
    }

}
