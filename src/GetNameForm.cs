using System;
using System.Windows.Forms;


namespace Tetris
{
    partial class GetNameForm : Form
    {
        public GetNameForm()
        {
            InitializeComponent();

            textBoxName.Text = "player1";
        }

        public event EventHandler<NameEventArgs> NameChanged;

        private void OnButtonStartClick(object sender, EventArgs e)
        {
            NameChanged?.Invoke(this, new NameEventArgs(textBoxName.Text));
            MainForm.Timer.Start();
            Close();
        }

        private void OnButtonExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnTextBoxNameKeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxName.Text.Length >= 16)
            {
                e.Handled = true;
            }

            base.OnKeyPress(e);
        }
    }

    internal class NameEventArgs : EventArgs
    {
        internal readonly string Name;

        internal NameEventArgs(string name)
        {
            Name = name;
        }
    }
}
