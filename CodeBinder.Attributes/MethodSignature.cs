using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    public class MethodSignature
    {
        public bool HasExplictParamNames;
        public string Name;
        public ParameterData ReturnType;
        public ParameterData[] Parameters;

        public static MethodSignature Create()
        {
            var ret = new MethodSignature();
            ret.HasExplictParamNames = true;
            ret.Parameters = new ParameterData[0];
            return ret;
        }

        public static MethodSignature Create(string methodName, object returnType, Type[] paramTypes)
        {
            return create(methodName, returnType, false, getParameters(paramTypes));
        }

        public static MethodSignature Create(string methodName, object returnType, object[] paramObjects)
        {
            return create(methodName, returnType, true, getParameters(paramObjects));
        }

        static MethodSignature create(string methodName, object returnType, bool hasExplictParamNames, ParameterData[] parameters)
        {
            var ret = new MethodSignature();
            ret.Name = methodName;
            ret.HasExplictParamNames = hasExplictParamNames;
            ret.ReturnType = new ParameterData(returnType, null);
            ret.Parameters = parameters;
            return ret;
        }

        static ParameterData[] getParameters(object[] paramObjects)
        {
            if (paramObjects.Length % 2 != 0)
                    throw new Exception("Object count with parameters name must be divisible by two");

            int paramCount = paramObjects.Length / 2;
            var parameters = new ParameterData[paramCount];
            for (int i = 0; i < paramCount; i++)
                parameters[i] = new ParameterData(paramObjects[i * 2], paramObjects[i * 2 + 1].ToString());

            return parameters;
        }

        static ParameterData[] getParameters(Type[] paramTypes)
        {
            var parameters = new ParameterData[paramTypes.Length];
            for (int i = 0; i < paramTypes.Length; i++)
                parameters[i] = new ParameterData(paramTypes[i], "param" + (i + 1));

            return parameters;
        }
    }

    public struct ParameterData
    {
        Type _Type;
        string _TypeName;

        public string Name;

        public ParameterData(object typeObj, string name)
        {
            if (typeObj == null)
            {
                _Type = null;
                _TypeName = null;
            }
            else
            {
                var type = typeObj as Type;
                if (type == null)
                {
                    _TypeName = typeObj.ToString();
                    _Type = null;
                }
                else
                {
                    _Type = type;
                    _TypeName = null;
                }
            }

            Name = name;
        }

        public ParameterData(Type type, string name)
        {
            _Type = type;
            _TypeName = null;
            Name = name;
        }

        public Type Type
        {
            get
            {
                if (_TypeName != null)
                    throw new Exception("Explicit type name set");
                else if (_Type != null)
                    return _Type;
                else
                    return typeof(void);
            }
            set
            {
                _Type = value;
                _TypeName = null;
            }
        }
        public string TypeName
        {
            get
            {
                if (_TypeName != null)
                    return _TypeName;
                else if (_Type != null)
                    return _Type.FullName;
                else
                    return typeof(void).FullName;
            }
            set
            {
                _TypeName = value;
                _Type = null;
            }
        }

        public bool HasExplicitTypeName
        {
            get { return _TypeName != null; }
        }
    }
}
