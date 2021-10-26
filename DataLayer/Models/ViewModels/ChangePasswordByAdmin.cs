using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

namespace Datalayer.ViewModels
{
    public class ChangePasswordByAdmin
    {
        public string Password { get; set; }

        public int Id { get; set; }
    }
}
