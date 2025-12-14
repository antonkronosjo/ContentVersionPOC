using ContentVersionsPOC.Data;
using ContentVersionsPOC.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ContentVersionsPOC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        IContentRepository _contentRepository;

        public NewsController(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        [HttpPost]
        public IActionResult AddNewsContent([FromBody] string name)
        {
            var newsContent = new NewsContent()
            {
                VersionId = Guid.NewGuid(),
                NewsPreamble = name
            };
            var versionedContent = _contentRepository.Create(newsContent);
            return Ok(versionedContent.Id);
        }


        [HttpGet("all")]
        public IActionResult GetNewsContent()
        {
            var news = _contentRepository
                .QueryCurrentVersion<NewsContent>()
                .ToList()
                .Select(x => x.VersionId)
                .ToList();

            return Ok(news);
        }

        [HttpGet("latest")]
        public IActionResult GetLatestNews()
        {
            var fromDate = DateTime.UtcNow.AddMinutes(-1);
            var latestNews = _contentRepository
                .QueryCurrentVersion<NewsContent>()
                .Where(x => x.Content.Created > fromDate)
                .Select(x => x.VersionId)
                .ToList();

            return Ok(latestNews);
        }
    }
}
