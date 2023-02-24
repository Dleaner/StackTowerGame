using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingCube : MonoBehaviour
{
    public static MovingCube CurrentCube { get; private set; }
    public static MovingCube LastCube { get; private set; }
    public CubeSpawner.IDirections MoveDirection { get; set; }

    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _maxBounds;
    private bool _initialDirection = true;

    private void OnEnable() 
    {
        if (LastCube == null)
        {
            LastCube = GameObject.Find("Ground").GetComponent<MovingCube>();
        }

        CurrentCube = this;
        GetComponent<Renderer>().material.color = GameObject.FindObjectOfType<GameManager>().CubeColor;

        transform.localScale = new Vector3(LastCube.transform.localScale.x, transform.localScale.y, LastCube.transform.lossyScale.z);
    }

    private void FixedUpdate()
    {
        if (_initialDirection)
        {
            if (MoveDirection == CubeSpawner.IDirections.Z)
                transform.position += transform.forward * Time.deltaTime * _moveSpeed;
            else
                transform.position += transform.right * Time.deltaTime * _moveSpeed;
        }
        else
        {
            if (MoveDirection == CubeSpawner.IDirections.Z)
                transform.position -= transform.forward * Time.deltaTime * _moveSpeed;
            else
                transform.position -= transform.right * Time.deltaTime * _moveSpeed;
        }
        
        if (
            !((CurrentCube.transform.position.x < _maxBounds && CurrentCube.transform.position.x > -_maxBounds) && 
            (CurrentCube.transform.position.z < _maxBounds && CurrentCube.transform.position.z > -_maxBounds))
            )
        {
            _initialDirection = !_initialDirection;
        }

    }

    private void GameOverCheck(float hangover)
    {
        float max = MoveDirection == CubeSpawner.IDirections.Z ? LastCube.transform.localScale.z : LastCube.transform.localScale.x;
        if (Math.Abs(hangover) >= max)
        {
            LastCube = null;
            CurrentCube = null;
            SceneManager.LoadScene(0);
        }
    }

    internal void Stop()
    {
        _moveSpeed = 0;
        float hangover = GetHangover();

        GameOverCheck(hangover);

        float direction = hangover > 0 ? 1f : -1f;

        if (MoveDirection == CubeSpawner.IDirections.Z)
            SplitCubeOnZ(hangover, direction);
        if (MoveDirection == CubeSpawner.IDirections.X)
            SplitCubeOnX(hangover, direction);
        LastCube = this;
    }

    private float GetHangover()
    {
        if (MoveDirection == CubeSpawner.IDirections.Z)
            return transform.position.z - LastCube.transform.position.z;
        else
            return transform.position.x - LastCube.transform.position.x;
    }

    private void SplitCubeOnX(float hangover, float direction)
    {
        float newXSize = LastCube.transform.localScale.x - Math.Abs(hangover);
        float fallingCubeSize = transform.localScale.x - newXSize;
        float newXPosition = LastCube.transform.position.x + (hangover / 2);

        transform.localScale = new Vector3(newXSize, transform.localScale.y, transform.localScale.z);
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);

        float cubeEdge = transform.position.x + (newXSize / 2f * direction);
        float fallingCubeXPosition = cubeEdge + fallingCubeSize / 2f * direction;

        SpawnDropCube(fallingCubeXPosition, fallingCubeSize);
    }

    private void SplitCubeOnZ(float hangover, float direction)
    {
        float newZSize = LastCube.transform.localScale.z - Math.Abs(hangover);
        float fallingCubeSize = transform.localScale.z - newZSize;
        float newZPosition = LastCube.transform.position.z + (hangover / 2);

        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZSize);
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);

        float cubeEdge = transform.position.z + (newZSize / 2f * direction);
        float fallingCubeZPosition = cubeEdge + fallingCubeSize / 2f * direction;

        SpawnDropCube(fallingCubeZPosition, fallingCubeSize);
    }

    private void SpawnDropCube(float fallingBlockPosition, float fallingCubeSize)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        if (MoveDirection == CubeSpawner.IDirections.Z)
        {
            cube.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, fallingCubeSize);
            cube.transform.position = new Vector3(transform.position.x, transform.      position.y, fallingBlockPosition);
        }
        else
        {
            cube.transform.localScale = new Vector3(fallingCubeSize, transform.localScale.y, transform.localScale.z);
            cube.transform.position = new Vector3(fallingBlockPosition, transform.      position.y, transform.position.z);
        }

        cube.AddComponent<Rigidbody>();
        cube.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;
        
        Destroy(cube, 1f);
    }
}
