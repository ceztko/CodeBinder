// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license

using System;
using System.IO;

namespace CodeBinder
{
    public class Project
    {
        public Solution? Solution { get; internal set; }

        public string FilePath { get; private set;}

        public string Name { get; private set; }

        internal Guid? Id { get; private set; }

        internal Project(string filePath, string name, Guid? id)
        {
            FilePath = filePath;
            Name = name;
            Id = id;
        }

        public static Project Open(string filepath)
        {
            var name = Path.GetFileNameWithoutExtension(filepath);
            return new Project(filepath, name, null);
        }
    }
}
