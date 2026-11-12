using System;
using System.Reflection;
using System.Linq;

var asm = Assembly.LoadFrom("src/TKCompass.App/bin/Release/net10.0/Avalonia.Base.dll");
var iclipboard = asm.GetType("Avalonia.Input.Platform.IClipboard");
if (iclipboard != null) {
    Console.WriteLine("=== IClipboard methods ===");
    foreach (var m in iclipboard.GetMethods().OrderBy(m => m.Name))
        Console.WriteLine($"  {m.ReturnType.Name} {m.Name}({string.Join(", ", m.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})");
}
