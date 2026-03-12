using System;
using System.Linq;
using System.Security.Principal;

namespace RegInitializer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var pathToSet = args.Length > 0 ? args[0] : @"C:\WinPure\ER\api\lib";

            SetPath(EnvironmentVariableTarget.User, pathToSet);

            if (IsAdministrator())
            {
                SetPath(EnvironmentVariableTarget.Machine, pathToSet);
            }
            else
            {
                Console.WriteLine("Cannot set system path. User is not Admin!");
                throw new Exception("Cannot set system path. User is not Admin!");
            }

            Console.WriteLine("Registry was initialized!");
        }


        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void SetPath(EnvironmentVariableTarget targetNode, string pathToSet)
        {
            var pathEnvironment = Environment.GetEnvironmentVariable("Path", targetNode).Split(';').ToList();
            if (!pathEnvironment.Any(x => x == pathToSet))
            {
                pathEnvironment.Add(pathToSet);
                var pathVariable = string.Join(";", pathEnvironment);
                Environment.SetEnvironmentVariable("Path", pathVariable, targetNode);
            }
        }
    }
}
