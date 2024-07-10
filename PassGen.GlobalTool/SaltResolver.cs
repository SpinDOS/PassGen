# nullable enable
using System.Runtime.InteropServices;

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

    private string? TryExtractSaltFromWindowsCredentialManager()
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		    return null;

		try
		{
			return WindowsCredentialsManager.Instance.GetPassword(PgSalt);
		}
		catch (WindowsCredentialsManager.NotFoundException)
		{
			return null;
		}
	}

	private string? TryExtractSaltFromMacOSXKeychain()
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			return null;

		try
		{
			return OSXKeyChain.Instance.Query(PgSalt).password;
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
