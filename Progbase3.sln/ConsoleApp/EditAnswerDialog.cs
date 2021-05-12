using System;
using Terminal.Gui;

public class EditAnswerDialog : CreateAnswerDialog
{
    public EditAnswerDialog()
    {
        this.Title = "Edit answer";
    }
    public void SetAnswer(Answer answer)
    {
        this.inputId.Text = answer.questionId.ToString();
        this.textView.Text = answer.text.ToString();
        this.pinned.Checked = (answer.pinned == "yes") ? true : false;
    }    
}