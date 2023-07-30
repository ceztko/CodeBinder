// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using System.Collections.Specialized;

namespace CodeBinder.Shared;

/// <summary>
/// Validation context that's used to validate a compilation
/// </summary>
/// <remarks>Use this class for a generic compilation</remarks>
public abstract class ValidationContext<TVisitor> : ValidationContext
    where TVisitor : NodeVisitor
{
    public new event Action<TVisitor>? Init;

    protected ValidationContext() { }

    internal sealed override void OnInit(NodeVisitor visitor)
    {
        Init?.Invoke((TVisitor)visitor);
    }
}

/// <summary>
/// Validation context that's used to validate a compilation
/// </summary>
/// <remarks>This class is for infrastructure only</remarks>
public abstract class ValidationContext : ICompilationProvider, IReadOnlyDictionary<SyntaxTree, IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>>
{
    List<string> _Errors;

    Dictionary<SyntaxTree, ReplacementDictionary> _Replacements;

    /// <summary>
    /// The compilation provider
    /// </summary>
    /// <remarks>The compilation instance is not available during construction. Use the Init event</remarks>
    public CompilationProvider Compilation { get; internal set; } = null!;

    internal ValidationContext()
    {
        _Errors = new List<string>();
        _Replacements = new Dictionary<SyntaxTree, ReplacementDictionary>();
    }

    protected void Unsupported(SyntaxNode node, string? message = null)
    {
        if (message == null)
            addError(string.Format("Unsupported node: {0}", node));
        else
            addError(string.Format("Unsupported node: {0}, {1}", node, message));
    }

    /// <summary>
    /// Push a node replacement action
    /// </summary>
    public void PushReplacement<TNode>(TNode nodeToReplace, NodeReplacementAction<TNode> replacementAction)
        where TNode : SyntaxNode
    {
        if (!_Replacements.TryGetValue(nodeToReplace.SyntaxTree, out var nodeReplacements))
        {
            nodeReplacements = new ReplacementDictionary();
            _Replacements[nodeToReplace.SyntaxTree] = nodeReplacements;
        }

        if (!nodeReplacements.TryGetValue(nodeToReplace, out var replacements))
        {
            replacements = new List<NodeReplacementAction>();
            nodeReplacements[nodeToReplace] = replacements;
        }

        replacements.Add((node, options) => replacementAction((TNode)node, options));
    }

    void addError(string error)
    {
        _Errors.Add(error);
    }

    internal void Init(CompilationProvider compilation, NodeVisitor visitor)
    {
        Compilation = compilation;
        OnInit(visitor);
    }

    internal abstract void OnInit(NodeVisitor visitor);

    /// <summary>
    /// The language conversion
    /// </summary>
    /// <remarks>It may not be avaiable during construction. Use the Init event</remarks>
    public abstract LanguageConversion Conversion { get; }

    public IReadOnlyList<string> Errors => _Errors;

    public IReadOnlyDictionary<SyntaxTree, IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>> Replacements => this;

    bool IReadOnlyDictionary<SyntaxTree, IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>>.ContainsKey(SyntaxTree key)
    {
        return _Replacements.ContainsKey(key);
    }

    bool IReadOnlyDictionary<SyntaxTree, IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>>.TryGetValue(
        SyntaxTree key, [NotNullWhen(true)] out IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>? value)
    {
        if (_Replacements.TryGetValue(key, out var value_))
        {
            value = value_;
            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }

    int IReadOnlyCollection<KeyValuePair<SyntaxTree, IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>>>.Count => _Replacements.Count;

    IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>> IReadOnlyDictionary<SyntaxTree, IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>>.this[SyntaxTree key]
    => _Replacements[key];

    IEnumerable<SyntaxTree> IReadOnlyDictionary<SyntaxTree, IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>>.Keys => _Replacements.Keys;

    IEnumerable<IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>> IReadOnlyDictionary<SyntaxTree, IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>>.Values => _Replacements.Values;

    IEnumerator<KeyValuePair<SyntaxTree, IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>>> IEnumerable<KeyValuePair<SyntaxTree, IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>>>.GetEnumerator()
    {
        return getReadOnlyPairs();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return getReadOnlyPairs();
    }

    IEnumerator<KeyValuePair<SyntaxTree, IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>>> getReadOnlyPairs()
    {
        foreach (var pair in _Replacements)
            yield return KeyValuePair.Create(pair.Key, (IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>)pair.Value);
    }

    // Wrap a OrderedDictionary so reaplacement actions will be ordered
    class ReplacementDictionary : IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>
    {
        OrderedDictionary _dictionary;

        public ReplacementDictionary()
        {
            _dictionary = new OrderedDictionary();
        }

        public int Count => _dictionary.Count;

        public bool ContainsKey(SyntaxNode key)
        {
            return _dictionary.Contains(key);
        }

        public bool TryGetValue(SyntaxNode key, [MaybeNullWhen(false)] out List<NodeReplacementAction> value)
        {
            if (_dictionary.Contains(key))
            {
                value = (List<NodeReplacementAction>)_dictionary[key]!;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public List<NodeReplacementAction> this[SyntaxNode key]
        {
            get { return (List<NodeReplacementAction>)_dictionary[key]!; }
            set { _dictionary[key] = value; }
        }

        IReadOnlyList<NodeReplacementAction> IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>.this[SyntaxNode key] => this[key];

        IEnumerable<SyntaxNode> IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>.Keys
        {
            get
            {
                foreach (var key in _dictionary.Keys)
                    yield return (SyntaxNode)key;
            }
        }

        IEnumerable<IReadOnlyList<NodeReplacementAction>> IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>.Values
        {
            get
            {
                foreach (var value in _dictionary.Values)
                    yield return (IReadOnlyList<NodeReplacementAction>)value;
            }
        }

        bool IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>.TryGetValue(SyntaxNode key, [MaybeNullWhen(false)]out IReadOnlyList<NodeReplacementAction> value)
        {
            if (_dictionary.Contains(key))
            {
                value = (IReadOnlyList<NodeReplacementAction>)_dictionary[key]!;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        IEnumerator<KeyValuePair<SyntaxNode, IReadOnlyList<NodeReplacementAction>>> IEnumerable<KeyValuePair<SyntaxNode, IReadOnlyList<NodeReplacementAction>>>.GetEnumerator()
        {
            return getReadOnlyPairs();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return getReadOnlyPairs();
        }

        IEnumerator<KeyValuePair<SyntaxNode, IReadOnlyList<NodeReplacementAction>>> getReadOnlyPairs()
        {
            var enumerator = _dictionary.GetEnumerator();
            while (enumerator.MoveNext())
                yield return new KeyValuePair<SyntaxNode, IReadOnlyList<NodeReplacementAction>>(
                    (SyntaxNode)enumerator.Key, (IReadOnlyList<NodeReplacementAction>)enumerator.Value!);
        }
    }
}

public class ReplacementOptions
{
    public bool SkipNormalization { get; set; }
}

public delegate SyntaxNode NodeReplacementAction(SyntaxNode nodeToReplace, ReplacementOptions options);

public delegate SyntaxNode NodeReplacementAction<TNode>(TNode nodeToReplace, ReplacementOptions options)
    where TNode : SyntaxNode;
