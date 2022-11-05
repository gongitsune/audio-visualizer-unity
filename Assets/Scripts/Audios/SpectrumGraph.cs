using System;
using System.Linq;
using Lasp;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

//
// Spectrum graph rendering example
//
// You can retrieve the spectrum data from SpectrumAnalyzer using the following
// properties.
//
// - spectrumSpan / spectrumArray: Return raw spectrum data, which preserves the
//   full resolution of the FFT. It's useful for audio signal processing but not
//   very convenient for visual effects because it's not log-scaled.
//
//   spectrumSpan returns ReadOnlySpan<float>, and spectrumArray returns
//   NativeArray<float>. There is no performance difference between them.
//
// - logSpectrumSpan / logSpectrumArray: Returns log-scaled spectrum data, which
//   is useful for creating visual effects. Note that it causes inconsistencies
//   in the visual resolution. The low frequency band gets softened, and the
//   high frequency band gets sharpened.
//
// This script retrieves the spectrum data from a given SpectrumAnalyzer and
// renders it as a line strip mesh.
//
namespace Audios
{
    internal sealed class SpectrumGraph : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] private SpectrumAnalyzer input;
        [SerializeField] private bool logScale = true;
        [SerializeField] private Material material;

        #endregion

        #region MonoBehaviour implementation

        private void Update()
        {
            //
            // Retrieve the spectrum data (linear or log-scaled) and then construct
            // a line strip mesh with it.
            //
            var span = logScale ? input.logSpectrumSpan : input.spectrumSpan;
            if (_mesh == null) InitializeMesh(span);
            else UpdateMesh(span);

            // Draw the line strip mesh.
            Graphics.DrawMesh
            (_mesh, transform.localToWorldMatrix,
                material, gameObject.layer);
        }

        private void OnDestroy()
        {
            if (_mesh != null) Destroy(_mesh);
        }

        #endregion

        #region Line mesh operations

        private Mesh _mesh;

        private void InitializeMesh(ReadOnlySpan<float> source)
        {
            _mesh = new Mesh
            {
                bounds = new Bounds(Vector3.zero, Vector3.one * 10)
            };

            // Initial vertices
            using (var vertices = CreateVertexArray(source))
            {
                var desc = new VertexAttributeDescriptor
                    (VertexAttribute.Position);

                _mesh.SetVertexBufferParams(vertices.Length, desc);
                _mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);
            }

            // Initial indices
            using (var indices = CreateIndexArray(source.Length))
            {
                var desc = new SubMeshDescriptor
                    (0, indices.Length, MeshTopology.LineStrip);

                _mesh.SetIndexBufferParams(indices.Length, IndexFormat.UInt32);
                _mesh.SetIndexBufferData(indices, 0, 0, indices.Length);
                _mesh.SetSubMesh(0, desc);
            }
        }

        private void UpdateMesh(ReadOnlySpan<float> source)
        {
            using var vertices = CreateVertexArray(source);
            _mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);
        }

        private NativeArray<int> CreateIndexArray(int vertexCount)
        {
            return new NativeArray<int>
                (Enumerable.Range(0, vertexCount).ToArray(), Allocator.Temp);
        }

        private NativeArray<float3> CreateVertexArray(ReadOnlySpan<float> source)
        {
            var vertices = new NativeArray<float3>
            (source.Length, Allocator.Temp,
                NativeArrayOptions.UninitializedMemory);

            // Transfer spectrum data to the vertex array.
            var xScale = 1.0f / (source.Length - 1);
            for (var i = 0; i < source.Length; i++)
                vertices[i] = math.float3(i * xScale, source[i], 0);

            return vertices;
        }

        #endregion
    }
}