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
        public NoSqlObject(dynamic input) 
        {
            try
            {
                if (input.id != null) id = input.id;
                if (input._rid != null) _rid = input._rid;
                if (input._self != null) _self = input._self;
                if (input._etag != null) _etag = input._etag;
                if (input._attachments != null) _attachments = input._attachments;
                if (input._rid != null) id = input._rid;
            } catch (Exception) { } // We don't care if it errors out
        }
    }
    public class Widget : NoSqlObject
    {
        public Widget(dynamic input) : base((object)input)
        {
            try
            {
                if (input.sn != null) sn = (string)input.sn;
                if (input.name != null) name = (string)input.name;
                if (input.size != null) size = (int)input.size;
                if (input.features != null) features = (List<string>)input.features.ToObject<List<string>>();
                if (input.modules != null)
                {
                    modules = new List<Module>();
                    foreach (var item in input.modules)
                    {
                        modules.Add(item.ToObject<Module>());
                    }
                }
            } catch (Exception){ } // We don't care if it errors out
        }

        [Required, NotNull]
        public string sn { get; set; }
        public string name { get; set; }
        public int? size { get; set; }
        public List<string> features { get; set; }
        public List<Module> modules { get; set; }
        public bool Matches(Widget search, bool partial = false)
        {
            if (id != null && id == search.id) return true;
            if (this == search) return true;
            if (partial)
            {
                if (sn != null && search.sn != null && sn.Contains(search.sn)) return true;
                if (name != null && search.name != null && name.Contains(search.name)) return true;
                if (size != null && search.size != null && size.Equals(search.size)) return true;
                if (features != null && search.features != null && features.Where(m=> search.features.Where(n => m.Contains(n)).Any()).Any()) return true;
                if (modules != null && search.modules != null && modules.Where(m => search.modules.Where(n => n.name != null && m.name.Contains(n.name)).Any()).Any()) return true;
                if (modules != null && search.modules != null && modules.Where(m => search.modules.Where(n => n.size != null && m.size.Equals(n.size)).Any()).Any()) return true;
            }
            else
            {
                
                if (sn != null && sn == search.sn) return true;
                if (name != null && name == search.name) return true;
                if (modules != null && modules == search.modules) return true;
                if (features != null && features == search.features) return true;
                if (size != null && size == search.size) return true;
                if (features != null && search.features != null && features.Intersect(search.features).Any()) return true;
                if (modules != null && search.modules != null && modules.Intersect(search.modules).Any()) return true;
            }
            return false;
        }

    }
    public class Module 
    {
        public string name { get;set; }
        public int? size { get; set; }
    }

}
