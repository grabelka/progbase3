using System;
using ClassLibrary;
using Terminal.Gui;

public class OpenUserDialog : Dialog
{
    public bool deleted;
    public bool updated;
    protected User user;
    Label inputName;
    Label inputLogin;
    Label inputPass;
    Label isModerator;
    public OpenUserDialog()
    {
        this.Title = "Open user";
        Button backBtn = new Button(2, 0, "Back");
        backBtn.Clicked += OnBackButtonClicked;
        this.Add(backBtn);
        Label nLabel = new Label(2, 2, "Name: ");
        this.inputName = new Label(20, 2, "");
        this.Add(nLabel, inputName);
        Label lLabel = new Label(2, 4, "Login: ");
        this.inputLogin = new Label(20, 4, "");
        this.Add(lLabel, inputLogin);
        Label pLabel = new Label(2, 6, "Password: ");
        this.inputPass = new Label(20, 6, "");
        this.Add(pLabel, inputPass);
        Label mLabel = new Label(2, 8, "Is moderator: ");
        this.isModerator = new Label(20, 8, "");
        this.Add(mLabel, isModerator);
        Button edit = new Button(30, 14, "Edit");
        edit.Clicked += OnEdit;
        this.Add(edit);
        Button delete = new Button(40, 14, "Delete");
        delete.Clicked += OnDelete;
        this.Add(delete);
    }
    void OnEdit()
    {
        EditUserDialog dialog = new EditUserDialog();
        dialog.SetUser(this.user);
        Application.Run(dialog);
        if (!dialog.canceled)
        {
            User updatedUser = dialog.GetUser();  
            this.updated = true;
            this.SetUser(updatedUser);
        }
    }
    void OnDelete()
    {
        int index = MessageBox.Query("Delete user", "Are you sure?", "No", "Yes");
        if (index == 1)
        {
            this.deleted = true;
            Application.RequestStop();
        }
    }
    public void SetUser(User user)
    {
        this.user = user;
        this.inputName.Text = user.name;
        this.inputLogin.Text = user.login;
        this.inputPass.Text = user.password;
        this.isModerator.Text = user.isModerator;
    }
    public User GetUser()
    {
        return this.user;
    }
    private void OnBackButtonClicked()
    {
        Application.RequestStop();
    }
}