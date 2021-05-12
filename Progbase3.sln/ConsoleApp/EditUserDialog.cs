using System;
using Terminal.Gui;

public class EditUserDialog : CreateUserDialog
{
    public EditUserDialog()
    {
        this.Title = "Edit user";
    }
    public void SetUser(User user)
    {
        this.inputLogin.Text = user.login.ToString();
        this.inputName.Text = user.name.ToString();
        this.inputPass.Text = user.password.ToString();
        bool mod = (user.isModerator == "yes") ? true : false;
        this.isModerator.Checked = mod;
    }    
}