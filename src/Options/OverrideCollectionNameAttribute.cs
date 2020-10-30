using System;
using Microsoft.VisualStudio.Settings;

namespace Tweakster
{
    /// <summary>   Apply this attribute on a get/set property in the <see cref="BaseOptionModel{T}"/> class to 
    ///             use the specific <c>CollectionName</c> used to store the value in the 
    ///             <see cref="WritableSettingsStore"/> rather than using the default.  </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    internal class OverrideCollectionNameAttribute : Attribute
    {
        /// <summary>   Specifies the <c>CollectionName</c> in the <see cref="WritableSettingsStore"/> where
        ///             this setting is stored rather than using the default. </summary>
        /// <param name="collectionName">   The <c>CollectionName</c> in the <see cref="WritableSettingsStore"/> that
        ///             contains this value.  </param>
        public OverrideCollectionNameAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }

        /// <summary>   The <c>CollectionName</c> in the <see cref="WritableSettingsStore"/> that 
        ///             contains this value.  </summary>
        public string CollectionName { get; }
    }
}