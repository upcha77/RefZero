using System;

namespace RefZero.GUI
{
    public class ReferenceItem
    {
        public string Name { get; set; }
        public string PhysicalPath { get; set; }
        public string Version { get; set; }
        public string SourceType { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Version})";
        }
    }
}
