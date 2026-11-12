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
var dataTransfer = asm.GetType("Avalonia.Input.DataTransfer");
if (dataTransfer != null) {
    Console.WriteLine("\n=== DataTransfer methods ===");
    foreach (var m in dataTransfer.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).OrderBy(m => m.Name))
        Console.WriteLine($"  {m.ReturnType.Name} {m.Name}({string.Join(", ", m.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})");
}
var dataFormat = asm.GetType("Avalonia.Input.DataFormat");
if (dataFormat != null) {
    Console.WriteLine("\n=== DataFormat static fields/props ===");
    foreach (var f in dataFormat.GetFields(BindingFlags.Public | BindingFlags.Static).OrderBy(f => f.Name))
        Console.WriteLine($"  {f.FieldType.Name} {f.Name}");
    foreach (var p in dataFormat.GetProperties(BindingFlags.Public | BindingFlags.Static).OrderBy(p => p.Name))
        Console.WriteLine($"  {p.PropertyType.Name} {p.Name}");
}
