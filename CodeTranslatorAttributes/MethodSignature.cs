using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    public class MethodSignature
    {
        public bool HasExplictParamsName;
        public string Name;
        public ParameterData ReturnParam;
        public ParameterData[] Parameters;

        /// <param name="hasExplictParamsName"></param>
        public MethodSignature CreateFromParamStrings(string methodName, string returnType, IReadOnlyList<string> paramStrings, bool hasExplictParamsName = true)
        {
            var ret = new MethodSignature();
            ret.Name = methodName;
            ret.HasExplictParamsName = hasExplictParamsName;
            ret.ReturnParam.TypeName = returnType;

            var parameters = new List<ParameterData>();
            foreach (var parameter in getParameters(paramStrings, hasExplictParamsName))
                parameters.Add(new ParameterData() { Name = parameter.Name, TypeName = parameter.TypeName });

            ret.Parameters = parameters.ToArray();
            return ret;
        }

        IEnumerable<(string TypeName, string Name)> getParameters(IReadOnlyList<string> paramStrings, bool hasExplictParamsName)
        {
            if (hasExplictParamsName)
            {
                if (paramStrings.Count % 2 != 0)
                    throw new Exception("String count with parameters name must be divisible by two");

                int paramCount = paramStrings.Count / 2;
                for (int i = 0; i < paramCount; i++)
                    yield return (paramStrings[i * 2], paramStrings[i * 2 + 1]);
            }
            else
            {
                for (int i = 0; i < paramStrings.Count; i++)
                    yield return (paramStrings[i], "param" + (i + 1));
            }
        }
    }

    public struct ParameterData
    {
        Type _Type;
        string _TypeName;

        public string Name;

        public void Set(object typeObj, string name)
        {
            if (typeObj == null)
            {
                Type = null;
            }
            else
            {
                var type = typeObj as Type;
                if (type == null)
                    TypeName = typeObj.ToString();
                else
                    Type = type;
            }

            Name = name;
        }

        public void Set(string type, string name)
        {
            TypeName = type;
            Name = name;
        }

        public void Set(Type type, string name)
        {
            Type = type;
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
