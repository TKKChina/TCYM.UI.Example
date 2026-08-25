using System.Runtime.CompilerServices;
using TCYM.UI.Native;

internal static class SkiaSharpAssemblyBootstrap
{
    [ModuleInitializer]
    internal static void RegisterTCYMNativeDependencies()
    {
        TCYMNativeDependencyResolver.Register();
    }
}
