﻿// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Shared;

public delegate void BeforeNodeVisitDelegate(NodeVisitor visitor, SyntaxNode node, NodeVisitorToken token);

/// <summary>
/// A class that visit syntax trees of a specific language
/// </summary>
public abstract class NodeVisitor
{
    protected NodeVisitor() { }

    public event Action<NodeVisitor>? Visited;

    /// <summary>
    /// Called after actual node visit. It is reset
    /// before visit, so registration must happen
    /// inside any event
    /// </summary>
    public event Action<NodeVisitor, SyntaxNode>? AfterNodeVisit;

    /// <summary>
    /// Called before every node visit. It can cancel the actual visit
    /// </summary>
    public event BeforeNodeVisitDelegate? BeforeNodeVisit;

    public void Visit(IEnumerable<SyntaxTree> trees)
    {
        foreach (var tree in trees)
            VisitTree(tree);

        Visited?.Invoke(this);
    }

    protected void OnBeforeNodeVisit(SyntaxNode node, NodeVisitorToken token)
    {
        // Early reset after node visit event
        AfterNodeVisit = null;
        var evnt = BeforeNodeVisit;
        if (evnt != null)
        {
            // Manually invoke registered delegates so we can block
            // further processing on the first if visiting is canceled
            var delegates = evnt.GetInvocationList();
            for (int i = 0; i < delegates.Length; i++)
            {
                var dlg = (BeforeNodeVisitDelegate)delegates[i];
                dlg.Invoke(this, node, token);
                if (token.IsCanceled)
                    break;
            }
        }
    }

    protected void OnAfterNodeVisit(SyntaxNode node)
    {
        AfterNodeVisit?.Invoke(this, node);
    }

    protected abstract void VisitTree(SyntaxTree tree);
}

public class NodeVisitorToken
{
    public bool IsCanceled { get; private set; }

    public void Cancel()
    {
        IsCanceled = true;
    }
}
