using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class InfrastructureController : Controller
    {
        // GET: Infrastructure
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDistrict(long code)
        {
            var GetDistrict = new ServiceUtilities().GetProvinceDistricts(code); //client.GetProvinceCities(code);
            if (GetDistrict.ValueNamePairList == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            return Json(GetDistrict.ValueNamePairList.Select(m => new { text = m.Name, value = m.Value }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetRegions(long code)
        {
            var GetRegions = new ServiceUtilities().GetDistrictRuralRegions(code); //client.GetCityRegions(code);
            if (GetRegions.ValueNamePairList == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            return Json(GetRegions.ValueNamePairList.Select(m => new { text = m.Name, value = m.Value }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNeighborhoods(long code)
        {
            var GetNeighborhoods = new ServiceUtilities().GetRuralRegionNeighbourhoods(code); /*client.GetRegionDistrict(code);*/
            if (GetNeighborhoods.ValueNamePairList == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            return Json(GetNeighborhoods.ValueNamePairList.Select(m => new { text = m.Name, value = m.Value }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStreets(long code)
        {
            var GetStreets = new ServiceUtilities().GetNeighbourhoodStreets(code); //client.GetDistrictStreets(code);
            if (GetStreets.ValueNamePairList == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            return Json(GetStreets.ValueNamePairList.Select(m => new { text = m.Name, value = m.Value }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBuildings(long code)
        {
            var GetBuildings = new ServiceUtilities().GetStreetBuildings(code); //client.GetStreetBuildingsCodes(code);
            if (GetBuildings.ValueNamePairList == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            return Json(GetBuildings.ValueNamePairList.Select(m => new { text = m.Name, value = m.Value }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetApartments(long code)
        {
            var GetApartments = new ServiceUtilities().GetBuildingApartments(code);
            if (GetApartments.ValueNamePairList == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            return Json(GetApartments.ValueNamePairList.Select(m => new { text = m.Name, value = m.Value }), JsonRequestBehavior.AllowGet);
        }
    }
}