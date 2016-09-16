﻿using System.Collections.Generic;

namespace TJ.Dominio.Entidades
{
    public class Entidade
    {
        public Entidade()
        {
            this.Enderecos = new List<Endereco>();
        }
        public int Id { get; set; }
        public string Nome { get; set; }
        public virtual  ICollection<Endereco> Enderecos { get; set; }
    }
}
