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
}
