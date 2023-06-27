// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Shared;

// CHECK-ME Evaluate create a base NodeVisitor class
public interface INodeVisitor
{
    void Visit(SyntaxTree context);

    void AfterVisit();

    IReadOnlyList<string> Errors
    {
        get;
    }
}
