using NUnit.Framework;
using PassGen.Lib;

namespace PassGen.Tests
{
    public class BasicTests
    {
        [Test]
        public void SimpleTest()
        {
            var passwordGenerator = new PasswordGenerator();
            var generatedPassword = passwordGenerator.GeneratePassword("a", "b");
            Assert.AreEqual("p078064d5#7G", generatedPassword);
        }

        [Test]
        public void UnicodeTest()
        {
            var passwordGenerator = new PasswordGenerator();
            var generatedPassword = passwordGenerator.GeneratePassword("а", "б");
            Assert.AreEqual("p1e8cb032#7G", generatedPassword);
        }
    }
}