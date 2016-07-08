using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiHuRobot.Library
{
    public class Config
    {
        public static string TopicUrl(int topicId, int page)
        {
            return String.Format("https://www.zhihu.com/topic/{0}/top-answers?page={1}", topicId, page);
        }
        public static string QuestionUrl()
        {
            return String.Format("https://www.zhihu.com/node/QuestionAnswerListV2");
        }
        public static string QuestionUrl(int questionId)
        {
            return String.Format("https://www.zhihu.com/question/{0}", questionId);
        }
    }
}
