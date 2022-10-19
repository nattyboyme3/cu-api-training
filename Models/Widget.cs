using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace CuApiTraining.Models
{
    public class NoSqlObject
    {
        [Required, NotNull]
        internal string id { get; set; }
        protected string _rid { get; set; }
        protected string _self { get; set; }
        protected string _etag { get; set; }
        protected string _attachments { get; set; }
        protected double _ts { get; set; }
    }
    public class Widget : NoSqlObject
    {
        [Required, NotNull]
        public string sn { get; set; }
        public string name { get; set; }
        public int size { get; set; }
        public List<string> features { get; set; }
        public List<Module> modules { get; set; }

    }
    public class Module 
    {
        public string name { get;set; }
        public int size { get; set; }
    }
}
