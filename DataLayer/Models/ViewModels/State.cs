using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.ViewModels
{
    public class SelectedAttributes
    {
        public int?[] attr { get; set; }
    }

    public class selectedAttribute
    {
        public int itemId { get; set; }
        public List<long> attrs { get; set; }
    }
    public class State
    {
        public int? skip { get; set; } = 0;
        public int? take { get; set; } = 0;
        public long? total { get; set; } = 0;
        public Sort[] sort { get; set; }
        public object[] group { get; set; }
        public CompositeFilterDescriptor filter { get; set; }
        public int? type { get; set; }
        public int? requestFilterType { get; set; } = 0;
        public int? contractFilterType { get; set; } = 0;

        public long categoryId { get; set; } = 0;
        public int? level { get; set; } = 0;
        public string query { get; set; }
        public string q { get; set; }
        public string extant { get; set; }
        public int? source { get; set; }
        public long? from { get; set; }
        public long? to { get; set; }

        public List<selectedAttribute> attr { get; set; }
    }

    public struct _State
    {
        /**
     * The number of records to be skipped by the pager.
     */
        int? skip => 0;
        /**
         * The number of records to take.
         */
        int? take => 0;
        /**
         * The descriptors used for sorting.
         */
        Sort[] sort => null;
        /**
         * The descriptors used for filtering.
         */
        CompositeFilterDescriptor filter => new CompositeFilterDescriptor();
        /**
         * The descriptors used for grouping.
         */
        List<GroupDescriptor> group => null;
    }
    public class Sort
    {
        public string field { get; set; } = "";
        public string dir { get; set; } = "asc"; // | "desc";
        public bool checkDesc { get; set; }
    }
    public class CompositeFilter
    {
        public List<CompositeFilterDescriptor> filters { get; set; } = new List<CompositeFilterDescriptor>();
        public string logic { get; set; } = "and"; // | "or"        
    }
    public class CompositeFilterDescriptor
    {
        public List<FilterDescriptor> filters { get; set; } = new List<FilterDescriptor>();
        public string logic { get; set; } = "and"; // | "or"        
    }
    public class FilterDescriptor
    {
        public string field { get; set; }
        public string @operator { get; set; } = "contains";
        public object value { get; set; }
        public bool ignoreCase { get; set; } = false;
    }
    public class GroupDescriptor
    {
        public string field { get; set; }
        public string dir { get; set; } = "asc"; // | "desc";
        public object aggregates { get; set; }
    }
    public class AggregateDescriptor
    {
        public string field { get; set; }
        public object aggregate { get; set; } = "count"; // | "sum" | "average" | "min" | "max";
    }

    public class StateValidator : AbstractValidator<State>
    {
       
    }
}
