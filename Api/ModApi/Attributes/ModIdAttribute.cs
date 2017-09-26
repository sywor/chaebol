using System;

namespace ModApi.Attributes
{
    public class ModIdAttribute : Attribute
    {
        public ModIdAttribute(string _id)
        {
            Id = _id;
        }

        public string Id;
    }
}