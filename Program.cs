using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GitDir
{
    class Program
    {
        const string GIT_DIRECTORY_NAME = ".git";
        const string GIT_COMMAND = "git";
        const string ENVIRONMENT_VARIABLE_NAME = "PATH";
        const char ARGS_SEPARATOR = ' ';
        const char ENVIRONMENT_VARIABLE_SPLITER = ';';
        static void Main(string[] args)
        {
            string location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string variable = Environment.GetEnvironmentVariable(ENVIRONMENT_VARIABLE_NAME, EnvironmentVariableTarget.User);

            string[] values = variable.Split(ENVIRONMENT_VARIABLE_SPLITER);

            if (!values.Any(val => val.Equals(location)))
            {
                List<String> tempValues = values.ToList();

                tempValues.Add(location);

                variable = String.Join(ENVIRONMENT_VARIABLE_SPLITER, tempValues.ToArray());

                Environment.SetEnvironmentVariable(ENVIRONMENT_VARIABLE_NAME, variable, EnvironmentVariableTarget.User);
            }

            string[] directories = Directory.GetDirectories(Environment.CurrentDirectory);

            List<string> gitDirectories = new List<string>();

            foreach (string directory in directories)
            {
                if (Directory.GetDirectories(directory).Any(d => d.EndsWith(GIT_DIRECTORY_NAME)))
                {
                    gitDirectories.Add(directory);
                }
            };

            foreach (string gitDirectory in gitDirectories)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(gitDirectory);

                Console.ForegroundColor = ConsoleColor.DarkGray;

                ProcessStartInfo processStartInfo = new ProcessStartInfo(GIT_COMMAND, String.Join(ARGS_SEPARATOR, args));

                Process process = new Process();

                processStartInfo.WorkingDirectory = gitDirectory;

                process.StartInfo = processStartInfo;
                process.Start();
                process.WaitForExit();

                Console.WriteLine();
            }

            Console.ResetColor();
        }
    }
}