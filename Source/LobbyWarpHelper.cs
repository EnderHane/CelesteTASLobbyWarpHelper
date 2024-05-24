using System;
using System.Linq;
using System.Reflection;

using Microsoft.Xna.Framework;

using Celeste.Mod.CollabUtils2;
using Celeste.Mod.CollabUtils2.Entities;
using Celeste.Mod.CollabUtils2.UI;


namespace Celeste.Mod.CelesteTASLobbyWarpHelper;
public static class LobbyWarpHelper {

    static readonly Func<LobbyMapController.ControllerInfo, LobbyVisitManager, ByteArray2D> generateVisitedTiles = typeof(LobbyMapUI)
        .GetMethod("generateVisitedTiles", BindingFlags.Static | BindingFlags.NonPublic)
        .CreateDelegate<Func<LobbyMapController.ControllerInfo, LobbyVisitManager, ByteArray2D>>();

    static readonly Func<LobbyMapWarp, LobbyMapController.MarkerInfo> getMarkerInfo = w => (LobbyMapController.MarkerInfo)typeof(LobbyMapWarp)
        .GetField("info", BindingFlags.Instance | BindingFlags.NonPublic)
        .GetValue(w);

    public static bool IsVisited(ByteArray2D visitedTiles, Vector2 position, byte threshold = 0x7F) {
        return visitedTiles.TryGet((int)(position.X / 8), (int)(position.Y / 8), out var value) && value > threshold;
    }


    public static bool TryGetActiveWarps(out string[] activeWarps) {
        activeWarps = null;
        if (Celeste.Scene is not Level level) {
            return false;
        }
        var warps = level.Tracker.GetEntities<LobbyMapWarp>();
        var controller = level.Tracker.GetEntity<LobbyMapController>();
        if (controller is null) {
            return false;
        }
        var visitedTiles = generateVisitedTiles(controller.Info, controller.VisitManager);
        activeWarps = warps
            .Select(w => getMarkerInfo((LobbyMapWarp)w))
            .Where(m => IsVisited(visitedTiles, m.Position) &&
                m.Type == LobbyMapController.MarkerType.Warp &&
                (!m.WarpRequiresActivation || controller.VisitManager.ActivatedWarps.Contains(m.MarkerId))
                )
            .Select(m => m.MarkerId)
            .ToArray();
        return true;
    }
}
