using System;
using System.Windows.Forms;
using LibraryManager.Repositories;
using LibraryManager.Entities;
using System.Configuration;
using LibraryManager.Code;

namespace LibraryManager
{
    public partial class SetFineForm : Form
    {    
        private readonly IBookedRepository _bookedRepository;
        private Booked _booked;

        public SetFineForm(Booked booked)
        {
            // IP: Можливо було б доцільним винести IMemberRepository _memberRepository та IBookRepository _bookRepository з конкретних класів форм у спільний предок 
            _bookedRepository = new SqlBookedRepository(ConfigurationManager.ConnectionStrings["dbLibrary"].ConnectionString);
            _booked = booked;

            InitializeComponent();
            
            txtDateBooked.Text = _booked.dateBooked.ToString();
            txtDueDateBooked.Text = _booked.dueBooked.ToString();
            if (DateTime.Now > _booked.dueBooked)
            {
                txtDaysOverBooked.Text = (DateTime.Now - _booked.dueBooked).TotalDays.ToString();
            }
            else
            {
                txtDaysOverBooked.Text = "0";
            }
        }

        private void btnCancelBooked_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddFineBooked_Click(object sender, EventArgs e)
        {
            if(txtResultBooked.Text == "")
            {
                MessageBox.Show("Result field is empty");
            }
            else
            {                                                
                _booked.fine = Convert.ToDecimal(txtResultBooked.Text);                

                _bookedRepository.SetBookedFine(_booked);

                this.Close();                
            }
        }

        private void btnCountBooked_Click(object sender, EventArgs e)
        {
            double count;
            if (txtFineBooked.Text == "" || !(Double.TryParse(txtFineBooked.Text, out count)))
            {
                MessageBox.Show("Fine field is empty or wrong data");
            }
            else
            {
                txtResultBooked.Text = ((DateTime.Now - _booked.dueBooked).TotalDays * count).ToString();
            }
        }
    }
}
