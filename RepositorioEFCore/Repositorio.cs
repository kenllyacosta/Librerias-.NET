using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace RepositorioEFCore
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
        readonly DbContext Contexto = null;
        private DbSet<TEntity> EntitySet { get { return Contexto.Set<TEntity>(); } }

        public TEntity Create(TEntity toCreate)
        {
            TEntity Result = null;
            try
            {
                Result = EntitySet.Add(toCreate).Entity;
                Contexto.SaveChanges();
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
                EntitySet.AddRange(toCreate);
                Contexto.SaveChanges();
                Result = toCreate;
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
    }
}
