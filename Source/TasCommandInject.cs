using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TAS.Input.Commands;
using static On.TAS.Input.Commands.TasCommandAttribute;

namespace Celeste.Mod.CelesteTASLobbyWarpHelper;
public static class TasCommandInject {

    private static readonly Dictionary<TasCommandAttribute, MethodInfo> methodInfos = [];

    private static readonly Action<string, Func<Command, string>, Func<Command, bool>> updateAllMetadata = typeof(MetadataCommands)
        .GetMethod("UpdateAllMetadata", BindingFlags.NonPublic | BindingFlags.Static)
        .CreateDelegate<Action<string, Func<Command, string>, Func<Command, bool>>>();

    public static void UpdateAllMetadata(string commandName, Func<Command, string> getMetadata, Func<Command, bool> predicate = null) {
        updateAllMetadata(commandName, getMetadata, predicate);
    }

    internal static void CollectMethods() {
        methodInfos.Clear();
        IEnumerable<MethodInfo> infos = typeof(TasCommandInject)
            .Assembly
            .GetTypesSafe()
            .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(info => info.GetCustomAttributes<TasCommandAttribute>().Any());
        foreach (MethodInfo info in infos) {
            IEnumerable<TasCommandAttribute> tasCommandAttributes = info.GetCustomAttributes<TasCommandAttribute>();
            foreach (TasCommandAttribute tasCommandAttribute in tasCommandAttributes) {
                methodInfos[tasCommandAttribute] = info;
            }
        }
    }

    internal static KeyValuePair<TasCommandAttribute, MethodInfo> OnFindMethod(orig_FindMethod orig, string commandName) {
        var pair = methodInfos.FirstOrDefault(pair => pair.Key.IsName(commandName));
        if (pair.Key is null || pair.Value is null) {
            return orig(commandName);
        }
        return pair;
    }
}
