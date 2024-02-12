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
        builder.AppendLine("import * as fs from 'fs';");
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

function getLibraryPath(libFileName: string): string
{
""");

        if (Context.Conversion.GenerationFlags.HasFlag(TypeScriptGenerationFlags.CommonJSCompat))
            builder.AppendLine($"    return path.join(__dirname, arch, libFileName);");
        else
            builder.AppendLine($"    return fileURLToPath(new URL(path.join(arch, libFileName), import.meta.url));");

        builder.AppendLine(
"""
}

function loadLibrary(exports: object, libName: string): void
{
    let libpath = getLibraryPath(`${libName}.${shext}`);
    if (!fs.existsSync(libpath))
    {
        libpath = getLibraryPath(`${shprefix}${libName}.${shext}`);
        if (!fs.existsSync(libpath))
           throw new Error(`Could not find library ${libName}`); 
    }

    // https://github.com/DefinitelyTyped/DefinitelyTyped/discussions/65252
    (proc as any).dlopen(exports, libpath);
}

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
        builder.AppendLine($"loadLibrary(mod, '{libraryName}');");
    }
}
