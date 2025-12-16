using ContentVersionsPOC.Data;
using ContentVersionsPOC.Data.Enums;
using ContentVersionsPOC.Data.Models;
using ContentVersionsPOC.Data.Summaries;
using ContentVersionsPOC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ContentVersionsPOC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    IContentRepository _contentRepository;
    ContentVersionsPOCContext _context;

    public NewsController(IContentRepository contentRepository, ContentVersionsPOCContext context)
    {
        _contentRepository = contentRepository;
        _context = context;
    }

    [HttpPost]
    public IActionResult AddNewsContent([FromQuery] string heading)
    {
        var newsContent = new NewsContent(Guid.NewGuid(), Language.SV)
        {
            Heading = heading,
            Text = "News text"
        };
        var createdContent = _contentRepository.Create(newsContent, Language.SV);
        return Ok(createdContent);
    }

    [HttpPut]
    public IActionResult UpdateNewsContent([FromQuery]Guid contentId, [FromBody]Dictionary<string, string?> updates)
    {
        var updatedContent = _contentRepository.Update<NewsContent>(contentId, Language.SV, updates);

        // Can be used like this =>
        //_contentRepository.Update<EventContent>(contentId, updates);
        //_contentRepository.Update<ContentVersion>(contentId, updates);

        return Ok(new ContentSummary(updatedContent));
    }

    private IActionResult POC_API()
    {
        //Create
        var firstVersion = new NewsContent(Guid.NewGuid(), Language.SV)
        {
            Heading = "News heading 1",
            Text = "News text 1",
            Language = Language.SV
        };
        var createdContent = _contentRepository.Create(firstVersion, Language.SV);

        //Update
        var updatedContent = new NewsContent(Guid.NewGuid(), Language.SV)
        {
            Heading = "News heading 2",
            Text = "News text 2",
            Language = Language.SV
        };

        _contentRepository.Update(createdContent.ContentId, Language.SV, updatedContent);

        //Delete
        _contentRepository.Delete(updatedContent.ContentId);

        return Ok();
    }

    [HttpGet("all")]
    public IActionResult GetNewsContent()
    {
        var news = _context.Content.ToList();

        //var news = _contentRepository
        //    .QueryActiveVersions<NewsContent>(Language.SV)
        //    .Select(x => new ContentSummary(x))
        //    .ToList();

        return Ok(news);
    }

    //[HttpGet("latest")]
    //public IActionResult GetLatestNews()
    //{
    //    var fromDate = DateTime.UtcNow.AddMinutes(-1);
    //    var latestNews = _contentRepository
    //        .QueryActiveVersions<NewsContent>(Language.SV)
    //        //.Where(x => x.Created > fromDate)
    //        .Select(x => new ContentSummary(x))
    //        .ToList();

    //    return Ok(latestNews);
    //}
}
