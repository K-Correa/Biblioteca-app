﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibreriaControlMVC.Models
{
    public class LibroAutor
    {
        public int Id { get; set; }

        public int AutorId { get; set; }
        public Autor Autor { get; set; }

        public int LibroId { get; set; }
        public Libro Libro { get; set; }
    }
}
