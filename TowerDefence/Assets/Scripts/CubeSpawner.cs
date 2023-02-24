using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CubeSpawner : MonoBehaviour
{
    [SerializeField] private MovingCube _cubePrefab;
    [SerializeField] private IDirections _moveDirection;

    public void SpawnCube()
    {
        var cube = Instantiate(_cubePrefab);

        if (MovingCube.LastCube != null && MovingCube.LastCube.gameObject != GameObject.Find("Ground"))
        {
            float x = _moveDirection == IDirections.X ? transform.position.x : MovingCube.LastCube.transform.position.x;
            float z = _moveDirection == IDirections.Z ? transform.position.z : MovingCube.LastCube.transform.position.z;

            cube.transform.position = new Vector3(x,
                MovingCube.LastCube.transform.position.y + _cubePrefab.transform.localScale.y, z);
        }
        else
        {
            cube.transform.position = transform.position;
        }

        cube.MoveDirection = _moveDirection;
    } 

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, _cubePrefab.transform.localScale);
    }
}
