﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TJ.Dominio.Interfaces.Repositorios
{
    public interface IRepositorioBase<TEntity> : IDisposable where TEntity : class
    {
        void Adiciona(TEntity objeto);
        TEntity RetornaPorId(int Id);
        IEnumerable<TEntity> RetornaTodos();
        IEnumerable<TEntity> RetornaTodosAsNoTracking();
        void Alterar(TEntity objeto);
        void Remover(TEntity objeto);
        void Dispose();
        void ReloadElement(TEntity objeto);
        void Reload(IEnumerable<TEntity> objetos);
    }
}
