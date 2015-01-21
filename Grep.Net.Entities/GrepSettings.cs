using System;
using System.Linq;
using System.Xml.Serialization;

namespace Grep.Net.Entities
{
    public enum GrepEncodings
    {
        UTF8,
        UTF7,
        UTF32,
        ASCII,
        Unicode,
        Default
    }

    public class GrepSettings
    {
        [XmlElement]
        public bool Recursive { get; set; }

        [XmlElement]
        public int LinesBefore { get; set; }

        [XmlElement]
        public int LinesAfter { get; set; }
        
        public bool AllMatches { get; set; }

        public bool CaseSensitive { get; set; }

        public GrepEncodings GrepEncoding { get; set; }

        public int MaxLineSize { get; set; }

        public int MaxContextSize { get; set; }

        public int MaxThreads { get; set; }

        public GrepSettings()
        {
            this.LinesAfter = 2;
            this.LinesBefore = 2;
            this.MaxContextSize = 1000;
            this.MaxLineSize = 1000;
            this.MaxThreads = 25; 
        }
    }
}