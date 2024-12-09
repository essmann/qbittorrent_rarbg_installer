using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequests.Classes
{
    internal class SetEnvVariables
    {
        public static int SetEnvironmentVariables()
        {
            try
            {
                string? programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string? projectRoot = Directory.GetParent(programDirectory).Parent.Parent.Parent.FullName;
                string scriptsDirectory = Path.Combine(projectRoot, "Scripts");

                // Step 3: Retrieve the current PATH for the user
                string? currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);

                // Step 4: Append the 'root/' directory (if not already included)
                if (!currentPath.Contains(projectRoot))
                {
                    currentPath += $";{projectRoot}";
                    Console.WriteLine("The current directory has been added to the PATH permanently.");
                }

                // Step 5: Set the updated PATH permanently for the user

                Environment.SetEnvironmentVariable("PATH", currentPath, EnvironmentVariableTarget.User);
                // Step 6: Set a new permanent user environment variable called 'rarbgpath'
                // Example value, adjust as needed
                Environment.SetEnvironmentVariable("rarbg_cli_path", projectRoot, EnvironmentVariableTarget.User);
                return 1;
            }
            catch(NullReferenceException )
            {
                string errorMsg = "Cannot run this script from outside the project directory unless program is set to path. Refresh terminal if this is your first time running the program.";
                Console.WriteLine(errorMsg);
                return 0;
            }
            // Step 2: Create the 'scripts/' directory path relative to the project root
           
        }
    }
}
