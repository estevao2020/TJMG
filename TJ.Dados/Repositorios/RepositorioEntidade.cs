﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TJ.Dados.Contexto;
using TJ.Dominio.Entidades;
using TJ.Dominio.Interfaces.Repositorios;

namespace TJ.Dados.Repositorios
{
    public class RepositorioEntidade : RepositorioBase<Entidade>, IRepositorioEntidade
    {
        public IEnumerable<Entidade> RetornaEntidadesAtivas()
        {
            try
            {
                return db.Entidades.Where(e => e.Ativo.Equals("True", StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                throw new Exception("Problema inesperado ao carregar entidades ativas! " + ex.Message);
            }
        }

        public Entidade RetornaPorId(int Id)
        {
            return db.Entidades.Include(e => e.SentenciadoEntidades).Include(e => e.Bairro).Include(e => e.Cidade).Include(e => e.UsuarioCadastro).Include(e => e.UsuarioAlteracao).FirstOrDefault(e => e.Id == Id);
        }
    }
}
