﻿using System;
using System.Drawing;
using System.Globalization;
using System.Reflection;

namespace pdfforge.PDFCreator.COM
{
    internal class ValueReflector
    {
        private const BindingFlags RequiredFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.SetProperty;
        private static readonly IFormatProvider InvariantFormatProvider = CultureInfo.InvariantCulture.NumberFormat;

        public static bool HasProperty(Object parent, string propertyName)
        {
            return FindProperty(parent, propertyName) != null;
        }

        public static bool SetPropertyValue(Object parent, string propertyName, string value)
        {
            var pi = FindProperty(parent, propertyName);

            if (pi == null)
                throw new ArgumentException("propertyName");

            SetProperty(pi.Parent, pi.Property, value);
            return true;
        }

        private static PropertyWrapper FindProperty(Object parent, string propertyName)
        {
            Type t = parent.GetType();

            string itemRemainder = null;
            int splitPos = propertyName.IndexOf('.');
            if (splitPos >= 0)
            {
                itemRemainder = propertyName.Substring(splitPos + 1);
                propertyName = propertyName.Substring(0, splitPos);
            }

            MemberInfo[] mi = t.GetMember(propertyName, RequiredFlags);

            if (mi.Length == 0)
            {
                return null;
            }

            switch (mi[0].MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo pi = t.GetProperty(propertyName, RequiredFlags);
                    if (!pi.CanRead)
                        return null;

                    if (itemRemainder != null)
                    {
                        MethodInfo m = pi.GetGetMethod();
                        Object newParent = m.Invoke(parent, new object[] { });
                        return FindProperty(newParent, itemRemainder);
                    }
                    return new PropertyWrapper(parent, parent.GetType().GetProperty(propertyName, RequiredFlags));

                default:
                    throw new Exception("MemberType " + mi[0].MemberType + " not supported in " + t + " for Member " + propertyName);
            }
        }

        private static void SetProperty(object parent, PropertyInfo property, String value)
        {
            object v;

            if (property.PropertyType.IsEnum)
            {
                v = ConvertEnum(value, property.PropertyType);
            }
            else
            {
                v = ConvertValue(value, property.PropertyType);
            }

            property.SetValue(parent, v, null);
        }

        private static object ConvertEnum(string value, Type type)
        {
            if (!Enum.IsDefined(type, value))
            {
                throw new ArgumentException("value");
            }

            return Enum.Parse(type, value);
        }

        private static object ConvertValue(string value, Type type)
        {
            if (type == typeof(Color))
            {
                return ColorTranslator.FromHtml(value);
            }
            if (type == typeof(Version))
            {
                return Version.Parse(value);
            }
            return Convert.ChangeType(value, type, InvariantFormatProvider);
        }

        public static string GetPropertyValue(Object parent, string propertyName)
        {
            var pInfo = FindProperty(parent, propertyName);

            if(pInfo == null)
                throw new ArgumentException("Property not found.");

            return pInfo.Property.GetValue(pInfo.Parent, null).ToString();
        }
    }

    internal class PropertyWrapper
    {
        public object Parent { get; set; }
        public PropertyInfo Property { get; set; }

        public PropertyWrapper(object parent, PropertyInfo property)
        {
            Parent = parent;
            Property = property;
        }
    }
}
