// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Metalama.Open.DependencyEmbedder.Weaver;

public class AssemblyLoaderInfo
{
    public const string AssemblyNamesField = "assemblyNames";

    public const string SymbolNamesField = "symbolNames";

    public const string PreloadListField = "preloadList";

    public const string Preload32ListField = "preload32List";

    public const string Preload64ListField = "preload64List";

    public string? ChecksumsField { get; }

    public string? Md5HashField { get; }

    public CompilationUnitSyntax SourceTypeSyntax { get; }

    public string SourceTypeName { get; }

    public AssemblyLoaderInfo( string? checksumsField, string? md5HashField, CompilationUnitSyntax sourceTypeSyntax, string sourceTypeName )
    {
        this.ChecksumsField = checksumsField;
        this.Md5HashField = md5HashField;
        this.SourceTypeSyntax = sourceTypeSyntax;
        this.SourceTypeName = sourceTypeName;
    }

    public static AssemblyLoaderInfo LoadAssemblyLoader(
        bool createTemporaryAssemblies,
        bool hasUnmanaged )
    {
        string sourceTypeCode;
        string sourceTypeName;

        if ( createTemporaryAssemblies )
        {
            sourceTypeCode = Resources.TemplateWithTempAssembly;
            sourceTypeName = "TemplateWithTempAssembly";
        }
        else if ( hasUnmanaged )
        {
            sourceTypeCode = Resources.TemplateWithUnmanagedHandler;
            sourceTypeName = "TemplateWithUnmanagedHandler";
        }
        else
        {
            sourceTypeCode = Resources.Template;
            sourceTypeName = "Template";
        }

        var sourceTypeSyntax = SyntaxFactory.ParseCompilationUnit( sourceTypeCode );

        return new AssemblyLoaderInfo( Optional( "checksums" ), Optional( "md5Hash" ), sourceTypeSyntax, sourceTypeName );

        string? Optional( string field )
        {
            return sourceTypeSyntax.DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .SelectMany( f => f.Declaration.Variables )
                .Any( f => f.Identifier.ValueText == field )
                ? field
                : null;
        }
    }
}