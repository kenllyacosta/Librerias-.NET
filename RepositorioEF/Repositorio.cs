using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepositorioEF
{
    internal interface IRepositorio<TEntity> : IDisposable where TEntity : class
    {
        //Operaciones que expondrá la interface
        TEntity Create(TEntity toCreate);
        TEntity Retrieve(Expression<Func<TEntity, bool>> criterio);
        bool Update(TEntity toUpdate);
        bool Delete(TEntity toDelete);
        List<TEntity> Filter(Expression<Func<TEntity, bool>> criterio, bool asNoTrack);
    }

    //Crear delegado para el manejo de las exceptions
    public delegate void ExceptionEventHandler(object sender, ExceptionEvenArgs e);

    public class Repositorio<TEntity> : IRepositorio<TEntity> where TEntity : class
    {
        /// <summary>
        /// Evento para manejo de las excepciones lanzadas desde el repositorio genérico
        /// </summary>
        public event ExceptionEventHandler Excepcion;
        DbContext Contexto = null;

        /// <summary>
        /// Repositorio genérico con las operaciones básicas
        /// </summary>
        /// <param name="contexto">
        /// Recibe el contexto de la base de datos actual
        /// </param>
        /// <param name="lazyLoadingEnabled">
        /// Obtiene o establece un valor booleano que determina si los objetos relacionados se cargan automáticamente cuando se tiene acceso a una propiedad de navegación.
        /// </param>
        /// <param name="proxyCreationEnabled">
        /// Obtiene o establece un valor que indica si el marco creará o no instancias de clases proxy generadas dinámicamente siempre que cree una instancia de un tipo de entidad.Tenga en cuenta que incluso si la creación de proxy está habilitada con este indicador, las instancias de proxy solo se crearán para los tipos de entidad que cumplan con los requisitos para ser procesados.La creación de proxy está habilitada por defecto.
        /// </param>
        public Repositorio(DbContext contexto, bool lazyLoadingEnabled = false, bool proxyCreationEnabled = false)
        {
            this.Contexto = contexto;
            Contexto.Configuration.LazyLoadingEnabled = lazyLoadingEnabled;
            Contexto.Configuration.ProxyCreationEnabled = proxyCreationEnabled;
        }

        private DbSet<TEntity> EntitySet { get { return Contexto.Set<TEntity>(); } }

        public TEntity Create(TEntity toCreate)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.Add(toCreate);
                Contexto.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public IEnumerable<TEntity> Create(IEnumerable<TEntity> toCreate)
        {
            IEnumerable<TEntity> Result = null;
            try
            {
                Result = EntitySet.AddRange(toCreate);
                Contexto.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public bool Delete(TEntity toDelete)
        {
            bool Result = false;
            try
            {
                EntitySet.Attach(toDelete);
                EntitySet.Remove(toDelete);
                Result = Contexto.SaveChanges() > 0;
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public bool Delete(IEnumerable<TEntity> toDelete)
        {
            bool Result = false;

            try
            {
                EntitySet.RemoveRange(toDelete);
                Result = Contexto.SaveChanges() > 0;

            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { InnerException = ex.InnerException, Message = ex.Message, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { InnerException = ex.InnerException, Message = ex.Message, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public bool Update(TEntity toUpdate)
        {
            bool Result = false;
            try
            {
                EntitySet.Attach(toUpdate);
                Contexto.Entry<TEntity>(toUpdate).State = EntityState.Modified;
                Result = Contexto.SaveChanges() > 0;
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public bool Update(Expression<Func<TEntity, bool>> criterio, string propertyName, object valor)
        {
            bool Result = false;
            try
            {
                Contexto.Entry<TEntity>(EntitySet.FirstOrDefault(criterio)).Property(propertyName).CurrentValue = valor;
                Result = Contexto.SaveChanges() > 0;
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public TEntity Retrieve(Expression<Func<TEntity, bool>> criterio)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.FirstOrDefault(criterio);
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public TEntity Retrieve(Expression<Func<TEntity, bool>> criterio, string include1)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.Include(include1).FirstOrDefault(criterio);
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public TEntity Retrieve(Expression<Func<TEntity, bool>> criterio, string include1, string include2)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.Include(include1).Include(include2).FirstOrDefault(criterio);
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public TEntity Retrieve(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.Include(include1).Include(include2).Include(include3).FirstOrDefault(criterio);
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public TEntity Retrieve(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).FirstOrDefault(criterio);
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public TEntity Retrieve(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).FirstOrDefault(criterio);
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public TEntity Retrieve(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).FirstOrDefault(criterio);
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public TEntity Retrieve(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).FirstOrDefault(criterio);
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public TEntity Retrieve(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).FirstOrDefault(criterio);
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public TEntity Retrieve(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Include(include9).FirstOrDefault(criterio);
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public TEntity Retrieve(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Include(include9).Include(include10).FirstOrDefault(criterio);
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public List<TEntity> Filter(Expression<Func<TEntity, bool>> criterio, bool asNoTrack = false)
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = EntitySet.AsNoTracking().Where(criterio).ToList();
                else
                    Result = EntitySet.Where(criterio).ToList();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public List<TEntity> Filter(Expression<Func<TEntity, bool>> criterio, string include1, bool asNoTrack = false)
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = EntitySet.AsNoTracking().Include(include1).Where(criterio).ToList();
                else
                    Result = EntitySet.Include(include1).Where(criterio).ToList();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public List<TEntity> Filter(Expression<Func<TEntity, bool>> criterio, string include1, string include2, bool asNoTrack = false)
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = EntitySet.AsNoTracking().Include(include1).Include(include2).Where(criterio).ToList();
                else
                    Result = EntitySet.Include(include1).Include(include2).Where(criterio).ToList();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public List<TEntity> Filter(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, bool asNoTrack = false)
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = EntitySet.AsNoTracking().Include(include1).Include(include2).Include(include3).Where(criterio).ToList();
                else
                    Result = EntitySet.Include(include1).Include(include2).Include(include3).Where(criterio).ToList();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public List<TEntity> Filter(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, bool asNoTrack = false)
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = EntitySet.AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Where(criterio).ToList();
                else
                    Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Where(criterio).ToList();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public List<TEntity> Filter(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, bool asNoTrack = false)
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = EntitySet.AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Where(criterio).ToList();
                else
                    Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Where(criterio).ToList();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public List<TEntity> Filter(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, bool asNoTrack = false)
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = EntitySet.AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Where(criterio).ToList();
                else
                    Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Where(criterio).ToList();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public List<TEntity> Filter(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, bool asNoTrack = false)
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = EntitySet.AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Where(criterio).ToList();
                else
                    Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Where(criterio).ToList();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public List<TEntity> Filter(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, bool asNoTrack = false)
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = EntitySet.AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Where(criterio).ToList();
                else
                    Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Where(criterio).ToList();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public List<TEntity> Filter(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, bool asNoTrack = false)
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = EntitySet.AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Include(include9).Where(criterio).ToList();
                else
                    Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Include(include9).Where(criterio).ToList();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public List<TEntity> Filter(Expression<Func<TEntity, bool>> criterio, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10, bool asNoTrack = false)
        {
            List<TEntity> Result = null;
            try
            {
                if (asNoTrack)
                    Result = EntitySet.AsNoTracking().Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include10).Where(criterio).ToList();
                else
                    Result = EntitySet.Include(include1).Include(include2).Include(include3).Include(include4).Include(include5).Include(include6).Include(include7).Include(include8).Include(include9).Include(include10).Where(criterio).ToList();
            }
            catch (DbEntityValidationException ex)
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite, EntityValidationErrors = ex.EntityValidationErrors });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(this, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return Result;
        }

        public void Dispose()
        {
            if (Contexto != null)
                Contexto.Dispose();
        }
    }

    public class ExceptionEvenArgs : EventArgs
    {
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public MethodBase TargetSite { get; set; }
        public Exception InnerException { get; set; }
        public IEnumerable<DbEntityValidationResult> EntityValidationErrors { get; set; }
    }

    public class Repositorio
    {
        /// <summary>
        /// Evento para manejo de las excepciones lanzadas desde el repositorio
        /// </summary>
        public static event ExceptionEventHandler Excepcion;

        /// <summary>
        /// Método para realizar una consulta directa mediante T-SQL
        /// </summary>
        /// <param name="query">
        /// T-SQL a ejecutar
        /// </param>
        /// <param name="connectionString">
        /// Cadena de conexión que se usará para realizar la consulta
        /// </param>
        /// <param name="commandTimeout">
        /// Tiempo de ejecución del comando
        /// </param>
        /// <returns>
        /// DataTable con todas las filas retornadas en caso hayan
        /// </returns>
        public static DataTable QuerySQL(string query, string connectionString, int commandTimeout = 15, CommandType commandType = CommandType.Text, IEnumerable<SqlParametro> Parameters = null)
        {
            DataTable resultado = new DataTable();
            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, cnn);

                    #region Para uso de los parámetros (opcionales)
                    //Add Parameters
                    if (Parameters != null)
                    {
                        foreach (var kvp in Parameters)
                        {
                            SqlParameter parameter = adapter.SelectCommand.CreateParameter();
                            parameter.SqlDbType = kvp.DbType;
                            adapter.SelectCommand.Parameters.AddWithValue(kvp.Name, kvp.Value);
                        }
                    }
                    #endregion

                    cnn.Open();
                    adapter.SelectCommand.CommandType = commandType;
                    adapter.SelectCommand.CommandTimeout = commandTimeout;
                    adapter.Fill(resultado);
                }
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(new { }, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return resultado;
        }

        /// <summary>
        /// Método para realizar una consulta directa mediante T-SQL
        /// </summary>
        /// <param name="query">
        /// T-SQL a ejecutar
        /// </param>
        /// <param name="connectionString">
        /// Cadena de conexión que se usará para realizar la consulta
        /// <param name="commandTimeout">
        /// Tiempo de ejecución del comando
        /// </param>
        /// </param>
        /// <returns>
        /// DataTable con todas las filas retornadas en caso hayan
        /// </returns>
        public static DataTable QueryMySQL(string query, string connectionString, int commandTimeout = 15, CommandType commandType = CommandType.Text, IEnumerable<MySqlParametro> Parameters = null)
        {
            DataTable resultado = new DataTable();
            try
            {
                using (MySqlConnection cnn = new MySqlConnection(connectionString))
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, cnn);

                    #region Para uso de los parámetros (opcionales)
                    //Add Parameters
                    if (Parameters != null)
                    {
                        foreach (var kvp in Parameters)
                        {
                            MySqlParameter parameter = adapter.SelectCommand.CreateParameter();
                            parameter.MySqlDbType = kvp.DbType;
                            adapter.SelectCommand.Parameters.AddWithValue(kvp.Name, kvp.Value);
                        }
                    }
                    #endregion

                    cnn.Open();
                    adapter.SelectCommand.CommandTimeout = commandTimeout;
                    adapter.SelectCommand.CommandType = commandType;
                    adapter.Fill(resultado);
                }
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(new { }, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return resultado;
        }

        /// <summary>
        /// Método para realizar una consulta directa mediante T-SQL
        /// 
        /// Más información: 
        /// https://gist.github.com/kschieck/11290305
        /// </summary>
        /// <param name="query">
        /// T-SQL a ejecutar
        /// </param>
        /// <param name="connectionString">
        /// Cadena de conexión que se usará para realizar la consulta
        /// </param>
        /// <param name="providerName">
        /// Proveedor a ser usado para la conexión, por ejemplo:
        /// System.Data.SqlClient        
        /// System.Data.OracleClient
        /// System.Data.Odbc
        /// System.Data.OleDb
        /// System.Data.SqlServerCe.4.0
        /// MySql.Data.MySqlClient
        /// ...
        /// </param>
        /// <param name="commandTimeout">
        /// Tiempo de ejecución del comando
        /// </param>
        /// <param name="commandType">
        /// El tipo de comando que quieres ejecutar, por ejemplo:
        /// Text
        /// TableDirect
        /// StoredProcedure
        /// </param>
        /// <param name="Parameters">
        /// Parámetros que serán enviados en el comando
        /// </param>
        /// <returns></returns>
        public static DataTable QueryCommonSQL(string query, string connectionString, string providerName, int commandTimeout = 15, CommandType commandType = CommandType.Text, IEnumerable<CommonSqlParametro> Parameters = null)
        {
            DataTable resultado = new DataTable();
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);

                using (DbConnection conn = factory.CreateConnection())
                {
                    conn.ConnectionString = connectionString;
                    using (DbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = commandType;
                        cmd.CommandTimeout = commandTimeout;

                        using (DbDataAdapter da = factory.CreateDataAdapter())
                        {
                            cmd.CommandText = query;
                            cmd.CommandType = commandType;
                            da.SelectCommand = cmd;

                            #region Para uso de los parámetros (opcionales)
                            //Add Parameters
                            if (Parameters != null)
                            {
                                foreach (var kvp in Parameters)
                                {
                                    DbParameter parameter = cmd.CreateParameter();
                                    parameter.ParameterName = kvp.Name;
                                    parameter.Value = kvp.Value;
                                    parameter.DbType = kvp.DbType;
                                    cmd.Parameters.Add(parameter);
                                }
                            }
                            #endregion

                            conn.Open();
                            da.Fill(resultado);
                        }
                    }
                }
            }
            catch (DbException ex)
            {
                Excepcion?.Invoke(new { }, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            catch (Exception ex)
            {
                Excepcion?.Invoke(new { }, new ExceptionEvenArgs() { Message = ex.Message, InnerException = ex.InnerException, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
            }
            return resultado;
        }

        public static List<T> MakeEntityFromDataTable<T>(DataTable data)
        {
            Type objType = typeof(T);
            List<T> collection = new List<T>();
            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow row in data.Rows)
                {
                    // create an instance of our object
                    T item = (T)Activator.CreateInstance(objType);

                    // get our object type's properties
                    PropertyInfo[] properties = objType.GetProperties();

                    // set the object's properties as they are found.
                    foreach (PropertyInfo property in properties)
                    {
                        if (data.Columns.Contains(property.Name))
                        {
                            Type pType = property.PropertyType;
                            var defaultValue = pType.GetDefaultValue();
                            var value = row[property.Name];
                            value = ConvertFromDBValue(value, defaultValue);
                            try
                            {
                                property.SetValue(item, value);
                            }
                            catch { }
                        }
                    }
                    collection.Add(item);
                }
            }
            return collection;
        }

        /// <summary>
        /// Converts a value from its database value to something we can use (we need this because 
        /// we're using reflection to populate our entities)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <paramname="obj"></param>
        /// <paramname="defaultValue"></param>
        /// <returns></returns>
        protected static T ConvertFromDBValue<T>(object obj, T defaultValue)
        {
            T result = (obj == null || obj == DBNull.Value) ? defaultValue : (T)obj;
            return result;
        }

        //
        // Summary:
        //     Returns a System.Data.DataTable that contains information about all installed
        //     providers that implement System.Data.Common.DbProviderFactory.
        //
        // Returns:
        //     Returns a System.Data.DataTable containing System.Data.DataRow objects that contain
        //     the following data. Column ordinalColumn nameDescription0 Name Human-readable
        //     name for the data provider.1 Description Human-readable description of the data
        //     provider.2 InvariantName Name that can be used programmatically to refer to the
        //     data provider.3 AssemblyQualifiedName Fully qualified name of the factory class,
        //     which contains enough information to instantiate the object.
        public static DataTable GetProviderFactoryClasses()
        {
            // Retrieve the installed providers and factories.
            DataTable table = DbProviderFactories.GetFactoryClasses();

            // Display each row and column value.
            foreach (DataRow row in table.Rows)
                foreach (DataColumn column in table.Columns)
                    Console.WriteLine(row[column]);

            return table;
        }

        public static string GetConnectionStringByProvider(string providerName)
        {
            // Return null on failure.
            string returnValue = null;

            // Get the collection of connection strings.
            ConnectionStringSettingsCollection settings = ConfigurationManager.ConnectionStrings;

            // Walk through the collection and return the first 
            // connection string matching the providerName.
            if (settings != null)
            {
                foreach (ConnectionStringSettings cs in settings)
                {
                    if (cs.ProviderName == providerName)
                        returnValue = cs.ConnectionString;
                    break;
                }
            }
            return returnValue;
        }
    }

    public class SqlParametro
    {
        /// <summary>
        /// Nombre del parámetro.
        /// ejemplo: @Id, @Nombre ...
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Aquí el valor del parámetro
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Tipo de dato que almacena en la fuente de datos
        /// </summary>
        public SqlDbType DbType { get; set; }
    }

    public class MySqlParametro
    {
        /// <summary>
        /// Nombre del parámetro.
        /// ejemplo: @Id, @Nombre ...
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Aquí el valor del parámetro
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Tipo de dato que almacena en la fuente de datos
        /// </summary>
        public MySqlDbType DbType { get; set; }
    }

    public class CommonSqlParametro
    {
        /// <summary>
        /// Nombre del parámetro.
        /// ejemplo: @Id, @Nombre ...
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Aquí el valor del parámetro
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Tipo de dato que almacena en la fuente de datos
        /// </summary>
        public DbType DbType { get; set; }
    }

    public static class TypeExtension
    {
        //a thread-safe way to hold default instances created at run-time
        private static ConcurrentDictionary<Type, object> typeDefaults = new ConcurrentDictionary<Type, object>();

        public static object GetDefaultValue(this Type type)
        {
            return type.IsValueType ? typeDefaults.GetOrAdd(type, Activator.CreateInstance) : null;
        }
    }
}
