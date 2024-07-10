# nullable enable
using System.Runtime.InteropServices;
using AdysTech.CredentialManager;

namespace PassGen.GlobalTool;

public sealed class SaltResolver
{
    public const string PgSalt = "PG_SALT";

	public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
	public static bool IsOSX => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
	public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    public string? ResolveSalt(string? argsSalt)
	{
		return argsSalt
			?? TryExtractSaltFromEnvironment()
			?? TryExtractSaltFromWindowsCredentialManager()
			?? TryExtractSaltFromMacOSXKeychain()
			?? TryExtractSaltFromLinuxHomeDirectory()
			?? null;
	}

    private string? TryExtractSaltFromEnvironment() => Environment.GetEnvironmentVariable(PgSalt);

    private string? TryExtractSaltFromWindowsCredentialManager() =>
		IsWindows ? CredentialManager.GetCredentials(PgSalt)?.Password : null;

	private string? TryExtractSaltFromMacOSXKeychain()
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			return null;

		try
		{
			var (username, password) = OSXKeyChain.Instance.Query(PgSalt);
			return password;
		}
		catch (OSXKeyChain.NotFoundException)
		{
			return null;
		}
	}

    private string? TryExtractSaltFromLinuxHomeDirectory()
    {
        if (!IsLinux)
            return null;

        var saltFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".passgen",
            "salt"
        );

        if (!File.Exists(saltFilePath))
            return null;

        try
        {
            return File.ReadAllText(saltFilePath).Trim();
        }
        catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
        {
            throw new LoggedException($"secret salt file access error: {e.Message}");
        }
    }
}
