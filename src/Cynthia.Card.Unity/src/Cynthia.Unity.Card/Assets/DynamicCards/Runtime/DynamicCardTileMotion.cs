using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.DynamicCards
{
    // Portable version of the source VFXTileMotion/VFXTile background conveyor.
    // Templates belong to this card; property blocks keep cameras and UVs instance-local.
    public sealed class DynamicCardTileMotion : MonoBehaviour
    {
        public GameObject[] TilePrefabs;
        public Transform WorldSpaceCamPos;
        public Vector3 StartPoint, EndPoint;
        public float Speed, TileSpacing;
        public int TileNum = 6;
        public Vector2 NoiseSpeed;
        private sealed class Tile
        {
            public Transform Transform;
            public Renderer[] Renderers;
            public MeshRenderer Surface;
            public Vector4 Noise;
            public readonly MaterialPropertyBlock Block = new MaterialPropertyBlock();
        }
        private readonly List<Tile> tiles = new List<Tile>();
        private Vector3 direction;
        private Vector3 coverageStart;
        private float age;

        private void Start()
        {
            if (TilePrefabs == null || TilePrefabs.Length == 0) return;
            direction = (EndPoint - StartPoint).normalized;
            // The shared portrait crop exposes more foreground than the source viewport.
            // Extend the conveyor by one original tile instead of stretching its artwork.
            coverageStart = StartPoint - direction * TileSpacing;
            transform.localRotation = Quaternion.identity;
            for (int i = 0; i < TileNum; i++)
            {
                var source = TilePrefabs[i % TilePrefabs.Length];
                if (source == null) continue;
                var clone = Instantiate(source, transform, false);
                clone.transform.localScale = Vector3.one;
                clone.transform.localPosition = coverageStart + direction * TileSpacing * i;
                clone.SetActive(true);
                var tile = new Tile { Transform = clone.transform, Renderers = clone.GetComponentsInChildren<Renderer>(true), Surface = clone.GetComponent<MeshRenderer>() };
                if (tile.Surface != null && tile.Surface.sharedMaterial != null)
                {
                    var material = tile.Surface.sharedMaterial;
                    var scale = material.GetTextureScale("_Noise");
                    var offset = material.GetTextureOffset("_Noise");
                    tile.Noise = new Vector4(scale.x, scale.y, offset.x, offset.y);
                }
                tiles.Add(tile);
            }
            ApplyCamera();
        }

        private void LateUpdate()
        {
            age += Time.deltaTime;
            foreach (var tile in tiles)
            {
                tile.Transform.Translate(direction * Speed * Time.deltaTime, Space.Self);
                if (tile.Transform.localPosition.z >= EndPoint.z)
                    tile.Transform.localPosition = coverageStart + direction * Vector3.Distance(tile.Transform.localPosition, EndPoint);
            }
            ApplyCamera();
        }

        private void ApplyCamera()
        {
            if (WorldSpaceCamPos == null) return;
            var position = WorldSpaceCamPos.position;
            foreach (var tile in tiles)
                foreach (var renderer in tile.Renderers)
                {
                    renderer.GetPropertyBlock(tile.Block);
                    tile.Block.SetVector("_CamPosition", new Vector4(position.x, position.y, position.z, 0));
                    if (renderer == tile.Surface)
                        tile.Block.SetVector("_Noise_ST", new Vector4(tile.Noise.x, tile.Noise.y, tile.Noise.z + age * NoiseSpeed.x, tile.Noise.w + age * NoiseSpeed.y));
                    renderer.SetPropertyBlock(tile.Block);
                }
        }
    }
}
