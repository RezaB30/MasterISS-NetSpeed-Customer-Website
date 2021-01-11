using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Supports
{
    public class RequestSupportMessage
    {
        public long ID { get; set; }
        [Display(ResourceType = typeof(CMS.Localization.Common), Name = "Message")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(CMS.Localization.Errors))]
        public string Message { get; set; }

        private bool forOpenRequest = false;

        public bool ForOpenRequest
        {
            get { return forOpenRequest; }
            set { forOpenRequest = value; }
        }


        private bool isSolved = false;
        public bool IsSolved
        {
            get { return isSolved; }
            set { isSolved = value; }
        }


        private bool forAddNote = false;
        public bool ForAddNote
        {
            get { return forAddNote; }
            set { forAddNote = value; }
        }

    }
}
