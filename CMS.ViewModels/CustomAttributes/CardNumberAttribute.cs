﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CMS.ViewModels.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CardNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value is int || value is long || value is short)
                return ValidationResult.Success;
            if (Regex.IsMatch(value as string, @"^([0-9]{4}-){3}([0-9]{4}){1}$", RegexOptions.ECMAScript))
                return ValidationResult.Success;
            return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName));
        }
    }
}