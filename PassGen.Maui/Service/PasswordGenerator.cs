namespace PassGen.Maui;

public interface IPasswordGenerator
{
    string GeneratePassword(string targetSite, string salt);
}

public sealed class PasswordGeneratorAdapter : IPasswordGenerator
{
    // private readonly PasswordGenerator _passwordGenerator = new PasswordGenerator();

    public string GeneratePassword(string targetSite, string salt) => "test_for_" + targetSite;
        // _passwordGenerator.GeneratePassword(targetSite, salt);
}
