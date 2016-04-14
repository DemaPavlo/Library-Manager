using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManager.Repositories;
using LibraryManager.Entities;
using System.Configuration;
using System.IO;

namespace LibraryManager
{
    public partial class EditBookForm : Form
    {
        private Book _bookToEdit;        
        public readonly IBookRepository _bookRepository;
        public EditBookForm(Book bookToEdit)
        {
            this._bookToEdit = bookToEdit;
            
            // IP: Можливо було б доцільним винести IMemberRepository _memberRepository та IBookRepository _bookRepository з конкретних класів форм у спільний предок 
            _bookRepository = new SqlBookRepository(ConfigurationManager.ConnectionStrings["dbLibrary"].ConnectionString);

            InitializeComponent();

            txtNameBook.Text = _bookToEdit.Name;
            txtGenreBook.Text = _bookToEdit.Genre;
            txtAuthorBook.Text = _bookToEdit.Author;
            txtYearBook.Text = _bookToEdit.YearPublished.ToString();
            txtPublisherBook.Text = _bookToEdit.Publisher;
            txtStatus.Text = _bookToEdit.Status.ToString();
            if (_bookToEdit.Photo != null && _bookToEdit.Photo.Length > 0)
            {                
                pcBPhotoBook.Image = ImageManager.ByteArrayToImage(_bookToEdit.Photo);                                    
            }
        }
        private void btnBrowsePhotoBook_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "All Files|*.*|JPEGs|*.jpg|Bitmaps|*.bmp";
            file.FilterIndex = 2;
            if (file.ShowDialog() == DialogResult.OK)
            {
                
                pcBPhotoBook.Image = Image.FromFile(file.FileName);
            }
        }

        private void btnCancelBook_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            txtStatus.Text = "";
        }

        private void btnEditBook_Click(object sender, EventArgs e)
        {
            int year;
            if (txtNameBook.Text == "" || !(Int32.TryParse(txtYearBook.Text, out year)))
            {
                MessageBox.Show("One of * fields are empty or wrong data");
            }

            else
            {
                _bookToEdit.Name = txtNameBook.Text;
                _bookToEdit.Author = txtAuthorBook.Text;
                _bookToEdit.Genre = txtGenreBook.Text;
                _bookToEdit.Publisher = txtPublisherBook.Text;
                if (pcBPhotoBook.Image != null)
                {
                    _bookToEdit.Photo = ImageManager.ImageToByteArray(pcBPhotoBook.Image);
                }
                _bookToEdit.YearPublished = year;
                _bookToEdit.Status = 1;

                _bookRepository.EditBook(_bookToEdit);                

                this.Close();
            }
        }
    } 
}
