using System;

namespace ModApi.Attributes
{
    public class ModIdAttribute : Attribute
    {
        public string Id;
        
        public ModIdAttribute(string _id)
        {
            Id = _id;
        }
    }
}