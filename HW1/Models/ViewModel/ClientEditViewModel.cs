using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW1.Models.ViewModel
{
    public class ClientEditViewModel
    {
        public 客戶資料 Client { get; set; }
        public IList<客戶聯絡人> Contacts { get; set; }
        public IList<客戶銀行資訊> Banks { get; set; }
    }
}