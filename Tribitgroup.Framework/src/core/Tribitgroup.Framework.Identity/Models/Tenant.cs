﻿using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Models
{
    public class Tenant : Entity {
        public string PathToParent { get; set; } = string.Empty;
        public string ShortKey { get; set; } = string.Empty;
        public string Title{get;set;} = string.Empty;

        public Tenant(string pathToParent, string shortKey, string title)
        {
            PathToParent = pathToParent;
            ShortKey = shortKey;
            Title = title;
        }

        public Tenant(string shortKey, string title) : this("", shortKey, title) { }
    }
}