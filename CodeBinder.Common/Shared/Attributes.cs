// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Shared;

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
