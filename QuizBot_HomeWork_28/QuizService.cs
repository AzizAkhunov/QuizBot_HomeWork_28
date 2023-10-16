using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace QuizBot_HomeWork_28
{
    public class QuizService
    {

        private string PATH = @"";
        public List<Question> questions = new List<Question>();

        public List<Question> GetQuestions()
        {
            using (StreamReader reader = new StreamReader(PATH))
            {
                string json = reader.ReadToEnd();
                questions = JsonConvert.DeserializeObject<List<Question>>(json);
                return questions.Where(x => x.isTest == 1).ToList();
            }
        }
        public IList<string> GetSubjects()
        {
            var questions = GetQuestions();
            return questions.Select(x => x.subject.ToLower()).Distinct().ToList();
        }
        public Question SetOptions(Question savol)
        {
            string questionText = savol.question;
            questionText = ExtractQuestionText(questionText);
            List<string> answerOptions = Regex.Matches(savol.question, @"\(([A-Z])\) ([^\(\)]+)").Cast<Match>()
                    .Select(m => m.Groups[2].Value).ToList();
            savol.question = questionText;
            savol.options = answerOptions;

            return savol;
        }

        private string ExtractQuestionText(string text)
        {
            int openingParenIndex = text.IndexOf('(');
            if (openingParenIndex != -1)
            {
                return text.Substring(0, openingParenIndex).Trim();
            }
            else
            {
                return text;
            }
        }
        public Question GetQuestionBySubject(string subject)
        {
            var questions = GetQuestions();

            var random = new Random();
            int index = random.Next(questions.Count);
            Question question = questions[index];
            question = SetOptions(question);
            if (question.options == null)
            {
                question = GetQuestionBySubject(subject);
            }

            return question;

        }

    }
}
