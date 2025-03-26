using ChildTracking.Repositories.Models;
using ChildTracking.SoapService.MVC.Models;
using ChildTracking.SoapService.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ChildTracking.SoapService.MVC.Controllers
{
    public class ChildController : Controller
    {
        private readonly ChildSoapClient _soapClient;

        public ChildController(ChildSoapClient soapClient)
        {
            _soapClient = soapClient;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var children = await _soapClient.GetAllChildrenAsync();
                return View(children);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { RequestId = ex.Message });
            }
        }
    }
}