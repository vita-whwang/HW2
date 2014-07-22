namespace HW1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public interface I客戶聯絡人更新 {

        [Required]
        int 客戶Id { get; set; }

        [StringLength(50, ErrorMessage = "欄位長度不得大於 50 個字元")]
        [Required]
        string 職稱 { get; set; }

        [StringLength(50, ErrorMessage = "欄位長度不得大於 50 個字元")]
        [Required]
        string 姓名 { get; set; }

        [StringLength(250, ErrorMessage = "欄位長度不得大於 250 個字元")]
        [Required]
        string Email { get; set; }

        [StringLength(50, ErrorMessage = "欄位長度不得大於 50 個字元")]
        string 手機 { get; set; }

        [StringLength(50, ErrorMessage = "欄位長度不得大於 50 個字元")]
        string 電話 { get; set; }
    }


    [MetadataType(typeof(客戶聯絡人MetaData))]
    public partial class 客戶聯絡人 : I客戶聯絡人更新, IValidatableObject
    {
        客戶聯絡人Repository ContactRepository = RepositoryHelper.Get客戶聯絡人Repository();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //　* 實作客戶聯絡人的「模型驗證」，同一個客戶下的聯絡人，其 Email 不能重複。
            if (ContactRepository.findEmailByClientId(this.客戶Id,this.Id,this.Email))
            {
                yield return new ValidationResult("Email與其他聯絡人重複", new string[] { "Email" });
            }
        }   
    }
    
    public partial class 客戶聯絡人MetaData
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int 客戶Id { get; set; }
        
        [StringLength(50, ErrorMessage="欄位長度不得大於 50 個字元")]
        [Required]
        public string 職稱 { get; set; }
        
        [StringLength(50, ErrorMessage="欄位長度不得大於 50 個字元")]
        [Required]
        public string 姓名 { get; set; }
        
        [StringLength(250, ErrorMessage="欄位長度不得大於 250 個字元")]
        [Required]
        public string Email { get; set; }
        
        [StringLength(50, ErrorMessage="欄位長度不得大於 50 個字元")]
        public string 手機 { get; set; }
        
        [StringLength(50, ErrorMessage="欄位長度不得大於 50 個字元")]
        public string 電話 { get; set; }
        [Required]
        public bool isDelete { get; set; }
    
        public virtual 客戶資料 客戶資料 { get; set; }
    }
}
