using System.Diagnostics.CodeAnalysis;

namespace Gunk;

public static class CreatureTemplateType
{
    [AllowNull] public static CreatureTemplate.Type GunkLizard = new(nameof(GunkLizard), true);

    public static void UnregisterValues()
    {
        if (GunkLizard != null)
        {
            GunkLizard.Unregister();
            GunkLizard = null;
        }
    }
}

public static class SandboxUnlockID
{
    [AllowNull] public static MultiplayerUnlocks.SandboxUnlockID GunkLizard = new(nameof(GunkLizard), true);

    public static void UnregisterValues()
    {
        if (GunkLizard != null)
        {
            GunkLizard.Unregister();
            GunkLizard = null;
        }
    }
}