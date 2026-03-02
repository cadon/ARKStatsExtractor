using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARKBreedingStats.Tests
{
    /// <summary>
    /// Tests for SpeechRecognition — primarily verifies System.Speech.dll loads correctly.
    /// </summary>
    [TestClass]
    public class SpeechRecognitionTests
    {
        [TestMethod]
        public void SpeechRecognition_AssemblyLoads_NoFileNotFoundException()
        {
            // Passing an empty aliases list causes the constructor to return immediately,
            // before creating a SpeechRecognitionEngine or touching audio hardware.
            // However, just referencing the SpeechRecognition type forces the runtime to
            // resolve System.Speech.dll — so this will throw FileNotFoundException if
            // the DLL is missing or has an incompatible assembly version.
            var sr = new SpeechRecognition(100, 1, new List<string>(), null);

            Assert.IsFalse(sr.Initialized,
                "Initialized should be false when no aliases are provided.");
        }
    }
}
