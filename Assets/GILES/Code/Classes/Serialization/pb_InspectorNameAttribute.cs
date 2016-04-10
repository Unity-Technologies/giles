using System;

namespace GILES.Serialization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class pb_InspectorNameAttribute : Attribute
    {

        public readonly string name;

        public pb_InspectorNameAttribute(string name)
        {
            this.name = name;
        }
    }
}