﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

namespace CustomerManagementSystem
{
    public class GenericServiceSettings
    {
        GenericCustomerServiceReference.GenericCustomerServiceClient client = new GenericCustomerServiceReference.GenericCustomerServiceClient();
        public string Culture { get; set; }
        public string Rand { get; set; }
        public string Username { get; set; }
        private string Password { get; set; }
        public GenericServiceSettings()
        {
            Culture = Thread.CurrentThread.CurrentUICulture.Name;
            Rand = Guid.NewGuid().ToString();
            Username = Properties.Settings.Default.WebServiceUsername;
            Password = Properties.Settings.Default.WebServicePassword;
        }
        public string Hash { get { return HashUtilities.CalculateHash<SHA1>(Username + Rand + HashUtilities.CalculateHash<SHA1>(Password) + client.GetKeyFragment(Username)); } }
    }
    public static class HashUtilities
    {
        public static string CalculateHash<HAT>(string value) where HAT : HashAlgorithm
        {
            HAT algorithm = (HAT)HashAlgorithm.Create(typeof(HAT).Name);
            var calculatedHash = string.Join(string.Empty, algorithm.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(b => b.ToString("x2")));
            return calculatedHash;
        }
    }
}