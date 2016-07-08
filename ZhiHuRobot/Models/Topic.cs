using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiHuRobot.Library;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace ZhiHuRobot.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Question> Questions { get; set; }

        public Topic()
        {
            this.Questions = new List<Question>();
        }

        public async Task Init()
        {
            string url = Config.TopicUrl(this.Id, 1);

            HtmlDocument doc = await HttpUtils.Html(url);
            var nameNode = doc.GetElementByClassName("h3", "zm-topic-side-organize-title");
            if (nameNode != null) this.Name = nameNode.InnerText.Trim();

            HtmlNode descriptionNode = doc.GetElementbyId("zh-topic-desc");
            if (descriptionNode != null) this.Description = descriptionNode.InnerText.Trim();
        }

        public Topic InitSync()
        {
            string url = Config.TopicUrl(this.Id, 1);

            HtmlDocument doc = HttpUtils.HtmlSync(url);
            var nameNode = doc.GetElementByClassName("h3", "zm-topic-side-organize-title");
            if (nameNode != null) this.Name = nameNode.InnerText.Trim();

            HtmlNode descriptionNode = doc.GetElementbyId("zh-topic-desc");
            if (descriptionNode != null) this.Description = descriptionNode.InnerText.Trim();

            return this;
        }
        public async Task InitQuestions(int page = 1)
        {
            this.Questions = await GetQuestions(this.Id, page);
        }

        public Topic InitQuestionsSync(int page = 1)
        {

            this.Questions = GetQuestionsSync(this.Id, page);
            return this;
        }


        public static async Task<List<Question>> GetQuestions(int topicId, int page = 1)
        {
            string url = Config.TopicUrl(topicId, page);
            Console.WriteLine("GetQuestions({0}, {1}): {2}", topicId, page, url);
            HtmlDocument doc = null;
            List<Question> retQuestions = new List<Question>();
            try
            {
                doc = await HttpUtils.Html(url);
                retQuestions = doc.GetElementsByClassName("a", "question_link")
                .Select(node =>
                {
                    var hrefValue = node.GetAttributeValue("href", "");
                    return new Question()
                    {
                        Id = Convert.ToInt32(Regex.Match(hrefValue, @"(\d+)").Groups[0].Value)
                    };
                })
                .ToList();


                retQuestions.AddRange(await GetQuestions(topicId, ++page));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return retQuestions;
        }

        public static List<Question> GetQuestionsSync(int topicId, int page = 1)
        {
            string url = Config.TopicUrl(topicId, page);
            Console.WriteLine("GetQuestionsSync({0}, {1}): {2}", topicId, page, url);
            HtmlDocument doc = null;
            List<Question> retQuestions = new List<Question>();
            try
            {
                doc = HttpUtils.HtmlSync(url);
                retQuestions = doc.GetElementsByClassName("a", "question_link")
                .Select(node =>
                {
                    var hrefValue = node.GetAttributeValue("href", "");
                    return new Question()
                    {
                        Id = Convert.ToInt32(Regex.Match(hrefValue, @"(\d+)").Groups[0].Value)
                    };
                })
                .ToList();


                retQuestions.AddRange(GetQuestionsSync(topicId, ++page));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return retQuestions;
        }
    }
}
