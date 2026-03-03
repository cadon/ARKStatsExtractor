using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARKBreedingStats.Tests
{
    /// <summary>
    /// Basic smoke tests to verify the project builds and assemblies load correctly
    /// </summary>
    [TestClass]
    public class BasicSmokeTests
    {
        [TestMethod]
        public void ARKBreedingStats_Assembly_Loads()
        {
            // Arrange & Act
            var assembly = Assembly.GetAssembly(typeof(Utils));

            // Assert
            Assert.IsNotNull(assembly, "ARKBreedingStats assembly should load");
            Assert.AreEqual("ARK Smart Breeding", assembly.GetName().Name, "Assembly name should be ARK Smart Breeding");
        }

        [TestMethod]
        public void ASBUpdater_Assembly_Loads()
        {
            // Arrange & Act
            var assembly = Assembly.GetAssembly(typeof(ASB_Updater.ASBUpdater));

            // Assert
            Assert.IsNotNull(assembly, "ASB Updater assembly should load");
        }

        [TestMethod]
        public void ARKBreedingStats_TargetFramework_IsNet48()
        {
            // Arrange
            var assembly = Assembly.GetAssembly(typeof(Utils));

            // Act
            var targetFrameworkAttribute = assembly.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>();

            // Assert
            Assert.IsNotNull(targetFrameworkAttribute, "Assembly should have TargetFramework attribute");
            Assert.IsTrue(targetFrameworkAttribute.FrameworkName.Contains(".NETFramework,Version=v4.8"), 
                $"Expected .NET Framework 4.8 but got: {targetFrameworkAttribute.FrameworkName}");
        }

        [TestMethod]
        public void Utils_Class_IsAccessible()
        {
            // Arrange & Act
            var utilsType = typeof(Utils);

            // Assert
            Assert.IsNotNull(utilsType, "Utils class should be accessible");
            Assert.IsTrue(utilsType.IsClass, "Utils should be a class");
        }

        [TestMethod]
        public void NewtonsoftJson_IsReferenced()
        {
            // Arrange & Act
            var assembly = Assembly.GetAssembly(typeof(Newtonsoft.Json.JsonConvert));

            // Assert
            Assert.IsNotNull(assembly, "Newtonsoft.Json assembly should be referenced and loadable");
            Assert.AreEqual("Newtonsoft.Json", assembly.GetName().Name);
        }

        [TestMethod]
        public void SystemDrawing_IsReferenced()
        {
            // Arrange & Act
            var color = System.Drawing.Color.Red;

            // Assert
            Assert.AreEqual(255, color.R, "System.Drawing should be available");
        }
    }
}
