
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using project_aurora.Models;
using project_aurora.Services;

namespace project_aurora.Pages;

public class TestJudge0Model : PageModel
{
    private readonly Judge0Service _judge0Service;

    [BindProperty]
    public string SourceCode { get; set; } = "";

    [BindProperty]
    public int LanguageId { get; set; } = 46;

    public Judge0Result? Result { get; private set; }
    public string? Error { get; private set; }

    public TestJudge0Model(Judge0Service judge0Service)
    {
        _judge0Service = judge0Service;
    }

    public async Task OnPostAsync()
    {
        try
        {
            Result = await _judge0Service.SubmitAsync(
                SourceCode,
                LanguageId);
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }
}
