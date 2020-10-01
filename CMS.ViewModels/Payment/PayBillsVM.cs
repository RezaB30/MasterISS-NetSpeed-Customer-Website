using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModels.Payment
{
    public class PayBillsVM
    {
        [Required]
        public string CCFullName { get; set; }
        [Required]
        public string CCNumber { get; set; }
        [Required]
        public int ExpirationMonthId { get; set; }
        public List<SelectListItem> ExpirationMonthDate
        {
            get
            {
                var list = new List<SelectListItem>();
                for (int i = 1; i < 13; i++)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }
                return list;
            }
        }
        [Required]
        public int ExpirationYearId { get; set; }
        public List<SelectListItem> ExpirationYearDate
        {
            get
            {
                var list = new List<SelectListItem>();
                for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 11; i++)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }
                return list;
            }
        }
        [Required]
        public int CreditSecureCode { get; set; }
    }
}
