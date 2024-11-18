#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public static class XMSListenerDefineSymbolAutoAdder
{
    // The define symbol you want to add
    private const string DefineSymbol = "XMSListener";

    // Static constructor is called when Unity loads or recompiles scripts
    static XMSListenerDefineSymbolAutoAdder()
    {
        AddDefineSymbol();
    }

    private static void AddDefineSymbol()
    {
        // Get the current build target group (Standalone, Android, etc.)
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

        // Get the current define symbols for this build target group
        string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        // Check if the define symbol already exists
        if (!defineSymbols.Contains(DefineSymbol))
        {
            // If not, add it
            defineSymbols += ";" + DefineSymbol;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defineSymbols);
            Debug.Log($"Added scripting define symbol: {DefineSymbol}");
        }
    }
}

#endif