/**
*┌──────────────────────────────────────────────────────────────┐
*│　描    述：                                                    
*│　作    者：{Author}                                              
*│　版    本：1.0   模板代码自动生成                                              
*│　创建时间：{GeneratorTime}                            
*└──────────────────────────────────────────────────────────────┘
*┌──────────────────────────────────────────────────────────────┐
*│　命名空间: Samples.Models                                  
*│　类    名：demo                                     
*└──────────────────────────────────────────────────────────────┘
*/
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YDal.Models;

namespace Samples.Models
{
    /// <summary>
    /// {Author}
    /// {GeneratorTime}
    /// 
    /// </summary>
    public partial class demo:BaseEntity
    {
        		[Key]
		[Column("Id")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Int32 Id {get;set;}


		[Column("UserName")]
		[Required]
		[MaxLength(100)]
		public String UserName {get;set;}



    }
}