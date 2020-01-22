using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Util
{

    public enum NamespaceNormalization
    {
        None = 0,
        LowerCase
    }

    /// <summary>
    /// Structure where to efficiently store namespace mappings
    /// </summary>
    public class NamespaceMappingTree
    {
        Node m_root;

        public NamespaceMappingTree() { }

        public void PushMapping(string refns, string mappedns)
        {
            var splitted = refns.Split('.');
            if (m_root == null)
                m_root = new Node();

            Node node = m_root;
            for (int i = 0; i < splitted.Length; i++)
                node = node.GetChildren(splitted[i]);

            node.MappedNamespace = mappedns;
        }

        public string GetMappedNamespace(string refns,
            NamespaceNormalization leftOverNorm = NamespaceNormalization.None)
        {
            string ret = GetMappedNamespace(refns, out string leftoverns);
            if (leftoverns == null)
                return ret;
            switch (leftOverNorm)
            {
                case NamespaceNormalization.None:
                    return ret + "." + leftoverns;
                case NamespaceNormalization.LowerCase:
                    return ret + "." + leftoverns.ToLower();
                default:
                    throw new Exception();
            }
        }

        public string GetMappedNamespace(string refns, out string leftoverns)
        {
            var splitted = refns.Split('.');
            if (m_root == null)
                goto Exit;

            Node node = m_root;
            int i = 0;
            for (; i < splitted.Length; i++)
            {
                Node found;
                if (!node.TryGetChildren(splitted[i], out found))
                    break;

                node = found;
            }

            if (i != 0)
            {
                if (node.MappedNamespace == null)
                    leftoverns = refns;
                else
                    leftoverns = getSubNamespace(splitted, i);

                return node.MappedNamespace;
            }

            Exit:
            leftoverns = refns;
            return null;
        }

        string getSubNamespace(string[] splitted, int index, int npos = -1)
        {
            if (npos == 0 || index >= splitted.Length)
                return null;

            var builder = new StringBuilder();
            npos = npos < 0 ? splitted.Length : npos;
            for (int i = index; i < npos; i++)
            {
                if (i != index)
                    builder.Append(".");

                builder.Append(splitted[i]);
            }

            return builder.ToString();
        }

        class Node
        {
            Dictionary<string, Node> m_children;

            public Node()
            {
                Namespace = null;
                FullNamespace = null;
            }

            public Node(string parentns, string nspart)
            {
                Namespace = nspart;
                if (parentns == null)
                    FullNamespace = nspart;
                else
                    FullNamespace = parentns + "." + nspart;
            }

            public bool TryGetChildren(string nspart, out Node node)
            {
                if (m_children == null)
                {
                    node = null;
                    return false;
                }

                return m_children.TryGetValue(nspart, out node);
            }

            public Node GetChildren(string nspart)
            {
                if (m_children == null)
                    m_children = new Dictionary<string, Node>();

                Node ret;
                if (!m_children.TryGetValue(nspart, out ret))
                {
                    ret = new Node(FullNamespace, nspart);
                    m_children[nspart] = ret;
                }

                return ret;
            }

            public string Namespace { get; private set; }
            public string FullNamespace { get; private set; }
            public string MappedNamespace { get; set; }
        }
    }
}
