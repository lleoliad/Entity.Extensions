using Fantasy;
using Fantasy.Platform.Net;

namespace Entities.Extensions;

/// <summary>
/// Provides scene helpers for extension-level runtime features.
/// </summary>
public static class SceneExtensions
{
    /// <summary>
    /// Allocates the least-loaded scene for the specified scene type from the scene load balancer.
    /// </summary>
    /// <param name="scene">The source scene that owns the load-balancing component.</param>
    /// <param name="sceneType">The scene type to allocate.</param>
    /// <returns>The selected scene configuration.</returns>
    public static SceneConfig Allocator(this Scene scene, int sceneType)
    {
        return scene.GetComponent<LoadBalancing>().Allocator(sceneType);
    }
}
