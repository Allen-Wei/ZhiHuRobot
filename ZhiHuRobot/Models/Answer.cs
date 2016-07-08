using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZhiHuRobot.Models
{
    public class Answer
    {

        public int VoteCount { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }

        public List<string> DownloadImages()
        {
            return Regex.Matches(this.Content, @"https://pic4\.zhimg\.com/[\w_\d]+_r.\w+")
              .Cast<Match>()
              .AsParallel()
              .Select(match =>
              {
                  var img = match.Value;
                  var fileName = String.Format(@"{0}\{1}", Environment.CurrentDirectory, Path.GetFileName(img));
                  Console.WriteLine(fileName);
                  var client = new WebClient();
                  client.DownloadFile(img, fileName);
                  return fileName;
              })
              .ToList();
        }

    }
}
