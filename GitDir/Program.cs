using GitDir.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GitDir
{
    class Program
    {
        #region FIELDS
        static List<string> NonGitDirectories { get; set; }
        static string[] Directories { get; set; }
        static int CurrentDirectoryIndex { get; set; }
        static Process Process { get; set; }
        static bool AlreadyShownNonGitDirectories { get; set; }
        static string GitCommand { get; set; }
        static IConfigurationRoot Configuration { get; set; }
        static Messages Messages { get; set; }
        static string DefaultLanguage { get; set; }
        #endregion
        #region CONSTRUCT
        private static void Main(string[] args)
        {
            Messages = new Messages();

            var environmentVariable = Environment.GetEnvironmentVariable(Constants.ENVIRONMENT_VARIABLE_NAME);

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var builder = new ConfigurationBuilder().SetBasePath(baseDirectory).AddJsonFile(Constants.SETTINGS_JSON_FILE);

            Configuration = builder.Build();

            Configuration.GetSection(Constants.SETTINGS_SECTION_MESSAGES).Bind(Messages);

            DefaultLanguage = Configuration.GetSection(Constants.SETTINGS_SECTION_DEFAULT_LANGUAGE).Value;

            if (environmentVariable.Contains(baseDirectory) && args.Length > 0)
            {
                List<string> tempList = Environment.CommandLine.Split(Constants.SPACE_CHAR).ToList();

                GitCommand = string.Join(Constants.SPACE_CHAR, tempList);

                Directories = Directory.GetDirectories(Environment.CurrentDirectory);

                if (Directories.Length > 0)

                    ProcessDirectory(CurrentDirectoryIndex);

                else

                    Console.WriteLine(Messages.GetMessage(Constants.MESSAGES_IDENTIFIER_NO_DIRECTORIES).GetText(DefaultLanguage));
            }
            else if (environmentVariable.Contains(baseDirectory) == false)
            {
                environmentVariable += string.Concat(Constants.ENVIRONMENT_VARIABLE_SEPARATOR, baseDirectory);

                Environment.SetEnvironmentVariable(Constants.ENVIRONMENT_VARIABLE_NAME, environmentVariable, EnvironmentVariableTarget.User);

                Console.Write(Messages.GetMessage(Constants.MESSAGES_IDENTIFIER_INSTALLED_REBOOT).GetText(DefaultLanguage));

                var reboot = Console.ReadKey();

                if (reboot.KeyChar.ToString().ToLower() == Constants.YES_CHAR.ToString().ToLower())
                {
                    Process.Start(Constants.SHUTDOWN_COMMAND, Constants.SHUTDOWN_ARGS);
                }
            }
            else
            {
                Console.WriteLine(Messages.GetMessage(Constants.MESSAGES_IDENTIFIER_NO_GIT_COMMAND).GetText(DefaultLanguage));
            }
        }
        #endregion
        #region METHODS
        private static void ProcessDirectory(int directoryIndex)
        {
            Environment.CurrentDirectory = Directories[directoryIndex];

            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, Constants.GIT_DIR_NAME)))
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(string.Concat(Messages.GetMessage(Constants.MESSAGES_IDENTIFIER_DIR_INDICATOR).GetText(DefaultLanguage), Environment.CurrentDirectory, Constants.LINE_FEED));

                Console.ResetColor();

                if (Process == null)
                {
                    Process = new Process();

                    ProcessStartInfo startInfo = new ProcessStartInfo();

                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    startInfo.FileName = Constants.GIT_COMMAND;

                    startInfo.Arguments = GitCommand;

                    Process.StartInfo = startInfo;
                }

                Process.Exited += Process_Exited;

                Process.EnableRaisingEvents = true;

                Process.Start();

                Process.WaitForExit();

            }
            else
            {
                NonGitDirectories = NonGitDirectories ?? new List<string>();

                NonGitDirectories.Add(Environment.CurrentDirectory);

                Process_Exited(null, null);
            }
        }
        private static void Process_Exited(object sender, EventArgs e)
        {
            CurrentDirectoryIndex = CurrentDirectoryIndex + 1;

            if (CurrentDirectoryIndex < Directories.Length)
            {
                ProcessDirectory(CurrentDirectoryIndex);
            }
            else if (AlreadyShownNonGitDirectories == false)
            {
                AlreadyShownNonGitDirectories = true;

                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(Messages.GetMessage(Constants.MESSAGES_IDENTIFIER_DIR_INFO).GetText(DefaultLanguage));

                Console.ResetColor();

                foreach (string directory in Directories)
                {
                    NonGitDirectories = NonGitDirectories ?? new List<string>();

                    string cross = NonGitDirectories.Any(i => i == directory) ? Constants.CHECK_BOX_CROSS : Constants.CHECK_BOX_CHECK;

                    Console.Write(Constants.CHECK_BOX_START);

                    if (cross != Constants.CHECK_BOX_CROSS)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Console.Write(cross);

                    Console.ResetColor();

                    Console.Write(string.Concat(Constants.CHECK_BOX_END, directory, Constants.LINE_FEED));
                }
            }

            Process?.Refresh();

        }
        #endregion
    }
}