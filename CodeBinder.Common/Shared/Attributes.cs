// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    /// <summary>
    /// Used to specify the name of the conversion for the command line tool ("l|language=" switch)
    /// </summary>
    public class ConversionLanguageName : Attribute
    {
        public string Name { get; private set; }
        public ConversionLanguageName(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ConfigurationSwitch : Attribute
    {
        public string Name { get; private set; }

        public string Description { get; private set; }
        public ConfigurationSwitch(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
