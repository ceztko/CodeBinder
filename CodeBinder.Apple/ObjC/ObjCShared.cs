using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CodeBinder.Apple
{
    /// <summary>
    /// Header type
    /// </summary>
    enum ObjCFileType
    {
        /// <summary>The file is a public header</summary>
        PublicHeader,
        /// <summary>The file is an internal header</summary>
        InternalHeader,
        /// <summary>The file is an internal only header</summary>
        InternalOnlyHeader,
        /// <summary>The file is an implementation</summary>
        Implementation,
    }

    /// <summary>
    /// Header type
    /// </summary>
    enum ObjCHeaderType
    {
        /// <summary>The header contains the public interface</summary>
        Public,
        /// <summary>The header contains the internal interface</summary>
        Internal,
        /// <summary>The header is internal only</summary>
        InternalOnly,
    }

    /// <summary>
    /// Implementation type
    /// </summary>
    enum ObjImplementationType
    {
        /// <summary>This is the implementation of a public type</summary>
        PublicType,
        /// <summary>>This is the implementation of an internal type</summary>
        InternalType,
    }

    [DebuggerDisplay("TypeName = " + nameof(TypeName))]
    struct ObjCTypeInfo
    {
        public string TypeName;
        public ObjCTypeKind Kind;
        public ObjCTypeReachability Reachability;
    }

    enum ObjCTypeKind
    {
        /// <summary>
        /// ObjectiveC class (interface) type
        /// </summary>
        Class,
        /// <summary>
        /// ObjectiveC protocol type
        /// </summary>
        Protocol,
        /// <summary>
        /// Value types
        /// </summary>
        Value,
        /// <summary>
        /// Special type "void"
        /// </summary>
        Void,
    }

    enum ObjCTypeReachability
    {
        /// <summary>
        /// The type is defined in this library and it's public
        /// </summary>
        Public,
        /// <summary>
        /// The type is defined in this library and it's internal
        /// </summary>
        Internal,
        /// <summary>
        /// The type is defined outside of this library
        /// </summary>
        External,
    }

    enum ObjCHeaderNameUse
    {
        /// <summary>
        /// Normal header name usage, as Header.h
        /// </summary>
        Normal,
        /// <summary>
        /// Header relative first include usage, as "Header.h"
        /// </summary>
        IncludeRelativeFirst,
        /// <summary>
        /// Header global first include usage, as &lt;Header.h&gt;
        /// </summary>
        IncludeGlobalFirst
    }

    enum ObjCSymbolUsage
    {
        /// <summary>
        /// Normal symbol usage (eg. invocation, assignment, access..)
        /// </summary>
        Normal,
        /// <summary>
        /// Symbol declaration (field, method signature..)
        /// </summary>
        Declaration
    }
}
