using System;
using System.Collections.Generic;
using System.Transactions;
using LibraryManager.Entities;
using System.Data.SqlClient;
using System.Data;

namespace LibraryManager.Repositories
{
    public class SqlBookedRepository: IBookedRepository
    {
        private string _connectionString;
        private const string spAddBooked = "spAddBooked";
        private const string spDeleteBooked = "spDeleteBooked";
        private const string spSetBookedFine = "spSetBookedFine";

        public SqlBookedRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<BookedInfo> SelectInfo(string filterName, string filterValue)
        {
            string bookedQuery;
            if (filterName == "")
            {
                bookedQuery = "SELECT b.BookedId, m.PassportId, book.Name, b.DateBooked, b.DueDate FROM tblBooked b INNER JOIN tblMember m ON b.MemberId = m.Id INNER JOIN tblBook book ON b.BookId = book.Id WHERE b.StatusId = 1";
            }
            else
            {
                bookedQuery = String.Format("SELECT b.BookedId, m.PassportId, book.Name, b.DateBooked, b.DueDate FROM tblBooked b INNER JOIN tblMember m ON b.MemberId = m.Id INNER JOIN tblBook book ON b.BookId = book.Id WHERE [{0}] LIKE '%{1}%' AND b.StatusId = 1", filterName, filterValue);
            }

            List<BookedInfo> bookedList = new List<BookedInfo>();
            BookedInfo booked;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = bookedQuery;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) 
                        {
                            booked = new BookedInfo();
                            booked.BookedId = (int)reader["BookedId"];
                            booked.MemberPassport = (string)reader["PassportId"];
                            booked.BookName = (string)reader["Name"];
                            booked.DateBooked = ((DateTime)reader["DateBooked"]).ToString();
                            booked.DueDate = ((DateTime)reader["DueDate"]).ToString();
                            bookedList.Add(booked);
                        }
                    }
                }

                return bookedList;
            }
        }
        public void AddBooked(Booked bookedToAdd)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                try
                {
                    using (TransactionScope tran1 = new TransactionScope())
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = spAddBooked;

                            command.Parameters.Clear();

                            command.Parameters.AddWithValue("@MemberId", bookedToAdd.MemberId);
                            command.Parameters.AddWithValue("@BookId", bookedToAdd.BookId);
                            command.Parameters.AddWithValue("@DateBooked", bookedToAdd.dateBooked);
                            command.Parameters.AddWithValue("@DueDate", bookedToAdd.dueBooked);
                            command.Parameters.AddWithValue("@StatusId", 1); //1 means active booked, 0 means book was returned
                            command.Parameters.AddWithValue("@FineAmount", 0);
                            command.Parameters.AddWithValue("@UserId", bookedToAdd.UserId);

                            SqlParameter BookedIdOut = new SqlParameter();
                            BookedIdOut.ParameterName = "@BookedIdOut";
                            BookedIdOut.DbType = DbType.Int32;
                            BookedIdOut.Direction = ParameterDirection.Output;

                            command.Parameters.Add(BookedIdOut);

                            command.ExecuteNonQuery();
                        }

                        tran1.Complete();
                    }
                }
                catch (Exception ex)
                {
                    // IP: exception is raised when trying to submit "Add book form" with default dates
                    throw new Exception(ex.Message);
                }
            }
        }
        public void DeleteBooked(Booked bookedToDelete)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spDeleteBooked;

                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@BookedId", bookedToDelete.Id);
                    command.Parameters.AddWithValue("@BookId", bookedToDelete.BookId);                   
                    command.Parameters.AddWithValue("@StatusId", 0); //1 means active booked, 0 means book was returned                                                           

                    command.ExecuteNonQuery();
                }
            }
        }
        public Booked GetBookedById(int Id)
        {
            if (Id == 0)
            {
                throw new Exception("No information about id");
            }
            else
            {
                string bookedQuery = String.Format("SELECT [BookedId], [MemberId], [BookId], [DateBooked], [DueDate], [StatusId], [FineAmount], [UserId] FROM [dbo].[tblBooked] WHERE BookedId = {0} AND StatusId = 1", Id);

                Booked booked = new Booked();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = bookedQuery;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                booked.Id = (int)reader["BookedId"];
                                booked.MemberId = (int)reader["MemberId"];
                                booked.BookId = (int)reader["BookId"];
                                booked.dateBooked = (DateTime)reader["DateBooked"];
                                booked.dueBooked = (DateTime)reader["DueDate"];
                                booked.fine = (decimal)reader["FineAmount"];
                                booked.StatusId= (int)reader["StatusId"];
                                booked.UserId = (int)reader["UserId"];
                            }
                        }
                    }

                    return booked;
                }
            }
        }
        public void SetBookedFine(Booked booked)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                try
                {
                    using (TransactionScope tran1 = new TransactionScope())
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = spSetBookedFine;

                            command.Parameters.Clear();
                            
                            command.Parameters.AddWithValue("@BookedId", booked.Id);    
                            command.Parameters.AddWithValue("@StatusId", booked.StatusId); //1 means active booked, 0 means book was returned
                            command.Parameters.AddWithValue("@Fine", booked.fine);                                                                             

                            command.ExecuteNonQuery();
                        }

                        tran1.Complete();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
