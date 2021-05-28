using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace ClassLibrary
{
    public static class Import
    {
        public static void Read(string file1, string file2, QuestionRepository questionRepository, AnswerRepository answerRepository)
        {
            List<ExportQuestion> questions = ReadQuestions(file1).questions;
            foreach(ExportQuestion item in questions)
            {
                if(questionRepository.GetById(item.id) == null)
                {
                    questionRepository.Insert(new Question(item.id, item.userId, item.title, item.text, item.created, null, null));
                }
            }
            List<ExportAnswer> answers = ReadAnswers(file2).answers;
            foreach(ExportAnswer item in answers)
            {
                if(answerRepository.GetById(item.id) == null)
                {
                    answerRepository.Insert(new Answer(item.id, item.questionId, item.text, item.created, item.pinned, null));
                }
                else if (answerRepository.GetById(item.id).pinned == "no" && item.pinned == "yes")
                {
                    answerRepository.Update(new Answer(item.id, item.questionId, item.text, item.created, item.pinned, null));
                }
            }
        }
        static RootQuestions ReadQuestions(string file)
        {
            if (!File.Exists(file)) return null;
            string data = File.ReadAllText(file);
            XmlSerializer ser = new XmlSerializer(typeof(RootQuestions));
            StringReader reader = new StringReader(data);
            RootQuestions questions = (RootQuestions)ser.Deserialize(reader);
            reader.Close();
            return questions;
        }
        static RootAnswers ReadAnswers(string file)
        {
            if (!File.Exists(file)) return null;
            string data = File.ReadAllText(file);
            XmlSerializer ser = new XmlSerializer(typeof(RootAnswers));
            StringReader reader = new StringReader(data);
            RootAnswers answers = (RootAnswers)ser.Deserialize(reader);
            reader.Close();
            return answers;
        }
    }
}