using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModels.Supports
{
    public class NewRequestVM
    {
        public NewRequestVM()
        {
            var add = new Load();
            var list = add.AddRequest();
            RequestList = list;
        }
        public int? NewRequestID { get; set; }
        public IEnumerable<SelectListItem> RequestList { get; set; }
        public string Description { get; set; }
        public int? subRequest { get; set; }
    }
    public class Load
    {
        public IEnumerable<SelectListItem> AddRequest()
        {
            var list = new List<SelectListItem>();            
            list.Add(new SelectListItem()
            {
                Text = "Arıza Kaydı Açmak İstiyorum",
                Value = ((int)RequestTypes.Fault).ToString()
            });
            list.Add(new SelectListItem()
            {
                Text = "Nakil Yaptırmak İstiyorum",
                Value = ((int)RequestTypes.Transfer).ToString()
            });
            list.Add(new SelectListItem()
            {
                Text = "Fatura Konusunda Sorun Yaşıyorum",
                Value = ((int)RequestTypes.Invoice).ToString()
            });
            list.Add(new SelectListItem()
            {
                Text = "Tarife Değişikliği Yapmak İstiyorum",
                Value = ((int)RequestTypes.Tariff).ToString()
            });
            list.Add(new SelectListItem()
            {
                Text = "Hizmetimi Dondurmak İstiyorum",
                Value = ((int)RequestTypes.Freeze).ToString()
            });
            list.Add(new SelectListItem()
            {
                Text = "Statik IP Konusunda Destek",
                Value = ((int)RequestTypes.StaticIP).ToString()
            });
            list.Add(new SelectListItem()
            {
                Text = "Diğer Talepler",
                Value = ((int)RequestTypes.Others).ToString()
            });
            return list;
        }
    }
}
