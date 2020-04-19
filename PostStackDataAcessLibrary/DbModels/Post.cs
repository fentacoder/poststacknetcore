using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace PostStackDataAccessLibrary.DbModels
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Post()
        {

        }

        public List<Post> GetPosts(string connectionStr, int userId)
        {
            using(DbConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();

                using(var transaction = conn.BeginTransaction())
                {
                    using(var retrieveCmd = conn.CreateCommand())
                    {
                        try
                        {

                            retrieveCmd.Connection = conn;
                            retrieveCmd.Transaction = transaction;
                            retrieveCmd.CommandText = "SELECT * FROM dbo.Posts WHERE UserId = @Id";
                            retrieveCmd.Parameters.Add(new SqlParameter("@Id", userId));

                            using(var reader = retrieveCmd.ExecuteReader())
                            {
                                List<Post> postList = new List<Post>();

                                while (reader.Read())
                                {
                                    int postId = int.Parse(reader["Id"].ToString());
                                    int postUserId = int.Parse(reader["UserId"].ToString());
                                    string title = reader["Title"].ToString();
                                    string body = reader["Body"].ToString();
                                    DateTime createdAt = DateTime.Parse(reader["CreatedAt"].ToString());
                                    DateTime updatedAt = DateTime.Parse(reader["UpdatedAt"].ToString());

                                    postList.Add(new Post
                                    {
                                        Id = postId,
                                        UserId = postUserId,
                                        Title = title,
                                        Body = body,
                                        CreatedAt = createdAt,
                                        UpdatedAt = updatedAt
                                    });
                                }

                                conn.Close();

                                return postList ?? null;
                            }

                        }
                        catch(DbException err)
                        {
                            Console.WriteLine(err.Message);
                            conn.Close();
                            return null;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                            conn.Close();
                            return null;
                        }
                    }
                }
            }
        }

        public bool AddPost(string connectionStr, int userId, string title, string body)
        {
            using (DbConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    using (var addCmd = conn.CreateCommand())
                    {
                        try
                        {
                            DateTime currentTime = DateTime.Now;

                            addCmd.Connection = conn;
                            addCmd.Transaction = transaction;
                            addCmd.CommandText = "INSERT INTO dbo.Posts VALUES (@Id, @Title, @Body, @CreatedAt, @UpdatedAt)";
                            addCmd.Parameters.Add(new SqlParameter("@Id",userId));
                            addCmd.Parameters.Add(new SqlParameter("@Title",title));
                            addCmd.Parameters.Add(new SqlParameter("@Body",body));
                            addCmd.Parameters.Add(new SqlParameter("@CreatedAt",currentTime));
                            addCmd.Parameters.Add(new SqlParameter("@UpdatedAt",currentTime));

                            addCmd.ExecuteNonQuery();
                            transaction.Commit();
                            conn.Close();
                            return true;
                            

                        }
                        catch (DbException err)
                        {
                            Console.WriteLine(err.Message);
                            conn.Close();
                            return false;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            conn.Close();
                            return false;
                        }
                    }
                }
            }
        }

        public bool UpdatePost(string connectionStr, int userId, int postId, string title, string body)
        {
            using (DbConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    using (var updateCmd = conn.CreateCommand())
                    {
                        try
                        {
                            DateTime currentTime = DateTime.Now;

                            updateCmd.Connection = conn;
                            updateCmd.Transaction = transaction;
                            updateCmd.CommandText = "UPDATE dbo.Posts SET Title = @Title, Body = @Body, UpdatedAt = @UpdatedAt WHERE UserId = @UserIdParam AND Id = @PostId";
                            updateCmd.Parameters.Add(new SqlParameter("@Title", title));
                            updateCmd.Parameters.Add(new SqlParameter("@Body", body));
                            updateCmd.Parameters.Add(new SqlParameter("@UpdatedAt", currentTime));
                            updateCmd.Parameters.Add(new SqlParameter("@UserIdParam", userId));
                            updateCmd.Parameters.Add(new SqlParameter("@PostId", postId));

                            updateCmd.ExecuteNonQuery();
                            transaction.Commit();
                            conn.Close();
                            return true;


                        }
                        catch (DbException err)
                        {
                            Console.WriteLine(err.Message);
                            conn.Close();
                            return false;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            conn.Close();
                            return false;
                        }
                    }
                }
            }
        }

        public bool DeletePost(string connectionStr, int userId, int postId)
        {
            using (DbConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    using (var deleteCmd = conn.CreateCommand())
                    {
                        try
                        {

                            deleteCmd.Connection = conn;
                            deleteCmd.Transaction = transaction;
                            deleteCmd.CommandText = "DELETE FROM dbo.Posts WHERE UserId = @UserIdParam AND Id = @PostId";
                            deleteCmd.Parameters.Add(new SqlParameter("@UserIdParam", userId));
                            deleteCmd.Parameters.Add(new SqlParameter("@PostId",postId));

                            deleteCmd.ExecuteNonQuery();
                            transaction.Commit();
                            conn.Close();
                            return true;


                        }
                        catch (DbException err)
                        {
                            Console.WriteLine(err.Message);
                            conn.Close();
                            return false;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            conn.Close();
                            return false;
                        }
                    }
                }
            }
        }

    }
}
