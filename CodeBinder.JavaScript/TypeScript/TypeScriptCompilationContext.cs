// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

public class TypeScriptCompilationContext : CSharpCompilationContext<ConversionCSharpToTypeScript,
    TypeScriptClassContext, TypeScriptStructContext, TypeScriptInterfaceContext,
    TypeScriptEnumContext, TypeScriptDelegateContext>
{
    public TypeScriptCompilationContext(ConversionCSharpToTypeScript conversion)
        : base(conversion)
    {
    }

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            yield return new TypeScriptVerbatimConversionWriter($"NAPI{LibraryName}.mts", null,
$$"""
import * as proc from 'node:process';
import * as CodeBinder from './CodeBinder.mjs';

import { fileURLToPath } from 'node:url';

let shext = 'so';
switch (proc.platform)
{
	case 'win32':
	{
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
(proc as any).dlopen(mod, fileURLToPath(new URL(`{{LibraryName}}.${shext}`, import.meta.url)));
let napi = (mod.exports as any)({{ConversionCSharpToTypeScript.CodeBinderNamespace}});
export default napi;
""");

            yield return new TypeScriptLibraryConversion(this);
        }
    }

    protected override TypeScriptClassContext CreateContext(ClassDeclarationSyntax cls)
    {
        return new TypeScriptClassContext(cls, this);
    }

    protected override TypeScriptStructContext CreateContext(StructDeclarationSyntax str)
    {
        return new TypeScriptStructContext(str, this);
    }

    protected override TypeScriptInterfaceContext CreateContext(InterfaceDeclarationSyntax iface)
    {
        return new TypeScriptInterfaceContext(iface, this);
    }

    protected override TypeScriptEnumContext CreateContext(EnumDeclarationSyntax enm)
    {
        return new TypeScriptEnumContext(enm, this);
    }

    protected override TypeScriptDelegateContext CreateContext(DelegateDeclarationSyntax dlg)
    {
        return new TypeScriptDelegateContext(dlg, this);
    }

    protected override INodeVisitor CreateVisitor()
    {
        return new TypeScriptNodeVisitor(this);
    }
}
