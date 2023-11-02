namespace CodeBinder.JavaScript.TypeScript;

class TypeScriptNAPIWrapperWriter : TypeScriptConversionWriter
{
    public TypeScriptNAPIWrapperWriter(TypeScriptCompilationContext context)
        : base(context)
    {
    }

    protected override string GetFileName()
    {
        return $"NAPI{Context.LibraryName}.{Context.Conversion.TypeScriptSourceExtension}";
    }

    protected override void write(CodeBuilder builder)
    {
        builder.AppendLine("import * as proc from 'node:process';");
        builder.AppendLine($"import * as CodeBinder from './CodeBinder{Context.Conversion.TypeScriptModuleLoadSuffix}';");
        if (Context.Conversion.GenerationFlags.HasFlag(TypeScriptGenerationFlags.CommonJSCompat))
            builder.AppendLine("import * as path from 'path';");
        else
            builder.AppendLine("import { fileURLToPath } from 'node:url';");
        builder.AppendLine();

        builder.AppendLine("""
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

const mod = { exports: {} };
// https://github.com/DefinitelyTyped/DefinitelyTyped/discussions/65252
""");

        if (Context.Conversion.GenerationFlags.HasFlag(TypeScriptGenerationFlags.CommonJSCompat))
            builder.AppendLine($"(proc as any).dlopen(mod, path.join(__dirname, `${{shprefix}}{Context.LibraryName}.${{shext}}`));");
        else
            builder.AppendLine($"(proc as any).dlopen(mod, fileURLToPath(new URL(`{{shprefix}}{Context.LibraryName}.${{shext}}`, import.meta.url)));");

        builder.AppendLine($"""
let napi = (mod.exports as any)({ConversionCSharpToTypeScript.CodeBinderNamespace});
export default napi;
""");
    }
}
