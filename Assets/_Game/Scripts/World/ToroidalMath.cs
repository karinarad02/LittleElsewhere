using UnityEngine;

namespace LittleElsewhere.World
{
    /// <summary>
    /// Utility functions for toroidal (wraparound) world math.
    /// Provides canonical position wrapping, shortest wrapped displacement vectors,
    /// wrapped distance calculations, and 2D periodic (seamless) noise sampling.
    /// </summary>
    public static class ToroidalMath
    {
        /// <summary>
        /// Wraps a 2D world position into canonical world bounds [0, worldSize.x) and [0, worldSize.y).
        /// </summary>
        public static Vector2 WrapPosition(Vector2 pos, Vector2 worldSize)
        {
            float x = Mathf.Repeat(pos.x, worldSize.x);
            float y = Mathf.Repeat(pos.y, worldSize.y);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Computes the shortest displacement vector from 'from' to 'to' on a torus of size 'worldSize'.
        /// </summary>
        public static Vector2 WrappedDelta(Vector2 from, Vector2 to, Vector2 worldSize)
        {
            float dx = to.x - from.x;
            if (dx > worldSize.x * 0.5f) dx -= worldSize.x;
            else if (dx < -worldSize.x * 0.5f) dx += worldSize.x;

            float dy = to.y - from.y;
            if (dy > worldSize.y * 0.5f) dy -= worldSize.y;
            else if (dy < -worldSize.y * 0.5f) dy += worldSize.y;

            return new Vector2(dx, dy);
        }

        /// <summary>
        /// Computes the shortest distance between two points on a toroidal map.
        /// </summary>
        public static float WrappedDistance(Vector2 a, Vector2 b, Vector2 worldSize)
        {
            return WrappedDelta(a, b, worldSize).magnitude;
        }

        /// <summary>
        /// Samples 2D periodic (seamless) Perlin noise across opposite boundaries of a toroidal rectangle.
        /// Uses 4D cylinder-to-torus projection to ensure mathematical continuity across all edges.
        /// </summary>
        public static float SamplePeriodicNoise(float x, float y, Vector2 worldSize, float scale, int seed)
        {
            if (worldSize.x <= 0f || worldSize.y <= 0f) return 0f;

            // Map 2D rectangular coordinates (x, y) into 4D toroidal angles (u, v)
            float u = (x / worldSize.x) * Mathf.PI * 2f;
            float v = (y / worldSize.y) * Mathf.PI * 2f;

            // Compute 4D torus coordinates
            float nx = Mathf.Cos(u) * scale + (seed * 0.1337f);
            float ny = Mathf.Sin(u) * scale + (seed * 0.1337f);
            float nz = Mathf.Cos(v) * scale + (seed * 0.7331f);
            float nw = Mathf.Sin(v) * scale + (seed * 0.7331f);

            // Combine 2D Perlin noise pairs on 4D torus projection
            float noise1 = Mathf.PerlinNoise(nx, ny);
            float noise2 = Mathf.PerlinNoise(nz, nw);

            return (noise1 + noise2) * 0.5f;
        }
    }
}
