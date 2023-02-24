using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    private Transform _tower;

    void Start()
    {
        _tower = GetComponent<Transform>();
    }

    void Update()
    {
        
    }

    public void MoveTower()
    {
        _tower.position -= new Vector3(0, 0.2f, 0);
    }
}
