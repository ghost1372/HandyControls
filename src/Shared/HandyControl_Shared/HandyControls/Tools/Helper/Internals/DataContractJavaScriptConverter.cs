#if !(NETCOREAPP || NET40)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace HandyControl.Tools
{
    internal class DataContractJavaScriptConverter : JavaScriptConverter
    {
        private static readonly List<Type> _supportedTypes = new List<Type>();
        static DataContractJavaScriptConverter()
        {
            foreach (Type type in Assembly.GetEntryAssembly().DefinedTypes)
            {
                if (Attribute.IsDefined(type, typeof(DataContractAttribute)))
                {
                    _supportedTypes.Add(type);
                }
            }
        }

        private bool ConvertEnumToString = false;
        public DataContractJavaScriptConverter() : this(false)
        {
        }
        public DataContractJavaScriptConverter(bool convertEnumToString)
        {
            ConvertEnumToString = convertEnumToString;
        }
        public override IEnumerable<Type> SupportedTypes
        {
            get { return _supportedTypes; }
        }
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (Attribute.IsDefined(type, typeof(DataContractAttribute)))
            {
                try
                {
                    object instance = Activator.CreateInstance(type);

                    IEnumerable<MemberInfo> members = ((IEnumerable<MemberInfo>) type.GetFields())
                        .Concat(type.GetProperties().Where(property => property.CanWrite && property.GetIndexParameters().Length == 0))
                        .Where((member) => Attribute.IsDefined(member, typeof(DataMemberAttribute)));
                    foreach (MemberInfo member in members)
                    {
                        DataMemberAttribute attribute = (DataMemberAttribute) Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute));
                        object value;
                        if (dictionary.TryGetValue(attribute.Name, out value) == false)
                        {
                            if (attribute.IsRequired)
                            {
                                throw new SerializationException(String.Format("Required DataMember with name {0} not found", attribute.Name));
                            }
                            continue;
                        }
                        if (member.MemberType == MemberTypes.Field)
                        {
                            FieldInfo field = (FieldInfo) member;
                            object fieldValue;
                            if (ConvertEnumToString && field.FieldType.IsEnum)
                            {
                                fieldValue = Enum.Parse(field.FieldType, value.ToString());
                            }
                            else
                            {
                                fieldValue = serializer.ConvertToType(value, field.FieldType);
                            }
                            field.SetValue(instance, fieldValue);
                        }
                        else if (member.MemberType == MemberTypes.Property)
                        {
                            PropertyInfo property = (PropertyInfo) member;
                            object propertyValue;
                            if (ConvertEnumToString && property.PropertyType.IsEnum)
                            {
                                propertyValue = Enum.Parse(property.PropertyType, value.ToString());
                            }
                            else
                            {
                                propertyValue = serializer.ConvertToType(value, property.PropertyType);
                            }
                            property.SetValue(instance, propertyValue);
                        }
                    }
                    return instance;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
