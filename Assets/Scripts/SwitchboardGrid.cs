using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class SwitchboardGrid : MonoBehaviour
{
    public int WIDTH = 5, HEIGHT = 5;

    [SerializeField] private SwitchboardPort _portPrefab;
    private Camera _camera;

    private SwitchboardPort[,] _portGrid;

    [Button()]
    void GenerateGrid()
    {
        _camera = Camera.main;

        if (_portGrid?.Length > 0)
        {
            foreach (var port in _portGrid)
            {
                DestroyImmediate(port.gameObject);
            }
        }
        _portGrid = new SwitchboardPort[HEIGHT, WIDTH];
        
        for (int jj = 0; jj < HEIGHT; ++jj)
        {
            for (int ii = 0; ii < WIDTH; ++ii)
            {
                Rect camRect = _camera.rect;
                Vector3 pos = new Vector3(
                    (ii - WIDTH * 0.5f + 0.5f) / WIDTH * _camera.orthographicSize * 2, 
                    (jj - HEIGHT * 0.5f + 0.5f) / HEIGHT * _camera.orthographicSize * 2,
                    0);

                _portGrid[jj, ii] = Instantiate(_portPrefab, transform);
                _portGrid[jj, ii].transform.localPosition = pos;
            }
        }
    }

}
