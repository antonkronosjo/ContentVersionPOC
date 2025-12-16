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
                LanguageBranch = LanguageBranchEnum.SV,
                StartPublish = DateTime.Now
            };
            var versionedContent = _contentRepository.Create(newsContent, LanguageBranchEnum.SV);
            return Ok(versionedContent);
        }

        [HttpPut]
        public IActionResult UpdateNewsContent([FromQuery]Guid contentId, [FromBody]Dictionary<string, string?> updates)
        {
            var updatedContent = _contentRepository.Update<NewsContent>(contentId, LanguageBranchEnum.SV, updates);

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
                LanguageBranch = LanguageBranchEnum.SV,
                StartPublish = DateTime.Now
            };
            var createdContent = _contentRepository.Create(firstVersion, LanguageBranchEnum.SV);

            //Update
            var updatedContent = new NewsContent()
            {
                ContentId = createdContent.ContentId, //<-- Same content id as first version
                VersionId = Guid.NewGuid(),
                Heading = "News heading",
                Text = "News text",
                LanguageBranch = LanguageBranchEnum.SV,
                StartPublish = DateTime.Now
            };

            _contentRepository.Update(createdContent.ContentId, LanguageBranchEnum.SV, updatedContent);

            //Delete
            _contentRepository.Delete(updatedContent.ContentId);

            return Ok();
        }

        [HttpGet("all")]
        public IActionResult GetNewsContent()
        {
            var news = _contentRepository
                .QueryActiveVersion<NewsContent>(LanguageBranchEnum.SV)
                .ToList();

            return Ok(news);
        }

        [HttpGet("latest")]
        public IActionResult GetLatestNews()
        {
            var fromDate = DateTime.UtcNow.AddMinutes(-1);
            var latestNews = _contentRepository
                .QueryActiveVersion<NewsContent>(LanguageBranchEnum.SV)
                .Where(x => x.Created > fromDate)
                .ToList();

            return Ok(latestNews);
        }
    }
}
