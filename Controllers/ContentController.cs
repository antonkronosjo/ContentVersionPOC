using ContentVersionsPOC.Data;
using ContentVersionsPOC.Data.Enums;
using ContentVersionsPOC.Data.Models;
using ContentVersionsPOC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContentVersionsPOC.Controllers;

[ApiController]
[Route("api/content")]
public class ContentController : ControllerBase
{
    IContentRepository _contentRepository;

    public ContentController(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    [HttpPost("/news/create")]
    public IActionResult AddNewsContent([FromQuery] string heading)
    {
        var newsContent = new NewsContent(Guid.NewGuid(), Language.SV)
        {
            Heading = heading,
            Text = "News text"
        };
        var createdContent = _contentRepository.Create(newsContent);
        return Ok(createdContent);
    }

    [HttpPost("/events/create")]
    public IActionResult AddEventContent([FromQuery] string heading)
    {
        var newsContent = new EventContent(Guid.NewGuid(), Language.SV)
        {
            Heading = heading,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now
        };
        var createdContent = _contentRepository.Create(newsContent);
        return Ok(createdContent);
    }

    [HttpPut("update")]
    public IActionResult UpdateNewsContent([FromQuery]Guid contentId, [FromBody]Dictionary<string, string?> updates)
    {
        var updatedContent = _contentRepository.Update<NewsContent>(contentId, Language.SV, updates);

        // Can be used like this =>
        //_contentRepository.Update<EventContent>(contentId, updates);
        //_contentRepository.Update<ContentVersion>(contentId, updates);

        return Ok(updatedContent);
    }

    private IActionResult POC_API()
    {
        //Create
        var firstVersion = new NewsContent(Guid.NewGuid(), Language.SV)
        {
            Heading = "News heading 1",
            Text = "News text 1"
        };
        var createdContent = _contentRepository.Create(firstVersion);

        //Update
        var updatedContent = new NewsContent(Guid.NewGuid(), Language.SV)
        {
            Heading = "News heading 2",
            Text = "News text 2"
        };

        _contentRepository.Update(createdContent.ContentId, updatedContent);

        //Delete
        _contentRepository.Delete(updatedContent.ContentId);

        return Ok();
    }

    [HttpGet("all")]
    public IActionResult GetNewsContent()
    {
        var news = _contentRepository
            .QueryActiveVersions<Content>(Language.SV)
            .Include(x => x.ContentRoot)
            .ToList();

        return Ok(news);
    }

    [HttpGet("latest")]
    public IActionResult GetLatestNews()
    {
        var fromDate = DateTime.UtcNow.AddMinutes(-1);
        var latestNews = _contentRepository
            .QueryActiveVersions<NewsContent>(Language.SV)
            .Where(x => x.Created > fromDate)
            .Include(x => x.ContentRoot)
            .ToList();

        return Ok(latestNews);
    }
}
