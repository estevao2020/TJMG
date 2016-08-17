﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJ.Dominio.Entidades;

namespace TJ.Dados.EntiConfig
{
    public class EstadoConfig : EntityTypeConfiguration<Estado>
    {
        public EstadoConfig()
        {
            HasKey(e => e.Id);

            Property(e => e.Id).HasColumnName("Estado_Id");
            Property(e => e.Nome).HasColumnName("Estado_Nome").IsRequired().HasMaxLength(20);
        }
    }
}