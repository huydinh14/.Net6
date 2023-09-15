using API.Models;
using Dapper;
using System.Data.SqlClient;

namespace API.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration _configuration;
        private readonly string DBConnection;
        public DataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
            DBConnection = configuration["connectionStrings:DBConnect"] ?? "";
        }

        public bool AuthenticateUser(string email, string password, out User? user)
        {
            var result = false;
            using (var conn = new SqlConnection(DBConnection))
            {
                result = conn.ExecuteScalar<bool>("select count(1) from Users where email=@email and password=@password;", new { email, password });
                if (result)
                {
                    user = conn.QueryFirst<User>("select * from Users where email=@email;", new { email });
                }
                else
                {
                    user = null;
                }
            }
            return result;
        }

        public int CreateUser(User user)
        {
            var result = 0;
            using (var conn = new SqlConnection(DBConnection))
            {
                var parameters = new
                {
                    fn = user.FirstName,
                    ln = user.LastName,
                    em = user.Email,
                    mb = user.Mobile,
                    pwd = user.Password,
                    blk = user.Blocked,
                    act = user.Active,
                    con = Convert.ToDateTime(user.CreatedOn),
                    type = user.UserType.ToString()
                };
                var sql = "insert into Users (FirstName, LastName, Email, Mobile, Password, Blocked, Active, CreatedOn, UserType)" +
                    "      values (@fn, @ln, @em, @mb, @pwd, @blk, @act, @con, @type)";
                result = conn.Execute(sql, parameters);
            }
            return result;
        }

        public bool IsEmailAvailable(string email)
        {
            var result = false;

            using (var conn = new SqlConnection(DBConnection))
            {
                result = conn.ExecuteScalar<bool>("select count(*) from Users where Email=@email;", new { email });

            };

            return !result;
        }
    }
}
