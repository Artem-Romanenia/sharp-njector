﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SharpNjector.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SharpNjector.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to {0}
        ///
        ///namespace SharpNjector
        ///{{
        ///    public class ExpressionEvaluator : System.MarshalByRefObject, SharpNjector.CustomTools.Base.IExpressionEvaluator
        ///    {{
        ///        private readonly System.Func&lt;object&gt;[] _expressions;
        ///
        ///        public ExpressionEvaluator()
        ///        {{
        ///            _expressions = new System.Func&lt;object&gt;[]
        ///            {{
        ///                {1}
        ///            }};
        ///        }}
        ///
        ///        public string this[int i]
        ///        {{
        ///            get
        ///            {{
        ///                return _expressions [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ExpressionEvaluatorFormat {
            get {
                return ResourceManager.GetString("ExpressionEvaluatorFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #nject.
        /// </summary>
        internal static string NjectKeyWord {
            get {
                return ResourceManager.GetString("NjectKeyWord", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #nject-using.
        /// </summary>
        internal static string NjectUsingKeyWord {
            get {
                return ResourceManager.GetString("NjectUsingKeyWord", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to () =&gt; {{ return {0}; }},.
        /// </summary>
        internal static string PropertyFormat {
            get {
                return ResourceManager.GetString("PropertyFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using {0};.
        /// </summary>
        internal static string UsingFormat {
            get {
                return ResourceManager.GetString("UsingFormat", resourceCulture);
            }
        }
    }
}
