// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeBinder
{
    public class Solution
    {
        public IReadOnlyList<Project> Projects { get; private set; }

        public string FilePath { get; private set; }

        public string Name { get; private set; }

        private Solution(string filepath, string name, IReadOnlyList<Project> projects)
        {
            FilePath = filepath;
            Name = name;
            Projects = projects;
            foreach (var project in Projects)
                project.Solution = this;
        }

        public static Solution Open(string filepath)
        {
            var name = Path.GetFileNameWithoutExtension(filepath);
            var solution = SolutionFile.Parse(filepath);
            var projects = new List<Project>();
            foreach (var project in solution.ProjectsInOrder)
                projects.Add(new Project(project.AbsolutePath, project.ProjectName, Guid.Parse(project.ProjectGuid)));
            return new Solution(filepath, name, projects);
        }
    }
}
