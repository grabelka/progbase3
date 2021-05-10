using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO.Compression;

public static class Export 
{
    public static void Write(List<Question> q, string file)
    {
        List<ExportQuestion> questions = new List<ExportQuestion>();
        foreach(Question item in q)
        {
            questions.Add(new ExportQuestion(item));
        }
        RootQuestions rootQuestions = new RootQuestions()
        {
            questions = questions,
        };

        XmlSerializer ser = new XmlSerializer(typeof(RootQuestions));
        System.IO.StreamWriter writer = new System.IO.StreamWriter(@".\start\questions.xml");
        ser.Serialize(writer, rootQuestions);
        writer.Close();

        List<ExportAnswer> answers = new List<ExportAnswer>();
        foreach(Question item in q)
        {
            if(item.answers != null) answers.Add(new ExportAnswer(item));
        }
        RootAnswers rootAnswers = new RootAnswers()
        {
            answers = answers,
        };

        XmlSerializer ser2 = new XmlSerializer(typeof(RootAnswers));
        System.IO.StreamWriter writer2 = new System.IO.StreamWriter(@".\start\answers.xml");
        ser2.Serialize(writer2, rootAnswers);
        writer2.Close();

        string startPath = @".\start";

        ZipFile.CreateFromDirectory(startPath, file);
    }
}