/*
                           _ooOoo_
                          o8888888o
                          88" . "88
                          (| -_- |)
                          O\  =  /O
                       ____/`---'\____
                     .'  \\|     |//  `.
                    /  \\|||  :  |||//  \
                   /  _||||| -:- |||||-  \
                   |   | \\\  -  /// |   |
                   | \_|  ''\---/''  |   |
                   \  .-\__  `-`  ___/-. /
                 ___`. .'  /--.--\  `. . __
              ."" '<  `.___\_<|>_/___.'  >'"".
             | | :  `- \`.;`\ _ /`;.`/ - ` : | |
             \  \ `-.   \_ __\ /__ _/   .-` /  /
        ======`-.____`-.___\_____/___.-`____.-'======
                           `=---='
        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                 佛祖保佑       永无BUG
*/
/**
*┌──────────────────────────────────────────────────────────────┐
*│　描    述：Ordermaster                                                    
*│　作    者：https://github.com/KenneyChen                                              
*│　版    本：1.0   模板代码自动生成                                              
*│　创建时间：2021-01-02 23:08:32                            
*└──────────────────────────────────────────────────────────────┘
*┌──────────────────────────────────────────────────────────────┐
*│　命名空间: NSql2Table.Models                                  
*│　类    名：Ordermaster                                     
*└──────────────────────────────────────────────────────────────┘
*/
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YDal.Models;

namespace NSql2Table.Models
{
    /// <summary>
    /// https://github.com/KenneyChen
    /// 2021-01-02 23:08:32
    /// Ordermaster
    /// </summary>
    [Table("ordermaster")]
    public partial class Ordermaster:BaseEntity
    {
        [Key]
[Column("orderid")]
[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
public Int32 Id {get;set;}


[Column("paytime")]
[Required]
public DateTime paytime {get;set;}


[Column("totalprice")]
[Required]
public Decimal totalprice {get;set;}


[Column("username")]
[Required]
[MaxLength(255)]
public String username {get;set;}


[Column("userphone")]
[Required]
[MaxLength(255)]
public String userphone {get;set;}



    }
}