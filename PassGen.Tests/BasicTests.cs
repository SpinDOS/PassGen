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
    }
}