using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mouse : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    private Camera _cameraMain;

    void Start()
    {
        _cameraMain = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickWorldPosition = _cameraMain.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int clicedCellPosition = _tilemap.WorldToCell(clickWorldPosition);

            Debug.Log(clicedCellPosition);
        }
    }
}
