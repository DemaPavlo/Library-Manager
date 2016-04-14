using System;
using System.Windows.Forms;
using LibraryManager.Repositories;
using LibraryManager.Entities;
using System.Configuration;
using LibraryManager.Code;

namespace LibraryManager
{
    public partial class AddBookedForm : Form
    {
        public IBookedRepository _bookedRepository;

        public AddBookedForm()
        {
            // IP: Можливо було б доцільним винести IMemberRepository _memberRepository та IBookRepository _bookRepository з конкретних класів форм у спільний предок 
            _bookedRepository = new SqlBookedRepository(ConfigurationManager.ConnectionStrings["dbLibrary"].ConnectionString);
            
            InitializeComponent();

            txtDateBooked.Text = monthCalendarBooked.SelectionStart.ToString();
            txtDueDateBooked.Text = monthCalendarBooked.SelectionStart.ToString();
        }

        private void btnCancelBooked_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddBooked_Click(object sender, EventArgs e)
        {
            if(txtMemberIdBooked.Text == "" || txtDueDateBooked.Text == "" || txtDateBooked.Text == "" || txtBookIdBooked.Text == "")
            {
                MessageBox.Show("One of * fields are empty");
            }
            else
            {
                Booked booked = new Booked();
                booked.MemberId = Convert.ToInt32(txtMemberIdBooked.Text);
                booked.BookId = Convert.ToInt32(txtBookIdBooked.Text);
                booked.dateBooked = Convert.ToDateTime(txtDateBooked.Text);
                booked.dueBooked = Convert.ToDateTime(txtDueDateBooked.Text);
                booked.StatusId = 1;
                booked.fine = 0;
                booked.UserId = CurrentUser.Id;

                _bookedRepository.AddBooked(booked);

                this.Close();
            }
        }

        private void monthCalendarBooked_MouseCaptureChanged(object sender, EventArgs e)
        {
            txtDueDateBooked.Text = monthCalendarBooked.SelectionStart.ToString();
        }

        private void btnBrowseBooksBooked_Click(object sender, EventArgs e)
        {
            BrowseBooksForm frmBrowseBooks = new BrowseBooksForm();
            if(frmBrowseBooks.ShowDialog() == DialogResult.OK)
            {
                txtBookIdBooked.Text = frmBrowseBooks.StartSearch().ToString();
            }
        }

        private void btnBrowseMembersBooked_Click(object sender, EventArgs e)
        {
            BrowseMembersForm frmBrowseMembers = new BrowseMembersForm();
            if (frmBrowseMembers.ShowDialog() == DialogResult.OK)
            {
                txtMemberIdBooked.Text = frmBrowseMembers.StartSearch().ToString();
            }
        }
    }
}
