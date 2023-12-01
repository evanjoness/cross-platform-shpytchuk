using McMaster.Extensions.CommandLineUtils;
using Labs;


[Command(Name = "lab-runner", Description = "A utility for running different lab programs.")]
[Subcommand(typeof(VersionCommand), typeof(RunLabCommand), typeof(SetPathCommand))]
class LabRunnerApp
{
    static int Main(string[] args)
    {
        return CommandLineApplication.Execute<LabRunnerApp>(args);
    }
}

[Command("version", Description = "Displays author and version")]
public class VersionCommand
{
    private void OnExecute()
    {
        Console.WriteLine("Author: Ivan Shpytchuk");
        Console.WriteLine("Version: 1.0");
    }
}

[Command("run", Description = "Run the labs")]
public class RunLabCommand
{
    [Argument(0, Description = "Name of the lab - lab1, lab2, lab3)")]
    public string Lab { get; set; }

    [Option("-i|--input <INPUT>", Description = "Path to input file")]
    public string Input { get; set; }

    [Option("-o|--output <OUTPUT>", Description = "Path to output file")]
    public string Output { get; set; }

    private static readonly Dictionary<string, Action<string, string>> LabRunners
        = new Dictionary<string, Action<string, string>>
        {
            { "lab1", FirstLab.Run },
            { "lab2", SecondLab.Run },
            { "lab3", ThirdLab.Run }
        };

    private void OnExecute()
    {
        if (LabRunners.ContainsKey(Lab))
        {
            string inputPath = Input;
            string outputPath = Output;

            if (string.IsNullOrEmpty(inputPath))
            {
                inputPath = Environment.GetEnvironmentVariable("LAB_PATH", EnvironmentVariableTarget.User);
                if (string.IsNullOrEmpty(inputPath))
                {
                    inputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "input.txt");
                }
                else
                {
                    inputPath = Path.Combine(inputPath, "input.txt");
                }
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = Environment.GetEnvironmentVariable("LAB_PATH", EnvironmentVariableTarget.User);
                if (string.IsNullOrEmpty(outputPath))
                {
                    outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "output.txt");
                }
                else
                {
                    outputPath = Path.Combine(outputPath, "output.txt");
                }
            }

            if (File.Exists(inputPath))
            {
                LabRunners[Lab](inputPath, outputPath);
                Console.WriteLine($"Running {Lab} with input={inputPath} and output={outputPath}");
            }
            else
            {
                Console.Error.WriteLine($"Error: Cannot find input.txt file at {inputPath}.");
                Console.WriteLine($"Resolved input path: {inputPath}");
                Console.WriteLine($"Resolved output path: {outputPath}");
            }
        }
        else
        {
            Console.Error.WriteLine($"Error: Unknown lab {Lab}.");
        }
    }

}

[Command("set-path", Description = "Sets the path to the folder with input and output files.")]
public class SetPathCommand
{
    [Option("-p|--path <PATH>", CommandOptionType.SingleValue, Description = "Path to the folder with input and output files.")]
    public string Path { get; set; }

    private void OnExecute()
    {
        try
        {
            Environment.SetEnvironmentVariable("LAB_PATH", Path, EnvironmentVariableTarget.User);
            Console.WriteLine($"Successfully set LAB_PATH to {Path}");

        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error setting LAB_PATH: {ex.Message}");
        }
    }
}
