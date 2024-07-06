using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using AdysTech.CredentialManager;

namespace PassGen.GlobalTool
{
    public sealed class PassGenArgsParseException : ArgumentException
    {
        public PassGenArgsParseException(string message) : base(message) { }
        public PassGenArgsParseException(string message, Exception innerException) : base(message, innerException) { }
    }
    
    public sealed class PassGenArgsParser
    {
        public static readonly string PgSalt = "PG_SALT";
        
        private bool _parsed;
        private PassGenArgs _passGenArgs;

        public PassGenArgs GetParsedArgs()
        {
            if (!_parsed)
                throw new InvalidOperationException("Args were not parsed");
            return _passGenArgs;
        }
        
        public bool TryParsePassGenArgs(string[] args)
        {
            if (args.Contains("--help") || !AreValidArgs(args))
            {
                PrintUsageHelp();
                return false;
            }
            if (!TryExtractSalt(args, out var salt))
            {
                PrintSaltHelp();
                return false;
            }
            
            _passGenArgs.Target = args[0];
            _passGenArgs.Salt = salt;
            _parsed = true;
            return true;
        }

        private void PrintUsageHelp()
        {
            Console.WriteLine("Usage: dotnet passgen.dll <target> [salt]");
            Console.WriteLine("target   key of target website/program to generate password for");
            Console.WriteLine("salt     secret salt which is the password encrypted with");
        }

        private void PrintSaltHelp()
        {
            Console.WriteLine("Salt must be provided with one of the following ways: ");
            Console.WriteLine("1. Command line arguments: dotnet passgen.dll <target> [salt]");
            Console.WriteLine($"2. Environment variable '{PgSalt}'");
            if (IsOsWindows())
                Console.WriteLine($"3. Windows CredentialManager's generic credential '{PgSalt}'");
            else if (IsOsUnix())
                Console.WriteLine("3. Contents of passgen salt file in your home directory: ~/.passgen/salt");
        }

        private bool AreValidArgs(string[] args) => 
            args != null && args.Length > 0 && args.Length <= 2 && !string.IsNullOrEmpty(args[0]);
        
        private bool TryExtractSalt(string[] args, out string salt)
        {
            try
            {
                salt = TryExtractSaltFromArgs(args) ??
                       TryExtractSaltFromEnvironment() ??
                       TryExtractSaltFromWindowsCredentialManager() ??
                       TryExtractSaltFromUnixHomeDirectory();
            }
            catch (PassGenArgsParseException e)
            {
                Console.WriteLine(e.Message);
                salt = null;
            }
            
            return !string.IsNullOrEmpty(salt);
        }

        private string TryExtractSaltFromArgs(string[] args)
        {
            if (args.Length != 2)
                return null;

            var salt = args[1];
            if (string.IsNullOrEmpty(salt))
                throw new PassGenArgsParseException("Salt argument should not be empty");

            return salt;
        }

        private string TryExtractSaltFromEnvironment()
        {
            var salt = Environment.GetEnvironmentVariable(PgSalt);
            if (salt == string.Empty)
                throw new PassGenArgsParseException($"{PgSalt} environment variable should not contain empty salt");
            return salt;
        }

        private string TryExtractSaltFromWindowsCredentialManager() {
            if (!IsOsWindows())
                return null;
            var credential = CredentialManager.GetCredentials(PgSalt);
            if (credential == null) 
                return null;
            if (string.IsNullOrEmpty(credential.Password))
                throw new PassGenArgsParseException($"Credential {PgSalt} password should not be empty");
            return credential.Password;
        }

        private string TryExtractSaltFromUnixHomeDirectory()
        {
            if (!IsOsUnix())
                return null;

            var saltFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".passgen",
                "salt"
            );

            if (!File.Exists(saltFilePath))
                return null;

            string[] lines;
            try
            {
                lines = File.ReadAllLines(saltFilePath);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                throw new PassGenArgsParseException($"Could not access secret salt file {saltFilePath}", e);
            }

            if (lines == null || lines.Length != 1)
                throw new PassGenArgsParseException($"Expected single line with secret salt in file {saltFilePath}");

            var salt = lines[0];
            if (string.IsNullOrEmpty(salt))
                throw new PassGenArgsParseException($"Secret salt in file {saltFilePath} should not be empty");
            
            return salt;
        }

        private static bool IsOsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private static bool IsOsUnix() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) | RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    }
}