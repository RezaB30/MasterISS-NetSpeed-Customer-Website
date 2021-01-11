﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CustomerManagementSystem.CustomAttributes
{
    public sealed class CardExpirationMonth : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value is int || value is long || value is short)
                return ValidationResult.Success;
            if (Regex.IsMatch(value as string, @"^([0]{0,1}[1-9]$)|([1][0-2]$)", RegexOptions.ECMAScript))
                return ValidationResult.Success;
            return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName));
        }
    }
}