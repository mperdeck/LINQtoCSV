using System;

namespace LINQtoCSV
{
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property,
        AllowMultiple = false)
    ]
    public class IgnoreColumnAttribute : Attribute
    {
        
    }
}