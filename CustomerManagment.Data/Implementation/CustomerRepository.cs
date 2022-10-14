using CustomerManagment.Data.Abstraction;
using CustomerManagment.Data.Models;
using CustomerManagment.Domain.POCO;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagment.Data.Implementation
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string SECRET_KEY = "AFcd5L2f"; // the key we hash inputs with
        private readonly string _connection;

        private string GenerateHash(string input)
        {
            // this generates MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = md5.ComputeHash(bytes); // returns computed hash code

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));// the string is formatted in hexadecimal
                }
                return sb.ToString();
            }
        }

        public CustomerRepository(IOptions<ConnectionStrings> options)
        {
            _connection = options.Value.DefaultConnection; // in appsettings.json
        }

        public async Task<int> CreateAsync(Customer customer)
        {
            using (SqlConnection con = new SqlConnection(_connection))
            {
                string addCustomerQuery = "INSERT INTO Customer(FirstName, LastName, Email, Username, Password, GUID, IsVerified) OUTPUT INSERTED.Id VALUES(@FirstName, @LastName, @Email, @Username, @Password, @GUID, @IsVerified)";

                SqlCommand AddCustomerCommand = new SqlCommand(addCustomerQuery, con);
                //specifying parameters for my AddCustomerCommand
                AddCustomerCommand.Parameters.AddWithValue("FirstName", customer.FirstName);
                AddCustomerCommand.Parameters.AddWithValue("LastName", customer.LastName);
                AddCustomerCommand.Parameters.AddWithValue("Username", customer.UserName);
                AddCustomerCommand.Parameters.AddWithValue("Email", customer.Email);
                AddCustomerCommand.Parameters.AddWithValue("GUID", customer.Guid);
                AddCustomerCommand.Parameters.AddWithValue("IsVerified", customer.IsVerified);
                // here we don't pass Password directly to our database
                var newPass = GenerateHash(customer.Password + SECRET_KEY);
                AddCustomerCommand.Parameters.AddWithValue("Password", newPass); // we pass the hashed password for security reasons

                con.Open();

                return (int)await AddCustomerCommand.ExecuteScalarAsync().ConfigureAwait(false); //returns first row of first column of the output
            }
        }

        public async Task<bool> DeleteAsync(string email)
        {
            using (SqlConnection con = new SqlConnection(_connection))
            {
                string deleteQuery = "DELETE FROM Customer WHERE Email = @Email";

                SqlCommand deleteCommand = new SqlCommand(deleteQuery, con);
                deleteCommand.Parameters.AddWithValue("Email", email);

                con.Open();

                var rowsDeleted = await deleteCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

                return rowsDeleted > 0;
            }
        }

        public async Task<bool> Exists(string email)
        {
            using (SqlConnection con = new SqlConnection(_connection))
            {
                string existsQuery = "SELECT COUNT(*) FROM Customer WHERE Email = @Email";

                SqlCommand existsCommand = new SqlCommand(existsQuery, con);
                existsCommand.Parameters.AddWithValue("Email", email);

                con.Open();

                var count = (int)await existsCommand.ExecuteScalarAsync().ConfigureAwait(false);

                return count > 0;
            }
        }

        public async Task<Customer> GetAsync(string email, string password)
        {
            using (SqlConnection con = new SqlConnection(_connection))
            {
                string getQuery = "SELECT * FROM Customer WHERE Password = @Password AND Email = @Email AND IsVerified = 1";

                SqlCommand getCommand = new SqlCommand(getQuery, con);

                var pass = GenerateHash(password + SECRET_KEY);
                getCommand.Parameters.AddWithValue("Password", pass);
                getCommand.Parameters.AddWithValue("Email", email);

                con.Open();

                SqlDataReader reader = await getCommand.ExecuteReaderAsync().ConfigureAwait(false);

                Customer customer = null;

                while (reader.Read())
                {
                    customer = new Customer
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        UserName = reader.GetString(3),
                        Password = reader.GetString(4),
                        Email = reader.GetString(5),
                        Guid = reader.GetString(6),
                        IsVerified = reader.GetBoolean(7)
                    };
                }

                reader.Close();

                return customer;
            }
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword, string currentPassword)
        {
            using (SqlConnection con = new SqlConnection(_connection))
            {
                string updatePasswordQuery = "UPDATE Customer SET Password = @NewPassword WHERE Email = @Email AND Password = @CurrentPassword";

                SqlCommand UpdatePasswordCommand = new SqlCommand(updatePasswordQuery, con);

                var newPass = GenerateHash(newPassword + SECRET_KEY);
                UpdatePasswordCommand.Parameters.AddWithValue("NewPassword", newPass);

                var currentPass = GenerateHash(currentPassword + SECRET_KEY);
                UpdatePasswordCommand.Parameters.AddWithValue("CurrentPassword", currentPass);

                UpdatePasswordCommand.Parameters.AddWithValue("Email", email);

                con.Open();

                var rowsUpdated = await UpdatePasswordCommand.ExecuteNonQueryAsync();

                return rowsUpdated > 0;
            }
        }

        public async Task<bool> VerifyAsync(string guid)
        {
            using (SqlConnection con = new SqlConnection(_connection))
            {
                string validateQuery = "SELECT * FROM Customer WHERE Guid = @Guid";
                string verifyQuery = "UPDATE Customer SET IsVerified = 1 WHERE Email = @Email";

                SqlCommand validateCommand = new SqlCommand(validateQuery, con);
                validateCommand.Parameters.AddWithValue("Guid", guid);

                con.Open();

                var reader = await validateCommand.ExecuteReaderAsync();

                string emailToVerify = "";
                while (reader.Read())
                {
                    if (reader[0] == DBNull.Value)
                        return false;

                    emailToVerify = reader[5].ToString();
                }
                reader.Close();

                SqlCommand verifyCommand = new SqlCommand(verifyQuery, con);
                verifyCommand.Parameters.AddWithValue("Email", emailToVerify);

                await verifyCommand.ExecuteNonQueryAsync();
                return true;
            }
        }
    }
}
