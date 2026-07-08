using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static List<User> users = new List<User>();
    static List<UserLoginInfo> loginInfos = new List<UserLoginInfo>();

    static void Main()
    {
        SeedData();

        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        string result = Login(username, password);
        Console.WriteLine(result);
    }

    static string Login(string username, string password)
    {
        // STEP 1: Validate input
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return "Invalid login.";

        // STEP 2: Find user
        var user = users.FirstOrDefault(u => u.UserName == username);
        if (user == null)
            return "Invalid login.";

        // STEP 3: Check LoginStatus
        var status = GetLoginInfo(user.Id, "LoginStatus");

        if (status != "Active")
            return "Your account is LockedOut, have an Admin unlock your account first";

        // STEP 4: Get stored password
        var storedPassword = GetLoginInfo(user.Id, "Password");

        // STEP 5: Hash entered password
        string hashedInput = HashPassword(password);

        // STEP 6: Compare passwords
        if (hashedInput == storedPassword)
        {
            // Reset retries on success
            SetLoginInfo(user.Id, "LoginRetries", "0");

            return "User is logged in successfully.";
        }
        else
        {
            // STEP 7: Failed login
            int retries = 0;
            var retryValue = GetLoginInfo(user.Id, "LoginRetries");

            if (retryValue != null)
                retries = int.Parse(retryValue);

            retries++;

            SetLoginInfo(user.Id, "LoginRetries", retries.ToString());

            // STEP 8: Lockout
            if (retries > 2)
            {
                SetLoginInfo(user.Id, "LoginStatus", "LockedOut");
            }

            return "Invalid login.";
        }
    }

    static string HashPassword(string password)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    static string GetLoginInfo(int userId, string key)
    {
        return loginInfos
            .FirstOrDefault(x => x.UserId == userId && x.Key == key)?.Value;
    }

    static void SetLoginInfo(int userId, string key, string value)
    {
        var record = loginInfos
            .FirstOrDefault(x => x.UserId == userId && x.Key == key);

        if (record == null)
        {
            loginInfos.Add(new UserLoginInfo
            {
                UserId = userId,
                Key = key,
                Value = value
            });
        }
        else
        {
            record.Value = value;
        }
    }

    static void SeedData()
    {
        users.Add(new User
        {
            Id = 1,
            UserName = "admin",
            FirstName = "Adrian",
            LastName = "Reyes"
        });

        loginInfos.Add(new UserLoginInfo
        {
            UserId = 1,
            Key = "LoginStatus",
            Value = "Active"
        });

        loginInfos.Add(new UserLoginInfo
        {
            UserId = 1,
            Key = "Password",
            Value = HashPassword("admin123")
        });

        loginInfos.Add(new UserLoginInfo
        {
            UserId = 1,
            Key = "LoginRetries",
            Value = "0"
        });
    }
}

class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

class UserLoginInfo
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}