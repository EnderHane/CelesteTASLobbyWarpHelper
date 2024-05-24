using TAS.Input.Commands;

namespace Celeste.Mod.CelesteTASLobbyWarpHelper;

public static class LwhMetadataCommands {

    [TasCommand("Shit", AliasNames = ["Shit:", "Shit："], CalcChecksum = false)]
    private static void ShitCommand() {
        // dummy
    }

    [TasCommand("ActiveWarps", AliasNames = ["ActiveWarps:", "ActiveWarps："], CalcChecksum = false)]
    private static void ActiveWarpsCommand() {
        // dummy
    }
}
