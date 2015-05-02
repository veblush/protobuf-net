#if !NO_RUNTIME
using System;
#if FEAT_IKVM
using Type = IKVM.Reflection.Type;
using IKVM.Reflection;
#else
using System.Reflection;
#endif

namespace ProtoBuf.Meta
{
    internal abstract class AttributeMap
    {
#if DEBUG
        [Obsolete("Please use AttributeType instead")]
        new public Type GetType() { return AttributeType; }
#endif
        public abstract bool TryGet(string key, bool publicOnly, out object value);
        public bool TryGet(string key, out object value)
        {
            return TryGet(key, true, out value);
        }
        public abstract Type AttributeType { get; }
        public static AttributeMap[] Create(TypeModel model, Type type, bool inherit)
        {
#if FEAT_IKVM
            Type attribType = model.MapType(typeof(System.Attribute));
            System.Collections.Generic.IList<CustomAttributeData> all = type.__GetCustomAttributes(attribType, inherit);
            AttributeMap[] result = new AttributeMap[all.Count];
            int index = 0;
            foreach (CustomAttributeData attrib in all)
            {
                result[index++] = new AttributeDataMap(attrib);
            }
            return result;
#elif DNXCORE50
            Type attribType = model.MapType(typeof(System.Attribute));
            System.Collections.Generic.IList<CustomAttributeData> all = System.Linq.Enumerable.ToList(type.GetTypeInfo().CustomAttributes);
            AttributeMap[] result = new AttributeMap[all.Count];
            int index = 0;
            foreach (CustomAttributeData attrib in all)
            {
                result[index++] = new AttributeDataMap(attrib);
            }
            return result;
#else
#if WINRT
            Attribute[] all = System.Linq.Enumerable.ToArray(type.GetTypeInfo().GetCustomAttributes(inherit));
#else
            object[] all = type.GetCustomAttributes(inherit);
#endif
            AttributeMap[] result = new AttributeMap[all.Length];
            for(int i = 0 ; i < all.Length ; i++)
            {
                result[i] = new ReflectionAttributeMap((Attribute)all[i]);
            }
            return result;
#endif
        }

