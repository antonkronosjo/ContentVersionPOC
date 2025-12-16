using ContentVersionsPOC.Data.Enums;
using ContentVersionsPOC.Data.Models;
using ContentVersionsPOC.Data.Services;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult AddNewsContent([FromQuery] string heading)
        {
            var newsContent = new NewsContent()
            {
                VersionId = Guid.NewGuid(),
                Heading = heading,
                Text = "News text",
                LanguageBranch = LanguageBranch.SV,
                StartPublish = DateTime.Now
            };
            var versionedContent = _contentRepository.Create(newsContent);
            return Ok(versionedContent);
        }

        [HttpPut]
        public IActionResult UpdateNewsContent([FromQuery]Guid contentId, [FromBody]Dictionary<string, string?> updates)
        {
            var updatedContent = _contentRepository.Update<NewsContent>(contentId, updates);

            // Can be used like this =>
            //_contentRepository.Update<EventContent>(contentId, updates);
            //_contentRepository.Update<ContentVersion>(contentId, updates);

            return Ok(updatedContent);
        }

        private IActionResult POC_API()
        {
            //Create
            var firstVersion = new NewsContent()
            {
                VersionId = Guid.NewGuid(),
                Heading = "News heading",
                Text = "News text",
                LanguageBranch = LanguageBranch.SV,
                StartPublish = DateTime.Now
            };
            var createdContent = _contentRepository.Create(firstVersion);

            //Update
            var updatedContent = new NewsContent()
            {
                ContentId = createdContent.Id, //<-- Same content id as first version
                VersionId = Guid.NewGuid(),
                Heading = "News heading",
                Text = "News text",
                LanguageBranch = LanguageBranch.SV,
                StartPublish = DateTime.Now
            };

            _contentRepository.Update(createdContent.Id, updatedContent);

            //Delete
            _contentRepository.Delete(updatedContent.ContentId);

            return Ok();
        }

        [HttpGet("all")]
        public IActionResult GetNewsContent()
        {
            var news = _contentRepository
                .QueryActiveVersion<NewsContent>(LanguageBranch.SV)
                .ToList();

            return Ok(news);
        }

        [HttpGet("latest")]
        public IActionResult GetLatestNews()
        {
            var fromDate = DateTime.UtcNow.AddMinutes(-1);
            var latestNews = _contentRepository
                .QueryActiveVersion<NewsContent>(LanguageBranch.SV)
                .Where(x => x.Content.Created > fromDate)
                .ToList();

            return Ok(latestNews);
        }
    }
}
