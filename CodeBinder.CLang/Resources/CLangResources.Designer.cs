﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CodeBinder.CLang {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CLangResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CLangResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CodeBinder.CLang.Resources.CLangResources", typeof(CLangResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #ifndef CODE_BINDER_BASE_TYPES
        ///#define CODE_BINDER_BASE_TYPES
        ///#pragma once
        ///
        ///#ifdef __cplusplus
        ///#include &lt;cstdint&gt;
        ///#include &lt;cstdlib&gt;
        ///#include &lt;climits&gt;
        ///#else // __cplusplus
        ///#include &lt;stdint.h&gt;
        ///#include &lt;stdlib.h&gt;
        ///#include &lt;limits.h&gt;
        ///#endif // __cplusplus
        ///
        ///#ifdef __APPLE__
        ///#include &lt;objc/objc.h&gt;
        ///#endif
        ///
        ///#if defined(__cplusplus) &amp;&amp; defined(_MSC_VER)
        ///// In MSVC bool is guaranteed to be 1 byte, with true == 1 and false == 0
        ///typedef bool cbbool;
        ///#elif defined(__APPLE__)
        ///typedef BOOL cbbool; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CBBaseTypes_h {
            get {
                return ResourceManager.GetString("CBBaseTypes_h", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #ifndef CODE_BINDER_INTEROP_HEADER
        ///#define CODE_BINDER_INTEROP_HEADER
        ///#pragma once
        ///
        ///#include &quot;CBBaseTypes.h&quot;
        ///
        ///#ifdef __cplusplus
        ///#include &lt;cstring&gt;
        ///#else // __cplusplus
        ///#include &lt;string.h&gt;
        ///#endif // __cplusplus
        ///
        ///#if UINTPTR_MAX == UINT32_MAX
        ///#define CB_STRING_OWNSDATA_FLAG (1u &lt;&lt; 31)
        ///#elif UINTPTR_MAX == UINT64_MAX
        ///#define CB_STRING_OWNSDATA_FLAG (1ull &lt;&lt; 63)
        ///#else
        ///#error &quot;Environment not 32 or 64-bit.&quot;
        ///#endif
        ///
        ///#define CBSLEN(str) (size_t)((str).opaque &amp; ~CB_STRING_OWNSDATA_FLAG)
        ///
        ///#ifd [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CBInterop_h {
            get {
                return ResourceManager.GetString("CBInterop_h", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #ifndef CODE_BINDER_INTEROP_CPP_HEADER
        ///#define CODE_BINDER_INTEROP_CPP_HEADER
        ///#pragma once
        ///
        ///#include &quot;CBInterop.h&quot;
        ///#include &lt;cstring&gt;
        ///#include &lt;string&gt;
        ///#include &lt;string_view&gt;
        ///#include &lt;utility&gt;
        ///#include &lt;new&gt;
        ///#include &lt;stdexcept&gt;
        ///
        ///namespace cb
        ///{
        ///    /// &lt;summary&gt;
        ///    /// This exception can be used just to unwind the stack,
        ///    /// for example in Java interop scenario when returning
        ///    /// from callbacks. It should be catched in outer C functions
        ///    /// and just return from them
        ///    ///  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CBInterop_hpp {
            get {
                return ResourceManager.GetString("CBInterop_hpp", resourceCulture);
            }
        }
    }
}
