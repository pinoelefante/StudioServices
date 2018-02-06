using System;

namespace StudioServices.Data.Models
{
    public class Model : DataFile
    {
        public string Code {get;set;}
        public string Name {get;set;}
        public double RequestCost {get;set;}
        public double RequestPrintCost {get;set;}
    }
}