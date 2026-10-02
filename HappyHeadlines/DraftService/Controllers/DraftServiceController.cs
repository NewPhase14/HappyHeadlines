using System.Diagnostics;
using CoreDomain.Dtos.ArticleDtos;
using DraftService.Service;
using Microsoft.AspNetCore.Mvc;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace DraftService.Controllers;

[ApiController]
[Route("[controller]")]
public class DraftServiceController(TraceContextPropagator traceContextPropagator, IDraftService draftService)
    : ControllerBase
{
    [HttpPost("createDraft", Name = "CreateDraft")]
    public async Task<ActionResult<ArticleResponseDto>> CreateDraft([FromBody] CreateArticleDto createDraftDto)
    {
        // Extract context sent from ArticleService
        var parentContext = traceContextPropagator.Extract(
            default,
            Request,
            (request, key) => request.Headers[key].ToArray());

        Baggage.Current = parentContext.Baggage;

        using var activity = MonitoringService.ActivitySource.StartActivity(
            "DraftService.CreateDraft",
            ActivityKind.Consumer,
            parentContext.ActivityContext);

        var draft = await draftService.CreateDraft(createDraftDto);

        return Ok(draft);
    }

    [HttpGet(Name = "GetAllDrafts")]
    public async Task<ActionResult<List<ArticleResponseDto>>> GetAllDrafts()
    {
        var drafts = await draftService.GetAllDrafts();

        return Ok(drafts);
    }

    [HttpPut(Name = "UpdateDraft")]
    public async Task<ActionResult<ArticleResponseDto>> UpdateDraft([FromBody] UpdateArticleDto updateDraftDto)
    {
        //TODO: If draft is updates from draft to non-draft, it should be deleted from DraftDatabase and inserted into ArticleDatabase.

        var updatedDraft = await draftService.UpdateDraft(updateDraftDto);
        return Ok(updatedDraft);
    }

    [HttpDelete("{id}", Name = "DeleteDraft")]
    public async Task<IActionResult> DeleteDraft(string id)
    {
        await draftService.DeleteDraft(id);

        return Ok("Draft deleted successfully");
    }
}