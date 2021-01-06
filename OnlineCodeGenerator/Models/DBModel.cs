using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineCodeGenerator.Models
{
    /// <summary>
    /// 数据库信息
    /// </summary>
    public class DBModel
    {
        private string _codeNameSpace;
        private List<SelectListItem> _tableList;
        private List<SelectListItem> _dbList;
        private string _connStr;

        public DBModel()
        {
            _codeNameSpace = "Models";
            _tableList = new List<SelectListItem>();
            _dbList = new List<SelectListItem>();
        }

        /// <summary>
        /// IP
        /// </summary>
        [Required(ErrorMessage = "请填写IP")]
        public string IP { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "请填写登录用户")]
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Required(ErrorMessage = "请填写登录密码")]
        public string Password { get; set; }

        /// <summary>
        /// 代码命名空间
        /// </summary>
        public string CodeNameSpace { get => _codeNameSpace; set => _codeNameSpace = value; }

        /// <summary>
        /// 数据库集合
        /// </summary>
        public List<SelectListItem> DbList { get => _dbList; set => _dbList = value; }

        /// <summary>
        /// 表集合
        /// </summary>
        public List<SelectListItem> TableList { get => _tableList; set => _tableList = value; }

        /// <summary>
        /// 数据库
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 数据表
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DBType { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName { get; set; }

        public string ConnStr { get => _connStr; set => _connStr = value; }
    }
}
