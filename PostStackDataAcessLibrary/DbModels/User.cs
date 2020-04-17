using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Data;

namespace PostStackDataAccessLibrary.DbModels
{
    public class User
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public DateTime UserCreatedAt { get; set; }

        public int IsAdmin { get; set; }

        public User()
        {

        }

        public int LogUserIn(string connectionStr, string email, string password)
        {
            
            using(DbConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();

                using(var transaction = conn.BeginTransaction())
                {

                    using(var loginCommand = conn.CreateCommand())
                    {

                        try
                        {

                            var passwordMatches = false;

                            loginCommand.Connection = conn;
                            loginCommand.Transaction = transaction;
                            loginCommand.CommandText = "SELECT Id,Password FROM dbo.Users WHERE Email = @UserEmail";

                            loginCommand.Parameters.Add(new SqlParameter("@UserEmail", email));

                            using(DbDataReader reader = loginCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var userIdStr = reader["Id"].ToString();

                                    var userId = int.Parse(userIdStr);

                                    var userPassword = reader["Password"].ToString();

                                    //compare password here to compare it in procedure
                                    PasswordHasher passwordHasher = new PasswordHasher();

                                    var result = passwordHasher.VerifyHashedPassword(userPassword, password);


                                    if(result == PasswordVerificationResult.Success)
                                    {
                                        passwordMatches = true;
                                    }else if(result == PasswordVerificationResult.SuccessRehashNeeded)
                                    {
                                        passwordMatches = true;
                                    }

                                    if (passwordMatches)
                                    {
                                        conn.Close();
                                        return userId;
                                    }
                                    else
                                    {
                                        conn.Close();
                                        return -1;
                                    }
                                }
                                else
                                {
                                    return -1;
                                }
                            }

                        }
                        catch(DbException err)
                        {
                            Console.WriteLine(err.Message);
                            conn.Close();
                            return -1;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                            conn.Close();
                            return -1;                        
                        }

                    }

                }
            }

        }

        public int RegisterUser(string connectionStr, string name, string email, string password)
        {
            using(DbConnection conn = new SqlConnection(connectionStr))
            {
                //hash password here before it goes into the database
                var passwordHasher = new PasswordHasher();

                string hashedPassword = passwordHasher.HashPassword(password);

                var isAdmin = 0;

                DateTime currentTime = DateTime.Now;

                conn.Open();

                using(var transaction = conn.BeginTransaction())
                {

                    using(var registerCommand = conn.CreateCommand())
                    {
                        try
                        {

                            registerCommand.Connection = conn;
                            registerCommand.Transaction = transaction;
                            registerCommand.CommandText = "INSERT INTO dbo.Users VALUES (@UserName,@UserEmail,@UserPassword,@CreatedAt,@Admin); SELECT @UserId = scope_identity()";

                            registerCommand.Parameters.Add(new SqlParameter("@UserName",name));
                            registerCommand.Parameters.Add(new SqlParameter("@UserEmail",email));
                            registerCommand.Parameters.Add(new SqlParameter("@UserPassword",hashedPassword));
                            registerCommand.Parameters.Add(new SqlParameter("@CreatedAt",currentTime));
                            registerCommand.Parameters.Add(new SqlParameter("@Admin",isAdmin));

                            var outputParam = registerCommand.CreateParameter();
                            outputParam.ParameterName = "@UserId";
                            outputParam.Direction = ParameterDirection.Output;
                            outputParam.DbType = DbType.Int32;
                            registerCommand.Parameters.Add(outputParam);

                            int rowsAffected = registerCommand.ExecuteNonQuery();

                            transaction.Commit();

                            int userId = int.Parse(outputParam.Value.ToString());
                            conn.Close();
                            return userId;

                        }
                        catch (DbException err)
                        {
                            Console.WriteLine(err.Message);
                            transaction.Rollback();
                            conn.Close();
                            return -1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            transaction.Rollback();
                            conn.Close();
                            return -1;
                        }
                    }

                }
            }
        }

    }
}
