﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AirBears.Web.ViewModels.Manage
{
    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }
    }
}
