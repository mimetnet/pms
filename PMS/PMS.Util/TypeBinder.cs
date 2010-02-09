using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PMS.Util
{
    public class TypeBinder : Binder
    {
        protected readonly static DateTime UnixDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public delegate object ChangeTypeDelegate(Object value, Type type);

        public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state)
        {
            throw new NotImplementedException();
        }

        public override object ChangeType(object value, Type type, CultureInfo culture)
        {
            if (null == value)
                return null;

            Type vtype = value.GetType();

            if (type.IsByRef)
                type = type.GetElementType();

            if (vtype == type || type.IsInstanceOfType(value))
                return value;

            if (vtype.IsArray && type.IsArray)
                if (IsArrayAssignable(vtype.GetElementType(), type.GetElementType()))
                    return value;

            ChangeTypeDelegate func = GetTypeChanger(vtype, type);

            if (null == func)
                return null;

            return func(value, type);
        }

        public override void ReorderArgumentArray(ref object[] args, object state)
        {
            throw new NotImplementedException();
        }

        public override MethodBase SelectMethod(BindingFlags flags, MethodBase[] mbase, Type[] types, ParameterModifier[] pMods)
        {
            throw new NotImplementedException();
        }

        public override PropertyInfo SelectProperty(BindingFlags flags, PropertyInfo[] pinfo, Type type, Type[] types, ParameterModifier[] pMods)
        {
            throw new NotImplementedException();
        }

        protected virtual ChangeTypeDelegate GetTypeChanger(Type from, Type to)
        {
            if (from == to)
                return Convert.ChangeType;

            if (null == from)
                return Convert.ChangeType;

            if (to.IsByRef != from.IsByRef)
                return null;

            if (to.IsInterface) {
                if (to.IsAssignableFrom(from))
                    return Convert.ChangeType;
                return null;
            }

            bool ret = false;
            TypeCode fromt = Type.GetTypeCode(from);
            TypeCode tot = Type.GetTypeCode(to);

            switch (fromt) {
                case TypeCode.Char:
                    switch (tot) {
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.Int32:
                        case TypeCode.UInt64:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.String:
                            ret = true; break;
                        default:
                            ret = (to == typeof(object)); break;
                    }
                    break;

                case TypeCode.Byte:
                    switch (tot) {
                        case TypeCode.Char:
                        case TypeCode.UInt16:
                        case TypeCode.Int16:
                        case TypeCode.UInt32:
                        case TypeCode.Int32:
                        case TypeCode.UInt64:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                            ret = true; break;
                        default:
                            ret = (to == typeof(object) || (from.IsEnum && to == typeof(Enum)));
                            break;
                    }
                    break;

                case TypeCode.SByte:
                    switch (tot) {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                            ret = true; break;
                        default:
                            ret = (to == typeof(object) || (from.IsEnum && to == typeof(Enum)));
                            break;
                    }
                    break;

                case TypeCode.UInt16:
                    switch (tot) {
                        case TypeCode.UInt32:
                        case TypeCode.Int32:
                        case TypeCode.UInt64:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.String:
                            ret = true; break;
                        default:
                            ret = (to == typeof(object) || (from.IsEnum && to == typeof(Enum)));
                            break;
                    }
                    break;

                case TypeCode.Int16:
                    switch (tot) {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.String:
                            ret = true; break;
                        default:
                            ret = (to == typeof(object) || (from.IsEnum && to == typeof(Enum)));
                            break;
                    }
                    break;

                case TypeCode.UInt32:
                    switch (tot) {
                        case TypeCode.UInt64:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.String:
                            ret = true; break;
                        default:
                            ret = (to == typeof(object) || (from.IsEnum && to == typeof(Enum)));
                            break;
                    }
                    break;

                case TypeCode.Int32:
                    switch (tot) {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.String:
                            ret = true; break;
                        default:
                            ret = (to == typeof(object) || (from.IsEnum && to == typeof(Enum)));
                            break;
                    }
                    break;

                case TypeCode.UInt64:
                case TypeCode.Int64:
                    switch (tot) {
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.String:
                            ret = true; break;
                        case TypeCode.DateTime:
                            return ConvertLongToDateTime;
                        default:
                            ret = (to == typeof(object) || (from.IsEnum && to == typeof(Enum)));
                            break;
                    }
                    break;

                case TypeCode.Single:
                    ret = (tot == TypeCode.Double || to == typeof(object));
                    break;

                case TypeCode.String:
                    switch (tot) {
                        case TypeCode.DateTime:
                            return ConvertStringToDateTime;

                        default:
                            ret = true; break;
                    }
                    break;

                case TypeCode.DateTime:
                    switch (tot) {
                        case TypeCode.String:
                            ret = true; break;
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                            return ConvertDateTimeToLong;
                    }
                    break;

                default:
                    ret = (to == typeof(object) && from.IsValueType)?
                        true : to.IsAssignableFrom(from);
                    break;
            }

            if (ret)
                return Convert.ChangeType;

            return null;
        }

        protected virtual object ConvertStringToDateTime(object obj, Type type)
        {
            Int64 val = 0;
            String foo = obj as String;

            if (!String.IsNullOrEmpty(foo)) {
                if (Int64.TryParse(foo, out val)) {
                    return ConvertLongToDateTime(val, type);
                } else {
                    try {
                        return new DateTime(DateTime.Parse(foo).Ticks, DateTimeKind.Utc);
                    } catch (Exception) { }
                }
            }

            return DateTime.MinValue;
        }

        protected virtual object ConvertToType(object obj, Type type)
        {
            return Convert.ChangeType(obj, type);
        }

        protected virtual object ConvertLongToDateTime(object obj, Type type)
        {
            Double val = (Double)obj;
            DateTime x = DateTime.MinValue;

            try {
                x = new DateTime(UnixDateTime.AddMilliseconds(val).Ticks, DateTimeKind.Utc);
            } catch (Exception) {}

            return x;
        }

        protected virtual object ConvertDateTimeToLong(object obj, Type type)
        {
            if (typeof(Int64) == type)
                return (Int64)((((DateTime)obj) - UnixDateTime).TotalSeconds * 1000);
            else if (typeof(UInt64) == type)
                return (UInt64)((((DateTime)obj) - UnixDateTime).TotalSeconds * 1000);

            return null;
        }

        private static bool IsArrayAssignable(Type otype, Type ttype)
        {
            if (otype.IsArray && ttype.IsArray)
                return IsArrayAssignable(otype.GetElementType(), ttype.GetElementType());

            return ttype.IsAssignableFrom(otype);
        }
    }
}
