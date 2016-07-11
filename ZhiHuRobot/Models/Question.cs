using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZhiHuRobot.Library;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZhiHuRobot.Models
{
    public class Question
    {
        public List<Topic> Topics { get; set; }
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public List<Answer> Answers { get; set; }

        public async Task Init()
        {
            var url = Config.QuestionUrl(this.Id);
            Console.WriteLine("Question Init: {0}", url);

            var doc = await HttpUtils.Html(url);
            var titleNode = doc.GetElementbyId("zh-question-title");
            if (titleNode != null) this.Title = titleNode.InnerText.Trim();

            var descriptionNode = doc.GetElementByClassName("div", "zm-editable-content");
            if (descriptionNode != null)
                this.Description = descriptionNode.InnerText.Trim();

            var topicNodes = doc.GetElementsByClassName("a", "zm-item-tag");
            if (topicNodes != null)
            {
                this.Topics = topicNodes.Select(topicNode =>
                {
                    var hrefValue = topicNode.GetAttributeValue("href", "");
                    var topicId = Convert.ToInt32(Regex.Match(hrefValue, @"\d+").Groups[0].Value);
                    return new Topic()
                    {
                        Id = topicId,
                        Name = topicNode.InnerText.Trim()
                    };
                }).ToList();
            }

        }


        public Question InitSync()
        {
            var url = Config.QuestionUrl(this.Id);
            Console.WriteLine("Question Init: {0}", url);

            var doc = HttpUtils.HtmlSync(url);
            var titleNode = doc.GetElementbyId("zh-question-title");
            if (titleNode != null) this.Title = titleNode.InnerText.Trim();

            var descriptionNode = doc.GetElementByClassName("div", "zm-editable-content");
            if (descriptionNode != null)
                this.Description = descriptionNode.InnerText.Trim();

            var topicNodes = doc.GetElementsByClassName("a", "zm-item-tag");
            if (topicNodes != null)
            {
                this.Topics = topicNodes.Select(topicNode =>
                {
                    var hrefValue = topicNode.GetAttributeValue("href", "");
                    var topicId = Convert.ToInt32(Regex.Match(hrefValue, @"\d+").Groups[0].Value);
                    return new Topic()
                    {
                        Id = topicId,
                        Name = topicNode.InnerText.Trim()
                    };
                }).ToList();
            }

            return this;

        }

        public Question InitAnswersSync(int offset = 0)
        {
            this.Answers = GetAnswersSync(this.Id, offset);
            return this;
        }

        public async Task InitAnswers(int offset = 0)
        {
            this.Answers = await GetAnswers(this.Id, offset);
        }

        public static List<Answer> GetAnswersSync(int questionId, int offset)
        {
            var url = Config.QuestionUrl();
            Console.WriteLine("GetAnswersSync({0}, {1}): {2}", questionId, offset, url);

            var data = String.Format("method=next&params={{\"url_token\":{0},\"pagesize\":10,\"offset\":{1}}}", questionId, offset);
            try
            {
                var responseData = HttpUtils.StringSync(url, data);
                var answers = JObject.Parse(responseData)["msg"].Select(htmlPartial =>
                 {
                     HtmlDocument node = new HtmlDocument();
                     node.LoadHtml((string)htmlPartial);

                     Answer answer = new Answer();

                     var contentNode = node.GetElementByClassName("div", "zm-editable-content");
                     if (contentNode != null) answer.Content = contentNode.InnerHtml.Trim();

                     var userNameNode = node.GetElementByClassName("a", "author-link");
                     if (userNameNode != null) answer.UserName = userNameNode.InnerText.Trim();

                     var voteNode = node.GetElementByClassName("span", "count");
                     if (voteNode != null) answer.VoteCount = Convert.ToInt32(voteNode.InnerText);

                     return answer;
                 }).ToList();

                if (answers.Count < 1) return answers;

                answers.AddRange(GetAnswersSync(questionId, offset + 10));
                return answers;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return new List<Answer>();
        }

        public static async Task<List<Answer>> GetAnswers(int questionId, int offset)
        {
            var url = Config.QuestionUrl();
            Console.WriteLine("GetAnswers({0}, {1}): {2}", questionId, offset, url);

            var data = String.Format("method=next&params={{\"url_token\":{0},\"pagesize\":10,\"offset\":{1}}}", questionId, offset);
            try
            {
                var responseData = await HttpUtils.String(url, data);
                var answers = JObject.Parse(responseData)["msg"].Select(htmlPartial =>
                 {
                     HtmlDocument node = new HtmlDocument();
                     node.LoadHtml((string)htmlPartial);

                     Answer answer = new Answer();

                     var contentNode = node.GetElementByClassName("div", "zm-editable-content");
                     if (contentNode != null) answer.Content = contentNode.InnerHtml.Trim();

                     var userNameNode = node.GetElementByClassName("a", "author-link");
                     if (userNameNode != null) answer.UserName = userNameNode.InnerText.Trim();

                     var voteNode = node.GetElementByClassName("span", "count");
                     if (voteNode != null) answer.VoteCount = Convert.ToInt32(voteNode.InnerText);

                     return answer;
                 }).ToList();


                answers.AddRange(GetAnswersSync(questionId, offset + 10));
                return answers;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return new List<Answer>();
        }
    }
}
