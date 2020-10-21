using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CustomerManagementSystem.Auth
{
    public static class Hasher
    {
        public static string HashPassword(string password)
        {
            // Create a SHA256   
            return string.Join(string.Empty, SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)).Select(b => b.ToString("X2")));
        }
    }
}