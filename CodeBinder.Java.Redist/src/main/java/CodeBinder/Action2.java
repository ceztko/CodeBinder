/*
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT
 */
package CodeBinder;

@FunctionalInterface
public interface Action2<T0, T1>
{
    void apply(T0 arg0, T1 arg1);
}
