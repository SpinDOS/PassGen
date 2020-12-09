using System;
using PassGen.Lib;

namespace PassGen.GlobalTool
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var passGenArgsParser = new PassGenArgsParser();
            if (!passGenArgsParser.TryParsePassGenArgs(args))
                return;

            var parsedArgs = passGenArgsParser.GetParsedArgs();
            
            var passwordGenerator = new PasswordGenerator();
            var password = passwordGenerator.GeneratePassword(parsedArgs.Target, parsedArgs.Salt);
            
            Console.WriteLine(password);
        }
    }
}