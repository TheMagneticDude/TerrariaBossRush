using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeMetrics;

if (args.Length != 2)
    throw new ArgumentException("Usage: GenerateMetrics <repository-root> <output.csv>");

string root = Path.GetFullPath(args[0]);
string packageRoot = Environment.GetEnvironmentVariable("NUGET_PACKAGES")
    ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
string FindPackageDirectory(string name, string pattern) => Directory.GetDirectories(Path.Combine(packageRoot, name), pattern)
    .OrderByDescending(path => Version.Parse(Path.GetFileName(path))).First();
string referenceDirectory = Path.Combine(FindPackageDirectory("microsoft.netcore.app.ref", "9.*"), "ref", "net9.0");
string monoGameAssembly = Path.Combine(FindPackageDirectory("monogame.framework.desktopgl", "3.8.*"), "lib", "net8.0", "MonoGame.Framework.dll");
var references = Directory.GetFiles(referenceDirectory, "*.dll").Select(path => MetadataReference.CreateFromFile(path)).ToList();
references.Add(MetadataReference.CreateFromFile(monoGameAssembly));
var parseOptions = new CSharpParseOptions(LanguageVersion.CSharp13,
    preprocessorSymbols: new[] { "DEBUG", "TRACE", "NET", "NET9_0", "NETCOREAPP" });
var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
var rows = new List<string[]>();

CSharpCompilation LoadProject(string name, IEnumerable<MetadataReference> projectReferences)
{
    var sources = Directory.EnumerateFiles(Path.Combine(root, name), "*.cs", SearchOption.AllDirectories)
        .Where(path => !Path.GetRelativePath(Path.Combine(root, name), path).Split(Path.DirectorySeparatorChar)
            .Any(part => part is "bin" or "obj"))
        .OrderBy(path => path, StringComparer.Ordinal)
        .Select(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path), parseOptions, path));
    var compilation = CSharpCompilation.Create(name, sources, projectReferences, compilationOptions.WithOutputKind(name == "terraria" ? OutputKind.WindowsApplication : OutputKind.DynamicallyLinkedLibrary));
    var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
    if (errors.Length > 0)
        throw new InvalidOperationException(string.Join(Environment.NewLine, errors.Select(d => d.ToString())));
    return compilation;
}

void Collect(CodeAnalysisMetricData data, string project)
{
    ISymbol symbol = data.Symbol;
    if (symbol is INamedTypeSymbol || (symbol.ContainingType != null && symbol is not INamespaceSymbol))
    {
        bool isType = symbol is INamedTypeSymbol;
        var type = isType ? (INamedTypeSymbol)symbol : symbol.ContainingType;
        string ns = type.ContainingNamespace.IsGlobalNamespace ? "<global namespace>" : type.ContainingNamespace.ToDisplayString();
        string member = isType ? "" : symbol is IMethodSymbol method && method.MethodKind is MethodKind.Constructor or MethodKind.StaticConstructor
            ? type.Name : symbol.Name;
        rows.Add(new[] { isType ? "Type" : "Member", project, ns, type.Name, member,
            data.MaintainabilityIndex.ToString(), data.CyclomaticComplexity.ToString(),
            (data.DepthOfInheritance ?? 0).ToString(), data.CoupledNamedTypes.Count.ToString(),
            data.SourceLines.ToString(), data.ExecutableLines.ToString() });
    }
    foreach (var child in data.Children) Collect(child, project);
}

var library = LoadProject("MonoGameLibrary", references);
Collect(await CodeAnalysisMetricData.ComputeAsync(new CodeMetricsAnalysisContext(library, CancellationToken.None)), "MonoGameLibrary");
var gameReferences = references.Cast<MetadataReference>().Append(library.ToMetadataReference());
var game = LoadProject("terraria", gameReferences);
Collect(await CodeAnalysisMetricData.ComputeAsync(new CodeMetricsAnalysisContext(game, CancellationToken.None)), "terraria");

string Escape(string value) => value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0
    ? "\"" + value.Replace("\"", "\"\"") + "\"" : value;
using var output = new StreamWriter(args[1], false, new System.Text.UTF8Encoding(false));
output.WriteLine("Scope,Project,Namespace,Type,Member,Maintainability Index,Cyclomatic Complexity,Depth of Inheritance,Class Coupling,Lines of Source code,Lines of Executable code");
foreach (var row in rows) output.WriteLine(string.Join(",", row.Select(Escape)));
Console.WriteLine($"Wrote {rows.Count} metric rows to {args[1]}.");
