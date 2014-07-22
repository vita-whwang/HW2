回家作業作業需求如下：

1. 請使用 ASP . NET MVC 5 + Entity Framework 6
2. 請使用上週提供的資料庫進行開發
3. 請接續上週的功能繼續實作，需求如下：

　* 對於從網址列上出現的 id 值，撰寫一個自訂的 Action Filter (動作過濾器) 檢查傳入的 id 格式是否符合要求，格式不對就導向回首頁。
	public class IdFilterAttribute : ActionFilterAttribute
    {
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string id = filterContext.HttpContext.Request.QueryString["id"];
            if (!String.IsNullOrEmpty(id))
            {
                if (Regex.Match(id,@"[\D]+").Success)
                {
                    filterContext.Result = new RedirectToRouteResult(
                                                new RouteValueDictionary 
                                                { 
                                                    { "controller", "Home" }, 
                                                    { "action", "Index" } 
                                                });
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
	
	[IdFilter]
    public class BaseController : Controller
    {
	...
	}
	
	
　* 寫一個 BaseController 並覆寫 HandleUnknownAction 方法，找不到 Action 就顯示一頁自訂的 404 錯誤頁面。
	[IdFilter]
    public class BaseController : Controller
    {
        protected override void HandleUnknownAction(string actionName)
        {

            Response.Redirect("/Home/Error404");
        }
	}

　* 所有看到 db.Entry(client).State = EntityState.Modified; 的寫法，都要改成資料繫結延遲驗證的方式做檢查 ( TryUpdateModel )，然後搭配自訂的 Interface 去針對特定表單欄位做 Model Binding，避免 over-posting 的問題發生。
　

  * 實作匯出資料功能，可將「客戶資料」匯出，用 FileResult 輸出檔案，輸出格式不拘 (XLS, CSV, ...)，下載檔名規則："YYYYMMDD_客戶資料匯出.xlsx"
  	public ActionResult Report()
	{
		string result = "";
		foreach (var data in ClientRepository.GetAll().ToList()) {
			result += data.客戶名稱 + ", " + data.Email + "\r\n";
		}
		return File(System.Text.Encoding.Unicode.GetBytes(result), "text/plain", string.Format("{0}_客戶資料匯出.txt",DateTime.Now.ToString("yyyyMMdd"))); 
	}
  
  
  
　* 實作 Master/Details 頁面，修改「客戶資料」的 Details 與 Edit 頁面，讓該頁面同時顯示「客戶銀行資訊」與「客戶聯絡人」的清單資料。
	不知道Master/Details是指哪隻程式



　* 「客戶資料」的 Details 頁面，請列出客戶銀行資訊與客戶連絡人的清單 (List)，並且可以增加「刪除」功能，可在這頁刪除客戶銀行資訊與客戶連絡人的資料，刪除成功後會繼續顯示客戶資料的 Details 頁面 (提示: 透過 ViewBag 或 ViewData 傳遞多個 Model 到 View 裡顯示 )
	@foreach (var row in ((IQueryable<HW1.Models.客戶聯絡人>)ViewBag.Contact)){
		...
		<td>
            @Html.ActionLink("Delete", "DeleteContact", new { id=row.Id })
        </td>
	}
	
	@foreach (var row in ((IQueryable<HW1.Models.客戶銀行資訊>)ViewBag.Bank)){
		...
		<td>
            @Html.ActionLink("Delete", "DeleteBank", new { id=row.Id })
        </td>
	}

	
	
　* 「客戶資料」的 Edit 頁面，請實作客戶銀行資訊與客戶連絡人的批次編輯功能，按下儲存按鈕，可以一次將所有資料儲存
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

		
		
　* 無論做哪個表單，都要做欄位輸入驗證。


　* 實作客戶聯絡人的「模型驗證」，同一個客戶下的聯絡人，其 Email 不能重複。
	[MetadataType(typeof(客戶聯絡人MetaData))]
    public partial class 客戶聯絡人 : I客戶聯絡人更新, IValidatableObject
    {
        客戶聯絡人Repository ContactRepository = RepositoryHelper.Get客戶聯絡人Repository();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ContactRepository.findEmailByClientId(this.客戶Id,this.Id,this.Email))
            {
                yield return new ValidationResult("Email與其他聯絡人重複", new string[] { "Email" });
            }
        }   
    }
