// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Java;

class JavaEnumWriter : JavaBaseTypeWriter<EnumDeclarationSyntax>
{
    bool _isOrdinalEnum;
    List<EnumMember> m_members;
    List<EnumAlias> m_aliases;

    public JavaEnumWriter(EnumDeclarationSyntax syntax, JavaCodeConversionContext context)
        : base(syntax, context)
    {
        m_members = new List<EnumMember>();
        m_aliases = new List<EnumAlias>();

        var residuals = new List<EnumMember>();
        foreach (var member in syntax.Members)
        {
            long value = member.GetEnumValue(Context);
            string name = member.GetName();
            EnumMember? found;
            if (!((member.EqualsValue is EqualsValueClauseSyntax equalVal)
                && equalVal.Value.IsKind(SyntaxKind.StringLiteralExpression))
                && (found = m_members.Find((m) => m.Value == value)) == null)
            {
                m_members.Add(new EnumMember() { Name = name, Value = value });
            }
            else
            {
                residuals.Add(new EnumMember() { Name = name, Value = value });
            }
        }

        if (Item.IsFlag(Context))
        {
            _isOrdinalEnum = false;

            var allMembers = new SortedDictionary<long, (string Name, bool Generated)>();
            var bitFields = new SortedDictionary<long, string>();
            foreach (var member in m_members)
            {
                allMembers.Add(member.Value, (member.Name, false));
                if (isPowerOfTwo(member.Value))
                    bitFields.Add(member.Value, member.Name);
            }

            // Generate all members with the flag combinations
            var bitPositions = new List<int>();
            foreach (var bitField in bitFields)
            {
                if (bitField.Key == 0)
                {
                    // Ignore zero bitfield
                    continue;
                }    

                int position = findPosition(bitField.Key);
                bitPositions.Add(position);
                if (bitPositions.Count > 1)
                    addGeneratedBitmasks(allMembers, bitPositions);
            }

            // Append all generated members
            foreach (var member in allMembers)
            {
                if (member.Value.Generated)
                    m_members.Add(new EnumMember() { Name = member.Value.Name, Value = member.Key });
            }

            // Create aliases from residual members
            foreach (var residual in residuals)
            {
                if (!allMembers.TryGetValue(residual.Value, out var alias))
                    throw new Exception($"Could not find an alias for residual {residual.Name}");

                m_aliases.Add(new EnumAlias() { Name = residual.Name, Value = alias.Name });
            }
        }
        else
        {
            _isOrdinalEnum = true;
            for (int i = 0; i < m_members.Count; i++)
            {
                var member = m_members[i];
                if (member.Value != i)
                    _isOrdinalEnum = false;
            }

            // Create aliases from residual members
            foreach (var residual in residuals)
            {
                var found = m_members.Find((m) => m.Value == residual.Value);
                if (found == null)
                    throw new Exception($"Could not find an alias for residual {residual.Name}");

                m_aliases.Add(new EnumAlias() { Name = residual.Name, Value = found.Name });
            }
        }
    }

    protected override void WriteTypeMembers()
    {
        WriteEnumMembers();
        Builder.AppendLine();
        Builder.Append("public final int value").EndOfStatement();
        Builder.AppendLine();
        WriteConstructor();
        Builder.AppendLine();
        WriteFromValueMethod();
    }

    private void WriteEnumMembers()
    {
        bool first = true;
        foreach (var member in m_members)
        {
            Builder.CommaAppendLine(ref first);
            WriteMember(member);
        }
        Builder.EndOfStatement();

        foreach (var alias in m_aliases)
        {
            Builder.Append("static final").Space().Append(TypeName).Space().Append(alias.Name)
                .Space().Append("=").Space().Append(alias.Value).EndOfStatement();
        }
    }

    private void WriteMember(EnumMember member)
    {
        Builder.Append(member.Name);
        if (!_isOrdinalEnum)
        {
            Builder.Append("(");
            Builder.Append(member.Value.ToString());
            Builder.Append(")");
        }
    }

    private void WriteConstructor()
    {
        Builder.Append(TypeName);
        if (_isOrdinalEnum)
            Builder.EmptyParameterList();
        else
            Builder.Parenthesized().Append("int value");

        Builder.AppendLine();
        using (Builder.Block())
        {
            if (_isOrdinalEnum)
                Builder.Append("value = this.ordinal()").EndOfStatement();
            else
                Builder.Append("this.value = value").EndOfStatement();
        }
    }

    private void WriteFromValueMethod()
    {
        Builder.Append("public static").Space().Append(TypeName).Space()
            .Append("fromValue(int value)").AppendLine();
        using (Builder.Block())
        {
            if (_isOrdinalEnum)
            {
                Builder.Append("try").AppendLine();
                using (Builder.Block())
                {
                    Builder.Append("return").Space();
                    Builder.Append(TypeName);
                    Builder.Append(".values()[value]").EndOfStatement();
                }
                Builder.Append("catch (Exception e)").AppendLine();
                using (Builder.Block())
                {
                    Builder.Append("throw new RuntimeException(\"Invalid value \" + value + \" for enum").Space()
                        .Append(TypeName).Append("\")").EndOfStatement();
                }
            }
            else
            {
                Builder.Append("switch").Space().Parenthesized().Append("value").Close().AppendLine();
                using (Builder.Block())
                {
                    foreach (var member in m_members)
                    {
                        Builder.Append($"case {member.Value}:").AppendLine().
                            IndentChild().Append("return").Space().Append(TypeName).Dot().Append(member.Name).EndOfStatement();
                    }
                    Builder.Append("default:").AppendLine();
                    Builder.IndentChild().Append("throw new RuntimeException(\"Invalid value \" + value + \" for enum").Space()
                        .Append(TypeName).Append("\")").EndOfStatement();
                }
            }
        }
    }

    void addGeneratedBitmasks(SortedDictionary<long, (string Name, bool Generated)> allMembers, List<int> bitPositions)
    {
        addGeneratedBitmasks(allMembers, bitPositions, 0, 0);
    }

    void addGeneratedBitmasks(SortedDictionary<long, (string Name, bool Generated)> allMembers, List<int> bitPositions,
        int posIndex, ulong bitmask)
    {
        Debug.Assert(posIndex < 64);
        if (posIndex == bitPositions.Count - 1)
        {
            // NOTE: Don't try to re-add single bit fields
            if (bitmask != 0)
            {
                ulong value = bitmask | (ulong)(1 << bitPositions[posIndex]);
                string name = $"_bitmask_{value}";
                // Ignore possibly already existing members
                _ = allMembers.TryAdd((long)value, (name, true));
            }

            return;
        }

        addGeneratedBitmasks(allMembers, bitPositions, posIndex + 1, bitmask);
        addGeneratedBitmasks(allMembers, bitPositions, posIndex + 1, bitmask | (ulong)(1 << bitPositions[posIndex]));
    }

    bool isPowerOfTwo(long x)
    {
        return (x & (x - 1)) == 0;
    }

    static int findPosition(long n)
    {
        int i = 1, pos = 0;

        // Iterate through bits of n till we find a set bit
        // i&n will be non-zero only when 'i' and 'n' have a set bit
        // at same position
        while ((i & n) == 0)
        {
            // Unset current bit and set the next bit in 'i'
            i = i << 1;

            // increment position
            ++pos;
        }

        return pos;
    }

    class EnumMember
    {
        public string Name = null!;
        public long Value;
    }

    class EnumAlias
    {
        public string Name = null!;
        public string Value = null!;
    }
}
