using System;
using System.Collections.Generic;
using System.Linq;

namespace ModApi.Attributes
{
    public class DependsOnAttribute : Attribute
    {
        public DependsOnAttribute(params string[] _dependencies)
        {
            Dependencies = _dependencies.ToList();
        }

        public List<string> Dependencies { get; private set; }
    }
}