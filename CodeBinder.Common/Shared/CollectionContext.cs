// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Shared;

/// <summary>
/// Context class that is used when collecting entities for a compilation context
/// </summary>
/// <remarks>Inherit this class if you need to collect a generic compilation context</remarks>
public abstract class CollectionContext<TVisitor> : CollectionContext
    where TVisitor : NodeVisitor
{
    protected CollectionContext() { }

    public new event Action<TVisitor>? Init;

    internal override void OnInit(NodeVisitor visitor)
    {
        Init?.Invoke((TVisitor)visitor);
    }
}

/// <summary>
/// Context class that is used when collecting entities for a compilation context
/// </summary>
/// <remarks>This class is for infrastructure only</remarks>
public abstract class CollectionContext : ICompilationProvider
{
    internal CollectionContext() { }

    internal void Init(NodeVisitor visitor)
    {
        OnInit(visitor);
    }

    internal abstract void OnInit(NodeVisitor visitor);


    /// <summary>
    /// The compilatio context
    /// </summary>
    /// <remarks>It may not be avaiable during construction. Use the Init event</remarks>
    public abstract CompilationContext Compilation { get; }

    CompilationProvider ICompilationProvider.Compilation => Compilation;
}
