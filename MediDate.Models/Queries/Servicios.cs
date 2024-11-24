using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Servicios: BaseQuery
    {
        public Servicios() : base(){}

        /// <summary>
        /// Mostrar todos los Servicios de la tabla Servicios
        /// </summary>
        /// <returns>Lista de Servicios</returns>
        public List<Servicio> GetAll()
        {
            var Servicios = new List<Servicio>();

            using(var db = GetConnection())
            {
                Servicios = db.Query<Servicio>("sp_Servicios 5,'',''").ToList();
            }

            return Servicios;
        }

        public List<Servicio> GetAllIdServicioDesc(string Descripcion)
        {
            var Servicios = new List<Servicio>();

            using (var db = GetConnection())
            {
                Servicios = db.Query<Servicio>("sp_Servicios 7,'',@Descripcion", new {Descripcion}).ToList();
            }

            return Servicios;
        }

        public Servicio GetById(int IdServicio)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Servicio>("sp_Servicios 4,@IdServicio,''", new { IdServicio });
            }
        }

        /// <summary>
        /// Busca todos los Servicios que coincidan con el parametro Descripcion
        /// </summary>
        /// <param name="Descripcion"></param>
        /// <returns>Lista de Servicios</returns>
        public List<Servicio> GetDescripciones(string Descripcion)
        {
            var Servicios = new List<Servicio>();

            using (var db = GetConnection())
            {
                Servicios = db.Query<Servicio>("sp_Servicios 6,'',@Descripcion", new {Descripcion}).ToList();
            }

            return Servicios;
        }

        public BaseResult Edit(Servicio servicio)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_Servicios 3,@IdServicio,@Descripcion",
                    new { servicio.IdServicio, servicio.Descripcion });
            }
        }
    }
}
