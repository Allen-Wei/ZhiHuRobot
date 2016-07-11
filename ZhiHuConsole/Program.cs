using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhiHuRobot.Models;

namespace ZhiHuConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var questions = Topic.GetQuestionsSync(19590712, 46);
            var last = questions.Last();
            last.InitAnswersSync();
            last.Answers.ForEach(a => a.DownloadImages());
        }
    }
}
