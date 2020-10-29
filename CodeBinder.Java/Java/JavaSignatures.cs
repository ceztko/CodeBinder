/*
 *  COPYRIGHT NOTICE  
 *  Copyright (C) 2014, ticktick <lujun.hust@gmail.com>
 *  http://ticktick.blog.51cto.com/
 *   
 *  @license under the Apache License, Version 2.0 
 *
 *  @file    SignatureGen.java
 *  @brief   Implement a java class for jni signature generate
 *  
 *  @version 1.0     
 *  @author  ticktick
 *  @date    2014/12/15
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java.Extras
{
    public static class JavaSignatures
    {
        public static Dictionary<Type, string> Primitives = new Dictionary<Type, string>();

        static JavaSignatures()
        {
            Primitives.Add(typeof(void), "V");
            Primitives.Add(typeof(bool), "Z");
            Primitives.Add(typeof(byte), "B");
            Primitives.Add(typeof(char), "C");
            Primitives.Add(typeof(short), "S");
            Primitives.Add(typeof(int), "I");
            Primitives.Add(typeof(long), "J");
            Primitives.Add(typeof(float), "F");
            Primitives.Add(typeof(double), "D");
        }

        public static string GetSignature(Type ret, params Type[] parms)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(");
            foreach (var param in parms)
            {
                builder.Append(GetSignature(param));
            }
            builder.Append(")");
            builder.Append(GetSignature(ret));
            return builder.ToString();
        }

        private static string GetSignature(Type param)
        {
            StringBuilder builder = new StringBuilder();
            Type type;
            if (param.IsArray)
            {
                type = param.GetElementType()!;
                builder.Append("[");
            }
            else
            {
                type = param;
            }

            if (Primitives.TryGetValue(type, out var value))
            {
                builder.Append(value);
            }
            else
            {
                builder.Append("L" + type.FullName!.Replace(".", "/") + ";");
            }

            return builder.ToString();
        }
    }
}
