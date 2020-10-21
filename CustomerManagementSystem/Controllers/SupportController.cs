using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.ViewModels.Supports;
using PagedList;

namespace CustomerManagementSystem.Controllers
{
    public class SupportController : Controller
    {
        // GET: Support
        public ActionResult SupportRequests(int page = 1, int pageSize = 6)   // destek.html
        {
            var note = "35 mbps internet bağlatmama rağmen sık sık bu hızlarda düşüş yaşıyor ve 15 mbps hızları zor görüyorum." +
                " Sanırım kuruluma gelen arkadaş kabloları bağlarken bir hata yaptı." +
                " Kurulumun kontrol edilmesi için destek talep ediyorum.";
            var req = new List<CMS.ViewModels.Supports.SupportRequestsVM>();
            for (int i = 0; i < 100; i++)
            {
                req.Add(new CMS.ViewModels.Supports.SupportRequestsVM()
                {
                    ChangeType = ChangeType.Agent,
                    Date = DateTime.Now.AddDays(-1),
                    Department = "TEKNİK DESTEK",
                    Note = note.Length > 200 ? note.Substring(0, 200) : note,
                    State = "TAMAMLANDI",
                    SupportNo = 1,
                    SupportRequestName = "BAĞLANTI SORUNU",
                    SupportRequestSummary = "İNTERNETE BAĞLANMADA SORUN YAŞIYORUM",
                    SupportState = SupportState.Complete
                });
            }
            req.Add(new CMS.ViewModels.Supports.SupportRequestsVM()
            {
                ChangeType = ChangeType.Agent,
                Date = DateTime.Now.AddDays(-1),
                Department = "TEKNİK DESTEK",
                Note = note.Length > 200 ? note.Substring(0, 200) : note,
                State = "TAMAMLANDI",
                SupportNo = 1,
                SupportRequestName = "BAĞLANTI SORUNU",
                SupportRequestSummary = "İNTERNETE BAĞLANMADA SORUN YAŞIYORUM",
                SupportState = SupportState.Complete
            });
            req.Add(new CMS.ViewModels.Supports.SupportRequestsVM()
            {
                ChangeType = null,
                Date = DateTime.Now.AddDays(-2),
                Department = "TEKNİK DESTEK",
                Note = note.Length > 200 ? note.Substring(0, 200) : note,
                State = "İŞLEMDE",
                SupportNo = 2,
                SupportRequestName = "BAĞLANTI SORUNU",
                SupportRequestSummary = "İNTERNETE BAĞLANMADA SORUN YAŞIYORUM",
                SupportState = SupportState.Processing
            });
            req.Add(new CMS.ViewModels.Supports.SupportRequestsVM()
            {
                ChangeType = ChangeType.Customer,
                Date = DateTime.Now.AddDays(-2),
                Department = "TEKNİK DESTEK",
                Note = note.Length > 200 ? note.Substring(0, 200) : note,
                State = "TAMAMLANDI",
                SupportNo = 3,
                SupportRequestName = "BAĞLANTI SORUNU",
                SupportRequestSummary = "İNTERNETE BAĞLANMADA SORUN YAŞIYORUM",
                SupportState = SupportState.Complete
            });
            var TotalCount = req.Count();
            var list = req.OrderByDescending(m => m.Date).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            StaticPagedList<SupportRequestsVM> model = new StaticPagedList<SupportRequestsVM>(list, page, pageSize, TotalCount);
            return View(model);
        }
        //public ActionResult SupportDetails(long SupportNo) //destek-detay.html
        //{
        //    return View();
        //}
        public ActionResult SupportResults(long SupportNo) // destek-detay-biten.html - destek-detay-tam-biten
        {
            var message = new SupportMessagesVM();
            var messageList = new List<SupportMessageList>();
            var Files = new List<SupportFileList>();
            Files.Add(new SupportFileList()
            {
                FileName = "Tester.jpg",
                FileUrl = "#"
            });
            var msgDate = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                messageList.Add(new SupportMessageList()
                {
                    Files = i == 7 ? Files : null,
                    Message = "Bu bir test mesajıdır.",
                    MessageDate = msgDate.AddDays(-(i * 7)),
                    SenderName = i % 2 == 1 ? "Onur Civanoğlu" : "Müşteri Temsilcisi",
                    SenderType = i % 2 == 1 ? SupportMessageSenderType.Customer : SupportMessageSenderType.Agent,
                    State = SupportNo == 1 ? "TAMAMLANDI" : SupportNo == 2 ? "İŞLEMDE" : "TAMAMLANDI",
                    SupportState = SupportNo == 1 ? SupportState.Complete : SupportNo == 2 ? SupportState.Processing : SupportState.Complete,

                });
            }
            if (SupportNo == 1)
            {
                message = new SupportMessagesVM()
                {
                    State = "TAMAMLANDI",
                    SupportState = SupportState.Complete,
                    SupportNo = SupportNo,
                    AgentID = 1,
                    AgentName = "Müşteri Temsilcisi",
                    ChangeType = ChangeType.Agent,
                    CustomerFullName = "Onur Civanoğlu",
                    Department = "TEKNİK DESTEK",
                    SupportDate = DateTime.Now,
                    SupportMessageList = messageList,
                    SupportRequestName = "BAĞLANTI SORUNU",
                    SupportRequestSummary = "İNTERNETE BAĞLANMADA SORUN YAŞIYORUM",
                };
                return View(message);
            }
            if (SupportNo == 2)
            {
                message = new SupportMessagesVM()
                {
                    State = "İŞLEMDE",
                    SupportState = SupportState.Processing,
                    SupportNo = SupportNo,
                    AgentID = 1,
                    AgentName = "Müşteri Temsilcisi",
                    ChangeType = null,
                    CustomerFullName = "Onur Civanoğlu",
                    Department = "TEKNİK DESTEK",
                    SupportDate = DateTime.Now,
                    SupportMessageList = messageList,
                    SupportRequestName = "BAĞLANTI SORUNU",
                    SupportRequestSummary = "İNTERNETE BAĞLANMADA SORUN YAŞIYORUM",
                };
                return View(message);
            }
            else
            {
                message = new SupportMessagesVM()
                {
                    State = "TAMAMLANDI",
                    SupportState = SupportState.Complete,
                    SupportNo = SupportNo,
                    AgentID = 1,
                    AgentName = "Müşteri Temsilcisi",
                    ChangeType = ChangeType.Customer,
                    CustomerFullName = "Onur Civanoğlu",
                    Department = "TEKNİK DESTEK",
                    SupportDate = DateTime.Now,
                    SupportMessageList = messageList,
                    SupportRequestName = "BAĞLANTI SORUNU",
                    SupportRequestSummary = "İNTERNETE BAĞLANMADA SORUN YAŞIYORUM",
                };
                return View(message);
            }
        }
        public ActionResult NewRequests() //yeni-talep.html
        {
            var req = new NewRequestVM();
            return View(req);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewRequests(NewRequestVM request) //yeni-talep.html
        {
            if (request.NewRequestID == null)
            {
                return Json(new { errorCode = 1, errorMessage = "Lütfen Bir Konu Seçiniz" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { errorCode = 0, errorMessage = "Destek Kaydı Başarıyla Açıldı" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SupportRequestTable()
        {
            var req = new List<CMS.ViewModels.Supports.SupportRequestsVM>();
            req.Add(new CMS.ViewModels.Supports.SupportRequestsVM()
            {
                ChangeType = ChangeType.Agent,
                Date = DateTime.Now.AddDays(-1),
                Department = "TEKNİK DESTEK",
                State = "TAMAMLANDI",
                SupportNo = 1,
                SupportRequestName = "BAĞLANTI SORUNU",
                SupportRequestSummary = "İNTERNETE BAĞLANMADA SORUN YAŞIYORUM",
                SupportState = SupportState.Complete,
                CompletedDate = DateTime.Now.ToString("dd.MM.yyyy")
            });
            req.Add(new CMS.ViewModels.Supports.SupportRequestsVM()
            {
                ChangeType = ChangeType.Agent,
                Date = DateTime.Now.AddDays(-1),
                Department = "TEKNİK DESTEK",
                State = "TAMAMLANDI",
                SupportNo = 1,
                SupportRequestName = "BAĞLANTI SORUNU",
                SupportRequestSummary = "İNTERNETE BAĞLANMADA SORUN YAŞIYORUM",
                SupportState = SupportState.Complete,
                CompletedDate = DateTime.Now.ToString("dd.MM.yyyy")
            });
            req.Add(new CMS.ViewModels.Supports.SupportRequestsVM()
            {
                ChangeType = null,
                Date = DateTime.Now.AddDays(-2),
                Department = "TEKNİK DESTEK",
                State = "İŞLEMDE",
                SupportNo = 2,
                SupportRequestName = "BAĞLANTI SORUNU",
                SupportRequestSummary = "İNTERNETE BAĞLANMADA SORUN YAŞIYORUM",
                SupportState = SupportState.Processing,
                CompletedDate = DateTime.Now.ToString("dd.MM.yyyy")
            });
            req.Add(new CMS.ViewModels.Supports.SupportRequestsVM()
            {
                ChangeType = ChangeType.Customer,
                Date = DateTime.Now.AddDays(-2),
                Department = "TEKNİK DESTEK",
                State = "TAMAMLANDI",
                SupportNo = 3,
                SupportRequestName = "BAĞLANTI SORUNU",
                SupportRequestSummary = "İNTERNETE BAĞLANMADA SORUN YAŞIYORUM",
                SupportState = SupportState.Complete,
                CompletedDate = DateTime.Now.ToString("dd.MM.yyyy")
            });
            req.Add(new CMS.ViewModels.Supports.SupportRequestsVM()
            {
                ChangeType = null,
                Date = DateTime.Now.AddDays(-2),
                Department = "TEKNİK DESTEK",
                State = "İŞLEMDE",
                SupportNo = 2,
                SupportRequestName = "BAĞLANTI SORUNU",
                SupportRequestSummary = "İNTERNETE BAĞLANMADA SORUN YAŞIYORUM",
                SupportState = SupportState.Processing,
                CompletedDate = DateTime.Now.ToString("dd.MM.yyyy")
            });
            return PartialView("~/Views/Shared/DisplayTemplates/Index/RequestsTable.cshtml", req.OrderByDescending(m => m.Date).Take(4).ToList());
        }
        public ActionResult RequestData(int NewRequestID)
        {
            var label = string.Empty;
            var list = new List<SelectListItem>();
            switch ((RequestTypes)NewRequestID)
            {
                case RequestTypes.Transfer:
                    {
                        label = "NAKİL";
                        list.Add(new SelectListItem()
                        {
                            Text = "Nakil Talebinde Bulunmak İstiyorum",
                            Value = "1"
                        });
                    }
                    break;
                case RequestTypes.Fault:
                    {
                        label = "ARIZA";
                        list.Add(new SelectListItem()
                        {
                            Text = "ADSL Işığı Yanmıyor",
                            Value = "1"
                        });
                        list.Add(new SelectListItem()
                        {
                            Text = "Sık Sık Kopma Problemi",
                            Value = "2"
                        });
                        list.Add(new SelectListItem()
                        {
                            Text = "İnternet Işığı Yanmıyor",
                            Value = "3"
                        });
                        list.Add(new SelectListItem()
                        {
                            Text = "Bazı Sayfalara Erişemiyorum",
                            Value = "4"
                        });
                        list.Add(new SelectListItem()
                        {
                            Text = "Düşük Hız Alıyorum",
                            Value = "5"
                        });
                    }
                    break;
                case RequestTypes.Invoice:
                    {
                        label = "FATURA";
                        list.Add(new SelectListItem()
                        {
                            Text = "Faturam Yüksek Geldi",
                            Value = "1"
                        });
                        list.Add(new SelectListItem()
                        {
                            Text = "Faturamı Ödeyemiyorum",
                            Value = "2"
                        });
                    }
                    break;
                case RequestTypes.Tariff:
                    {
                        label = "TARİFE";
                        list.Add(new SelectListItem()
                        {
                            Text = "Tarifemi Değiştirmek İstiyorum",
                            Value = "1"
                        });
                    }
                    break;
                case RequestTypes.Freeze:
                    {
                        label = "DONDURMA";
                        list.Add(new SelectListItem()
                        {
                            Text = "Hizmetimi Bir Süreliğine Dondurmak İstiyorum",
                            Value = "1"
                        });
                    }
                    break;
                case RequestTypes.StaticIP:
                    {
                        label = "STATİK IP";
                        list.Add(new SelectListItem()
                        {
                            Text = "Statik IP Hizmeti Almak İstiyorum",
                            Value = "1"
                        });
                    }
                    break;
                case RequestTypes.Others:
                    break;
                default:
                    break;
            }
            return Json(new { label = string.Format("Talebiniz Nedir ? ({0})", label), list = list.Select(m => new { Text = m.Text, Value = m.Value }) }, JsonRequestBehavior.AllowGet);
        }
    }
}