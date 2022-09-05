// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.IO;
using System.Collections;
using System.Reflection;

public class ObjectDumper
{
    // NB: copied from VS2008 LINQ samples; does the job for now
    // TODO: improve this to produce better output

    public static string Write(object o)
    {
        return Write(o, 4);
    }

    public static string Write(object o, int depth)
    {
        var sw = new StringWriter { NewLine = "\r\n" };
        ObjectDumper dumper = new ObjectDumper(sw, depth);
        dumper.WriteObject(null, o);
        return sw.ToString();
    }

    private readonly TextWriter writer;
    private readonly int depth;

    private int pos;
    private int level;

    private ObjectDumper(TextWriter writer, int depth)
    {
        this.writer = writer;
        this.depth = depth;
    }

    private void Write(string s)
    {
        if (s != null)
        {
            writer.Write(s);
            pos += s.Length;
        }
    }

    private void WriteIndent()
    {
        for (int i = 0; i < level; i++) writer.Write("  ");
    }

    private void WriteLine()
    {
        writer.WriteLine();
        pos = 0;
    }

    private void WriteTab()
    {
        Write("  ");
        while (pos % 8 != 0) Write(" ");
    }

    private void WriteObject(string prefix, object o)
    {
        if (o == null || o is ValueType || o is string)
        {
            WriteIndent();
            Write(prefix);
            WriteValue(o);
            WriteLine();
        }
        else if (o is IEnumerable)
        {
            foreach (object element in (IEnumerable)o)
            {
                if (element is IEnumerable && !(element is string))
                {
                    WriteIndent();
                    Write(prefix);
                    Write("...");
                    WriteLine();
                    if (level < depth)
                    {
                        level++;
                        WriteObject(prefix, element);
                        level--;
                    }
                }
                else
                {
                    WriteObject(prefix, element);
                }
            }
        }
        else
        {
            MemberInfo[] members = o.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            WriteIndent();
            Write(prefix);
            bool propWritten = false;
            foreach (MemberInfo m in members)
            {
                FieldInfo f = m as FieldInfo;
                PropertyInfo p = m as PropertyInfo;
                if (f != null || p != null)
                {
                    if (propWritten)
                    {
                        WriteTab();
                    }
                    else
                    {
                        propWritten = true;
                    }
                    Write(m.Name);
                    Write("=");
                    Type t = f != null ? f.FieldType : p.PropertyType;
                    if (t.IsValueType || t == typeof(string))
                    {
                        WriteValue(f != null ? f.GetValue(o) : p.GetValue(o, null));
                    }
                    else
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(t))
                        {
                            Write("...");
                        }
                        else
                        {
                            Write("{ }");
                        }
                    }
                }
            }
            if (propWritten) WriteLine();
            if (level < depth)
            {
                foreach (MemberInfo m in members)
                {
                    FieldInfo f = m as FieldInfo;
                    PropertyInfo p = m as PropertyInfo;
                    if (f != null || p != null)
                    {
                        Type t = f != null ? f.FieldType : p.PropertyType;
                        if (!(t.IsValueType || t == typeof(string)))
                        {
                            object value = f != null ? f.GetValue(o) : p.GetValue(o, null);
                            if (value != null)
                            {
                                level++;
                                WriteObject(m.Name + ": ", value);
                                level--;
                            }
                        }
                    }
                }
            }
        }
    }

    private void WriteValue(object o)
    {
        if (o == null)
        {
            Write("null");
        }
        else if (o is DateTime)
        {
            Write(((DateTime)o).ToShortDateString());
        }
        else if (o is ValueType || o is string)
        {
            Write(o.ToString());
        }
        else if (o is IEnumerable)
        {
            Write("...");
        }
        else
        {
            Write("{ }");
        }
    }
}
