using System;
using ClassLibrary;
using Terminal.Gui;

public class EditQuestionDialog : CreateQuestionDialog
{
    public EditQuestionDialog()
    {
        this.Title = "Edit question";
    }
    public void SetQuestion(Question question)
    {
        this.inputUser.Text = question.userId.ToString();
        this.inputTitle.Text = question.title.ToString();
        this.textView.Text = question.text.ToString();
    }    
}