# nullable enable
using System.Diagnostics;
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
		if (!IsOSX)
			return null;

		const string errorMessageProlog = "security tool error";
		var processStartInfo = new ProcessStartInfo()
		{
			FileName = "security",
			CreateNoWindow = true,
			RedirectStandardInput = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
		};

		var args = new string[] {"find-generic-password", "-a", Environment.UserName, "-s", PgSalt, "-w"};
		foreach (var arg in args)
		    processStartInfo.ArgumentList.Add(arg);

		using var process = Process.Start(processStartInfo);
		if (process == null)
			throw new LoggedException($"{errorMessageProlog}: failed to start");

		try
		{
			if (!process.WaitForExit(TimeSpan.FromMinutes(1)))
			    throw new LoggedException($"{errorMessageProlog}: process has not finished within 10 seconds");
		}
		finally
		{
			if (!process.HasExited)
				process.Kill(true);
		}

		if (process.ExitCode != 0)
			return null; // missing credential results in non-zero exit code

		return process.StandardOutput.ReadToEnd()?.Trim();
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
