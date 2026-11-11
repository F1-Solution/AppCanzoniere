using System.Globalization;
using System.Runtime.CompilerServices;

namespace AppCanzoniere.Application.Tests;

internal static class TurkishCultureInitializer
{
    // Runs the whole suite under tr-TR so culture-dependent casing shows up in every test, including static regex setup.
    [ModuleInitializer]
    internal static void Initialize()
    {
        var turkish = new CultureInfo("tr-TR");
        CultureInfo.DefaultThreadCurrentCulture = turkish;
        CultureInfo.CurrentCulture = turkish;
    }
}