        public static AttributeMap[] Create(TypeModel model, MemberInfo member, bool inherit)
        {
#if FEAT_IKVM
            System.Collections.Generic.IList<CustomAttributeData> all = member.__GetCustomAttributes(model.MapType(typeof(Attribute)), inherit);
            AttributeMap[] result = new AttributeMap[all.Count];
            int index = 0;
            foreach (CustomAttributeData attrib in all)
            {
                result[index++] = new AttributeDataMap(attrib);
            }
            return result;
#elif DNXCORE50
            System.Collections.Generic.IList<CustomAttributeData> all = System.Linq.Enumerable.ToList(member.CustomAttributes);
            AttributeMap[] result = new AttributeMap[all.Count];
            int index = 0;
            foreach (CustomAttributeData attrib in all)
            {
                result[index++] = new AttributeDataMap(attrib);
            }
            return result;
#else
#if WINRT
            Attribute[] all = System.Linq.Enumerable.ToArray(member.GetCustomAttributes(inherit));
#else
            object[] all = member.GetCustomAttributes(inherit);
#endif
            AttributeMap[] result = new AttributeMap[all.Length];
            for(int i = 0 ; i < all.Length ; i++)
            {
                result[i] = new ReflectionAttributeMap((Attribute)all[i]);
            }
            return result;
#endif
        }
        public static AttributeMap[] Create(TypeModel model, Assembly assembly)
        {

#if FEAT_IKVM
            const bool inherit = false;
            System.Collections.Generic.IList<CustomAttributeData> all = assembly.__GetCustomAttributes(model.MapType(typeof(Attribute)), inherit);
            AttributeMap[] result = new AttributeMap[all.Count];
            int index = 0;
            foreach (CustomAttributeData attrib in all)
            {
                result[index++] = new AttributeDataMap(attrib);
            }
            return result;
#elif DNXCORE50
            System.Collections.Generic.IList<CustomAttributeData> all = System.Linq.Enumerable.ToList(assembly.CustomAttributes);
            AttributeMap[] result = new AttributeMap[all.Count];
            int index = 0;
            foreach (CustomAttributeData attrib in all)
            {
                result[index++] = new AttributeDataMap(attrib);
            }
            return result;
#else
#if WINRT
            Attribute[] all = System.Linq.Enumerable.ToArray(assembly.GetCustomAttributes());
#else
            const bool inherit = false;
            object[] all = assembly.GetCustomAttributes(inherit);
#endif
            AttributeMap[] result = new AttributeMap[all.Length];
            for(int i = 0 ; i < all.Length ; i++)
            {
                result[i] = new ReflectionAttributeMap((Attribute)all[i]);
            }
            return result;
#endif
        }

#if FEAT_IKVM || DNXCORE50
        private sealed class AttributeDataMap : AttributeMap
        {
            public override Type AttributeType
            {
#if DNXCORE50
                get { return attribute.AttributeType; }
#else
                get { return attribute.Constructor.DeclaringType; }
#endif
            }
            private readonly CustomAttributeData attribute;
            public AttributeDataMap(CustomAttributeData attribute)
            {
                this.attribute = attribute;
            }
            public override bool TryGet(string key, bool publicOnly, out object value)
            {
                foreach (CustomAttributeNamedArgument arg in attribute.NamedArguments)
                {
#if DNXCORE50
                    string name = arg.MemberName;
#else
                    string name = arg.MemberInfo.Name;
#endif
                    if (string.Equals(name, key, StringComparison.OrdinalIgnoreCase))
                    {
                        value = arg.TypedValue.Value;
                        return true;
                    }
                }



#if DNXCORE50
                // CoreCLR doesn't expose the parameter names / infos; need to look up the ctor
                System.Collections.Generic.IList<CustomAttributeTypedArgument> ctorArgs = attribute.ConstructorArguments;
                Type[] types = new Type[ctorArgs.Count];
                for(int i = 0; i < types.Length; i++)
                {
                    types[i] = ctorArgs[i].ArgumentType;
                }
                ParameterInfo[] parameters = attribute.AttributeType.GetConstructor(types).GetParameters();
#else
                ParameterInfo[] parameters = attribute.Constructor.GetParameters();
#endif
                int index = 0;
                foreach (CustomAttributeTypedArgument arg in attribute.ConstructorArguments)
                {
                    if (string.Equals(parameters[index++].Name, key, StringComparison.OrdinalIgnoreCase))
                    {
                        value = arg.Value;
                        return true;
                    }
                }
                value = null;
                return false;
            }
        }
#else
        public abstract object Target { get; }

        private sealed class ReflectionAttributeMap : AttributeMap
        {
            public override object Target
            {
                get { return attribute; }
            }
            public override Type AttributeType
            {
                get { return attribute.GetType(); }
            }
            public override bool TryGet(string key, bool publicOnly, out object value)
            {
                MemberInfo[] members = Helpers.GetInstanceFieldsAndProperties(attribute.GetType(), publicOnly);
                foreach (MemberInfo member in members)
                {
#if FX11
                    if (member.Name.ToUpper() == key.ToUpper())
#else
                    if (string.Equals(member.Name, key, StringComparison.OrdinalIgnoreCase))
#endif
                    {
                        PropertyInfo prop = member as PropertyInfo;
                        if (prop != null) {
                            value = prop.GetValue(attribute, null);
                            return true;
                        }
                        FieldInfo field = member as FieldInfo;
                        if (field != null) {
                            value = field.GetValue(attribute);
                            return true;
                        }

                        throw new NotSupportedException(member.GetType().Name);
                    }
                }
                value = null;
                return false;
            }
            private readonly Attribute attribute;
            public ReflectionAttributeMap(Attribute attribute)
            {
                this.attribute = attribute;
            }
        }
#endif
            }
}
#endif