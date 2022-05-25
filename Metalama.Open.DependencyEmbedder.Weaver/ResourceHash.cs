// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Metalama.Open.DependencyEmbedder.Weaver;

public static class ResourceHash
{
    public static string CalculateHash( List<(string Name, Stream Stream)> resources )
    {
        var data = resources
            .OrderBy( r => r.Name )
            .Where( r => r.Name.StartsWith( "DependencyEmbedder", StringComparison.Ordinal ) )
            .Select( r => r.Stream )
            .ToArray();

        var allStream = new ConcatenatedStream( data );

#pragma warning disable CA5351
        using var md5 = MD5.Create();
#pragma warning restore CA5351
        var hashBytes = md5.ComputeHash( allStream );

        var sb = new StringBuilder();

        foreach ( var t in hashBytes )
        {
            sb.Append( t.ToString( "X2", CultureInfo.InvariantCulture ) );
        }

        allStream.ResetAllToZero();

        return sb.ToString();
    }
}