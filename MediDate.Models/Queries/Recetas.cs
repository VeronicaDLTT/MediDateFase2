using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Recetas : BaseQuery
    {
        public Recetas() : base() { }

        public BaseResult Create(Receta receta, String Nombre, byte[] ArchivoBytes, String Extension)
        {
            float PesoFloat = float.Parse(receta.Peso);
            float TallaFloat = float.Parse(receta.Talla);
            float TemperaturaFloat = float.Parse(receta.Temperatura);

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_recetas 1,'',@IdCita,0,@Nombre,@ArchivoBytes,@Extension,@Fecha,@Hora,@PesoFloat,@TallaFloat,@TemperaturaFloat,@TA,@FC,@FR,@Saturacion,@Alergias,@Diagnostico,@Tratamiento,@Observaciones",
                    new {receta.IdCita, Nombre, ArchivoBytes, Extension, receta.Fecha, receta.Hora, PesoFloat, TallaFloat, TemperaturaFloat, receta.TA, receta.FC, receta.FR, receta.Saturacion, receta.Alergias, receta.Diagnostico, receta.Tratamiento, receta.Observaciones });
            }
        }

        public BaseResult Delete(int IdReceta)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_recetas 2,@IdReceta",
                    new { IdReceta });
            }
        }

        public BaseResult Edit(Receta receta)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_recetas 3,@IdReceta,'','','','','','',@Peso,@Talla,@Temperatura,@TA,@FC,@FR,@Saturacion,@Alergias,@Diagnostico,@Tratamiento,@Observaciones",
                    new { receta.IdReceta,receta.Peso, receta.Talla, receta.Temperatura, receta.TA, receta.FC, receta.FR, receta.Saturacion, receta.Alergias, receta.Diagnostico, receta.Tratamiento, receta.Observaciones });
            }
        }

        /// <summary>
        /// Busca una Receta por IdReceta
        /// </summary>
        /// <param name="IdReceta"></param>
        /// <returns>Registro de Receta que conincida con IdReceta</returns>
        public Receta GetById(int IdReceta)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Receta>("sp_recetas 4,@IdReceta,''", new { IdReceta });
            }
        }

        /// <summary>
        /// Mostrar todos los Recetas de la tabla Recetas
        /// </summary>
        /// <returns>Lista de Recetas</returns>
        public List<Receta> GetAll()
        {
            var Recetas = new List<Receta>();

            using (var db = GetConnection())
            {
                Recetas = db.Query<Receta>("sp_recetas 5,'','','',''").ToList();
            }

            return Recetas;
        }

        public Receta GetByIdCita(int IdCita)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Receta>("sp_recetas 6,'',@IdCita", new { IdCita });
            }
        }
    }
}
