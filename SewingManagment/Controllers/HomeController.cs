using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SewingManagment.Models;

namespace SewingManagment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }



        public IActionResult Index()
        {
            /** 傳遞資料到 View 的幾種方法：
             * 1. 透過 ViewData (用於傳遞少量、非結構化的資料)
             * 2. 透過 ViewBag (類似 ViewData，但以動態屬性方式存取)
             * 3. 透過 Model (推薦用於傳遞強型別、結構化的資料，通常是一個物件)
             */

            ViewData["TestString1"] = "ViewData from Controller";
            ViewBag.Test2 = "VieBag Test2 from Controller";
            return View();
        }

        public IActionResult Create()
        {
            // 用 ViewBag 傳下拉選項給 View
            //SetDropdownLists();

            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeViewModel model)
        {
            //Q: ModelState 是後端所有表單驗證結果的集合，不用自己建立
            if (!ModelState.IsValid)
            {
                // 重傳 ViewBag 傳下拉選項給 View
                //SetDropdownLists();

                return View(model);
            }

            // 資料庫儲存
            TempData["Message"] = "員工新增成功！";

            //重置 Create 頁面
            return RedirectToAction("Create");
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var model = new EmployeeViewModel
            {
                Name = "Joyce",
                Age = "18",
                Gender = "F",
                Position = "01"
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult Edit(EmployeeViewModel model)
        {
            // 模擬接收結果
            ViewBag.Message = $"已收到更新資料：{model.Name} ({model.Gender})，{model.Age}歲，職務：{model.Position}";

            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
