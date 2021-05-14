using System;

namespace ClassLibrary
{
    public class ExportQuestion
    {
        public int id;
        public int userId;
        public string title;
        public string text;
        public DateTime created;
        public ExportQuestion(Question q)
        {
            this.id = q.id;
            this.userId = q.userId;
            this.title = q.title;
            this.text = q.text;
            this.created = q.created;
        }
        public ExportQuestion()
        {}
    }
}