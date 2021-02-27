﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Testing : MonoBehaviour
{
    private Grid grid;

    private void Start()
    {
        grid = new Grid(50, 50, 4f, new Vector3(0, 0));
        HeatMapVisual heatMapVisual = new HeatMapVisual(grid, GetComponent<MeshFilter>());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.SetValue(UtilsClass.GetMouseWorldPosition(), 56);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetValue(UtilsClass.GetMouseWorldPosition()));
        }
    }

    private class HeatMapVisual
    {
        private Grid grid;

        public HeatMapVisual(Grid grid, MeshFilter meshFilter)
        {
            this.grid = grid;

            mesh = new Mesh();
            meshFilter.mesh = mesh;

            UpdateHeatMapVisual();

            grid.OnGridValueChanged += Grid_OnGridValueChanged;
        }

        private void Grid_OnGridValueChanged(object sender, System.EventArgs e)
        {
            UpdateHeatMapVisual();
        }

        public void UpdateHeatMapVisual()
        {
            Vector3[] vertices;
            Vector2[] uv;
            int[] triangles;

            MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out vertices, out uv, out triangles);

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    int index = x * grid.GetHeight() + y;
                    Vector3 baseSize = new Vector3(1, 1) * grid.GetCellSize();
                    int gridValue = grid.GetValue(x, y);
                    int maxGridValue = 100;
                    float GridValueNormalized = Mathf.Clamp01((float)gridValue / maxGridValue);
                    Vector2 gridCellUV = new Vector2(gridValueNormalized, 0f);
                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, GetWorldPosition(x, y) + baseSize * .5f, 0f, baseSize, gridCellUV, Vector2.zero);
                }
            }
            
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
    }
}
