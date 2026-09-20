using System;
using UnityEngine;

namespace Assets.Script.DynamicCards
{
    [Serializable]
    public sealed class DynamicCardBundleIndex
    {
        public const int CurrentVersion = 2;
        public int version = CurrentVersion;
        public DynamicCardBundlePart[] parts;
    }

    [Serializable]
    public sealed class DynamicCardBundlePart
    {
        public string file;
        public string[] prefabs;
        public int animationControllers;
    }

    [Serializable]
    public class DynamicCardCatalog
    {
        public int version = 1;
        public DynamicCardEntry[] cards;
    }

    [Serializable]
    public class DynamicCardEntry
    {
        public string id;
        public string sourceId;
        public string sourceVersion;
        public string[] artIds;
        public string prefab;
        public string audio;
        public string pivot;
        public float fieldOfView = 25;
        public float cameraDistance = -29.87103f;
        public float nearClip = 1;
        public float farClip = 300;
        public float xStart = -6, xEnd = 6, yStart = -2, yEnd = 2;
        public float introDuration;
        public float topMargin;
        public float loopDuration = 1;
        public DynamicCardParticleEvent[] particleEvents;
        public DynamicCardUvMotion[] uvMotions;
        public DynamicCardTransformPair[] transformPairs;
        public DynamicCardViewMotion[] viewMotions;
        public DynamicCardCameraParent[] cameraParents;
        public DynamicCardJiggle[] jiggles;
        public DynamicCardWiggle[] wiggles;
        public DynamicCardMaterialValue[] materialValues;
        public DynamicCardInitialTransform[] initialTransforms;
        public string[] nonRenderingPaths;
        // Additional square-render offset when the source backdrop cannot cover the default portrait.
        public float verticalFramingCorrection;
        // Minimum framing offset at the drag boundary; zero uses the standard margin.
        public float dragFramingCorrection;
        // Independent correction for the narrow deck-list focal region.
        public float thumbnailFramingCorrection;
        public float cutTime = -1;
        public string beforeCut, afterCut;
    }

    [Serializable]
    public class DynamicCardParticleEvent
    {
        public string path;
        public float time;
        public bool loop,sourceTiming;
        public float phaseStart,period;
    }

    [Serializable]
    public class DynamicCardUvMotion
    {
        public string path, property;
        public int materialIndex;
        public Vector2 speed, start;
        public bool forceStart;
    }

    [Serializable]
    public class DynamicCardTransformPair
    {
        public string source, target;
    }
}
