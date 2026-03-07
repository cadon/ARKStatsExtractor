using ARKBreedingStats.Mods;
using ARKBreedingStats.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkSmartBreeding.Tests
{
    /// <summary>
    /// Tests for the Mod class.
    /// </summary>
    [TestClass]
    public class ModTests
    {
        [TestMethod]
        public void OtherMod_Singleton_HasExpectedName()
        {
            var other = Mod.OtherMod;
            Assert.IsNotNull(other);
            Assert.AreEqual(Mod.OtherModName, other.Title);
        }

        [TestMethod]
        public void Equals_SameIdAndTag_ReturnsTrue()
        {
            var m1 = new Mod { Id = "123", Tag = "TestMod" };
            var m2 = new Mod { Id = "123", Tag = "TestMod" };
            Assert.IsTrue(m1.Equals(m2));
        }

        [TestMethod]
        public void Equals_DifferentId_ReturnsFalse()
        {
            var m1 = new Mod { Id = "123", Tag = "Mod" };
            var m2 = new Mod { Id = "456", Tag = "Mod" };
            Assert.IsFalse(m1.Equals(m2));
        }

        [TestMethod]
        public void ToString_ReturnsTitle()
        {
            var mod = new Mod { Title = "My Cool Mod" };
            Assert.AreEqual("My Cool Mod", mod.ToString());
        }
    }

    /// <summary>
    /// Tests for simple library model classes.
    /// </summary>
    [TestClass]
    public class LibraryModelTests
    {
        [TestMethod]
        public void Note_DefaultConstructor_HasNullFields()
        {
            var note = new Note();
            Assert.IsNull(note.Title);
            Assert.IsNull(note.Text);
        }

        [TestMethod]
        public void Note_TitleConstructor_SetsTitle()
        {
            var note = new Note("My Note");
            Assert.AreEqual("My Note", note.Title);
        }

        [TestMethod]
        public void Player_DefaultValues()
        {
            var player = new Player();
            Assert.IsNull(player.PlayerName);
            Assert.AreEqual(0, player.Level);
        }

        [TestMethod]
        public void Tribe_DefaultValues()
        {
            var tribe = new Tribe();
            Assert.AreEqual("", tribe.TribeName);
            Assert.AreEqual(Tribe.Relation.Neutral, tribe.TribeRelation);
            Assert.AreEqual("", tribe.Note);
        }

        [TestMethod]
        public void Tribe_AllRelationsExist()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(Tribe.Relation), Tribe.Relation.Neutral));
            Assert.IsTrue(System.Enum.IsDefined(typeof(Tribe.Relation), Tribe.Relation.Allied));
            Assert.IsTrue(System.Enum.IsDefined(typeof(Tribe.Relation), Tribe.Relation.Friendly));
            Assert.IsTrue(System.Enum.IsDefined(typeof(Tribe.Relation), Tribe.Relation.Hostile));
        }

        [TestMethod]
        public void TimerListEntry_DefaultIsRunning()
        {
            var timer = new TimerListEntry();
            Assert.IsTrue(timer.timerIsRunning);
        }
    }
}
