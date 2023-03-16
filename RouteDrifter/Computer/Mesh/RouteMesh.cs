﻿using System.Collections.Generic;
using RouteDrifter.Models;
using RouteDrifter.Utility.Extensions;
using UnityEngine;

namespace RouteDrifter.Computer.Mesh
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RouteMesh : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private RouteComputer _RouteComputer;
        [SerializeField] private bool _BuildOnAwake;
        [SerializeField] private float _HalfWidth;

        #endregion

        private MeshFilter _meshFilter;

        private UnityEngine.Mesh _mesh;
        private List<Vector3> _vertices;
        private List<int> _triangles;

        private void Awake()
        {
            Initialize();
            
            if (_BuildOnAwake)
            {
                if (_RouteComputer)
                {
                    Build(_RouteComputer.SamplePoints);
                }
            }
        }

        private void Initialize()
        {
            _meshFilter = GetComponent<MeshFilter>();
            
            _mesh = new UnityEngine.Mesh();
            _vertices = new List<Vector3>();
            _triangles = new List<int>();

            _meshFilter.sharedMesh = _mesh;
        }

        private void OnEnable()
        {
            RegisterEvents();
        }

        private void OnDisable()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            _RouteComputer?.RegisterBuildCompleted(OnBuild);
        }

        private void UnregisterEvents()
        {
            _RouteComputer?.UnregisterBuildCompleted(OnBuild);
        }

        private void OnBuild(List<SamplePoint> samplePoints)
        {
            Build(samplePoints);
        }
        
        public void Build(List<SamplePoint> samplePoints)
        {
            if (_RouteComputer == null)
            {
                return;
            }

            BuildMesh(samplePoints);
        }

        private void BuildMesh(List<SamplePoint> samplePoints)
        {
            ClearMesh();
            CreateVertices(samplePoints);
            CreateTriangles();
            _mesh.UploadMeshData(false);
        }

        private void ClearMesh()
        {
            _mesh.Clear();
        }

        private void CreateVertices(List<SamplePoint> samplePoints)
        {
            _vertices.Clear();

            var samplePointCount = samplePoints.Count;
            for (int pointIndex = 0; pointIndex < samplePointCount; pointIndex++)
            {
                var leftPoint = samplePoints[pointIndex]._LocalPosition
                                + samplePoints[pointIndex]._Forward.TurnLeftAroundY().normalized * _HalfWidth;
                var rightPoint = samplePoints[pointIndex]._LocalPosition
                                + samplePoints[pointIndex]._Forward.TurnRightAroundY().normalized * _HalfWidth;
                
                _vertices.Add(leftPoint);
                _vertices.Add(rightPoint);
            }

            _mesh.vertices = _vertices.ToArray();
        }
        
        private void CreateTriangles()
        {
            _triangles.Clear();
            
            var vertexCount = _mesh.vertices.Length;

            for (int vertexIndex = 0; vertexIndex < vertexCount - 2; vertexIndex +=2)
            {
                _triangles.Add(vertexIndex);
                _triangles.Add(vertexIndex + 3);
                _triangles.Add(vertexIndex + 1);
                
                _triangles.Add(vertexIndex);
                _triangles.Add(vertexIndex + 2);
                _triangles.Add(vertexIndex + 3);
            }

            _mesh.triangles = _triangles.ToArray();
        }
    }
}