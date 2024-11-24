using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Archivos : BaseQuery
    {
        public Archivos() : base() { }

        /// <summary>
        /// Mostrar todos los Archivos de la tabla Archivos
        /// </summary>
        /// <returns>Lista de Archivos</returns>
        public List<Archivo> GetAll()
        {
            var Archivos = new List<Archivo>();

            using (var db = GetConnection())
            {
                Archivos = db.Query<Archivo>("sp_archivos 5").ToList();
            }

            return Archivos;
        }

        //public List<Archivo> GetAllNoValidados()
        //{
        //    var Archivos = new List<Archivo>();

        //    using (var db = GetConnection())
        //    {
        //        Archivos = db.Query<Archivo>("sp_archivos 7").ToList();
        //    }

        //    return Archivos;
        //}

        /// <summary>
        /// Busca un Archivo en la tabla Archivos por el IdArchivo
        /// </summary>
        /// <param name="IdArchivo"></param>
        /// <returns>Archivo que coincida con el IdArchivo</returns>
        public Archivo GetById(int IdArchivo)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Archivo>("sp_archivos 4,@IdArchivo", new { IdArchivo });
            }
        }

        /// <summary>
        /// Crea un nuevo Archivo en la tabla Archivos
        /// </summary>
        /// <param name="Archivo"></param>
        /// <returns>Mensaje si se agregó el Archivo o no se pudo agregar</returns>
        //public BaseResult Create(Archivo Archivo)
        //{

        //    using (var db = GetConnection())
        //    {
        //        return db.QueryFirstOrDefault<BaseResult>("sp_Archivos 1,'',@IdUsuario,@IdEspecialidad,@Nombre,@PrimerApellido,@SegundoApellido,@NumCedula,@Telefono",
        //                                                    new { Archivo.IdUsuario, Archivo.IdEspecialidad, Archivo.Nombre, Archivo.PrimerApellido, Archivo.SegundoApellido, Archivo.NumCedula, Archivo.Telefono });
        //    }
        //}

        //public BaseResult Edit(Archivo Archivo)
        //{

        //    using (var db = GetConnection())
        //    {
        //        return db.QueryFirstOrDefault<BaseResult>("sp_Archivos 3,@IdArchivo,'',@IdEspecialidad,@Nombre,@PrimerApellido,@SegundoApellido,@NumCedula,@Telefono,@Estado,@IdArchivo",
        //                                                    new { Archivo.IdArchivo, Archivo.IdEspecialidad, Archivo.Nombre, Archivo.PrimerApellido, Archivo.SegundoApellido, Archivo.NumCedula, Archivo.Telefono, Archivo.Estado, Archivo.IdArchivo });
        //    }
        //}

    }
}
