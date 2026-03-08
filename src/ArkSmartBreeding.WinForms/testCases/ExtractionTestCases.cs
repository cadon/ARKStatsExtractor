using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ARKBreedingStats.testCases
{
    [Serializable]
    public class ExtractionTestCases
    {
        [XmlArray]
        public List<ExtractionTestCase> testCases = new List<ExtractionTestCase>();
    }
}
