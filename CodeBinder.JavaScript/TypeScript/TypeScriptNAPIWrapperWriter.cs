using CodeBinder.Attributes;

namespace CodeBinder.JavaScript.TypeScript;

class TypeScriptNAPIWrapperWriter : TypeScriptConversionWriter
{
    public TypeScriptNAPIWrapperWriter(TypeScriptCompilationContext context)
        : base(context)
    {
    }

    protected override string GetFileName()
    {
        return $"{Context.NAPIWrapperName}.{Context.Conversion.TypeScriptSourceExtension}";
    }

    protected override void write(CodeBuilder builder)
    {
        builder.AppendLine("import * as proc from 'node:process';");
        builder.AppendLine($"import * as CodeBinder from './CodeBinder{Context.Conversion.TypeScriptModuleLoadSuffix}';");
        builder.AppendLine("import * as path from 'path';");
        if (!Context.Conversion.GenerationFlags.HasFlag(TypeScriptGenerationFlags.CommonJSCompat))
            builder.AppendLine("import { fileURLToPath } from 'node:url';");
        builder.AppendLine();

        builder.AppendLine("""
const archsMap: { [key: string]: any } = {
    'win32|x86': 'Win32',
    'win32|x64': 'Win64',
    'linux|x64': 'linux-x86_64',
    'darwin|x64': 'macos-x86_64',
    'darwin|arm64': 'macos-arm64',
  };

let narch = `${proc.platform}|${proc.arch}`
let arch = archsMap[narch];
if (arch === undefined)
    throw new Error(`Unsupported architecture ${narch}`);

let shprefix = 'lib';
let shext = 'so';
switch (proc.platform)
{
    case 'win32':
    {
        shprefix = '';
        shext = 'dll'
        break;
    }
    case 'darwin':
    {
        shext = 'dylib'
        break;
    }
}

// https://github.com/DefinitelyTyped/DefinitelyTyped/discussions/65252

const mod = { exports: {} };
""");

        var dependencies = Context.Compilation.Assembly.GetAttributes<NativeDependencyAttribute>();
        foreach (var dependency in dependencies)
            appendLoadLibrary(builder, dependency.GetConstructorArgument<string>(0));

        appendLoadLibrary(builder, Context.LibraryName);

        builder.AppendLine($"""
let napi = (mod.exports as any)({ConversionCSharpToTypeScript.CodeBinderNamespace});
export default napi;
""");
    }

    void appendLoadLibrary(CodeBuilder builder, string libraryName)
    {
        if (Context.Conversion.GenerationFlags.HasFlag(TypeScriptGenerationFlags.CommonJSCompat))
            builder.AppendLine($"(proc as any).dlopen(mod, path.join(__dirname, arch, `${{shprefix}}{libraryName}.${{shext}}`));");
        else
            builder.AppendLine($"(proc as any).dlopen(mod, fileURLToPath(new URL(path.join(arch, `${{shprefix}}{libraryName}.${{shext}}`), import.meta.url)));");

    }
}
