/*
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT
 */
package CodeBinder;

@FunctionalInterface
public interface Function3<T0, T1, T2, R>
{
    R apply(T0 arg0, T1 arg1, T2 arg2);
}
