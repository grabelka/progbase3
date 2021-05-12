using System;
using Terminal.Gui;

public class OpenAnswerDialog : Dialog
{
    public bool deleted;
    public bool updated;
    protected Answer answer;
    Label inputId;
    TextView textView;
    Label created;
    Label pinned;
    public OpenAnswerDialog()
    {
        this.Title = "Open answer";
        Button backBtn = new Button(2, 0, "Back");
        backBtn.Clicked += OnBackButtonClicked;
        this.Add(backBtn);
        this.Add(backBtn);
        Label iLabel = new Label(2, 2, "Question id: ");
        this.inputId = new Label(20, 2, "");
        this.Add(iLabel, inputId);
        Label vLabel = new Label(2, 4, "Text: ");
        this.textView = new TextView()
        {
            X = 20,
            Y = 4,
            Width = Dim.Fill(5),  // margin width
            Height = Dim.Percent(50),
            Text = "",
            ReadOnly = true
        };
        this.Add(vLabel, textView);
        Label cLabel = new Label(2, 10, "Created : ");
        this.created = new Label(20, 10, "");
        this.Add(cLabel, created);
        Label pLabel = new Label(2, 8, "Is pinned: ");
        this.pinned = new Label(20, 8, "");
        this.Add(pLabel, pinned);
        Button edit = new Button(30, 14, "Edit");
        edit.Clicked += OnEdit;
        this.Add(edit);
        Button delete = new Button(40, 14, "Delete");
        delete.Clicked += OnDelete;
        this.Add(delete);
    }
    void OnEdit()
    {
        EditAnswerDialog dialog = new EditAnswerDialog();
        dialog.SetAnswer(this.answer);
        Application.Run(dialog);
        if (!dialog.canceled)
        {
            Answer updatedAnswer = dialog.GetAnswer();  
            this.updated = true;
            this.SetAnswer(updatedAnswer); 
        }
    }
    void OnDelete()
    {
        int index = MessageBox.Query("Delete answer", "Are you sure?", "No", "Yes");
        if (index == 1)
        {
            this.deleted = true;
            Application.RequestStop();
        }
    }
    public void SetAnswer(Answer answer)
    {
        this.answer = answer;
        this.inputId.Text = answer.questionId.ToString();
        this.textView.Text = answer.text.ToString();
        this.created.Text = answer.created.ToShortDateString();
        this.pinned.Text = answer.questionId.ToString();
    }
    public Answer GetAnswer()
    {
        return this.answer;
    }
    private void OnBackButtonClicked()
    {
        Application.RequestStop();
    }
}