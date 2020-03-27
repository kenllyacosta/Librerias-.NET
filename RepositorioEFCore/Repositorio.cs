using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace RepositorioEFCore
{
    //Crear delegado para el manejo de las exceptions
    public delegate void ExceptionEventHandler(object sender, ExceptionEvenArgs e);

    public class Repositorio : IDisposable
    {
        /// <summary>
        /// Evento para manejo de las excepciones lanzadas desde el repositorio genérico
        /// </summary>
        public event ExceptionEventHandler Excepcion;
        internal DbContext Contexto = null;
        readonly bool IsUnitOfWork = false;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio con el contexto de datos a utilizar
        /// </summary>
        /// <param name="contexto">
        /// Contexto de los datos
        /// </param>
        /// <param name="isUnitOfWork">
        /// Indica si se usará el contexto como unidad de trabajo, esto significa que se usará como las transacciones, Ejemplo:
        /// Relizar cambios y al final invocar el método SaveChanges del repositorio, no del contexto.
        /// </param>
        /// <param name="lazyLoadingEnabled"></param>
        /// <param name="proxyCreationEnabled"></param>
        public Repositorio(DbContext contexto, bool isUnitOfWork, bool lazyLoadingEnabled = false, bool proxyCreationEnabled = false)
        {
            this.Contexto = contexto;
            contexto.ChangeTracker.LazyLoadingEnabled = lazyLoadingEnabled;            
            Contexto.ChangeTracker.AutoDetectChangesEnabled = proxyCreationEnabled;
            this.IsUnitOfWork = isUnitOfWork;
        }

        public TEntity Create<TEntity>(TEntity toCreate) where TEntity : class
        {
            TEntity Result = null;
            try
            {
                Result = Contexto.Set<TEntity>().Add(toCreate).Entity;
                Save();
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public IEnumerable<TEntity> Create<TEntity>(IEnumerable<TEntity> toCreate) where TEntity : class
        {
            IEnumerable<TEntity> Result = null;
            try
            {
                Contexto.Set<TEntity>().AddRange(toCreate);
                Save();
                Result = toCreate;
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public bool Delete<TEntity>(TEntity toDelete) where TEntity : class
        {
            bool Result = false;
            try
            {
                Contexto.Set<TEntity>().Attach(toDelete);
                Contexto.Set<TEntity>().Remove(toDelete);
                Result = Save() > 0;
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public bool Delete<TEntity>(IEnumerable<TEntity> toDelete) where TEntity : class
        {
            bool Result = false;

            try
            {
                Contexto.Set<TEntity>().RemoveRange(toDelete);
                Result = Save() > 0;

            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { InnerException = ex.InnerException, Message = ex.Message, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public bool Update<TEntity>(TEntity toUpdate) where TEntity : class
        {
            bool Result = false;
            try
            {
                Contexto.Set<TEntity>().Attach(toUpdate);
                Contexto.Entry<TEntity>(toUpdate).State = EntityState.Modified;
                Result = Save() > 0;
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public bool Update<TEntity>(Expression<Func<TEntity, bool>> criterio, string propertyName, object valor) where TEntity : class
        {
            bool Result = false;
            try
            {
                Contexto.Entry<TEntity>(Contexto.Set<TEntity>().FirstOrDefault(criterio)).Property(propertyName).CurrentValue = valor;
                Result = Save() > 0;
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public TEntity Retrieve<TEntity>(Expression<Func<TEntity, bool>> criterio) where TEntity : class
        {
            TEntity Result = null;
            try
            {
                Result = Contexto.Set<TEntity>().FirstOrDefault(criterio);
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public TEntity Retrieve<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1) where TEntity : class
        {
            TEntity Result = null;
            try
            {
                Result = Contexto.Set<TEntity>().Include(include1).FirstOrDefault(criterio);
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public TEntity Retrieve<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2) where TEntity : class
        {
            TEntity Result = null;
            try
            {
                Result = Contexto.Set<TEntity>().Include(include1).Include(include2).FirstOrDefault(criterio);
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public TEntity Retrieve<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3) where TEntity : class
        {
            TEntity Result = null;
            try
            {
                Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).FirstOrDefault(criterio);
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public TEntity Retrieve<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4) where TEntity : class
        {
            TEntity Result = null;
            try
            {
                Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).FirstOrDefault(criterio);
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public TEntity Retrieve<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5) where TEntity : class
        {
            TEntity Result = null;
            try
            {
                Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).FirstOrDefault(criterio);
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public TEntity Retrieve<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6) where TEntity : class
        {
            TEntity Result = null;
            try
            {
                Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).FirstOrDefault(criterio);
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public TEntity Retrieve<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7) where TEntity : class
        {
            TEntity Result = null;
            try
            {
                Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).FirstOrDefault(criterio);
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public TEntity Retrieve<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8) where TEntity : class
        {
            TEntity Result = null;
            try
            {
                Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).FirstOrDefault(criterio);
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public TEntity Retrieve<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9) where TEntity : class
        {
            TEntity Result = null;
            try
            {
                Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Include(include9).FirstOrDefault(criterio);
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public TEntity Retrieve<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10) where TEntity : class
        {
            TEntity Result = null;
            try
            {
                Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Include(include9).Include(include10).FirstOrDefault(criterio);
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public List<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criterio, bool asNoTrack = false) where TEntity : class
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = Contexto.Set<TEntity>().AsNoTracking().Where(criterio).ToList();
                else
                    Result = Contexto.Set<TEntity>().Where(criterio).ToList();
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public List<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, bool asNoTrack = false) where TEntity : class
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = Contexto.Set<TEntity>().AsNoTracking().Include(include1).Where(criterio).ToList();
                else
                    Result = Contexto.Set<TEntity>().Include(include1).Where(criterio).ToList();
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public List<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, bool asNoTrack = false) where TEntity : class
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = Contexto.Set<TEntity>().AsNoTracking().Include(include1).Include(include2).Where(criterio).ToList();
                else
                    Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Where(criterio).ToList();
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public List<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, bool asNoTrack = false) where TEntity : class
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = Contexto.Set<TEntity>().AsNoTracking().Include(include1).Include(include2).Include(include3).Where(criterio).ToList();
                else
                    Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Where(criterio).ToList();
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public List<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, bool asNoTrack = false) where TEntity : class
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = Contexto.Set<TEntity>().AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Where(criterio).ToList();
                else
                    Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Where(criterio).ToList();
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public List<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, bool asNoTrack = false) where TEntity : class
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = Contexto.Set<TEntity>().AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Where(criterio).ToList();
                else
                    Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Where(criterio).ToList();
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public List<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, bool asNoTrack = false) where TEntity : class
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = Contexto.Set<TEntity>().AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Where(criterio).ToList();
                else
                    Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Where(criterio).ToList();
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public List<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, bool asNoTrack = false) where TEntity : class
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = Contexto.Set<TEntity>().AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Where(criterio).ToList();
                else
                    Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Where(criterio).ToList();
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public List<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, bool asNoTrack = false) where TEntity : class
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = Contexto.Set<TEntity>().AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Where(criterio).ToList();
                else
                    Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Where(criterio).ToList();
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public List<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, bool asNoTrack = false) where TEntity : class
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = Contexto.Set<TEntity>().AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Include(include9).Where(criterio).ToList();
                else
                    Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Include(include9).Where(criterio).ToList();
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public List<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10, bool asNoTrack = false) where TEntity : class
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = Contexto.Set<TEntity>().AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include10).Where(criterio).ToList();
                else
                    Result = Contexto.Set<TEntity>().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Include(include9).Include(include10).Where(criterio).ToList();
            }
            catch (DbUpdateException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink, Entries = ex.Entries });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, HelpLink = ex.HelpLink });
            }
            return Result;
        }

        public void Dispose()
        {
            if (Contexto != null)
                Contexto.Dispose();
        }

        /// <summary>
        /// Método que se debe usar solo cuando el contexto está como IsUnitOfWork = true
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return Contexto.SaveChanges();
        }

        /// <summary>
        /// Método usado cuando el contexto está como IsUnitOfWork = false
        /// </summary>
        /// <returns></returns>
        private int Save()
        {
            if (!IsUnitOfWork)
                return Contexto.SaveChanges();
            else
                return 0;
        }
    }
    
    public class ExceptionEvenArgs : EventArgs
    {
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public MethodBase TargetSite { get; set; }
        public Exception InnerException { get; set; }
        public string HelpLink { get; set; }        
        public IReadOnlyList<EntityEntry> Entries { get; set; }
    }
}
