using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using project_aurora.Services;

namespace project_aurora.Pages
{
    public class logged_compilerModel : PageModel
    {
        private readonly Judge0Service _judge0Service;

        [BindProperty]
        public string SourceCode { get; set; } = "";

        [BindProperty]
        public int LanguageId { get; set; } = 46;

        public string Output { get; private set; } = "";

        public logged_compilerModel(Judge0Service judge0Service)
        {
            _judge0Service = judge0Service;
        }

        public async Task OnPostAsync()
        {
            try
            {
                var result = await _judge0Service.SubmitAsync(
                    SourceCode,
                    LanguageId);

                Output = BuildOutput(result);
            }
            catch (Exception ex)
            {
                Output = $"Request failed: {ex.Message}";
            }
        }

        private static string BuildOutput(project_aurora.Models.Judge0Result result)
        {
            var sections = new List<string>();

            if (!string.IsNullOrWhiteSpace(result.Status?.Description))
                sections.Add($"Status: {result.Status.Description}");

            if (!string.IsNullOrWhiteSpace(result.Stdout))
                sections.Add(result.Stdout.TrimEnd());

            if (!string.IsNullOrWhiteSpace(result.CompileOutput))
                sections.Add($"Compilation errors:\n{result.CompileOutput.TrimEnd()}");

            if (!string.IsNullOrWhiteSpace(result.Stderr))
                sections.Add($"Runtime errors:\n{result.Stderr.TrimEnd()}");

            if (!string.IsNullOrWhiteSpace(result.Message))
                sections.Add($"Message:\n{result.Message}");

            if (sections.Count == 0)
                sections.Add("Execution completed with no output.");

            return string.Join("\n\n", sections);
        }
    }
}