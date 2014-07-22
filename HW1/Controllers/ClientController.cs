using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HW1.Models.ViewModel;
using HW1.Models;

namespace HW1.Controllers
{
    public class ClientController : BaseController
    {
        //private 資料庫Entities db = new 資料庫Entities();
        客戶資料Repository ClientRepository = RepositoryHelper.Get客戶資料Repository();
        客戶聯絡人Repository ContactRepository = RepositoryHelper.Get客戶聯絡人Repository();
        客戶銀行資訊Repository BankRepository = RepositoryHelper.Get客戶銀行資訊Repository();
        
        // GET: /Client/
        public ActionResult Index()
        {
            return View(ClientRepository.GetAll().ToList());
        }

        [HttpPost]
        public ActionResult Index(List<string> deleteContact, List<string> deleteRank)
        {
            return View(ClientRepository.GetAll().ToList());
        }

        /// <summary>
        /// * 「客戶資料」的 Details 頁面，請列出客戶銀行資訊與客戶連絡人的清單 (List)，並且可以增加「刪除」功能，可在這頁刪除客戶銀行資訊與客戶連絡人的資料，刪除成功後會繼續顯示客戶資料的 Details 頁面 (提示: 透過 ViewBag 或 ViewData 傳遞多個 Model 到 View 裡顯示 )
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            客戶資料 客戶資料 =ClientRepository.FindClientById(id.Value);
            ViewBag.Contact=ContactRepository.FindContactByClientId(id.Value);
            ViewBag.Bank = BankRepository.FindContactByClientId(id.Value);
            if (客戶資料 == null)
            {
                return HttpNotFound();
            }
            return View(客戶資料);
        }

        // GET: /Client/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Client/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,客戶名稱,統一編號,電話,傳真,地址,Email")] 客戶資料 客戶資料)
        {
            if (ModelState.IsValid)
            {
                ClientRepository.Add(客戶資料);
                ClientRepository.UnitOfWork.Commit();
                //db.客戶資料.Add(客戶資料);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(客戶資料);
        }

        // GET: /Client/Edit/5
        /// <summary>
        /// 　* 「客戶資料」的 Edit 頁面，請實作客戶銀行資訊與客戶連絡人的批次編輯功能，按下儲存按鈕，可以一次將所有資料儲存
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            ClientEditViewModel model = new ClientEditViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            model.Client = ClientRepository.FindClientById(id.Value);
            model.Contacts = ContactRepository.FindContactByClientId(id.Value).ToList();
            model.Banks = BankRepository.FindContactByClientId(id.Value).ToList();
            if (model.Client == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /Client/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClientEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                客戶資料 Client = ClientRepository.FindClientById(model.Client.Id);
                if (TryUpdateModel<I客戶資料更新>(Client,"Client"))
                {
                    ClientRepository.UnitOfWork.Commit();
                }

                if (model.Contacts != null)
                {
                    for (int i = 0; i < model.Contacts.Count(); i++)
                    {
                        var data = model.Contacts[i];
                        var Contact = ContactRepository.FindContactById(data.Id);
                        if (TryUpdateModel<I客戶聯絡人更新>(Contact, "Contacts[" + i + "]"))
                        {
                            ContactRepository.UnitOfWork.Commit();
                        }
                    }
                }

                if (model.Banks != null)
                {
                    for (int i = 0; i < model.Banks.Count(); i++)
                    {
                        var data = model.Banks[i];
                        var Bank = BankRepository.FindBankById(data.Id);
                        if (TryUpdateModel<I客戶銀行資訊更新>(Bank, "Banks[" + i + "]"))
                        {
                            BankRepository.UnitOfWork.Commit();
                        }
                    }
                }
            }
            return RedirectToAction("Edit", new { id = model.Client.Id });
        }



        // GET: /Client/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            客戶資料 客戶資料 = ClientRepository.FindClientById(id.Value);
            if (客戶資料 == null)
            {
                return HttpNotFound();
            }
            return View(客戶資料);
        }

        public ActionResult DeleteContact(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            客戶聯絡人 客戶聯絡人 = ContactRepository.FindContactById(id.Value);
            if (客戶聯絡人 != null)
            {
                客戶聯絡人.isDelete = true;
                ContactRepository.UnitOfWork.Context.Entry(客戶聯絡人).State = System.Data.Entity.EntityState.Modified;
                ContactRepository.UnitOfWork.Commit();
            }
            return RedirectToAction("Details", new { id = 客戶聯絡人.客戶Id});
        }

        public ActionResult DeleteBank(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            客戶銀行資訊 客戶銀行資訊 = BankRepository.FindBankById(id.Value);
            if (客戶銀行資訊 != null)
            {
                客戶銀行資訊.isDelete = true;
                BankRepository.UnitOfWork.Context.Entry(客戶銀行資訊).State = System.Data.Entity.EntityState.Modified;
                BankRepository.UnitOfWork.Commit();
            }

            return RedirectToAction("Details", new { id = 客戶銀行資訊.客戶Id });
        }

        // POST: /Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            客戶資料 客戶資料 = ClientRepository.FindClientById(id);
            foreach (var item in 客戶資料.客戶聯絡人)
            {
                item.isDelete = true;
            }

            foreach (var item in 客戶資料.客戶銀行資訊)
            {
                item.isDelete = true;
            }

            客戶資料.isDelete = true;
            ClientRepository.UnitOfWork.Context.Entry(客戶資料).State = System.Data.Entity.EntityState.Modified;
            ClientRepository.UnitOfWork.Commit();

            return RedirectToAction("Index");
        }

        //　* 實作匯出資料功能，可將「客戶資料」匯出，用 FileResult 輸出檔案，輸出格式不拘 (XLS, CSV, ...)，下載檔名規則："YYYYMMDD_客戶資料匯出.xlsx"
        public ActionResult Report()
        {
            string result = "";
            foreach (var data in ClientRepository.GetAll().ToList()) {
                result += data.客戶名稱 + ", " + data.Email + "\r\n";
            }
            return File(System.Text.Encoding.Unicode.GetBytes(result), "text/plain", string.Format("{0}_客戶資料匯出.txt",DateTime.Now.ToString("yyyyMMdd"))); 
        }
    }
}
