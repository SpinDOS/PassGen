using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PassGen
{
    public sealed class PassGenArgsParser
    {
        public static readonly string SaltEnvironmentVariableName = "PG_SALT";
        
        private static readonly string SetSaltEnvVariableCommand, SaltFilePathForHelp;

        private bool _parsed;
        private PassGenArgs _passGenArgs;

        static PassGenArgsParser()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SetSaltEnvVariableCommand = $"set {SaltEnvironmentVariableName}=...";
                SaltFilePathForHelp = Path.Combine("%userprofile%", ".passgen", "salt");
            }
            else
            {
                SetSaltEnvVariableCommand = $"export {SaltEnvironmentVariableName}='...'";
                SaltFilePathForHelp = Path.Combine("~", ".passgen", "salt");
            }
        }

        public PassGenArgs GetParsedArgs()
        {
            if (!_parsed)
                throw new InvalidOperationException("Args were not parsed");
            return _passGenArgs;
        }
        
        public bool TryParsePassGenArgs(string[] args)
        {
            if (!AreValidArgs(args))
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
            Console.WriteLine("Command line arguments: dotnet passgen.dll <target> [salt]");
            Console.WriteLine("Environment variable: " + SetSaltEnvVariableCommand);
            Console.WriteLine("Contents of passgen salt file in your home directory: " + SaltFilePathForHelp);
        }

        private bool AreValidArgs(string[] args) => 
            args != null && args.Length > 0 && args.Length <= 2 && !string.IsNullOrEmpty(args[0]);
        
        private bool TryExtractSalt(string[] args, out string salt)
        {
            salt = TryExtractSaltFromArgs(args) ??
                   TryExtractSaltFromEnvironment() ??
                   TryExtractSaltFromHomeDirectory();
            return salt != null;
        }

        private string TryExtractSaltFromArgs(string[] args) => 
            args.Length == 2? args[1] : null;

        private string TryExtractSaltFromEnvironment() =>
            Environment.GetEnvironmentVariable(SaltEnvironmentVariableName);

        private string TryExtractSaltFromHomeDirectory()
        {
            var saltFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".passgen",
                "salt"
            );

            if (!File.Exists(saltFilePath))
                return null;

            try
            {
                return File.ReadAllText(saltFilePath);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Console.WriteLine("Could not access salt file in .passgen directory");
                return null;
            }
        }
    }
}