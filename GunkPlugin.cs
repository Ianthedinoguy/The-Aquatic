using BepInEx;
using System.Security.Permissions;
using System.Security;
using BepInEx.Logging;
using Fisobs.Core;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS0618 // ignore false message
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Gunk;

[BepInPlugin("toothandclaw.Gunk-lizard", "Gunk Lizard", "1.2.0")]
sealed class GunkPlugin : BaseUnityPlugin
{
    [AllowNull] internal static ManualLogSource logger;

    public void OnEnable()
    {
        logger = Logger;
        On.RainWorld.OnModsDisabled += (orig, self, newlyDisabledMods) =>
        {
            orig(self, newlyDisabledMods);
            for (var i = 0; i < newlyDisabledMods.Length; i++)
            {
                if (newlyDisabledMods[i].id == "toothandclaw.Gunk-lizard")
                {
                    if (MultiplayerUnlocks.CreatureUnlockList.Contains(SandboxUnlockID.GunkLizard))
                        MultiplayerUnlocks.CreatureUnlockList.Remove(SandboxUnlockID.GunkLizard);
                    CreatureTemplateType.UnregisterValues();
                    SandboxUnlockID.UnregisterValues();
                    break;
                }
            }
        };
        Content.Register(new GunkLizardCritob());
    }

    public void OnDisable() => logger = default;
}