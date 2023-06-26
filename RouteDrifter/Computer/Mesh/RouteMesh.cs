using System;
using System.Collections.Generic;
using RouteDrifter.Models;
using RouteDrifter.Utility.Extensions;
using UnityEngine;

namespace RouteDrifter.Computer.Mesh
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RouteMesh : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private RouteComputer _RouteComputer;
        [SerializeField] private bool _BuildOnAwake;
        [SerializeField] private float _HalfWidth;
        [SerializeField] private float _Thickness;
        
        #endregion

        private MeshFilter _meshFilter;
        private UnityEngine.Mesh _mesh;
        private MeshRenderer _meshRenderer;
        private List<Vector3> _vertices;
        private List<int> _triangles;

        private bool HaveThickness => _Thickness != 0f;
        public MeshRenderer MeshRenderer => _meshRenderer;
        

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

        public void Set(bool buildOnAwake, float halfWidth, float thickness)
        {
            _HalfWidth = halfWidth;
            _Thickness = thickness;

            // Needed to call it again for instantiating at runtime, not as a scene root object.
            // Because Awake is called as soon as the object gets instantiated.
            if (buildOnAwake)
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
            _meshRenderer = GetComponent<MeshRenderer>();
            
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
            SetSubMeshes();
            CreateVertices(samplePoints);
            CreateTriangles();
            SetUV();
            SetNormals();
            
            _mesh.UploadMeshData(false);
        }

        private void SetNormals()
        {
            var vertexCount = _mesh.vertices.Length;
            var normals = new Vector3[vertexCount];
            
            for (var vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
            {
                normals[vertexIndex] = Vector3.up;
            }
            
            _mesh.SetNormals(normals);
        }

        private void SetUV()
        {
            var uvs = new List<Vector2>();

            var vertexCount = HaveThickness ? _mesh.vertices.Length / 2 : _mesh.vertices.Length;

            #region Upper Surface
            for (var vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
            {
                float u;
                float v;
                if (vertexIndex % 2 == 0)
                {
                    u = 0;
                    v = (float) vertexIndex / vertexCount;
                    uvs.Add(new Vector2(u, v));

                    continue;
                }
                
                u = 1;
                v = (float) vertexIndex / vertexCount;
                uvs.Add(new Vector2(u, v));
            }
            #endregion

            #region Lower Surface

            if (HaveThickness)
            {
                uvs.AddRange(uvs);
            }
            
            #endregion

            _mesh.uv = uvs.ToArray();
        }

        private void ClearMesh()
        {
            _mesh.Clear();
        }

        private void SetSubMeshes()
        {
            _mesh.subMeshCount = HaveThickness ? 6 : 1;
        }

        private void CreateVertices(List<SamplePoint> samplePoints)
        {
            _vertices.Clear();

            var topPoints = new List<Vector3>();

            var samplePointCount = samplePoints.Count;
            for (int pointIndex = 0; pointIndex < samplePointCount; pointIndex++)
            {
                var topLeftPoint = samplePoints[pointIndex]._LocalPosition
                                + samplePoints[pointIndex]._Forward.TurnLeftAroundY().normalized * _HalfWidth;
                var topRightPoint = samplePoints[pointIndex]._LocalPosition
                                + samplePoints[pointIndex]._Forward.TurnRightAroundY().normalized * _HalfWidth;

                topPoints.Add(topLeftPoint);
                topPoints.Add(topRightPoint);
            }
            
            _vertices.AddRange(topPoints);

            if (HaveThickness)
            {
                var bottomPoints = new List<Vector3>();

                topPoints.ForEach(topPoint =>
                {
                    var bottomPoint = topPoint;
                    bottomPoint += _Thickness * Vector3.down;
                    bottomPoints.Add(bottomPoint);
                });
                
                _vertices.AddRange(bottomPoints);
            }

            _mesh.vertices = _vertices.ToArray();
        }
        
        private void CreateTriangles()
        {
            _triangles.Clear();
            
            CreateUpperSurfaceTriangles();

            if (HaveThickness)
            {
                CreateLowerSurfaceTriangles();
                CreateLeftSurfaceTriangles();
                CreateRightSurfaceTriangles();
                CreateCapTriangles();
            }
        }

        private void CreateCapTriangles()
        {
            var halfVertexCount = _mesh.vertices.Length / 2;

            var rearCapTriangles = new List<int>()
            {
                0, 1, halfVertexCount + 1,
                0, halfVertexCount + 1, halfVertexCount
            };
            
            _mesh.SetTriangles(rearCapTriangles, 4);


            var frontCapTriangles = new List<int>()
            {
                halfVertexCount - 1, halfVertexCount - 2, halfVertexCount * 2 - 2,
                halfVertexCount - 1, halfVertexCount * 2 - 2, halfVertexCount * 2 - 1
            };
            
            _mesh.SetTriangles(frontCapTriangles, 5);
        }

        private void CreateRightSurfaceTriangles()
        {
            var rightSurfaceTriangles = new List<int>();

            var halfVertexCount = _mesh.vertices.Length / 2;

            for (int vertexIndex = 1; vertexIndex < halfVertexCount - 1; vertexIndex +=2)
            {
                rightSurfaceTriangles.Add(vertexIndex);
                rightSurfaceTriangles.Add(vertexIndex + 2);
                rightSurfaceTriangles.Add(vertexIndex + halfVertexCount + 2);
                
                rightSurfaceTriangles.Add(vertexIndex);
                rightSurfaceTriangles.Add(vertexIndex + halfVertexCount + 2);
                rightSurfaceTriangles.Add(vertexIndex + halfVertexCount);
            }
            
            _mesh.SetTriangles(rightSurfaceTriangles, 3);
        }

        private void CreateLeftSurfaceTriangles()
        {
            var leftSurfaceTriangles = new List<int>();

            var halfVertexCount = _mesh.vertices.Length / 2;

            for (int vertexIndex = 0; vertexIndex < halfVertexCount - 2; vertexIndex +=2)
            {
                leftSurfaceTriangles.Add(vertexIndex);
                leftSurfaceTriangles.Add(vertexIndex + halfVertexCount);
                leftSurfaceTriangles.Add(vertexIndex + halfVertexCount + 2);
                
                leftSurfaceTriangles.Add(vertexIndex);
                leftSurfaceTriangles.Add(vertexIndex + halfVertexCount + 2);
                leftSurfaceTriangles.Add(vertexIndex + 2);
            }
            
            _mesh.SetTriangles(leftSurfaceTriangles, 2);
        }

        private void CreateLowerSurfaceTriangles()
        {
            var lowerSurfaceTriangles = new List<int>();
                
            var lowerSurfaceVertexCount = _mesh.vertices.Length / 2;
                
            for (int vertexIndex = 0; vertexIndex < lowerSurfaceVertexCount - 2; vertexIndex +=2)
            {
                lowerSurfaceTriangles.Add(lowerSurfaceVertexCount + vertexIndex);
                lowerSurfaceTriangles.Add(lowerSurfaceVertexCount + vertexIndex + 1);
                lowerSurfaceTriangles.Add(lowerSurfaceVertexCount + vertexIndex + 3);
                
                lowerSurfaceTriangles.Add(lowerSurfaceVertexCount + vertexIndex);
                lowerSurfaceTriangles.Add(lowerSurfaceVertexCount + vertexIndex + 3);
                lowerSurfaceTriangles.Add(lowerSurfaceVertexCount + vertexIndex + 2);
            }

            _mesh.SetTriangles(lowerSurfaceTriangles, 1);
        }

        private void CreateUpperSurfaceTriangles()
        {
            var upperSurfaceVertexCount = HaveThickness ? _mesh.vertices.Length / 2 : _mesh.vertices.Length;
            
            for (int upperSurfaceVertexIndex = 0;
                 upperSurfaceVertexIndex < upperSurfaceVertexCount - 2;
                 upperSurfaceVertexIndex += 2)
            {
                _triangles.Add(upperSurfaceVertexIndex);
                _triangles.Add(upperSurfaceVertexIndex + 3);
                _triangles.Add(upperSurfaceVertexIndex + 1);

                _triangles.Add(upperSurfaceVertexIndex);
                _triangles.Add(upperSurfaceVertexIndex + 2);
                _triangles.Add(upperSurfaceVertexIndex + 3);
            }

            _mesh.SetTriangles(_triangles, 0);
        }

        private UnityEngine.Mesh CreateLeftSurface()
        {
            var leftSurfaceMesh = new UnityEngine.Mesh();

            var leftSurfaceVertices = new List<Vector3>();
            var leftSurfaceTriangles = new List<int>();
            var leftSurfaceUVs = new List<Vector2>();
            
            for (int i = 0; i < _vertices.Count; i++)
            {
                if (i % 2 == 0)
                {
                    leftSurfaceVertices.Add(_vertices[i]);
                }
            }
            
            var vertexCount = leftSurfaceVertices.Count;
            
            for (int vertexIndex = 0; vertexIndex < vertexCount / 2 - 1 ; vertexIndex++)
            {
                leftSurfaceTriangles.Add(vertexIndex);
                leftSurfaceTriangles.Add(vertexCount / 2 + vertexIndex);
                leftSurfaceTriangles.Add(vertexCount / 2 + vertexIndex + 1);
                
                leftSurfaceTriangles.Add(vertexIndex);
                leftSurfaceTriangles.Add(vertexCount / 2 + vertexIndex + 1);
                leftSurfaceTriangles.Add(vertexIndex + 1);
            }
            
            for (var vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
            {
                float u;
                float v;
                if (vertexIndex < vertexCount / 2)
                {
                    u = 1;
                    v = (float) vertexIndex / (vertexCount / 2 - 1);
                    leftSurfaceUVs.Add(new Vector2(u, v));

                    continue;
                }
                
                u = 0;
                v = (float) (vertexIndex - vertexCount / 2) / (vertexCount / 2 - 1);
                leftSurfaceUVs.Add(new Vector2(u, v));
            }

            leftSurfaceMesh.vertices = leftSurfaceVertices.ToArray();
            leftSurfaceMesh.triangles = leftSurfaceTriangles.ToArray();
            leftSurfaceMesh.uv = leftSurfaceUVs.ToArray();
            leftSurfaceMesh.UploadMeshData(false);

            return leftSurfaceMesh;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            try
            {
                if (_RouteComputer != null && _RouteComputer.SamplePoints != null)
                {
                    Initialize();

                    Build(_RouteComputer.SamplePoints);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return;
            }
        }
#endif
        
    }
}