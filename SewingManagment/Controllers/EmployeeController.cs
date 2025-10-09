using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SewingManagment.Data;
using SewingManagment.Models;
using System.Reflection;

namespace SewingManagment.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        public ActionResult Query()
        {
            try
            {
                // 從資料表取得所有 Employee
                var entity = _context.Employees.ToList();

                return View(entity);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }

        }

        //Get 進入 Create 頁面
        public ActionResult Create()
        {
            SetDropdownLists();

            return View();
        }

        // POST: EmployeeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeViewModel collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SetDropdownLists(); // 重新設定下拉式選單，以便在返回視圖時顯示
                    return View();
                }

                // 新增一筆資料
                var newEmployee = new Employee
                {
                    //Id = 1, ID 自動產生
                    Name = collection.Name,
                    Age = collection.Age,
                    Gender = collection.Gender,
                    Position = collection.Position,
                    IsManager = false // 若有這欄位可自行調整
                };

                // add 資料後一定要 SaveChanges 才會存入!!
                _context.Employees.Add(newEmployee);
                _context.SaveChanges();

                // 可設定提示訊息
                TempData["Message"] = "員工新增成功！";

                //執行成功後，重新導向到 Query 這個 Action（通常是列表頁）
                return RedirectToAction(nameof(Query));

            }
            catch
            {
                // 若有錯誤，回到原本畫面（可能附帶錯誤訊息）
                return View();
            }
        }

        // GET: EmployeeController/Edit/5
        public ActionResult Edit(int id)
        {
            SetDropdownLists();

            // 從資料表取得所有 Employee
            var employee = _context.Employees.Find(id);

            if (employee == null)
            {
                return NotFound(); // 找不到就回 404
            }

            // entity => view model 轉換
            var viewModel = new EmployeeViewModel
            {
                Name = employee == null ? "" : employee.Name,
                Age = employee == null ? "" : employee.Age,
                Gender = employee == null ? "" : employee.Gender,
                Position = employee == null ? "" : employee.Position
            };

            return View(viewModel);
        }

        // POST: EmployeeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                SetDropdownLists(); // 重新設定下拉式選單，以便在返回視圖時顯示
                return View(viewModel);
            }

            try
            {
                var employee = _context.Employees.Find(id);

                if (employee != null)
                {
                    employee.Name = viewModel.Name;
                    employee.Age = viewModel.Age;
                    employee.Gender = viewModel.Gender;
                    employee.Position = viewModel.Position;

                    // 儲存變更到資料庫
                    _context.SaveChanges();

                    // 模擬接收結果
                    TempData["Message"] = $"已收到更新資料：{employee.Name} ({employee.Gender})，{employee.Age}歲，職務：{employee.Position}";
                }

                return RedirectToAction(nameof(Query));
            }
            catch
            {
                SetDropdownLists(); // 若有錯誤，重新設定下拉式選單
                return View(viewModel); // 若有錯誤，回到原本畫面（可能附帶錯誤訊息）
            }
        }

        // GET: HomeController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // 用 ViewBag 傳下拉選項給 View
        private void SetDropdownLists()
        {
            ViewBag.GenderList = new List<SelectListItem>
            {
                new SelectListItem { Text = "男", Value = "M" },
                new SelectListItem { Text = "女", Value = "F" }
            };

            ViewBag.PositionList = new List<SelectListItem>
            {
                new SelectListItem { Text = "一般員工", Value = "01" },
                new SelectListItem { Text = "組長", Value = "02" },
                new SelectListItem { Text = "經理", Value = "03" }
            };
        }


    }
}
