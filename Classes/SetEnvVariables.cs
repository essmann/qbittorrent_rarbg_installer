using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequests.Classes
{
    internal class SetEnvVariables
    {
        public static void SetEnvironmentVariables()
        {
            string projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

            // Step 2: Create the 'scripts/' directory path relative to the project root
            string scriptsDirectory = Path.Combine(projectRoot, "Scripts");

            // Step 3: Retrieve the current PATH for the user
            string? currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);

            // Step 4: Append the 'scripts/' directory (if not already included)
            if (!currentPath.Contains(scriptsDirectory))
            {
                currentPath += $";{scriptsDirectory}";
                Console.WriteLine("The current directory has been added to the PATH permanently.");
            }
            
            // Step 5: Set the updated PATH permanently for the user

            Environment.SetEnvironmentVariable("PATH", currentPath, EnvironmentVariableTarget.User);
            // Step 6: Set a new permanent user environment variable called 'rarbgpath'
              // Example value, adjust as needed
            Environment.SetEnvironmentVariable("rarbg_cli_path", projectRoot, EnvironmentVariableTarget.User);
        }
    }
}
