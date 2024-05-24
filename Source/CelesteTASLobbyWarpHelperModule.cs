using System;
using System.Linq;

using TAS;

namespace Celeste.Mod.CelesteTASLobbyWarpHelper;

public class CelesteTASLobbyWarpHelperModule : EverestModule {
    public static CelesteTASLobbyWarpHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(CelesteTASLobbyWarpHelperModuleSettings);
    public static CelesteTASLobbyWarpHelperModuleSettings Settings => (CelesteTASLobbyWarpHelperModuleSettings)Instance._Settings;

    public override Type SessionType => typeof(CelesteTASLobbyWarpHelperModuleSession);
    public static CelesteTASLobbyWarpHelperModuleSession Session => (CelesteTASLobbyWarpHelperModuleSession)Instance._Session;

    public override Type SaveDataType => typeof(CelesteTASLobbyWarpHelperModuleSaveData);
    public static CelesteTASLobbyWarpHelperModuleSaveData SaveData => (CelesteTASLobbyWarpHelperModuleSaveData)Instance._SaveData;

    public CelesteTASLobbyWarpHelperModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(CelesteTASLobbyWarpHelperModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(CelesteTASLobbyWarpHelperModule), LogLevel.Info);
#endif
    }

    public override void Load() {
        TasCommandInject.CollectMethods();

        On.TAS.Manager.DisableRun += OnDisableRun;

        On.TAS.Input.Commands.TasCommandAttribute.FindMethod += TasCommandInject.OnFindMethod;
    }

    public override void Unload() {
        On.TAS.Manager.DisableRun -= OnDisableRun;

        On.TAS.Input.Commands.TasCommandAttribute.FindMethod -= TasCommandInject.OnFindMethod;
    }

    static void OnDisableRun(On.TAS.Manager.orig_DisableRun orig) {
        Logger.Log(nameof(CelesteTASLobbyWarpHelperModule), "Run disable.");

        TasCommandInject.UpdateAllMetadata("shit", _ => "a pile of shit");

        if (!Manager.Controller.CanPlayback && LobbyWarpHelper.TryGetActiveWarps(out var warps) && warps is not null) {
            var res = '{' + string.Join(", ", warps.Select(w => '\"' + w + '\"')) + '}';
            TasCommandInject.UpdateAllMetadata("ActiveWarps", _ => res);
        }
        orig();
    }
}