using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSWeb.Models
{
    public class CityModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<District> Districts { get; set; }
    }

    public class District
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Ward> Wards { get; set; }
    }

    public class Ward
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }



}