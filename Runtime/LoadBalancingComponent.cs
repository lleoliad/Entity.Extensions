using Fantasy;
using Fantasy.Entitas;
using Fantasy.Platform.Net;

namespace Entities.Extensions;

/// <summary>
/// Tracks scene load counts and allocates the least-loaded scene for a given scene type.
/// </summary>
public class LoadBalancingComponent : Entity
{
    private readonly Dictionary<uint, int> _sceneLoads = new Dictionary<uint, int>();

    /// <summary>
    /// Returns the least-loaded scene for the specified scene type and increments its load count.
    /// </summary>
    /// <param name="sceneType">The scene type to allocate.</param>
    /// <returns>The selected scene configuration.</returns>
    /// <exception cref="Exception">Thrown when no scene is available for the requested type.</exception>
    public SceneConfig Allocator(int sceneType)
    {
        var gateScenes = SceneConfigData.Instance.GetSceneBySceneType(sceneType);

        if (gateScenes.Count == 0)
        {
            throw new Exception($"No available scene found for scene type {sceneType}.");
        }

        // Pick the scene with the lowest current load for this scene type.
        var bestGate = gateScenes
            .OrderBy(s => GetSceneLoad(s.Id))
            .First();

        _sceneLoads[bestGate.Id] = GetSceneLoad(bestGate.Id) + 1;

#if ENTITY_DEBUG
        Log.Info($"Allocated scene {bestGate.Id} for type {bestGate.SceneTypeString} ({sceneType}). Current load: {_sceneLoads[bestGate.Id]}");
#endif
        return bestGate;
    }

    /// <summary>
    /// Decrements the tracked load for the specified scene.
    /// </summary>
    /// <param name="sceneId">The scene identifier to release.</param>
    public void Deallocator(uint sceneId)
    {
        _sceneLoads[sceneId] = GetSceneLoad(sceneId) - 1;
    }

    /// <summary>
    /// Gets the currently tracked load for a scene.
    /// </summary>
    /// <param name="sceneId">The scene identifier.</param>
    /// <returns>The current load count, or <c>0</c> if the scene is not tracked yet.</returns>
    private int GetSceneLoad(uint sceneId)
    {
        return _sceneLoads.TryGetValue(sceneId, out var load) ? load : 0;
    }
}
