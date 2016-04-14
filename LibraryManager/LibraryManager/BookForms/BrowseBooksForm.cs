using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LibraryManager.Entities;
using LibraryManager.Repositories;
using System.Configuration;
using System.Text.RegularExpressions;

namespace LibraryManager
{
    public partial class BrowseBooksForm : Form
    {
        private readonly IBookRepository _bookRepository;
        public int _bookId;

        public BrowseBooksForm()
        {
            // IP: Можливо було б доцільним винести IMemberRepository _memberRepository та IBookRepository _bookRepository з конкретних класів форм у спільний предок 
            _bookRepository = new SqlBookRepository(ConfigurationManager.ConnectionStrings["dbLibrary"].ConnectionString);
            
            InitializeComponent();

            IEnumerable<BookInfo> bookList = _bookRepository.SelectBooksInfo(cmbFilterBooks.Text, txtSearchBooks.Text);
            dataGridViewBooks.ClearSelection();
            dataGridViewBooks.DataSource = bookList;
        }

        private void btnSearchBooks_Click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(txtSearchBooks.Text, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Wrong data in search");
            }
            else
            {
                IEnumerable<BookInfo> bookList = _bookRepository.SelectBooksInfo(cmbFilterBooks.Text, txtSearchBooks.Text);
                dataGridViewBooks.ClearSelection();
                dataGridViewBooks.DataSource = bookList;
            }
        }

        private void btnGetId_Click(object sender, EventArgs e)
        {
            int selectedRowCount = dataGridViewBooks.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount == 1)
            {
                string name = dataGridViewBooks.SelectedRows[0].Cells[0].Value.ToString();
                string author = dataGridViewBooks.SelectedRows[0].Cells[1].Value.ToString();
                BookId book = _bookRepository.GetBookIdByNameAndAuthor(name, author);
                this._bookId = book.Id;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Select 1 row first");
            }
        }

        public int StartSearch()
        {
            return _bookId;
        }

        private void btnCancelBrowesBooksBooked_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
