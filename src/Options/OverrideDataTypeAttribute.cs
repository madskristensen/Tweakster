using System;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.Settings;

namespace Tweakster
{

    /// <summary>   Enumeration that specifies the underlying type that is to be stored/retrieved from the
    ///             <see cref="WritableSettingsStore"/>.  </summary>
    internal enum SettingDataType
    {
        /// <summary>   Uses the <see cref="WritableSettingsStore"/>.<see cref="WritableSettingsStore.SetString"/>
        /// to update the value of the attributed property, using an underlying string type in the settings store. 
        /// The raw value of the property is first serialized using the <see cref="BinaryFormatter"/> then converted 
        /// to a Base64 string for storage. </summary>
        Serialized,
        /// <summary>   Uses the <see cref="WritableSettingsStore"/>.<see cref="WritableSettingsStore.SetBoolean"/>
        /// to update the value of the attributed <see cref="Boolean"/> property, using an underlying Int32 type 
        /// in the settings store. </summary>
        Bool,
    }

    /// <summary>   Apply this attribute on a get/set property in the <see cref="BaseOptionModel{T}"/> class to 
    ///             alter the default mechanism used to store/retrieve the value of this property from the
    ///             <see cref="WritableSettingsStore"/>. By default, the <see cref="SettingDataType"/><c>.Serialized</c>
    ///             mechanism is used if this attribute is not applied.  </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    internal class OverrideDataTypeAttribute : Attribute
    {
        /// <summary>   Alters the default mechanism used to store/retrieve the value of this property from the setting store. </summary>
        /// <param name="settingDataType">  Specifies the type and/or method the value of the attributed property is
        ///             serialized and deserialized in the <see cref="WritableSettingsStore"/>. </param>
        public OverrideDataTypeAttribute(SettingDataType settingDataType)
        {
            SettingDataType = settingDataType;
        }

        /// <summary>   Specifies the type and/or method the value of the attributed property is
        ///             serialized and deserialized in the <see cref="WritableSettingsStore"/>. </summary>
        public SettingDataType SettingDataType { get; }
    }
}